import { useState, useMemo } from "react"
import { useNavigate } from "react-router-dom"
import { AlertCircle, Compass } from "lucide-react"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { APP_ROUTES, INSCRIPCION_ESTADO, INSCRIPCION_ESTADO_LABELS } from "@shared/constants"
import { usePromotor } from "@/features/promotor/application"
import { useMisProgramas } from "../../application/hooks/useMisProgramas"
import { InscripcionItem } from "../components/InscripcionItem"
import { InscripcionItemSkeleton } from "../components/InscripcionItemSkeleton"
import { EmptyStateMisProgramas } from "../components/EmptyStateMisProgramas"
import type { InscripcionEstado } from "../../domain"

type EstadoFiltro = InscripcionEstado | "Todos"

const ESTADO_CHIPS: Array<{
    value: EstadoFiltro
    label: string
    activeClasses: string
    inactiveClasses: string
}> = [
    {
        value: "Todos",
        label: "Todos",
        activeClasses: "bg-[#1e1e38] text-white border border-[#a855f7]",
        inactiveClasses: "text-[#64748b] hover:text-white hover:bg-[#1e1e38]",
    },
    {
        value: INSCRIPCION_ESTADO.APROBADO as InscripcionEstado,
        label: INSCRIPCION_ESTADO_LABELS[INSCRIPCION_ESTADO.APROBADO],
        activeClasses: "bg-green-950/50 text-green-400 border border-green-800/50",
        inactiveClasses: "text-[#64748b] hover:text-green-400/70 hover:bg-green-950/30",
    },
    {
        value: INSCRIPCION_ESTADO.PENDIENTE as InscripcionEstado,
        label: INSCRIPCION_ESTADO_LABELS[INSCRIPCION_ESTADO.PENDIENTE],
        activeClasses: "bg-amber-950/50 text-amber-400 border border-amber-800/50",
        inactiveClasses: "text-[#64748b] hover:text-amber-400/70 hover:bg-amber-950/30",
    },
    {
        value: INSCRIPCION_ESTADO.BLOQUEADO as InscripcionEstado,
        label: INSCRIPCION_ESTADO_LABELS[INSCRIPCION_ESTADO.BLOQUEADO],
        activeClasses: "bg-red-950/50 text-red-400 border border-red-800/50",
        inactiveClasses: "text-[#64748b] hover:text-red-400/70 hover:bg-red-950/30",
    },
]

export default function MisProgramasPage() {
    const navigate = useNavigate()
    const [estadoFiltro, setEstadoFiltro] = useState<EstadoFiltro>("Todos")

    const promotorQuery = usePromotor()
    const { data, isLoading, isError, refetch } = useMisProgramas(1, 50)

    // Redirect if no promotor profile
    if (promotorQuery.isError && !promotorQuery.isLoading) {
        navigate(APP_ROUTES.landing.promotor.registro, { replace: true })
        return null
    }

    const inscripcionesFiltradas = useMemo(() => {
        if (estadoFiltro === "Todos") return data?.items ?? []
        return (data?.items ?? []).filter((i) => i.estado === estadoFiltro)
    }, [data?.items, estadoFiltro])

    const hasItems = (data?.items?.length ?? 0) > 0

    return (
        <div className="min-h-screen bg-[#1a1a2e]">
            <div className="max-w-4xl mx-auto px-4 py-8">
                {/* Header */}
                <div className="flex items-start justify-between mb-6">
                    <div>
                        <h1 className="text-2xl font-bold text-white">Mis programas</h1>
                        <p className="text-sm text-[#94a3b8] mt-1">
                            Gestiona tus inscripciones
                        </p>
                    </div>
                    <Button
                        variant="outline"
                        size="sm"
                        className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-9 px-4 text-sm"
                        onClick={() => navigate(APP_ROUTES.landing.crowdpromotion.explorar)}
                    >
                        <Compass className="w-4 h-4 mr-2" aria-hidden="true" />
                        Explorar programas
                    </Button>
                </div>

                {/* Loading */}
                {isLoading && (
                    <div className="space-y-4" aria-busy="true" aria-label="Cargando inscripciones">
                        {Array.from({ length: 3 }).map((_, i) => (
                            <InscripcionItemSkeleton key={i} />
                        ))}
                    </div>
                )}

                {/* Error */}
                {isError && !isLoading && (
                    <div className="text-center py-12">
                        <AlertCircle className="w-8 h-8 text-red-400 mx-auto mb-3" aria-hidden="true" />
                        <p className="text-sm text-[#94a3b8] mb-4">No se pudieron cargar tus inscripciones</p>
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
                        {!hasItems ? (
                            <EmptyStateMisProgramas
                                hasActiveEstadoFilter={false}
                                onClearFilter={() => setEstadoFiltro("Todos")}
                                onExplorar={() => navigate(APP_ROUTES.landing.crowdpromotion.explorar)}
                            />
                        ) : (
                            <>
                                {/* Filter chips */}
                                <div
                                    className="flex items-center gap-2 mb-5"
                                    role="group"
                                    aria-label="Filtrar inscripciones por estado"
                                >
                                    {ESTADO_CHIPS.map((chip) => (
                                        <Button
                                            key={chip.value}
                                            size="sm"
                                            variant="ghost"
                                            onClick={() => setEstadoFiltro(chip.value)}
                                            className={cn(
                                                "h-8 px-4 text-sm rounded-full",
                                                estadoFiltro === chip.value
                                                    ? chip.activeClasses
                                                    : chip.inactiveClasses
                                            )}
                                            aria-pressed={estadoFiltro === chip.value}
                                        >
                                            {chip.label}
                                        </Button>
                                    ))}
                                </div>

                                {inscripcionesFiltradas.length === 0 && estadoFiltro !== "Todos" ? (
                                    <EmptyStateMisProgramas
                                        hasActiveEstadoFilter={true}
                                        onClearFilter={() => setEstadoFiltro("Todos")}
                                        onExplorar={() => navigate(APP_ROUTES.landing.crowdpromotion.explorar)}
                                    />
                                ) : (
                                    <div className="space-y-4">
                                        {inscripcionesFiltradas.map((inscripcion) => (
                                            <InscripcionItem
                                                key={inscripcion.id}
                                                inscripcion={inscripcion}
                                            />
                                        ))}
                                    </div>
                                )}
                            </>
                        )}
                    </>
                )}
            </div>
        </div>
    )
}
