import { Card, CardContent } from "@/components/ui/card"
import { UserCheck, Clock, Activity, TrendingUp, Ban } from "lucide-react"
import type { PromoProgramaResumen } from "@shared/types"

interface PromoProgramaKpiCardsProps {
    resumen: PromoProgramaResumen
}

const kpis = [
    {
        key: "aprobados" as const,
        label: "Promotores aprobados",
        icon: UserCheck,
        color: "text-emerald-400",
        bgColor: "bg-emerald-500/10",
        getValue: (r: PromoProgramaResumen) => r.totalPromotoresAprobados,
    },
    {
        key: "pendientes" as const,
        label: "Promotores pendientes",
        icon: Clock,
        color: "text-yellow-400",
        bgColor: "bg-yellow-500/10",
        getValue: (r: PromoProgramaResumen) => r.totalPromotoresPendientes,
    },
    {
        key: "eventos" as const,
        label: "Total eventos",
        icon: Activity,
        color: "text-blue-400",
        bgColor: "bg-blue-500/10",
        getValue: (r: PromoProgramaResumen) => r.totalEventos,
    },
    {
        key: "valor" as const,
        label: "Valor generado",
        icon: TrendingUp,
        color: "text-purple-400",
        bgColor: "bg-purple-500/10",
        getValue: (r: PromoProgramaResumen) =>
            new Intl.NumberFormat("es-ES", { style: "currency", currency: "EUR" }).format(r.valorTotalGenerado),
    },
    {
        key: "bloqueados" as const,
        label: "Promotores bloqueados",
        icon: Ban,
        color: "text-red-400",
        bgColor: "bg-red-500/10",
        getValue: (r: PromoProgramaResumen) =>
            (r as PromoProgramaResumen & { totalPromotoresBloqueados?: number }).totalPromotoresBloqueados ?? 0,
    },
]

export function PromoProgramaKpiCards({ resumen }: PromoProgramaKpiCardsProps) {
    return (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-5 gap-4">
            {kpis.map((kpi) => {
                const Icon = kpi.icon
                return (
                    <Card key={kpi.key} className="bg-[#1a1a2e] border-zinc-800">
                        <CardContent className="p-4">
                            <div className="flex items-center gap-3">
                                <div className={`rounded-lg p-2 ${kpi.bgColor}`}>
                                    <Icon className={`h-5 w-5 ${kpi.color}`} />
                                </div>
                                <div>
                                    <p className="text-xs text-zinc-400">{kpi.label}</p>
                                    <p className={`text-xl font-bold ${kpi.color}`}>
                                        {kpi.getValue(resumen)}
                                    </p>
                                </div>
                            </div>
                        </CardContent>
                    </Card>
                )
            })}
        </div>
    )
}
