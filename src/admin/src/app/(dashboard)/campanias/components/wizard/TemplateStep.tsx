"use client"

import { useState } from "react"
import { Music, SkipForward, Check, Users } from "lucide-react"
import { cn } from "@/lib/utils"
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
    Button,
    Skeleton,
} from "@/components/ui"
import { useTemplates } from "@/hooks/use-templates"
import type { PlantillaProyectoList } from "@shared/types"
import type { TemplateData } from "@/hooks/use-wizard-state"

interface TemplateStepProps {
    selectedTemplateId?: string
    onNext: (templateData: TemplateData | null) => void
}

function TemplateCardSkeleton() {
    return (
        <div className="rounded-lg border border-border p-5 space-y-3">
            <div className="flex items-center gap-3">
                <Skeleton className="w-10 h-10 rounded-lg" />
                <div className="space-y-2 flex-1">
                    <Skeleton className="h-5 w-2/3" />
                    <Skeleton className="h-3 w-full" />
                </div>
            </div>
            <Skeleton className="h-4 w-1/2" />
            <Skeleton className="h-4 w-1/3" />
        </div>
    )
}

export function TemplateStep({ selectedTemplateId, onNext }: TemplateStepProps) {
    const { data: templates, isLoading } = useTemplates()
    const [selectedId, setSelectedId] = useState<string | null>(
        selectedTemplateId || null
    )

    const handleSelect = (template: PlantillaProyectoList) => {
        setSelectedId(template.id)
    }

    const handleContinue = () => {
        if (!selectedId) {
            onNext(null)
            return
        }
        const selected = templates?.find((t) => t.id === selectedId)
        if (selected) {
            onNext({
                templateId: selected.id,
                necesidadIds: [],
            })
        }
    }

    const handleSkip = () => {
        setSelectedId(null)
        onNext(null)
    }

    const formatPrice = (min: number, max: number) => {
        if (min === 0 && max === 0) return "Sin rango definido"
        return `${min.toLocaleString("es-ES")}\u20AC - ${max.toLocaleString("es-ES")}\u20AC`
    }

    return (
        <Card className="bg-card border-border">
            <CardHeader>
                <CardTitle className="text-2xl font-bold">
                    Elige un Template de Proyecto
                </CardTitle>
                <p className="text-sm text-muted-foreground">
                    Selecciona un template para recibir guia de necesidades
                    profesionales, o salta este paso para crear tu campania
                    manualmente.
                </p>
            </CardHeader>
            <CardContent className="space-y-6">
                {isLoading ? (
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <TemplateCardSkeleton />
                        <TemplateCardSkeleton />
                        <TemplateCardSkeleton />
                    </div>
                ) : templates && templates.length > 0 ? (
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        {templates.map((template) => {
                            const isSelected = selectedId === template.id
                            return (
                                <button
                                    key={template.id}
                                    type="button"
                                    onClick={() => handleSelect(template)}
                                    className={cn(
                                        "rounded-lg border-2 p-5 text-left transition-all duration-200 hover:border-purple-500/50 hover:shadow-md",
                                        isSelected
                                            ? "border-purple-500 bg-purple-500/10 shadow-lg shadow-purple-500/10"
                                            : "border-border bg-card"
                                    )}
                                >
                                    <div className="flex items-start gap-3">
                                        <div
                                            className={cn(
                                                "w-10 h-10 rounded-lg flex items-center justify-center text-lg shrink-0",
                                                isSelected
                                                    ? "bg-purple-500/20"
                                                    : "bg-muted/20"
                                            )}
                                        >
                                            {template.icono || (
                                                <Music className="w-5 h-5" />
                                            )}
                                        </div>
                                        <div className="flex-1 min-w-0">
                                            <div className="flex items-center gap-2">
                                                <h3 className="font-semibold text-foreground truncate">
                                                    {template.nombre}
                                                </h3>
                                                {isSelected && (
                                                    <Check className="w-4 h-4 text-purple-500 shrink-0" />
                                                )}
                                            </div>
                                            {template.descripcion && (
                                                <p className="text-sm text-muted-foreground mt-1 line-clamp-2">
                                                    {template.descripcion}
                                                </p>
                                            )}
                                        </div>
                                    </div>

                                    <div className="mt-4 flex items-center gap-4 text-xs text-muted-foreground">
                                        <span className="flex items-center gap-1">
                                            <Users className="w-3.5 h-3.5" />
                                            {template.cantidadNecesidades}{" "}
                                            {template.cantidadNecesidades === 1
                                                ? "necesidad"
                                                : "necesidades"}
                                        </span>
                                        <span>
                                            {formatPrice(
                                                template.precioMinTotal,
                                                template.precioMaxTotal
                                            )}
                                        </span>
                                    </div>

                                    {template.fases.length > 0 && (
                                        <div className="mt-3 flex flex-wrap gap-1.5">
                                            {template.fases.map((fase) => (
                                                <span
                                                    key={fase}
                                                    className="text-xs px-2 py-0.5 rounded-full bg-muted/30 text-muted-foreground"
                                                >
                                                    {fase}
                                                </span>
                                            ))}
                                        </div>
                                    )}
                                </button>
                            )
                        })}
                    </div>
                ) : (
                    <div className="text-center py-8 text-muted-foreground">
                        <Music className="w-12 h-12 mx-auto mb-3 opacity-50" />
                        <p>No hay templates disponibles por el momento.</p>
                    </div>
                )}

                {/* Footer Navigation */}
                <div className="flex items-center justify-between pt-6 border-t border-border">
                    <Button
                        type="button"
                        variant="outline"
                        disabled
                        className="border-border text-muted-foreground"
                    >
                        &larr; Anterior
                    </Button>

                    <span className="text-sm text-muted-foreground">
                        Paso 1 de 5
                    </span>

                    <div className="flex gap-2">
                        <Button
                            type="button"
                            variant="ghost"
                            onClick={handleSkip}
                            className="text-muted-foreground hover:text-foreground"
                        >
                            <SkipForward className="w-4 h-4 mr-1" />
                            Sin template
                        </Button>
                        {selectedId && (
                            <Button
                                type="button"
                                onClick={handleContinue}
                                className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8"
                            >
                                Siguiente &rarr;
                            </Button>
                        )}
                    </div>
                </div>
            </CardContent>
        </Card>
    )
}
