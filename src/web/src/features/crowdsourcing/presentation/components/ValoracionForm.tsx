import { FC, useState } from "react"
import { Textarea } from "@/components/ui/textarea"
import { Label } from "@/components/ui/label"
import { Button } from "@/components/ui/button"
import { Star, Loader2 } from "lucide-react"
import { cn } from "@/lib/utils"
import { StarRating } from "./StarRating"
import { useCreateValoracion } from "../../application/hooks/useCreateValoracion"
import type { ValoracionCreatedResult } from "../../domain"

interface ValoracionFormProps {
    acuerdoId: string
    userIdValorado: string
    onSuccess: (valoracion: ValoracionCreatedResult) => void
}

export const ValoracionForm: FC<ValoracionFormProps> = ({
    acuerdoId,
    userIdValorado,
    onSuccess,
}) => {
    const [puntuacion, setPuntuacion] = useState(0)
    const [comentario, setComentario] = useState("")
    const [showPuntuacionError, setShowPuntuacionError] = useState(false)

    const { mutate, isPending } = useCreateValoracion(
        acuerdoId,
        userIdValorado,
        onSuccess
    )

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault()

        if (puntuacion === 0) {
            setShowPuntuacionError(true)
            return
        }

        setShowPuntuacionError(false)
        mutate({
            puntuacion,
            comentario: comentario.trim() || undefined,
        })
    }

    const handlePuntuacionChange = (value: number) => {
        setPuntuacion(value)
        if (value > 0) setShowPuntuacionError(false)
    }

    return (
        <div
            className={cn(
                "bg-[#0f1729] border border-[#334155] rounded-xl p-6 mt-6",
                isPending && "opacity-70"
            )}
        >
            <div className="flex items-center justify-between mb-2">
                <h3 className="text-lg font-semibold text-white">
                    Deja tu valoracion
                </h3>
                <Star
                    className="w-5 h-5 text-[#f59e0b]"
                    fill="currentColor"
                    aria-hidden="true"
                />
            </div>

            <p className="text-sm text-[#94a3b8] mb-5">
                Tu comentario ayuda a otros artistas/profesionales
            </p>

            <form onSubmit={handleSubmit} aria-busy={isPending}>
                <div className="mb-4">
                    <Label
                        className="text-sm font-medium text-[#cbd5e1] mb-2 block"
                        id="puntuacion-label"
                    >
                        Puntuacion
                        <span className="text-red-400 ml-1" aria-hidden="true">
                            *
                        </span>
                    </Label>
                    <StarRating
                        value={puntuacion}
                        onChange={handlePuntuacionChange}
                        size="lg"
                        disabled={isPending}
                    />
                    {puntuacion > 0 && (
                        <p className="text-xs text-[#64748b] mt-1 ml-1">
                            {puntuacion} de 5 estrellas
                        </p>
                    )}
                    {showPuntuacionError && (
                        <p
                            role="alert"
                            aria-live="polite"
                            className="text-sm text-[#ef4444] mt-1"
                        >
                            Selecciona una puntuacion
                        </p>
                    )}
                </div>

                <div className="mt-4">
                    <Label
                        htmlFor="comentario"
                        className="text-sm font-medium text-[#cbd5e1] mb-1.5 block"
                    >
                        Comentario
                        <span className="text-[#64748b] font-normal ml-1">
                            (opcional)
                        </span>
                    </Label>
                    <Textarea
                        id="comentario"
                        placeholder="Comparte tu experiencia con este profesional..."
                        disabled={isPending}
                        aria-invalid={comentario.length > 1000}
                        aria-describedby="comentario-counter"
                        className={cn(
                            "bg-[#16213e] text-white placeholder:text-[#64748b] resize-none min-h-[100px] text-sm leading-relaxed",
                            comentario.length > 1000
                                ? "border-[#ef4444] focus:border-[#ef4444]"
                                : "border-[#334155] focus:border-[#a855f7]"
                        )}
                        value={comentario}
                        onChange={(e) => setComentario(e.target.value)}
                    />
                    <p
                        id="comentario-counter"
                        className={cn(
                            "text-xs text-right mt-1",
                            comentario.length > 1000
                                ? "text-[#ef4444]"
                                : comentario.length > 900
                                  ? "text-[#f59e0b]"
                                  : "text-[#64748b]"
                        )}
                        aria-live="polite"
                        aria-atomic="true"
                    >
                        {comentario.length} / 1000 caracteres
                    </p>
                </div>

                <div className="flex justify-end mt-5">
                    <Button
                        type="submit"
                        disabled={
                            isPending ||
                            puntuacion === 0 ||
                            comentario.length > 1000
                        }
                        className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 h-10 px-6 text-sm font-medium transition-all disabled:opacity-50 disabled:cursor-not-allowed focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729] w-full md:w-auto"
                    >
                        {isPending ? (
                            <>
                                <Loader2
                                    className="animate-spin w-4 h-4 mr-2"
                                    aria-hidden="true"
                                />
                                Enviando...
                            </>
                        ) : (
                            "Enviar valoracion"
                        )}
                    </Button>
                </div>
            </form>
        </div>
    )
}
