"use client"

import { Card, CardHeader, CardContent, CardFooter } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { EstadoNecesidadBadge } from "../shared/EstadoNecesidadBadge"
import { ModalidadBadge } from "../shared/ModalidadBadge"
import { Music, Users, Calendar } from "lucide-react"
import { formatPresupuesto, formatRelativeDate, getDaysRemaining } from "@shared/utils/format"
import { ESTADO_NECESIDAD } from "@shared/constants"
import type { NecesidadCrowdsourcingList } from "@shared/types"

interface NecesidadCardProps {
    necesidad: NecesidadCrowdsourcingList
    onClick?: () => void
    onEdit?: () => void
    onCerrar?: () => void
}

export function NecesidadCard({ necesidad, onClick, onEdit, onCerrar }: NecesidadCardProps) {
    const daysRemaining = necesidad.fechaLimitePropuestas
        ? getDaysRemaining(necesidad.fechaLimitePropuestas)
        : null

    const isUrgent = daysRemaining !== null && daysRemaining < 3
    const isClosingSoon = daysRemaining !== null && daysRemaining < 7 && !isUrgent

    return (
        <Card
            role="article"
            className="transition-all hover:shadow-md hover:border-primary/30 cursor-pointer"
            onClick={onClick}
        >
            <CardHeader className="pb-3">
                <div className="flex items-center justify-between gap-2">
                    <EstadoNecesidadBadge estadoId={necesidad.estadoNecesidadId} />
                    {isUrgent && (
                        <Badge variant="destructive" className="text-xs">Urgente</Badge>
                    )}
                    {isClosingSoon && (
                        <Badge variant="outline" className="text-xs bg-yellow-900/20 text-yellow-400 border-yellow-700">
                            Cierra pronto
                        </Badge>
                    )}
                </div>
                <h3 className="text-lg font-semibold line-clamp-2 mt-2">{necesidad.titulo}</h3>
            </CardHeader>

            <CardContent className="space-y-3 pb-3">
                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                    <Music className="h-4 w-4 shrink-0" />
                    <span>{necesidad.tipoNecesidadNombre}</span>
                    <ModalidadBadge modalidadId={necesidad.modalidadTrabajoId} />
                </div>

                <div className="text-base font-medium">
                    {formatPresupuesto(necesidad.presupuestoMin, necesidad.presupuestoMax, necesidad.monedaId)}
                </div>

                <div className="flex items-center gap-4 text-sm text-muted-foreground">
                    <div className="flex items-center gap-1">
                        <Users className="h-4 w-4" />
                        <span>{necesidad.numeroPropuestas} propuestas</span>
                    </div>
                    <div className="flex items-center gap-1">
                        <Calendar className="h-4 w-4" />
                        <span>{formatRelativeDate(necesidad.fechaCreacion)}</span>
                    </div>
                </div>
            </CardContent>

            <CardFooter className="flex gap-2 pt-3 border-t" onClick={(e) => e.stopPropagation()}>
                <Button variant="ghost" size="sm" onClick={onClick}>
                    Ver Detalle
                </Button>
                {necesidad.estadoNecesidadId === ESTADO_NECESIDAD.ABIERTA && (
                    <Button variant="ghost" size="sm" onClick={onEdit}>
                        Editar
                    </Button>
                )}
                {(necesidad.estadoNecesidadId === ESTADO_NECESIDAD.ABIERTA ||
                    necesidad.estadoNecesidadId === ESTADO_NECESIDAD.EN_PROGRESO) && (
                    <Button
                        variant="ghost"
                        size="sm"
                        className="text-red-400 hover:text-red-300"
                        onClick={onCerrar}
                    >
                        Cerrar
                    </Button>
                )}
            </CardFooter>
        </Card>
    )
}
