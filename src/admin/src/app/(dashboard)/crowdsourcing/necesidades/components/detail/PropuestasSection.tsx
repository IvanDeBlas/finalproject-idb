"use client"

import { Users } from "lucide-react"
import { PropuestaCard } from "./PropuestaCard"
import type { PropuestaCrowdsourcing } from "@shared/types"

interface PropuestasSectionProps {
    propuestas: PropuestaCrowdsourcing[]
    readonly: boolean
    onVerPerfil?: (profesionalId: string) => void
    onAceptar?: (propuestaId: string) => void
    onRechazar?: (propuestaId: string) => void
}

export function PropuestasSection({
    propuestas,
    readonly,
    onVerPerfil,
    onAceptar,
    onRechazar,
}: PropuestasSectionProps) {
    return (
        <div className="space-y-4">
            <div className="flex items-center gap-2">
                <Users className="h-5 w-5" />
                <h2 className="text-lg font-semibold">
                    Propuestas recibidas ({propuestas.length})
                </h2>
            </div>

            {propuestas.length === 0 ? (
                <div className="text-center py-12 text-muted-foreground">
                    <Users className="h-12 w-12 mx-auto mb-3 opacity-40" />
                    <p>Aun no has recibido propuestas</p>
                    <p className="text-sm">Los profesionales podran enviarte propuestas una vez vean tu necesidad</p>
                </div>
            ) : (
                <div className="space-y-3">
                    {propuestas.map((propuesta) => (
                        <PropuestaCard
                            key={propuesta.id}
                            propuesta={propuesta}
                            readonly={readonly}
                            onVerPerfil={() => onVerPerfil?.(propuesta.profesionalId)}
                            onAceptar={() => onAceptar?.(propuesta.id)}
                            onRechazar={() => onRechazar?.(propuesta.id)}
                        />
                    ))}
                </div>
            )}
        </div>
    )
}
