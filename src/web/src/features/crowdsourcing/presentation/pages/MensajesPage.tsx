import { useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import { Badge } from "@/components/ui/badge"
import { useAuthStore } from "@/store/auth-store"
import { FILTRO_CONVERSACION } from "@shared/constants"
import { useConversaciones } from "../../application/hooks/useConversaciones"
import { ConversacionList } from "../components/ConversacionList"
import type { FiltroConversacion } from "../../domain"

export default function MensajesPage() {
    const { isAuthenticated } = useAuthStore()
    const navigate = useNavigate()
    const [filtroActivo, setFiltroActivo] = useState<FiltroConversacion>(
        FILTRO_CONVERSACION.TODAS as FiltroConversacion
    )

    useEffect(() => {
        if (!isAuthenticated) {
            navigate("/auth/login", { replace: true })
        }
    }, [isAuthenticated, navigate])

    const { data } = useConversaciones(filtroActivo, 1)
    const totalNoLeidos = data?.totalNoLeidos ?? 0

    if (!isAuthenticated) return null

    return (
        <div className="max-w-3xl mx-auto px-3 md:px-4 py-4 md:py-8">
            <div className="flex items-center justify-between mb-6">
                <h1 className="text-2xl font-bold text-white">Mensajes</h1>
                {totalNoLeidos > 0 && (
                    <Badge className="bg-[#ec4899] text-white rounded-full px-2 py-0.5 text-xs font-semibold hover:bg-[#ec4899]">
                        {totalNoLeidos > 99 ? "99+" : totalNoLeidos}
                    </Badge>
                )}
            </div>

            <ConversacionList
                filtro={filtroActivo}
                onFiltroChange={setFiltroActivo}
            />
        </div>
    )
}
