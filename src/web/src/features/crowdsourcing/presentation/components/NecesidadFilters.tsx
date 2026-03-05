import { FC, useState } from "react"
import { Card } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { Separator } from "@/components/ui/separator"
import {
    Collapsible,
    CollapsibleContent,
    CollapsibleTrigger,
} from "@/components/ui/collapsible"
import { SlidersHorizontal, X } from "lucide-react"
import type { NecesidadesPublicasFilter } from "../../domain/types"

interface NecesidadFiltersProps {
    filters: NecesidadesPublicasFilter
    onFilterChange: (partial: Partial<NecesidadesPublicasFilter>) => void
    onClear: () => void
    tiposNecesidad: Array<{ id: number; nombre: string }>
    isMobile?: boolean
}

const MODALIDADES = [
    { value: "all", label: "Todas" },
    { value: "2", label: "Remoto" },
    { value: "1", label: "Presencial" },
    { value: "3", label: "Hibrido" },
]

const FilterContent: FC<
    Omit<NecesidadFiltersProps, "isMobile">
> = ({ filters, onFilterChange, onClear, tiposNecesidad }) => {
    return (
        <>
            <h3 className="text-xs font-semibold text-[#64748b] uppercase tracking-wider mb-2">
                Tipo
            </h3>
            <div
                className="flex flex-wrap gap-2 mb-5"
                role="group"
                aria-label="Filtrar por tipo de necesidad"
            >
                {tiposNecesidad.map((tipo) => {
                    const isSelected = filters.tipoNecesidadId === tipo.id
                    return (
                        <button
                            key={tipo.id}
                            aria-pressed={isSelected}
                            aria-label={`Filtrar por tipo: ${tipo.nombre}`}
                            className={
                                isSelected
                                    ? "px-3 py-1 rounded-full text-sm bg-purple-600/20 border border-[#a855f7] text-white"
                                    : "px-3 py-1 rounded-full text-sm border border-[#334155] text-[#94a3b8] hover:border-[#a855f7] hover:text-white transition-all duration-150"
                            }
                            onClick={() =>
                                onFilterChange({
                                    tipoNecesidadId: isSelected ? undefined : tipo.id,
                                })
                            }
                        >
                            {tipo.nombre}
                        </button>
                    )
                })}
            </div>

            <h3 className="text-xs font-semibold text-[#64748b] uppercase tracking-wider mb-2">
                Modalidad
            </h3>
            <Select
                value={filters.modalidad?.toString() ?? "all"}
                onValueChange={(val) =>
                    onFilterChange({
                        modalidad: val === "all" ? undefined : Number(val),
                    })
                }
            >
                <SelectTrigger className="bg-[#0f1729] border-[#334155] text-white mb-5 w-full">
                    <SelectValue placeholder="Todas" />
                </SelectTrigger>
                <SelectContent className="bg-[#0f1729] border-[#334155]">
                    {MODALIDADES.map((m) => (
                        <SelectItem key={m.value} value={m.value}>
                            {m.label}
                        </SelectItem>
                    ))}
                </SelectContent>
            </Select>

            <h3 className="text-xs font-semibold text-[#64748b] uppercase tracking-wider mb-2">
                Presupuesto
            </h3>
            <div className="flex gap-2 items-center mb-5">
                <Input
                    type="number"
                    placeholder="Min"
                    value={filters.presupuestoMin ?? ""}
                    onChange={(e) =>
                        onFilterChange({
                            presupuestoMin: e.target.value
                                ? Number(e.target.value)
                                : undefined,
                        })
                    }
                    className="bg-[#1a1a2e] border-[#334155] text-white text-sm"
                />
                <span className="text-[#64748b] text-sm">-</span>
                <Input
                    type="number"
                    placeholder="Max"
                    value={filters.presupuestoMax ?? ""}
                    onChange={(e) =>
                        onFilterChange({
                            presupuestoMax: e.target.value
                                ? Number(e.target.value)
                                : undefined,
                        })
                    }
                    className="bg-[#1a1a2e] border-[#334155] text-white text-sm"
                />
            </div>

            <h3 className="text-xs font-semibold text-[#64748b] uppercase tracking-wider mb-2">
                Pais
            </h3>
            <Input
                placeholder="Ej: Espana"
                value={filters.pais ?? ""}
                onChange={(e) =>
                    onFilterChange({
                        pais: e.target.value || undefined,
                    })
                }
                className="bg-[#1a1a2e] border-[#334155] text-white text-sm mb-5"
            />

            <Separator className="my-4 bg-[#334155]" />

            <Button
                variant="ghost"
                className="w-full text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]"
                onClick={onClear}
            >
                <X className="w-4 h-4 mr-2" aria-hidden="true" />
                Limpiar filtros
            </Button>
        </>
    )
}

export const NecesidadFilters: FC<NecesidadFiltersProps> = ({
    filters,
    onFilterChange,
    onClear,
    tiposNecesidad,
    isMobile = false,
}) => {
    const [isOpen, setIsOpen] = useState(false)

    if (isMobile) {
        return (
            <Collapsible open={isOpen} onOpenChange={setIsOpen} className="lg:hidden mb-4">
                <CollapsibleTrigger asChild>
                    <Button
                        variant="outline"
                        className="w-full border-[#334155] text-white flex items-center justify-between mb-2"
                    >
                        <span className="flex items-center gap-2">
                            <SlidersHorizontal className="w-4 h-4" aria-hidden="true" />
                            Filtros
                        </span>
                    </Button>
                </CollapsibleTrigger>
                <CollapsibleContent className="bg-[#0f1729] border border-[#334155] rounded-lg p-4 space-y-1">
                    <FilterContent
                        filters={filters}
                        onFilterChange={onFilterChange}
                        onClear={onClear}
                        tiposNecesidad={tiposNecesidad}
                    />
                </CollapsibleContent>
            </Collapsible>
        )
    }

    return (
        <aside className="w-64 flex-shrink-0 hidden lg:block">
            <Card className="bg-[#0f1729] border-[#334155] p-5 sticky top-24">
                <h2 className="text-base font-semibold text-white mb-4">
                    Filtros
                </h2>
                <FilterContent
                    filters={filters}
                    onFilterChange={onFilterChange}
                    onClear={onClear}
                    tiposNecesidad={tiposNecesidad}
                />
            </Card>
        </aside>
    )
}
