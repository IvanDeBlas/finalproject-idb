import { FC, useEffect } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import {
    Dialog,
    DialogContent,
    DialogTitle,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Label } from "@/components/ui/label"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { X, Info, ExternalLink, Loader2 } from "lucide-react"
import { completarTareaSchema } from "@shared/schemas/crowdpromotion.schema"
import type { CompletarTareaFormData } from "@shared/schemas/crowdpromotion.schema"
import { VALIDATION } from "@shared/constants"
import type { MisTareasItem } from "../../domain"

interface CompletarTareaDialogProps {
    open: boolean
    onOpenChange: (open: boolean) => void
    tarea: MisTareasItem | null
    programaId: string
    esReenvio: boolean
    onSubmit: (data: CompletarTareaFormData) => Promise<void>
    isSubmitting: boolean
}

function getDialogTitle(tarea: MisTareasItem | null, esReenvio: boolean): string {
    if (!tarea) return "Completar tarea"
    if (esReenvio) return `Re-enviar: ${tarea.nombre}`
    if (tarea.miEstado) return `Completar de nuevo: ${tarea.nombre}`
    return `Completar: ${tarea.nombre}`
}

export const CompletarTareaDialog: FC<CompletarTareaDialogProps> = ({
    open,
    onOpenChange,
    tarea,
    esReenvio,
    onSubmit,
    isSubmitting,
}) => {
    const {
        register,
        handleSubmit,
        reset,
        watch,
        formState: { errors },
    } = useForm<CompletarTareaFormData>({
        resolver: zodResolver(completarTareaSchema),
        defaultValues: {
            urlPruebaCompletado: "",
            comentarioPromotor: "",
        },
    })

    const comentarioValue = watch("comentarioPromotor")

    useEffect(() => {
        if (open) {
            reset({ urlPruebaCompletado: "", comentarioPromotor: "" })
        }
    }, [open, reset])

    const handleFormSubmit = (data: CompletarTareaFormData) => {
        onSubmit(data)
    }

    return (
        <Dialog
            open={open}
            onOpenChange={isSubmitting ? undefined : onOpenChange}
        >
            <DialogContent
                className="bg-[#151525] border border-[#334155] text-white max-w-lg w-full rounded-xl p-0 overflow-hidden sm:max-w-lg max-sm:fixed max-sm:bottom-0 max-sm:left-0 max-sm:right-0 max-sm:max-w-none max-sm:w-full max-sm:rounded-t-xl max-sm:rounded-b-none max-sm:translate-x-0 max-sm:translate-y-0 max-sm:top-auto"
                onInteractOutside={(e) => {
                    if (isSubmitting) e.preventDefault()
                }}
                onEscapeKeyDown={(e) => {
                    if (isSubmitting) e.preventDefault()
                }}
            >
                {/* Mobile handle */}
                <div className="sm:hidden flex justify-center pt-3 pb-1">
                    <div
                        className="w-10 h-1 bg-[#334155] rounded-full"
                        aria-hidden="true"
                    />
                </div>

                <div className="max-sm:max-h-[85vh] max-sm:overflow-y-auto">
                    {/* Close button */}
                    <button
                        onClick={() => !isSubmitting && onOpenChange(false)}
                        className="absolute top-4 right-4 text-[#64748b] hover:text-white transition-colors rounded-sm focus-visible:ring-2 focus-visible:ring-[#a855f7]"
                        aria-label="Cerrar dialog"
                        disabled={isSubmitting}
                    >
                        <X className="w-4 h-4" />
                    </button>

                    {/* Header */}
                    <div className="px-6 pt-6 pb-4 border-b border-[#334155]">
                        <DialogTitle className="text-xl font-bold text-white">
                            {getDialogTitle(tarea, esReenvio)}
                        </DialogTitle>
                    </div>

                    {/* Re-send note */}
                    {esReenvio && (
                        <div className="mx-6 mt-4 bg-amber-950/30 border border-amber-800/50 rounded-lg p-3 flex items-start gap-2.5">
                            <Info
                                className="w-4 h-4 text-amber-400 shrink-0 mt-0.5"
                                aria-hidden="true"
                            />
                            <div>
                                <p className="text-sm text-amber-300 font-medium">
                                    Re-envio por rechazo previo
                                </p>
                                <p className="text-xs text-amber-300/70 mt-0.5">
                                    Tu envio anterior fue rechazado. Proporciona una
                                    nueva prueba.
                                </p>
                            </div>
                        </div>
                    )}

                    {/* Instructions */}
                    {tarea && (
                        <div className="px-6 py-4">
                            <p className="text-xs font-semibold text-[#94a3b8] uppercase tracking-wide mb-2">
                                Instrucciones
                            </p>
                            <p className="text-sm text-[#cbd5e1] leading-relaxed">
                                {tarea.descripcion ?? "Sin instrucciones disponibles"}
                            </p>
                            {tarea.instruccionesUrl && (
                                <a
                                    href={tarea.instruccionesUrl}
                                    target="_blank"
                                    rel="noopener noreferrer"
                                    className="inline-flex items-center gap-1 text-xs text-[#a855f7] hover:text-purple-400 mt-2 transition-colors"
                                >
                                    <ExternalLink className="w-3 h-3" aria-hidden="true" />
                                    Ver instrucciones completas
                                </a>
                            )}
                        </div>
                    )}

                    <Separator className="bg-[#334155] mx-6" />

                    {/* Form */}
                    <form
                        id="form-completar-tarea"
                        onSubmit={handleSubmit(handleFormSubmit)}
                        className="px-6 py-4 flex flex-col gap-5"
                    >
                        {/* URL field */}
                        <div className="flex flex-col gap-1.5">
                            <Label
                                htmlFor="urlPruebaCompletado"
                                className="text-sm font-medium text-[#cbd5e1]"
                            >
                                URL de prueba
                                <span className="text-red-400 ml-1" aria-hidden="true">
                                    *
                                </span>
                                <span className="sr-only">(requerido)</span>
                            </Label>
                            <Input
                                id="urlPruebaCompletado"
                                type="url"
                                placeholder="https://instagram.com/stories/..."
                                className="bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] h-10 focus:border-[#a855f7] focus-visible:ring-1 focus-visible:ring-[#a855f7]/30"
                                aria-required="true"
                                aria-invalid={!!errors.urlPruebaCompletado}
                                aria-describedby={
                                    errors.urlPruebaCompletado
                                        ? "url-prueba-error"
                                        : undefined
                                }
                                disabled={isSubmitting}
                                {...register("urlPruebaCompletado")}
                            />
                            {errors.urlPruebaCompletado && (
                                <p
                                    id="url-prueba-error"
                                    className="text-xs text-red-400 mt-1"
                                    role="alert"
                                >
                                    {errors.urlPruebaCompletado.message}
                                </p>
                            )}
                        </div>

                        {/* Comment field */}
                        <div className="flex flex-col gap-1.5">
                            <Label
                                htmlFor="comentarioPromotor"
                                className="text-sm font-medium text-[#cbd5e1]"
                            >
                                Comentario{" "}
                                <span className="text-[#64748b] font-normal">
                                    (opcional)
                                </span>
                            </Label>
                            <Textarea
                                id="comentarioPromotor"
                                placeholder="Describe brevemente la accion realizada..."
                                className="bg-[#0f0f1f] border-[#334155] text-white placeholder:text-[#64748b] min-h-[80px] resize-none focus:border-[#a855f7] focus-visible:ring-1 focus-visible:ring-[#a855f7]/30"
                                maxLength={VALIDATION.TAREA_COMENTARIO_PROMOTOR_MAX}
                                aria-describedby="comentario-counter"
                                disabled={isSubmitting}
                                {...register("comentarioPromotor")}
                            />
                            <div className="flex justify-between items-center mt-1">
                                {errors.comentarioPromotor && (
                                    <p
                                        id="comentario-error"
                                        className="text-xs text-red-400"
                                        role="alert"
                                    >
                                        {errors.comentarioPromotor.message}
                                    </p>
                                )}
                                <p
                                    id="comentario-counter"
                                    className="text-right text-xs text-[#64748b] ml-auto"
                                    aria-live="polite"
                                    aria-atomic="true"
                                >
                                    {comentarioValue?.length ?? 0} /{" "}
                                    {VALIDATION.TAREA_COMENTARIO_PROMOTOR_MAX} caracteres
                                </p>
                            </div>
                        </div>
                    </form>

                    {/* Footer */}
                    <div className="px-6 pb-6 pt-4 border-t border-[#334155] flex items-center justify-end gap-3">
                        <Button
                            type="button"
                            variant="ghost"
                            className="text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-10 px-5"
                            onClick={() => onOpenChange(false)}
                            disabled={isSubmitting}
                        >
                            Cancelar
                        </Button>
                        <Button
                            type="submit"
                            form="form-completar-tarea"
                            className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-10 px-6"
                            disabled={isSubmitting}
                            aria-busy={isSubmitting || undefined}
                        >
                            {isSubmitting ? (
                                <>
                                    <Loader2
                                        className="w-4 h-4 animate-spin mr-2"
                                        aria-label="Cargando"
                                    />
                                    Enviando...
                                </>
                            ) : (
                                "Enviar prueba"
                            )}
                        </Button>
                    </div>
                </div>
            </DialogContent>
        </Dialog>
    )
}
