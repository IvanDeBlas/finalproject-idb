import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import type { PromoProgramaResumen } from "@shared/types"

interface PromoProgramaResumenTabProps {
    resumen: PromoProgramaResumen
}

export function PromoProgramaResumenTab({ resumen }: PromoProgramaResumenTabProps) {
    const metricas = [
        {
            label: "Promotores aprobados",
            value: resumen.totalPromotoresAprobados,
            descripcion: "Promotores que han sido aprobados para participar en el programa",
        },
        {
            label: "Promotores pendientes",
            value: resumen.totalPromotoresPendientes,
            descripcion: "Promotores que estan esperando aprobacion",
        },
        {
            label: "Total de eventos",
            value: resumen.totalEventos,
            descripcion: "Numero total de eventos de promocion registrados (clicks, shares, posts, etc.)",
        },
        {
            label: "Total de conversiones",
            value: resumen.totalConversiones,
            descripcion: "Eventos que resultaron en una accion de conversion (backing)",
        },
        {
            label: "Valor total generado",
            value: new Intl.NumberFormat("es-ES", { style: "currency", currency: "EUR" }).format(
                resumen.valorTotalGenerado
            ),
            descripcion: "Valor monetario total generado por las promociones de este programa",
        },
    ]

    return (
        <Card className="bg-[#1a1a2e] border-zinc-800">
            <CardHeader>
                <CardTitle className="text-base text-white">Resumen de metricas</CardTitle>
            </CardHeader>
            <CardContent>
                <div className="divide-y divide-zinc-800">
                    {metricas.map((m) => (
                        <div key={m.label} className="flex items-center justify-between py-3 first:pt-0 last:pb-0">
                            <div>
                                <p className="text-sm font-medium text-white">{m.label}</p>
                                <p className="text-xs text-zinc-500">{m.descripcion}</p>
                            </div>
                            <span className="text-lg font-bold text-white">{m.value}</span>
                        </div>
                    ))}
                </div>
            </CardContent>
        </Card>
    )
}
