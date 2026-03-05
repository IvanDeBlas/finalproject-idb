import { Badge } from "@/components/ui/badge"

interface PromoProgramaStatusBadgeProps {
    esActivo: boolean
}

export function PromoProgramaStatusBadge({ esActivo }: PromoProgramaStatusBadgeProps) {
    return esActivo ? (
        <Badge className="bg-emerald-500/20 text-emerald-400 border-emerald-500/30 hover:bg-emerald-500/20">
            ACTIVO
        </Badge>
    ) : (
        <Badge className="bg-zinc-500/20 text-zinc-400 border-zinc-500/30 hover:bg-zinc-500/20">
            INACTIVO
        </Badge>
    )
}
