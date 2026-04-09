import { FC } from "react"
import { Badge } from "@/components/ui/badge"
import { ESTADO_ACUERDO } from "@shared/constants"

interface EstadoAcuerdoBadgeProps {
    estadoId: number
    estadoNombre: string
    className?: string
}

const ESTADO_STYLES: Record<number, string> = {
    [ESTADO_ACUERDO.ACTIVO]: "bg-blue-900/30 border-blue-700 text-blue-300",
    [ESTADO_ACUERDO.COMPLETADO]: "bg-green-900/30 border-green-600 text-green-300",
    [ESTADO_ACUERDO.CANCELADO]: "bg-gray-900/30 border-gray-600 text-gray-400",
}

export const EstadoAcuerdoBadge: FC<EstadoAcuerdoBadgeProps> = ({
    estadoId,
    estadoNombre,
    className = "",
}) => {
    const styles = ESTADO_STYLES[estadoId] ?? "bg-gray-900/30 border-gray-600 text-gray-400"

    return (
        <Badge
            className={`border text-xs font-medium px-2.5 py-0.5 rounded-full ${styles} ${className}`}
            aria-label={`Estado: ${estadoNombre}`}
        >
            {estadoNombre}
        </Badge>
    )
}
