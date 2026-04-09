"use client"

import { useState } from "react"
import { useRouter, useParams } from "next/navigation"
import Link from "next/link"
import { toast } from "sonner"
import { Skeleton, Card, CardContent, CardHeader, CardTitle, Button } from "@/components/ui"
import {
    useCampania,
    usePublicarCampania,
    useDeleteCampania,
} from "@/hooks/use-campanias"
import { useCampaniaStats } from "@/hooks/use-campania-stats"
import { CAMPANIA_ESTADOS } from "@shared/constants"
import { formatCurrency } from "@shared/utils"
import { StatsCard } from "@/components/dashboard/stats-card"
import { ProgressBar } from "@/components/dashboard/ProgressBar"
import { PreviewLayout } from "../components/preview/PreviewLayout"
import { PublishConfirmModal } from "../components/preview/PublishConfirmModal"
import {
    TrendingUp,
    Users,
    Calendar,
    DollarSign,
    ChevronRight,
} from "lucide-react"

export default function CampaniaPreviewPage() {
    const params = useParams<{ id: string }>()
    const router = useRouter()
    const { data: campania, isLoading } = useCampania(params.id)
    const publishMutation = usePublicarCampania()
    const deleteMutation = useDeleteCampania()
    const [showPublishModal, setShowPublishModal] = useState(false)

    const isBorrador = campania?.estadoCampaniaId === CAMPANIA_ESTADOS.BORRADOR
    const {
        data: campaniaStats,
        isLoading: statsLoading,
    } = useCampaniaStats(isBorrador ? "" : params.id)

    if (isLoading) {
        return (
            <div className="space-y-6">
                <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
                    {[1, 2, 3, 4].map((i) => (
                        <Skeleton key={i} className="h-32 rounded-lg" />
                    ))}
                </div>
                <Skeleton className="h-24 rounded-lg" />
                <Skeleton className="w-full h-64 md:h-96 rounded-lg" />
                <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                    <div className="lg:col-span-2 space-y-4">
                        <Skeleton className="h-10 w-3/4" />
                        <Skeleton className="h-4 w-1/2" />
                        <Card className="bg-card border-border">
                            <CardContent className="p-6 space-y-3">
                                <Skeleton className="h-8 w-48" />
                                <Skeleton className="h-3 w-full" />
                                <Skeleton className="h-4 w-64" />
                            </CardContent>
                        </Card>
                    </div>
                    <div>
                        <Card className="bg-card border-border">
                            <CardContent className="p-6 space-y-4">
                                <Skeleton className="h-12 w-full" />
                                <Skeleton className="h-4 w-32" />
                                <Skeleton className="h-4 w-48" />
                            </CardContent>
                        </Card>
                    </div>
                </div>
            </div>
        )
    }

    if (!campania) {
        return (
            <div className="flex flex-col items-center justify-center py-16">
                <h2 className="text-xl font-semibold text-foreground mb-2">
                    Campania no encontrada
                </h2>
                <p className="text-muted-foreground mb-4">
                    La campania que buscas no existe o fue eliminada
                </p>
                <button
                    onClick={() => router.push("/campanias")}
                    className="text-primary hover:underline"
                >
                    Volver a mis campanias
                </button>
            </div>
        )
    }

    const handlePublish = async () => {
        try {
            await publishMutation.mutateAsync(params.id)
            toast.success("Campania publicada exitosamente")
            setShowPublishModal(false)
            router.push("/campanias")
        } catch {
            toast.error("Error al publicar la campania")
        }
    }

    const handleDelete = async () => {
        if (
            window.confirm(
                "Eliminar campania? Esta accion no se puede deshacer."
            )
        ) {
            try {
                await deleteMutation.mutateAsync(params.id)
                toast.success("Campania eliminada exitosamente")
                router.push("/campanias")
            } catch {
                toast.error("Error al eliminar la campania")
            }
        }
    }

    return (
        <div className="space-y-6">
            {/* Stats Section - only for published campaigns */}
            {!isBorrador && (
                <>
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
                                    title="Recaudado"
                                    value={formatCurrency(campaniaStats?.importeRecaudado ?? campania.importePledgedActual)}
                                    icon={TrendingUp}
                                />
                                <StatsCard
                                    title="Backers"
                                    value={(campaniaStats?.numBackers ?? 0).toString()}
                                    icon={Users}
                                />
                                <StatsCard
                                    title="Dias Restantes"
                                    value={campaniaStats?.diasRestantes?.toString() ?? "—"}
                                    icon={Calendar}
                                />
                                <StatsCard
                                    title="Promedio por Backing"
                                    value={formatCurrency(campaniaStats?.backingPromedio ?? 0)}
                                    icon={DollarSign}
                                />
                            </>
                        )}
                    </div>

                    {/* Progress Bar */}
                    <Card>
                        <CardHeader>
                            <CardTitle>Progreso de Meta</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <ProgressBar
                                current={campaniaStats?.importeRecaudado ?? campania.importePledgedActual}
                                goal={campaniaStats?.importeObjetivo ?? campania.importeObjetivo}
                            />
                        </CardContent>
                    </Card>

                    {/* Link to backings */}
                    <div className="flex justify-end">
                        <Link href={`/campanias/${params.id}/backings`}>
                            <Button variant="ghost" size="sm">
                                Ver todos los backers
                                <ChevronRight className="ml-2 h-4 w-4" />
                            </Button>
                        </Link>
                    </div>
                </>
            )}

            {/* Preview Layout */}
            <PreviewLayout
                campania={campania}
                showDraftBanner={isBorrador}
                onEdit={() =>
                    router.push(`/campanias/${params.id}/editar`)
                }
                onPublish={() => setShowPublishModal(true)}
                onDelete={handleDelete}
                onViewBackings={() =>
                    router.push(
                        `/campanias/${params.id}/backings`
                    )
                }
            />

            <PublishConfirmModal
                isOpen={showPublishModal}
                onClose={() => setShowPublishModal(false)}
                onConfirm={handlePublish}
                hasRewards={false}
                isPublishing={publishMutation.isPending}
            />
        </div>
    )
}
