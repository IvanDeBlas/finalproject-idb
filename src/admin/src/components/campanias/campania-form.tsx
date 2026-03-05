"use client"

import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { Card, CardContent } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"

/**
 * @deprecated Use the wizard form at campanias/components/wizard/WizardContainer instead.
 * This legacy form is kept for reference only.
 */

const legacySchema = z.object({
    titulo: z.string().min(1, "El titulo es obligatorio").max(200),
    descripcionCorta: z.string().max(500).optional().or(z.literal("")),
    importeObjetivo: z.number({ invalid_type_error: "Debe ser un numero" }).positive().min(100),
    fechaFin: z.date().optional(),
    imagenPrincipalUrl: z.string().url().optional().or(z.literal("")),
})

type LegacyFormData = z.infer<typeof legacySchema>

interface CampaniaFormProps {
    defaultValues?: Partial<LegacyFormData>
    onSubmit: (data: LegacyFormData) => Promise<void>
    isSubmitting?: boolean
}

export function CampaniaForm({ defaultValues, onSubmit, isSubmitting }: CampaniaFormProps) {
    const {
        register,
        handleSubmit,
        formState: { errors },
    } = useForm<LegacyFormData>({
        resolver: zodResolver(legacySchema),
        defaultValues,
    })

    return (
        <Card>
            <CardContent className="pt-6">
                <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                    <div className="space-y-2">
                        <Label htmlFor="titulo">Titulo</Label>
                        <Input
                            id="titulo"
                            placeholder="Nombre de tu campania"
                            {...register("titulo")}
                        />
                        {errors.titulo && (
                            <p className="text-sm text-destructive">{errors.titulo.message}</p>
                        )}
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="descripcionCorta">Descripcion</Label>
                        <Textarea
                            id="descripcionCorta"
                            placeholder="Describe tu proyecto musical..."
                            rows={6}
                            {...register("descripcionCorta")}
                        />
                        {errors.descripcionCorta && (
                            <p className="text-sm text-destructive">{errors.descripcionCorta.message}</p>
                        )}
                    </div>

                    <div className="grid gap-4 sm:grid-cols-2">
                        <div className="space-y-2">
                            <Label htmlFor="importeObjetivo">Meta financiera (EUR)</Label>
                            <Input
                                id="importeObjetivo"
                                type="number"
                                min={100}
                                placeholder="5000"
                                {...register("importeObjetivo", { valueAsNumber: true })}
                            />
                            {errors.importeObjetivo && (
                                <p className="text-sm text-destructive">{errors.importeObjetivo.message}</p>
                            )}
                        </div>

                        <div className="space-y-2">
                            <Label htmlFor="fechaFin">Fecha de finalizacion</Label>
                            <Input
                                id="fechaFin"
                                type="date"
                                {...register("fechaFin", { valueAsDate: true })}
                            />
                            {errors.fechaFin && (
                                <p className="text-sm text-destructive">{errors.fechaFin.message}</p>
                            )}
                        </div>
                    </div>

                    <div className="space-y-2">
                        <Label htmlFor="imagenPrincipalUrl">URL de imagen (opcional)</Label>
                        <Input
                            id="imagenPrincipalUrl"
                            type="url"
                            placeholder="https://..."
                            {...register("imagenPrincipalUrl")}
                        />
                        {errors.imagenPrincipalUrl && (
                            <p className="text-sm text-destructive">{errors.imagenPrincipalUrl.message}</p>
                        )}
                    </div>

                    <div className="flex gap-4">
                        <Button type="submit" disabled={isSubmitting}>
                            {isSubmitting ? "Guardando..." : "Guardar campania"}
                        </Button>
                    </div>
                </form>
            </CardContent>
        </Card>
    )
}
