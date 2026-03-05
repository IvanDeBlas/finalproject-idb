import { useState, useCallback } from "react"
import { Search } from "lucide-react"
import { Input } from "@/components/ui/input"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { useCampanias } from "../../application/useCampanias"
import { useDebounce } from "@/hooks/useDebounce"
import { CampaniaList } from "../components/CampaniaList"
import { CAMPANIA_ESTADOS } from "@/lib/constants"
import type { CampaniaFilters } from "../../domain"

const ESTADO_OPTIONS = [
    { value: "all", label: "Todas" },
    { value: String(CAMPANIA_ESTADOS.PUBLICADA), label: "Activas" },
    { value: String(CAMPANIA_ESTADOS.FINALIZADA), label: "Finalizadas" },
] as const

export default function CampaniasPage() {
    const [searchTerm, setSearchTerm] = useState("")
    const [estadoValue, setEstadoValue] = useState(String(CAMPANIA_ESTADOS.PUBLICADA))

    const debouncedSearch = useDebounce(searchTerm, 300)

    const filters: CampaniaFilters = {
        searchTerm: debouncedSearch || undefined,
        estadoCampaniaId: estadoValue === "all" ? undefined : Number(estadoValue),
    }

    const hasActiveFilters = !!debouncedSearch || estadoValue !== String(CAMPANIA_ESTADOS.PUBLICADA)

    const { data: campanias, isLoading, error, refetch } = useCampanias(filters)

    const campaniasList = campanias ?? []

    const handleClearFilters = useCallback(() => {
        setSearchTerm("")
        setEstadoValue(String(CAMPANIA_ESTADOS.PUBLICADA))
    }, [])

    return (
        <div className="min-h-screen bg-[#1a1a2e]">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                <div className="mb-8">
                    <h1 className="text-3xl font-bold text-white">Campanias</h1>
                    <p className="text-[#94a3b8]">
                        Explora todas las campanias activas y apoya a tus artistas favoritos
                    </p>
                </div>

                {/* Filters */}
                <div className="flex flex-col sm:flex-row gap-4 mb-6">
                    <div className="flex-1 relative">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-[#64748b]" />
                        <Input
                            type="search"
                            placeholder="Buscar campanias..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="pl-10 bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] focus-visible:ring-[#a855f7]"
                            aria-label="Buscar campanias"
                        />
                    </div>

                    <Select value={estadoValue} onValueChange={setEstadoValue}>
                        <SelectTrigger className="w-full sm:w-[180px]" aria-label="Filtrar por estado">
                            <SelectValue placeholder="Estado" />
                        </SelectTrigger>
                        <SelectContent>
                            {ESTADO_OPTIONS.map((option) => (
                                <SelectItem key={option.value} value={option.value}>
                                    {option.label}
                                </SelectItem>
                            ))}
                        </SelectContent>
                    </Select>

                    {!isLoading && (
                        <span className="flex items-center text-sm text-[#94a3b8] whitespace-nowrap">
                            {campaniasList.length} {campaniasList.length === 1 ? "campania" : "campanias"}
                        </span>
                    )}
                </div>

                <CampaniaList
                    campanias={campaniasList}
                    isLoading={isLoading}
                    error={error instanceof Error ? error : null}
                    onRetry={refetch}
                    onClearFilters={handleClearFilters}
                    hasFilters={hasActiveFilters}
                />
            </div>
        </div>
    )
}
