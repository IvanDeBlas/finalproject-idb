import type { FC } from "react"
import { Card } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { UserPlus, CheckCircle, Ban, Music, Calendar, ClipboardList, Loader2 } from "lucide-react"
import { cn } from "@/lib/utils"
import { INSCRIPCION_ESTADO } from "@shared/constants"
import type { ProgramaExplorarItem } from "../../domain"
import { InscripcionEstadoBadge } from "./InscripcionEstadoBadge"

interface ProgramaCardProps {
    programa: ProgramaExplorarItem
    isSolicitando: boolean
    onSolicitar: (programaId: string) => void
    onVerMisDatos: (programaId: string) => void
    className?: string
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

function formatFechaVigencia(inicio: string | null, fin: string | null): string {
    const fmt = (d: string) => {
        const date = new Date(d)
        return date.toLocaleDateString("es-ES", { day: "2-digit", month: "short", year: "numeric" })
    }
    if (inicio && fin) return `${fmt(inicio)} - ${fmt(fin)}`
    if (inicio) return `Desde ${fmt(inicio)}`
    if (fin) return `Hasta ${fmt(fin)}`
    return "Sin fecha definida"
}

export const ProgramaCard: FC<ProgramaCardProps> = ({
    programa,
    isSolicitando,
    onSolicitar,
    onVerMisDatos,
    className,
}) => {
    const {
        id, titulo, artistaNombre, tipoPromoNombre, numeroTareas,
        importeComisionPorcentaje, importeComisionFija, monedaNombre,
        campaniaTitulo, fechaInicio, fechaFin, miEstado,
    } = programa

    const esBloqueado = miEstado === INSCRIPCION_ESTADO.BLOQUEADO
    const esAprobado = miEstado === INSCRIPCION_ESTADO.APROBADO

    return (
        <Card
            className={cn(
                "bg-[#151525] border-[#334155] p-5 flex flex-col gap-3 transition-colors",
                esBloqueado
                    ? "opacity-60 cursor-not-allowed"
                    : esAprobado
                        ? "hover:border-green-800/50"
                        : "hover:border-[#a855f7]/50",
                className
            )}
            role="article"
            aria-label={`Programa: ${titulo}. Estado: ${miEstado ?? "Disponible"}`}
            aria-disabled={esBloqueado}
        >
            {/* Badges */}
            <div className="flex items-center gap-2">
                <Badge variant="outline" className="bg-[#1e1e38] text-[#94a3b8] border-[#334155] text-xs font-medium">
                    {tipoPromoNombre}
                </Badge>
                <Badge variant="outline" className="bg-[#1e1e38] text-[#64748b] border-[#334155] text-xs">
                    <ClipboardList className="w-3 h-3 mr-1" aria-hidden="true" />
                    {numeroTareas} {numeroTareas === 1 ? "tarea" : "tareas"}
                </Badge>
            </div>

            {/* Title and artist */}
            <div>
                <h3 className="text-base font-semibold text-white leading-tight">{titulo}</h3>
                <p className="text-sm text-[#94a3b8] mt-0.5">
                    <Music className="w-3.5 h-3.5 mr-1.5 inline text-[#64748b]" aria-hidden="true" />
                    {artistaNombre}
                </p>
            </div>

            <Separator className="bg-[#334155]" />

            {/* Metrics */}
            <div className="space-y-1.5">
                <div className="flex items-start gap-2">
                    <span className="text-xs text-[#64748b] w-20 shrink-0">Comision:</span>
                    <span className="text-sm font-semibold text-[#a855f7]">
                        {formatComision(importeComisionPorcentaje, importeComisionFija, monedaNombre)}
                    </span>
                </div>
                {campaniaTitulo && (
                    <div className="flex items-start gap-2">
                        <span className="text-xs text-[#64748b] w-20 shrink-0">Campana:</span>
                        <span className="text-sm text-white">{campaniaTitulo}</span>
                    </div>
                )}
                <div className="flex items-start gap-2">
                    <span className="text-xs text-[#64748b] w-20 shrink-0">Vigencia:</span>
                    <span className="text-xs text-[#64748b]">
                        <Calendar className="w-3 h-3 mr-1 inline" aria-hidden="true" />
                        {formatFechaVigencia(fechaInicio, fechaFin)}
                    </span>
                </div>
            </div>

            <Separator className="bg-[#334155]" />

            {/* Action zone */}
            {miEstado === null && (
                <Button
                    className="w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-10"
                    onClick={() => onSolicitar(id)}
                    disabled={isSolicitando}
                >
                    {isSolicitando ? (
                        <>
                            <Loader2 className="w-4 h-4 mr-2 animate-spin" aria-hidden="true" />
                            Enviando...
                        </>
                    ) : (
                        <>
                            <UserPlus className="w-4 h-4 mr-2" aria-hidden="true" />
                            Solicitar inscripcion
                        </>
                    )}
                </Button>
            )}
            {miEstado === INSCRIPCION_ESTADO.PENDIENTE && (
                <div className="flex flex-col gap-1">
                    <InscripcionEstadoBadge estado="Pendiente" className="self-start" />
                    <p className="text-xs text-[#64748b]">El artista revisara tu solicitud</p>
                </div>
            )}
            {miEstado === INSCRIPCION_ESTADO.APROBADO && (
                <div className="flex flex-col gap-1.5">
                    <div className="flex items-center gap-2">
                        <InscripcionEstadoBadge estado="Aprobado" className="self-start" />
                    </div>
                    <Button
                        variant="ghost"
                        size="sm"
                        className="text-[#a855f7] hover:text-purple-400 p-0 h-auto text-xs self-start"
                        onClick={() => onVerMisDatos(id)}
                    >
                        <CheckCircle className="w-3 h-3 mr-1" aria-hidden="true" />
                        Ver mis datos →
                    </Button>
                </div>
            )}
            {miEstado === INSCRIPCION_ESTADO.BLOQUEADO && (
                <p className="text-xs text-red-400 flex items-center gap-1.5">
                    <Ban className="w-3.5 h-3.5 shrink-0" aria-hidden="true" />
                    No puedes inscribirte en este programa
                </p>
            )}
            {miEstado === INSCRIPCION_ESTADO.DADO_DE_BAJA && (
                <div className="flex flex-col gap-1">
                    <InscripcionEstadoBadge estado="DadoDeBaja" className="self-start" />
                    <p className="text-xs text-[#64748b]">Tu inscripcion fue dada de baja</p>
                </div>
            )}
        </Card>
    )
}
