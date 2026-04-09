"use client"

import { Button } from "@/components/ui/button"
import { EstadoNecesidadBadge } from "../shared/EstadoNecesidadBadge"
import { Pencil, XCircle } from "lucide-react"
import { ESTADO_NECESIDAD } from "@shared/constants"
import type { NecesidadCrowdsourcing } from "@shared/types"

interface NecesidadDetailHeaderProps {
    necesidad: NecesidadCrowdsourcing
    onEdit: () => void
    onCerrar: () => void
}

export function NecesidadDetailHeader({
    necesidad,
    onEdit,
    onCerrar,
}: NecesidadDetailHeaderProps) {
    const canEdit = necesidad.estadoNecesidadId === ESTADO_NECESIDAD.ABIERTA
    const canClose =
        necesidad.estadoNecesidadId === ESTADO_NECESIDAD.ABIERTA ||
        necesidad.estadoNecesidadId === ESTADO_NECESIDAD.EN_PROGRESO

    return (
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
            <div className="space-y-2">
                <div className="flex items-center gap-3">
                    <EstadoNecesidadBadge estadoId={necesidad.estadoNecesidadId} />
                </div>
                <h1 className="text-3xl font-bold">{necesidad.titulo}</h1>
                <p className="text-muted-foreground">
                    Proyecto: {necesidad.proyectoArtisticoNombre}
                </p>
            </div>
            <div className="flex gap-2">
                {canEdit && (
                    <Button variant="outline" onClick={onEdit}>
                        <Pencil className="h-4 w-4 mr-2" />
                        Editar
                    </Button>
                )}
                {canClose && (
                    <Button variant="destructive" onClick={onCerrar}>
                        <XCircle className="h-4 w-4 mr-2" />
                        Cerrar Necesidad
                    </Button>
                )}
            </div>
        </div>
    )
}
