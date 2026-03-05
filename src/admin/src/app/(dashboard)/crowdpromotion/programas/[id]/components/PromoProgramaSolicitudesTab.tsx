"use client"

import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { Inbox } from "lucide-react"
import { useInscripcionesPendientes } from "@/hooks/use-inscripciones"
import { SolicitudCard } from "./SolicitudCard"

interface PromoProgramaSolicitudesTabProps {
    programaId: string
}

export function PromoProgramaSolicitudesTab({ programaId }: PromoProgramaSolicitudesTabProps) {
    const { data, isLoading, isError, refetch } = useInscripcionesPendientes(programaId)

    if (isLoading) {
        return (
            <div className="space-y-3 mt-4" aria-busy="true" aria-label="Cargando solicitudes">
                {[1, 2, 3].map((i) => (
                    <Skeleton key={i} className="h-[140px] rounded-xl bg-[#1e1e38]" />
                ))}
            </div>
        )
    }

    if (isError) {
        return (
            <div className="flex flex-col items-center justify-center py-12 text-center mt-4">
                <p className="text-sm text-red-400 mb-4">Error al cargar las solicitudes</p>
                <Button variant="outline" onClick={() => refetch()}>
                    Reintentar
                </Button>
            </div>
        )
    }

    const items = data?.items ?? []

    if (items.length === 0) {
        return (
            <div className="flex flex-col items-center justify-center py-12 text-center mt-4">
                <Inbox className="w-10 h-10 text-[#64748b] mb-3" aria-hidden="true" />
                <p className="text-sm text-[#64748b]">No hay solicitudes pendientes</p>
            </div>
        )
    }

    return (
        <div className="space-y-1 mt-4">
            <div className="flex items-start justify-between mb-4">
                <div>
                    <h3 className="text-base font-semibold text-white">
                        Solicitudes pendientes ({items.length})
                    </h3>
                    <p className="text-sm text-[#94a3b8] mt-0.5">
                        Revisa y gestiona las solicitudes de inscripcion
                    </p>
                </div>
            </div>
            {items.map((inscripcion) => (
                <SolicitudCard
                    key={inscripcion.id}
                    inscripcion={inscripcion}
                    programaId={programaId}
                />
            ))}
        </div>
    )
}
