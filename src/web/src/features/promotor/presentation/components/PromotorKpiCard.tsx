import type { FC } from "react"
import type { LucideIcon } from "lucide-react"
import { Card, CardContent } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"

interface PromotorKpiCardProps {
    icon: LucideIcon
    iconColorClass: string
    iconBgClass: string
    label: string
    value: string | number
    isLoading?: boolean
}

export const PromotorKpiCard: FC<PromotorKpiCardProps> = ({
    icon: Icon,
    iconColorClass,
    iconBgClass,
    label,
    value,
    isLoading,
}) => {
    return (
        <Card className="bg-[#151525] border-[#334155] p-5">
            <CardContent className="p-0">
                <div className={`w-10 h-10 rounded-lg flex items-center justify-center mb-3 ${iconBgClass}`}>
                    <Icon className={`w-5 h-5 ${iconColorClass}`} aria-hidden="true" />
                </div>
                {isLoading ? (
                    <Skeleton className="h-8 w-16 rounded bg-[#1e1e38]" />
                ) : (
                    <p className="text-3xl font-bold text-white">{value}</p>
                )}
                <p className="text-sm text-[#94a3b8] mt-1">{label}</p>
            </CardContent>
        </Card>
    )
}
