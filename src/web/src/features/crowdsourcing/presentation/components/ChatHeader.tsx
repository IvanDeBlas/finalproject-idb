import { FC } from "react"
import { Avatar, AvatarImage, AvatarFallback } from "@/components/ui/avatar"
import type { ContextoConversacion } from "../../domain"
import { getInitials } from "./mensajeria.utils"

interface ChatHeaderProps {
    nombreOtraParte: string
    imagenOtraParte?: string
    asunto: string
    contextoTipo: ContextoConversacion
    contextoTitulo: string
}

export const ChatHeader: FC<ChatHeaderProps> = ({
    nombreOtraParte,
    imagenOtraParte,
    asunto,
    contextoTipo,
    contextoTitulo,
}) => {
    return (
        <div className="bg-[#0f1729] border border-[#334155] rounded-xl p-4 mb-4 flex items-start gap-3">
            <Avatar className="w-12 h-12 flex-shrink-0">
                <AvatarImage src={imagenOtraParte} alt={nombreOtraParte} />
                <AvatarFallback className="bg-[#334155] text-white text-sm font-semibold">
                    {getInitials(nombreOtraParte)}
                </AvatarFallback>
            </Avatar>
            <div className="flex-1 min-w-0">
                <h2 className="text-lg font-semibold text-white truncate">
                    {nombreOtraParte}
                </h2>
                <p className="text-sm text-[#94a3b8] italic truncate mt-0.5">
                    &ldquo;{asunto}&rdquo;
                </p>
                <div className="flex items-center gap-1 mt-1">
                    <span className="text-xs text-[#64748b] uppercase tracking-wider">
                        {contextoTipo === "necesidad"
                            ? "Necesidad:"
                            : "Acuerdo:"}
                    </span>
                    <span className="text-sm text-[#a855f7] font-medium">
                        {contextoTitulo}
                    </span>
                </div>
            </div>
        </div>
    )
}
