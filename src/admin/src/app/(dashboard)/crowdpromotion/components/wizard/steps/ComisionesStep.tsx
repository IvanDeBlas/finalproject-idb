"use client"

import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import type { CreatePromoProgramaFormData } from "@shared/schemas/crowdpromotion.schema"

const promoProgramaStep2Schema = z.object({
    monedaId: z.number({ required_error: "La moneda es obligatoria", invalid_type_error: "La moneda es obligatoria" }).int().min(1, "La moneda es obligatoria"),
    importeComisionPorcentaje: z.number().min(0, "No puede ser negativo").max(100, "No puede superar 100%").optional(),
    importeComisionFija: z.number().min(0, "No puede ser negativa").optional(),
}).refine(
    (data) => data.importeComisionPorcentaje != null || data.importeComisionFija != null,
    { message: "Debe definir al menos una comision (porcentaje o fija)", path: ["importeComisionPorcentaje"] }
)

type ComisionesStepData = z.infer<typeof promoProgramaStep2Schema>

interface ComisionesStepProps {
    defaultValues?: Partial<CreatePromoProgramaFormData>
    onNext: (data: Partial<CreatePromoProgramaFormData>) => void
    tienePromotores?: boolean
}

const MONEDAS_MVP = [
    { id: 1, nombre: "EUR" },
    { id: 2, nombre: "USD" },
]

export function ComisionesStep({ defaultValues, onNext, tienePromotores }: ComisionesStepProps) {
    const {
        register,
        handleSubmit,
        setValue,
        watch,
        formState: { errors },
    } = useForm<ComisionesStepData>({
        resolver: zodResolver(promoProgramaStep2Schema),
        defaultValues: {
            monedaId: defaultValues?.monedaId,
            importeComisionPorcentaje: defaultValues?.importeComisionPorcentaje ?? undefined,
            importeComisionFija: defaultValues?.importeComisionFija ?? undefined,
        },
    })

    const monedaId = watch("monedaId")
    const porcentaje = watch("importeComisionPorcentaje")
    const fija = watch("importeComisionFija")

    const monedaNombre = MONEDAS_MVP.find((m) => m.id === monedaId)?.nombre ?? "EUR"

    const onSubmit = (data: ComisionesStepData) => {
        onNext(data as Partial<CreatePromoProgramaFormData>)
    }

    // Preview calculation
    const baseAmount = 50
    const comisionPorcentaje = porcentaje != null ? (baseAmount * porcentaje) / 100 : 0
    const comisionFijaVal = fija ?? 0
    const totalComision = comisionPorcentaje + comisionFijaVal
    const hasPreviw = porcentaje != null || fija != null

    return (
        <form id="paso-2-form" onSubmit={handleSubmit(onSubmit)} className="space-y-6 max-w-2xl mx-auto">
            {tienePromotores && (
                <div className="rounded-lg border border-amber-500/30 bg-amber-500/5 p-3">
                    <p className="text-sm text-amber-400">
                        Los cambios en comisiones se aplicaran a nuevos eventos. Los existentes no se recalculan.
                    </p>
                </div>
            )}

            <Card className="bg-[#1a1a2e] border-zinc-800">
                <CardHeader>
                    <CardTitle className="text-base text-white">Configuracion de comisiones</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                    <div className="space-y-2">
                        <Label>Moneda *</Label>
                        <Select
                            value={monedaId ? String(monedaId) : ""}
                            onValueChange={(val) => setValue("monedaId", Number(val), { shouldValidate: true })}
                        >
                            <SelectTrigger className="bg-[#0d0d1a] border-zinc-700">
                                <SelectValue placeholder="Selecciona moneda" />
                            </SelectTrigger>
                            <SelectContent>
                                {MONEDAS_MVP.map((m) => (
                                    <SelectItem key={m.id} value={String(m.id)}>{m.nombre}</SelectItem>
                                ))}
                            </SelectContent>
                        </Select>
                        {errors.monedaId && <p className="text-xs text-red-400">{errors.monedaId.message}</p>}
                    </div>

                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <div className="space-y-2">
                            <Label htmlFor="importeComisionPorcentaje">Comision porcentaje (%)</Label>
                            <Input
                                id="importeComisionPorcentaje"
                                type="number"
                                step="0.01"
                                min="0"
                                max="100"
                                placeholder="Ej: 10"
                                className="bg-[#0d0d1a] border-zinc-700"
                                {...register("importeComisionPorcentaje", { valueAsNumber: true })}
                            />
                            {errors.importeComisionPorcentaje && (
                                <p className="text-xs text-red-400">{errors.importeComisionPorcentaje.message}</p>
                            )}
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="importeComisionFija">Comision fija ({monedaNombre})</Label>
                            <Input
                                id="importeComisionFija"
                                type="number"
                                step="0.01"
                                min="0"
                                placeholder="Ej: 5.00"
                                className="bg-[#0d0d1a] border-zinc-700"
                                {...register("importeComisionFija", { valueAsNumber: true })}
                            />
                            {errors.importeComisionFija && (
                                <p className="text-xs text-red-400">{errors.importeComisionFija.message}</p>
                            )}
                        </div>
                    </div>

                    <p className="text-xs text-zinc-500">
                        Debe definir al menos una comision (porcentaje, fija, o ambas).
                    </p>
                </CardContent>
            </Card>

            <Card className="bg-[#1a1a2e] border-zinc-800">
                <CardHeader>
                    <CardTitle className="text-base text-white">Vista previa de comision</CardTitle>
                </CardHeader>
                <CardContent>
                    {hasPreviw ? (
                        <div className="text-sm text-zinc-300 space-y-1">
                            <p>Para un backing de <strong>{baseAmount} {monedaNombre}</strong>:</p>
                            {porcentaje != null && porcentaje > 0 && (
                                <p className="text-zinc-400">
                                    Porcentaje ({porcentaje}%): {comisionPorcentaje.toFixed(2)} {monedaNombre}
                                </p>
                            )}
                            {fija != null && fija > 0 && (
                                <p className="text-zinc-400">
                                    Fija: {comisionFijaVal.toFixed(2)} {monedaNombre}
                                </p>
                            )}
                            <p className="text-emerald-400 font-medium pt-1 border-t border-zinc-700">
                                Total comision: {totalComision.toFixed(2)} {monedaNombre}
                            </p>
                        </div>
                    ) : (
                        <p className="text-sm text-zinc-500 italic">
                            Ingresa al menos una comision para ver la vista previa
                        </p>
                    )}
                </CardContent>
            </Card>
        </form>
    )
}
