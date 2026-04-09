"use client"

import { cn } from "@/lib/utils"
import { Badge } from "@/components/ui"
import { CAMPANIA_ESTADOS_LABELS } from "@shared/constants"

interface CampaniaStatusBadgeProps {
    estadoId: number
    className?: string
}

const COLOR_MAP: Record<number, string> = {
    1: "bg-yellow-500/20 text-yellow-400 border-yellow-500/50", // Borrador
    2: "bg-green-500/20 text-green-400 border-green-500/50", // Publicada
    3: "bg-blue-500/20 text-blue-400 border-blue-500/50", // Finalizada
    4: "bg-red-500/20 text-red-400 border-red-500/50", // Cancelada
}

export function CampaniaStatusBadge({
    estadoId,
    className,
}: CampaniaStatusBadgeProps) {
    const label = CAMPANIA_ESTADOS_LABELS[estadoId] || "Desconocido"
    const colorClass = COLOR_MAP[estadoId] || ""

    return (
        <Badge
            variant="outline"
            className={cn(colorClass, className)}
            role="status"
            aria-label={`Estado de campania: ${label}`}
        >
            {label}
        </Badge>
    )
}
