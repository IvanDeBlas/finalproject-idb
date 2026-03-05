"use client"

import { Card, CardContent, CardHeader } from "@/components/ui/card"
import { ModalidadBadge } from "../shared/ModalidadBadge"
import { Music, MapPin, Calendar, Banknote } from "lucide-react"
import { formatPresupuesto, formatDate } from "@shared/utils/format"
import type { NecesidadCrowdsourcing } from "@shared/types"

interface NecesidadDetailsCardProps {
    necesidad: NecesidadCrowdsourcing
}

export function NecesidadDetailsCard({ necesidad }: NecesidadDetailsCardProps) {
    return (
        <Card>
            <CardHeader>
                <h2 className="text-lg font-semibold">Detalles</h2>
            </CardHeader>
            <CardContent className="space-y-6">
                {necesidad.descripcion && (
                    <div>
                        <h3 className="text-sm font-medium text-muted-foreground mb-2">
                            Descripcion
                        </h3>
                        <p className="whitespace-pre-wrap">{necesidad.descripcion}</p>
                    </div>
                )}

                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                    <div className="flex items-start gap-3">
                        <Music className="h-5 w-5 text-muted-foreground mt-0.5" />
                        <div>
                            <p className="text-sm text-muted-foreground">Tipo</p>
                            <p className="font-medium">{necesidad.tipoNecesidadNombre}</p>
                        </div>
                    </div>

                    <div className="flex items-start gap-3">
                        <div className="mt-0.5">
                            <ModalidadBadge modalidadId={necesidad.modalidadTrabajoId} />
                        </div>
                    </div>

                    <div className="flex items-start gap-3">
                        <Banknote className="h-5 w-5 text-muted-foreground mt-0.5" />
                        <div>
                            <p className="text-sm text-muted-foreground">Presupuesto</p>
                            <p className="font-medium">
                                {formatPresupuesto(
                                    necesidad.presupuestoMin,
                                    necesidad.presupuestoMax,
                                    necesidad.monedaId
                                )}
                            </p>
                        </div>
                    </div>

                    {(necesidad.ubicacionCiudad || necesidad.ubicacionPais) && (
                        <div className="flex items-start gap-3">
                            <MapPin className="h-5 w-5 text-muted-foreground mt-0.5" />
                            <div>
                                <p className="text-sm text-muted-foreground">Ubicacion</p>
                                <p className="font-medium">
                                    {[necesidad.ubicacionCiudad, necesidad.ubicacionPais]
                                        .filter(Boolean)
                                        .join(", ")}
                                </p>
                            </div>
                        </div>
                    )}

                    {necesidad.fechaLimitePropuestas && (
                        <div className="flex items-start gap-3">
                            <Calendar className="h-5 w-5 text-muted-foreground mt-0.5" />
                            <div>
                                <p className="text-sm text-muted-foreground">
                                    Fecha limite propuestas
                                </p>
                                <p className="font-medium">
                                    {formatDate(necesidad.fechaLimitePropuestas)}
                                </p>
                            </div>
                        </div>
                    )}

                    {necesidad.fechaInicioPrevista && (
                        <div className="flex items-start gap-3">
                            <Calendar className="h-5 w-5 text-muted-foreground mt-0.5" />
                            <div>
                                <p className="text-sm text-muted-foreground">
                                    Fecha inicio prevista
                                </p>
                                <p className="font-medium">
                                    {formatDate(necesidad.fechaInicioPrevista)}
                                </p>
                            </div>
                        </div>
                    )}

                    <div className="flex items-start gap-3">
                        <Calendar className="h-5 w-5 text-muted-foreground mt-0.5" />
                        <div>
                            <p className="text-sm text-muted-foreground">Creada</p>
                            <p className="font-medium">
                                {formatDate(necesidad.fechaCreacion)}
                            </p>
                        </div>
                    </div>
                </div>
            </CardContent>
        </Card>
    )
}
