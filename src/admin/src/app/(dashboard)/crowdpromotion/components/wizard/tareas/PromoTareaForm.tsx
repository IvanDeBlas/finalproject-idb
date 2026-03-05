"use client"

import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Label } from "@/components/ui/label"
import { Button } from "@/components/ui/button"
import { Switch } from "@/components/ui/switch"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { createPromoTareaSchema } from "@shared/schemas/crowdpromotion.schema"
import {
    TIPO_EVENTO_PROMO_LABELS,
    TIPO_REWARD_PROMO_LABELS,
} from "@shared/constants"
import type { CreatePromoTareaItem } from "@shared/types"

interface PromoTareaFormProps {
    defaultValues?: Partial<CreatePromoTareaItem>
    onGuardar: (tarea: CreatePromoTareaItem) => void
    onCancelar: () => void
    titulo?: string
}

export function PromoTareaForm({
    defaultValues,
    onGuardar,
    onCancelar,
    titulo = "Nueva tarea",
}: PromoTareaFormProps) {
    const {
        register,
        handleSubmit,
        setValue,
        watch,
        formState: { errors },
    } = useForm<CreatePromoTareaItem>({
        resolver: zodResolver(createPromoTareaSchema),
        defaultValues: {
            titulo: defaultValues?.titulo ?? "",
            descripcion: defaultValues?.descripcion ?? "",
            tipoEventoPromoId: defaultValues?.tipoEventoPromoId,
            tipoRewardId: defaultValues?.tipoRewardId,
            importeRecompensa: defaultValues?.importeRecompensa,
            monedaId: defaultValues?.monedaId,
            puntosRecompensa: defaultValues?.puntosRecompensa,
            urlInstrucciones: defaultValues?.urlInstrucciones ?? "",
            esRepetible: defaultValues?.esRepetible ?? true,
            maxRepeticiones: defaultValues?.maxRepeticiones,
            fechaInicio: defaultValues?.fechaInicio ?? "",
            fechaFin: defaultValues?.fechaFin ?? "",
        },
    })

    const tipoRewardId = watch("tipoRewardId")
    const esRepetible = watch("esRepetible")

    const showMoney = tipoRewardId === 1 || tipoRewardId === 3
    const showPoints = tipoRewardId === 2 || tipoRewardId === 3

    const onSubmit = (data: CreatePromoTareaItem) => {
        onGuardar(data)
    }

    return (
        <Card className="bg-[#1a1a2e] border-[#a855f7] border-2">
            <CardHeader className="pb-3">
                <CardTitle className="text-sm text-purple-400">{titulo}</CardTitle>
            </CardHeader>
            <CardContent>
                <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="tarea-titulo">Titulo *</Label>
                        <Input
                            id="tarea-titulo"
                            placeholder="Nombre de la tarea"
                            className="bg-[#0d0d1a] border-zinc-700"
                            {...register("titulo")}
                        />
                        {errors.titulo && <p className="text-xs text-red-400">{errors.titulo.message}</p>}
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="tarea-descripcion">Descripcion</Label>
                        <Textarea
                            id="tarea-descripcion"
                            placeholder="Instrucciones para el promotor..."
                            className="bg-[#0d0d1a] border-zinc-700 min-h-[60px]"
                            {...register("descripcion")}
                        />
                    </div>

                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <div className="space-y-2">
                            <Label>Tipo de evento *</Label>
                            <Select
                                value={watch("tipoEventoPromoId") ? String(watch("tipoEventoPromoId")) : ""}
                                onValueChange={(val) => setValue("tipoEventoPromoId", Number(val), { shouldValidate: true })}
                            >
                                <SelectTrigger className="bg-[#0d0d1a] border-zinc-700">
                                    <SelectValue placeholder="Selecciona evento" />
                                </SelectTrigger>
                                <SelectContent>
                                    {Object.entries(TIPO_EVENTO_PROMO_LABELS).map(([id, label]) => (
                                        <SelectItem key={id} value={id}>{label}</SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                            {errors.tipoEventoPromoId && <p className="text-xs text-red-400">{errors.tipoEventoPromoId.message}</p>}
                        </div>
                        <div className="space-y-2">
                            <Label>Tipo de recompensa *</Label>
                            <Select
                                value={tipoRewardId ? String(tipoRewardId) : ""}
                                onValueChange={(val) => setValue("tipoRewardId", Number(val), { shouldValidate: true })}
                            >
                                <SelectTrigger className="bg-[#0d0d1a] border-zinc-700">
                                    <SelectValue placeholder="Selecciona recompensa" />
                                </SelectTrigger>
                                <SelectContent>
                                    {Object.entries(TIPO_REWARD_PROMO_LABELS).map(([id, label]) => (
                                        <SelectItem key={id} value={id}>{label}</SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                            {errors.tipoRewardId && <p className="text-xs text-red-400">{errors.tipoRewardId.message}</p>}
                        </div>
                    </div>

                    {showMoney && (
                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                            <div className="space-y-2">
                                <Label htmlFor="importeRecompensa">Importe recompensa *</Label>
                                <Input
                                    id="importeRecompensa"
                                    type="number"
                                    step="0.01"
                                    min="0"
                                    className="bg-[#0d0d1a] border-zinc-700"
                                    {...register("importeRecompensa", { valueAsNumber: true })}
                                />
                                {errors.importeRecompensa && <p className="text-xs text-red-400">{errors.importeRecompensa.message}</p>}
                            </div>
                            <div className="space-y-2">
                                <Label>Moneda *</Label>
                                <Select
                                    value={watch("monedaId") ? String(watch("monedaId")) : ""}
                                    onValueChange={(val) => setValue("monedaId", Number(val), { shouldValidate: true })}
                                >
                                    <SelectTrigger className="bg-[#0d0d1a] border-zinc-700">
                                        <SelectValue placeholder="Moneda" />
                                    </SelectTrigger>
                                    <SelectContent>
                                        <SelectItem value="1">EUR</SelectItem>
                                        <SelectItem value="2">USD</SelectItem>
                                    </SelectContent>
                                </Select>
                            </div>
                        </div>
                    )}

                    {showPoints && (
                        <div className="space-y-2">
                            <Label htmlFor="puntosRecompensa">Puntos de recompensa *</Label>
                            <Input
                                id="puntosRecompensa"
                                type="number"
                                min="0"
                                className="bg-[#0d0d1a] border-zinc-700"
                                {...register("puntosRecompensa", { valueAsNumber: true })}
                            />
                            {errors.puntosRecompensa && <p className="text-xs text-red-400">{errors.puntosRecompensa.message}</p>}
                        </div>
                    )}

                    <div className="space-y-2">
                        <Label htmlFor="urlInstrucciones">URL Instrucciones</Label>
                        <Input
                            id="urlInstrucciones"
                            placeholder="https://..."
                            className="bg-[#0d0d1a] border-zinc-700"
                            {...register("urlInstrucciones")}
                        />
                        {errors.urlInstrucciones && <p className="text-xs text-red-400">{errors.urlInstrucciones.message}</p>}
                    </div>

                    <div className="flex items-center gap-3">
                        <Switch
                            id="esRepetible"
                            checked={esRepetible}
                            onCheckedChange={(checked) => setValue("esRepetible", checked)}
                        />
                        <Label htmlFor="esRepetible">Tarea repetible</Label>
                    </div>

                    {esRepetible && (
                        <div className="space-y-2">
                            <Label htmlFor="maxRepeticiones">Max repeticiones *</Label>
                            <Input
                                id="maxRepeticiones"
                                type="number"
                                min="1"
                                className="bg-[#0d0d1a] border-zinc-700"
                                {...register("maxRepeticiones", { valueAsNumber: true })}
                            />
                            {errors.maxRepeticiones && <p className="text-xs text-red-400">{errors.maxRepeticiones.message}</p>}
                        </div>
                    )}

                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <div className="space-y-2">
                            <Label htmlFor="tarea-fechaInicio">Fecha inicio</Label>
                            <Input
                                id="tarea-fechaInicio"
                                type="date"
                                className="bg-[#0d0d1a] border-zinc-700"
                                {...register("fechaInicio")}
                            />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="tarea-fechaFin">Fecha fin</Label>
                            <Input
                                id="tarea-fechaFin"
                                type="date"
                                className="bg-[#0d0d1a] border-zinc-700"
                                {...register("fechaFin")}
                            />
                        </div>
                    </div>

                    <div className="flex justify-end gap-2 pt-2">
                        <Button type="button" variant="outline" size="sm" onClick={onCancelar}>
                            Cancelar
                        </Button>
                        <Button type="submit" size="sm" className="bg-purple-600 hover:bg-purple-700">
                            Guardar tarea
                        </Button>
                    </div>
                </form>
            </CardContent>
        </Card>
    )
}
