import { useState, useCallback } from "react"
import { useParams, Link } from "react-router-dom"
import { useCampania } from "../../application/useCampanias"
import { useRewardsByCampania } from "../../application/hooks/useRewardsByCampania"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { CAMPANIA_ESTADOS_LABELS, CAMPANIA_ESTADOS } from "@/lib/constants"
import { cn } from "@/lib/utils"
import { SearchX, DollarSign, Lightbulb, Megaphone } from "lucide-react"
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

                            <div className="flex items-center gap-2 flex-wrap">
                                <Badge
                                    variant="outline"
                                    className={cn("text-xs", getEstadoBadgeClasses(estadoCampaniaId))}
                                    role="status"
                                    aria-label={`Estado de campana: ${estadoLabel}`}
                                >
                                    {estadoLabel}
                                </Badge>
                                <Badge variant="outline" className="text-xs bg-amber-500/15 text-amber-400 border-amber-500/40 gap-1">
                                    <DollarSign className="w-3 h-3" />
                                    Crowdfunding
                                </Badge>
                                {campania.proyectoArtisticoId && (
                                    <>
                                        <Badge variant="outline" className="text-xs bg-cyan-500/15 text-cyan-400 border-cyan-500/40 gap-1">
                                            <Lightbulb className="w-3 h-3" />
                                            Crowdsourcing
                                        </Badge>
                                        <Badge variant="outline" className="text-xs bg-fuchsia-500/15 text-fuchsia-400 border-fuchsia-500/40 gap-1">
                                            <Megaphone className="w-3 h-3" />
                                            Crowdpromotion
                                        </Badge>
                                    </>
                                )}
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
                        <Tabs defaultValue="rewards" className="w-full">
                            <TabsList className="w-full bg-[#0f1729] border border-[#334155]">
                                <TabsTrigger value="rewards" className="group/tab flex-1 gap-1.5 data-[state=active]:bg-amber-500/20 data-[state=active]:text-amber-400 text-[#64748b]">
                                    <DollarSign className="w-4 h-4 shrink-0" />
                                    <span className="hidden group-data-[state=active]/tab:inline text-sm">Recompensas</span>
                                </TabsTrigger>
                                {campania.proyectoArtisticoId && (
                                    <TabsTrigger value="sourcing" className="group/tab flex-1 gap-1.5 data-[state=active]:bg-cyan-500/20 data-[state=active]:text-cyan-400 text-[#64748b]">
                                        <Lightbulb className="w-4 h-4 shrink-0" />
                                        <span className="hidden group-data-[state=active]/tab:inline text-sm">Necesidades</span>
                                    </TabsTrigger>
                                )}
                                {campania.proyectoArtisticoId && (
                                    <TabsTrigger value="promo" className="group/tab flex-1 gap-1.5 data-[state=active]:bg-fuchsia-500/20 data-[state=active]:text-fuchsia-400 text-[#64748b]">
                                        <Megaphone className="w-4 h-4 shrink-0" />
                                        <span className="hidden group-data-[state=active]/tab:inline text-sm">Promocion</span>
                                    </TabsTrigger>
                                )}
                            </TabsList>
                            <TabsContent value="rewards">
                                <CampaniaRewardsSection
                                    campaniaId={campania.id}
                                    monedaId={monedaId}
                                    campaniaFinalizada={isFinalizada}
                                    onApoyar={isFinalizada ? undefined : handleApoyar}
                                    onSelectReward={isFinalizada ? undefined : handleSelectReward}
                                />
                            </TabsContent>
                            {campania.proyectoArtisticoId && (
                                <TabsContent value="sourcing">
                                    <div className="bg-[#0f1729] border border-[#334155] rounded-lg p-6 space-y-4">
                                        <h3 className="text-lg font-semibold text-white">Necesidades del proyecto</h3>
                                        <p className="text-sm text-[#94a3b8]">
                                            Este proyecto busca profesionales para colaborar. Consulta las necesidades abiertas y envia tu propuesta.
                                        </p>
                                        <Button
                                            variant="outline"
                                            className="w-full border-cyan-500/40 text-cyan-400 hover:bg-cyan-500/10"
                                            asChild
                                        >
                                            <Link to="/crowdsourcing/necesidades">
                                                <Lightbulb className="w-4 h-4 mr-2" />
                                                Ver necesidades abiertas
                                            </Link>
                                        </Button>
                                    </div>
                                </TabsContent>
                            )}
                            {campania.proyectoArtisticoId && (
                                <TabsContent value="promo">
                                    <div className="bg-[#0f1729] border border-[#334155] rounded-lg p-6 space-y-4">
                                        <h3 className="text-lg font-semibold text-white">Programa de promocion</h3>
                                        <p className="text-sm text-[#94a3b8]">
                                            Ayuda a difundir este proyecto y gana comisiones por cada backer que refieras.
                                        </p>
                                        <Button
                                            variant="outline"
                                            className="w-full border-fuchsia-500/40 text-fuchsia-400 hover:bg-fuchsia-500/10"
                                            asChild
                                        >
                                            <Link to="/crowdpromotion/explorar">
                                                <Megaphone className="w-4 h-4 mr-2" />
                                                Ver programas de promocion
                                            </Link>
                                        </Button>
                                    </div>
                                </TabsContent>
                            )}
                        </Tabs>
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
