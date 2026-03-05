"use client"

import { useState } from "react"
import { useParams, useRouter } from "next/navigation"
import { ArrowLeft, Download, PackageOpen } from "lucide-react"
import { toast } from "sonner"
import { Button, Skeleton } from "@/components/ui"
import { useCampania } from "@/hooks/use-campanias"
import { useCampaignBackings, useCampaignStats } from "@/hooks/use-backings"
import { formatDateShort } from "@shared/utils"
import { BackingStatsGrid } from "./components/BackingStatsGrid"
import { BackingsTable } from "./components/BackingsTable"
import { EmptyState } from "@/app/(dashboard)/campanias/components/shared/EmptyState"

export default function CampaignBackingsPage() {
    const params = useParams<{ id: string }>()
    const router = useRouter()
    const campaniaId = params.id

    const { data: campania, isLoading: campaniaLoading } =
        useCampania(campaniaId)
    const { data: stats, isLoading: statsLoading } =
        useCampaignStats(campaniaId)
    const {
        data: backings = [],
        isLoading: backingsLoading,
        error: backingsError,
    } = useCampaignBackings(campaniaId)

    const [searchTerm, setSearchTerm] = useState("")
    const [selectedReward, setSelectedReward] = useState<string | null>(null)

    const isLoading = campaniaLoading || statsLoading || backingsLoading

    const handleExportCSV = () => {
        try {
            const csv = [
                ["Backer", "Monto", "Recompensa", "Fecha", "Estado"].join(","),
                ...backings.map((b) =>
                    [
                        b.nombreBacker,
                        b.monto.toFixed(2),
                        b.rewardNombre ?? "Sin recompensa",
                        formatDateShort(b.fechaCreacion),
                        "Confirmado",
                    ].join(",")
                ),
            ].join("\n")

            const blob = new Blob([csv], { type: "text/csv" })
            const url = URL.createObjectURL(blob)
            const a = document.createElement("a")
            a.href = url
            a.download = `backings-${campaniaId}-${Date.now()}.csv`
            a.click()
            URL.revokeObjectURL(url)

            toast.success("CSV descargado exitosamente")
        } catch {
            toast.error("Error al exportar CSV")
        }
    }

    if (!isLoading && !campania) {
        return (
            <div className="container max-w-5xl mx-auto py-8 px-4 md:px-6">
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
            </div>
        )
    }

    return (
        <div className="container max-w-5xl mx-auto py-8 px-4 md:px-6">
            {/* Header */}
            <div className="mb-6">
                <Button
                    variant="ghost"
                    onClick={() =>
                        router.push(`/dashboard/campanias/${campaniaId}`)
                    }
                    className="flex items-center gap-2 text-muted-foreground hover:text-foreground mb-4 transition"
                >
                    <ArrowLeft className="w-4 h-4" />
                    Volver a Campania
                </Button>

                <div className="flex items-center justify-between">
                    <div>
                        <h1 className="text-2xl md:text-3xl font-bold text-foreground">
                            Apoyos Recibidos
                        </h1>
                        {campaniaLoading ? (
                            <Skeleton className="h-4 w-48 mt-1" />
                        ) : (
                            <p className="text-sm text-muted-foreground mt-1">
                                {campania?.titulo}
                            </p>
                        )}
                    </div>

                    {backings.length > 0 && (
                        <Button
                            variant="outline"
                            onClick={handleExportCSV}
                            className="border-primary text-primary hover:bg-primary/10"
                        >
                            <Download className="w-4 h-4 mr-2" />
                            Exportar CSV
                        </Button>
                    )}
                </div>
            </div>

            {/* Stats Grid */}
            <div className="mb-6">
                <BackingStatsGrid stats={stats} isLoading={statsLoading} />
            </div>

            {/* Error State */}
            {backingsError && (
                <div className="text-center py-8">
                    <p className="text-destructive mb-4">
                        Error al cargar los apoyos. Intenta nuevamente.
                    </p>
                    <Button
                        variant="outline"
                        onClick={() => window.location.reload()}
                    >
                        Reintentar
                    </Button>
                </div>
            )}

            {/* Empty State */}
            {!backingsLoading && !backingsError && backings.length === 0 && (
                <EmptyState
                    icon={<PackageOpen className="w-20 h-20" />}
                    title="No hay apoyos aun"
                    description="Cuando recibas tus primeros aportes, apareceran aqui."
                    actionLabel="Volver a Campania"
                    onAction={() =>
                        router.push(`/dashboard/campanias/${campaniaId}`)
                    }
                />
            )}

            {/* Backings Table */}
            {(backingsLoading || backings.length > 0) && !backingsError && (
                <BackingsTable
                    backings={backings}
                    isLoading={backingsLoading}
                    searchTerm={searchTerm}
                    onSearchChange={setSearchTerm}
                    selectedReward={selectedReward}
                    onRewardFilterChange={setSelectedReward}
                />
            )}
        </div>
    )
}
