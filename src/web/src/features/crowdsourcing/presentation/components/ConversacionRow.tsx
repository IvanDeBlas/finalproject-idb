import { FC } from "react"
import { Avatar, AvatarImage, AvatarFallback } from "@/components/ui/avatar"
import { Badge } from "@/components/ui/badge"
import type { ConversacionListItem } from "../../domain"
import { formatTimestamp, getInitials } from "./mensajeria.utils"

interface ConversacionRowProps {
    conversacion: ConversacionListItem
    onClick: () => void
}

export const ConversacionRow: FC<ConversacionRowProps> = ({
    conversacion,
    onClick,
}) => {
    const hasUnread = conversacion.mensajesNoLeidos > 0
    const timestampRelativo = formatTimestamp(conversacion.fechaUltimoMensaje)

    return (
        <button
            className={`w-full ${
                hasUnread ? "bg-[#1e2a42]" : "bg-[#0f1729]"
            } border border-[#334155] rounded-xl p-4 flex items-start gap-3 ${
                hasUnread ? "hover:bg-[#243347]" : "hover:bg-[#1e2a42]"
            } transition-colors duration-150 cursor-pointer text-left focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7] focus-visible:ring-offset-2 focus-visible:ring-offset-[#0f1729]`}
            onClick={onClick}
        >
            <Avatar className="w-12 h-12 flex-shrink-0">
                <AvatarImage
                    src={conversacion.imagenOtraParte}
                    alt={conversacion.nombreOtraParte}
                />
                <AvatarFallback className="bg-[#334155] text-white text-sm font-semibold">
                    {getInitials(conversacion.nombreOtraParte)}
                </AvatarFallback>
            </Avatar>
            <div className="flex-1 min-w-0">
                <div className="flex items-center justify-between gap-2 mb-0.5">
                    <span
                        className={`text-sm ${
                            hasUnread
                                ? "font-semibold text-white"
                                : "font-medium text-[#e2e8f0]"
                        } truncate`}
                    >
                        {conversacion.nombreOtraParte}
                    </span>
                    <div className="flex items-center gap-2 flex-shrink-0">
                        {hasUnread && (
                            <Badge
                                className="bg-[#ec4899] text-white rounded-full min-w-[1.25rem] h-5 px-1.5 text-xs font-bold hover:bg-[#ec4899]"
                                aria-label={`${conversacion.mensajesNoLeidos > 99 ? "99+" : conversacion.mensajesNoLeidos} mensajes no leidos`}
                            >
                                {conversacion.mensajesNoLeidos > 99
                                    ? "99+"
                                    : conversacion.mensajesNoLeidos}
                            </Badge>
                        )}
                        {timestampRelativo && (
                            <span className="text-xs text-[#64748b]">
                                {timestampRelativo}
                            </span>
                        )}
                    </div>
                </div>
                <p className="text-xs text-[#a855f7] font-medium truncate mb-0.5">
                    {conversacion.contextoTitulo}
                </p>
                {conversacion.ultimoMensaje && (
                    <p className="text-sm text-[#94a3b8] truncate">
                        {conversacion.ultimoMensaje}
                    </p>
                )}
            </div>
        </button>
    )
}
