import { FC } from "react"
import { Badge } from "@/components/ui/badge"
import { Clock } from "lucide-react"
import { URGENCIA_DIAS_UMBRAL } from "@shared/constants"

interface UrgenciaBadgeProps {
    fechaLimite: string
    className?: string
}

export const UrgenciaBadge: FC<UrgenciaBadgeProps> = ({
    fechaLimite,
    className = "",
}) => {
    const diasRestantes = Math.ceil(
        (new Date(fechaLimite).getTime() - Date.now()) / 86400000
    )

    if (diasRestantes >= URGENCIA_DIAS_UMBRAL) return null

    return (
        <Badge
            className={`bg-red-900/30 text-red-300 border border-red-700 text-xs flex items-center gap-1 animate-urgency-pulse ${className}`}
            aria-label={`Urgente: menos de ${URGENCIA_DIAS_UMBRAL} dias para el cierre`}
        >
            <Clock className="w-3 h-3" aria-hidden="true" />
            URGENTE
        </Badge>
    )
}
