import { FC } from "react"
import { Card } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Clock, Users, Calendar, DollarSign, Wifi, MapPin, GitBranch } from "lucide-react"
import { UrgenciaBadge } from "./UrgenciaBadge"
import type { NecesidadPublicaList } from "../../domain/types"
import { MODALIDAD_TRABAJO } from "@shared/constants"

interface NecesidadCardProps {
    necesidad: NecesidadPublicaList
    onClick?: () => void
    className?: string
}

const MODALIDAD_ICONS: Record<number, typeof Wifi> = {
    [MODALIDAD_TRABAJO.REMOTO]: Wifi,
    [MODALIDAD_TRABAJO.PRESENCIAL]: MapPin,
    [MODALIDAD_TRABAJO.HIBRIDO]: GitBranch,
}

function getFechaRelativa(fechaCreacion: string): string {
    const diffMs = Date.now() - new Date(fechaCreacion).getTime()
    const diffDias = Math.floor(diffMs / 86400000)
    if (diffDias === 0) return "hoy"
    if (diffDias === 1) return "hace 1 dia"
    if (diffDias < 30) return `hace ${diffDias} dias`
    const diffMeses = Math.floor(diffDias / 30)
    if (diffMeses === 1) return "hace 1 mes"
    return `hace ${diffMeses} meses`
}

function getFechaLimiteStyle(fechaLimite: string): string {
    const diasRestantes = Math.ceil(
        (new Date(fechaLimite).getTime() - Date.now()) / 86400000
    )
    if (diasRestantes < 3) return "text-red-400"
    if (diasRestantes < 7) return "text-amber-400"
    return "text-[#94a3b8]"
}

function formatFechaCorta(fecha: string): string {
    return new Date(fecha).toLocaleDateString("es-ES", {
        day: "numeric",
        month: "short",
        year: "numeric",
    })
}

export const NecesidadCard: FC<NecesidadCardProps> = ({
    necesidad,
    onClick,
    className = "",
}) => {
    const ModalidadIcon = MODALIDAD_ICONS[necesidad.modalidadTrabajoId] ?? Wifi

    const presupuestoText =
        necesidad.presupuestoMin !== undefined && necesidad.presupuestoMax !== undefined
            ? `${necesidad.presupuestoMin} - ${necesidad.presupuestoMax} ${necesidad.monedaNombre ?? ""}`
            : necesidad.presupuestoMin !== undefined
                ? `Desde ${necesidad.presupuestoMin} ${necesidad.monedaNombre ?? ""}`
                : "Sin presupuesto"

    return (
        <Card
            role="article"
            tabIndex={0}
            aria-label={`${necesidad.titulo}. Tipo: ${necesidad.tipoNecesidadNombre}. Modalidad: ${necesidad.modalidadTrabajoNombre}. Presupuesto ${presupuestoText}. ${necesidad.numeroPropuestas} propuestas.`}
            onClick={onClick}
            onKeyDown={(e) => {
                if (e.key === "Enter" || e.key === " ") {
                    e.preventDefault()
                    onClick?.()
                }
            }}
            className={`bg-[#0f1729] border border-[#334155] rounded-xl p-5 cursor-pointer transition-all duration-200 ease-out hover:bg-[#1e2a42] hover:border-purple-500/50 hover:shadow-[0_8px_24px_rgba(0,0,0,0.4)] hover:scale-[1.01] ${className}`}
        >
            <div className="flex items-center gap-2 mb-3 flex-wrap">
                <Badge className="bg-purple-900/30 text-purple-300 border border-purple-700 text-xs">
                    {necesidad.tipoNecesidadNombre}
                </Badge>
                <Badge
                    variant="outline"
                    className="border-[#334155] text-[#94a3b8] text-xs flex items-center gap-1"
                >
                    <ModalidadIcon className="w-3 h-3" aria-hidden="true" />
                    {necesidad.modalidadTrabajoNombre}
                </Badge>
                {necesidad.esUrgente && necesidad.fechaLimitePropuestas && (
                    <UrgenciaBadge fechaLimite={necesidad.fechaLimitePropuestas} />
                )}
            </div>

            <h3 className="text-base font-bold text-white mb-2 leading-tight">
                {necesidad.titulo}
            </h3>
            <p className="text-sm text-[#94a3b8] leading-relaxed mb-4 line-clamp-3">
                {necesidad.descripcion}
            </p>

            <div className="flex items-center justify-between text-sm mb-3">
                <span className="text-[#94a3b8] font-medium">
                    {necesidad.artistaNombre}
                </span>
                <span className="text-[#a855f7] font-semibold flex items-center gap-1">
                    <DollarSign className="w-3 h-3" aria-hidden="true" />
                    {presupuestoText}
                </span>
            </div>

            <div className="flex items-center gap-4 text-xs text-[#64748b] mb-2">
                <span className="flex items-center gap-1">
                    <Clock className="w-3 h-3" aria-hidden="true" />
                    {getFechaRelativa(necesidad.fechaCreacion)}
                </span>
                <span className="flex items-center gap-1">
                    <Users className="w-3 h-3" aria-hidden="true" />
                    {necesidad.numeroPropuestas > 0
                        ? `${necesidad.numeroPropuestas} propuesta(s)`
                        : "Sin propuestas aun"}
                </span>
            </div>

            {necesidad.fechaLimitePropuestas && (
                <div
                    className={`flex items-center gap-1 text-xs ${getFechaLimiteStyle(necesidad.fechaLimitePropuestas)}`}
                >
                    <Calendar className="w-3 h-3" aria-hidden="true" />
                    <span
                        aria-label={`Fecha limite: ${formatFechaCorta(necesidad.fechaLimitePropuestas)}`}
                    >
                        Limite: {formatFechaCorta(necesidad.fechaLimitePropuestas)}
                    </span>
                </div>
            )}
        </Card>
    )
}
