"use client"

import { Badge } from "@/components/ui/badge"
import { cn } from "@/lib/utils"
import { ESTADO_NECESIDAD_LABELS, ESTADO_NECESIDAD_BADGES } from "@shared/constants"

interface EstadoNecesidadBadgeProps {
    estadoId: number
}

export function EstadoNecesidadBadge({ estadoId }: EstadoNecesidadBadgeProps) {
    const label = ESTADO_NECESIDAD_LABELS[estadoId] || "Desconocido"
    const color = ESTADO_NECESIDAD_BADGES[estadoId] || "gray"

    return (
        <Badge
            variant="outline"
            className={cn(
                "font-medium",
                color === "green" && "bg-green-900/20 text-green-400 border-green-700",
                color === "blue" && "bg-blue-900/20 text-blue-400 border-blue-700",
                color === "gray" && "bg-gray-900/20 text-gray-400 border-gray-700",
                color === "red" && "bg-red-900/20 text-red-400 border-red-700"
            )}
            aria-label={`Estado: ${label}`}
        >
            {label}
        </Badge>
    )
}
