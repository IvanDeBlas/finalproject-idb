"use client"

import { useEffect } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import {
    Dialog,
    DialogContent,
    DialogTitle,
    DialogFooter,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Textarea } from "@/components/ui/textarea"
import { Label } from "@/components/ui/label"
import { Separator } from "@/components/ui/separator"
import { X, Loader2, ExternalLink, AlertTriangle } from "lucide-react"
import { toast } from "sonner"
import { useRechazarTarea } from "@/hooks/use-rechazar-tarea"
import { rechazarTareaSchema } from "@shared/schemas/crowdpromotion.schema"
import type { RechazarTareaFormData } from "@shared/schemas/crowdpromotion.schema"
import type { TareaPendienteItem } from "@shared/types/crowdpromotion"

interface RechazarTareaDialogProps {
    isOpen: boolean
    onClose: () => void
    item: TareaPendienteItem | null
    programaId: string
}

function formatOrdinal(n: number): string {
    if (n === 1) return "1ra vez"
    if (n === 2) return "2da vez"
    if (n === 3) return "3ra vez"
    return `${n}ta vez`
}

export function RechazarTareaDialog({ isOpen, onClose, item, programaId }: RechazarTareaDialogProps) {
    const { mutate, isPending } = useRechazarTarea()

    const {
        register,
        handleSubmit,
        reset,
        watch,
        formState: { errors },
    } = useForm<RechazarTareaFormData>({
        resolver: zodResolver(rechazarTareaSchema),
        defaultValues: { comentarioValidacion: "" },
    })

    const comentarioValue = watch("comentarioValidacion") ?? ""

    useEffect(() => {
        if (isOpen) {
            reset({ comentarioValidacion: "" })
        }
    }, [isOpen, reset])

    if (!item) return null

    const onSubmit = (formData: RechazarTareaFormData) => {
        mutate(
            {
                programaId,
                tareaPromotorId: item.tareaPromotorId,
                data: { comentarioValidacion: formData.comentarioValidacion },
            },
            {
                onSuccess: () => {
                    toast.success("Tarea rechazada. El promotor podra re-enviar con nueva prueba.")
                    onClose()
                },
                onError: () => {
                    toast.error("No se pudo rechazar la tarea. Intentalo de nuevo.")
                },
            }
        )
    }

    return (
        <Dialog
            open={isOpen}
            onOpenChange={(open) => {
                if (!open && !isPending) onClose()
            }}
        >
            <DialogContent className="bg-[#151525] border-[#334155] max-w-md rounded-xl p-0">
                <div className="px-6 pt-6 pb-4 border-b border-[#334155]">
                    <DialogTitle className="text-white">Rechazar tarea</DialogTitle>
                </div>

                <div className="px-6 py-4 flex flex-col gap-3">
                    <div className="flex items-center gap-2">
                        <span className="text-xs text-[#64748b] w-20 shrink-0">Tarea:</span>
                        <span className="text-sm text-white">{item.tareaNombre}</span>
                    </div>
                    <div className="flex items-center gap-2">
                        <span className="text-xs text-[#64748b] w-20 shrink-0">Promotor:</span>
                        <span className="text-sm text-white">
                            {item.promotorNombre}
                            {item.promotorTipoNombre && (
                                <span className="text-xs text-[#64748b] ml-1">({item.promotorTipoNombre})</span>
                            )}
                        </span>
                    </div>
                    <div className="flex items-center gap-2">
                        <span className="text-xs text-[#64748b] w-20 shrink-0">Ejecucion:</span>
                        <span className="text-sm text-white">{formatOrdinal(item.vecesCompletada)}</span>
                    </div>
                    {item.urlPruebaCompletado && (
                        <div className="flex items-center gap-2">
                            <span className="text-xs text-[#64748b] w-20 shrink-0">Prueba:</span>
                            <a
                                href={item.urlPruebaCompletado}
                                target="_blank"
                                rel="noopener noreferrer"
                                className="text-sm text-[#a855f7] hover:underline truncate flex items-center gap-1"
                            >
                                {item.urlPruebaCompletado}
                                <ExternalLink className="w-3 h-3 shrink-0" aria-hidden="true" />
                            </a>
                        </div>
                    )}

                    <div className="bg-[#0f0f1f] border border-[#334155] rounded-lg p-3 mt-1">
                        {item.comentarioPromotor ? (
                            <p className="text-sm text-[#94a3b8] italic">{item.comentarioPromotor}</p>
                        ) : (
                            <p className="text-xs text-[#64748b] italic">(sin comentario del promotor)</p>
                        )}
                    </div>
                </div>

                <Separator className="bg-[#334155]" />

                <div className="px-6 py-3 bg-amber-950/20 border-y border-amber-900/30 flex items-start gap-2.5">
                    <AlertTriangle className="w-4 h-4 text-amber-400 shrink-0 mt-0.5" aria-hidden="true" />
                    <p className="text-xs text-amber-300/80">
                        El promotor vera este motivo y podra re-enviar la tarea con una nueva prueba.
                    </p>
                </div>

                <form onSubmit={handleSubmit(onSubmit)}>
                    <div className="px-6 py-4">
                        <Label className="text-sm text-[#94a3b8] mb-2 block">
                            Motivo del rechazo <span className="text-red-400">*</span>
                        </Label>
                        <Textarea
                            {...register("comentarioValidacion")}
                            placeholder="Explica al promotor por que se rechaza esta tarea..."
                            className="bg-[#0f0f1f] border-[#334155] min-h-[80px] resize-none focus:border-red-500 text-white placeholder:text-[#64748b]"
                            maxLength={500}
                        />
                        <div className="flex items-center justify-between mt-1.5">
                            {errors.comentarioValidacion ? (
                                <p className="text-xs text-red-400">
                                    {errors.comentarioValidacion.message}
                                </p>
                            ) : (
                                <span />
                            )}
                            <p className="text-right text-xs text-[#64748b]">
                                {comentarioValue.length} / 500 caracteres
                            </p>
                        </div>
                    </div>

                    <DialogFooter className="px-6 pb-6 pt-2 border-t border-[#334155]">
                        <Button
                            type="button"
                            variant="ghost"
                            onClick={onClose}
                            disabled={isPending}
                            className="text-[#94a3b8] hover:text-white"
                        >
                            Cancelar
                        </Button>
                        <Button
                            type="submit"
                            disabled={isPending}
                            className="bg-red-600/80 hover:bg-red-700 border border-red-700/50 text-white"
                        >
                            {isPending ? (
                                <>
                                    <Loader2 className="w-4 h-4 animate-spin mr-2" aria-hidden="true" />
                                    Rechazando...
                                </>
                            ) : (
                                <>
                                    <X className="w-4 h-4 mr-2" aria-hidden="true" />
                                    Rechazar tarea
                                </>
                            )}
                        </Button>
                    </DialogFooter>
                </form>
            </DialogContent>
        </Dialog>
    )
}
