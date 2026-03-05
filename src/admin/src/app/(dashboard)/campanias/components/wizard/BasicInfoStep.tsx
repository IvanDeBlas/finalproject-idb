"use client"

import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { z } from "zod"
import { format, addDays } from "date-fns"
import { es } from "date-fns/locale"
import { CalendarIcon, Loader2 } from "lucide-react"
import { cn } from "@/lib/utils"
import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
    Button,
    Input,
    Label,
} from "@/components/ui"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import {
    Popover,
    PopoverContent,
    PopoverTrigger,
} from "@/components/ui/popover"
import { Calendar } from "@/components/ui/calendar"
import { ImageUploadField } from "../shared/ImageUploadField"
import {
    TIPO_FINANCIACION,
    TIPO_FINANCIACION_LABELS,
    TIPO_FINANCIACION_DESCRIPTIONS,
    VALIDATION,
} from "@shared/constants"

const basicInfoStepSchema = z.object({
    titulo: z
        .string()
        .min(1, "El titulo es obligatorio")
        .max(200, "El titulo no puede superar los 200 caracteres"),
    importeObjetivo: z
        .number({ invalid_type_error: "Debe ser un numero" })
        .positive("El importe objetivo debe ser mayor a 0")
        .min(100, "El importe minimo es 100"),
    monedaId: z.number().default(1),
    tipoFinanciacionId: z
        .number({ invalid_type_error: "Selecciona un tipo de financiacion" })
        .positive("Selecciona un tipo de financiacion"),
    fechaFin: z.string().optional(),
    imagenPrincipalUrl: z
        .string()
        .url("Debe ser una URL valida")
        .optional()
        .or(z.literal("")),
    videoPrincipalUrl: z
        .string()
        .url("Debe ser una URL valida")
        .optional()
        .or(z.literal("")),
})

type BasicInfoFormData = z.infer<typeof basicInfoStepSchema>

interface BasicInfoStepProps {
    defaultValues?: Partial<BasicInfoFormData>
    onNext: (data: BasicInfoFormData) => void
    onBack?: () => void
    isSubmitting?: boolean
}

export function BasicInfoStep({
    defaultValues,
    onNext,
    onBack,
    isSubmitting,
}: BasicInfoStepProps) {
    const {
        register,
        handleSubmit,
        setValue,
        watch,
        formState: { errors },
    } = useForm<BasicInfoFormData>({
        resolver: zodResolver(basicInfoStepSchema),
        defaultValues: {
            titulo: defaultValues?.titulo || "",
            importeObjetivo: defaultValues?.importeObjetivo || 0,
            monedaId: defaultValues?.monedaId || 1,
            tipoFinanciacionId: defaultValues?.tipoFinanciacionId || 0,
            fechaFin: defaultValues?.fechaFin || "",
            imagenPrincipalUrl: defaultValues?.imagenPrincipalUrl || "",
            videoPrincipalUrl: defaultValues?.videoPrincipalUrl || "",
        },
    })

    const tituloValue = watch("titulo") || ""
    const fechaFinValue = watch("fechaFin")
    const imagenUrl = watch("imagenPrincipalUrl")

    const minDate = addDays(new Date(), 7)
    const maxDate = addDays(new Date(), 60)

    const onSubmit = (data: BasicInfoFormData) => {
        onNext(data)
    }

    return (
        <Card className="bg-card border-border">
            <CardHeader>
                <CardTitle className="text-2xl font-bold">
                    Informacion Basica
                </CardTitle>
                <p className="text-sm text-muted-foreground">
                    Define los datos principales de tu campania
                </p>
            </CardHeader>
            <CardContent>
                <form
                    onSubmit={handleSubmit(onSubmit)}
                    className="space-y-6"
                >
                    {/* Titulo */}
                    <div className="space-y-2">
                        <Label htmlFor="titulo">
                            Titulo de la campania{" "}
                            <span className="text-destructive">*</span>
                        </Label>
                        <Input
                            id="titulo"
                            placeholder="Ej: Nuevo album 'Ecos de Medianoche'"
                            {...register("titulo")}
                            className="bg-input border-border"
                        />
                        <div className="flex items-center justify-between">
                            {errors.titulo ? (
                                <p className="text-sm text-destructive">
                                    {errors.titulo.message}
                                </p>
                            ) : (
                                <span className="text-xs text-muted-foreground">
                                    Un titulo claro y atractivo para tu proyecto
                                </span>
                            )}
                            <span
                                className={cn(
                                    "text-xs transition-colors",
                                    tituloValue.length /
                                        VALIDATION.TITULO_MAX >=
                                        0.95
                                        ? "text-destructive"
                                        : tituloValue.length /
                                                VALIDATION.TITULO_MAX >=
                                            0.8
                                          ? "text-yellow-500"
                                          : "text-muted-foreground"
                                )}
                            >
                                {tituloValue.length}/{VALIDATION.TITULO_MAX}
                            </span>
                        </div>
                    </div>

                    {/* Tipo de Financiacion */}
                    <div className="space-y-2">
                        <Label>
                            Tipo de financiacion{" "}
                            <span className="text-destructive">*</span>
                        </Label>
                        <Select
                            onValueChange={(value) =>
                                setValue("tipoFinanciacionId", Number(value), {
                                    shouldValidate: true,
                                })
                            }
                            defaultValue={
                                defaultValues?.tipoFinanciacionId
                                    ? String(defaultValues.tipoFinanciacionId)
                                    : undefined
                            }
                        >
                            <SelectTrigger className="bg-input border-border">
                                <SelectValue placeholder="Selecciona tipo" />
                            </SelectTrigger>
                            <SelectContent className="bg-popover border-border">
                                <SelectItem
                                    value={String(
                                        TIPO_FINANCIACION.TODO_O_NADA
                                    )}
                                >
                                    {
                                        TIPO_FINANCIACION_LABELS[
                                            TIPO_FINANCIACION.TODO_O_NADA
                                        ]
                                    }
                                </SelectItem>
                                <SelectItem
                                    value={String(
                                        TIPO_FINANCIACION.FLEXIBLE
                                    )}
                                >
                                    {
                                        TIPO_FINANCIACION_LABELS[
                                            TIPO_FINANCIACION.FLEXIBLE
                                        ]
                                    }
                                </SelectItem>
                            </SelectContent>
                        </Select>
                        {errors.tipoFinanciacionId && (
                            <p className="text-sm text-destructive">
                                {errors.tipoFinanciacionId.message}
                            </p>
                        )}
                        {watch("tipoFinanciacionId") > 0 && (
                            <p className="text-xs text-muted-foreground">
                                {
                                    TIPO_FINANCIACION_DESCRIPTIONS[
                                        watch("tipoFinanciacionId")
                                    ]
                                }
                            </p>
                        )}
                    </div>

                    {/* Grid: Meta + Fecha */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        {/* Meta de Financiacion */}
                        <div className="space-y-2">
                            <Label htmlFor="importeObjetivo">
                                Meta de financiacion (EUR){" "}
                                <span className="text-destructive">*</span>
                            </Label>
                            <div className="relative">
                                <span className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground">
                                    &euro;
                                </span>
                                <Input
                                    id="importeObjetivo"
                                    type="number"
                                    min={100}
                                    placeholder="5000"
                                    {...register("importeObjetivo", {
                                        valueAsNumber: true,
                                    })}
                                    className="bg-input border-border pl-8"
                                />
                            </div>
                            {errors.importeObjetivo && (
                                <p className="text-sm text-destructive">
                                    {errors.importeObjetivo.message}
                                </p>
                            )}
                        </div>

                        {/* Fecha de Finalizacion */}
                        <div className="space-y-2">
                            <Label>Fecha de finalizacion</Label>
                            <Popover>
                                <PopoverTrigger asChild>
                                    <Button
                                        type="button"
                                        variant="outline"
                                        className={cn(
                                            "w-full justify-start text-left font-normal bg-input border-border",
                                            !fechaFinValue &&
                                                "text-muted-foreground"
                                        )}
                                    >
                                        <CalendarIcon className="mr-2 h-4 w-4" />
                                        {fechaFinValue ? (
                                            format(
                                                new Date(fechaFinValue),
                                                "PPP",
                                                { locale: es }
                                            )
                                        ) : (
                                            <span>Selecciona fecha</span>
                                        )}
                                    </Button>
                                </PopoverTrigger>
                                <PopoverContent
                                    className="w-auto p-0 bg-popover border-border"
                                    align="start"
                                >
                                    <Calendar
                                        mode="single"
                                        selected={
                                            fechaFinValue
                                                ? new Date(fechaFinValue)
                                                : undefined
                                        }
                                        onSelect={(date) =>
                                            setValue(
                                                "fechaFin",
                                                date?.toISOString() || "",
                                                { shouldValidate: true }
                                            )
                                        }
                                        disabled={(date) =>
                                            date < minDate || date > maxDate
                                        }
                                        initialFocus
                                    />
                                </PopoverContent>
                            </Popover>
                            <p className="text-xs text-muted-foreground">
                                Entre 7 y 60 dias desde hoy
                            </p>
                            {errors.fechaFin && (
                                <p className="text-sm text-destructive">
                                    {errors.fechaFin.message}
                                </p>
                            )}
                        </div>
                    </div>

                    {/* Video URL */}
                    <div className="space-y-2">
                        <Label htmlFor="videoPrincipalUrl">
                            URL del video (YouTube/Vimeo)
                        </Label>
                        <Input
                            id="videoPrincipalUrl"
                            type="url"
                            placeholder="https://youtube.com/watch?v=..."
                            {...register("videoPrincipalUrl")}
                            className="bg-input border-border"
                        />
                        <p className="text-xs text-muted-foreground">
                            Un video ayuda a tus fans a conocer mejor tu
                            proyecto
                        </p>
                        {errors.videoPrincipalUrl && (
                            <p className="text-sm text-destructive">
                                {errors.videoPrincipalUrl.message}
                            </p>
                        )}
                    </div>

                    {/* Imagen de Portada */}
                    <div className="space-y-2">
                        <Label>Imagen de portada</Label>
                        <ImageUploadField
                            value={imagenUrl || ""}
                            onChange={(url) =>
                                setValue("imagenPrincipalUrl", url, {
                                    shouldValidate: true,
                                })
                            }
                            label="Imagen de portada"
                        />
                        {errors.imagenPrincipalUrl && (
                            <p className="text-sm text-destructive">
                                {errors.imagenPrincipalUrl.message}
                            </p>
                        )}
                    </div>

                    {/* Footer Navigation */}
                    <div className="flex items-center justify-between pt-6 border-t border-border">
                        <Button
                            type="button"
                            variant="outline"
                            onClick={onBack}
                            disabled={!onBack}
                            className="border-border text-muted-foreground hover:text-foreground hover:bg-secondary"
                        >
                            &larr; Anterior
                        </Button>

                        <span className="text-sm text-muted-foreground">
                            Paso 2 de 5
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
