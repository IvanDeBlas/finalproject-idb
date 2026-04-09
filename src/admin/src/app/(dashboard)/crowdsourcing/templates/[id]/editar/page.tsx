"use client"

import { useParams, useRouter } from "next/navigation"
import Link from "next/link"
import { ChevronLeft } from "lucide-react"
import { Skeleton } from "@/components/ui/skeleton"
import { useTemplate } from "@/hooks/use-templates"
import { useUpdateTemplate } from "@/hooks/use-templates-mutations"
import { useRolesProfesionales } from "@/hooks/use-roles-profesionales"
import { TemplateForm } from "../../components/form/TemplateForm"
import { APP_ROUTES } from "@shared/constants"
import type { CreateTemplateFormData } from "@/lib/validations/template.schema"

export default function EditarTemplatePage() {
    const params = useParams()
    const router = useRouter()
    const id = params.id as string
    const { data: template, isLoading: templateLoading } = useTemplate(id)
    const { data: rolesProfesionales, isLoading: rolesLoading } = useRolesProfesionales()
    const updateMutation = useUpdateTemplate()

    const isLoading = templateLoading || rolesLoading

    const handleSubmit = async (data: CreateTemplateFormData) => {
        await updateMutation.mutateAsync({
            id,
            data: {
                nombre: data.nombre,
                descripcion: data.descripcion,
                icono: data.icono,
                orden: data.orden,
                activo: data.activo,
                necesidades: data.necesidades.map((n) => ({
                    fase: n.fase,
                    titulo: n.titulo,
                    descripcion: n.descripcion,
                    rolProfesionalId: n.rolProfesionalId,
                    precioMinOrientativo: n.precioMinOrientativo,
                    precioMaxOrientativo: n.precioMaxOrientativo,
                    prioridad: n.prioridad,
                    orden: n.orden,
                })),
            },
        })
        router.push(APP_ROUTES.dashboard.crowdsourcing.templates)
    }

    if (isLoading) {
        return (
            <div className="space-y-6">
                <Skeleton className="h-6 w-32" />
                <Skeleton className="h-8 w-48" />
                <Skeleton className="h-64 w-full" />
            </div>
        )
    }

    if (!template) {
        return (
            <div className="space-y-6">
                <Link
                    href={APP_ROUTES.dashboard.crowdsourcing.templates}
                    className="inline-flex items-center gap-1 text-sm text-primary hover:underline"
                >
                    <ChevronLeft className="h-4 w-4" />
                    Volver a Templates
                </Link>
                <p className="text-muted-foreground">Template no encontrado</p>
            </div>
        )
    }

    return (
        <div className="space-y-6">
            <Link
                href={APP_ROUTES.dashboard.crowdsourcing.templates}
                className="inline-flex items-center gap-1 text-sm text-primary hover:underline"
            >
                <ChevronLeft className="h-4 w-4" />
                Volver a Templates
            </Link>

            <h1 className="text-3xl font-bold">Editar Template</h1>

            <TemplateForm
                mode="edit"
                initialData={template}
                rolesProfesionales={rolesProfesionales ?? []}
                onSubmit={handleSubmit}
                onCancel={() => router.push(APP_ROUTES.dashboard.crowdsourcing.templates)}
                isPending={updateMutation.isPending}
            />
        </div>
    )
}
