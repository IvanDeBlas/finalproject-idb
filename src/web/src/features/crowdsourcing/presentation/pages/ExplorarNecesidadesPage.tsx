import { useState, useEffect, useCallback } from "react"
import { useSearchParams, useNavigate } from "react-router-dom"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import {
    Pagination,
    PaginationContent,
    PaginationItem,
    PaginationLink,
    PaginationPrevious,
    PaginationNext,
} from "@/components/ui/pagination"
import { Search, Loader2 } from "lucide-react"
import { useDebounce } from "@/hooks/useDebounce"
import { useNecesidadesPublicas } from "../../application"
import { NecesidadCard } from "../components/NecesidadCard"
import { NecesidadCardSkeleton } from "../components/NecesidadCardSkeleton"
import { NecesidadFilters } from "../components/NecesidadFilters"
import { EmptyStateNecesidades } from "../components/EmptyStateNecesidades"
import type { NecesidadesPublicasFilter, OrderByNecesidades } from "../../domain/types"
import {
    ORDER_BY_NECESIDADES_LABELS,
    APP_ROUTES,
} from "@shared/constants"

const TIPOS_NECESIDAD = [
    { id: 1, nombre: "Produccion" },
    { id: 2, nombre: "Mezcla" },
    { id: 3, nombre: "Mastering" },
    { id: 4, nombre: "Composicion" },
    { id: 5, nombre: "Diseno" },
    { id: 6, nombre: "Video" },
    { id: 7, nombre: "Fotografia" },
    { id: 8, nombre: "Marketing" },
]

const PAGE_SIZE = 12

function parseFiltersFromUrl(searchParams: URLSearchParams): NecesidadesPublicasFilter {
    return {
        search: searchParams.get("search") ?? undefined,
        tipoNecesidadId: searchParams.get("tipo")
            ? Number(searchParams.get("tipo"))
            : undefined,
        modalidad: searchParams.get("modalidad")
            ? Number(searchParams.get("modalidad"))
            : undefined,
        presupuestoMin: searchParams.get("presupuestoMin")
            ? Number(searchParams.get("presupuestoMin"))
            : undefined,
        presupuestoMax: searchParams.get("presupuestoMax")
            ? Number(searchParams.get("presupuestoMax"))
            : undefined,
        pais: searchParams.get("pais") ?? undefined,
        orderBy:
            (searchParams.get("orderBy") as OrderByNecesidades) ?? "recientes",
        page: Number(searchParams.get("page") ?? "1"),
        pageSize: PAGE_SIZE,
    }
}

export default function ExplorarNecesidadesPage() {
    const [searchParams, setSearchParams] = useSearchParams()
    const navigate = useNavigate()
    const filters = parseFiltersFromUrl(searchParams)

    const [searchInput, setSearchInput] = useState(filters.search ?? "")
    const debouncedSearch = useDebounce(searchInput, 300)

    const { data, isLoading, isFetching } = useNecesidadesPublicas({
        ...filters,
        search: debouncedSearch || undefined,
    })

    useEffect(() => {
        setSearchParams((prev) => {
            const next = new URLSearchParams(prev)
            if (debouncedSearch) {
                next.set("search", debouncedSearch)
            } else {
                next.delete("search")
            }
            next.set("page", "1")
            return next
        })
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [debouncedSearch])

    const updateFilter = useCallback(
        (partial: Partial<NecesidadesPublicasFilter>) => {
            setSearchParams((prev) => {
                const next = new URLSearchParams(prev)
                const isPageChange = "page" in partial && Object.keys(partial).length === 1

                for (const [key, value] of Object.entries(partial)) {
                    if (key === "search") continue
                    if (value === undefined || value === null || value === "") {
                        next.delete(key === "tipoNecesidadId" ? "tipo" : key)
                    } else {
                        next.set(
                            key === "tipoNecesidadId" ? "tipo" : key,
                            String(value)
                        )
                    }
                }

                if (!isPageChange) {
                    next.set("page", "1")
                }

                return next
            })
        },
        [setSearchParams]
    )

    const clearFilters = useCallback(() => {
        setSearchParams({})
        setSearchInput("")
    }, [setSearchParams])

    const hasActiveFilters = Boolean(
        filters.search ||
            filters.tipoNecesidadId ||
            filters.modalidad ||
            filters.presupuestoMin ||
            filters.presupuestoMax ||
            filters.pais
    )

    const items = data?.items ?? []
    const totalPages = data?.totalPages ?? 0
    const totalCount = data?.totalCount ?? 0
    const currentPage = filters.page ?? 1

    return (
        <div className="min-h-screen bg-[#1a1a2e]">
            {/* Hero Section */}
            <div className="bg-[#16213e] py-12 px-4">
                <h1 className="text-3xl font-bold text-white mb-2 text-center">
                    Explorar necesidades
                </h1>
                <p className="text-lg text-[#94a3b8] mb-8 text-center">
                    Encuentra oportunidades de trabajo en la industria musical
                </p>
                <div className="max-w-2xl mx-auto flex gap-2">
                    <Input
                        placeholder="Buscar en titulo y descripcion..."
                        value={searchInput}
                        onChange={(e) => setSearchInput(e.target.value)}
                        className="flex-1 bg-[#0f1729] border-[#334155] text-white placeholder:text-[#64748b] h-12 text-base focus:border-[#a855f7]"
                    />
                    <Button className="h-12 px-6 bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700">
                        <Search className="w-4 h-4 mr-2" aria-hidden="true" />
                        Buscar
                    </Button>
                </div>
            </div>

            {/* Content */}
            <div className="max-w-7xl mx-auto px-4 py-8">
                {/* Mobile filters */}
                <NecesidadFilters
                    filters={filters}
                    onFilterChange={updateFilter}
                    onClear={clearFilters}
                    tiposNecesidad={TIPOS_NECESIDAD}
                    isMobile
                />

                <div className="flex gap-8">
                    {/* Desktop sidebar filters */}
                    <NecesidadFilters
                        filters={filters}
                        onFilterChange={updateFilter}
                        onClear={clearFilters}
                        tiposNecesidad={TIPOS_NECESIDAD}
                    />

                    {/* Main content */}
                    <main className="flex-1 min-w-0">
                        <div className="flex items-center justify-between mb-6">
                            <p className="text-sm text-[#94a3b8]">
                                {isFetching && !isLoading ? (
                                    <span className="flex items-center gap-1">
                                        <Loader2 className="w-3 h-3 animate-spin" aria-hidden="true" />
                                        Actualizando...
                                    </span>
                                ) : (
                                    `${totalCount} necesidad(es) encontrada(s)`
                                )}
                            </p>
                            <Select
                                value={filters.orderBy ?? "recientes"}
                                onValueChange={(val) =>
                                    updateFilter({
                                        orderBy: val as OrderByNecesidades,
                                    })
                                }
                            >
                                <SelectTrigger className="w-48 bg-[#0f1729] border-[#334155] text-white">
                                    <SelectValue />
                                </SelectTrigger>
                                <SelectContent className="bg-[#0f1729] border-[#334155]">
                                    {Object.entries(ORDER_BY_NECESIDADES_LABELS).map(
                                        ([value, label]) => (
                                            <SelectItem key={value} value={value}>
                                                {label}
                                            </SelectItem>
                                        )
                                    )}
                                </SelectContent>
                            </Select>
                        </div>

                        {isLoading ? (
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                {Array.from({ length: 6 }).map((_, i) => (
                                    <NecesidadCardSkeleton key={i} />
                                ))}
                            </div>
                        ) : items.length === 0 ? (
                            <EmptyStateNecesidades
                                hasActiveFilters={hasActiveFilters}
                                onClearFilters={clearFilters}
                            />
                        ) : (
                            <>
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-8">
                                    {items.map((necesidad) => (
                                        <NecesidadCard
                                            key={necesidad.id}
                                            necesidad={necesidad}
                                            onClick={() =>
                                                navigate(
                                                    APP_ROUTES.landing.crowdsourcing.necesidadDetail(
                                                        necesidad.id
                                                    )
                                                )
                                            }
                                        />
                                    ))}
                                </div>

                                {totalPages > 1 && (
                                    <Pagination className="mt-8">
                                        <PaginationContent>
                                            {currentPage > 1 && (
                                                <PaginationItem>
                                                    <PaginationPrevious
                                                        onClick={() =>
                                                            updateFilter({
                                                                page: currentPage - 1,
                                                            })
                                                        }
                                                        className="cursor-pointer text-white"
                                                    />
                                                </PaginationItem>
                                            )}
                                            {Array.from(
                                                { length: totalPages },
                                                (_, i) => i + 1
                                            )
                                                .filter(
                                                    (p) =>
                                                        p === 1 ||
                                                        p === totalPages ||
                                                        Math.abs(p - currentPage) <= 1
                                                )
                                                .map((page) => (
                                                    <PaginationItem key={page}>
                                                        <PaginationLink
                                                            isActive={page === currentPage}
                                                            onClick={() =>
                                                                updateFilter({ page })
                                                            }
                                                            className="cursor-pointer text-white"
                                                        >
                                                            {page}
                                                        </PaginationLink>
                                                    </PaginationItem>
                                                ))}
                                            {currentPage < totalPages && (
                                                <PaginationItem>
                                                    <PaginationNext
                                                        onClick={() =>
                                                            updateFilter({
                                                                page: currentPage + 1,
                                                            })
                                                        }
                                                        className="cursor-pointer text-white"
                                                    />
                                                </PaginationItem>
                                            )}
                                        </PaginationContent>
                                    </Pagination>
                                )}
                            </>
                        )}
                    </main>
                </div>
            </div>
        </div>
    )
}
