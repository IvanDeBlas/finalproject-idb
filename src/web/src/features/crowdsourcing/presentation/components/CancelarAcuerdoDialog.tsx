import { FC } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from "@/components/ui/dialog"
import { Textarea } from "@/components/ui/textarea"
import { Label } from "@/components/ui/label"
import { Button } from "@/components/ui/button"
import { AlertTriangle, Loader2 } from "lucide-react"
import { cancelarAcuerdoSchema } from "@shared/schemas/crowdsourcing.schema"
import type { CancelarAcuerdoFormData } from "@shared/schemas/crowdsourcing.schema"
import { useCancelarAcuerdo } from "../../application/hooks/useCancelarAcuerdo"

interface CancelarAcuerdoDialogProps {
    acuerdoId: string
    isOpen: boolean
    onClose: () => void
}

export const CancelarAcuerdoDialog: FC<CancelarAcuerdoDialogProps> = ({
    acuerdoId,
    isOpen,
    onClose,
}) => {
    const mutation = useCancelarAcuerdo(acuerdoId, onClose)

    const {
        register,
        handleSubmit,
        reset,
        watch,
        formState: { errors },
    } = useForm<CancelarAcuerdoFormData>({
        resolver: zodResolver(cancelarAcuerdoSchema),
        defaultValues: { motivo: "" },
    })

    const motivoValue = watch("motivo")

    const onSubmit = (data: CancelarAcuerdoFormData) => {
        mutation.mutate({ motivo: data.motivo })
        reset()
    }

    return (
        <Dialog
            open={isOpen}
            onOpenChange={(open) => {
                if (!open) {
                    reset()
                    onClose()
                }
            }}
        >
            <DialogContent className="max-w-md bg-[#0f1729] border-[#334155] text-white">
                <DialogHeader>
                    <DialogTitle className="text-lg font-semibold text-white flex items-center gap-2">
                        <AlertTriangle className="h-5 w-5 text-red-400" />
                        Cancelar acuerdo
                    </DialogTitle>
                    <DialogDescription className="text-sm text-slate-400">
                        Esta accion es irreversible
                    </DialogDescription>
                </DialogHeader>

                <div className="bg-red-900/20 border border-red-700 rounded-lg p-3 flex items-start gap-2">
                    <AlertTriangle className="h-4 w-4 text-red-400 shrink-0 mt-0.5" />
                    <p className="text-sm text-red-300">
                        Cancelar un acuerdo es una accion irreversible. Ambas partes
                        seran notificadas.
                    </p>
                </div>

                <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="motivo" className="text-slate-300">
                            Motivo de cancelacion (obligatorio)
                        </Label>
                        <Textarea
                            id="motivo"
                            placeholder="Explica el motivo de la cancelacion (min. 20 caracteres)"
                            className="bg-slate-800 border-slate-600 text-white resize-none"
                            rows={4}
                            {...register("motivo")}
                        />
                        <div className="flex items-center justify-between">
                            {errors.motivo ? (
                                <p className="text-xs text-red-400">
                                    {errors.motivo.message}
                                </p>
                            ) : (
                                <span />
                            )}
                            <span className="text-xs text-slate-500">
                                {motivoValue?.length ?? 0}/1000
                            </span>
                        </div>
                    </div>

                    <DialogFooter className="pt-2">
                        <Button
                            type="button"
                            variant="outline"
                            disabled={mutation.isPending}
                            className="border-slate-600 text-white hover:bg-slate-800"
                            onClick={() => {
                                reset()
                                onClose()
                            }}
                        >
                            Volver
                        </Button>
                        <Button
                            type="submit"
                            variant="destructive"
                            disabled={mutation.isPending}
                            className="bg-red-600 hover:bg-red-700"
                        >
                            {mutation.isPending ? (
                                <>
                                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                                    Cancelando...
                                </>
                            ) : (
                                "Cancelar acuerdo"
                            )}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    )
}
