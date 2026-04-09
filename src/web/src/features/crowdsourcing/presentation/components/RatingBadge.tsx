import { FC } from "react"
import { Star } from "lucide-react"
import { Skeleton } from "@/components/ui/skeleton"
import { cn } from "@/lib/utils"
import { StarDisplay } from "./StarDisplay"

interface RatingBadgeProps {
    puntuacionMedia: number | null
    totalValoraciones: number
    variant?: "compact" | "medium"
    isLoading?: boolean
    className?: string
}

export const RatingBadge: FC<RatingBadgeProps> = ({
    puntuacionMedia,
    totalValoraciones,
    variant = "compact",
    isLoading = false,
    className,
}) => {
    if (isLoading) {
        return (
            <Skeleton className="h-5 w-[60px] rounded bg-[#1e2a42] animate-pulse" />
        )
    }

    if (totalValoraciones === 0 || puntuacionMedia === null) return null

    if (variant === "compact") {
        return (
            <div className={cn("flex items-center gap-1", className)}>
                <Star
                    className="w-3.5 h-3.5 text-[#f59e0b] flex-shrink-0"
                    fill="currentColor"
                    aria-hidden="true"
                />
                <span className="text-sm font-semibold text-white">
                    {puntuacionMedia.toFixed(1)}
                </span>
                <span className="text-xs text-[#64748b]">
                    ({totalValoraciones})
                </span>
            </div>
        )
    }

    return (
        <div className={cn("flex items-center gap-2", className)}>
            <StarDisplay value={puntuacionMedia} size="sm" />
            <span className="text-base font-semibold text-white">
                {puntuacionMedia.toFixed(1)}
            </span>
            <span className="text-sm text-[#94a3b8]">
                ({totalValoraciones} valoraciones)
            </span>
        </div>
    )
}
