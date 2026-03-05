import type { FC } from "react"
import { Input } from "@/components/ui/input"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { Button } from "@/components/ui/button"
import { Search, X } from "lucide-react"

interface ProgramaFiltrosProps {
    artistaNombre: string
    tipoPromoId: number | undefined
    tiposPromo: Array<{ id: number; nombre: string }>
    hasActiveFilters: boolean
    onArtistaNombreChange: (value: string) => void
    onTipoPromoChange: (value: number | undefined) => void
    onClearFilters: () => void
}

export const ProgramaFiltros: FC<ProgramaFiltrosProps> = ({
    artistaNombre,
    tipoPromoId,
    tiposPromo,
    hasActiveFilters,
    onArtistaNombreChange,
    onTipoPromoChange,
    onClearFilters,
}) => {
    return (
        <div className="flex flex-wrap items-center gap-3 mb-6" role="search" aria-label="Filtros de programas">
            <div className="relative flex-1 min-w-[200px] max-w-xs">
                <Search className="absolute left-3 top-3 w-4 h-4 text-[#64748b] pointer-events-none" aria-hidden="true" />
                <Input
                    type="search"
                    placeholder="Buscar artista..."
                    value={artistaNombre}
                    onChange={(e) => onArtistaNombreChange(e.target.value)}
                    className="pl-9 bg-[#151525] border-[#334155] text-white h-10 placeholder:text-[#64748b] focus-visible:ring-[#a855f7] focus:border-[#a855f7]"
                    aria-label="Buscar por nombre de artista"
                />
            </div>
            <Select
                value={tipoPromoId !== undefined ? String(tipoPromoId) : "all"}
                onValueChange={(val) => onTipoPromoChange(val === "all" ? undefined : Number(val))}
            >
                <SelectTrigger
                    className="w-[180px] bg-[#151525] border-[#334155] text-white h-10"
                    aria-label="Filtrar por tipo de programa"
                >
                    <SelectValue placeholder="Tipo de programa" />
                </SelectTrigger>
                <SelectContent className="bg-[#151525] border-[#334155]">
                    <SelectItem value="all">Todos los tipos</SelectItem>
                    {tiposPromo.map((tipo) => (
                        <SelectItem key={tipo.id} value={String(tipo.id)}>
                            {tipo.nombre}
                        </SelectItem>
                    ))}
                </SelectContent>
            </Select>
            {hasActiveFilters && (
                <Button
                    variant="ghost"
                    size="sm"
                    className="text-[#64748b] hover:text-white h-10 px-3 text-sm"
                    onClick={onClearFilters}
                    aria-label="Limpiar todos los filtros"
                >
                    <X className="w-3.5 h-3.5 mr-1.5" aria-hidden="true" />
                    Limpiar
                </Button>
            )}
        </div>
    )
}
