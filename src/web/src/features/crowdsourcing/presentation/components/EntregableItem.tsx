import { FC } from "react"
import { Button } from "@/components/ui/button"
import { ExternalLink, CheckCircle, XCircle, Calendar } from "lucide-react"
import { ESTADO_ACUERDO, ESTADO_ENTREGABLE } from "@shared/constants"
import { EstadoEntregableBadge } from "./EstadoEntregableBadge"
import type { Entregable, RolAcuerdo } from "../../domain"

interface EntregableItemProps {
    entregable: Entregable
    miRol: RolAcuerdo
    estadoAcuerdoId: number
    onAprobar?: (entregable: Entregable) => void
    onRechazar?: (entregable: Entregable) => void
}

function formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString("es-ES", {
        day: "2-digit",
        month: "short",
        year: "numeric",
    })
}

export const EntregableItem: FC<EntregableItemProps> = ({
    entregable,
    miRol,
    estadoAcuerdoId,
    onAprobar,
    onRechazar,
}) => {
    const canReview =
        miRol === "Artista" &&
        estadoAcuerdoId === ESTADO_ACUERDO.ACTIVO &&
        entregable.estadoEntregableId === ESTADO_ENTREGABLE.ENTREGADO

    return (
        <div className="py-3 space-y-2">
            <div className="flex items-start justify-between gap-2">
                <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2 flex-wrap">
                        <span className="text-sm font-medium text-slate-200">
                            {entregable.titulo}
                        </span>
                        <EstadoEntregableBadge
                            estadoId={entregable.estadoEntregableId}
                            estadoNombre={entregable.estadoEntregableNombre}
                        />
                    </div>
                    {entregable.descripcion && (
                        <p className="text-xs text-slate-400 mt-1">
                            {entregable.descripcion}
                        </p>
                    )}
                </div>
                {entregable.urlRecurso && (
                    <a
                        href={entregable.urlRecurso}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="text-blue-400 hover:text-blue-300 shrink-0"
                        aria-label="Ver recurso"
                    >
                        <ExternalLink className="h-4 w-4" />
                    </a>
                )}
            </div>

            <div className="flex items-center gap-3 text-xs text-slate-500">
                <span className="flex items-center gap-1">
                    <Calendar className="h-3 w-3" />
                    {formatDate(entregable.fechaCreacion)}
                </span>
                {entregable.fechaAprobacion && (
                    <span>Aprobado: {formatDate(entregable.fechaAprobacion)}</span>
                )}
            </div>

            {entregable.comentarioAprobacion && (
                <p className="text-xs text-green-400 bg-green-900/20 rounded px-2 py-1">
                    {entregable.comentarioAprobacion}
                </p>
            )}

            {entregable.comentarioRechazo && (
                <p className="text-xs text-red-400 bg-red-900/20 rounded px-2 py-1">
                    {entregable.comentarioRechazo}
                </p>
            )}

            {canReview && (
                <div className="flex gap-2 pt-1">
                    <Button
                        size="sm"
                        variant="outline"
                        className="h-7 text-xs gap-1 border-green-700 text-green-400 hover:bg-green-900/30"
                        onClick={() => onAprobar?.(entregable)}
                    >
                        <CheckCircle className="h-3 w-3" />
                        Aprobar
                    </Button>
                    <Button
                        size="sm"
                        variant="outline"
                        className="h-7 text-xs gap-1 border-red-700 text-red-400 hover:bg-red-900/30"
                        onClick={() => onRechazar?.(entregable)}
                    >
                        <XCircle className="h-3 w-3" />
                        Rechazar
                    </Button>
                </div>
            )}
        </div>
    )
}
