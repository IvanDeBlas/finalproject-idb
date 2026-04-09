"use client"

import { useEffect, useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Loader2 } from "lucide-react"
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogFooter,
    Input,
    Textarea,
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
    Checkbox,
    Button,
    Label,
} from "@/components/ui"
import { useCreateReward, useUpdateReward } from "@/hooks/use-rewards"
import {
    createRewardSchema,
    type CreateRewardFormData,
} from "@shared/schemas/reward.schema"
import { TIPO_REWARD_LABELS } from "@shared/constants"
import { toast } from "sonner"
import type { Reward } from "@shared/types"

interface RewardFormModalProps {
    isOpen: boolean
    onClose: () => void
    campaniaId: string
    reward: Reward | null
}

export function RewardFormModal({
    isOpen,
    onClose,
    campaniaId,
    reward,
}: RewardFormModalProps) {
    const isEdit = !!reward
    const { mutate: create, isPending: isCreating } = useCreateReward()
    const { mutate: update, isPending: isUpdating } = useUpdateReward()
    const isPending = isCreating || isUpdating

    const [stockLimitado, setStockLimitado] = useState(false)
    const [envioFisico, setEnvioFisico] = useState(false)

    const {
        register,
        handleSubmit,
        formState: { errors },
        watch,
        reset,
        setValue,
    } = useForm<CreateRewardFormData>({
        resolver: zodResolver(createRewardSchema),
        defaultValues: {
            campaniaId,
            monedaId: 1,
            esAddOn: false,
            incluyeEnvioFisico: false,
            orden: 0,
        },
    })

    const nombreValue = watch("nombre") || ""
    const descripcionValue = watch("descripcion") || ""

    useEffect(() => {
        if (isOpen && reward) {
            reset({
                campaniaId: reward.campaniaId,
                tipoRewardId: reward.tipoRewardId,
                nombre: reward.nombre,
                descripcion: reward.descripcion || "",
                importeMinimo: reward.importeMinimo,
                monedaId: reward.monedaId,
                esAddOn: reward.esAddOn,
                cantidadMaxima: reward.cantidadMaxima ?? undefined,
                cantidadPorBacker: reward.cantidadPorBacker ?? undefined,
                incluyeEnvioFisico: reward.incluyeEnvioFisico,
                tiempoEntregaEstimado: reward.tiempoEntregaEstimado || "",
                orden: reward.orden,
            })
            setStockLimitado(reward.cantidadMaxima != null)
            setEnvioFisico(reward.incluyeEnvioFisico)
        } else if (isOpen) {
            reset({
                campaniaId,
                monedaId: 1,
                esAddOn: false,
                incluyeEnvioFisico: false,
                orden: 0,
            })
            setStockLimitado(false)
            setEnvioFisico(false)
        }
    }, [isOpen, reward, reset, campaniaId])

    const onSubmit = (formData: CreateRewardFormData) => {
        // Convert null to undefined for API compatibility
        const data = {
            ...formData,
            cantidadMaxima: formData.cantidadMaxima ?? undefined,
            cantidadPorBacker: formData.cantidadPorBacker ?? undefined,
        }

        if (isEdit && reward) {
            update(
                {
                    id: reward.id,
                    data: { id: reward.id, ...data },
                },
                {
                    onSuccess: () => {
                        toast.success("Recompensa actualizada exitosamente")
                        onClose()
                    },
                    onError: () => {
                        toast.error("Error al actualizar recompensa")
                    },
                }
            )
        } else {
            create(data, {
                onSuccess: () => {
                    toast.success("Recompensa creada exitosamente")
                    onClose()
                },
                onError: () => {
                    toast.error("Error al crear recompensa")
                },
            })
        }
    }

    return (
        <Dialog open={isOpen} onOpenChange={onClose}>
            <DialogContent className="bg-card border-border max-w-2xl max-h-[90vh] overflow-y-auto">
                <DialogHeader className="border-b border-border pb-4 mb-6">
                    <DialogTitle className="text-2xl font-bold">
                        {isEdit ? "Editar Recompensa" : "Nueva Recompensa"}
                    </DialogTitle>
                </DialogHeader>

                <form
                    onSubmit={handleSubmit(onSubmit)}
                    className="space-y-6 px-1"
                >
                    {/* Nombre */}
                    <div>
                        <Label
                            htmlFor="nombre"
                            className="text-sm font-medium text-foreground mb-2 block after:content-['*'] after:text-red-500 after:ml-1"
                        >
                            Nombre de la recompensa
                        </Label>
                        <Input
                            id="nombre"
                            {...register("nombre")}
                            placeholder="Ej: Descarga Digital"
                            className="bg-background border-border text-foreground placeholder:text-muted-foreground focus:border-primary"
                        />
                        <div className="flex items-center justify-between mt-1">
                            {errors.nombre && (
                                <p className="text-xs text-red-500">
                                    {errors.nombre.message}
                                </p>
                            )}
                            <p
                                className={`text-xs ml-auto ${
                                    nombreValue.length > 190
                                        ? "text-red-500"
                                        : nombreValue.length > 160
                                          ? "text-amber-500"
                                          : "text-muted-foreground"
                                }`}
                            >
                                {nombreValue.length}/200 caracteres
                            </p>
                        </div>
                    </div>

                    {/* Descripcion */}
                    <div>
                        <Label
                            htmlFor="descripcion"
                            className="text-sm font-medium text-foreground mb-2 block"
                        >
                            Descripcion
                        </Label>
                        <Textarea
                            id="descripcion"
                            {...register("descripcion")}
                            placeholder="Describe que incluye esta recompensa..."
                            className="bg-background border-border text-foreground min-h-[120px] resize-none placeholder:text-muted-foreground"
                        />
                        <p
                            className={`text-xs mt-1 ${
                                descripcionValue.length > 1900
                                    ? "text-red-500"
                                    : descripcionValue.length > 1600
                                      ? "text-amber-500"
                                      : "text-muted-foreground"
                            }`}
                        >
                            {descripcionValue.length}/2000 caracteres
                        </p>
                        {errors.descripcion && (
                            <p className="text-xs text-red-500 mt-1">
                                {errors.descripcion.message}
                            </p>
                        )}
                    </div>

                    {/* Grid 2 columns */}
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        {/* Importe Minimo */}
                        <div>
                            <Label
                                htmlFor="importeMinimo"
                                className="text-sm font-medium text-foreground mb-2 block after:content-['*'] after:text-red-500 after:ml-1"
                            >
                                Importe minimo
                            </Label>
                            <div className="relative">
                                <span className="absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground">
                                    &euro;
                                </span>
                                <Input
                                    id="importeMinimo"
                                    type="number"
                                    step="0.01"
                                    {...register("importeMinimo", {
                                        valueAsNumber: true,
                                    })}
                                    placeholder="10.00"
                                    className="bg-background border-border text-foreground pl-8"
                                />
                            </div>
                            {errors.importeMinimo && (
                                <p className="text-xs text-red-500 mt-1">
                                    {errors.importeMinimo.message}
                                </p>
                            )}
                        </div>

                        {/* Tipo Recompensa */}
                        <div>
                            <Label
                                htmlFor="tipoRewardId"
                                className="text-sm font-medium text-foreground mb-2 block after:content-['*'] after:text-red-500 after:ml-1"
                            >
                                Tipo de recompensa
                            </Label>
                            <Select
                                defaultValue={
                                    reward?.tipoRewardId?.toString() ?? undefined
                                }
                                onValueChange={(value) =>
                                    setValue("tipoRewardId", Number(value))
                                }
                            >
                                <SelectTrigger className="bg-background border-border text-foreground">
                                    <SelectValue placeholder="Seleccionar tipo" />
                                </SelectTrigger>
                                <SelectContent>
                                    {Object.entries(TIPO_REWARD_LABELS).map(
                                        ([id, label]) => (
                                            <SelectItem key={id} value={id}>
                                                {label}
                                            </SelectItem>
                                        )
                                    )}
                                </SelectContent>
                            </Select>
                            {errors.tipoRewardId && (
                                <p className="text-xs text-red-500 mt-1">
                                    {errors.tipoRewardId.message}
                                </p>
                            )}
                        </div>
                    </div>

                    {/* Es Add-on */}
                    <div className="flex items-center gap-2">
                        <Checkbox
                            id="esAddOn"
                            checked={watch("esAddOn")}
                            onCheckedChange={(checked) =>
                                setValue("esAddOn", checked as boolean)
                            }
                            className="border-border data-[state=checked]:bg-primary data-[state=checked]:border-primary"
                        />
                        <Label
                            htmlFor="esAddOn"
                            className="text-sm text-foreground cursor-pointer"
                        >
                            Es add-on (complemento)
                        </Label>
                    </div>

                    {/* Stock Limitado */}
                    <div>
                        <div className="flex items-center gap-2 mb-2">
                            <Checkbox
                                id="stockLimitado"
                                checked={stockLimitado}
                                onCheckedChange={(checked) => {
                                    setStockLimitado(checked as boolean)
                                    if (!checked) {
                                        setValue("cantidadMaxima", undefined)
                                        setValue(
                                            "cantidadPorBacker",
                                            undefined
                                        )
                                    }
                                }}
                                className="border-border data-[state=checked]:bg-primary data-[state=checked]:border-primary"
                            />
                            <Label
                                htmlFor="stockLimitado"
                                className="text-sm text-foreground cursor-pointer"
                            >
                                Stock limitado
                            </Label>
                        </div>

                        {stockLimitado && (
                            <div className="bg-background border border-border p-4 rounded-lg mt-2 space-y-4">
                                <div>
                                    <Label
                                        htmlFor="cantidadMaxima"
                                        className="text-sm font-medium text-foreground mb-2 block"
                                    >
                                        Cantidad maxima disponible
                                    </Label>
                                    <Input
                                        id="cantidadMaxima"
                                        type="number"
                                        {...register("cantidadMaxima", {
                                            valueAsNumber: true,
                                        })}
                                        placeholder="200"
                                        className="bg-card border-border text-foreground"
                                    />
                                    {errors.cantidadMaxima && (
                                        <p className="text-xs text-red-500 mt-1">
                                            {errors.cantidadMaxima.message}
                                        </p>
                                    )}
                                </div>

                                <div>
                                    <Label
                                        htmlFor="cantidadPorBacker"
                                        className="text-sm font-medium text-foreground mb-2 block"
                                    >
                                        Maximo por backer (opcional)
                                    </Label>
                                    <Input
                                        id="cantidadPorBacker"
                                        type="number"
                                        {...register("cantidadPorBacker", {
                                            valueAsNumber: true,
                                        })}
                                        placeholder="1"
                                        className="bg-card border-border text-foreground"
                                    />
                                    {errors.cantidadPorBacker && (
                                        <p className="text-xs text-red-500 mt-1">
                                            {errors.cantidadPorBacker.message}
                                        </p>
                                    )}
                                </div>
                            </div>
                        )}
                    </div>

                    {/* Envio Fisico */}
                    <div>
                        <div className="flex items-center gap-2 mb-2">
                            <Checkbox
                                id="incluyeEnvioFisico"
                                checked={envioFisico}
                                onCheckedChange={(checked) => {
                                    setEnvioFisico(checked as boolean)
                                    setValue(
                                        "incluyeEnvioFisico",
                                        checked as boolean
                                    )
                                    if (!checked) {
                                        setValue("tiempoEntregaEstimado", "")
                                    }
                                }}
                                className="border-border data-[state=checked]:bg-primary data-[state=checked]:border-primary"
                            />
                            <Label
                                htmlFor="incluyeEnvioFisico"
                                className="text-sm text-foreground cursor-pointer"
                            >
                                Incluye envio fisico
                            </Label>
                        </div>

                        {envioFisico && (
                            <div className="bg-background border border-border p-4 rounded-lg mt-2">
                                <Label
                                    htmlFor="tiempoEntregaEstimado"
                                    className="text-sm font-medium text-foreground mb-2 block"
                                >
                                    Tiempo de entrega estimado
                                </Label>
                                <Input
                                    id="tiempoEntregaEstimado"
                                    {...register("tiempoEntregaEstimado")}
                                    placeholder="Marzo 2025"
                                    className="bg-card border-border text-foreground"
                                />
                                {errors.tiempoEntregaEstimado && (
                                    <p className="text-xs text-red-500 mt-1">
                                        {
                                            errors.tiempoEntregaEstimado
                                                .message
                                        }
                                    </p>
                                )}
                            </div>
                        )}
                    </div>

                    {/* Footer */}
                    <DialogFooter className="border-t border-border pt-4 mt-6 flex gap-3 justify-end">
                        <Button
                            type="button"
                            variant="outline"
                            onClick={onClose}
                            disabled={isPending}
                            className="border-border text-muted-foreground hover:text-foreground hover:bg-secondary"
                        >
                            Cancelar
                        </Button>
                        <Button
                            type="submit"
                            disabled={isPending}
                            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8"
                        >
                            {isPending && (
                                <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                            )}
                            {isPending ? "Guardando..." : "Guardar"}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    )
}
