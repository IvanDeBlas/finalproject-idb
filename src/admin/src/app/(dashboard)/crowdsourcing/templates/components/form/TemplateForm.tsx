"use client"

import { Button } from "@/components/ui/button"
import { Loader2 } from "lucide-react"
import { useForm, FormProvider } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import {
    createTemplateSchema,
    type CreateTemplateFormData,
} from "@/lib/validations/template.schema"
import { GeneralDataSection } from "./GeneralDataSection"
import { NecesidadesFormSection } from "./NecesidadesFormSection"
import type { PlantillaProyecto, RolProfesionalConCategoria } from "@shared/types"

interface TemplateFormProps {
    mode: "create" | "edit"
    initialData?: PlantillaProyecto
    rolesProfesionales: RolProfesionalConCategoria[]
    onSubmit: (data: CreateTemplateFormData) => Promise<void>
    onCancel: () => void
    isPending?: boolean
}

function mapTemplateToFormData(
    template: PlantillaProyecto
): CreateTemplateFormData {
    return {
        nombre: template.nombre,
        descripcion: template.descripcion ?? "",
        icono: template.icono ?? "",
        orden: template.orden,
        activo: true,
        necesidades: template.necesidades.map((n, i) => ({
            fase: n.fase,
            titulo: n.titulo,
            descripcion: n.descripcion ?? "",
            rolProfesionalId: n.rolProfesional.id,
            precioMinOrientativo: n.precioMinOrientativo,
            precioMaxOrientativo: n.precioMaxOrientativo,
            prioridad: n.prioridad,
            orden: i,
        })),
    }
}

export function TemplateForm({
    mode,
    initialData,
    rolesProfesionales,
    onSubmit,
    onCancel,
    isPending = false,
}: TemplateFormProps) {
    const form = useForm<CreateTemplateFormData>({
        resolver: zodResolver(createTemplateSchema),
        defaultValues: initialData
            ? mapTemplateToFormData(initialData)
            : {
                  nombre: "",
                  descripcion: "",
                  icono: "",
                  orden: 0,
                  activo: true,
                  necesidades: [],
              },
    })

    const handleFormSubmit = form.handleSubmit(async (data) => {
        await onSubmit(data)
    })

    return (
        <FormProvider {...form}>
            <form onSubmit={handleFormSubmit} className="space-y-6">
                <GeneralDataSection />
                <NecesidadesFormSection rolesProfesionales={rolesProfesionales} />

                <div className="flex gap-4 justify-end">
                    <Button
                        type="button"
                        variant="outline"
                        onClick={onCancel}
                        disabled={isPending}
                    >
                        Cancelar
                    </Button>
                    <Button type="submit" disabled={isPending}>
                        {isPending && (
                            <Loader2 className="animate-spin h-4 w-4 mr-2" />
                        )}
                        {isPending
                            ? "Guardando..."
                            : mode === "create"
                              ? "Crear template"
                              : "Guardar cambios"}
                    </Button>
                </div>
            </form>
        </FormProvider>
    )
}
