import { FC, useMemo } from "react"
import { NecesidadItem } from "./NecesidadItem"
import type { PlantillaProyectoNecesidad } from "../../domain"

interface NecesidadListProps {
    necesidades: PlantillaProyectoNecesidad[]
    selectedNecesidades: Map<string, { min?: number; max?: number }>
    onToggleNecesidad: (id: string) => void
    onBudgetChange: (id: string, min: number, max: number) => void
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

export const NecesidadList: FC<NecesidadListProps> = ({
    necesidades,
    selectedNecesidades,
    onToggleNecesidad,
    onBudgetChange,
}) => {
    const grouped = useMemo(() => groupByFase(necesidades), [necesidades])

    return (
        <div className="space-y-8">
            {Object.entries(grouped).map(([fase, items]) => (
                <div key={fase} className="mb-8">
                    <h2 className="text-lg md:text-xl font-semibold text-white mb-4 uppercase tracking-wide flex items-center gap-2">
                        <span className="w-1 h-6 bg-gradient-to-b from-pink-500 to-purple-600 rounded-full" />
                        {fase}
                    </h2>
                    <div className="space-y-3">
                        {items.map((necesidad) => (
                            <NecesidadItem
                                key={necesidad.id}
                                necesidad={necesidad}
                                isSelected={selectedNecesidades.has(necesidad.id)}
                                presupuestoMin={selectedNecesidades.get(necesidad.id)?.min}
                                presupuestoMax={selectedNecesidades.get(necesidad.id)?.max}
                                onToggle={onToggleNecesidad}
                                onBudgetChange={onBudgetChange}
                            />
                        ))}
                    </div>
                </div>
            ))}
        </div>
    )
}
