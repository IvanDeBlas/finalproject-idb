"use client"

import { useForm, Controller, type FieldValues } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Card } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { Button } from "@/components/ui/button"
import { Alert, AlertTitle, AlertDescription } from "@/components/ui/alert"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"
import { Calendar } from "@/components/ui/calendar"
import { AlertCircle, Loader2, CalendarIcon } from "lucide-react"
import { cn } from "@/lib/utils"
import { format } from "date-fns"
import { es } from "date-fns/locale"
import { useTiposNecesidad } from "@/hooks/use-tipos-necesidad"
import { useModalidadesTrabajo } from "@/hooks/use-modalidades-trabajo"
import { useMonedas } from "@/hooks/use-monedas"
import { UbicacionFields } from "./UbicacionFields"
import { PresupuestoFields } from "./PresupuestoFields"
import { createNecesidadSchema, updateNecesidadSchema } from "@shared/schemas/crowdsourcing.schema"
import type { CreateNecesidadFormData, UpdateNecesidadFormData } from "@shared/schemas/crowdsourcing.schema"
import { MODALIDAD_TRABAJO } from "@shared/constants"

interface NecesidadFormProps {
    mode: "create" | "edit"
    defaultValues?: Partial<CreateNecesidadFormData | UpdateNecesidadFormData>
    necesidadId?: string
    numeroPropuestas?: number
    onSubmit: (data: CreateNecesidadFormData | UpdateNecesidadFormData) => Promise<void>
    onCancel: () => void
}

export function NecesidadForm({
    mode,
    defaultValues,
    numeroPropuestas = 0,
    onSubmit,
    onCancel,
}: NecesidadFormProps) {
    const schema = mode === "create" ? createNecesidadSchema : updateNecesidadSchema

    const {
        register,
        control,
        handleSubmit,
        watch,
        formState: { errors, isSubmitting },
    } = useForm<FieldValues>({
        resolver: zodResolver(schema),
        defaultValues: (defaultValues as FieldValues) || {},
    })

    const { data: tiposNecesidad } = useTiposNecesidad()
    const { data: modalidades } = useModalidadesTrabajo()
    const { data: monedas } = useMonedas()

    const watchModalidad = watch("modalidadTrabajoId") as number | undefined
    const watchDescripcion = watch("descripcion") as string | undefined
    const showUbicacion =
        watchModalidad === MODALIDAD_TRABAJO.PRESENCIAL ||
        watchModalidad === MODALIDAD_TRABAJO.HIBRIDO

    const handleFormSubmit = async (data: FieldValues) => {
        await onSubmit(data as CreateNecesidadFormData | UpdateNecesidadFormData)
    }

    return (
        <form onSubmit={handleSubmit(handleFormSubmit)}>
            {mode === "edit" && numeroPropuestas > 0 && (
                <Alert variant="destructive" className="mb-6">
                    <AlertCircle className="h-5 w-5" />
                    <AlertTitle>Necesidad con propuestas</AlertTitle>
                    <AlertDescription>
                        Esta necesidad ya tiene {numeroPropuestas} propuestas. Los cambios seran
                        visibles para los profesionales.
                    </AlertDescription>
                </Alert>
            )}

            <Card className="p-6 mb-6">
                <h2 className="text-lg font-semibold mb-4">Informacion basica</h2>

                <div className="space-y-4">
                    <div>
                        <Label htmlFor="titulo">Titulo *</Label>
                        <Input id="titulo" {...register("titulo")} placeholder="Describe brevemente tu necesidad" />
                        {errors.titulo && (
                            <p className="text-sm text-red-400 mt-1">
                                {errors.titulo.message as string}
                            </p>
                        )}
                    </div>

                    <div>
                        <Label htmlFor="descripcion">Descripcion</Label>
                        <Textarea
                            id="descripcion"
                            {...register("descripcion")}
                            rows={4}
                            placeholder="Detalla lo que necesitas..."
                        />
                        <span className="text-xs text-muted-foreground">
                            {watchDescripcion?.length || 0} / 4000 caracteres
                        </span>
                        {errors.descripcion && (
                            <p className="text-sm text-red-400 mt-1">
                                {errors.descripcion.message as string}
                            </p>
                        )}
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        {mode === "create" && (
                            <div>
                                <Label htmlFor="tipoNecesidadId">Tipo de necesidad *</Label>
                                <Controller
                                    name="tipoNecesidadId"
                                    control={control}
                                    render={({ field }) => (
                                        <Select
                                            value={field.value?.toString() || ""}
                                            onValueChange={(val) => field.onChange(Number(val))}
                                        >
                                            <SelectTrigger id="tipoNecesidadId">
                                                <SelectValue placeholder="Selecciona tipo" />
                                            </SelectTrigger>
                                            <SelectContent>
                                                {tiposNecesidad?.map((tipo) => (
                                                    <SelectItem
                                                        key={tipo.id}
                                                        value={tipo.id.toString()}
                                                    >
                                                        {tipo.nombre}
                                                    </SelectItem>
                                                ))}
                                            </SelectContent>
                                        </Select>
                                    )}
                                />
                                {errors.tipoNecesidadId && (
                                    <p className="text-sm text-red-400 mt-1">
                                        {(errors.tipoNecesidadId as { message?: string }).message}
                                    </p>
                                )}
                            </div>
                        )}

                        <div>
                            <Label htmlFor="modalidadTrabajoId">Modalidad *</Label>
                            <Controller
                                name="modalidadTrabajoId"
                                control={control}
                                render={({ field }) => (
                                    <Select
                                        value={field.value?.toString() || ""}
                                        onValueChange={(val) => field.onChange(Number(val))}
                                    >
                                        <SelectTrigger id="modalidadTrabajoId">
                                            <SelectValue placeholder="Selecciona modalidad" />
                                        </SelectTrigger>
                                        <SelectContent>
                                            {modalidades?.map((mod) => (
                                                <SelectItem
                                                    key={mod.id}
                                                    value={mod.id.toString()}
                                                >
                                                    {mod.nombre}
                                                </SelectItem>
                                            ))}
                                        </SelectContent>
                                    </Select>
                                )}
                            />
                            {errors.modalidadTrabajoId && (
                                <p className="text-sm text-red-400 mt-1">
                                    {(errors.modalidadTrabajoId as { message?: string }).message}
                                </p>
                            )}
                        </div>
                    </div>
                </div>
            </Card>

            <Card className="p-6 mb-6">
                <h2 className="text-lg font-semibold mb-4">Presupuesto</h2>
                <PresupuestoFields
                    control={control}
                    errors={errors}
                    monedas={monedas}
                />
            </Card>

            <Card className="p-6 mb-6">
                <h2 className="text-lg font-semibold mb-4">Ubicacion y fechas</h2>

                {showUbicacion && (
                    <UbicacionFields
                        control={control as Parameters<typeof UbicacionFields>[0]["control"]}
                        errors={errors as Parameters<typeof UbicacionFields>[0]["errors"]}
                    />
                )}

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                        <Label>Fecha limite propuestas</Label>
                        <Controller
                            name="fechaLimitePropuestas"
                            control={control}
                            render={({ field }) => (
                                <Popover>
                                    <PopoverTrigger asChild>
                                        <Button
                                            variant="outline"
                                            className={cn(
                                                "w-full justify-start text-left font-normal",
                                                !field.value && "text-muted-foreground"
                                            )}
                                        >
                                            <CalendarIcon className="mr-2 h-4 w-4" />
                                            {field.value
                                                ? format(new Date(field.value as string), "PPP", { locale: es })
                                                : "Selecciona fecha"}
                                        </Button>
                                    </PopoverTrigger>
                                    <PopoverContent className="w-auto p-0">
                                        <Calendar
                                            mode="single"
                                            selected={field.value ? new Date(field.value as string) : undefined}
                                            onSelect={(date) =>
                                                field.onChange(date?.toISOString())
                                            }
                                            disabled={(date) => date < new Date()}
                                        />
                                    </PopoverContent>
                                </Popover>
                            )}
                        />
                        {errors.fechaLimitePropuestas && (
                            <p className="text-sm text-red-400 mt-1">
                                {(errors.fechaLimitePropuestas as { message?: string }).message}
                            </p>
                        )}
                    </div>
                    <div>
                        <Label>Fecha inicio prevista</Label>
                        <Controller
                            name="fechaInicioPrevista"
                            control={control}
                            render={({ field }) => (
                                <Popover>
                                    <PopoverTrigger asChild>
                                        <Button
                                            variant="outline"
                                            className={cn(
                                                "w-full justify-start text-left font-normal",
                                                !field.value && "text-muted-foreground"
                                            )}
                                        >
                                            <CalendarIcon className="mr-2 h-4 w-4" />
                                            {field.value
                                                ? format(new Date(field.value as string), "PPP", { locale: es })
                                                : "Selecciona fecha"}
                                        </Button>
                                    </PopoverTrigger>
                                    <PopoverContent className="w-auto p-0">
                                        <Calendar
                                            mode="single"
                                            selected={field.value ? new Date(field.value as string) : undefined}
                                            onSelect={(date) =>
                                                field.onChange(date?.toISOString())
                                            }
                                            disabled={(date) => date < new Date()}
                                        />
                                    </PopoverContent>
                                </Popover>
                            )}
                        />
                        {errors.fechaInicioPrevista && (
                            <p className="text-sm text-red-400 mt-1">
                                {(errors.fechaInicioPrevista as { message?: string }).message}
                            </p>
                        )}
                    </div>
                </div>

                {mode === "create" && (
                    <div className="mt-4">
                        <Label htmlFor="proyectoArtisticoId">Proyecto artistico *</Label>
                        <Input
                            id="proyectoArtisticoId"
                            {...register("proyectoArtisticoId")}
                            placeholder="ID del proyecto"
                        />
                        {errors.proyectoArtisticoId && (
                            <p className="text-sm text-red-400 mt-1">
                                {(errors.proyectoArtisticoId as { message?: string }).message}
                            </p>
                        )}
                    </div>
                )}
            </Card>

            <div className="flex justify-between">
                <Button type="button" variant="outline" onClick={onCancel}>
                    Cancelar
                </Button>
                <Button type="submit" disabled={isSubmitting}>
                    {isSubmitting && <Loader2 className="animate-spin mr-2 h-4 w-4" />}
                    {mode === "create" ? "Publicar Necesidad" : "Guardar Cambios"}
                </Button>
            </div>
        </form>
    )
}
