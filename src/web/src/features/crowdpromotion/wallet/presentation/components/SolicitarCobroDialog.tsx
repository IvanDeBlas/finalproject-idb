import { type FC, useState, useEffect } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import {
    Dialog,
    DialogContent,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import { Textarea } from "@/components/ui/textarea"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { AlertCircle, Loader2 } from "lucide-react"
import { QUERY_KEYS, VALIDATION } from "@shared/constants"
import { solicitarCobroSchema, type SolicitarCobroFormData } from "@shared/schemas/crowdpromotion.schema"
import { formatWalletImporte } from "@shared/utils/format"
import { getWalletErrorMessage } from "@shared/utils/error-messages"
import { useSolicitarCobro } from "../../application/hooks/useSolicitarCobro"

interface SolicitarCobroDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    saldoDisponible: number
    minimoRetiro: number
    monedaNombre?: string
    onSuccess: (saldoRestante: number) => void
}

export const SolicitarCobroDialog: FC<SolicitarCobroDialogProps> = ({
    open,
    onOpenChange,
    saldoDisponible,
    minimoRetiro,
    monedaNombre = "EUR",
    onSuccess,
}) => {
    const [apiError, setApiError] = useState<string | null>(null)
    const queryClient = useQueryClient()
    const { mutateAsync, isPending } = useSolicitarCobro()

    const form = useForm<SolicitarCobroFormData>({
        resolver: zodResolver(solicitarCobroSchema),
        defaultValues: {
            importe: undefined,
            descripcion: "",
        },
    })

    const watchedImporte = form.watch("importe")
    const watchedDescripcion = form.watch("descripcion") ?? ""

    useEffect(() => {
        if (watchedImporte !== undefined && watchedImporte > saldoDisponible) {
            form.setError("importe", {
                message: `El importe no puede superar tu saldo disponible (${formatWalletImporte(saldoDisponible, monedaNombre)})`,
            })
        } else {
            const currentError = form.formState.errors.importe
            if (currentError?.message?.includes("saldo disponible")) {
                form.clearErrors("importe")
            }
        }
    }, [watchedImporte, saldoDisponible, monedaNombre, form])

    useEffect(() => {
        if (open) {
            form.reset({ importe: undefined, descripcion: "" })
            setApiError(null)
        }
    }, [open, form])

    const handleSubmit = async (data: SolicitarCobroFormData) => {
        if (data.importe > saldoDisponible) {
            form.setError("importe", {
                message: `El importe no puede superar tu saldo disponible (${formatWalletImporte(saldoDisponible, monedaNombre)})`,
            })
            return
        }

        setApiError(null)

        try {
            const response = await mutateAsync({
                importe: data.importe,
                descripcion: data.descripcion || undefined,
            })

            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.wallet.resumen })
            queryClient.invalidateQueries({ queryKey: ["crowdpromotion", "wallet", "transacciones"] })

            onSuccess(response.saldoRestante)
            onOpenChange(false)
            toast.success("Solicitud de cobro registrada. Procesaremos tu pago en breve.")
        } catch (error) {
            const errorCode = (error as Error & { errorCode?: string }).errorCode ?? "5000"

            if (errorCode === "4042") {
                toast.error(getWalletErrorMessage(errorCode))
                onOpenChange(false)
                return
            }

            setApiError(getWalletErrorMessage(errorCode))
        }
    }

    return (
        <Dialog open={open} onOpenChange={(value) => { if (!isPending) onOpenChange(value) }}>
            <DialogContent
                onInteractOutside={(e) => { if (isPending) e.preventDefault() }}
                onEscapeKeyDown={(e) => { if (isPending) e.preventDefault() }}
            >
                <DialogHeader>
                    <DialogTitle className="text-white">Solicitar cobro</DialogTitle>
                </DialogHeader>

                <Separator className="bg-[#334155]" />

                <div className="text-sm text-[#94a3b8] space-y-1">
                    <p>Saldo disponible: <span className="text-[#f59e0b] font-semibold">{formatWalletImporte(saldoDisponible, monedaNombre)}</span></p>
                    <p>Minimo de retiro: {formatWalletImporte(minimoRetiro, monedaNombre)}</p>
                </div>

                {apiError && (
                    <Alert variant="destructive">
                        <AlertCircle className="h-4 w-4" />
                        <AlertDescription>{apiError}</AlertDescription>
                    </Alert>
                )}

                <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="importe" className="text-[#94a3b8]">
                            Importe ({monedaNombre})
                        </Label>
                        <Input
                            id="importe"
                            type="number"
                            step="0.01"
                            min={0}
                            placeholder={`Min. ${minimoRetiro.toFixed(2)}`}
                            className={`bg-[#0f0f1f] border-[#334155] focus:border-[#a855f7] ${
                                form.formState.errors.importe ? "border-red-500" : ""
                            }`}
                            {...form.register("importe", { valueAsNumber: true })}
                        />
                        {form.formState.errors.importe && (
                            <p role="alert" className="text-xs text-red-500">
                                {form.formState.errors.importe.message}
                            </p>
                        )}
                    </div>

                    <div className="space-y-2">
                        <div className="flex items-center justify-between">
                            <Label htmlFor="descripcion" className="text-[#94a3b8]">
                                Descripcion (opcional)
                            </Label>
                            <span className="text-xs text-[#64748b]">
                                {watchedDescripcion.length}/{VALIDATION.WALLET_DESCRIPCION_COBRO_MAX}
                            </span>
                        </div>
                        <Textarea
                            id="descripcion"
                            rows={3}
                            placeholder="Motivo de la solicitud..."
                            className={`bg-[#0f0f1f] border-[#334155] focus:border-[#a855f7] resize-none ${
                                form.formState.errors.descripcion ? "border-red-500" : ""
                            }`}
                            {...form.register("descripcion")}
                        />
                        {form.formState.errors.descripcion && (
                            <p role="alert" className="text-xs text-red-500">
                                {form.formState.errors.descripcion.message}
                            </p>
                        )}
                    </div>

                    <DialogFooter>
                        <Button
                            type="button"
                            variant="outline"
                            onClick={() => onOpenChange(false)}
                            disabled={isPending}
                            className="border-[#334155]"
                        >
                            Cancelar
                        </Button>
                        <Button
                            type="submit"
                            disabled={isPending}
                            className="bg-[#a855f7] hover:bg-[#9333ea] text-white"
                        >
                            {isPending && <Loader2 className="h-4 w-4 mr-2 animate-spin" />}
                            Confirmar cobro
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    )
}
