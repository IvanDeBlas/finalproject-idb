import { useState, useEffect, useMemo } from "react"
import { useNavigate } from "react-router-dom"
import { AlertCircle } from "lucide-react"
import { Button } from "@/components/ui/button"
import {
    Pagination,
    PaginationContent,
    PaginationItem,
    PaginationLink,
    PaginationPrevious,
    PaginationNext,
} from "@/components/ui/pagination"
import { APP_ROUTES, TIPO_PROMO_LABELS } from "@shared/constants"
import { useAuthStore } from "@/store/auth-store"
import { useDebounce } from "@/hooks/useDebounce"
import { usePromotor } from "@/features/promotor/application"
import { useExplorarProgramas } from "../../application/hooks/useExplorarProgramas"
import { useSolicitarInscripcion } from "../../application/hooks/useSolicitarInscripcion"
import { ProgramaCard } from "../components/ProgramaCard"
import { ProgramaCardSkeleton } from "../components/ProgramaCardSkeleton"
import { ProgramaFiltros } from "../components/ProgramaFiltros"
import { EmptyStateProgramas } from "../components/EmptyStateProgramas"
import { SinPerfilPromotorBanner } from "../components/SinPerfilPromotorBanner"

const TIPOS_PROMO = Object.entries(TIPO_PROMO_LABELS).map(([id, nombre]) => ({
    id: Number(id),
    nombre,
}))

const PAGE_SIZE = 10

export default function ExplorarProgramasPage() {
    const navigate = useNavigate()
    const { isAuthenticated } = useAuthStore()

    const [artistaNombreInput, setArtistaNombreInput] = useState("")
    const [tipoPromoId, setTipoPromoId] = useState<number | undefined>(undefined)
    const [page, setPage] = useState(1)

    const debouncedArtistaNombre = useDebounce(artistaNombreInput, 300)

    // Reset page when filters change
    useEffect(() => {
        setPage(1)
    }, [debouncedArtistaNombre, tipoPromoId])

    const promotorQuery = usePromotor()
    const sinPerfil = isAuthenticated && promotorQuery.isError

    const { data, isLoading, isFetching, isError, refetch } = useExplorarProgramas({
        artistaNombre: debouncedArtistaNombre || undefined,
        tipoPromoId,
        page,
        pageSize: PAGE_SIZE,
    })

    const solicitarMutation = useSolicitarInscripcion()

    const hasActiveFilters = useMemo(
        () => Boolean(debouncedArtistaNombre) || tipoPromoId !== undefined,
        [debouncedArtistaNombre, tipoPromoId]
    )

    const handleClearFilters = () => {
        setArtistaNombreInput("")
        setTipoPromoId(undefined)
        setPage(1)
    }

    const handleSolicitar = (programaId: string) => {
        if (!isAuthenticated) {
            navigate(`/auth/login?redirect=/crowdpromotion/explorar`)
            return
        }
        solicitarMutation.mutate(programaId)
    }

    const handleVerMisDatos = () => {
        navigate(APP_ROUTES.landing.crowdpromotion.misProgramas)
    }

    const handlePageChange = (newPage: number) => {
        if (!data) return
        if (newPage < 1 || newPage > data.totalPages) return
        setPage(newPage)
        document.querySelector(".programa-grid")?.scrollIntoView({ behavior: "smooth" })
    }

    const totalPages = data?.totalPages ?? 0

    return (
        <div className="min-h-screen bg-[#1a1a2e]">
            {/* Hero */}
            <section className="py-10 bg-[#0d0d1a] border-b border-[#334155]">
                <div className="max-w-5xl mx-auto px-4">
                    <h1 className="text-2xl font-bold text-white">
                        Programas de promocion disponibles
                    </h1>
                    <p className="text-sm text-[#94a3b8] mt-1">
                        Encuentra programas y empieza a ganar comisiones
                    </p>
                </div>
            </section>

            {/* Content */}
            <div className="max-w-5xl mx-auto px-4 py-6">
                {sinPerfil && (
                    <SinPerfilPromotorBanner
                        onCrearPerfil={() => navigate(APP_ROUTES.landing.promotor.registro)}
                    />
                )}

                <ProgramaFiltros
                    artistaNombre={artistaNombreInput}
                    tipoPromoId={tipoPromoId}
                    tiposPromo={TIPOS_PROMO}
                    hasActiveFilters={hasActiveFilters}
                    onArtistaNombreChange={setArtistaNombreInput}
                    onTipoPromoChange={setTipoPromoId}
                    onClearFilters={handleClearFilters}
                />

                {/* Loading */}
                {isLoading && (
                    <div
                        className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5 programa-grid"
                        aria-busy="true"
                        aria-label="Cargando programas"
                    >
                        {Array.from({ length: 6 }).map((_, i) => (
                            <ProgramaCardSkeleton key={i} />
                        ))}
                    </div>
                )}

                {/* Error */}
                {isError && !isLoading && (
                    <div className="text-center py-12">
                        <AlertCircle className="w-8 h-8 text-red-400 mx-auto mb-3" aria-hidden="true" />
                        <p className="text-sm text-[#94a3b8] mb-4">No se pudieron cargar los programas</p>
                        <Button
                            variant="outline"
                            size="sm"
                            onClick={() => refetch()}
                            className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]"
                        >
                            Reintentar
                        </Button>
                    </div>
                )}

                {/* Data */}
                {!isLoading && !isError && data && (
                    <>
                        {data.items.length === 0 ? (
                            <EmptyStateProgramas
                                hasActiveFilters={hasActiveFilters}
                                onClearFilters={handleClearFilters}
                            />
                        ) : (
                            <>
                                <p className="text-sm text-[#64748b] mb-4" aria-live="polite">
                                    Mostrando {Math.min(page * PAGE_SIZE, data.totalCount)} de {data.totalCount} programas
                                    {isFetching && " (actualizando...)"}
                                </p>
                                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5 programa-grid">
                                    {data.items.map((programa) => (
                                        <ProgramaCard
                                            key={programa.id}
                                            programa={programa}
                                            isSolicitando={
                                                solicitarMutation.isPending &&
                                                solicitarMutation.variables === programa.id
                                            }
                                            onSolicitar={handleSolicitar}
                                            onVerMisDatos={handleVerMisDatos}
                                        />
                                    ))}
                                </div>
                                {totalPages > 1 && (
                                    <Pagination className="mt-8 pb-10">
                                        <PaginationContent>
                                            <PaginationItem>
                                                <PaginationPrevious
                                                    href="#"
                                                    onClick={(e) => { e.preventDefault(); handlePageChange(page - 1) }}
                                                    className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]"
                                                    aria-disabled={page === 1}
                                                />
                                            </PaginationItem>
                                            {Array.from({ length: totalPages }, (_, i) => i + 1).map((p) => (
                                                <PaginationItem key={p}>
                                                    <PaginationLink
                                                        href="#"
                                                        onClick={(e) => { e.preventDefault(); handlePageChange(p) }}
                                                        isActive={p === page}
                                                        className={
                                                            p === page
                                                                ? "bg-[#1e1e38] text-white border-[#a855f7]"
                                                                : "text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] border-[#334155]"
                                                        }
                                                    >
                                                        {p}
                                                    </PaginationLink>
                                                </PaginationItem>
                                            ))}
                                            <PaginationItem>
                                                <PaginationNext
                                                    href="#"
                                                    onClick={(e) => { e.preventDefault(); handlePageChange(page + 1) }}
                                                    className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38]"
                                                    aria-disabled={page === totalPages}
                                                />
                                            </PaginationItem>
                                        </PaginationContent>
                                    </Pagination>
                                )}
                            </>
                        )}
                    </>
                )}
            </div>
        </div>
    )
}
