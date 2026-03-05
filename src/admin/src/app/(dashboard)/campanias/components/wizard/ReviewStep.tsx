/* eslint-disable @next/next/no-img-element */
"use client"

import { Loader2, AlertTriangle, Pencil, Calendar, Target, Image } from "lucide-react"
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
    Button,
    Badge,
} from "@/components/ui"
import { Alert, AlertTitle, AlertDescription } from "@/components/ui/alert"
import { Separator } from "@/components/ui/separator"
import type { CreateCampaniaFormData } from "@shared/schemas"
import {
    TIPO_FINANCIACION_LABELS,
    MONEDA_SYMBOLS,
} from "@shared/constants"
import { formatDate } from "@shared/utils/format"

interface ReviewStepProps {
    formData: Partial<CreateCampaniaFormData>
    onSubmit: () => void
    onBack: () => void
    onEdit: (step: number) => void
    isSubmitting?: boolean
}

export function ReviewStep({
    formData,
    onSubmit,
    onBack,
    onEdit,
    isSubmitting,
}: ReviewStepProps) {
    const currencySymbol = MONEDA_SYMBOLS[formData.monedaId || 1] || "\u20AC"
    const tipoLabel =
        TIPO_FINANCIACION_LABELS[formData.tipoFinanciacionId || 0] || "N/A"

    return (
        <Card className="bg-card border-border">
            <CardHeader>
                <CardTitle className="text-2xl font-bold">
                    Revision Final
                </CardTitle>
                <p className="text-sm text-muted-foreground">
                    Revisa todos los detalles antes de crear tu campania
                </p>
            </CardHeader>
            <CardContent className="space-y-6">
                {/* Preview Card */}
                <div className="rounded-lg border border-border overflow-hidden">
                    {/* Image preview */}
                    {formData.imagenPrincipalUrl ? (
                        <img
                            src={formData.imagenPrincipalUrl}
                            alt="Portada de campania"
                            className="w-full h-64 object-cover"
                        />
                    ) : (
                        <div className="w-full h-64 bg-muted/20 flex items-center justify-center">
                            <Image className="w-12 h-12 text-muted-foreground" />
                        </div>
                    )}

                    <div className="p-6 space-y-4">
                        {/* Title & Edit */}
                        <div className="flex items-start justify-between">
                            <div>
                                <h2 className="text-2xl font-bold text-foreground">
                                    {formData.titulo || "Sin titulo"}
                                </h2>
                                {formData.subtitulo && (
                                    <p className="text-muted-foreground mt-1">
                                        {formData.subtitulo}
                                    </p>
                                )}
                            </div>
                            <Button
                                type="button"
                                variant="ghost"
                                size="sm"
                                onClick={() => onEdit(2)}
                                className="text-primary hover:text-purple-400"
                            >
                                <Pencil className="w-4 h-4 mr-1" />
                                Editar
                            </Button>
                        </div>

                        <Separator />

                        {/* Meta info */}
                        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                            <div className="flex items-center gap-2">
                                <Target className="w-4 h-4 text-muted-foreground" />
                                <div>
                                    <p className="text-xs text-muted-foreground">
                                        Meta
                                    </p>
                                    <p className="font-semibold">
                                        {currencySymbol}{" "}
                                        {formData.importeObjetivo?.toLocaleString(
                                            "es-ES"
                                        ) || "0"}
                                    </p>
                                </div>
                            </div>
                            <div className="flex items-center gap-2">
                                <Calendar className="w-4 h-4 text-muted-foreground" />
                                <div>
                                    <p className="text-xs text-muted-foreground">
                                        Finaliza
                                    </p>
                                    <p className="font-semibold">
                                        {formData.fechaFin
                                            ? formatDate(formData.fechaFin)
                                            : "Sin definir"}
                                    </p>
                                </div>
                            </div>
                            <div>
                                <p className="text-xs text-muted-foreground">
                                    Financiacion
                                </p>
                                <Badge
                                    variant="outline"
                                    className="mt-1"
                                >
                                    {tipoLabel}
                                </Badge>
                            </div>
                        </div>

                        <Separator />

                        {/* Descripcion */}
                        <div className="flex items-start justify-between">
                            <div className="flex-1">
                                <p className="text-xs text-muted-foreground mb-2">
                                    Descripcion
                                </p>
                                {formData.descripcionCorta ? (
                                    <p className="text-sm text-muted-foreground line-clamp-3">
                                        {formData.descripcionCorta}
                                    </p>
                                ) : (
                                    <p className="text-sm text-muted-foreground italic">
                                        Sin descripcion
                                    </p>
                                )}
                            </div>
                            <Button
                                type="button"
                                variant="ghost"
                                size="sm"
                                onClick={() => onEdit(3)}
                                className="text-primary hover:text-purple-400"
                            >
                                <Pencil className="w-4 h-4 mr-1" />
                                Editar
                            </Button>
                        </div>

                        <Separator />

                        {/* Recompensas */}
                        <div className="flex items-start justify-between">
                            <div>
                                <p className="text-xs text-muted-foreground mb-2">
                                    Recompensas
                                </p>
                                <p className="text-sm text-muted-foreground italic">
                                    Sin recompensas (puedes agregar despues)
                                </p>
                            </div>
                            <Button
                                type="button"
                                variant="ghost"
                                size="sm"
                                onClick={() => onEdit(4)}
                                className="text-primary hover:text-purple-400"
                            >
                                <Pencil className="w-4 h-4 mr-1" />
                                Editar
                            </Button>
                        </div>

                        {/* Video */}
                        {formData.videoPrincipalUrl && (
                            <>
                                <Separator />
                                <div>
                                    <p className="text-xs text-muted-foreground mb-1">
                                        Video
                                    </p>
                                    <a
                                        href={formData.videoPrincipalUrl}
                                        target="_blank"
                                        rel="noopener noreferrer"
                                        className="text-sm text-primary hover:underline break-all"
                                    >
                                        {formData.videoPrincipalUrl}
                                    </a>
                                </div>
                            </>
                        )}
                    </div>
                </div>

                {/* Warning alert */}
                <Alert className="bg-yellow-500/10 border-l-4 border-yellow-500">
                    <AlertTriangle className="h-4 w-4 text-yellow-500" />
                    <AlertTitle className="font-semibold text-yellow-500">
                        Nota: Tu campania se creara como BORRADOR
                    </AlertTitle>
                    <AlertDescription className="text-sm text-yellow-400/80">
                        Podras editarla y publicarla cuando estes listo
                    </AlertDescription>
                </Alert>

                {/* Footer Navigation */}
                <div className="flex flex-col sm:flex-row items-center justify-between gap-4 pt-6 border-t border-border">
                    <Button
                        type="button"
                        variant="outline"
                        onClick={onBack}
                        className="w-full sm:w-auto border-border text-muted-foreground hover:text-foreground hover:bg-secondary"
                    >
                        &larr; Anterior
                    </Button>

                    <div className="flex gap-3 w-full sm:w-auto">
                        <Button
                            type="button"
                            variant="outline"
                            onClick={onSubmit}
                            disabled={isSubmitting}
                            className="flex-1 sm:flex-none border-border text-muted-foreground hover:text-foreground"
                        >
                            Guardar borrador
                        </Button>
                        <Button
                            type="button"
                            onClick={onSubmit}
                            disabled={isSubmitting}
                            className="flex-1 sm:flex-none bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8"
                            aria-busy={isSubmitting}
                        >
                            {isSubmitting ? (
                                <>
                                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                                    Creando campania...
                                </>
                            ) : (
                                "Crear campania"
                            )}
                        </Button>
                    </div>
                </div>
            </CardContent>
        </Card>
    )
}
