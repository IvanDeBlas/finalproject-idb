"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { Plus } from "lucide-react"
import { useMisNecesidades } from "@/hooks/use-necesidades"
import { useCerrarNecesidad } from "@/hooks/use-necesidades-mutations"
import { useDebounce } from "@/hooks/use-debounce"
import { NecesidadCard } from "./components/list/NecesidadCard"
import { NecesidadFilters } from "./components/list/NecesidadFilters"
import { EmptyStateNecesidades } from "./components/list/EmptyStateNecesidades"
import { CerrarNecesidadDialog } from "./components/shared/CerrarNecesidadDialog"
import { APP_ROUTES } from "@shared/constants"

export default function MisNecesidadesPage() {
    const router = useRouter()
    const [searchQuery, setSearchQuery] = useState("")
    const [estadoFilter, setEstadoFilter] = useState("all")
    const [page, setPage] = useState(1)
    const [cerrarDialogId, setCerrarDialogId] = useState<string | null>(null)

    const debouncedSearch = useDebounce(searchQuery, 300)

    const { data, isLoading } = useMisNecesidades({
        page,
        pageSize: 12,
        estado: estadoFilter !== "all" ? Number(estadoFilter) : undefined,
        search: debouncedSearch || undefined,
    })

    const cerrarMutation = useCerrarNecesidad(cerrarDialogId || "")

    const hasFilters = searchQuery !== "" || estadoFilter !== "all"

    const handleCerrarConfirm = (motivo?: string) => {
        cerrarMutation.mutate(
            { motivo },
            {
                onSettled: () => setCerrarDialogId(null),
            }
        )
    }

    const handleEstadoChange = (value: string) => {
        setEstadoFilter(value)
        setPage(1)
    }

    const handleSearchChange = (value: string) => {
        setSearchQuery(value)
        setPage(1)
    }

    return (
        <div className="space-y-6">
            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
                <div>
                    <h1 className="text-3xl font-bold">Mis Necesidades</h1>
                    <p className="text-muted-foreground mt-1">
                        Gestiona tus solicitudes de servicios profesionales
                    </p>
                </div>
                <div className="flex gap-3">
                    <Button
                        onClick={() =>
                            router.push(APP_ROUTES.dashboard.crowdsourcing.nuevaNecesidad)
                        }
                    >
                        <Plus className="h-4 w-4 mr-2" />
                        Nueva Necesidad
                    </Button>
                    <Button
                        variant="outline"
                        onClick={() =>
                            router.push(APP_ROUTES.dashboard.crowdsourcing.templates)
                        }
                    >
                        Usar Plantilla
                    </Button>
                </div>
            </div>

            <NecesidadFilters
                estadoFilter={estadoFilter}
                onEstadoChange={handleEstadoChange}
                searchQuery={searchQuery}
                onSearchChange={handleSearchChange}
            />

            {isLoading ? (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                    {Array.from({ length: 6 }).map((_, i) => (
                        <Skeleton key={i} className="h-64" />
                    ))}
                </div>
            ) : data?.items.length === 0 ? (
                <EmptyStateNecesidades
                    hasFilters={hasFilters}
                    onClearFilters={() => {
                        setSearchQuery("")
                        setEstadoFilter("all")
                    }}
                    onNuevaNecesidad={() =>
                        router.push(APP_ROUTES.dashboard.crowdsourcing.nuevaNecesidad)
                    }
                    onUsarPlantilla={() =>
                        router.push(APP_ROUTES.dashboard.crowdsourcing.templates)
                    }
                />
            ) : (
                <>
                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                        {data?.items.map((necesidad) => (
                            <NecesidadCard
                                key={necesidad.id}
                                necesidad={necesidad}
                                onClick={() =>
                                    router.push(
                                        APP_ROUTES.dashboard.crowdsourcing.necesidadDetail(
                                            necesidad.id
                                        )
                                    )
                                }
                                onEdit={() =>
                                    router.push(
                                        APP_ROUTES.dashboard.crowdsourcing.editarNecesidad(
                                            necesidad.id
                                        )
                                    )
                                }
                                onCerrar={() => setCerrarDialogId(necesidad.id)}
                            />
                        ))}
                    </div>

                    {data && data.totalPages > 1 && (
                        <div className="flex justify-center gap-2 pt-4">
                            <Button
                                variant="outline"
                                size="sm"
                                disabled={page <= 1}
                                onClick={() => setPage((p) => p - 1)}
                            >
                                Anterior
                            </Button>
                            <span className="flex items-center text-sm text-muted-foreground px-4">
                                Pagina {page} de {data.totalPages}
                            </span>
                            <Button
                                variant="outline"
                                size="sm"
                                disabled={page >= data.totalPages}
                                onClick={() => setPage((p) => p + 1)}
                            >
                                Siguiente
                            </Button>
                        </div>
                    )}
                </>
            )}

            <CerrarNecesidadDialog
                open={!!cerrarDialogId}
                onOpenChange={(open) => !open && setCerrarDialogId(null)}
                onConfirm={handleCerrarConfirm}
                isPending={cerrarMutation.isPending}
            />
        </div>
    )
}
