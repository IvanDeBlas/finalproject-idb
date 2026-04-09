"use client"

import Link from "next/link"
import { cn } from "@/lib/utils"
import { Progress } from "@/components/ui/progress"
import { Badge } from "@/components/ui/badge"
import { CAMPANIA_ESTADOS_LABELS } from "@shared/constants"
import type { MiCampaniaListItem } from "@shared/types"

function getGradientByEstado(estadoId: number): string {
    const gradients: Record<number, string> = {
        1: "bg-gradient-to-br from-orange-500 to-orange-600",
        2: "bg-gradient-to-br from-pink-500 to-purple-600",
        3: "bg-gradient-to-br from-blue-500 to-cyan-600",
        4: "bg-gradient-to-br from-gray-600 to-gray-700",
        5: "bg-gradient-to-br from-slate-500 to-slate-600",
    }
    return gradients[estadoId] || gradients[1]
}

function getBadgeVariant(estadoId: number): string {
    const classes: Record<number, string> = {
        1: "bg-yellow-500/20 text-yellow-400 border-yellow-500/30",
        2: "bg-green-500/20 text-green-400 border-green-500/30",
        3: "bg-blue-500/20 text-blue-400 border-blue-500/30",
        4: "bg-red-500/20 text-red-400 border-red-500/30",
        5: "bg-slate-500/20 text-slate-400 border-slate-500/30",
    }
    return classes[estadoId] || classes[1]
}

interface CampaniaProgressItemProps {
    campania: MiCampaniaListItem
}

export function CampaniaProgressItem({ campania }: CampaniaProgressItemProps) {
    const porcentaje = Math.min(campania.porcentajeProgreso, 100)

    return (
        <Link href={`/campanias/${campania.id}`}>
            <div className="flex items-center gap-3 rounded-lg border p-3 hover:bg-card transition-colors cursor-pointer">
                <div
                    className={cn(
                        "w-12 h-12 rounded-lg flex-shrink-0",
                        getGradientByEstado(campania.estadoCampaniaId)
                    )}
                />

                <div className="flex-1 min-w-0">
                    <p className="font-medium truncate">{campania.titulo}</p>
                    <div className="flex items-center gap-2 mt-1">
                        <Progress value={porcentaje} className="h-2 flex-1" />
                        <span className="text-sm text-muted-foreground whitespace-nowrap">
                            {campania.porcentajeProgreso.toFixed(0)}%
                        </span>
                    </div>
                    <p className="text-xs text-muted-foreground mt-1">
                        {campania.numBackers} backers
                    </p>
                </div>

                <Badge className={getBadgeVariant(campania.estadoCampaniaId)}>
                    {CAMPANIA_ESTADOS_LABELS[campania.estadoCampaniaId] || "Desconocido"}
                </Badge>
            </div>
        </Link>
    )
}
