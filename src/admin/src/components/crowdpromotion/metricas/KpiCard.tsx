import { Card } from "@/components/ui/card"
import type { LucideIcon } from "lucide-react"

interface KpiCardProps {
    label: string
    value: string | number
    icon: LucideIcon
    iconBgClass: string
    iconColorClass: string
    valueColorClass?: string
    suffix?: string
    note?: string
    layout?: "default" | "horizontal"
    className?: string
}

export function KpiCard({
    label,
    value,
    icon: Icon,
    iconBgClass,
    iconColorClass,
    valueColorClass = "text-white",
    suffix,
    note,
    layout = "default",
    className,
}: KpiCardProps) {
    const formattedValue =
        typeof value === "number" ? value.toLocaleString("es-ES") : value

    if (layout === "horizontal") {
        return (
            <Card className={`bg-[#151525] border-zinc-800 p-4 ${className ?? ""}`}>
                <div className="flex items-center gap-4">
                    <div className={`flex items-center justify-center w-10 h-10 rounded-lg ${iconBgClass}`}>
                        <Icon className={`w-5 h-5 ${iconColorClass}`} />
                    </div>
                    <div className="flex-1 min-w-0">
                        <p className={`text-xl font-bold ${valueColorClass}`}>
                            {formattedValue}
                            {suffix && <span className="text-sm font-normal text-[#94a3b8] ml-1">{suffix}</span>}
                        </p>
                        <p className="text-xs text-[#64748b]">{label}</p>
                        {note && <p className="text-[10px] text-[#475569]">{note}</p>}
                    </div>
                </div>
            </Card>
        )
    }

    return (
        <Card className={`bg-[#151525] border-zinc-800 p-4 ${className ?? ""}`}>
            <div className={`flex items-center justify-center w-10 h-10 rounded-lg mb-3 ${iconBgClass}`}>
                <Icon className={`w-5 h-5 ${iconColorClass}`} />
            </div>
            <p className={`text-2xl font-bold ${valueColorClass}`}>
                {formattedValue}
                {suffix && <span className="text-sm font-normal text-[#94a3b8] ml-1">{suffix}</span>}
            </p>
            <p className="text-xs text-[#64748b] mt-1">{label}</p>
            {note && <p className="text-[10px] text-[#475569]">{note}</p>}
        </Card>
    )
}
