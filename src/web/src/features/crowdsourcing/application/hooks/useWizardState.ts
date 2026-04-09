import { useState, useCallback, useEffect } from "react"
import type { PlantillaProyecto } from "../../domain"

interface BudgetData {
    min?: number
    max?: number
}

export function useWizardState(template: PlantillaProyecto | undefined) {
    const [selectedNecesidades, setSelectedNecesidades] = useState<Set<string>>(new Set())
    const [presupuestos, setPresupuestos] = useState<Map<string, BudgetData>>(new Map())
    const [proyectoId, setProyectoId] = useState<string | undefined>()

    // Auto-select "Alta" priority needs when template loads
    useEffect(() => {
        if (!template) return

        const altaPriorityIds = template.necesidades
            .filter((n) => n.prioridad === "Alta")
            .map((n) => n.id)

        setSelectedNecesidades(new Set(altaPriorityIds))

        const initialBudgets = new Map<string, BudgetData>()
        altaPriorityIds.forEach((id) => {
            const nec = template.necesidades.find((n) => n.id === id)
            if (nec) {
                initialBudgets.set(id, {
                    min: nec.precioMinOrientativo,
                    max: nec.precioMaxOrientativo,
                })
            }
        })
        setPresupuestos(initialBudgets)
    }, [template])

    const toggleNecesidad = useCallback(
        (id: string) => {
            setSelectedNecesidades((prev) => {
                const next = new Set(prev)
                if (next.has(id)) {
                    next.delete(id)
                    setPresupuestos((p) => {
                        const newMap = new Map(p)
                        newMap.delete(id)
                        return newMap
                    })
                } else {
                    next.add(id)
                    const nec = template?.necesidades.find((n) => n.id === id)
                    if (nec) {
                        setPresupuestos((p) =>
                            new Map(p).set(id, {
                                min: nec.precioMinOrientativo,
                                max: nec.precioMaxOrientativo,
                            })
                        )
                    }
                }
                return next
            })
        },
        [template]
    )

    const updatePresupuesto = useCallback((id: string, min: number, max: number) => {
        setPresupuestos((prev) => new Map(prev).set(id, { min, max }))
    }, [])

    const getTotals = useCallback(() => {
        let minTotal = 0
        let maxTotal = 0

        selectedNecesidades.forEach((id) => {
            const budget = presupuestos.get(id)
            minTotal += budget?.min ?? 0
            maxTotal += budget?.max ?? 0
        })

        return {
            minTotal,
            maxTotal,
            selectedCount: selectedNecesidades.size,
            totalCount: template?.necesidades.length ?? 0,
        }
    }, [selectedNecesidades, presupuestos, template])

    const reset = useCallback(() => {
        setSelectedNecesidades(new Set())
        setPresupuestos(new Map())
        setProyectoId(undefined)
    }, [])

    return {
        selectedNecesidades,
        presupuestos,
        proyectoId,
        toggleNecesidad,
        updatePresupuesto,
        setProyectoId,
        getTotals,
        reset,
    }
}
