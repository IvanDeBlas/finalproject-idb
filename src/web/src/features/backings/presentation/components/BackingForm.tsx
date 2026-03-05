import { useForm, Controller } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { createBackingSchema, type CreateBackingFormData } from "@shared/schemas/backing.schema"

interface RewardForBacking {
    id: string
    nombre: string
    descripcion?: string
    importeMinimo: number
}
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Textarea } from "@/components/ui/textarea"
import { Card } from "@/components/ui/card"
import { Checkbox } from "@/components/ui/checkbox"
import { EyeOff, Loader2 } from "lucide-react"
import { AmountInput } from "./AmountInput"
import { formatCurrency } from "@/features/campanias/application/utils"
import { useMemo } from "react"

interface BackingFormProps {
    campaniaId: string
    reward: RewardForBacking | null
    onChangeReward?: () => void
    onSubmit: (data: CreateBackingFormData & { campaniaId: string }) => void
    isSubmitting?: boolean
    onCancel?: () => void
}

export function BackingForm({
    campaniaId,
    reward,
    onChangeReward,
    onSubmit,
    isSubmitting = false,
    onCancel,
}: BackingFormProps) {
    const minAmount = reward?.importeMinimo || 5

    const form = useForm<CreateBackingFormData>({
        resolver: zodResolver(createBackingSchema),
        defaultValues: {
            rewardId: reward?.id,
            monto: minAmount,
            mensaje: "",
            esAnonimo: false,
        },
    })

    const montoValue = form.watch("monto")
    const mensajeValue = form.watch("mensaje") || ""

    const rewardAmountError = useMemo(() => {
        if (!montoValue || !reward) return null
        if (montoValue < reward.importeMinimo) {
            return `El monto debe ser al menos ${reward.importeMinimo} EUR para esta recompensa`
        }
        return null
    }, [montoValue, reward])

    const handleSubmit = form.handleSubmit((data) => {
        if (rewardAmountError) return
        onSubmit({ ...data, campaniaId })
    })

    const addAmount = (extra: number) => {
        const current = form.getValues("monto") || 0
        form.setValue("monto", Math.min(current + extra, 100000), { shouldValidate: true })
    }

    return (
        <form onSubmit={handleSubmit} className="space-y-6">
            {/* Selected Reward */}
            <Card className="bg-[#1a1a2e] border-[#334155] p-4">
                <div className="flex items-start justify-between">
                    <div>
                        <h4 className="text-base font-semibold text-white">
                            {reward?.nombre || "Aporte libre"}
                        </h4>
                        <p className="text-sm text-[#94a3b8] mt-1">
                            Minimo {formatCurrency(minAmount)}
                        </p>
                    </div>
                    {onChangeReward && (
                        <Button
                            type="button"
                            variant="link"
                            size="sm"
                            className="text-primary hover:underline"
                            onClick={onChangeReward}
                        >
                            Cambiar
                        </Button>
                    )}
                </div>
            </Card>

            {/* Amount */}
            <Controller
                control={form.control}
                name="monto"
                render={({ field, fieldState }) => (
                    <AmountInput
                        value={field.value}
                        onChange={field.onChange}
                        minAmount={minAmount}
                        error={fieldState.error?.message || rewardAmountError || undefined}
                        hint={reward ? `Minimo ${formatCurrency(reward.importeMinimo)} para esta recompensa` : undefined}
                    />
                )}
            />

            {/* Quick Amount Buttons */}
            <div className="flex gap-2">
                {[5, 10, 25].map((amount) => (
                    <Button
                        key={amount}
                        type="button"
                        variant="outline"
                        size="sm"
                        className="border-[#334155] text-white hover:border-primary hover:bg-primary/10"
                        onClick={() => addAmount(amount)}
                    >
                        +€{amount}
                    </Button>
                ))}
            </div>

            {/* Message */}
            <div>
                <Label htmlFor="mensaje" className="text-sm font-medium text-[#cbd5e1]">
                    Mensaje para el artista (opcional)
                </Label>
                <Textarea
                    id="mensaje"
                    {...form.register("mensaje")}
                    placeholder="Escribe un mensaje de apoyo..."
                    maxLength={500}
                    className="mt-2 bg-[#1a1a2e] border-[#334155] text-white min-h-[80px] resize-none placeholder:text-[#64748b] focus:border-primary"
                />
                <span
                    className={`text-xs block mt-1 ${
                        mensajeValue.length > 475
                            ? "text-red-500"
                            : mensajeValue.length > 400
                              ? "text-yellow-500"
                              : "text-[#64748b]"
                    }`}
                >
                    {mensajeValue.length}/500
                </span>
                {form.formState.errors.mensaje && (
                    <p className="text-xs text-red-500 mt-1">
                        {form.formState.errors.mensaje.message}
                    </p>
                )}
            </div>

            {/* Anonymous Checkbox */}
            <Controller
                control={form.control}
                name="esAnonimo"
                render={({ field }) => (
                    <div className="flex items-center space-x-2">
                        <Checkbox
                            id="esAnonimo"
                            checked={field.value}
                            onCheckedChange={field.onChange}
                        />
                        <Label
                            htmlFor="esAnonimo"
                            className="text-sm text-white cursor-pointer"
                        >
                            <EyeOff className="w-4 h-4 inline mr-1 text-[#64748b]" />
                            Hacer anonimo mi apoyo
                        </Label>
                    </div>
                )}
            />

            {/* Summary */}
            <Card className="bg-[#1a1a2e] border-[#334155] p-4">
                <h4 className="text-lg font-bold text-white mb-3">Resumen</h4>
                <div className="space-y-2">
                    <div className="flex justify-between text-sm">
                        <span className="text-[#94a3b8]">Aportacion:</span>
                        <span className="text-white font-semibold">
                            {formatCurrency(montoValue || 0)}
                        </span>
                    </div>
                    {reward && (
                        <div className="flex justify-between text-sm">
                            <span className="text-[#94a3b8]">Reward:</span>
                            <span className="text-white line-clamp-1">{reward.nombre}</span>
                        </div>
                    )}
                    <div className="flex justify-between text-lg font-bold border-t border-[#334155] pt-3 mt-3">
                        <span className="text-white">Total:</span>
                        <span className="text-primary">
                            {formatCurrency(montoValue || 0)}
                        </span>
                    </div>
                </div>
            </Card>

            {/* Actions */}
            <div className="flex gap-3">
                {onCancel && (
                    <Button
                        type="button"
                        variant="outline"
                        className="border-[#334155] text-[#94a3b8] hover:text-white"
                        onClick={onCancel}
                    >
                        Cancelar
                    </Button>
                )}
                <Button
                    type="submit"
                    className="flex-1 bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold"
                    disabled={isSubmitting || !!rewardAmountError}
                >
                    {isSubmitting ? (
                        <>
                            <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                            Procesando...
                        </>
                    ) : (
                        "Confirmar Apoyo"
                    )}
                </Button>
            </div>
        </form>
    )
}
