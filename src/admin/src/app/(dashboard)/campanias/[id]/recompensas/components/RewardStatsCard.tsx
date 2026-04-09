"use client"

import { Package, CheckCircle, TrendingUp } from "lucide-react"
import { Card } from "@/components/ui"
import type { Reward } from "@shared/types"

interface RewardStatsCardProps {
    rewards: Reward[]
}

export function RewardStatsCard({ rewards }: RewardStatsCardProps) {
    const total = rewards.length
    const activas = rewards.filter((r) => r.esActivo).length
    const stockTotal = rewards.reduce((sum, r) => {
        if (r.cantidadMaxima != null) {
            return sum + Math.max(0, r.cantidadMaxima - (r.cantidadMaxima - (r.cantidadMaxima ?? 0)))
        }
        return sum
    }, 0)

    const hasLimitedStock = rewards.some((r) => r.cantidadMaxima != null)

    return (
        <Card className="bg-card border-border p-4 mb-6">
            <div className="flex flex-wrap items-center gap-4 sm:gap-6 text-sm">
                <div className="flex items-center gap-2">
                    <Package className="w-4 h-4 text-primary" />
                    <span className="text-muted-foreground">Total:</span>
                    <span className="font-semibold text-foreground">
                        {total}
                    </span>
                </div>
                <div className="flex items-center gap-2">
                    <CheckCircle className="w-4 h-4 text-primary" />
                    <span className="text-muted-foreground">Activas:</span>
                    <span className="font-semibold text-foreground">
                        {activas}
                    </span>
                </div>
                <div className="flex items-center gap-2">
                    <TrendingUp className="w-4 h-4 text-primary" />
                    <span className="text-muted-foreground">
                        Stock disponible:
                    </span>
                    <span className="font-semibold text-foreground">
                        {hasLimitedStock
                            ? `${stockTotal} unidades`
                            : "Ilimitado"}
                    </span>
                </div>
            </div>
        </Card>
    )
}
