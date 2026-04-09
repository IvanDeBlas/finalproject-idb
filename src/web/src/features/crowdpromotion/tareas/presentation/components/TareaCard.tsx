import { FC } from "react"
import {
    RefreshCw,
    Play,
    RotateCcw,
    ExternalLink,
    Lock,
    AlertCircle,
    Clock,
    Loader2,
} from "lucide-react"
import { Card } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { cn } from "@/lib/utils"
import { ESTADO_TAREA_PROMO } from "@shared/constants"
import { puedeCompletarTarea } from "@shared/utils/mappers"
import type { MisTareasItem } from "../../domain"
import { TareaStatusBadge } from "./TareaStatusBadge"
import { TareaRewardInfo } from "./TareaRewardInfo"

interface TareaCardProps {
    tarea: MisTareasItem
    onCompletar: (tarea: MisTareasItem, esReenvio: boolean) => void
    isProgramaActivo: boolean
    isSubmitting?: boolean
    className?: string
}

const AccionTarea: FC<{
    tarea: MisTareasItem
    isProgramaActivo: boolean
    isSubmitting: boolean
    onCompletar: (tarea: MisTareasItem, esReenvio: boolean) => void
}> = ({ tarea, isProgramaActivo, isSubmitting, onCompletar }) => {
    if (!isProgramaActivo) {
        return (
            <div className="text-xs text-[#64748b] flex items-center gap-1.5">
                <Lock className="w-3.5 h-3.5" aria-hidden="true" />
                Programa inactivo
            </div>
        )
    }

    const { miEstado } = tarea

    // Rechazada -> Re-enviar
    if (miEstado?.estadoTareaId === ESTADO_TAREA_PROMO.RECHAZADA) {
        return (
            <Button
                variant="outline"
                size="sm"
                className="border-[#a855f7]/50 text-[#a855f7] hover:bg-[#a855f7]/10 hover:border-[#a855f7] h-9 px-4 text-sm"
                onClick={() => onCompletar(tarea, true)}
                disabled={isSubmitting}
                aria-busy={isSubmitting || undefined}
            >
                {isSubmitting ? (
                    <Loader2 className="w-4 h-4 animate-spin mr-1.5" aria-label="Cargando" />
                ) : (
                    <RotateCcw className="w-3.5 h-3.5 mr-1.5" aria-hidden="true" />
                )}
                {isSubmitting ? "Enviando..." : "Re-enviar con nueva prueba"}
            </Button>
        )
    }

    // Pendiente validacion -> Ver prueba
    if (miEstado?.estadoTareaId === ESTADO_TAREA_PROMO.COMPLETADA) {
        return (
            <Button
                variant="ghost"
                size="sm"
                className="text-[#64748b] hover:text-[#94a3b8] h-9 px-4 text-sm"
                asChild
            >
                <a
                    href={miEstado.urlPruebaCompletado}
                    target="_blank"
                    rel="noopener noreferrer"
                >
                    <ExternalLink className="w-3.5 h-3.5 mr-1.5" aria-hidden="true" />
                    Ver prueba enviada
                </a>
            </Button>
        )
    }

    // Validada y no repetible -> sin boton
    if (
        miEstado?.estadoTareaId === ESTADO_TAREA_PROMO.VALIDADA &&
        !tarea.esRepetible
    ) {
        return null
    }

    // No puede completar (limite alcanzado)
    if (!puedeCompletarTarea(tarea)) {
        return (
            <div className="text-xs text-[#64748b] flex items-center gap-1.5">
                <Lock className="w-3.5 h-3.5" aria-hidden="true" />
                Maximo de repeticiones alcanzado
            </div>
        )
    }

    // Puede completar (primera vez o repeticion)
    const label = miEstado ? "Completar de nuevo" : "Completar tarea"
    const Icon = miEstado ? RefreshCw : Play

    return (
        <Button
            size="sm"
            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-9 px-4 text-sm"
            onClick={() => onCompletar(tarea, false)}
            disabled={isSubmitting}
            aria-busy={isSubmitting || undefined}
        >
            {isSubmitting ? (
                <Loader2 className="w-4 h-4 animate-spin mr-1.5" aria-label="Cargando" />
            ) : (
                <Icon className="w-3.5 h-3.5 mr-1.5" aria-hidden="true" />
            )}
            {isSubmitting ? "Enviando..." : label}
        </Button>
    )
}

export const TareaCard: FC<TareaCardProps> = ({
    tarea,
    onCompletar,
    isProgramaActivo,
    isSubmitting = false,
    className,
}) => {
    return (
        <Card
            className={cn(
                "bg-[#151525] border border-[#334155] p-5 flex flex-col gap-3 transition-colors duration-200 hover:border-[#a855f7]/30",
                className
            )}
            role="listitem"
        >
            {/* Top row: badges */}
            <div className="flex items-start justify-between gap-2 flex-wrap">
                <div className="flex items-center gap-2 flex-wrap">
                    <Badge
                        variant="outline"
                        className="bg-[#1e1e38] text-[#94a3b8] border-[#334155] text-xs font-medium"
                    >
                        {tarea.tipoEventoPromoNombre}
                    </Badge>
                    {tarea.esRepetible && (
                        <Badge className="bg-[#1e1e38] text-[#64748b] border border-[#334155] text-xs font-medium inline-flex items-center gap-1">
                            <RefreshCw className="w-3 h-3" aria-hidden="true" />
                            Repetible
                        </Badge>
                    )}
                </div>
                <TareaStatusBadge estadoTareaId={tarea.miEstado?.estadoTareaId} />
            </div>

            {/* Name and description */}
            <div>
                <h3 className="text-base font-semibold text-white leading-tight">
                    {tarea.nombre}
                </h3>
                {tarea.descripcion && (
                    <p className="text-sm text-[#94a3b8] leading-relaxed line-clamp-3 mt-1">
                        {tarea.descripcion}
                    </p>
                )}
            </div>

            <Separator className="bg-[#334155]" />

            {/* Reward info */}
            <TareaRewardInfo tarea={tarea} />

            {/* Rejection block */}
            {tarea.miEstado?.estadoTareaId === ESTADO_TAREA_PROMO.RECHAZADA &&
                tarea.miEstado.comentarioValidacion && (
                    <div
                        className="bg-red-950/20 border border-red-900/40 rounded-lg p-3 flex items-start gap-2.5"
                        role="alert"
                    >
                        <AlertCircle
                            className="w-4 h-4 text-red-400 shrink-0 mt-0.5"
                            aria-hidden="true"
                        />
                        <div className="flex flex-col gap-0.5">
                            <p className="text-xs font-medium text-red-400">
                                Motivo del rechazo:
                            </p>
                            <p className="text-sm text-red-300/80 leading-relaxed">
                                {tarea.miEstado.comentarioValidacion}
                            </p>
                        </div>
                    </div>
                )}

            {/* Pending proof block */}
            {tarea.miEstado?.estadoTareaId === ESTADO_TAREA_PROMO.COMPLETADA && (
                <div className="bg-amber-950/20 border border-amber-900/40 rounded-lg p-3 flex items-center gap-2.5">
                    <Clock
                        className="w-4 h-4 text-amber-400 shrink-0"
                        aria-hidden="true"
                    />
                    <p className="text-xs text-amber-300/80">
                        Enviada:{" "}
                        {tarea.miEstado.fechaUltimaCompletada ? (
                            <time dateTime={tarea.miEstado.fechaUltimaCompletada}>
                                {new Date(
                                    tarea.miEstado.fechaUltimaCompletada
                                ).toLocaleDateString("es-ES", {
                                    day: "2-digit",
                                    month: "short",
                                    year: "numeric",
                                })}
                            </time>
                        ) : (
                            "\u2014"
                        )}{" "}
                        - Pendiente de validacion del artista
                    </p>
                </div>
            )}

            {/* Action */}
            <div className="flex items-center justify-end mt-1">
                <AccionTarea
                    tarea={tarea}
                    isProgramaActivo={isProgramaActivo}
                    isSubmitting={isSubmitting}
                    onCompletar={onCompletar}
                />
            </div>
        </Card>
    )
}
