import { FC } from "react"
import { Link } from "react-router-dom"
import { MessageSquare } from "lucide-react"
import { cn } from "@/lib/utils"

interface NavbarMensajesIconProps {
    totalNoLeidos: number
}

export const NavbarMensajesIcon: FC<NavbarMensajesIconProps> = ({
    totalNoLeidos,
}) => {
    return (
        <Link
            to="/crowdsourcing/mensajes"
            className="relative flex items-center gap-1.5 text-muted-foreground hover:text-foreground transition-colors text-sm font-medium"
        >
            <MessageSquare className="w-5 h-5" />
            <span
                className={cn(
                    "absolute -top-1.5 -right-1.5 min-w-[1.125rem] h-[1.125rem] rounded-full",
                    "bg-[#ec4899] text-white text-[10px] font-bold",
                    "flex items-center justify-center px-1 leading-none",
                    totalNoLeidos === 0 && "hidden"
                )}
                aria-label={`${totalNoLeidos} mensajes no leidos`}
            >
                {totalNoLeidos > 99 ? "99+" : totalNoLeidos}
            </span>
        </Link>
    )
}
