"use client"

import { useState, useEffect } from "react"
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
import { Check, Loader2, ExternalLink } from "lucide-react"
import { toast } from "sonner"
import { useValidarTarea } from "@/hooks/use-validar-tarea"
import type { TareaPendienteItem } from "@shared/types/crowdpromotion"

interface ValidarTareaDialogProps {
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

export function ValidarTareaDialog({ isOpen, onClose, item, programaId }: ValidarTareaDialogProps) {
    const [comentarioValidacion, setComentarioValidacion] = useState("")
    const { mutate, isPending } = useValidarTarea()

    useEffect(() => {
        if (isOpen) {
            setComentarioValidacion("")
        }
    }, [isOpen])

    if (!item) return null

    const handleValidar = () => {
        mutate(
            {
                programaId,
                tareaPromotorId: item.tareaPromotorId,
                data: { comentarioValidacion: comentarioValidacion || undefined },
            },
            {
                onSuccess: (response) => {
                    const msg = response.recompensaAcreditada
                        ? `Tarea validada. Se acreditaron ${response.recompensaAcreditada} ${response.monedaNombre ?? "EUR"} en la wallet del promotor.`
                        : "Tarea validada exitosamente."
                    toast.success(msg)
                    onClose()
                },
                onError: () => {
                    toast.error("No se pudo validar la tarea. Intentalo de nuevo.")
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
                    <DialogTitle className="text-white">Validar tarea completada</DialogTitle>
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

                <div className="px-6 py-4">
                    <Label className="text-sm text-[#94a3b8] mb-2 block">
                        Comentario de validacion (opcional)
                    </Label>
                    <Textarea
                        value={comentarioValidacion}
                        onChange={(e) => setComentarioValidacion(e.target.value)}
                        placeholder="Escribe un comentario opcional..."
                        className="bg-[#0f0f1f] border-[#334155] min-h-[72px] resize-none focus:border-[#a855f7] text-white placeholder:text-[#64748b]"
                        maxLength={500}
                    />
                </div>

                <DialogFooter className="px-6 pb-6 pt-2 border-t border-[#334155]">
                    <Button
                        variant="ghost"
                        onClick={onClose}
                        disabled={isPending}
                        className="text-[#94a3b8] hover:text-white"
                    >
                        Cancelar
                    </Button>
                    <Button
                        onClick={handleValidar}
                        disabled={isPending}
                        className="bg-green-600 hover:bg-green-700 text-white"
                    >
                        {isPending ? (
                            <>
                                <Loader2 className="w-4 h-4 animate-spin mr-2" aria-hidden="true" />
                                Validando...
                            </>
                        ) : (
                            <>
                                <Check className="w-4 h-4 mr-2" aria-hidden="true" />
                                Validar tarea
                            </>
                        )}
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    )
}
