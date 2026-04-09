import type { FC } from "react"
import { Badge } from "@/components/ui/badge"
import type { TareaResumen } from "../../domain"

interface TareaItemProps {
    tarea: TareaResumen
}

export const TareaItem: FC<TareaItemProps> = ({ tarea }) => {
    const recompensaText = tarea.importeRecompensa
        ? `${tarea.importeRecompensa.toFixed(2)} ${tarea.monedaNombre ?? "EUR"}`
        : null

    const repetibilidadText = tarea.esRepetible
        ? `Repetible x${tarea.maxRepeticiones ?? "N"}`
        : "No repetible"

    return (
        <div className="flex items-start justify-between gap-3 py-2">
            <div className="flex-1 min-w-0">
                <div className="flex items-center gap-2 mb-1">
                    <Badge
                        variant="outline"
                        className="bg-[#1e1e38] text-[#94a3b8] border-[#334155] text-xs shrink-0"
                    >
                        {tarea.tipoEventoPromoNombre}
                    </Badge>
                    <span className="text-sm text-white truncate">{tarea.titulo}</span>
                </div>
                <p className="text-xs text-[#64748b]">
                    {recompensaText && (
                        <>Dinero: {recompensaText} | </>
                    )}
                    {repetibilidadText}
                </p>
            </div>
        </div>
    )
}
