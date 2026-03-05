import { MousePointerClick, ShoppingCart, Wallet } from "lucide-react"
import { Skeleton } from "@/components/ui/skeleton"
import type { PromotorMetricasKpis } from "../../domain"
import { MetricasKpiCard } from "./MetricasKpiCard"

interface MetricasKpiGridProps {
    kpis: PromotorMetricasKpis
    isLoading?: boolean
}

const KPI_CONFIG = [
    {
        key: "misClicks" as const,
        label: "Mis clicks",
        icon: MousePointerClick,
        iconColor: "#3b82f6",
        iconBgClass: "bg-blue-950/50",
        monetario: false,
    },
    {
        key: "misConversiones" as const,
        label: "Conversiones",
        icon: ShoppingCart,
        iconColor: "#a855f7",
        iconBgClass: "bg-purple-950/50",
        monetario: false,
    },
    {
        key: "miComisionAcumulada" as const,
        label: "Comision",
        icon: Wallet,
        iconColor: "#10b981",
        iconBgClass: "bg-green-950/50",
        monetario: true,
    },
]

export function MetricasKpiGrid({ kpis, isLoading }: MetricasKpiGridProps) {
    if (isLoading) {
        return (
            <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6" aria-busy="true">
                {[0, 1, 2].map(i => (
                    <div key={i} className="bg-[#151525] border border-[#334155] rounded-lg p-5">
                        <Skeleton className="w-10 h-10 rounded-lg mb-3 bg-[#1e1e38]" />
                        <Skeleton className="w-20 h-8 mb-2 bg-[#1e1e38]" />
                        <Skeleton className="w-24 h-4 bg-[#1e1e38]" />
                    </div>
                ))}
            </div>
        )
    }

    return (
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6">
            {KPI_CONFIG.map(config => (
                <MetricasKpiCard
                    key={config.key}
                    label={config.label}
                    value={kpis[config.key]}
                    icon={config.icon}
                    iconColor={config.iconColor}
                    iconBgClass={config.iconBgClass}
                    monetario={config.monetario}
                    monedaNombre={config.monetario ? kpis.monedaNombre : undefined}
                />
            ))}
        </div>
    )
}
