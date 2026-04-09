"use client"

import { useState } from "react"
import { Card } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Clock, Calendar } from "lucide-react"
import { formatPresupuesto, formatRelativeDate } from "@shared/utils/format"
import type { PropuestaCrowdsourcing } from "@shared/types"

interface PropuestaCardProps {
    propuesta: PropuestaCrowdsourcing
    readonly?: boolean
    onVerPerfil?: () => void
    onAceptar?: () => void
    onRechazar?: () => void
}

export function PropuestaCard({
    propuesta,
    readonly = false,
    onVerPerfil,
    onAceptar,
    onRechazar,
}: PropuestaCardProps) {
    const [expanded, setExpanded] = useState(false)
    const shouldTruncate = propuesta.mensaje.length > 150

    return (
        <Card className="p-4">
            <div className="flex items-center gap-3 mb-3">
                <Avatar>
                    <AvatarFallback>
                        {propuesta.profesionalNombre.charAt(0).toUpperCase()}
                    </AvatarFallback>
                </Avatar>
                <div className="flex-1">
                    <div className="font-semibold">{propuesta.profesionalNombre}</div>
                    <div className="text-sm text-muted-foreground">
                        {propuesta.estadoPropuestaNombre}
                    </div>
                </div>
            </div>

            <p className="text-sm text-muted-foreground mb-3">
                {expanded || !shouldTruncate
                    ? propuesta.mensaje
                    : `${propuesta.mensaje.slice(0, 150)}...`}
                {shouldTruncate && (
                    <Button
                        variant="link"
                        size="sm"
                        className="px-1 h-auto"
                        onClick={() => setExpanded(!expanded)}
                    >
                        {expanded ? "Leer menos" : "Leer mas"}
                    </Button>
                )}
            </p>

            <div className="flex items-center gap-4 text-sm text-muted-foreground mb-3">
                <Badge variant="secondary" className="font-semibold">
                    {formatPresupuesto(propuesta.precioPropuesto, undefined, propuesta.monedaId)}
                </Badge>
                {propuesta.tiempoEstimadoDias && (
                    <div className="flex items-center gap-1">
                        <Clock className="h-4 w-4" />
                        <span>{propuesta.tiempoEstimadoDias} dias</span>
                    </div>
                )}
                <div className="flex items-center gap-1">
                    <Calendar className="h-4 w-4" />
                    <span>{formatRelativeDate(propuesta.fechaCreacion)}</span>
                </div>
            </div>

            {!readonly && (
                <div className="flex gap-2 pt-3 border-t">
                    <Button variant="ghost" size="sm" onClick={onVerPerfil}>
                        Ver Perfil
                    </Button>
                    <Button variant="default" size="sm" onClick={onAceptar}>
                        Aceptar
                    </Button>
                    <Button
                        variant="ghost"
                        size="sm"
                        className="text-red-400 hover:text-red-300"
                        onClick={onRechazar}
                    >
                        Rechazar
                    </Button>
                </div>
            )}
        </Card>
    )
}
