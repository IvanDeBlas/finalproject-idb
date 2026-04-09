import { useState, useCallback } from "react"
import { useSearchParams } from "react-router-dom"
import { Button } from "@/components/ui/button"
import {
    Pagination,
    PaginationContent,
    PaginationItem,
    PaginationLink,
    PaginationPrevious,
    PaginationNext,
} from "@/components/ui/pagination"
import { Loader2 } from "lucide-react"
import { useMisPropuestas } from "../../application"
import { PropuestaCard } from "../components/PropuestaCard"
import { PropuestaCardSkeleton } from "../components/PropuestaCardSkeleton"
import { EmptyStatePropuestas } from "../components/EmptyStatePropuestas"
import { RetirarPropuestaDialog } from "../components/RetirarPropuestaDialog"
import type { MiPropuestaList } from "../../domain/types"
import { ESTADO_PROPUESTA, ESTADO_PROPUESTA_LABELS } from "@shared/constants"

const PAGE_SIZE = 12

const ESTADO_CHIPS: Array<{ id: number | null; label: string }> = [
    { id: null, label: "Todas" },
    { id: ESTADO_PROPUESTA.PENDIENTE, label: ESTADO_PROPUESTA_LABELS[ESTADO_PROPUESTA.PENDIENTE] },
    { id: ESTADO_PROPUESTA.ACEPTADA, label: ESTADO_PROPUESTA_LABELS[ESTADO_PROPUESTA.ACEPTADA] },
    { id: ESTADO_PROPUESTA.RECHAZADA, label: ESTADO_PROPUESTA_LABELS[ESTADO_PROPUESTA.RECHAZADA] },
    { id: ESTADO_PROPUESTA.RETIRADA, label: ESTADO_PROPUESTA_LABELS[ESTADO_PROPUESTA.RETIRADA] },
]

export default function MisPropuestasPage() {
    const [searchParams, setSearchParams] = useSearchParams()
    const [propuestaARetirar, setPropuestaARetirar] = useState<MiPropuestaList | null>(null)

    const estadoParam = searchParams.get("estado")
    const estadoFiltro = estadoParam ? Number(estadoParam) : undefined
    const currentPage = Number(searchParams.get("page") ?? "1")

    const { data, isLoading, isFetching } = useMisPropuestas({
        estado: estadoFiltro,
        page: currentPage,
        pageSize: PAGE_SIZE,
    })

    const setEstadoFiltro = useCallback(
        (estado: number | null) => {
            setSearchParams((prev) => {
                const next = new URLSearchParams(prev)
                if (estado === null) {
                    next.delete("estado")
                } else {
                    next.set("estado", String(estado))
                }
                next.set("page", "1")
                return next
            })
        },
        [setSearchParams]
    )

    const setPage = useCallback(
        (page: number) => {
            setSearchParams((prev) => {
                const next = new URLSearchParams(prev)
                next.set("page", String(page))
                return next
            })
        },
        [setSearchParams]
    )

    const items = data?.items ?? []
    const totalPages = data?.totalPages ?? 0

    const estadoNombre = estadoFiltro
        ? ESTADO_PROPUESTA_LABELS[estadoFiltro]
        : undefined

    return (
        <div className="min-h-screen bg-[#1a1a2e]">
            <div className="max-w-4xl mx-auto px-4 py-8">
                <h1 className="text-3xl font-bold text-white mb-2">
                    Mis propuestas
                </h1>
                <p className="text-[#94a3b8] mb-8">
                    Gestiona las propuestas que has enviado a artistas
                </p>

                {/* Estado filter chips */}
                <div
                    className="flex flex-wrap gap-2 mb-6"
                    role="group"
                    aria-label="Filtrar por estado"
                >
                    {ESTADO_CHIPS.map((chip) => {
                        const isSelected =
                            chip.id === null
                                ? estadoFiltro === undefined
                                : estadoFiltro === chip.id
                        return (
                            <Button
                                key={chip.id ?? "all"}
                                variant="outline"
                                size="sm"
                                aria-pressed={isSelected}
                                className={
                                    isSelected
                                        ? "bg-purple-600/20 border-[#a855f7] text-white"
                                        : "border-[#334155] text-[#94a3b8] hover:border-[#a855f7] hover:text-white"
                                }
                                onClick={() => setEstadoFiltro(chip.id)}
                            >
                                {chip.label}
                            </Button>
                        )
                    })}
                    {isFetching && !isLoading && (
                        <Loader2 className="w-4 h-4 animate-spin text-[#94a3b8] self-center" aria-hidden="true" />
                    )}
                </div>

                {/* Content */}
                {isLoading ? (
                    <div className="space-y-4">
                        {Array.from({ length: 4 }).map((_, i) => (
                            <PropuestaCardSkeleton key={i} />
                        ))}
                    </div>
                ) : items.length === 0 ? (
                    <EmptyStatePropuestas
                        estadoFiltroActivo={estadoFiltro ?? null}
                        estadoNombre={estadoNombre}
                    />
                ) : (
                    <>
                        <div className="space-y-4 mb-8">
                            {items.map((propuesta) => (
                                <PropuestaCard
                                    key={propuesta.id}
                                    propuesta={propuesta}
                                    onRetirar={setPropuestaARetirar}
                                />
                            ))}
                        </div>

                        {totalPages > 1 && (
                            <Pagination className="mt-8">
                                <PaginationContent>
                                    {currentPage > 1 && (
                                        <PaginationItem>
                                            <PaginationPrevious
                                                onClick={() => setPage(currentPage - 1)}
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
                                                    onClick={() => setPage(page)}
                                                    className="cursor-pointer text-white"
                                                >
                                                    {page}
                                                </PaginationLink>
                                            </PaginationItem>
                                        ))}
                                    {currentPage < totalPages && (
                                        <PaginationItem>
                                            <PaginationNext
                                                onClick={() => setPage(currentPage + 1)}
                                                className="cursor-pointer text-white"
                                            />
                                        </PaginationItem>
                                    )}
                                </PaginationContent>
                            </Pagination>
                        )}
                    </>
                )}
            </div>

            {/* Retirar Propuesta Dialog */}
            <RetirarPropuestaDialog
                propuesta={propuestaARetirar}
                isOpen={propuestaARetirar !== null}
                onClose={() => setPropuestaARetirar(null)}
            />
        </div>
    )
}
