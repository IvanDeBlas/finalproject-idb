"use client"

import { useState, useCallback } from "react"
import { useSearchParams, useRouter } from "next/navigation"
import { Card } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { AlertCircle, RotateCcw } from "lucide-react"
import { useProgramaMetricas } from "@/hooks/use-programa-metricas"
import { FiltroFechas } from "./FiltroFechas"
import { KpiCardsGrid } from "./KpiCardsGrid"
import { RankingPromotoresTable } from "./RankingPromotoresTable"
import { DesgloseEventosPanel } from "./DesgloseEventosPanel"
import { GraficoTemporal } from "./GraficoTemporal"

interface ProgramaMetricasTabProps {
    programaId: string
}

export function ProgramaMetricasTab({ programaId }: ProgramaMetricasTabProps) {
    const searchParams = useSearchParams()
    const router = useRouter()

    const fechaDesdeParam = searchParams.get("fechaDesde") ?? ""
    const fechaHastaParam = searchParams.get("fechaHasta") ?? ""

    const [fechaDesdeLocal, setFechaDesdeLocal] = useState(fechaDesdeParam)
    const [fechaHastaLocal, setFechaHastaLocal] = useState(fechaHastaParam)

    const tieneFiltroPeriodo = !!(fechaDesdeParam || fechaHastaParam)

    const { data, isLoading, isFetching, isError, refetch } = useProgramaMetricas(
        programaId,
        {
            fechaDesde: fechaDesdeParam || undefined,
            fechaHasta: fechaHastaParam || undefined,
        }
    )

    const handleAplicar = useCallback(() => {
        const params = new URLSearchParams(searchParams.toString())
        if (fechaDesdeLocal) {
            params.set("fechaDesde", fechaDesdeLocal)
        } else {
            params.delete("fechaDesde")
        }
        if (fechaHastaLocal) {
            params.set("fechaHasta", fechaHastaLocal)
        } else {
            params.delete("fechaHasta")
        }
        router.replace(`?${params.toString()}`, { scroll: false })
    }, [fechaDesdeLocal, fechaHastaLocal, searchParams, router])

    const handleLimpiar = useCallback(() => {
        setFechaDesdeLocal("")
        setFechaHastaLocal("")
        const params = new URLSearchParams(searchParams.toString())
        params.delete("fechaDesde")
        params.delete("fechaHasta")
        router.replace(`?${params.toString()}`, { scroll: false })
    }, [searchParams, router])

    if (isLoading) {
        return (
            <div className="space-y-6" aria-busy="true">
                <FiltroFechas
                    fechaDesde={fechaDesdeLocal}
                    fechaHasta={fechaHastaLocal}
                    onFechaDesdeChange={setFechaDesdeLocal}
                    onFechaHastaChange={setFechaHastaLocal}
                    onAplicar={handleAplicar}
                    onLimpiar={handleLimpiar}
                    tieneFiltroPeriodo={tieneFiltroPeriodo}
                    isLoading={true}
                />
                <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
                    {[1, 2, 3, 4].map((i) => (
                        <Skeleton key={i} className="h-24 bg-[#1e1e38] rounded-xl animate-pulse" />
                    ))}
                </div>
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    {[1, 2].map((i) => (
                        <Skeleton key={i} className="h-16 bg-[#1e1e38] rounded-xl animate-pulse" />
                    ))}
                </div>
                <Skeleton className="h-64 bg-[#1e1e38] rounded-lg animate-pulse" />
                <Skeleton className="h-64 bg-[#1e1e38] rounded-lg animate-pulse" />
            </div>
        )
    }

    if (isError) {
        return (
            <div className="space-y-6">
                <FiltroFechas
                    fechaDesde={fechaDesdeLocal}
                    fechaHasta={fechaHastaLocal}
                    onFechaDesdeChange={setFechaDesdeLocal}
                    onFechaHastaChange={setFechaHastaLocal}
                    onAplicar={handleAplicar}
                    onLimpiar={handleLimpiar}
                    tieneFiltroPeriodo={tieneFiltroPeriodo}
                    isLoading={false}
                />
                <Card
                    className="bg-[#151525] border-zinc-800 p-8 flex flex-col items-center justify-center text-center"
                    role="alert"
                >
                    <AlertCircle className="w-10 h-10 text-red-400 mb-3" />
                    <p className="text-sm text-red-400 mb-4">
                        No se pudieron cargar las metricas
                    </p>
                    <Button
                        variant="outline"
                        size="sm"
                        onClick={() => refetch()}
                        className="border-zinc-700 text-[#94a3b8] hover:text-white focus-visible:ring-2 focus-visible:ring-[#a855f7]"
                    >
                        <RotateCcw className="w-4 h-4 mr-1.5" />
                        Reintentar
                    </Button>
                </Card>
            </div>
        )
    }

    return (
        <div className="space-y-6">
            <FiltroFechas
                fechaDesde={fechaDesdeLocal}
                fechaHasta={fechaHastaLocal}
                onFechaDesdeChange={setFechaDesdeLocal}
                onFechaHastaChange={setFechaHastaLocal}
                onAplicar={handleAplicar}
                onLimpiar={handleLimpiar}
                tieneFiltroPeriodo={tieneFiltroPeriodo}
                isLoading={isFetching}
            />

            {data && (
                <div className={isFetching ? "opacity-60 pointer-events-none" : ""}>
                    <KpiCardsGrid kpis={data.kpis} />

                    <div className="grid grid-cols-1 lg:grid-cols-5 gap-4 mb-6">
                        <div className="lg:col-span-3">
                            <Card className="bg-[#151525] border-zinc-800">
                                <div className="p-4">
                                    <h3 className="text-sm font-medium text-[#94a3b8] mb-3">
                                        Ranking de promotores
                                    </h3>
                                    <RankingPromotoresTable items={data.rankingPromotores} />
                                </div>
                            </Card>
                        </div>
                        <div className="lg:col-span-2">
                            <DesgloseEventosPanel kpis={data.kpis} />
                        </div>
                    </div>

                    <GraficoTemporal datos={data.eventosPorDia} />
                </div>
            )}
        </div>
    )
}
