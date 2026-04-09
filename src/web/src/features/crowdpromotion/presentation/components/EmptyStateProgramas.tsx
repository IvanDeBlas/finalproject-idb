import type { FC } from "react"
import { Button } from "@/components/ui/button"
import { Megaphone, SearchX } from "lucide-react"

interface EmptyStateProgramasProps {
    hasActiveFilters: boolean
    onClearFilters: () => void
}

export const EmptyStateProgramas: FC<EmptyStateProgramasProps> = ({
    hasActiveFilters,
    onClearFilters,
}) => {
    if (hasActiveFilters) {
        return (
            <div className="text-center py-12" role="status">
                <SearchX className="w-10 h-10 text-[#64748b] mx-auto mb-4" aria-hidden="true" />
                <p className="text-sm text-[#94a3b8] mb-4">
                    No se encontraron programas con los filtros seleccionados
                </p>
                <Button
                    variant="outline"
                    size="sm"
                    onClick={onClearFilters}
                    className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]"
                >
                    Limpiar filtros
                </Button>
            </div>
        )
    }

    return (
        <div className="text-center py-16" role="status">
            <div className="mx-auto w-20 h-20 rounded-full bg-gradient-to-br from-pink-500/20 to-purple-600/20 flex items-center justify-center mb-6">
                <Megaphone className="w-10 h-10 text-[#a855f7]" aria-hidden="true" />
            </div>
            <h3 className="text-lg font-semibold text-white mb-2">
                No hay programas disponibles en este momento
            </h3>
            <p className="text-sm text-[#94a3b8]">
                Vuelve mas tarde para encontrar nuevas oportunidades
            </p>
        </div>
    )
}
