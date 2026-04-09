/* eslint-disable @next/next/no-img-element */
"use client"

import { Music } from "lucide-react"
import { Card, CardContent } from "@/components/ui"
import { calculatePercentage, formatCurrencyWithSymbol } from "@shared/utils/format"
import type { CampaniaListItem } from "@shared/types"
import { CampaniaStatusBadge } from "./CampaniaStatusBadge"
import { CampaniaActions } from "./CampaniaActions"

interface CampaniaListCardProps {
    campania: CampaniaListItem
    onEdit: (id: string) => void
    onView: (id: string) => void
    onPublish: (id: string) => void
    onDelete: (id: string) => void
}

export function CampaniaListCard({
    campania,
    onEdit,
    onView,
    onPublish,
    onDelete,
}: CampaniaListCardProps) {
    const porcentaje = calculatePercentage(
        campania.importePledgedActual,
        campania.importeObjetivo
    )

    return (
        <Card
            className="bg-card border-border hover:border-primary/50 transition-colors cursor-pointer"
            onClick={() => onView(campania.id)}
        >
            <CardContent className="p-4 sm:p-6">
                <div className="flex flex-col sm:flex-row items-start sm:items-center gap-4 sm:gap-6">
                    {/* Image */}
                    {campania.imagenPrincipalUrl ? (
                        <img
                            src={campania.imagenPrincipalUrl}
                            alt={campania.titulo}
                            className="w-full sm:w-24 h-48 sm:h-24 object-cover rounded-lg"
                        />
                    ) : (
                        <div className="w-full sm:w-24 h-48 sm:h-24 rounded-lg bg-muted/20 flex items-center justify-center">
                            <Music className="w-8 h-8 text-muted-foreground" />
                        </div>
                    )}

                    {/* Info */}
                    <div className="flex-1 min-w-0 w-full">
                        <h3 className="text-lg font-bold text-foreground mb-2 truncate">
                            {campania.titulo}
                        </h3>

                        <div className="flex flex-wrap items-center gap-3 mb-3">
                            <CampaniaStatusBadge
                                estadoId={campania.estadoCampaniaId}
                            />
                            <span className="text-sm text-muted-foreground">
                                {formatCurrencyWithSymbol(
                                    campania.importePledgedActual
                                )}{" "}
                                de{" "}
                                {formatCurrencyWithSymbol(
                                    campania.importeObjetivo
                                )}
                            </span>
                        </div>

                        {/* Progress bar */}
                        <div className="w-full h-2 bg-muted/20 rounded-full overflow-hidden">
                            <div
                                className="h-full bg-gradient-to-r from-pink-500 to-purple-600 rounded-full transition-all duration-600 ease-out"
                                style={{
                                    width: `${Math.min(porcentaje, 100)}%`,
                                }}
                                role="progressbar"
                                aria-valuenow={porcentaje}
                                aria-valuemin={0}
                                aria-valuemax={100}
                                aria-label={`Progreso de financiacion: ${porcentaje}%`}
                            />
                        </div>
                        <p className="text-xs text-muted-foreground mt-1">
                            {porcentaje}% financiado
                        </p>
                    </div>

                    {/* Actions */}
                    <CampaniaActions
                        campaniaId={campania.id}
                        estado={campania.estadoCampaniaId}
                        onEdit={onEdit}
                        onView={onView}
                        onPublish={onPublish}
                        onDelete={onDelete}
                    />
                </div>
            </CardContent>
        </Card>
    )
}
