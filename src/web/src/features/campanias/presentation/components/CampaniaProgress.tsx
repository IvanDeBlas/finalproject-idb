import { cn } from "@/lib/utils"
import { calcularPorcentaje, formatCurrency } from "../../application/utils"

interface CampaniaProgressProps {
    importeObjetivo: number
    importePledgedActual: number
    monedaId: number
    size?: "sm" | "md" | "lg"
    showLabels?: boolean
    className?: string
}

export function CampaniaProgress({
    importeObjetivo,
    importePledgedActual,
    monedaId,
    size = "md",
    showLabels = true,
    className,
}: CampaniaProgressProps) {
    const porcentaje = calcularPorcentaje(importePledgedActual, importeObjetivo)
    const isFunded = porcentaje >= 100

    const heightClass = {
        sm: "h-2",
        md: "h-3",
        lg: "h-4",
    }[size]

    return (
        <div className={cn("space-y-2", className)}>
            <div
                className={cn(
                    "relative w-full overflow-hidden rounded-full bg-[#334155]",
                    heightClass
                )}
                role="progressbar"
                aria-valuenow={porcentaje}
                aria-valuemin={0}
                aria-valuemax={100}
                aria-label={`Progreso de financiacion: ${porcentaje}% alcanzado`}
            >
                <div
                    className={cn(
                        "h-full rounded-full transition-all duration-600 ease-out",
                        isFunded
                            ? "bg-gradient-to-r from-green-400 to-green-600"
                            : "bg-gradient-to-r from-pink-500 to-purple-600"
                    )}
                    style={{ width: `${porcentaje}%` }}
                />
            </div>

            {showLabels && (
                <div className="flex items-center justify-between text-sm">
                    <span className="font-semibold text-white">
                        {formatCurrency(importePledgedActual, monedaId)}
                    </span>
                    <span className="text-[#94a3b8]">
                        {porcentaje}% de {formatCurrency(importeObjetivo, monedaId)}
                    </span>
                </div>
            )}
        </div>
    )
}
