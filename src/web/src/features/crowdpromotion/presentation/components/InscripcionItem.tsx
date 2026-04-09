import { type FC, useState } from "react"
import { Card } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"
import { Button } from "@/components/ui/button"
import { Music, ChevronDown, ChevronUp, Calendar, Clock, Ban, UserX } from "lucide-react"
import { INSCRIPCION_ESTADO } from "@shared/constants"
import type { MiInscripcion } from "../../domain"
import { InscripcionEstadoBadge } from "./InscripcionEstadoBadge"
import { CodigoReferidoBlock } from "./CodigoReferidoBlock"
import { UrlTrackingBlock } from "./UrlTrackingBlock"
import { TareaItem } from "./TareaItem"

interface InscripcionItemProps {
    inscripcion: MiInscripcion
    defaultExpanded?: boolean
}

function formatComision(
    porcentaje: number | null,
    fija: number | null,
    moneda: string | null
): string {
    if (porcentaje != null && porcentaje > 0) {
        return `${porcentaje}% por backing referido`
    }
    if (fija != null && fija > 0) {
        return `${fija} ${moneda ?? "EUR"}/conversion`
    }
    return "Sin comision definida"
}

function formatDate(isoDate: string): string {
    return new Date(isoDate).toLocaleDateString("es-ES", {
        day: "2-digit",
        month: "short",
        year: "numeric",
    })
}

export const InscripcionItem: FC<InscripcionItemProps> = ({
    inscripcion,
    defaultExpanded = false,
}) => {
    const [isExpanded, setIsExpanded] = useState(defaultExpanded)
    const isAprobado = inscripcion.estado === INSCRIPCION_ESTADO.APROBADO

    return (
        <Card className="bg-[#151525] border-[#334155] p-5 flex flex-col gap-4">
            {/* Header */}
            <div className="flex items-start justify-between gap-3">
                <div className="flex-1 min-w-0">
                    <h3 className="text-base font-semibold text-white leading-tight">
                        {inscripcion.programaTitulo}
                    </h3>
                    <p className="text-sm text-[#94a3b8] mt-0.5">
                        <Music className="w-3.5 h-3.5 mr-1.5 inline text-[#64748b]" aria-hidden="true" />
                        {inscripcion.artistaNombre}
                    </p>
                </div>
                <InscripcionEstadoBadge estado={inscripcion.estado} />
            </div>

            {/* Approved content */}
            {isAprobado && (
                <>
                    <Separator className="bg-[#334155]" />
                    <div className="space-y-3">
                        {inscripcion.codigoReferido && (
                            <CodigoReferidoBlock codigo={inscripcion.codigoReferido} />
                        )}
                        {inscripcion.urlTrackingPersonalizada && (
                            <UrlTrackingBlock url={inscripcion.urlTrackingPersonalizada} />
                        )}
                        <div className="flex flex-wrap items-center gap-x-6 gap-y-1 text-xs text-[#64748b]">
                            <span>
                                Comision: <span className="text-[#a855f7] font-medium">
                                    {formatComision(inscripcion.importeComisionPorcentaje, inscripcion.importeComisionFija, inscripcion.monedaNombre)}
                                </span>
                            </span>
                            <span>
                                <Calendar className="w-3 h-3 mr-1 inline" aria-hidden="true" />
                                Inscrito: {formatDate(inscripcion.fechaAlta)}
                            </span>
                        </div>
                    </div>
                    {inscripcion.tareas.length > 0 && (
                        <>
                            <Button
                                variant="ghost"
                                size="sm"
                                className="text-[#a855f7] hover:text-purple-400 self-start p-0 h-auto text-xs"
                                onClick={() => setIsExpanded(!isExpanded)}
                                aria-expanded={isExpanded}
                            >
                                {isExpanded ? (
                                    <>
                                        <ChevronUp className="w-3.5 h-3.5 mr-1" aria-hidden="true" />
                                        Ocultar detalles
                                    </>
                                ) : (
                                    <>
                                        <ChevronDown className="w-3.5 h-3.5 mr-1" aria-hidden="true" />
                                        Ver tareas y estadisticas
                                    </>
                                )}
                            </Button>
                            <div
                                className="overflow-hidden transition-all duration-300"
                                style={{ maxHeight: isExpanded ? "1000px" : "0px" }}
                            >
                                <Separator className="bg-[#334155] mb-3" />
                                <div className="space-y-1">
                                    {inscripcion.tareas.map((tarea) => (
                                        <TareaItem key={tarea.id} tarea={tarea} />
                                    ))}
                                </div>
                            </div>
                        </>
                    )}
                </>
            )}

            {/* Pending content */}
            {inscripcion.estado === INSCRIPCION_ESTADO.PENDIENTE && (
                <>
                    <Separator className="bg-[#334155]" />
                    <div className="flex items-center gap-2 text-amber-400">
                        <Clock className="w-4 h-4 shrink-0" aria-hidden="true" />
                        <div>
                            <p className="text-sm font-medium">Esperando aprobacion del artista</p>
                            <p className="text-xs text-[#64748b] mt-0.5">
                                Recibiras una notificacion cuando tu solicitud sea revisada
                            </p>
                        </div>
                    </div>
                </>
            )}

            {/* Blocked content */}
            {inscripcion.estado === INSCRIPCION_ESTADO.BLOQUEADO && (
                <>
                    <Separator className="bg-[#334155]" />
                    <div className="flex items-center gap-2 text-red-400">
                        <Ban className="w-4 h-4 shrink-0" aria-hidden="true" />
                        <p className="text-sm">Tu acceso a este programa ha sido bloqueado</p>
                    </div>
                </>
            )}

            {/* Dado de baja content */}
            {inscripcion.estado === INSCRIPCION_ESTADO.DADO_DE_BAJA && (
                <>
                    <Separator className="bg-[#334155]" />
                    <div className="flex items-center gap-2 text-[#64748b]">
                        <UserX className="w-4 h-4 shrink-0" aria-hidden="true" />
                        <div>
                            <p className="text-sm">Tu inscripcion fue dada de baja</p>
                            {inscripcion.fechaBaja && (
                                <p className="text-xs mt-0.5">
                                    Fecha de baja: {formatDate(inscripcion.fechaBaja)}
                                </p>
                            )}
                        </div>
                    </div>
                </>
            )}
        </Card>
    )
}
