import { FC } from "react"
import { Link } from "react-router-dom"
import { Button } from "@/components/ui/button"
import { MessageCircle } from "lucide-react"

interface ConversacionLinkProps {
    conversacionId: string | undefined
    className?: string
}

export const ConversacionLink: FC<ConversacionLinkProps> = ({
    conversacionId,
    className = "",
}) => {
    if (conversacionId) {
        return (
            <Link to={`/crowdsourcing/mensajes/${conversacionId}`}>
                <Button
                    variant="outline"
                    className={`w-full gap-2 border-slate-700 text-slate-300 hover:bg-slate-800 ${className}`}
                >
                    <MessageCircle className="h-4 w-4" />
                    Ir al chat
                </Button>
            </Link>
        )
    }

    return (
        <Button
            variant="outline"
            className={`w-full gap-2 border-slate-700 text-slate-300 hover:bg-slate-800 ${className}`}
            disabled
            title="Conversacion no disponible aun"
        >
            <MessageCircle className="h-4 w-4" />
            Ir al chat
        </Button>
    )
}
