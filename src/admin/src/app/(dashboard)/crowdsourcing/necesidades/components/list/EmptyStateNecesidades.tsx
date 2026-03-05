"use client"

import { Inbox, SearchX } from "lucide-react"
import { Button } from "@/components/ui/button"

interface EmptyStateNecesidadesProps {
    hasFilters: boolean
    onClearFilters?: () => void
    onNuevaNecesidad?: () => void
    onUsarPlantilla?: () => void
}

export function EmptyStateNecesidades({
    hasFilters,
    onClearFilters,
    onNuevaNecesidad,
    onUsarPlantilla,
}: EmptyStateNecesidadesProps) {
    return (
        <div className="flex flex-col items-center justify-center min-h-[400px] text-center p-8">
            {hasFilters ? (
                <>
                    <SearchX className="h-20 w-20 text-muted-foreground mb-4" />
                    <h3 className="text-xl font-semibold mb-2">
                        No se encontraron necesidades
                    </h3>
                    <p className="text-muted-foreground mb-6">
                        Intenta ajustar los filtros
                    </p>
                    <Button variant="outline" onClick={onClearFilters}>
                        Limpiar filtros
                    </Button>
                </>
            ) : (
                <>
                    <Inbox className="h-20 w-20 text-muted-foreground mb-4" />
                    <h3 className="text-xl font-semibold mb-2">
                        No tienes necesidades publicadas
                    </h3>
                    <p className="text-muted-foreground mb-6 max-w-md">
                        Publica lo que necesitas y recibe propuestas de profesionales cualificados
                    </p>
                    <div className="flex gap-3">
                        <Button onClick={onNuevaNecesidad}>
                            Publicar necesidad
                        </Button>
                        <Button variant="outline" onClick={onUsarPlantilla}>
                            Usar plantilla
                        </Button>
                    </div>
                </>
            )}
        </div>
    )
}
