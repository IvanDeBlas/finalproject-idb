import { FC } from "react"
import { Button } from "@/components/ui/button"
import { Inbox, SearchX } from "lucide-react"

interface EmptyStateNecesidadesProps {
    hasActiveFilters: boolean
    onClearFilters?: () => void
}

export const EmptyStateNecesidades: FC<EmptyStateNecesidadesProps> = ({
    hasActiveFilters,
    onClearFilters,
}) => {
    return (
        <div role="status" className="text-center py-12">
            {hasActiveFilters ? (
                <>
                    <SearchX
                        className="w-12 h-12 text-[#334155] mx-auto mb-4"
                        aria-hidden="true"
                    />
                    <h3 className="text-lg font-semibold text-white mb-2">
                        Sin resultados
                    </h3>
                    <p className="text-sm text-[#94a3b8] mb-6">
                        No hay necesidades que coincidan con tus filtros.
                        Intenta ampliar la busqueda.
                    </p>
                    {onClearFilters && (
                        <Button
                            variant="outline"
                            className="border-[#334155] text-white hover:bg-[#1e2a42]"
                            onClick={onClearFilters}
                        >
                            Limpiar filtros
                        </Button>
                    )}
                </>
            ) : (
                <>
                    <Inbox
                        className="w-12 h-12 text-[#334155] mx-auto mb-4"
                        aria-hidden="true"
                    />
                    <h3 className="text-lg font-semibold text-white mb-2">
                        Sin necesidades abiertas
                    </h3>
                    <p className="text-sm text-[#94a3b8]">
                        Vuelve pronto para ver nuevas oportunidades de trabajo
                    </p>
                </>
            )}
        </div>
    )
}
