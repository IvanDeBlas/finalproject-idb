import { FC } from "react"
import { Star } from "lucide-react"
import { cn } from "@/lib/utils"

interface StarDisplayProps {
    value: number
    size?: "sm" | "md" | "lg"
    showNumeric?: boolean
    className?: string
}

const SIZE_CONFIG = {
    sm: { icon: "w-3.5 h-3.5", gap: "gap-0.5" },
    md: { icon: "w-4 h-4", gap: "gap-1" },
    lg: { icon: "w-5 h-5", gap: "gap-1" },
} as const

function getStarType(
    starIndex: number,
    value: number
): "full" | "half" | "empty" {
    const rounded = Math.round(value * 2) / 2
    if (starIndex <= Math.floor(rounded)) return "full"
    if (starIndex === Math.ceil(rounded) && rounded % 1 !== 0) return "half"
    return "empty"
}

export const StarDisplay: FC<StarDisplayProps> = ({
    value,
    size = "md",
    showNumeric = false,
    className,
}) => {
    const config = SIZE_CONFIG[size]

    return (
        <div
            role="img"
            aria-label={`Puntuacion: ${value} de 5 estrellas`}
            className={cn("flex items-center", config.gap, className)}
        >
            {[1, 2, 3, 4, 5].map((i) => {
                const starType = getStarType(i, value)
                return (
                    <Star
                        key={i}
                        className={cn(
                            config.icon,
                            starType === "empty"
                                ? "text-[#334155]"
                                : "text-[#f59e0b]",
                            starType === "half" && "opacity-50"
                        )}
                        fill="currentColor"
                        aria-hidden="true"
                    />
                )
            })}
            {showNumeric && (
                <span
                    className="text-sm text-[#94a3b8] ml-2"
                    aria-hidden="true"
                >
                    {value} / 5
                </span>
            )}
        </div>
    )
}
