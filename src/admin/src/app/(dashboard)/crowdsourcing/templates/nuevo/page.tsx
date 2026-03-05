"use client"

import { useRouter } from "next/navigation"
import Link from "next/link"
import { ChevronLeft } from "lucide-react"
import { useCreateTemplate } from "@/hooks/use-templates-mutations"
import { useRolesProfesionales } from "@/hooks/use-roles-profesionales"
import { TemplateForm } from "../components/form/TemplateForm"
import { Skeleton } from "@/components/ui/skeleton"
import { APP_ROUTES } from "@shared/constants"
import type { CreateTemplateFormData } from "@/lib/validations/template.schema"

export default function NuevoTemplatePage() {
    const router = useRouter()
    const createMutation = useCreateTemplate()
    const { data: rolesProfesionales, isLoading: rolesLoading } = useRolesProfesionales()

    const handleSubmit = async (data: CreateTemplateFormData) => {
        await createMutation.mutateAsync({
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
        })
        router.push(APP_ROUTES.dashboard.crowdsourcing.templates)
    }

    if (rolesLoading) {
        return (
            <div className="space-y-6">
                <Skeleton className="h-8 w-48" />
                <Skeleton className="h-64 w-full" />
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

            <h1 className="text-3xl font-bold">Nuevo Template</h1>

            <TemplateForm
                mode="create"
                rolesProfesionales={rolesProfesionales ?? []}
                onSubmit={handleSubmit}
                onCancel={() => router.push(APP_ROUTES.dashboard.crowdsourcing.templates)}
                isPending={createMutation.isPending}
            />
        </div>
    )
}
