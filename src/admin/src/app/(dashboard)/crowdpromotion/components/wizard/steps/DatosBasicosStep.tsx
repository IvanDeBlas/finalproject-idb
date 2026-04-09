"use client"

import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Label } from "@/components/ui/label"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { TIPO_PROMO_LABELS, TIPO_PROMO_DESCRIPTIONS } from "@shared/constants"
import type { CreatePromoProgramaFormData } from "@shared/schemas/crowdpromotion.schema"

const promoProgramaStep1Schema = z.object({
    titulo: z.string().min(1, "El titulo es obligatorio").min(5, "Al menos 5 caracteres").max(200, "Maximo 200 caracteres"),
    descripcion: z.string().max(4000, "Maximo 4000 caracteres").optional().or(z.literal("")),
    tipoPromoId: z.number({ required_error: "El tipo de programa es obligatorio", invalid_type_error: "El tipo de programa es obligatorio" }).int().min(1, "El tipo de programa es obligatorio"),
    campaniaCrowdfundingId: z.string().uuid("UUID no valido").optional().or(z.literal("")),
    proyectoArtisticoId: z.string().uuid("UUID no valido").optional().or(z.literal("")),
    urlLanding: z.string().url("URL no valida").max(500, "Maximo 500 caracteres").optional().or(z.literal("")),
    codigoTrackingBase: z.string().max(50, "Maximo 50 caracteres").regex(/^[a-zA-Z0-9-]*$/, "Solo letras, numeros y guiones").optional().or(z.literal("")),
    fechaInicio: z.string().optional(),
    fechaFin: z.string().optional(),
}).refine(
    (data) => {
        if (data.fechaFin && data.fechaInicio) {
            return data.fechaFin > data.fechaInicio
        }
        return true
    },
    { message: "La fecha fin debe ser posterior a la fecha inicio", path: ["fechaFin"] }
)

type DatosBasicosStepData = z.infer<typeof promoProgramaStep1Schema>

interface DatosBasicosStepProps {
    defaultValues?: Partial<CreatePromoProgramaFormData>
    onNext: (data: Partial<CreatePromoProgramaFormData>) => void
    tienePromotores?: boolean
}

export function DatosBasicosStep({ defaultValues, onNext, tienePromotores }: DatosBasicosStepProps) {
    const {
        register,
        handleSubmit,
        setValue,
        watch,
        formState: { errors },
    } = useForm<DatosBasicosStepData>({
        resolver: zodResolver(promoProgramaStep1Schema),
        defaultValues: {
            titulo: defaultValues?.titulo ?? "",
            descripcion: defaultValues?.descripcion ?? "",
            tipoPromoId: defaultValues?.tipoPromoId,
            campaniaCrowdfundingId: defaultValues?.campaniaCrowdfundingId ?? "",
            proyectoArtisticoId: defaultValues?.proyectoArtisticoId ?? "",
            urlLanding: defaultValues?.urlLanding ?? "",
            codigoTrackingBase: defaultValues?.codigoTrackingBase ?? "",
            fechaInicio: defaultValues?.fechaInicio ?? "",
            fechaFin: defaultValues?.fechaFin ?? "",
        },
    })

    const tipoPromoId = watch("tipoPromoId")

    const onSubmit = (data: DatosBasicosStepData) => {
        onNext(data as Partial<CreatePromoProgramaFormData>)
    }

    return (
        <form id="paso-1-form" onSubmit={handleSubmit(onSubmit)} className="space-y-6 max-w-2xl mx-auto">
            {tienePromotores && (
                <div className="rounded-lg border border-amber-500/30 bg-amber-500/5 p-3">
                    <p className="text-sm text-amber-400">
                        Este programa tiene promotores inscritos. Algunos campos no se pueden modificar.
                    </p>
                </div>
            )}

            <Card className="bg-[#1a1a2e] border-zinc-800">
                <CardHeader>
                    <CardTitle className="text-base text-white">Datos del programa</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="titulo">Titulo *</Label>
                        <Input
                            id="titulo"
                            placeholder="Ej: Programa de Referidos Q1 2026"
                            className="bg-[#0d0d1a] border-zinc-700"
                            {...register("titulo")}
                        />
                        {errors.titulo && <p className="text-xs text-red-400">{errors.titulo.message}</p>}
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="descripcion">Descripcion</Label>
                        <Textarea
                            id="descripcion"
                            placeholder="Describe el objetivo del programa..."
                            className="bg-[#0d0d1a] border-zinc-700 min-h-[80px]"
                            {...register("descripcion")}
                        />
                        {errors.descripcion && <p className="text-xs text-red-400">{errors.descripcion.message}</p>}
                    </div>

                    <div className="space-y-2">
                        <Label>Tipo de programa *</Label>
                        <Select
                            value={tipoPromoId ? String(tipoPromoId) : ""}
                            onValueChange={(val) => setValue("tipoPromoId", Number(val), { shouldValidate: true })}
                        >
                            <SelectTrigger className="bg-[#0d0d1a] border-zinc-700">
                                <SelectValue placeholder="Selecciona tipo" />
                            </SelectTrigger>
                            <SelectContent>
                                {Object.entries(TIPO_PROMO_LABELS).map(([id, label]) => (
                                    <SelectItem key={id} value={id}>{label}</SelectItem>
                                ))}
                            </SelectContent>
                        </Select>
                        {errors.tipoPromoId && <p className="text-xs text-red-400">{errors.tipoPromoId.message}</p>}
                        {tipoPromoId && TIPO_PROMO_DESCRIPTIONS[tipoPromoId] && (
                            <p className="text-xs text-zinc-500">{TIPO_PROMO_DESCRIPTIONS[tipoPromoId]}</p>
                        )}
                    </div>

                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <div className="space-y-2">
                            <Label htmlFor="campaniaCrowdfundingId">Campana (UUID)</Label>
                            <Input
                                id="campaniaCrowdfundingId"
                                placeholder="UUID de la campana (opcional)"
                                className="bg-[#0d0d1a] border-zinc-700"
                                {...register("campaniaCrowdfundingId")}
                            />
                            {errors.campaniaCrowdfundingId && <p className="text-xs text-red-400">{errors.campaniaCrowdfundingId.message}</p>}
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="proyectoArtisticoId">Proyecto artistico (UUID)</Label>
                            <Input
                                id="proyectoArtisticoId"
                                placeholder="UUID del proyecto (opcional)"
                                className="bg-[#0d0d1a] border-zinc-700"
                                {...register("proyectoArtisticoId")}
                            />
                            {errors.proyectoArtisticoId && <p className="text-xs text-red-400">{errors.proyectoArtisticoId.message}</p>}
                        </div>
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="urlLanding">URL Landing</Label>
                        <Input
                            id="urlLanding"
                            placeholder="https://..."
                            className="bg-[#0d0d1a] border-zinc-700"
                            {...register("urlLanding")}
                        />
                        {errors.urlLanding && <p className="text-xs text-red-400">{errors.urlLanding.message}</p>}
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="codigoTrackingBase">Codigo de tracking</Label>
                        <Input
                            id="codigoTrackingBase"
                            placeholder="Ej: ref-q1-2026"
                            className="bg-[#0d0d1a] border-zinc-700"
                            {...register("codigoTrackingBase")}
                        />
                        {errors.codigoTrackingBase && <p className="text-xs text-red-400">{errors.codigoTrackingBase.message}</p>}
                    </div>

                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <div className="space-y-2">
                            <Label htmlFor="fechaInicio">Fecha inicio</Label>
                            <Input
                                id="fechaInicio"
                                type="date"
                                className="bg-[#0d0d1a] border-zinc-700"
                                {...register("fechaInicio")}
                            />
                        </div>
                        <div className="space-y-2">
                            <Label htmlFor="fechaFin">Fecha fin</Label>
                            <Input
                                id="fechaFin"
                                type="date"
                                className="bg-[#0d0d1a] border-zinc-700"
                                {...register("fechaFin")}
                            />
                            {errors.fechaFin && <p className="text-xs text-red-400">{errors.fechaFin.message}</p>}
                        </div>
                    </div>
                </CardContent>
            </Card>
        </form>
    )
}
