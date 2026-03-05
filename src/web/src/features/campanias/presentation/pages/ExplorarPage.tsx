import { useState, useCallback } from "react"
import { Search } from "lucide-react"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { useCampanias } from "../../application/useCampanias"
import { useDebounce } from "@/hooks/useDebounce"
import { CampaniaList } from "../components/CampaniaList"
import { DEFAULT_PAGE_SIZE } from "@/lib/constants"
import type { CampaniaFilters, PaginationParams } from "../../domain"

export default function ExplorarPage() {
    const [searchTerm, setSearchTerm] = useState("")
    const [pagination, setPagination] = useState<PaginationParams>({
        pageNumber: 1,
        pageSize: DEFAULT_PAGE_SIZE,
    })

    const debouncedSearch = useDebounce(searchTerm, 300)

    const filters: CampaniaFilters = {
        searchTerm: debouncedSearch || undefined,
    }

    const hasActiveFilters = !!debouncedSearch

    const { data: campanias, isLoading, error, refetch } = useCampanias(filters, pagination)

    const handleClearFilters = useCallback(() => {
        setSearchTerm("")
        setPagination({ pageNumber: 1, pageSize: DEFAULT_PAGE_SIZE })
    }, [])

    const handleLoadMore = useCallback(() => {
        setPagination((prev) => ({
            ...prev,
            pageSize: (prev.pageSize ?? DEFAULT_PAGE_SIZE) + DEFAULT_PAGE_SIZE,
        }))
    }, [])

    const campaniasList = campanias ?? []
    const showLoadMore = campaniasList.length >= (pagination.pageSize ?? DEFAULT_PAGE_SIZE)

    return (
        <div className="min-h-screen bg-[#1a1a2e]">
            {/* Search and Filters Header */}
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
                <h1 className="text-3xl sm:text-4xl font-bold text-white mb-2">
                    Descubre proyectos musicales
                </h1>
                <p className="text-lg text-[#94a3b8] mb-8">
                    Apoya a tus artistas favoritos y haz realidad sus suenos
                </p>

                <div className="flex gap-4 mb-8">
                    <div className="flex-1 relative">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-[#64748b]" />
                        <Input
                            type="search"
                            placeholder="Buscar campanas..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="pl-10 bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b] focus:border-[#a855f7] focus-visible:ring-[#a855f7]"
                            aria-label="Buscar campanas"
                        />
                    </div>
                </div>
            </div>

            {/* Campaign Grid */}
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pb-12">
                <CampaniaList
                    campanias={campaniasList}
                    isLoading={isLoading}
                    error={error instanceof Error ? error : null}
                    onRetry={refetch}
                    onClearFilters={handleClearFilters}
                    hasFilters={hasActiveFilters}
                />

                {/* Load More */}
                {!isLoading && !error && showLoadMore && campaniasList.length > 0 && (
                    <div className="flex justify-center mt-8">
                        <Button
                            variant="outline"
                            className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e2a42]"
                            onClick={handleLoadMore}
                        >
                            Cargar mas
                        </Button>
                    </div>
                )}
            </div>
        </div>
    )
}
