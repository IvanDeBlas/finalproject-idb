import type { LucideIcon } from "lucide-react"
import { Card } from "@/components/ui/card"
import { cn } from "@/lib/utils"

interface MetricasKpiCardProps {
    label: string
    value: number
    icon: LucideIcon
    iconColor: string
    iconBgClass: string
    monetario?: boolean
    monedaNombre?: string | null
}

export function MetricasKpiCard({
    label,
    value,
    icon: Icon,
    iconColor,
    iconBgClass,
    monetario = false,
    monedaNombre,
}: MetricasKpiCardProps) {
    return (
        <Card className="bg-[#151525] border-[#334155] p-5 transition-colors duration-150 hover:border-[#a855f7]/30">
            <div
                className={cn(
                    "w-10 h-10 rounded-lg flex items-center justify-center mb-3",
                    iconBgClass
                )}
            >
                <Icon className="w-5 h-5" style={{ color: iconColor }} />
            </div>

            {monetario ? (
                <p className="text-3xl font-bold text-[#f59e0b] tabular-nums">
                    {value.toLocaleString()}
                    <span className="text-lg text-[#64748b] ml-1">{monedaNombre ?? "EUR"}</span>
                </p>
            ) : (
                <p className="text-3xl font-bold text-white tabular-nums">
                    {value.toLocaleString()}
                </p>
            )}

            <p className="text-sm text-[#94a3b8] mt-1">{label}</p>
        </Card>
    )
}
