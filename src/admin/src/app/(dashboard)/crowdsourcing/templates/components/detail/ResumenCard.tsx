"use client"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { formatCurrency } from "@/lib/utils"
import type { PlantillaResumen } from "@shared/types"

interface ResumenCardProps {
    resumen: PlantillaResumen
}

export function ResumenCard({ resumen }: ResumenCardProps) {
    const totalNecesidades =
        resumen.cantidadNecesidadesAlta +
        resumen.cantidadNecesidadesMedia +
        resumen.cantidadNecesidadesBaja

    return (
        <Card>
            <CardHeader>
                <CardTitle>Resumen</CardTitle>
            </CardHeader>
            <CardContent>
                <div className="grid grid-cols-2 gap-4">
                    <div>
                        <p className="text-sm text-muted-foreground">Precio estimado</p>
                        <p className="text-lg font-semibold">
                            {formatCurrency(resumen.precioMinTotal)} -{" "}
                            {formatCurrency(resumen.precioMaxTotal)}
                        </p>
                    </div>
                    <div>
                        <p className="text-sm text-muted-foreground">Total necesidades</p>
                        <p className="text-lg font-semibold">{totalNecesidades}</p>
                    </div>
                    <div>
                        <p className="text-sm text-muted-foreground">Por prioridad</p>
                        <div className="flex gap-3 mt-1 text-sm">
                            <span className="text-red-500">
                                Alta: {resumen.cantidadNecesidadesAlta}
                            </span>
                            <span className="text-yellow-500">
                                Media: {resumen.cantidadNecesidadesMedia}
                            </span>
                            <span className="text-blue-500">
                                Baja: {resumen.cantidadNecesidadesBaja}
                            </span>
                        </div>
                    </div>
                </div>
            </CardContent>
        </Card>
    )
}
