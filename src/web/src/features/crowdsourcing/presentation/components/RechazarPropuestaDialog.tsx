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
import { XCircle, Loader2 } from "lucide-react"
import { rechazarPropuestaSchema } from "@shared/schemas/crowdsourcing.schema"
import type { RechazarPropuestaFormData } from "@shared/schemas/crowdsourcing.schema"
import { useRechazarPropuesta } from "../../application/hooks/useRechazarPropuesta"

interface RechazarPropuestaDialogProps {
    propuestaId: string | null
    necesidadId: string
    isOpen: boolean
    onClose: () => void
}

export const RechazarPropuestaDialog: FC<RechazarPropuestaDialogProps> = ({
    propuestaId,
    necesidadId,
    isOpen,
    onClose,
}) => {
    const mutation = useRechazarPropuesta(necesidadId, onClose)

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm<RechazarPropuestaFormData>({
        resolver: zodResolver(rechazarPropuestaSchema),
        defaultValues: { motivo: "" },
    })

    const onSubmit = (data: RechazarPropuestaFormData) => {
        if (!propuestaId) return
        mutation.mutate({
            propuestaId,
            data: { motivo: data.motivo || undefined },
        })
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
                        <XCircle className="h-5 w-5 text-red-400" />
                        Rechazar propuesta
                    </DialogTitle>
                    <DialogDescription className="text-sm text-slate-400">
                        El profesional no podra ver el motivo de rechazo
                    </DialogDescription>
                </DialogHeader>

                <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="motivo" className="text-slate-300">
                            Motivo (opcional)
                        </Label>
                        <Textarea
                            id="motivo"
                            placeholder="Motivo del rechazo (max 500 caracteres)"
                            className="bg-slate-800 border-slate-600 text-white resize-none"
                            rows={3}
                            {...register("motivo")}
                        />
                        {errors.motivo && (
                            <p className="text-xs text-red-400">
                                {errors.motivo.message}
                            </p>
                        )}
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
                            Cancelar
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
                                    Rechazando...
                                </>
                            ) : (
                                "Rechazar propuesta"
                            )}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    )
}
