"use client"

import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { Loader2 } from "lucide-react"
import { cn } from "@/lib/utils"
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
    Button,
    Input,
    Label,
    Textarea,
} from "@/components/ui"
import { VALIDATION } from "@shared/constants"

const storyStepSchema = z.object({
    subtitulo: z
        .string()
        .max(300, "El subtitulo no puede superar los 300 caracteres")
        .optional()
        .or(z.literal("")),
    descripcionCorta: z
        .string()
        .max(500, "La descripcion corta no puede superar los 500 caracteres")
        .optional()
        .or(z.literal("")),
})

type StoryFormData = z.infer<typeof storyStepSchema>

interface StoryStepProps {
    defaultValues?: Partial<StoryFormData>
    onNext: (data: StoryFormData) => void
    onBack: () => void
    isSubmitting?: boolean
}

export function StoryStep({
    defaultValues,
    onNext,
    onBack,
    isSubmitting,
}: StoryStepProps) {
    const {
        register,
        handleSubmit,
        watch,
        formState: { errors },
    } = useForm<StoryFormData>({
        resolver: zodResolver(storyStepSchema),
        defaultValues: {
            subtitulo: defaultValues?.subtitulo || "",
            descripcionCorta: defaultValues?.descripcionCorta || "",
        },
    })

    const subtituloValue = watch("subtitulo") || ""
    const descripcionValue = watch("descripcionCorta") || ""

    const onSubmit = (data: StoryFormData) => {
        onNext(data)
    }

    return (
        <Card className="bg-card border-border">
            <CardHeader>
                <CardTitle className="text-2xl font-bold">Historia</CardTitle>
                <p className="text-sm text-muted-foreground">
                    Cuenta la historia detras de tu proyecto musical
                </p>
            </CardHeader>
            <CardContent>
                <form
                    onSubmit={handleSubmit(onSubmit)}
                    className="space-y-6"
                >
                    {/* Subtitulo */}
                    <div className="space-y-2">
                        <Label htmlFor="subtitulo">Subtitulo</Label>
                        <Input
                            id="subtitulo"
                            placeholder="Un resumen breve de tu campania"
                            {...register("subtitulo")}
                            className="bg-input border-border"
                        />
                        <div className="flex items-center justify-between">
                            {errors.subtitulo ? (
                                <p className="text-sm text-destructive">
                                    {errors.subtitulo.message}
                                </p>
                            ) : (
                                <span className="text-xs text-muted-foreground">
                                    Opcional - aparece debajo del titulo
                                </span>
                            )}
                            <span
                                className={cn(
                                    "text-xs transition-colors",
                                    subtituloValue.length /
                                        VALIDATION.SUBTITULO_MAX >=
                                        0.95
                                        ? "text-destructive"
                                        : subtituloValue.length /
                                                VALIDATION.SUBTITULO_MAX >=
                                            0.8
                                          ? "text-yellow-500"
                                          : "text-muted-foreground"
                                )}
                            >
                                {subtituloValue.length}/
                                {VALIDATION.SUBTITULO_MAX}
                            </span>
                        </div>
                    </div>

                    {/* Descripcion Corta */}
                    <div className="space-y-2">
                        <Label htmlFor="descripcionCorta">
                            Descripcion corta
                        </Label>
                        <Textarea
                            id="descripcionCorta"
                            placeholder="Describe tu proyecto musical, que quieres lograr y por que tus fans deberian apoyarte..."
                            rows={8}
                            {...register("descripcionCorta")}
                            className="bg-input border-border resize-none"
                        />
                        <div className="flex items-center justify-between">
                            {errors.descripcionCorta ? (
                                <p className="text-sm text-destructive">
                                    {errors.descripcionCorta.message}
                                </p>
                            ) : (
                                <span className="text-xs text-muted-foreground">
                                    Cuenta la historia de tu proyecto
                                </span>
                            )}
                            <span
                                className={cn(
                                    "text-xs transition-colors",
                                    descripcionValue.length /
                                        VALIDATION.DESCRIPCION_CORTA_MAX >=
                                        0.95
                                        ? "text-destructive"
                                        : descripcionValue.length /
                                                VALIDATION.DESCRIPCION_CORTA_MAX >=
                                            0.8
                                          ? "text-yellow-500"
                                          : "text-muted-foreground"
                                )}
                            >
                                {descripcionValue.length}/
                                {VALIDATION.DESCRIPCION_CORTA_MAX}
                            </span>
                        </div>
                    </div>

                    {/* Tip box */}
                    <div className="bg-primary/5 border-l-4 border-primary p-4 rounded-r-lg flex items-start gap-3">
                        <span className="text-primary text-lg">&#128161;</span>
                        <div>
                            <p className="text-sm font-medium text-foreground">
                                Consejo
                            </p>
                            <p className="text-xs text-muted-foreground mt-1">
                                Una buena descripcion incluye tu historia
                                personal, que haras con los fondos y por que tu
                                musica importa. Se autentico.
                            </p>
                        </div>
                    </div>

                    {/* Footer Navigation */}
                    <div className="flex items-center justify-between pt-6 border-t border-border">
                        <Button
                            type="button"
                            variant="outline"
                            onClick={onBack}
                            className="border-border text-muted-foreground hover:text-foreground hover:bg-secondary"
                        >
                            &larr; Anterior
                        </Button>

                        <span className="text-sm text-muted-foreground">
                            Paso 3 de 5
                        </span>

                        <Button
                            type="submit"
                            disabled={isSubmitting}
                            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8"
                            aria-busy={isSubmitting}
                        >
                            {isSubmitting ? (
                                <>
                                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                                    Validando...
                                </>
                            ) : (
                                "Siguiente \u2192"
                            )}
                        </Button>
                    </div>
                </form>
            </CardContent>
        </Card>
    )
}
