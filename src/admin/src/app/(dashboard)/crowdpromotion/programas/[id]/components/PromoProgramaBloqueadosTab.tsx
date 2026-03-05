"use client"

import { Button } from "@/components/ui/button"
import { Card } from "@/components/ui/card"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Badge } from "@/components/ui/badge"
import { Skeleton } from "@/components/ui/skeleton"
import { Ban, Calendar, ShieldCheck, Info } from "lucide-react"
import { useInscripcionesBloqueadas } from "@/hooks/use-inscripciones"
import type { InscripcionListItem } from "@shared/types"

interface PromoProgramaBloqueadosTabProps {
    programaId: string
}

function getInitials(name: string): string {
    return name.slice(0, 2).toUpperCase()
}

function formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString("es-ES", {
        day: "2-digit",
        month: "short",
        year: "numeric",
    })
}

function BloqueadoCard({ inscripcion }: { inscripcion: InscripcionListItem }) {
    return (
        <Card className="bg-[#0f0f1f] border border-[#334155] p-4 opacity-80">
            <div className="flex items-center gap-3">
                <Avatar className="w-9 h-9">
                    <AvatarFallback className="bg-red-950/50 text-red-400 text-xs font-semibold">
                        {getInitials(inscripcion.promotorNombre)}
                    </AvatarFallback>
                </Avatar>
                <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium text-white truncate">
                        {inscripcion.promotorNombre}
                    </p>
                    <p className="text-xs text-[#94a3b8]">{inscripcion.tipoPromotorNombre}</p>
                    <p className="text-xs text-[#64748b] mt-1 flex items-center gap-1">
                        <Calendar className="w-3 h-3" aria-hidden="true" />
                        Bloqueado: {formatDate(inscripcion.fechaAlta)}
                    </p>
                </div>
                <Badge className="bg-red-950/50 text-red-400 border border-red-800/50 text-xs ml-auto shrink-0 flex items-center gap-1">
                    <Ban className="w-3 h-3" aria-hidden="true" />
                    BLOQUEADO
                </Badge>
            </div>
        </Card>
    )
}

export function PromoProgramaBloqueadosTab({ programaId }: PromoProgramaBloqueadosTabProps) {
    const { data, isLoading, isError, refetch } = useInscripcionesBloqueadas(programaId)

    if (isLoading) {
        return (
            <div className="space-y-3 mt-4" aria-busy="true" aria-label="Cargando promotores bloqueados">
                {[1, 2].map((i) => (
                    <Skeleton key={i} className="h-[80px] rounded-xl bg-[#1e1e38]" />
                ))}
            </div>
        )
    }

    if (isError) {
        return (
            <div className="flex flex-col items-center justify-center py-12 text-center mt-4">
                <p className="text-sm text-red-400 mb-4">Error al cargar los promotores bloqueados</p>
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
                <ShieldCheck className="w-10 h-10 text-[#64748b] mb-3" aria-hidden="true" />
                <p className="text-sm text-[#64748b]">No hay promotores bloqueados</p>
            </div>
        )
    }

    return (
        <div className="space-y-3 mt-4">
            <div className="flex items-start gap-2 p-3 rounded-lg bg-blue-950/40 border border-blue-800/50 mb-4">
                <Info className="w-4 h-4 text-blue-400 shrink-0 mt-0.5" aria-hidden="true" />
                <p className="text-sm text-blue-300">
                    Los promotores bloqueados no pueden re-solicitar inscripcion en este programa.
                </p>
            </div>
            {items.map((inscripcion) => (
                <BloqueadoCard key={inscripcion.id} inscripcion={inscripcion} />
            ))}
        </div>
    )
}
