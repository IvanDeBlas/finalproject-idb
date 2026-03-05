"use client"

import { Badge } from "@/components/ui/badge"
import { cn } from "@/lib/utils"
import type { PrioridadNecesidad } from "@shared/types"

interface PriorityBadgeProps {
    prioridad: PrioridadNecesidad
    variant?: "default" | "outline"
}

const colorMap: Record<PrioridadNecesidad, string> = {
    Alta: "bg-red-500/10 text-red-500 border-red-500/20",
    Media: "bg-yellow-500/10 text-yellow-500 border-yellow-500/20",
    Baja: "bg-blue-500/10 text-blue-500 border-blue-500/20",
}

export function PriorityBadge({ prioridad, variant = "outline" }: PriorityBadgeProps) {
    return (
        <Badge variant={variant} className={cn(colorMap[prioridad])}>
            {prioridad}
        </Badge>
    )
}
