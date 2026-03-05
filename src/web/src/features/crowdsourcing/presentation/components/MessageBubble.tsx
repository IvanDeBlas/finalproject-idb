import { FC } from "react"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Paperclip, AlertCircle } from "lucide-react"
import type { Mensaje } from "../../domain"
import { formatMessageTime, getInitials } from "./mensajeria.utils"

interface MessageBubbleProps {
    mensaje: Mensaje
    isOptimistic?: boolean
}

export const MessageBubble: FC<MessageBubbleProps> = ({
    mensaje,
    isOptimistic = false,
}) => {
    const isOwn = mensaje.esPropio

    return (
        <div
            className={`flex items-start gap-2 mb-3 ${
                isOwn ? "flex-row-reverse" : ""
            }`}
        >
            <Avatar className="w-8 h-8 flex-shrink-0 mt-0.5">
                <AvatarFallback className="bg-[#334155] text-white text-xs font-semibold">
                    {getInitials(mensaje.remitenteNombre)}
                </AvatarFallback>
            </Avatar>
            <div
                className={`flex flex-col max-w-[70%] ${
                    isOwn ? "items-end" : "items-start"
                }`}
            >
                {!isOwn && (
                    <span className="text-xs text-[#64748b] mb-1 ml-1">
                        {mensaje.remitenteNombre}
                    </span>
                )}
                <div
                    className={
                        isOwn
                            ? "bg-gradient-to-br from-[#ec4899] to-[#a855f7] text-white text-sm px-4 py-3 rounded-[1rem_1rem_0.25rem_1rem] leading-relaxed"
                            : "bg-[#16213e] border border-[#334155] text-[#e2e8f0] text-sm px-4 py-3 rounded-[1rem_1rem_1rem_0.25rem] leading-relaxed"
                    }
                >
                    <p className="break-words whitespace-pre-wrap">
                        {mensaje.contenido}
                    </p>
                    {mensaje.urlAdjunto && (
                        <a
                            href={mensaje.urlAdjunto}
                            target="_blank"
                            rel="noopener noreferrer"
                            className={`block mt-2 text-xs underline opacity-80 hover:opacity-100 truncate max-w-[200px] ${
                                isOwn
                                    ? "text-white/80 hover:text-white"
                                    : "text-[#a855f7] hover:text-[#c084fc]"
                            }`}
                        >
                            <Paperclip className="w-3 h-3 inline mr-1" />
                            {mensaje.urlAdjunto}
                        </a>
                    )}
                </div>
                <span
                    className={`text-xs text-[#64748b] mt-1 ${
                        isOwn ? "mr-1" : "ml-1"
                    }`}
                >
                    {isOptimistic ? (
                        <span className="text-[#64748b]/60">Enviando...</span>
                    ) : (
                        formatMessageTime(mensaje.fechaCreacion)
                    )}
                    {isOptimistic && (
                        <AlertCircle className="w-3 h-3 text-red-400 inline ml-1 hidden" />
                    )}
                </span>
            </div>
        </div>
    )
}
