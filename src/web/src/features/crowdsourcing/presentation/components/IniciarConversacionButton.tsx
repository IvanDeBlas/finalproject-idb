import { FC, useState } from "react"
import { Link } from "react-router-dom"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { MessageSquare } from "lucide-react"
import { IniciarConversacionDialog } from "./IniciarConversacionDialog"
import type { ContextoConversacion } from "../../domain"

interface IniciarConversacionButtonProps {
    destinatarioId: string
    destinatarioNombre: string
    contextoId: string
    contextoTipo: ContextoConversacion
    contextoTitulo: string
    conversacionExistenteId?: string
    mensajesNoLeidos?: number
}

export const IniciarConversacionButton: FC<IniciarConversacionButtonProps> = ({
    destinatarioId,
    destinatarioNombre,
    contextoId,
    contextoTipo,
    contextoTitulo,
    conversacionExistenteId,
    mensajesNoLeidos,
}) => {
    const [dialogOpen, setDialogOpen] = useState(false)

    if (conversacionExistenteId) {
        return (
            <Link
                to={`/crowdsourcing/mensajes/${conversacionExistenteId}`}
                className="flex items-center gap-2 text-[#a855f7] hover:text-[#c084fc] text-sm transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7]"
            >
                <MessageSquare className="w-4 h-4" />
                Ver conversacion
                {mensajesNoLeidos !== undefined && mensajesNoLeidos > 0 && (
                    <Badge className="bg-[#ec4899] text-white rounded-full min-w-[1.25rem] h-5 px-1.5 text-xs font-bold ml-1 hover:bg-[#ec4899]">
                        {mensajesNoLeidos > 99 ? "99+" : mensajesNoLeidos}
                    </Badge>
                )}
            </Link>
        )
    }

    return (
        <>
            <Button
                variant="ghost"
                onClick={() => setDialogOpen(true)}
                className="flex items-center gap-2 border border-[#334155] text-[#94a3b8] hover:bg-[#1e2a42] hover:text-white bg-transparent h-9 px-3 text-sm rounded-lg transition-colors"
            >
                <MessageSquare className="w-4 h-4" />
                Iniciar conversacion
            </Button>

            <IniciarConversacionDialog
                isOpen={dialogOpen}
                onClose={() => setDialogOpen(false)}
                destinatarioId={destinatarioId}
                destinatarioNombre={destinatarioNombre}
                contextoId={contextoId}
                contextoTipo={contextoTipo}
                contextoTitulo={contextoTitulo}
            />
        </>
    )
}
