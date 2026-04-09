"use client"

import { Card, CardContent } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Pencil, Trash2 } from "lucide-react"
import { TIPO_EVENTO_PROMO_LABELS, TIPO_REWARD_PROMO_LABELS } from "@shared/constants"
import type { CreatePromoTareaItem } from "@shared/types"

interface PromoTareaCardProps {
    tarea: CreatePromoTareaItem
    index: number
    onEditar: (index: number) => void
    onEliminar: (index: number) => void
    puedeEliminar: boolean
    eliminandoDeshabilitadoTooltip?: string
}

export function PromoTareaCard({
    tarea,
    index,
    onEditar,
    onEliminar,
    puedeEliminar,
}: PromoTareaCardProps) {
    return (
        <Card className="bg-[#1a1a2e] border-zinc-800">
            <CardContent className="p-4">
                <div className="flex items-start justify-between gap-3">
                    <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2 mb-1 flex-wrap">
                            <span className="text-sm font-medium text-white">{tarea.titulo}</span>
                            <Badge variant="outline" className="text-xs border-blue-500/30 text-blue-400">
                                {TIPO_EVENTO_PROMO_LABELS[tarea.tipoEventoPromoId] || `Evento ${tarea.tipoEventoPromoId}`}
                            </Badge>
                            <Badge variant="outline" className="text-xs border-amber-500/30 text-amber-400">
                                {TIPO_REWARD_PROMO_LABELS[tarea.tipoRewardId] || `Reward ${tarea.tipoRewardId}`}
                            </Badge>
                        </div>
                        {tarea.descripcion && (
                            <p className="text-xs text-zinc-400 mb-1">{tarea.descripcion}</p>
                        )}
                        <div className="flex flex-wrap gap-x-3 gap-y-1 text-xs text-zinc-500">
                            {tarea.importeRecompensa != null && (
                                <span>Recompensa: {tarea.importeRecompensa}</span>
                            )}
                            {tarea.puntosRecompensa != null && (
                                <span>{tarea.puntosRecompensa} puntos</span>
                            )}
                            {tarea.esRepetible && (
                                <span>Repetible (max: {tarea.maxRepeticiones ?? "?"})</span>
                            )}
                        </div>
                    </div>

                    <div className="flex items-center gap-1">
                        <Button
                            variant="ghost"
                            size="icon"
                            className="h-7 w-7 text-zinc-400 hover:text-white"
                            onClick={() => onEditar(index)}
                        >
                            <Pencil className="h-3.5 w-3.5" />
                        </Button>
                        <Button
                            variant="ghost"
                            size="icon"
                            className="h-7 w-7 text-zinc-400 hover:text-red-400"
                            onClick={() => onEliminar(index)}
                            disabled={!puedeEliminar}
                            title={!puedeEliminar ? "No se puede eliminar: tiene completados por promotores" : undefined}
                        >
                            <Trash2 className="h-3.5 w-3.5" />
                        </Button>
                    </div>
                </div>
            </CardContent>
        </Card>
    )
}
