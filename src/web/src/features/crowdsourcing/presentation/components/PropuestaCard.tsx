import { FC } from "react"
import { Card } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { ArrowRight } from "lucide-react"
import { Link } from "react-router-dom"
import { EstadoPropuestaBadge } from "./EstadoPropuestaBadge"
import { RatingBadge } from "./RatingBadge"
import type { MiPropuestaList } from "../../domain/types"
import { ESTADO_PROPUESTA, APP_ROUTES } from "@shared/constants"

interface PropuestaCardProps {
    propuesta: MiPropuestaList
    onRetirar?: (propuesta: MiPropuestaList) => void
    puntuacionMedia?: number
    totalValoraciones?: number
}

function formatFecha(fecha: string): string {
    return new Date(fecha).toLocaleDateString("es-ES", {
        day: "numeric",
        month: "short",
        year: "numeric",
    })
}

export const PropuestaCard: FC<PropuestaCardProps> = ({
    propuesta,
    onRetirar,
    puntuacionMedia,
    totalValoraciones,
}) => {
    return (
        <Card className="bg-[#0f1729] border-[#334155] rounded-xl p-5">
            <div className="flex items-start justify-between mb-3">
                <div className="flex-1 min-w-0">
                    <h3 className="text-base font-bold text-white mb-1 truncate">
                        {propuesta.necesidadTitulo}
                    </h3>
                    <p className="text-sm text-[#94a3b8]">
                        {propuesta.artistaNombre}
                    </p>
                </div>
                <EstadoPropuestaBadge
                    estadoId={propuesta.estadoPropuestaId}
                    estadoNombre={propuesta.estadoPropuestaNombre}
                    className="flex-shrink-0 ml-3"
                />
            </div>

            <div className="space-y-1 mb-4">
                <p className="text-sm text-white">
                    <span className="text-[#64748b]">Precio: </span>
                    <span className="font-semibold text-[#a855f7]">
                        {propuesta.precioPropuesto} {propuesta.monedaNombre}
                    </span>
                </p>
                <div className="flex items-center gap-3">
                    <p className="text-xs text-[#64748b]">
                        Enviada: {formatFecha(propuesta.fechaCreacion)}
                        {propuesta.fechaActualizacion && (
                            <> &middot; Actualizada: {formatFecha(propuesta.fechaActualizacion)}</>
                        )}
                    </p>
                    {puntuacionMedia !== undefined && totalValoraciones !== undefined && (
                        <RatingBadge
                            puntuacionMedia={puntuacionMedia}
                            totalValoraciones={totalValoraciones}
                            variant="compact"
                        />
                    )}
                </div>
            </div>

            <div className="flex justify-end gap-2">
                {propuesta.estadoPropuestaId === ESTADO_PROPUESTA.PENDIENTE && onRetirar && (
                    <Button
                        variant="outline"
                        size="sm"
                        className="border-red-600 text-red-400 hover:bg-red-900/20 hover:text-red-300"
                        onClick={() => onRetirar(propuesta)}
                    >
                        Retirar propuesta
                    </Button>
                )}
                {propuesta.estadoPropuestaId === ESTADO_PROPUESTA.ACEPTADA &&
                    propuesta.acuerdoId && (
                        <Link to={`/crowdsourcing/acuerdos/${propuesta.acuerdoId}`}>
                            <Button
                                size="sm"
                                className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
                            >
                                Ver acuerdo
                                <ArrowRight className="w-4 h-4 ml-1" aria-hidden="true" />
                            </Button>
                        </Link>
                    )}
                <Link to={APP_ROUTES.landing.crowdsourcing.necesidades}>
                    <Button
                        variant="ghost"
                        size="sm"
                        className="text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]"
                    >
                        Ver necesidad
                    </Button>
                </Link>
            </div>
        </Card>
    )
}
