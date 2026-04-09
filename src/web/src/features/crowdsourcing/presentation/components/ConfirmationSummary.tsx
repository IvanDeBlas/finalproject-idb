import { FC, useMemo } from "react"
import { Card } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { cn } from "@/lib/utils"
import { formatCurrency } from "@/features/campanias/application/utils"
import type { PlantillaProyecto, PlantillaProyectoNecesidad } from "../../domain"

interface ConfirmationSummaryProps {
    template: PlantillaProyecto
    selectedNecesidades: PlantillaProyectoNecesidad[]
    presupuestos: Map<string, { min?: number; max?: number }>
    minTotal: number
    maxTotal: number
}

function groupByFase(
    necesidades: PlantillaProyectoNecesidad[]
): Record<string, PlantillaProyectoNecesidad[]> {
    return necesidades.reduce(
        (acc, nec) => {
            const fase = nec.fase || "Sin fase"
            if (!acc[fase]) acc[fase] = []
            acc[fase].push(nec)
            return acc
        },
        {} as Record<string, PlantillaProyectoNecesidad[]>
    )
}

function getBadgeClasses(prioridad: string): string {
    switch (prioridad) {
        case "Alta":
            return "border border-red-500 bg-red-500/10 text-red-400"
        case "Media":
            return "border border-yellow-500 bg-yellow-500/10 text-yellow-400"
        default:
            return "border border-slate-500 bg-slate-500/10 text-slate-400"
    }
}

export const ConfirmationSummary: FC<ConfirmationSummaryProps> = ({
    template,
    selectedNecesidades,
    presupuestos,
    minTotal,
    maxTotal,
}) => {
    const grouped = useMemo(() => groupByFase(selectedNecesidades), [selectedNecesidades])

    return (
        <Card className="bg-[#0f1729] border-[#334155] p-4 md:p-8 max-w-4xl mx-auto">
            <h2 className="text-2xl font-semibold text-white mb-1">
                {template.nombre}
            </h2>
            <p className="text-base text-[#94a3b8] mb-6">
                Necesidades seleccionadas: {selectedNecesidades.length} de{" "}
                {template.necesidades.length}
            </p>

            <div className="bg-gradient-to-r from-purple-900/30 to-pink-900/30 border border-[#a855f7] rounded-lg p-6 mb-6">
                <h3 className="text-lg font-semibold text-white mb-3">
                    Resumen Presupuestario
                </h3>
                <div className="space-y-2">
                    <div className="flex justify-between">
                        <span className="text-white">Min total:</span>
                        <span className="text-xl font-bold text-white">
                            {formatCurrency(minTotal)}
                        </span>
                    </div>
                    <div className="flex justify-between">
                        <span className="text-white">Max total:</span>
                        <span className="text-xl font-bold text-white">
                            {formatCurrency(maxTotal)}
                        </span>
                    </div>
                    <div className="flex justify-between border-t border-[#a855f7]/30 pt-2">
                        <span className="text-[#94a3b8]">Rango promedio:</span>
                        <span className="text-base text-[#94a3b8]">
                            ~{formatCurrency(Math.round((minTotal + maxTotal) / 2))}
                        </span>
                    </div>
                </div>
            </div>

            <h3 className="text-lg font-semibold text-white mb-4">
                Necesidades Seleccionadas
            </h3>
            {Object.entries(grouped).map(([fase, items]) => (
                <div key={fase} className="mb-4">
                    <h4 className="text-sm font-semibold text-[#94a3b8] uppercase tracking-wide mb-2">
                        {fase}
                    </h4>
                    <ul className="space-y-2">
                        {items.map((nec) => {
                            const budget = presupuestos.get(nec.id)
                            const necMin = budget?.min ?? nec.precioMinOrientativo ?? 0
                            const necMax = budget?.max ?? nec.precioMaxOrientativo ?? 0

                            return (
                                <li
                                    key={nec.id}
                                    className="flex items-center gap-2 text-base"
                                >
                                    <span className="text-[#a855f7]">&bull;</span>
                                    <span className="text-white">{nec.titulo}</span>
                                    <span className="text-[#94a3b8]">
                                        ({formatCurrency(necMin)} - {formatCurrency(necMax)})
                                    </span>
                                    <Badge
                                        className={cn(
                                            "text-xs ml-auto",
                                            getBadgeClasses(nec.prioridad)
                                        )}
                                    >
                                        {nec.prioridad}
                                    </Badge>
                                </li>
                            )
                        })}
                    </ul>
                </div>
            ))}
        </Card>
    )
}
