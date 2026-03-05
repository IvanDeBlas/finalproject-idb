import { useNavigate } from "react-router-dom"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Users } from "lucide-react"
import { cn } from "@/lib/utils"
import { CAMPANIA_ESTADOS_LABELS } from "@/lib/constants"
import { calcularDiasRestantes, calcularPorcentaje, formatCurrency } from "../../application/utils"
import type { CampaniaListItem } from "../../domain"

interface CampaniaCardProps {
    campania: CampaniaListItem
    variant?: "default" | "compact"
    className?: string
}

function getEstadoBadgeClasses(estadoCampaniaId: number): string {
    switch (estadoCampaniaId) {
        case 2: return "bg-green-500/20 text-green-400 border-green-500/50"
        case 3: return "bg-blue-500/20 text-blue-400 border-blue-500/50"
        case 4: return "bg-red-500/20 text-red-400 border-red-500/50"
        default: return "bg-[#64748b]/20 text-[#64748b] border-[#64748b]/50"
    }
}

export function CampaniaCard({ campania, variant = "default", className }: CampaniaCardProps) {
    const navigate = useNavigate()

    const porcentaje = calcularPorcentaje(campania.importePledgedActual, campania.importeObjetivo)
    const diasRestantes = campania.fechaFin ? calcularDiasRestantes(campania.fechaFin) : null
    const isFunded = porcentaje >= 100
    const monedaId = campania.monedaId ?? 1

    const estadoLabel = CAMPANIA_ESTADOS_LABELS[campania.estadoCampaniaId] ?? "Activa"
    const imageUrl = campania.imagenPrincipalUrl || campania.imagenUrl

    const handleClick = () => {
        navigate(`/campanias/${campania.id}`)
    }

    const handleKeyDown = (e: React.KeyboardEvent) => {
        if (e.key === "Enter") {
            handleClick()
        }
    }

    if (variant === "compact") {
        return (
            <Card
                className={cn(
                    "bg-[#0f1729] border-[#334155] hover:border-[#a855f7] transition cursor-pointer overflow-hidden",
                    className
                )}
                onClick={handleClick}
                onKeyDown={handleKeyDown}
                tabIndex={0}
                role="article"
                aria-label={`Campana: ${campania.titulo}`}
            >
                <div className="flex gap-4 p-4">
                    {imageUrl && (
                        <img
                            src={imageUrl}
                            alt={`Imagen de portada de ${campania.titulo}`}
                            className="w-20 h-20 object-cover rounded-md shrink-0"
                            loading="lazy"
                        />
                    )}
                    <div className="flex-1 min-w-0">
                        <h3 className="text-sm font-bold text-white line-clamp-1">{campania.titulo}</h3>
                        <p className="text-xs text-[#94a3b8] mt-1">{porcentaje}% financiado</p>
                        <div className="mt-2 h-1.5 w-full rounded-full bg-[#334155] overflow-hidden">
                            <div
                                className={cn(
                                    "h-full rounded-full transition-all",
                                    isFunded
                                        ? "bg-gradient-to-r from-green-400 to-green-600"
                                        : "bg-gradient-to-r from-pink-500 to-purple-600"
                                )}
                                style={{ width: `${porcentaje}%` }}
                            />
                        </div>
                    </div>
                </div>
            </Card>
        )
    }

    return (
        <Card
            className={cn(
                "bg-[#0f1729] border-[#334155] hover:border-[#a855f7] transition cursor-pointer overflow-hidden group",
                className
            )}
            onClick={handleClick}
            onKeyDown={handleKeyDown}
            tabIndex={0}
            role="article"
            aria-label={`Campana: ${campania.titulo}`}
        >
            {/* Image */}
            <div className="relative overflow-hidden">
                {imageUrl ? (
                    <img
                        src={imageUrl}
                        alt={`Imagen de portada de ${campania.titulo}`}
                        className="w-full h-48 object-cover transition-transform duration-300 group-hover:scale-105"
                        loading="lazy"
                    />
                ) : (
                    <div className="w-full h-48 bg-[#1a1a2e] flex items-center justify-center">
                        <span className="text-[#64748b] text-4xl">
                            {campania.titulo.charAt(0).toUpperCase()}
                        </span>
                    </div>
                )}
            </div>

            <CardHeader className="p-4 pb-2">
                <div className="flex items-center justify-between mb-2">
                    <Badge
                        variant="outline"
                        className={cn("text-xs", getEstadoBadgeClasses(campania.estadoCampaniaId))}
                        role="status"
                        aria-label={`Estado: ${estadoLabel}`}
                    >
                        {estadoLabel}
                    </Badge>
                </div>
                <CardTitle className="text-lg font-bold text-white line-clamp-2">
                    {campania.titulo}
                </CardTitle>
            </CardHeader>

            <CardContent className="p-4 pt-0 space-y-3">
                {campania.descripcionCorta && (
                    <p className="text-sm text-[#94a3b8] line-clamp-2">
                        {campania.descripcionCorta}
                    </p>
                )}

                {/* Progress bar */}
                <div
                    className="h-2 w-full rounded-full bg-[#334155] overflow-hidden"
                    role="progressbar"
                    aria-valuenow={porcentaje}
                    aria-valuemin={0}
                    aria-valuemax={100}
                    aria-label={`Progreso de financiacion: ${porcentaje}%`}
                >
                    <div
                        className={cn(
                            "h-full rounded-full transition-all duration-500",
                            isFunded
                                ? "bg-gradient-to-r from-green-400 to-green-600"
                                : "bg-gradient-to-r from-pink-500 to-purple-600"
                        )}
                        style={{ width: `${porcentaje}%` }}
                    />
                </div>

                {/* Amount stats */}
                <div className="flex items-center justify-between">
                    <div className="space-y-1">
                        <p className="text-white font-semibold">
                            {formatCurrency(campania.importePledgedActual, monedaId)}
                        </p>
                        <p className="text-xs text-[#64748b]">
                            de {formatCurrency(campania.importeObjetivo, monedaId)}
                        </p>
                    </div>

                    <div className="flex items-center gap-1 text-[#64748b] text-sm">
                        <Users className="w-4 h-4" />
                        <span>{campania.backersCount ?? 0}</span>
                    </div>
                </div>

                {/* Days remaining */}
                {diasRestantes !== null && (
                    <p className="text-xs text-[#64748b]">
                        {diasRestantes === 0 ? "Campana finalizada" : `${diasRestantes} dias restantes`}
                    </p>
                )}
            </CardContent>
        </Card>
    )
}
