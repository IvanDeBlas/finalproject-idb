"use client"

import { useState } from "react"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { usePromoPrograma } from "@/hooks/use-promo-programas"
import { PromoProgramaHeader } from "./PromoProgramaHeader"
import { PromoProgramaKpiCards } from "./PromoProgramaKpiCards"
import { PromoProgramaInfoTab } from "./PromoProgramaInfoTab"
import { PromoProgramaTareasTab } from "./PromoProgramaTareasTab"
import { PromoProgramaPromotoresTab } from "./PromoProgramaPromotoresTab"
import { PromoProgramaResumenTab } from "./PromoProgramaResumenTab"
import { DesactivarPromoProgramaDialog } from "./DesactivarPromoProgramaDialog"
import { PromoProgramaSolicitudesTab } from "./PromoProgramaSolicitudesTab"
import { PromoProgramaAprobadosTab } from "./PromoProgramaAprobadosTab"
import { PromoProgramaBloqueadosTab } from "./PromoProgramaBloqueadosTab"
import { PromoProgramaPendientesTab } from "./PromoProgramaPendientesTab"
import { ProgramaMetricasTab } from "@/components/crowdpromotion/metricas/ProgramaMetricasTab"
import { useTareasPendientes } from "@/hooks/use-tareas-pendientes"

interface PromoProgramaDetailClientProps {
    programaId: string
}

export function PromoProgramaDetailClient({ programaId }: PromoProgramaDetailClientProps) {
    const [showDesactivarDialog, setShowDesactivarDialog] = useState(false)
    const { data: programa, isLoading, isError, refetch } = usePromoPrograma(programaId)
    const { data: tareasPendientesData } = useTareasPendientes(programaId)

    if (isLoading) {
        return (
            <div className="space-y-6">
                <div className="h-12 w-80 animate-pulse rounded bg-[#1e1e38]" />
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                    {[1, 2, 3, 4].map((i) => (
                        <div key={i} className="h-[80px] animate-pulse rounded-lg bg-[#1e1e38]" />
                    ))}
                </div>
                <div className="h-[300px] animate-pulse rounded-lg bg-[#1e1e38]" />
            </div>
        )
    }

    if (isError || !programa) {
        return (
            <div className="flex flex-col items-center justify-center py-16 text-center">
                <p className="text-sm text-red-400 mb-4">Error al cargar el programa</p>
                <Button variant="outline" onClick={() => refetch()}>
                    Reintentar
                </Button>
            </div>
        )
    }

    return (
        <div className="space-y-6">
            <PromoProgramaHeader
                programa={programa}
                onDesactivar={() => setShowDesactivarDialog(true)}
            />

            {!programa.esActivo && (
                <div className="rounded-lg border border-amber-500/30 bg-amber-500/5 p-3">
                    <p className="text-sm text-amber-400">
                        Este programa esta inactivo. No aparece en el catalogo publico.
                    </p>
                </div>
            )}

            <PromoProgramaKpiCards resumen={programa.resumen} />

            <Tabs defaultValue="info" className="w-full">
                <div className="overflow-x-auto">
                    <TabsList className="bg-[#1e1e38] border border-zinc-800 w-max min-w-full">
                        <TabsTrigger value="info">Info general</TabsTrigger>
                        <TabsTrigger value="tareas">Tareas ({programa.tareas.length})</TabsTrigger>
                        <TabsTrigger value="pendientes" className="flex items-center gap-1.5">
                            Pendientes
                            {(tareasPendientesData?.totalCount ?? 0) > 0 && (
                                <Badge
                                    className="bg-red-500/20 text-red-400 border border-red-500/30 text-[10px] rounded-full w-5 h-5 flex items-center justify-center font-bold p-0"
                                    aria-label={`${tareasPendientesData?.totalCount} tareas pendientes`}
                                >
                                    {tareasPendientesData?.totalCount}
                                </Badge>
                            )}
                        </TabsTrigger>
                        <TabsTrigger value="promotores">Promotores ({programa.promotores.length})</TabsTrigger>
                        <TabsTrigger value="solicitudes" className="flex items-center gap-1.5">
                            Solicitudes
                            {programa.resumen.totalPromotoresPendientes > 0 && (
                                <Badge
                                    className="bg-amber-500 text-black text-[10px] rounded-full w-5 h-5 flex items-center justify-center font-bold p-0"
                                    aria-label={`${programa.resumen.totalPromotoresPendientes} solicitudes pendientes`}
                                >
                                    {programa.resumen.totalPromotoresPendientes}
                                </Badge>
                            )}
                        </TabsTrigger>
                        <TabsTrigger value="aprobados">Aprobados</TabsTrigger>
                        <TabsTrigger value="bloqueados">Bloqueados</TabsTrigger>
                        <TabsTrigger value="metricas">Metricas</TabsTrigger>
                        <TabsTrigger value="resumen">Resumen</TabsTrigger>
                    </TabsList>
                </div>

                <TabsContent value="info" className="mt-4">
                    <PromoProgramaInfoTab programa={programa} />
                </TabsContent>
                <TabsContent value="tareas" className="mt-4">
                    <PromoProgramaTareasTab
                        tareas={programa.tareas}
                        esActivo={programa.esActivo}
                        programaId={programa.id}
                    />
                </TabsContent>
                <TabsContent value="pendientes" className="mt-4">
                    <PromoProgramaPendientesTab programaId={programa.id} />
                </TabsContent>
                <TabsContent value="promotores" className="mt-4">
                    <PromoProgramaPromotoresTab promotores={programa.promotores} />
                </TabsContent>
                <TabsContent value="solicitudes" className="mt-4">
                    <PromoProgramaSolicitudesTab programaId={programa.id} />
                </TabsContent>
                <TabsContent value="aprobados" className="mt-4">
                    <PromoProgramaAprobadosTab programaId={programa.id} />
                </TabsContent>
                <TabsContent value="bloqueados" className="mt-4">
                    <PromoProgramaBloqueadosTab programaId={programa.id} />
                </TabsContent>
                <TabsContent value="metricas" className="mt-4">
                    <ProgramaMetricasTab programaId={programa.id} />
                </TabsContent>
                <TabsContent value="resumen" className="mt-4">
                    <PromoProgramaResumenTab resumen={programa.resumen} />
                </TabsContent>
            </Tabs>

            <DesactivarPromoProgramaDialog
                open={showDesactivarDialog}
                onOpenChange={setShowDesactivarDialog}
                programaId={programa.id}
                tituloPrograma={programa.titulo}
            />
        </div>
    )
}
