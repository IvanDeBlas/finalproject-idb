"use client"

import { useParams, useRouter } from "next/navigation"
import Link from "next/link"
import { ChevronLeft } from "lucide-react"
import { Skeleton } from "@/components/ui/skeleton"
import { useTemplate } from "@/hooks/use-templates"
import { TemplateHeader } from "../components/detail/TemplateHeader"
import { NecesidadesSection } from "../components/detail/NecesidadesSection"
import { ResumenCard } from "../components/detail/ResumenCard"
import { APP_ROUTES } from "@shared/constants"

export default function TemplateDetailPage() {
    const params = useParams()
    const router = useRouter()
    const id = params.id as string
    const { data: template, isLoading } = useTemplate(id)

    if (isLoading) {
        return (
            <div className="space-y-6">
                <Skeleton className="h-6 w-32" />
                <Skeleton className="h-32 w-full" />
                <Skeleton className="h-64 w-full" />
                <Skeleton className="h-32 w-full" />
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

            <TemplateHeader
                template={template}
                onEdit={() =>
                    router.push(
                        APP_ROUTES.dashboard.crowdsourcing.templateDetail(id) + "/editar"
                    )
                }
            />

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <div className="lg:col-span-2">
                    <NecesidadesSection necesidades={template.necesidades} />
                </div>
                <div>
                    <ResumenCard resumen={template.resumen} />
                </div>
            </div>
        </div>
    )
}
