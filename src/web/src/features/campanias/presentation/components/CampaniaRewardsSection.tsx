import { useMemo } from "react"
import { Card } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { Lock, Package, AlertCircle } from "lucide-react"
import { cn } from "@/lib/utils"
import { formatCurrency } from "../../application/utils"
import { useRewardsByCampania } from "../../application/hooks/useRewardsByCampania"
import { RewardPublicCard } from "./RewardPublicCard"
import { RewardPublicCardSkeleton } from "./RewardPublicCardSkeleton"

interface CampaniaRewardsSectionProps {
    campaniaId: string
    monedaId: number
    onApoyar?: () => void
    onSelectReward?: (rewardId: string) => void
    campaniaFinalizada?: boolean
    className?: string
}

export function CampaniaRewardsSection({
    campaniaId,
    monedaId,
    onApoyar,
    onSelectReward,
    campaniaFinalizada = false,
    className,
}: CampaniaRewardsSectionProps) {
    const { data: rewards, isLoading, isError, refetch } = useRewardsByCampania(campaniaId)

    const minRewardAmount = useMemo(() => {
        if (!rewards || rewards.length === 0) return 5
        return Math.min(...rewards.map((r) => r.importeMinimo))
    }, [rewards])

    const mostPopularRewardId = useMemo(() => {
        if (!rewards || rewards.length === 0) return null
        // For MVP, mark as popular if first in the sorted list and there are multiple rewards
        // Real logic would use backingsCount when available
        return rewards.length > 1 ? rewards[0]?.id ?? null : null
    }, [rewards])

    const earliestDelivery = useMemo(() => {
        if (!rewards) return null
        const withDelivery = rewards.filter((r) => r.tiempoEntregaEstimado)
        return withDelivery[0]?.tiempoEntregaEstimado ?? null
    }, [rewards])

    const handleSelectReward = (rewardId: string) => {
        if (onSelectReward) {
            onSelectReward(rewardId)
        } else {
            onApoyar?.()
        }
    }

    return (
        <Card
            className={cn(
                "bg-[#0f1729] border-[#334155] p-4 sm:p-6",
                "sm:sticky sm:top-24",
                className
            )}
            role="region"
            aria-labelledby="rewards-section-title"
        >
            <Button
                className={cn(
                    "w-full font-semibold py-2 sm:py-3 mb-2 text-white",
                    campaniaFinalizada
                        ? "bg-[#334155] cursor-not-allowed"
                        : "bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700"
                )}
                size="lg"
                onClick={onApoyar}
                disabled={campaniaFinalizada}
                aria-label={campaniaFinalizada ? "Campana finalizada" : "Apoyar esta campana"}
            >
                {campaniaFinalizada ? "Campana finalizada" : "Apoyar esta campana"}
            </Button>

            {!campaniaFinalizada && !isLoading && (
                <p className="text-center text-sm text-[#94a3b8] mb-6">
                    Desde {formatCurrency(minRewardAmount, monedaId)}
                </p>
            )}

            <Separator className="my-6 bg-[#334155]" />

            <h3
                id="rewards-section-title"
                className="text-lg font-bold text-white mb-4"
            >
                Recompensas
            </h3>

            {isLoading ? (
                <RewardPublicCardSkeleton count={3} />
            ) : isError ? (
                <div className="text-center py-8">
                    <AlertCircle className="w-12 h-12 text-red-500 mx-auto mb-3" />
                    <p className="text-sm text-red-400 mb-3">
                        Error al cargar las recompensas
                    </p>
                    <Button
                        variant="outline"
                        size="sm"
                        className="border-[#334155] text-[#94a3b8] hover:text-white"
                        onClick={() => refetch()}
                    >
                        Intentar de nuevo
                    </Button>
                </div>
            ) : rewards && rewards.length > 0 ? (
                <div className="space-y-2 sm:space-y-3">
                    {rewards.map((reward) => (
                        <RewardPublicCard
                            key={reward.id}
                            reward={reward}
                            monedaId={monedaId}
                            onSelect={handleSelectReward}
                            disabled={campaniaFinalizada}
                            isPopular={reward.id === mostPopularRewardId}
                        />
                    ))}
                </div>
            ) : (
                <div className="text-center py-8">
                    <Package className="w-12 h-12 text-[#64748b] mx-auto mb-3" />
                    <p className="text-sm text-[#64748b]">
                        Esta campana no tiene recompensas especificas
                    </p>
                    <p className="text-xs text-[#64748b] mt-2">
                        Puedes hacer una contribucion libre
                    </p>
                </div>
            )}

            <Separator className="my-6 bg-[#334155]" />

            <div className="space-y-2 text-xs text-[#64748b]">
                <div className="flex items-center gap-2">
                    <Lock className="w-4 h-4" aria-hidden="true" />
                    <span>Pago seguro</span>
                </div>
                {earliestDelivery && (
                    <div className="flex items-center gap-2">
                        <Package className="w-4 h-4" aria-hidden="true" />
                        <span>Entrega estimada: {earliestDelivery}</span>
                    </div>
                )}
            </div>
        </Card>
    )
}
