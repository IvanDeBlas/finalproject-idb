"use client"

import { useState, useMemo, useCallback } from "react"
import { Button } from "@/components/ui/button"
import { Plus } from "lucide-react"
import Link from "next/link"
import { useMisProgramas } from "@/hooks/use-promo-programas"
import { PromoProgramaCard } from "./PromoProgramaCard"
import { PromoProgramaFilters } from "./PromoProgramaFilters"
import { PromoProgramaEmptyState } from "./PromoProgramaEmptyState"
import { DesactivarPromoProgramaDialog } from "../../programas/[id]/components/DesactivarPromoProgramaDialog"
import { APP_ROUTES } from "@shared/constants"
import type { PromoProgramaEstado, PromoProgramaListItem } from "@shared/types"

type FiltroEstado = PromoProgramaEstado | "todos"

export function PromoProgramaListClient() {
    const [filtroEstado, setFiltroEstado] = useState<FiltroEstado>("todos")
    const [busqueda, setBusqueda] = useState("")
    const [paginaActual, setPaginaActual] = useState(1)
    const [showDesactivarDialog, setShowDesactivarDialog] = useState(false)
    const [programaADesactivar, setProgramaADesactivar] = useState<PromoProgramaListItem | null>(null)

    const esActivo = filtroEstado === "todos" ? undefined : filtroEstado === "activo"

    const { data, isLoading, isError, refetch } = useMisProgramas({
        esActivo,
        page: paginaActual,
        pageSize: 10,
    })

    const filteredItems = useMemo(() => {
        if (!data?.items) return []
        if (!busqueda.trim()) return data.items
        const q = busqueda.toLowerCase()
        return data.items.filter(
            (p) =>
                p.titulo.toLowerCase().includes(q) ||
                p.tipoPromoNombre.toLowerCase().includes(q) ||
                p.campaniaTitulo?.toLowerCase().includes(q)
        )
    }, [data?.items, busqueda])

    const handleDesactivar = useCallback((programa: PromoProgramaListItem) => {
        setProgramaADesactivar(programa)
        setShowDesactivarDialog(true)
    }, [])

    const handleLimpiarFiltros = useCallback(() => {
        setFiltroEstado("todos")
        setBusqueda("")
        setPaginaActual(1)
    }, [])

    if (isLoading) {
        return (
            <div className="space-y-6">
                <div className="flex items-center justify-between">
                    <div className="h-8 w-64 animate-pulse rounded bg-[#1e1e38]" />
                    <div className="h-10 w-40 animate-pulse rounded bg-[#1e1e38]" />
                </div>
                <div className="grid gap-4">
                    {[1, 2, 3].map((i) => (
                        <div key={i} className="h-[120px] animate-pulse rounded-lg bg-[#1e1e38]" />
                    ))}
                </div>
            </div>
        )
    }

    if (isError) {
        return (
            <div className="flex flex-col items-center justify-center py-16 text-center">
                <p className="text-sm text-red-400 mb-4">Error al cargar los programas</p>
                <Button variant="outline" onClick={() => refetch()}>
                    Reintentar
                </Button>
            </div>
        )
    }

    const hasAnyPrograms = data && data.totalCount > 0
    const totalPages = data ? Math.ceil(data.totalCount / data.pageSize) : 1

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-bold text-white">Mis Programas de Promocion</h1>
                <Button asChild className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700">
                    <Link href={APP_ROUTES.dashboard.crowdpromotion.programas.nuevo}>
                        <Plus className="h-4 w-4 mr-2" />
                        Crear programa
                    </Link>
                </Button>
            </div>

            {hasAnyPrograms && (
                <PromoProgramaFilters
                    filtroEstado={filtroEstado}
                    busqueda={busqueda}
                    onFiltroEstadoChange={(v) => {
                        setFiltroEstado(v)
                        setPaginaActual(1)
                    }}
                    onBusquedaChange={setBusqueda}
                />
            )}

            {!hasAnyPrograms && filtroEstado === "todos" && !busqueda ? (
                <PromoProgramaEmptyState variant="no-programas" />
            ) : filteredItems.length === 0 ? (
                <PromoProgramaEmptyState
                    variant="no-resultados"
                    onLimpiarFiltros={handleLimpiarFiltros}
                />
            ) : (
                <>
                    <div className="grid gap-4">
                        {filteredItems.map((programa) => (
                            <PromoProgramaCard
                                key={programa.id}
                                programa={programa}
                                onDesactivar={handleDesactivar}
                            />
                        ))}
                    </div>

                    {totalPages > 1 && (
                        <div className="flex items-center justify-center gap-2 pt-4">
                            <Button
                                variant="outline"
                                size="sm"
                                disabled={paginaActual === 1}
                                onClick={() => setPaginaActual((p) => p - 1)}
                            >
                                Anterior
                            </Button>
                            <span className="text-sm text-zinc-400">
                                Pagina {paginaActual} de {totalPages}
                            </span>
                            <Button
                                variant="outline"
                                size="sm"
                                disabled={paginaActual === totalPages}
                                onClick={() => setPaginaActual((p) => p + 1)}
                            >
                                Siguiente
                            </Button>
                        </div>
                    )}
                </>
            )}

            {programaADesactivar && (
                <DesactivarPromoProgramaDialog
                    open={showDesactivarDialog}
                    onOpenChange={setShowDesactivarDialog}
                    programaId={programaADesactivar.id}
                    tituloPrograma={programaADesactivar.titulo}
                />
            )}
        </div>
    )
}
