import type { FC } from "react"
import { Badge } from "@/components/ui/badge"
import { CheckCircle, Clock, Ban, UserX } from "lucide-react"
import { cn } from "@/lib/utils"
import { INSCRIPCION_ESTADO, INSCRIPCION_ESTADO_LABELS } from "@shared/constants"
import type { InscripcionEstado } from "../../domain"

interface InscripcionEstadoBadgeProps {
    estado: InscripcionEstado
    className?: string
}

const ESTADO_CONFIG: Record<string, { classes: string; icon: FC<{ className?: string }> }> = {
    [INSCRIPCION_ESTADO.PENDIENTE]: {
        classes: "bg-amber-950/50 text-amber-400 border border-amber-800/50",
        icon: Clock,
    },
    [INSCRIPCION_ESTADO.APROBADO]: {
        classes: "bg-green-950/50 text-green-400 border border-green-800/50",
        icon: CheckCircle,
    },
    [INSCRIPCION_ESTADO.BLOQUEADO]: {
        classes: "bg-red-950/50 text-red-400 border border-red-800/50",
        icon: Ban,
    },
    [INSCRIPCION_ESTADO.DADO_DE_BAJA]: {
        classes: "bg-[#1e1e38] text-[#64748b] border border-[#334155]",
        icon: UserX,
    },
}

export const InscripcionEstadoBadge: FC<InscripcionEstadoBadgeProps> = ({ estado, className }) => {
    const config = ESTADO_CONFIG[estado]
    if (!config) return null

    const Icon = config.icon
    const label = INSCRIPCION_ESTADO_LABELS[estado] ?? estado

    return (
        <Badge
            className={cn(
                "text-xs font-medium inline-flex items-center gap-1 px-2.5 py-1",
                config.classes,
                className
            )}
            role="status"
            aria-label={`Estado: ${label}`}
        >
            <Icon className="w-3 h-3" aria-hidden="true" />
            {label}
        </Badge>
    )
}
