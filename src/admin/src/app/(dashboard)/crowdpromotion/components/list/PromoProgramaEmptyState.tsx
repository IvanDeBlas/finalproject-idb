import { Button } from "@/components/ui/button"
import { Megaphone, SearchX } from "lucide-react"
import Link from "next/link"
import { APP_ROUTES } from "@shared/constants"

interface PromoProgramaEmptyStateProps {
    variant: "no-programas" | "no-resultados"
    onLimpiarFiltros?: () => void
}

export function PromoProgramaEmptyState({ variant, onLimpiarFiltros }: PromoProgramaEmptyStateProps) {
    if (variant === "no-programas") {
        return (
            <div className="flex flex-col items-center justify-center py-16 text-center">
                <div className="rounded-full bg-[#1e1e38] p-4 mb-4">
                    <Megaphone className="h-8 w-8 text-purple-400" />
                </div>
                <h3 className="text-lg font-semibold text-white mb-2">
                    No tienes programas de promocion
                </h3>
                <p className="text-sm text-zinc-400 mb-6 max-w-md">
                    Crea tu primer programa de promocion para atraer promotores y aumentar la
                    visibilidad de tus campanas.
                </p>
                <Button asChild className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700">
                    <Link href={APP_ROUTES.dashboard.crowdpromotion.programas.nuevo}>
                        Crear primer programa
                    </Link>
                </Button>
            </div>
        )
    }

    return (
        <div className="flex flex-col items-center justify-center py-16 text-center">
            <div className="rounded-full bg-[#1e1e38] p-4 mb-4">
                <SearchX className="h-8 w-8 text-zinc-400" />
            </div>
            <h3 className="text-lg font-semibold text-white mb-2">
                Sin resultados
            </h3>
            <p className="text-sm text-zinc-400 mb-6">
                No se encontraron programas con los filtros aplicados.
            </p>
            {onLimpiarFiltros && (
                <Button variant="outline" onClick={onLimpiarFiltros}>
                    Limpiar filtros
                </Button>
            )}
        </div>
    )
}
