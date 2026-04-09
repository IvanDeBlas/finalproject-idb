import { Badge } from "@/components/ui/badge"
import { cn } from "@/lib/utils"
import { mapTipoEventoPromoToLabel, mapTipoEventoPromoToBadgeClass } from "@shared/utils/mappers"
import type { TipoEventoPromo } from "../../domain"

interface EventoTipoBadgeProps {
    tipoEventoId: TipoEventoPromo
    className?: string
}

export function EventoTipoBadge({ tipoEventoId, className }: EventoTipoBadgeProps) {
    return (
        <Badge
            variant="outline"
            className={cn(
                mapTipoEventoPromoToBadgeClass(tipoEventoId),
                "shrink-0",
                className
            )}
        >
            {mapTipoEventoPromoToLabel(tipoEventoId)}
        </Badge>
    )
}
