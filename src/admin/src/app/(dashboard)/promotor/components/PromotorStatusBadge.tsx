"use client"

import { Badge } from "@/components/ui/badge"
import { cn } from "@/lib/utils"

interface PromotorStatusBadgeProps {
    esActivo: boolean
    className?: string
}

export function PromotorStatusBadge({ esActivo, className }: PromotorStatusBadgeProps) {
    return (
        <Badge
            variant="outline"
            className={cn(
                esActivo
                    ? "border-green-500/50 bg-green-950/50 text-green-400"
                    : "border-slate-500/50 bg-slate-800 text-slate-400",
                className
            )}
        >
            {esActivo ? "Activo" : "Inactivo"}
        </Badge>
    )
}
