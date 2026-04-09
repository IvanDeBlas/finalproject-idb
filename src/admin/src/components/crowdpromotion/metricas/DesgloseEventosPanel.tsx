import { Card, CardHeader, CardTitle } from "@/components/ui/card"
import type { ProgramaMetricasKpis } from "@shared/types/crowdpromotion"

interface DesgloseEventosPanelProps {
    kpis: ProgramaMetricasKpis
}

const DESGLOSE_ITEMS = [
    { label: "Clicks", field: "totalClicks" as const, color: "bg-[#3b82f6]" },
    { label: "Page Views", field: "totalPageViews" as const, color: "bg-[#64748b]" },
    { label: "Signups", field: "totalSignups" as const, color: "bg-[#10b981]" },
    { label: "Conversiones", field: "totalConversiones" as const, color: "bg-[#a855f7]" },
]

export function DesgloseEventosPanel({ kpis }: DesgloseEventosPanelProps) {
    const total =
        kpis.totalClicks + kpis.totalPageViews + kpis.totalSignups + kpis.totalConversiones

    return (
        <Card className="bg-[#151525] border-zinc-800">
            <CardHeader className="pb-3">
                <CardTitle className="text-sm font-medium text-[#94a3b8]">
                    Desglose por tipo
                </CardTitle>
            </CardHeader>
            <div className="px-6 pb-6 space-y-4">
                {DESGLOSE_ITEMS.map((item) => {
                    const value = kpis[item.field]
                    const percentage = total > 0 ? (value / total) * 100 : 0

                    return (
                        <div key={item.field}>
                            <div className="flex items-center justify-between mb-1.5">
                                <div className="flex items-center gap-2">
                                    <div className={`w-2.5 h-2.5 rounded-full ${item.color}`} />
                                    <span className="text-xs text-[#94a3b8]">{item.label}</span>
                                </div>
                                <span className="text-xs text-white font-medium">
                                    {value.toLocaleString("es-ES")}
                                </span>
                            </div>
                            <div className="h-1.5 bg-[#1e1e38] rounded-full overflow-hidden">
                                <div
                                    className={`h-full rounded-full ${item.color} transition-[width] duration-500 ease-out`}
                                    style={{ width: `${percentage}%` }}
                                    data-testid={`bar-${item.field}`}
                                />
                            </div>
                        </div>
                    )
                })}
            </div>
        </Card>
    )
}
