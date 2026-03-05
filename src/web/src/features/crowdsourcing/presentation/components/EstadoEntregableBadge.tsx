import { FC } from "react"
import { Badge } from "@/components/ui/badge"
import { ESTADO_ENTREGABLE } from "@shared/constants"

interface EstadoEntregableBadgeProps {
    estadoId: number
    estadoNombre: string
    className?: string
}

const ESTADO_STYLES: Record<number, string> = {
    [ESTADO_ENTREGABLE.ENTREGADO]: "bg-amber-900/30 border-amber-600 text-amber-300",
    [ESTADO_ENTREGABLE.APROBADO]: "bg-green-900/30 border-green-600 text-green-300",
    [ESTADO_ENTREGABLE.RECHAZADO]: "bg-red-900/30 border-red-600 text-red-300",
}

export const EstadoEntregableBadge: FC<EstadoEntregableBadgeProps> = ({
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
