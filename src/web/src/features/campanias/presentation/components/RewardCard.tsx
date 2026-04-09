import { Card } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Package, AlertCircle, XCircle } from "lucide-react"
import { cn } from "@/lib/utils"
import { formatCurrency } from "../../application/utils"
import type { Reward } from "@shared/types/reward"

interface RewardCardProps {
    reward: Reward
    monedaId: number
    onSelect?: (rewardId: string) => void
    disabled?: boolean
    className?: string
}

export function RewardCard({
    reward,
    monedaId,
    onSelect,
    disabled = false,
    className,
}: RewardCardProps) {
    const isUnlimited = reward.cantidadMaxima === undefined || reward.cantidadMaxima === null
    const cantidadDisponible = isUnlimited ? null : reward.cantidadMaxima!
    const isSoldOut = !isUnlimited && cantidadDisponible !== null && cantidadDisponible <= 0
    const isLimited = !isUnlimited && cantidadDisponible !== null && cantidadDisponible < (reward.cantidadMaxima! * 0.5)
    const isDisabled = disabled || isSoldOut

    return (
        <Card
            className={cn(
                "bg-[#1a1a2e] border-[#334155] p-4 transition",
                !isDisabled && "hover:border-[#a855f7] cursor-pointer",
                isDisabled && "opacity-50 cursor-not-allowed",
                className
            )}
            role="article"
            aria-label={`Recompensa: ${reward.nombre} por ${formatCurrency(reward.importeMinimo, monedaId)}`}
            tabIndex={isDisabled ? -1 : 0}
            onClick={() => !isDisabled && onSelect?.(reward.id)}
            onKeyDown={(e) => {
                if (e.key === "Enter" && !isDisabled) {
                    onSelect?.(reward.id)
                }
            }}
        >
            <div className="flex items-start justify-between mb-2">
                <div className="text-xl font-bold text-primary">
                    {formatCurrency(reward.importeMinimo, monedaId)}
                </div>
                <div className="flex gap-1">
                    {isLimited && !isSoldOut && (
                        <Badge className="bg-amber-500/20 text-amber-400 border-amber-500/50 text-xs">
                            Pocas unidades
                        </Badge>
                    )}
                    {isSoldOut && (
                        <Badge className="bg-[#64748b]/20 text-[#64748b] border-[#64748b]/50 text-xs">
                            AGOTADO
                        </Badge>
                    )}
                </div>
            </div>

            <h4 className="text-white font-semibold mb-1">{reward.nombre}</h4>

            {reward.descripcion && (
                <p className="text-sm text-[#94a3b8] mb-2 line-clamp-2">
                    {reward.descripcion}
                </p>
            )}

            <p className="text-xs text-[#64748b] mb-3 flex items-center gap-1">
                {isSoldOut ? (
                    <>
                        <XCircle className="w-4 h-4 text-[#64748b]" aria-hidden="true" />
                        <span>Agotado</span>
                    </>
                ) : isLimited ? (
                    <>
                        <AlertCircle className="w-4 h-4 text-amber-400" aria-hidden="true" />
                        <span className="text-amber-400">
                            {cantidadDisponible} de {reward.cantidadMaxima} disponibles
                        </span>
                    </>
                ) : isUnlimited ? (
                    <>
                        <Package className="w-4 h-4 text-green-400" aria-hidden="true" />
                        <span>Ilimitadas disponibles</span>
                    </>
                ) : (
                    <>
                        <Package className="w-4 h-4 text-green-400" aria-hidden="true" />
                        <span>
                            {cantidadDisponible} de {reward.cantidadMaxima} disponibles
                        </span>
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
                className="w-full border-primary text-primary hover:bg-primary/10"
                disabled={isDisabled}
                onClick={(e) => {
                    e.stopPropagation()
                    if (!isDisabled) onSelect?.(reward.id)
                }}
            >
                {isSoldOut ? "Agotado" : "Seleccionar"}
            </Button>
        </Card>
    )
}
