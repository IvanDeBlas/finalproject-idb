"use client"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { PriorityBadge } from "../shared/PriorityBadge"
import { formatCurrency } from "@/lib/utils"
import type { PlantillaProyectoNecesidad } from "@shared/types"

interface NecesidadesSectionProps {
    necesidades: PlantillaProyectoNecesidad[]
}

function groupByFase(
    necesidades: PlantillaProyectoNecesidad[]
): Record<string, PlantillaProyectoNecesidad[]> {
    return necesidades.reduce(
        (acc, nec) => {
            const fase = nec.fase
            if (!acc[fase]) acc[fase] = []
            acc[fase].push(nec)
            return acc
        },
        {} as Record<string, PlantillaProyectoNecesidad[]>
    )
}

export function NecesidadesSection({ necesidades }: NecesidadesSectionProps) {
    const grouped = groupByFase(necesidades)

    return (
        <Card>
            <CardHeader>
                <CardTitle>
                    Necesidades ({necesidades.length})
                </CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
                {Object.entries(grouped).map(([fase, items]) => (
                    <div key={fase}>
                        <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide mb-3">
                            {fase}
                        </h3>
                        <div className="space-y-2">
                            {items
                                .sort((a, b) => a.orden - b.orden)
                                .map((nec) => (
                                    <div
                                        key={nec.id}
                                        className="flex items-start gap-3 p-3 rounded-lg bg-muted/50"
                                    >
                                        <div className="flex-1">
                                            <div className="flex items-center gap-2 mb-1">
                                                <span className="font-medium">{nec.titulo}</span>
                                                <PriorityBadge prioridad={nec.prioridad} />
                                            </div>
                                            <div className="text-sm text-muted-foreground">
                                                {nec.rolProfesional.nombre}
                                            </div>
                                            {(nec.precioMinOrientativo !== undefined ||
                                                nec.precioMaxOrientativo !== undefined) && (
                                                <div className="text-sm text-muted-foreground/60 mt-1">
                                                    {formatCurrency(nec.precioMinOrientativo ?? 0)} -{" "}
                                                    {formatCurrency(nec.precioMaxOrientativo ?? 0)}
                                                </div>
                                            )}
                                        </div>
                                    </div>
                                ))}
                        </div>
                    </div>
                ))}
            </CardContent>
        </Card>
    )
}
