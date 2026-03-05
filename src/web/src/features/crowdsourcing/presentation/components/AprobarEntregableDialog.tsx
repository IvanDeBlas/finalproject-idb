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
import { CheckCircle, Loader2 } from "lucide-react"
import { aprobarEntregableSchema } from "@shared/schemas/crowdsourcing.schema"
import type { AprobarEntregableFormData } from "@shared/schemas/crowdsourcing.schema"
import { useAprobarEntregable } from "../../application/hooks/useAprobarEntregable"
import type { Entregable } from "../../domain"

interface AprobarEntregableDialogProps {
    entregable: Entregable | null
    acuerdoId: string
    isOpen: boolean
    onClose: () => void
}

export const AprobarEntregableDialog: FC<AprobarEntregableDialogProps> = ({
    entregable,
    acuerdoId,
    isOpen,
    onClose,
}) => {
    const mutation = useAprobarEntregable(acuerdoId, onClose)

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm<AprobarEntregableFormData>({
        resolver: zodResolver(aprobarEntregableSchema),
        defaultValues: { comentario: "" },
    })

    const onSubmit = (data: AprobarEntregableFormData) => {
        if (!entregable) return
        mutation.mutate({
            entregableId: entregable.id,
            data: { comentario: data.comentario || undefined },
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
                        <CheckCircle className="h-5 w-5 text-green-400" />
                        Aprobar entregable
                    </DialogTitle>
                    <DialogDescription className="text-sm text-slate-400">
                        Aprobar &ldquo;{entregable?.titulo}&rdquo;
                    </DialogDescription>
                </DialogHeader>

                <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="comentario" className="text-slate-300">
                            Comentario (opcional)
                        </Label>
                        <Textarea
                            id="comentario"
                            placeholder="Deja un comentario sobre el entregable..."
                            className="bg-slate-800 border-slate-600 text-white resize-none"
                            rows={3}
                            {...register("comentario")}
                        />
                        {errors.comentario && (
                            <p className="text-xs text-red-400">
                                {errors.comentario.message}
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
                            disabled={mutation.isPending}
                            className="bg-green-600 hover:bg-green-700"
                        >
                            {mutation.isPending ? (
                                <>
                                    <Loader2 className="w-4 h-4 mr-2 animate-spin" />
                                    Aprobando...
                                </>
                            ) : (
                                "Confirmar aprobacion"
                            )}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    )
}
