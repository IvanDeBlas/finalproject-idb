import { FC, useRef, useState } from "react"
import { Skeleton } from "@/components/ui/skeleton"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { AlertCircle, ChevronLeft, ChevronRight } from "lucide-react"
import { cn } from "@/lib/utils"
import { useValoracionesUsuario } from "../../application/hooks/useValoracionesUsuario"
import { StarDisplay } from "./StarDisplay"
import { RatingHistogram } from "./RatingHistogram"
import { ValoracionListItem } from "./ValoracionListItem"
import { EmptyValoraciones } from "./EmptyValoraciones"

interface ValoracionesSectionProps {
    userId: string
}

function getPageNumbers(
    current: number,
    total: number,
    isMobile: boolean
): number[] {
    if (isMobile) return [current]
    const pages: number[] = []
    const start = Math.max(1, current - 2)
    const end = Math.min(total, start + 4)
    for (let i = start; i <= end; i++) {
        pages.push(i)
    }
    return pages
}

export const ValoracionesSection: FC<ValoracionesSectionProps> = ({
    userId,
}) => {
    const [page, setPage] = useState(1)
    const pageSize = 10
    const sectionRef = useRef<HTMLElement>(null)

    const { data, isLoading, isFetching, isError, refetch } =
        useValoracionesUsuario(userId, page, pageSize)

    const handlePageChange = (newPage: number) => {
        setPage(newPage)
        sectionRef.current?.scrollIntoView({
            behavior: "smooth",
            block: "start",
        })
    }

    if (isLoading) {
        return (
            <section className="mt-8">
                <h2 className="text-xl font-semibold text-white mb-5">
                    Valoraciones
                </h2>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
                    <Skeleton className="h-36 rounded-xl bg-[#1e2a42] animate-pulse" />
                    <Skeleton className="h-36 rounded-xl bg-[#1e2a42] animate-pulse" />
                </div>
                <div className="space-y-3">
                    <Skeleton className="h-28 rounded-xl bg-[#1e2a42] animate-pulse" />
                    <Skeleton className="h-28 rounded-xl bg-[#1e2a42] animate-pulse" />
                    <Skeleton className="h-28 rounded-xl bg-[#1e2a42] animate-pulse" />
                </div>
            </section>
        )
    }

    if (isError) {
        return (
            <section className="mt-8">
                <h2 className="text-xl font-semibold text-white mb-5">
                    Valoraciones
                </h2>
                <Alert className="border-[#ef4444]/50 bg-[#0f1729] text-white">
                    <AlertCircle
                        className="w-4 h-4 text-[#ef4444]"
                        aria-hidden="true"
                    />
                    <AlertDescription className="flex items-center justify-between">
                        <span className="text-[#94a3b8] text-sm">
                            No se pudieron cargar las valoraciones.
                        </span>
                        <Button
                            variant="ghost"
                            size="sm"
                            className="text-[#94a3b8] hover:text-white hover:bg-[#1e2a42] ml-4"
                            onClick={() => refetch()}
                        >
                            Reintentar
                        </Button>
                    </AlertDescription>
                </Alert>
            </section>
        )
    }

    if (!data || data.resumen.totalValoraciones === 0) {
        return (
            <section className="mt-8">
                <h2 className="text-xl font-semibold text-white mb-5">
                    Valoraciones
                </h2>
                <EmptyValoraciones />
            </section>
        )
    }

    const { resumen, valoraciones } = data
    const totalPages = Math.ceil(valoraciones.totalCount / pageSize)
    const isLoadingPage = isFetching && !isLoading

    return (
        <section className="mt-8" ref={sectionRef}>
            <h2 className="text-xl font-semibold text-white mb-5">
                Valoraciones
            </h2>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
                <div className="bg-[#0f1729] border border-[#334155] rounded-xl p-5 flex flex-col items-center justify-center">
                    <span className="text-5xl font-bold text-white leading-none">
                        {resumen.puntuacionMedia?.toFixed(1)}
                    </span>
                    <StarDisplay
                        value={resumen.puntuacionMedia ?? 0}
                        size="md"
                        className="my-2"
                    />
                    <p className="text-sm text-[#94a3b8] text-center">
                        {resumen.totalValoraciones} valoraciones
                    </p>
                </div>

                <div className="bg-[#0f1729] border border-[#334155] rounded-xl p-5">
                    <p className="text-xs text-[#64748b] uppercase tracking-wider mb-3">
                        Distribucion
                    </p>
                    <RatingHistogram
                        distribucion={resumen.distribucion}
                        total={resumen.totalValoraciones}
                    />
                </div>
            </div>

            <h3 className="text-base font-semibold text-white mb-4">
                Valoraciones recientes
            </h3>
            <div className="space-y-3">
                {isLoadingPage ? (
                    <>
                        <Skeleton className="h-28 rounded-xl bg-[#1e2a42] animate-pulse" />
                        <Skeleton className="h-28 rounded-xl bg-[#1e2a42] animate-pulse" />
                    </>
                ) : (
                    valoraciones.items.map((item) => (
                        <ValoracionListItem
                            key={item.id}
                            valoracion={item}
                        />
                    ))
                )}
            </div>

            {totalPages > 1 && (
                <nav
                    aria-label="Paginacion de valoraciones"
                    className="flex items-center justify-center gap-1 mt-6"
                >
                    <Button
                        variant="ghost"
                        size="sm"
                        className="h-8 w-8 p-0 text-[#94a3b8] hover:bg-[#16213e] hover:text-white disabled:opacity-30 focus-visible:ring-2 focus-visible:ring-[#a855f7]"
                        onClick={() => handlePageChange(page - 1)}
                        disabled={page <= 1 || isLoadingPage}
                        aria-label="Pagina anterior"
                    >
                        <ChevronLeft
                            className="w-4 h-4"
                            aria-hidden="true"
                        />
                    </Button>

                    {getPageNumbers(page, totalPages, false).map(
                        (pageNum) => (
                            <Button
                                key={pageNum}
                                variant={
                                    pageNum === page ? "default" : "ghost"
                                }
                                size="sm"
                                className={cn(
                                    "h-8 w-8 p-0 text-sm focus-visible:ring-2 focus-visible:ring-[#a855f7]",
                                    pageNum === page
                                        ? "bg-gradient-to-r from-pink-500 to-purple-600 text-white hover:from-pink-600 hover:to-purple-700"
                                        : "text-[#94a3b8] hover:bg-[#16213e] hover:text-white"
                                )}
                                onClick={() => handlePageChange(pageNum)}
                                disabled={isLoadingPage}
                                aria-label={
                                    pageNum === page
                                        ? undefined
                                        : `Ir a pagina ${pageNum}`
                                }
                                aria-current={
                                    pageNum === page ? "page" : undefined
                                }
                            >
                                {pageNum}
                            </Button>
                        )
                    )}

                    <Button
                        variant="ghost"
                        size="sm"
                        className="h-8 w-8 p-0 text-[#94a3b8] hover:bg-[#16213e] hover:text-white disabled:opacity-30 focus-visible:ring-2 focus-visible:ring-[#a855f7]"
                        onClick={() => handlePageChange(page + 1)}
                        disabled={page >= totalPages || isLoadingPage}
                        aria-label="Pagina siguiente"
                    >
                        <ChevronRight
                            className="w-4 h-4"
                            aria-hidden="true"
                        />
                    </Button>
                </nav>
            )}
        </section>
    )
}
