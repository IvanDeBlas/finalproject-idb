"use client"

import { cn } from "@/lib/utils"
import { Progress } from "@/components/ui/progress"
import { Badge } from "@/components/ui/badge"
import { formatCurrency } from "@shared/utils"
import { CheckCircle } from "lucide-react"

interface ProgressBarProps {
    current: number
    goal: number
    showPercentage?: boolean
    showAmount?: boolean
    className?: string
}

export function ProgressBar({
    current,
    goal,
    showPercentage = true,
    showAmount = true,
    className,
}: ProgressBarProps) {
    const percentage = goal > 0
        ? Math.round((current / goal) * 10000) / 100
        : 0
    const clampedPercentage = Math.min(percentage, 100)
    const colorClass =
        percentage >= 100
            ? "text-green-500"
            : percentage >= 50
                ? "text-yellow-500"
                : "text-red-500"

    return (
        <div className={cn("space-y-2", className)}>
            <div className="flex items-center justify-between text-sm">
                {showAmount && (
                    <span className="text-muted-foreground">
                        {formatCurrency(current)} de {formatCurrency(goal)}
                    </span>
                )}
                {showPercentage && (
                    <span className={cn("font-semibold", colorClass)}>
                        {percentage.toFixed(2)}%
                    </span>
                )}
            </div>
            <Progress
                value={clampedPercentage}
                className="h-3"
                aria-label="Progreso de meta"
            />
            {percentage >= 100 && (
                <Badge className="bg-green-500/20 text-green-400 border-green-500/30">
                    <CheckCircle className="mr-1 h-3 w-3" />
                    Meta alcanzada
                </Badge>
            )}
        </div>
    )
}
