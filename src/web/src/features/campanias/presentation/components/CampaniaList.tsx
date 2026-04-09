import { FolderOpen, Music } from "lucide-react"
import { Alert, AlertTitle, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { CampaniaCard } from "./CampaniaCard"
import { CampaniaListSkeleton } from "./CampaniaListSkeleton"
import type { CampaniaListItem } from "../../domain"

interface CampaniaListProps {
    campanias: CampaniaListItem[]
    isLoading?: boolean
    error?: Error | null
    variant?: "grid" | "list"
    onRetry?: () => void
    onClearFilters?: () => void
    hasFilters?: boolean
    className?: string
}

export function CampaniaList({
    campanias,
    isLoading,
    error,
    variant = "grid",
    onRetry,
    onClearFilters,
    hasFilters = false,
    className,
}: CampaniaListProps) {
    if (isLoading) {
        return <CampaniaListSkeleton count={6} />
    }

    if (error) {
        return (
            <Alert variant="destructive" className="max-w-2xl mx-auto bg-[#0f1729] border-destructive/50">
                <AlertTitle>Error al cargar campanas</AlertTitle>
                <AlertDescription>
                    Hubo un problema al cargar las campanas. Por favor, intenta nuevamente.
                    {onRetry && (
                        <Button
                            variant="outline"
                            size="sm"
                            onClick={onRetry}
                            className="mt-4 block"
                        >
                            Reintentar
                        </Button>
                    )}
                </AlertDescription>
            </Alert>
        )
    }

    if (campanias.length === 0) {
        return (
            <div className="flex flex-col items-center justify-center min-h-[400px] gap-4 py-12">
                {hasFilters ? (
                    <>
                        <FolderOpen className="h-16 w-16 text-[#64748b]" />
                        <h2 className="text-2xl font-bold text-white">
                            No se encontraron campanas
                        </h2>
                        <p className="text-[#94a3b8] text-center max-w-md">
                            Intenta ajustar tus filtros o busca otros terminos
                        </p>
                        {onClearFilters && (
                            <Button
                                variant="outline"
                                onClick={onClearFilters}
                                className="border-[#334155] text-[#94a3b8] hover:text-white"
                            >
                                Ver todas las campanas
                            </Button>
                        )}
                    </>
                ) : (
                    <>
                        <Music className="h-16 w-16 text-[#64748b]" />
                        <h2 className="text-2xl font-bold text-white">
                            No hay campanas disponibles
                        </h2>
                        <p className="text-[#94a3b8] text-center max-w-md">
                            Aun no hay proyectos musicales publicados
                        </p>
                    </>
                )}
            </div>
        )
    }

    return (
        <div
            className={cn(
                variant === "grid"
                    ? "grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6"
                    : "space-y-4",
                className
            )}
        >
            {campanias.map((campania) => (
                <CampaniaCard
                    key={campania.id}
                    campania={campania}
                    variant={variant === "list" ? "compact" : "default"}
                />
            ))}
        </div>
    )
}
