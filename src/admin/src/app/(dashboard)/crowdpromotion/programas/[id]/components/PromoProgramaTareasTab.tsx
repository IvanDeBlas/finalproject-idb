"use client"

import { Card, CardContent } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { ClipboardList, Plus } from "lucide-react"
import { useRouter } from "next/navigation"
import { APP_ROUTES } from "@shared/constants"
import { TIPO_EVENTO_PROMO_LABELS, TIPO_REWARD_PROMO_LABELS } from "@shared/constants"
import type { PromoTareaDetail } from "@shared/types"

interface PromoProgramaTareasTabProps {
    tareas: PromoTareaDetail[]
    esActivo: boolean
    programaId: string
}

export function PromoProgramaTareasTab({ tareas, esActivo, programaId }: PromoProgramaTareasTabProps) {
    const router = useRouter()

    if (tareas.length === 0) {
        return (
            <div className="flex flex-col items-center justify-center py-12 text-center">
                <div className="rounded-full bg-[#1e1e38] p-4 mb-4">
                    <ClipboardList className="h-8 w-8 text-zinc-400" />
                </div>
                <p className="text-sm text-zinc-400 mb-4">Este programa no tiene tareas definidas</p>
                {esActivo && (
                    <Button
                        variant="outline"
                        onClick={() => router.push(APP_ROUTES.dashboard.crowdpromotion.programas.editar(programaId))}
                    >
                        <Plus className="h-4 w-4 mr-2" />
                        Agregar tareas
                    </Button>
                )}
            </div>
        )
    }

    return (
        <div className="space-y-4">
            {esActivo && (
                <div className="flex justify-end">
                    <Button
                        variant="outline"
                        size="sm"
                        onClick={() => router.push(APP_ROUTES.dashboard.crowdpromotion.programas.editar(programaId))}
                    >
                        <Plus className="h-4 w-4 mr-2" />
                        Agregar tarea
                    </Button>
                </div>
            )}
            <div className="grid gap-3">
                {tareas.map((tarea) => (
                    <Card key={tarea.id} className="bg-[#1a1a2e] border-zinc-800">
                        <CardContent className="p-4">
                            <div className="flex items-start justify-between gap-3">
                                <div className="flex-1">
                                    <div className="flex items-center gap-2 mb-1 flex-wrap">
                                        <span className="text-sm font-medium text-white">{tarea.titulo}</span>
                                        <Badge variant="outline" className="text-xs border-blue-500/30 text-blue-400">
                                            {TIPO_EVENTO_PROMO_LABELS[tarea.tipoEventoPromoId] || tarea.tipoEventoPromoNombre}
                                        </Badge>
                                        <Badge variant="outline" className="text-xs border-amber-500/30 text-amber-400">
                                            {TIPO_REWARD_PROMO_LABELS[tarea.tipoRewardId] || tarea.tipoRewardNombre}
                                        </Badge>
                                        {tarea.esActivo ? (
                                            <Badge className="bg-emerald-500/20 text-emerald-400 text-xs">Activa</Badge>
                                        ) : (
                                            <Badge className="bg-zinc-500/20 text-zinc-400 text-xs">Inactiva</Badge>
                                        )}
                                    </div>
                                    {tarea.descripcion && (
                                        <p className="text-xs text-zinc-400 mb-2">{tarea.descripcion}</p>
                                    )}
                                    <div className="flex flex-wrap gap-x-4 gap-y-1 text-xs text-zinc-500">
                                        {tarea.importeRecompensa != null && (
                                            <span>Recompensa: {tarea.importeRecompensa} {tarea.monedaNombre}</span>
                                        )}
                                        {tarea.puntosRecompensa != null && (
                                            <span>{tarea.puntosRecompensa} puntos</span>
                                        )}
                                        {tarea.esRepetible && (
                                            <span>Repetible (max: {tarea.maxRepeticiones ?? "ilimitado"})</span>
                                        )}
                                        <span>{tarea.completadosPorPromotores} completados</span>
                                    </div>
                                </div>
                            </div>
                        </CardContent>
                    </Card>
                ))}
            </div>
        </div>
    )
}
