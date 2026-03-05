import { MousePointerClick, UserPlus, ShoppingCart, Euro, TrendingUp, Wallet } from "lucide-react"
import { KpiCard } from "./KpiCard"
import { formatTasaConversion } from "@shared/utils/format"
import type { ProgramaMetricasKpis } from "@shared/types/crowdpromotion"

interface KpiCardsGridProps {
    kpis: ProgramaMetricasKpis
}

export function KpiCardsGrid({ kpis }: KpiCardsGridProps) {
    const moneda = kpis.monedaNombre ?? "EUR"

    return (
        <div>
            <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 mb-4">
                <KpiCard
                    label="Clicks totales"
                    value={kpis.totalClicks}
                    icon={MousePointerClick}
                    iconBgClass="bg-blue-950/50"
                    iconColorClass="text-[#3b82f6]"
                />
                <KpiCard
                    label="Registros"
                    value={kpis.totalSignups}
                    icon={UserPlus}
                    iconBgClass="bg-green-950/50"
                    iconColorClass="text-[#10b981]"
                />
                <KpiCard
                    label="Backings"
                    value={kpis.totalConversiones}
                    icon={ShoppingCart}
                    iconBgClass="bg-purple-950/50"
                    iconColorClass="text-[#a855f7]"
                />
                <KpiCard
                    label="Valor generado"
                    value={kpis.valorTotalGenerado.toLocaleString("es-ES", {
                        minimumFractionDigits: 0,
                        maximumFractionDigits: 2,
                    })}
                    icon={Euro}
                    iconBgClass="bg-amber-950/50"
                    iconColorClass="text-[#f59e0b]"
                    valueColorClass="text-[#f59e0b]"
                    suffix={moneda}
                />
            </div>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-8">
                <KpiCard
                    label="Tasa de conversion"
                    value={formatTasaConversion(kpis.tasaConversion)}
                    icon={TrendingUp}
                    iconBgClass="bg-cyan-950/50"
                    iconColorClass="text-[#06b6d4]"
                    layout="horizontal"
                    note="conversiones / clicks"
                />
                <KpiCard
                    label="Comisiones"
                    value={kpis.comisionesTotales.toLocaleString("es-ES", {
                        minimumFractionDigits: 0,
                        maximumFractionDigits: 2,
                    })}
                    icon={Wallet}
                    iconBgClass="bg-pink-950/50"
                    iconColorClass="text-[#ec4899]"
                    layout="horizontal"
                    suffix={moneda}
                />
            </div>
        </div>
    )
}
