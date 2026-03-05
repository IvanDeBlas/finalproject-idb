import { FC } from "react"
import { DollarSign } from "lucide-react"
import type { MisTareasItem } from "../../domain"

interface TareaRewardInfoProps {
    tarea: MisTareasItem
}

function formatFechaCorta(fecha: string): string {
    return new Date(fecha).toLocaleDateString("es-ES", {
        day: "2-digit",
        month: "short",
        year: "numeric",
    })
}

function formatRecompensa(tarea: MisTareasItem): string {
    if (tarea.importeRecompensa !== undefined) {
        return `${tarea.importeRecompensa} ${tarea.monedaNombre ?? "EUR"} por ejecucion`
    }
    if (tarea.puntosRecompensa !== undefined) {
        return `${tarea.puntosRecompensa} puntos`
    }
    return "Sin recompensa definida"
}

export const TareaRewardInfo: FC<TareaRewardInfoProps> = ({ tarea }) => {
    const sinRecompensa =
        tarea.importeRecompensa === undefined && tarea.puntosRecompensa === undefined

    return (
        <div className="flex flex-col gap-1.5">
            <div className="flex items-center gap-2">
                <DollarSign
                    className="w-3.5 h-3.5 text-[#64748b] shrink-0"
                    aria-hidden="true"
                />
                <span className="text-xs text-[#64748b]">Recompensa:</span>
                <span
                    className={
                        sinRecompensa
                            ? "text-sm text-[#64748b]"
                            : "text-sm font-semibold text-[#10b981]"
                    }
                >
                    {formatRecompensa(tarea)}
                </span>
            </div>

            {tarea.esRepetible &&
                tarea.miEstado !== undefined &&
                tarea.miEstado.vecesCompletada > 0 && (
                    <div className="flex items-center gap-2 text-xs text-[#64748b]">
                        <span>
                            Completada: {tarea.miEstado.vecesCompletada} de{" "}
                            {tarea.maxRepeticiones ?? "N"} veces
                        </span>
                        {tarea.miEstado.fechaUltimaCompletada && (
                            <>
                                <span className="text-[#334155]">|</span>
                                <span>Ultima: </span>
                                <time dateTime={tarea.miEstado.fechaUltimaCompletada}>
                                    {formatFechaCorta(tarea.miEstado.fechaUltimaCompletada)}
                                </time>
                            </>
                        )}
                    </div>
                )}
        </div>
    )
}
