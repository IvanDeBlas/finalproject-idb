"use client"

import Link from "next/link"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { Alert, AlertTitle, AlertDescription } from "@/components/ui/alert"
import { StatsCard } from "@/components/dashboard/stats-card"
import { RecentBackings } from "@/components/dashboard/recent-backings"
import { CompleteProfileBanner } from "@/components/dashboard/complete-profile-banner"
import { PromotorBanner } from "@/components/dashboard/promotor-banner"
import { MisCampaniasCard } from "./components/MisCampaniasCard"
import { useMisCampanias } from "@/hooks/use-campanias"
import { useMyArtistProfile } from "@/hooks/use-artista"
import { useDashboardStats } from "@/hooks/use-dashboard-stats"
import { formatCurrency } from "@shared/utils"
import { CAMPANIA_ESTADOS, CAMPANIA_ESTADOS_LABELS } from "@shared/constants"
import {
    PlusCircle,
    Music,
    TrendingUp,
    Users,
    BarChart3,
    AlertCircle,
} from "lucide-react"
import type { MiCampaniaListItem } from "@shared/types"

export default function DashboardPage() {
    const { data: artista } = useMyArtistProfile()
    const { data: campanias, isLoading: campaniasLoading } = useMisCampanias()
    const {
        data: dashboardStats,
        isLoading: statsLoading,
        error: statsError,
        refetch: refetchStats,
    } = useDashboardStats()

    const misCampanias: MiCampaniaListItem[] | undefined = campanias?.map((c) => ({
        id: c.id,
        titulo: c.titulo,
        imagenPrincipalUrl: c.imagenPrincipalUrl,
        estadoCampaniaId: c.estadoCampaniaId,
        estadoCampaniaNombre: CAMPANIA_ESTADOS_LABELS[c.estadoCampaniaId] || "",
        importeObjetivo: c.importeObjetivo,
        importeRecaudado: c.importePledgedActual || 0,
        porcentajeProgreso:
            c.importeObjetivo > 0
                ? ((c.importePledgedActual || 0) / c.importeObjetivo) * 100
                : 0,
        numBackers: 0,
        fechaCreacion: c.fechaCreacion,
    }))

    const totalRecaudado = dashboardStats?.totalRecaudado ??
        campanias?.reduce((acc, c) => acc + (c.importePledgedActual || 0), 0) ?? 0
    const totalBackers = dashboardStats?.totalBackers ?? 0
    const campaniasActivas = dashboardStats?.campaniasActivas ??
        (misCampanias?.filter((c) => c.estadoCampaniaId === 2).length ?? 0)
    const campaniasCompletadas = dashboardStats?.campaniasCompletadas ?? 0

    const firstActiveCampaniaId = campanias?.find(
        (c) => c.estadoCampaniaId === CAMPANIA_ESTADOS.PUBLICADA
    )?.id

    return (
        <div className="space-y-6">
            <CompleteProfileBanner />
            <PromotorBanner />

            {/* Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold">Dashboard</h1>
                    <p className="text-muted-foreground">
                        Bienvenido, {artista?.nombreArtistico || "Artista"}
                    </p>
                </div>
                <Link href="/campanias/nueva">
                    <Button>
                        <PlusCircle className="mr-2 h-4 w-4" />
                        Nueva campania
                    </Button>
                </Link>
            </div>

            {/* Error State */}
            {statsError && (
                <Alert variant="destructive">
                    <AlertCircle className="h-4 w-4" />
                    <AlertTitle>Error al cargar estadisticas</AlertTitle>
                    <AlertDescription>
                        No pudimos cargar las estadisticas.{" "}
                        <Button
                            variant="link"
                            className="p-0 h-auto"
                            onClick={() => refetchStats()}
                        >
                            Reintentar
                        </Button>
                    </AlertDescription>
                </Alert>
            )}

            {/* Stats Cards */}
            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
                {statsLoading ? (
                    <>
                        {[1, 2, 3, 4].map((i) => (
                            <Skeleton key={i} className="h-32 rounded-lg" />
                        ))}
                    </>
                ) : (
                    <>
                        <StatsCard
                            title="Total Recaudado"
                            value={formatCurrency(totalRecaudado)}
                            icon={TrendingUp}
                        />
                        <StatsCard
                            title="Backers Totales"
                            value={totalBackers.toString()}
                            icon={Users}
                        />
                        <StatsCard
                            title="Campanias Activas"
                            value={campaniasActivas.toString()}
                            icon={Music}
                        />
                        <StatsCard
                            title="Completadas"
                            value={campaniasCompletadas.toString()}
                            icon={BarChart3}
                        />
                    </>
                )}
            </div>

            {/* Chart Placeholder */}
            <Card>
                <CardHeader>
                    <CardTitle>Recaudacion ultimos 30 dias</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className="h-[200px] flex items-center justify-center text-muted-foreground">
                        <div className="text-center">
                            <BarChart3 className="h-12 w-12 mx-auto mb-4 opacity-50" />
                            <p className="text-lg font-medium">Grafica proximamente</p>
                        </div>
                    </div>
                </CardContent>
            </Card>

            {/* Campanias + Backings Grid */}
            <div className="grid gap-6 lg:grid-cols-2">
                <MisCampaniasCard
                    campanias={misCampanias}
                    isLoading={campaniasLoading}
                />
                <RecentBackings campaniaId={firstActiveCampaniaId} />
            </div>
        </div>
    )
}
