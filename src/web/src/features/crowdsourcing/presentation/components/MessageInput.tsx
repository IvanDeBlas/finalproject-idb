import { FC, useState, useRef, KeyboardEvent } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Textarea } from "@/components/ui/textarea"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Send, Paperclip, Loader2 } from "lucide-react"
import {
    createMensajeSchema,
    type CreateMensajeFormData,
} from "@shared/schemas/crowdsourcing.schema"
import { useEnviarMensaje } from "../../application/hooks/useEnviarMensaje"
import type { Mensaje } from "../../domain"

interface MessageInputProps {
    conversacionId: string
    onMensajeEnviado: (mensaje: Mensaje) => void
}

export const MessageInput: FC<MessageInputProps> = ({
    conversacionId,
    onMensajeEnviado,
}) => {
    const [showUrlInput, setShowUrlInput] = useState(false)
    const textareaRef = useRef<HTMLTextAreaElement>(null)
    const enviarMensaje = useEnviarMensaje(conversacionId)

    const {
        register,
        handleSubmit,
        reset,
        watch,
        formState: { errors },
    } = useForm<CreateMensajeFormData>({
        resolver: zodResolver(createMensajeSchema),
        defaultValues: { contenido: "", urlAdjunto: "" },
    })

    const contenido = watch("contenido") ?? ""

    const onSubmit = (data: CreateMensajeFormData) => {
        const payload = {
            contenido: data.contenido,
            urlAdjunto: data.urlAdjunto || undefined,
        }

        enviarMensaje.mutate(payload, {
            onSuccess: (mensaje) => {
                onMensajeEnviado(mensaje)
                reset()
                setShowUrlInput(false)
                textareaRef.current?.focus()
            },
        })
    }

    const handleKeyDown = (e: KeyboardEvent<HTMLTextAreaElement>) => {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault()
            if (contenido.trim()) {
                handleSubmit(onSubmit)()
            }
        }
    }

    const autoResize = (e: React.FormEvent<HTMLTextAreaElement>) => {
        const target = e.currentTarget
        target.style.height = "auto"
        target.style.height = `${Math.min(target.scrollHeight, 160)}px`
    }

    const { ref: formRef, ...contenidoReg } = register("contenido")

    return (
        <div className="bg-[#0f1729] border border-[#334155] rounded-xl p-4 mt-4">
            <form onSubmit={handleSubmit(onSubmit)}>
                <div className="flex items-end gap-3">
                    <Textarea
                        {...contenidoReg}
                        ref={(e) => {
                            formRef(e)
                            ;(textareaRef as React.MutableRefObject<HTMLTextAreaElement | null>).current = e
                        }}
                        id="mensaje-input"
                        placeholder="Escribe un mensaje..."
                        rows={1}
                        className="bg-[#16213e] border-[#334155] text-white placeholder:text-[#64748b] resize-none min-h-[44px] max-h-[160px] focus:border-[#a855f7] focus-visible:ring-[#a855f7] flex-1 text-sm leading-relaxed"
                        aria-label="Escribe un mensaje"
                        onKeyDown={handleKeyDown}
                        onInput={autoResize}
                        disabled={enviarMensaje.isPending}
                    />
                    <Button
                        type="submit"
                        aria-label="Enviar mensaje"
                        disabled={
                            !contenido.trim() || enviarMensaje.isPending
                        }
                        className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 h-11 px-4 flex-shrink-0 transition-all duration-200 disabled:opacity-40 disabled:cursor-not-allowed"
                    >
                        {enviarMensaje.isPending ? (
                            <Loader2 className="animate-spin w-4 h-4" />
                        ) : (
                            <Send className="w-4 h-4" />
                        )}
                    </Button>
                </div>

                {showUrlInput && (
                    <div className="mt-2">
                        <Label htmlFor="url-adjunto" className="sr-only">
                            URL adjunta
                        </Label>
                        <Input
                            {...register("urlAdjunto")}
                            id="url-adjunto"
                            type="url"
                            placeholder="https://..."
                            className="bg-[#16213e] border-[#334155] text-white placeholder:text-[#64748b] text-sm h-9 focus:border-[#a855f7] focus-visible:ring-[#a855f7]"
                        />
                        {errors.urlAdjunto && (
                            <p
                                role="alert"
                                className="text-xs text-red-400 mt-1"
                            >
                                {errors.urlAdjunto.message}
                            </p>
                        )}
                    </div>
                )}

                <Button
                    type="button"
                    variant="ghost"
                    className="text-xs text-[#64748b] hover:text-[#94a3b8] h-8 px-2 mt-2 flex items-center gap-1"
                    onClick={() => setShowUrlInput(!showUrlInput)}
                >
                    <Paperclip className="w-3.5 h-3.5" />
                    {showUrlInput ? "Quitar adjunto" : "Adjuntar URL"}
                </Button>

                {contenido.length > 4000 && (
                    <span className="text-xs text-[#64748b] text-right block mt-1">
                        {contenido.length}/5000
                    </span>
                )}

                {errors.contenido && (
                    <p role="alert" className="text-xs text-red-400 mt-1">
                        {errors.contenido.message}
                    </p>
                )}
            </form>
        </div>
    )
}
