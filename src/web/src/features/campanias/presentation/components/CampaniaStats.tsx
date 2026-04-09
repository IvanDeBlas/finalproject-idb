import { Users, Clock } from "lucide-react"
import { cn } from "@/lib/utils"
import { calcularDiasRestantes, calcularPorcentaje, formatCurrency } from "../../application/utils"
import { CampaniaProgress } from "./CampaniaProgress"

interface CampaniaStatsProps {
    importeObjetivo: number
    importePledgedActual: number
    monedaId: number
    fechaFin?: string
    backersCount?: number
    className?: string
}

export function CampaniaStats({
    importeObjetivo,
    importePledgedActual,
    monedaId,
    fechaFin,
    backersCount = 0,
    className,
}: CampaniaStatsProps) {
    const porcentaje = calcularPorcentaje(importePledgedActual, importeObjetivo)
    const diasRestantes = fechaFin ? calcularDiasRestantes(fechaFin) : null

    return (
        <div className={cn("space-y-4", className)}>
            {/* Amount row */}
            <div className="flex items-baseline gap-2">
                <span className="text-3xl font-bold text-white">
                    {formatCurrency(importePledgedActual, monedaId)}
                </span>
                <span className="text-xl text-[#94a3b8]">
                    de {formatCurrency(importeObjetivo, monedaId)}
                </span>
            </div>

            {/* Progress bar */}
            <CampaniaProgress
                importeObjetivo={importeObjetivo}
                importePledgedActual={importePledgedActual}
                monedaId={monedaId}
                size="md"
                showLabels={false}
            />

            <p className="text-sm font-semibold text-[#94a3b8]">
                {porcentaje}% financiado
            </p>

            {/* Stats row */}
            <div className="flex items-center gap-6 text-[#94a3b8] flex-wrap">
                <div className="flex items-center gap-2">
                    <Users className="w-4 h-4" />
                    <span className="font-semibold">{backersCount}</span>
                    <span className="text-sm">backers</span>
                </div>

                {diasRestantes !== null && (
                    <div className="flex items-center gap-2">
                        <Clock className="w-4 h-4" />
                        <span className="font-semibold">{diasRestantes}</span>
                        <span className="text-sm">
                            {diasRestantes === 0 ? "finalizada" : "dias restantes"}
                        </span>
                    </div>
                )}
            </div>
        </div>
    )
}
