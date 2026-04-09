import { FC, useState, useEffect } from "react"
import { Card } from "@/components/ui/card"
import { Checkbox } from "@/components/ui/checkbox"
import { Label } from "@/components/ui/label"
import { Input } from "@/components/ui/input"
import { Badge } from "@/components/ui/badge"
import { cn } from "@/lib/utils"
import { RolProfesionalTooltip } from "./RolProfesionalTooltip"
import type { PlantillaProyectoNecesidad } from "../../domain"

interface NecesidadItemProps {
    necesidad: PlantillaProyectoNecesidad
    isSelected: boolean
    presupuestoMin?: number
    presupuestoMax?: number
    onToggle: (id: string) => void
    onBudgetChange: (id: string, min: number, max: number) => void
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

function getPrioridadLabel(prioridad: string): string {
    switch (prioridad) {
        case "Alta":
            return "ESENCIAL"
        case "Media":
            return "RECOMENDADO"
        default:
            return "OPCIONAL"
    }
}

export const NecesidadItem: FC<NecesidadItemProps> = ({
    necesidad,
    isSelected,
    presupuestoMin,
    presupuestoMax,
    onToggle,
    onBudgetChange,
}) => {
    const [localMin, setLocalMin] = useState<string>("")
    const [localMax, setLocalMax] = useState<string>("")
    const [validationError, setValidationError] = useState<string | null>(null)

    useEffect(() => {
        setLocalMin(presupuestoMin?.toString() ?? necesidad.precioMinOrientativo?.toString() ?? "")
        setLocalMax(presupuestoMax?.toString() ?? necesidad.precioMaxOrientativo?.toString() ?? "")
    }, [presupuestoMin, presupuestoMax, necesidad.precioMinOrientativo, necesidad.precioMaxOrientativo])

    const handleMinChange = (value: string) => {
        setLocalMin(value)
        const numMin = Number(value)
        const numMax = Number(localMax)

        if (value && numMin < 0) {
            setValidationError("El presupuesto debe ser mayor o igual a 0")
            return
        }
        if (value && localMax && numMax < numMin) {
            setValidationError("El maximo debe ser mayor que el minimo")
        } else {
            setValidationError(null)
        }
        if (value && localMax) {
            onBudgetChange(necesidad.id, numMin, numMax)
        }
    }

    const handleMaxChange = (value: string) => {
        setLocalMax(value)
        const numMin = Number(localMin)
        const numMax = Number(value)

        if (value && numMax < 0) {
            setValidationError("El presupuesto debe ser mayor o igual a 0")
            return
        }
        if (value && localMin && numMax < numMin) {
            setValidationError("El maximo debe ser mayor que el minimo")
        } else {
            setValidationError(null)
        }
        if (localMin && value) {
            onBudgetChange(necesidad.id, numMin, numMax)
        }
    }

    return (
        <Card
            className={cn(
                "bg-[#0f1729] border-[#334155] p-4 mb-3 transition-colors duration-150",
                isSelected && "bg-[#1e2a42]"
            )}
        >
            <div className="flex items-start gap-3">
                <Checkbox
                    id={necesidad.id}
                    checked={isSelected}
                    onCheckedChange={() => onToggle(necesidad.id)}
                    aria-label={`Seleccionar ${necesidad.titulo}`}
                    className="mt-1"
                />
                <div className="flex-1 space-y-2">
                    <div className="flex items-center justify-between gap-2">
                        <Label
                            htmlFor={necesidad.id}
                            className="text-base font-medium text-white cursor-pointer"
                        >
                            {necesidad.titulo}
                        </Label>
                        <Badge className={cn("text-xs font-semibold uppercase tracking-wide", getBadgeClasses(necesidad.prioridad))}>
                            {getPrioridadLabel(necesidad.prioridad)}
                        </Badge>
                    </div>

                    <div className="flex items-center gap-1 text-sm text-[#94a3b8]">
                        <span>Rol: {necesidad.rolProfesional.nombre}</span>
                        <RolProfesionalTooltip rol={necesidad.rolProfesional} />
                    </div>

                    {isSelected && (
                        <div className="flex flex-col sm:flex-row items-start sm:items-center gap-2 mt-2">
                            <Label className="text-xs text-[#cbd5e1]">Presupuesto:</Label>
                            <Input
                                type="number"
                                value={localMin}
                                onChange={(e) => handleMinChange(e.target.value)}
                                placeholder={necesidad.precioMinOrientativo?.toString()}
                                className={cn(
                                    "w-24 h-8 bg-[#1a1a2e] border-[#334155] text-white text-sm",
                                    validationError && "border-red-500"
                                )}
                                aria-label="Presupuesto minimo"
                            />
                            <span className="text-[#64748b]">-</span>
                            <Input
                                type="number"
                                value={localMax}
                                onChange={(e) => handleMaxChange(e.target.value)}
                                placeholder={necesidad.precioMaxOrientativo?.toString()}
                                className={cn(
                                    "w-24 h-8 bg-[#1a1a2e] border-[#334155] text-white text-sm",
                                    validationError && "border-red-500"
                                )}
                                aria-label="Presupuesto maximo"
                            />
                            <span className="text-sm text-[#94a3b8]">EUR</span>
                        </div>
                    )}

                    {validationError && (
                        <p className="text-xs text-red-400 mt-1">{validationError}</p>
                    )}
                </div>
            </div>
        </Card>
    )
}
