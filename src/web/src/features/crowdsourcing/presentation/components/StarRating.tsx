import { FC, useState, KeyboardEvent } from "react"
import { Star } from "lucide-react"
import { cn } from "@/lib/utils"

interface StarRatingProps {
    value: number
    onChange: (value: number) => void
    size?: "sm" | "md" | "lg"
    disabled?: boolean
    className?: string
}

const SIZE_CONFIG = {
    sm: { icon: "w-3.5 h-3.5", gap: "gap-0.5" },
    md: { icon: "w-8 h-8", gap: "gap-1" },
    lg: { icon: "w-10 h-10", gap: "gap-1" },
} as const

export const StarRating: FC<StarRatingProps> = ({
    value,
    onChange,
    size = "lg",
    disabled = false,
    className,
}) => {
    const [hoverValue, setHoverValue] = useState(0)
    const config = SIZE_CONFIG[size]

    const isActive = (i: number): boolean =>
        hoverValue > 0 ? i <= hoverValue : i <= value

    const handleKeyDown = (e: KeyboardEvent, i: number) => {
        if (disabled) return
        if (e.key === "ArrowRight" && value < 5) {
            e.preventDefault()
            onChange(value + 1)
        }
        if (e.key === "ArrowLeft" && value > 1) {
            e.preventDefault()
            onChange(value - 1)
        }
        if (e.key === "Enter" || e.key === " ") {
            e.preventDefault()
            onChange(i)
        }
    }

    return (
        <div
            role="radiogroup"
            aria-label="Puntuacion de 1 a 5 estrellas"
            className={cn(
                "flex items-center",
                config.gap,
                disabled && "opacity-50 cursor-not-allowed",
                className
            )}
            onMouseLeave={() => setHoverValue(0)}
        >
            {[1, 2, 3, 4, 5].map((i) => (
                <button
                    key={i}
                    type="button"
                    role="radio"
                    aria-label={`${i} estrella${i > 1 ? "s" : ""}`}
                    aria-checked={value === i}
                    aria-pressed={value === i}
                    disabled={disabled}
                    tabIndex={disabled ? -1 : 0}
                    className={cn(
                        "focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729] rounded-sm",
                        "disabled:cursor-not-allowed",
                        isActive(i) && hoverValue > 0
                            ? "text-[#fbbf24] scale-110 drop-shadow-[0_0_8px_rgba(245,158,11,0.5)] transition-transform duration-100"
                            : isActive(i)
                              ? "text-[#f59e0b] transition-transform duration-100"
                              : "text-[#334155] transition-transform duration-100",
                        "active:scale-125"
                    )}
                    onClick={() => !disabled && onChange(i)}
                    onMouseEnter={() => !disabled && setHoverValue(i)}
                    onKeyDown={(e) => handleKeyDown(e, i)}
                >
                    <Star
                        className={config.icon}
                        fill="currentColor"
                        aria-hidden="true"
                    />
                </button>
            ))}
        </div>
    )
}
