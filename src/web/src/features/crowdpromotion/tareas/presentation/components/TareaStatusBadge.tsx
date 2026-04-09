import { FC } from "react"
import { Clock, CheckCircle, XCircle, type LucideIcon } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { cn } from "@/lib/utils"
import type { EstadoTareaPromo } from "../../domain"

interface TareaStatusBadgeProps {
    estadoTareaId: EstadoTareaPromo | undefined
    className?: string
}

interface BadgeConfig {
    label: string
    classes: string
    Icon?: LucideIcon
}

const BADGE_CONFIG: Record<number, BadgeConfig> = {
    2: {
        label: "PENDIENTE",
        classes: "bg-amber-950/50 text-amber-400 border border-amber-800/50",
        Icon: Clock,
    },
    3: {
        label: "VALIDADA",
        classes: "bg-green-950/50 text-green-400 border border-green-800/50",
        Icon: CheckCircle,
    },
    4: {
        label: "RECHAZADA",
        classes: "bg-red-950/50 text-red-400 border border-red-800/50",
        Icon: XCircle,
    },
}

const DEFAULT_CONFIG: BadgeConfig = {
    label: "No completada",
    classes: "bg-[#1e1e38] text-[#94a3b8] border border-[#334155]",
}

export const TareaStatusBadge: FC<TareaStatusBadgeProps> = ({
    estadoTareaId,
    className,
}) => {
    const config = estadoTareaId
        ? (BADGE_CONFIG[estadoTareaId] ?? DEFAULT_CONFIG)
        : DEFAULT_CONFIG

    return (
        <Badge
            className={cn(
                "text-xs font-medium inline-flex items-center gap-1 px-2.5 py-1",
                config.classes,
                className
            )}
            role="status"
            aria-label={`Estado: ${config.label}`}
        >
            {config.Icon && (
                <config.Icon className="w-3 h-3" aria-hidden="true" />
            )}
            {config.label}
        </Badge>
    )
}
