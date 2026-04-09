"use client"

import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { Users } from "lucide-react"
import { useInscripcionesAprobadas } from "@/hooks/use-inscripciones"
import { AprobadoCard } from "./AprobadoCard"

interface PromoProgramaAprobadosTabProps {
    programaId: string
}

export function PromoProgramaAprobadosTab({ programaId }: PromoProgramaAprobadosTabProps) {
    const { data, isLoading, isError, refetch } = useInscripcionesAprobadas(programaId)

    if (isLoading) {
        return (
            <div className="space-y-3 mt-4" aria-busy="true" aria-label="Cargando promotores aprobados">
                {[1, 2, 3].map((i) => (
                    <Skeleton key={i} className="h-[90px] rounded-xl bg-[#1e1e38]" />
                ))}
            </div>
        )
    }

    if (isError) {
        return (
            <div className="flex flex-col items-center justify-center py-12 text-center mt-4">
                <p className="text-sm text-red-400 mb-4">Error al cargar los promotores aprobados</p>
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
                <Users className="w-10 h-10 text-[#64748b] mb-3" aria-hidden="true" />
                <p className="text-sm text-[#64748b]">No hay promotores aprobados en este programa</p>
            </div>
        )
    }

    return (
        <div className="space-y-3 mt-4">
            <div className="flex items-center justify-between mb-4">
                <h3 className="text-base font-semibold text-white">
                    Promotores aprobados ({items.length})
                </h3>
            </div>
            {items.map((inscripcion) => (
                <AprobadoCard
                    key={inscripcion.id}
                    inscripcion={inscripcion}
                    programaId={programaId}
                />
            ))}
        </div>
    )
}
