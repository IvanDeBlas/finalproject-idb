import { Card } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Package, AlertCircle, XCircle } from "lucide-react"
import { cn } from "@/lib/utils"
import { formatCurrency } from "../../application/utils"
import type { Reward } from "@shared/types/reward"

interface RewardPublicCardProps {
    reward: Reward
    monedaId: number
    onSelect?: (rewardId: string) => void
    disabled?: boolean
    isPopular?: boolean
    className?: string
}

export function RewardPublicCard({
    reward,
    monedaId,
    onSelect,
    disabled = false,
    isPopular = false,
    className,
}: RewardPublicCardProps) {
    const isUnlimited = reward.cantidadMaxima === undefined || reward.cantidadMaxima === null
    const cantidadDisponible = isUnlimited ? null : reward.cantidadMaxima!
    const isSoldOut = !isUnlimited && cantidadDisponible !== null && cantidadDisponible <= 0
    const isLimited = !isUnlimited && cantidadDisponible !== null && cantidadDisponible < (reward.cantidadMaxima! * 0.5)
    const isDisabled = disabled || isSoldOut

    const getStockText = (): string => {
        if (isUnlimited) return "Ilimitadas disponibles"
        if (isSoldOut) return "Agotado"
        return `${cantidadDisponible} de ${reward.cantidadMaxima} disponibles`
    }

    return (
        <Card
            className={cn(
                "bg-[#1a1a2e] border-[#334155] p-3 sm:p-4 transition group",
                !isDisabled && "hover:border-primary cursor-pointer",
                isSoldOut && "opacity-60 cursor-not-allowed",
                className
            )}
            data-testid="reward-card"
            role="article"
            aria-labelledby={`reward-${reward.id}-title`}
            aria-describedby={`reward-${reward.id}-stock`}
            tabIndex={isDisabled ? -1 : 0}
            onClick={() => !isDisabled && onSelect?.(reward.id)}
            onKeyDown={(e) => {
                if (e.key === "Enter" && !isDisabled) {
                    onSelect?.(reward.id)
                }
            }}
        >
            <div className="flex items-start justify-between mb-2">
                <div className="text-lg sm:text-xl font-bold text-primary">
                    {formatCurrency(reward.importeMinimo, monedaId)}
                </div>
                <div className="flex gap-1">
                    {isPopular && !isSoldOut && (
                        <Badge
                            className="bg-pink-500/20 text-pink-400 border-pink-500/50 text-xs"
                            role="status"
                            aria-label="Recompensa mas popular"
                        >
                            Mas popular
                        </Badge>
                    )}
                    {isLimited && !isSoldOut && (
                        <Badge
                            className="bg-amber-500/20 text-amber-400 border-amber-500/50 text-xs"
                            role="status"
                            aria-label="Pocas unidades disponibles"
                        >
                            Pocas unidades
                        </Badge>
                    )}
                    {isSoldOut && (
                        <Badge
                            className="bg-[#64748b]/20 text-[#64748b] border-[#64748b]/50 text-xs"
                            role="status"
                            aria-label="Recompensa agotada"
                        >
                            AGOTADO
                        </Badge>
                    )}
                </div>
            </div>

            <h4
                id={`reward-${reward.id}-title`}
                className="text-white font-semibold mb-1"
            >
                {reward.nombre}
            </h4>

            {reward.descripcion && (
                <p className="text-sm text-[#94a3b8] mb-2 line-clamp-2">
                    {reward.descripcion}
                </p>
            )}

            <p
                id={`reward-${reward.id}-stock`}
                className="text-xs mb-3 flex items-center gap-1"
                role="status"
                aria-label={`Stock: ${getStockText()}`}
            >
                {isSoldOut ? (
                    <>
                        <XCircle className="w-4 h-4 text-[#64748b]" aria-hidden="true" />
                        <span className="text-[#64748b]">Agotado</span>
                    </>
                ) : isLimited ? (
                    <>
                        <AlertCircle className="w-4 h-4 text-amber-400" aria-hidden="true" />
                        <span className="text-amber-400">{getStockText()}</span>
                    </>
                ) : (
                    <>
                        <Package className="w-4 h-4 text-green-400" aria-hidden="true" />
                        <span className="text-[#94a3b8]">{getStockText()}</span>
                    </>
                )}
            </p>

            {reward.tiempoEntregaEstimado && (
                <p className="text-xs text-[#64748b] mb-3">
                    Entrega estimada: {reward.tiempoEntregaEstimado}
                </p>
            )}

            <Button
                variant="outline"
                size="sm"
                className={cn(
                    "w-full",
                    isSoldOut
                        ? "border-[#64748b]/50 text-[#64748b] cursor-not-allowed"
                        : "border-primary text-primary hover:bg-primary/10 group-hover:bg-primary/10"
                )}
                disabled={isDisabled}
                onClick={(e) => {
                    e.stopPropagation()
                    if (!isDisabled) onSelect?.(reward.id)
                }}
                aria-label={
                    isSoldOut
                        ? "Recompensa agotada"
                        : `Seleccionar recompensa ${reward.nombre} por ${formatCurrency(reward.importeMinimo, monedaId)}`
                }
            >
                {isSoldOut ? "Agotado" : "Seleccionar"}
            </Button>
        </Card>
    )
}
