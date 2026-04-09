import { FC, useState, useEffect } from "react"
import { Star } from "lucide-react"
import { cn } from "@/lib/utils"

interface RatingHistogramProps {
    distribucion: Record<number, number>
    total: number
    className?: string
}

export const RatingHistogram: FC<RatingHistogramProps> = ({
    distribucion,
    total,
    className,
}) => {
    const [animado, setAnimado] = useState(false)

    useEffect(() => {
        const timer = setTimeout(() => setAnimado(true), 50)
        return () => clearTimeout(timer)
    }, [])

    const getPorcentaje = (count: number): number =>
        total > 0 ? Math.round((count / total) * 100) : 0

    return (
        <div className={cn("space-y-2", className)}>
            {([5, 4, 3, 2, 1] as const).map((nivel) => {
                const count = distribucion[nivel] ?? 0
                const porcentaje = getPorcentaje(count)
                return (
                    <div key={nivel} className="flex items-center gap-2">
                        <span className="text-xs text-[#94a3b8] w-3 text-right flex-shrink-0">
                            {nivel}
                        </span>
                        <Star
                            className="w-3 h-3 text-[#f59e0b] flex-shrink-0"
                            fill="currentColor"
                            aria-hidden="true"
                        />
                        <div className="flex-1 h-2 bg-[#334155] rounded-full overflow-hidden">
                            <div
                                className="h-full bg-gradient-to-r from-[#f59e0b] to-[#fbbf24] rounded-full transition-all duration-500"
                                style={{
                                    width: animado ? `${porcentaje}%` : "0%",
                                }}
                                role="presentation"
                            />
                        </div>
                        <span className="text-xs text-[#64748b] w-4 text-right flex-shrink-0">
                            {count}
                        </span>
                    </div>
                )
            })}
        </div>
    )
}
