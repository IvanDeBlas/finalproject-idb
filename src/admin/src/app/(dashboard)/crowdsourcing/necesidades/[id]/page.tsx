"use client"

import { useState } from "react"
import { useRouter, useParams } from "next/navigation"
import { ArrowLeft } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { useNecesidad } from "@/hooks/use-necesidades"
import { useCerrarNecesidad } from "@/hooks/use-necesidades-mutations"
import { NecesidadDetailHeader } from "../components/detail/NecesidadDetailHeader"
import { NecesidadDetailsCard } from "../components/detail/NecesidadDetailsCard"
import { PropuestasSection } from "../components/detail/PropuestasSection"
import { CerrarNecesidadDialog } from "../components/shared/CerrarNecesidadDialog"
import { APP_ROUTES, ESTADO_NECESIDAD } from "@shared/constants"

export default function NecesidadDetailPage() {
    const router = useRouter()
    const params = useParams<{ id: string }>()
    const id = params.id

    const { data: necesidad, isLoading } = useNecesidad(id)
    const cerrarMutation = useCerrarNecesidad(id)

    const [cerrarDialogOpen, setCerrarDialogOpen] = useState(false)

    const handleCerrarConfirm = (motivo?: string) => {
        cerrarMutation.mutate(
            { motivo },
            {
                onSettled: () => setCerrarDialogOpen(false),
            }
        )
    }

    if (isLoading) {
        return (
            <div className="space-y-6">
                <Skeleton className="h-10 w-48" />
                <Skeleton className="h-32" />
                <Skeleton className="h-64" />
            </div>
        )
    }

    if (!necesidad) {
        return (
            <div className="text-center py-12">
                <p className="text-muted-foreground">Necesidad no encontrada</p>
                <Button
                    variant="outline"
                    className="mt-4"
                    onClick={() =>
                        router.push(APP_ROUTES.dashboard.crowdsourcing.necesidades)
                    }
                >
                    Volver a Mis Necesidades
                </Button>
            </div>
        )
    }

    const readonly = necesidad.estadoNecesidadId !== ESTADO_NECESIDAD.ABIERTA

    return (
        <div className="space-y-6">
            <Button
                variant="ghost"
                onClick={() =>
                    router.push(APP_ROUTES.dashboard.crowdsourcing.necesidades)
                }
                className="flex items-center gap-2"
            >
                <ArrowLeft className="h-4 w-4" />
                Mis Necesidades
            </Button>

            <NecesidadDetailHeader
                necesidad={necesidad}
                onEdit={() =>
                    router.push(
                        APP_ROUTES.dashboard.crowdsourcing.editarNecesidad(id)
                    )
                }
                onCerrar={() => setCerrarDialogOpen(true)}
            />

            <NecesidadDetailsCard necesidad={necesidad} />

            <PropuestasSection
                propuestas={necesidad.propuestas}
                readonly={readonly}
            />

            <CerrarNecesidadDialog
                open={cerrarDialogOpen}
                onOpenChange={setCerrarDialogOpen}
                onConfirm={handleCerrarConfirm}
                isPending={cerrarMutation.isPending}
            />
        </div>
    )
}
