import { FC } from "react"
import { Avatar, AvatarImage, AvatarFallback } from "@/components/ui/avatar"
import { StarDisplay } from "./StarDisplay"
import type { ValoracionListItem as ValoracionListItemType } from "../../domain"

interface ValoracionListItemProps {
    valoracion: ValoracionListItemType
}

function getInitials(nombre: string): string {
    return nombre
        .split(" ")
        .slice(0, 2)
        .map((n) => n[0]?.toUpperCase() ?? "")
        .join("")
}

function formatFecha(iso: string): string {
    return new Date(iso).toLocaleDateString("es-ES", {
        day: "numeric",
        month: "short",
        year: "numeric",
    })
}

export const ValoracionListItem: FC<ValoracionListItemProps> = ({
    valoracion,
}) => {
    return (
        <div className="bg-[#0f1729] border border-[#334155] rounded-xl p-4 hover:border-[#475569] transition-colors duration-150">
            <div className="flex items-start gap-2 mb-2 flex-col md:flex-row md:justify-between">
                <div className="flex items-center gap-2 flex-wrap">
                    <StarDisplay value={valoracion.puntuacion} size="sm" />
                    <Avatar className="w-6 h-6 flex-shrink-0">
                        <AvatarImage
                            src={valoracion.autorImagenUrl ?? undefined}
                            alt={valoracion.autorNombre}
                        />
                        <AvatarFallback className="bg-[#334155] text-white text-[10px] font-semibold">
                            {getInitials(valoracion.autorNombre)}
                        </AvatarFallback>
                    </Avatar>
                    <span className="text-sm font-medium text-white">
                        {valoracion.autorNombre}
                    </span>
                </div>
                <span className="text-xs text-[#64748b] md:flex-shrink-0 mt-1 md:mt-0">
                    {formatFecha(valoracion.fechaCreacion)}
                </span>
            </div>

            <p className="text-xs text-[#a855f7] font-medium mb-2">
                {valoracion.acuerdoTituloInterno}
            </p>

            {valoracion.comentario && (
                <p className="text-sm text-[#e2e8f0] leading-relaxed italic">
                    {valoracion.comentario}
                </p>
            )}
        </div>
    )
}
