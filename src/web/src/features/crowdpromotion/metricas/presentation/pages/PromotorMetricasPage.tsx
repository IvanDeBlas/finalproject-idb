import { useEffect, useMemo } from "react"
import { useSearchParams, Link } from "react-router-dom"
import { BarChart2, AlertCircle } from "lucide-react"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { usePromotorMetricas } from "../../application/hooks/usePromotorMetricas"
import { useMisProgramasParaSelector } from "../../application/hooks/useMisProgramasParaSelector"
import { ProgramaSelector } from "../components/ProgramaSelector"
import { MetricasKpiGrid } from "../components/MetricasKpiGrid"
import { TasaConversionInline } from "../components/TasaConversionInline"
import { EnlaceReferido } from "../components/EnlaceReferido"
import { EventosRecientesList } from "../components/EventosRecientesList"

export default function PromotorMetricasPage() {
    const [searchParams, setSearchParams] = useSearchParams()
    const selectedProgramaId = searchParams.get("programaId") || undefined

    const { programas, isLoading: isProgramasLoading, isError: isProgramasError } = useMisProgramasParaSelector()
    const { data: metricas, isLoading: isMetricasLoading, isError: isMetricasError, refetch } = usePromotorMetricas(selectedProgramaId)

    useEffect(() => {
        if (!selectedProgramaId && programas.length > 0) {
            setSearchParams({ programaId: programas[0].id }, { replace: true })
        }
    }, [selectedProgramaId, programas, setSearchParams])

    const handleProgramaChange = (programaId: string) => {
        setSearchParams({ programaId }, { replace: true })
    }

    const selectedPrograma = useMemo(() => {
        return programas.find(p => p.id === selectedProgramaId)
    }, [programas, selectedProgramaId])

    const enlaceUrl = selectedPrograma?.urlTrackingPersonalizada ?? ""
    const hasProgramas = programas.length > 0
    const isLoading = isProgramasLoading || (isMetricasLoading && !!selectedProgramaId)
    const isError = isProgramasError || isMetricasError

    return (
        <div className="min-h-screen bg-[#1a1a2e]">
            <div className="max-w-4xl mx-auto px-4 pt-8 pb-10" role="main">
                {/* Header */}
                <div className="flex flex-col sm:flex-row sm:items-center gap-3 mb-8">
                    <h1 className="text-xl sm:text-2xl font-bold text-white">
                        Mis metricas de promocion
                    </h1>
                    {hasProgramas && (
                        <ProgramaSelector
                            programas={programas}
                            selectedId={selectedProgramaId}
                            onSelect={handleProgramaChange}
                            isLoading={isProgramasLoading}
                        />
                    )}
                </div>

                {/* Loading state */}
                {isProgramasLoading && (
                    <div aria-busy="true" aria-label="Cargando datos">
                        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-6">
                            {[0, 1, 2].map(i => (
                                <div key={i} className="bg-[#151525] border border-[#334155] rounded-lg p-5">
                                    <Skeleton className="w-10 h-10 rounded-lg mb-3 bg-[#1e1e38]" />
                                    <Skeleton className="w-20 h-8 mb-2 bg-[#1e1e38]" />
                                    <Skeleton className="w-24 h-4 bg-[#1e1e38]" />
                                </div>
                            ))}
                        </div>
                        <Skeleton className="h-16 bg-[#1e1e38] rounded-xl mb-6" />
                    </div>
                )}

                {/* Empty state: no programs */}
                {!isProgramasLoading && !hasProgramas && (
                    <div className="flex flex-col items-center justify-center py-20 text-center">
                        <div
                            className="w-16 h-16 rounded-full flex items-center justify-center mb-4"
                            style={{ background: "linear-gradient(135deg, #ec4899 0%, #a855f7 100%)" }}
                        >
                            <BarChart2 className="w-8 h-8 text-white" />
                        </div>
                        <h2 className="text-xl font-semibold text-white mb-2">Sin programas activos</h2>
                        <p className="text-sm text-[#94a3b8] mb-6 max-w-xs">
                            Inscribete en un programa de promocion para ver tus metricas
                        </p>
                        <Button
                            asChild
                            className="bg-gradient-to-r from-pink-500 to-purple-600 text-white hover:from-pink-400 hover:to-purple-500 focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
                        >
                            <Link to="/promotor/mis-programas">Ver programas disponibles</Link>
                        </Button>
                    </div>
                )}

                {/* Main content with data */}
                {!isProgramasLoading && hasProgramas && (
                    <div aria-busy={isMetricasLoading}>
                        {/* KPI Grid */}
                        {metricas && !isMetricasLoading ? (
                            <>
                                <MetricasKpiGrid kpis={metricas.kpis} />
                                <TasaConversionInline tasa={metricas.kpis.miTasaConversion} />
                            </>
                        ) : isMetricasLoading ? (
                            <MetricasKpiGrid
                                kpis={{
                                    misClicks: 0,
                                    misPageViews: 0,
                                    misSignups: 0,
                                    misConversiones: 0,
                                    miValorGenerado: 0,
                                    miComisionAcumulada: 0,
                                    monedaNombre: null,
                                    miTasaConversion: 0,
                                }}
                                isLoading
                            />
                        ) : null}

                        {/* Enlace referido */}
                        {enlaceUrl && !isMetricasLoading && (
                            <EnlaceReferido url={enlaceUrl} />
                        )}
                        {isMetricasLoading && (
                            <div className="bg-[#151525] border border-[#334155] rounded-lg p-5 mb-6">
                                <Skeleton className="w-32 h-4 mb-3 bg-[#1e1e38]" />
                                <div className="flex gap-2">
                                    <Skeleton className="flex-1 h-10 bg-[#1e1e38] rounded-md" />
                                    <Skeleton className="w-20 h-10 bg-[#1e1e38] rounded-md" />
                                </div>
                            </div>
                        )}

                        {/* Eventos recientes */}
                        {metricas && !isMetricasLoading ? (
                            <EventosRecientesList
                                eventos={metricas.eventosRecientes}
                                programaId={selectedProgramaId}
                            />
                        ) : isMetricasLoading ? (
                            <EventosRecientesList
                                eventos={[]}
                                programaId={selectedProgramaId}
                                isLoading
                            />
                        ) : null}

                        {/* Error state */}
                        {isError && !isLoading && (
                            <div
                                className="flex flex-col items-center justify-center py-16 text-center"
                                role="alert"
                            >
                                <AlertCircle className="w-10 h-10 text-red-400 mb-4" />
                                <h2 className="text-lg font-semibold text-white mb-2">
                                    No se pudieron cargar las metricas
                                </h2>
                                <p className="text-sm text-[#94a3b8] mb-6">
                                    Ocurrio un error al obtener tus datos. Intenta de nuevo.
                                </p>
                                <Button
                                    variant="outline"
                                    size="sm"
                                    onClick={() => refetch()}
                                    className="border-[#334155] text-[#94a3b8] hover:border-[#a855f7] hover:text-[#a855f7] focus-visible:ring-[#a855f7] focus-visible:ring-offset-[#1a1a2e]"
                                >
                                    Reintentar
                                </Button>
                            </div>
                        )}
                    </div>
                )}
            </div>
        </div>
    )
}
