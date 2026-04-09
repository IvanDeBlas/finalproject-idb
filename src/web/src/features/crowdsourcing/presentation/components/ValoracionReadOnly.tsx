import { FC } from "react"
import { CheckCircle2 } from "lucide-react"
import { cn } from "@/lib/utils"
import { StarDisplay } from "./StarDisplay"
import type { ValoracionCreatedResult } from "../../domain"

interface ValoracionReadOnlyProps {
    valoracion: ValoracionCreatedResult
    className?: string
}

function formatFecha(iso: string): string {
    return new Date(iso).toLocaleDateString("es-ES", {
        day: "numeric",
        month: "short",
        year: "numeric",
    })
}

export const ValoracionReadOnly: FC<ValoracionReadOnlyProps> = ({
    valoracion,
    className,
}) => {
    return (
        <div
            className={cn(
                "bg-[#0f1729] border border-[#10b981]/40 rounded-xl p-6 mt-6",
                "animate-in fade-in-0 slide-in-from-bottom-2 duration-200",
                className
            )}
        >
            <div className="flex items-center justify-between mb-4">
                <h3 className="text-lg font-semibold text-white">
                    Tu valoracion
                </h3>
                <CheckCircle2
                    className="w-5 h-5 text-[#10b981]"
                    aria-hidden="true"
                />
            </div>

            <StarDisplay value={valoracion.puntuacion} size="lg" showNumeric />

            {valoracion.comentario && (
                <p className="text-sm text-[#e2e8f0] leading-relaxed mt-3 italic border-l-2 border-[#334155] pl-3">
                    &ldquo;{valoracion.comentario}&rdquo;
                </p>
            )}

            <p className="text-xs text-[#64748b] mt-3">
                Enviada el {formatFecha(valoracion.fechaCreacion)}
            </p>
        </div>
    )
}
