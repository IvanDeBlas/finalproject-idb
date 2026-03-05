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
import { rechazarEntregableSchema } from "@shared/schemas/crowdsourcing.schema"
import type { RechazarEntregableFormData } from "@shared/schemas/crowdsourcing.schema"
import { useRechazarEntregable } from "../../application/hooks/useRechazarEntregable"
import type { Entregable } from "../../domain"

interface RechazarEntregableDialogProps {
    entregable: Entregable | null
    acuerdoId: string
    isOpen: boolean
    onClose: () => void
}

export const RechazarEntregableDialog: FC<RechazarEntregableDialogProps> = ({
    entregable,
    acuerdoId,
    isOpen,
    onClose,
}) => {
    const mutation = useRechazarEntregable(acuerdoId, onClose)

    const {
        register,
        handleSubmit,
        reset,
        watch,
        formState: { errors, isValid },
    } = useForm<RechazarEntregableFormData>({
        resolver: zodResolver(rechazarEntregableSchema),
        defaultValues: { comentario: "" },
        mode: "onChange",
    })

    const comentarioValue = watch("comentario")

    const onSubmit = (data: RechazarEntregableFormData) => {
        if (!entregable) return
        mutation.mutate({
            entregableId: entregable.id,
            data: { comentario: data.comentario },
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
                        Rechazar entregable
                    </DialogTitle>
                    <DialogDescription className="text-sm text-slate-400">
                        Rechazar &ldquo;{entregable?.titulo}&rdquo;
                    </DialogDescription>
                </DialogHeader>

                <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="comentario" className="text-slate-300">
                            Comentario (obligatorio)
                        </Label>
                        <Textarea
                            id="comentario"
                            placeholder="Explica que debe corregirse (min. 10 caracteres)"
                            className="bg-slate-800 border-slate-600 text-white resize-none"
                            rows={4}
                            {...register("comentario")}
                        />
                        <div className="flex items-center justify-between">
                            {errors.comentario ? (
                                <p className="text-xs text-red-400">
                                    {errors.comentario.message}
                                </p>
                            ) : (
                                <span />
                            )}
                            <span className="text-xs text-slate-500">
                                {comentarioValue?.length ?? 0}/500
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
                            Cancelar
                        </Button>
                        <Button
                            type="submit"
                            variant="destructive"
                            disabled={mutation.isPending || !isValid}
                            className="bg-red-600 hover:bg-red-700"
                        >
                            {mutation.isPending ? (
                                <>
                                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                                    Rechazando...
                                </>
                            ) : (
                                "Rechazar"
                            )}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    )
}
