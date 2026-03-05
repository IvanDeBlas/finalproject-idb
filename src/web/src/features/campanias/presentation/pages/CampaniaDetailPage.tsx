import { useState, useCallback } from "react"
import { useParams, Link } from "react-router-dom"
import { useCampania } from "../../application/useCampanias"
import { useRewardsByCampania } from "../../application/hooks/useRewardsByCampania"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { CAMPANIA_ESTADOS_LABELS, CAMPANIA_ESTADOS } from "@/lib/constants"
import { cn } from "@/lib/utils"
import { SearchX } from "lucide-react"
import { CampaniaHero } from "../components/CampaniaHero"
import { CampaniaStats } from "../components/CampaniaStats"
import { CampaniaTabs } from "../components/CampaniaTabs"
import { CampaniaRewardsSection } from "../components/CampaniaRewardsSection"
import { ArtistCard } from "../components/ArtistCard"
import { CampaniaDetailSkeleton } from "../components/CampaniaDetailSkeleton"
import { BackingModal } from "@/features/backings/presentation/components/BackingModal"
import type { Reward } from "@shared/types/reward"

function getEstadoBadgeClasses(estadoCampaniaId: number): string {
    switch (estadoCampaniaId) {
        case 2: return "bg-green-500/20 text-green-400 border-green-500/50"
        case 3: return "bg-blue-500/20 text-blue-400 border-blue-500/50"
        case 4: return "bg-red-500/20 text-red-400 border-red-500/50"
        default: return "bg-[#64748b]/20 text-[#64748b] border-[#64748b]/50"
    }
}

export default function CampaniaDetailPage() {
    const { id } = useParams<{ id: string }>()
    const { data: campania, isLoading, error, refetch } = useCampania(id || "")
    const { data: rewards } = useRewardsByCampania(id || "")
    const [showBackingModal, setShowBackingModal] = useState(false)
    const [selectedReward, setSelectedReward] = useState<Reward | null>(null)

    const handleApoyar = useCallback(() => {
        setSelectedReward(null)
        setShowBackingModal(true)
    }, [])

    const handleSelectReward = useCallback((rewardId: string) => {
        const reward = rewards?.find((r) => r.id === rewardId) || null
        setSelectedReward(reward)
        setShowBackingModal(true)
    }, [rewards])

    if (isLoading) {
        return <CampaniaDetailSkeleton />
    }

    if (error || !campania) {
        return (
            <div className="min-h-screen bg-[#1a1a2e]">
                <div className="max-w-2xl mx-auto px-6 py-24 text-center">
                    <SearchX className="w-24 h-24 text-[#64748b] mx-auto mb-6" />
                    <h1 className="text-3xl font-bold text-white mb-4">
                        Campana no encontrada
                    </h1>
                    <p className="text-lg text-[#94a3b8] mb-8">
                        Esta campana no existe o fue eliminada
                    </p>
                    <div className="flex gap-4 justify-center">
                        <Button
                            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white"
                            asChild
                        >
                            <Link to="/explorar">Explorar campanas</Link>
                        </Button>
                        {error && (
                            <Button
                                variant="outline"
                                className="border-[#334155] text-[#94a3b8] hover:text-white"
                                onClick={() => refetch()}
                            >
                                Reintentar
                            </Button>
                        )}
                    </div>
                </div>
            </div>
        )
    }

    const estadoCampaniaId = campania.estadoCampaniaId
    const estadoLabel = CAMPANIA_ESTADOS_LABELS[estadoCampaniaId] ?? "Activa"
    const isFinalizada = estadoCampaniaId === CAMPANIA_ESTADOS.FINALIZADA || estadoCampaniaId === CAMPANIA_ESTADOS.CANCELADA
    const imageUrl = campania.imagenPrincipalUrl || campania.imagenUrl || "/placeholder.jpg"
    const monedaId = campania.monedaId ?? 1

    return (
        <div className="min-h-screen bg-[#1a1a2e]">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6 sm:py-8">
                <CampaniaHero
                    imagenUrl={imageUrl}
                    videoUrl={campania.videoPrincipalUrl}
                    titulo={campania.titulo}
                    alt={`Imagen de portada de ${campania.titulo}`}
                    className="mb-8"
                />

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                    <div className="lg:col-span-2 space-y-8">
                        <div>
                            <h1 className="text-3xl sm:text-4xl font-bold text-white mb-4">
                                {campania.titulo}
                            </h1>

                            {campania.subtitulo && (
                                <p className="text-lg text-[#94a3b8] mb-4">
                                    {campania.subtitulo}
                                </p>
                            )}

                            <div className="flex items-center gap-3 flex-wrap">
                                <Badge
                                    variant="outline"
                                    className={cn("text-xs", getEstadoBadgeClasses(estadoCampaniaId))}
                                    role="status"
                                    aria-label={`Estado de campana: ${estadoLabel}`}
                                >
                                    {estadoLabel}
                                </Badge>
                            </div>
                        </div>

                        <CampaniaStats
                            importeObjetivo={campania.importeObjetivo}
                            importePledgedActual={campania.importePledgedActual}
                            monedaId={monedaId}
                            fechaFin={campania.fechaFin}
                            backersCount={campania.backersCount}
                        />

                        <CampaniaTabs
                            descripcion={campania.descripcion}
                            className="mb-8"
                        />

                        <ArtistCard
                            artistaId={campania.artistaId}
                            nombre={`Artista`}
                            bio="Creador de esta campana musical."
                            className="mt-8"
                        />
                    </div>

                    <div className="lg:col-span-1">
                        <CampaniaRewardsSection
                            campaniaId={campania.id}
                            monedaId={monedaId}
                            campaniaFinalizada={isFinalizada}
                            onApoyar={isFinalizada ? undefined : handleApoyar}
                            onSelectReward={isFinalizada ? undefined : handleSelectReward}
                        />
                    </div>
                </div>
            </div>

            {/* Backing Modal */}
            {!isFinalizada && (
                <BackingModal
                    open={showBackingModal}
                    onOpenChange={setShowBackingModal}
                    campaniaId={campania.id}
                    campaniaTitulo={campania.titulo}
                    reward={selectedReward}
                />
            )}
        </div>
    )
}
