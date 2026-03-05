import { useState, useEffect, useCallback } from "react"
import { useParams, useNavigate, Link } from "react-router-dom"
import { Alert, AlertTitle, AlertDescription } from "@/components/ui/alert"
import { ChevronLeft, AlertCircle } from "lucide-react"
import { useAuthStore } from "@/store/auth-store"
import { useMensajes } from "../../application/hooks/useMensajes"
import { useMarcarLeidos } from "../../application/hooks/useMarcarLeidos"
import { useConversaciones } from "../../application/hooks/useConversaciones"
import { ChatHeader } from "../components/ChatHeader"
import { ChatMessageList } from "../components/ChatMessageList"
import { MessageInput } from "../components/MessageInput"
import type { Mensaje } from "../../domain"

export default function ConversacionChatPage() {
    const { id } = useParams<{ id: string }>()
    const { isAuthenticated } = useAuthStore()
    const navigate = useNavigate()
    const [page, setPage] = useState(1)
    const [mensajesAcumulados, setMensajesAcumulados] = useState<Mensaje[]>([])

    const conversacionId = id ?? ""

    useEffect(() => {
        if (!isAuthenticated) {
            navigate("/auth/login", { replace: true })
        }
    }, [isAuthenticated, navigate])

    const {
        data: mensajesData,
        isLoading,
        isError,
        error,
    } = useMensajes(conversacionId, page)

    const marcarLeidos = useMarcarLeidos(conversacionId)

    // Get conversation info from the lista query cache
    const { data: conversacionesData } = useConversaciones("todas", 1)
    const conversacionInfo = conversacionesData?.items.find(
        (c) => c.id === conversacionId
    )

    // Mark as read on mount
    useEffect(() => {
        if (conversacionId && isAuthenticated) {
            marcarLeidos.mutate()
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [conversacionId, isAuthenticated])

    // Accumulate messages
    useEffect(() => {
        if (mensajesData?.items) {
            if (page === 1) {
                setMensajesAcumulados(mensajesData.items)
            } else {
                setMensajesAcumulados((prev) => {
                    const existingIds = new Set(prev.map((m) => m.id))
                    const newItems = mensajesData.items.filter(
                        (m) => !existingIds.has(m.id)
                    )
                    return [...newItems, ...prev]
                })
            }
        }
    }, [mensajesData, page])

    const hasMore =
        mensajesData && mensajesAcumulados.length < mensajesData.totalCount

    const handleLoadMore = useCallback(() => {
        setPage((prev) => prev + 1)
    }, [])

    const handleMensajeEnviado = useCallback((mensaje: Mensaje) => {
        setMensajesAcumulados((prev) => {
            if (prev.some((m) => m.id === mensaje.id)) return prev
            return [...prev, mensaje]
        })
    }, [])

    if (!isAuthenticated) return null

    // Error states
    const errorCode = error instanceof Error ? error.message : ""
    if (isError && errorCode === "3002") {
        return (
            <div className="max-w-3xl mx-auto px-3 md:px-4 py-4 md:py-8">
                <Alert
                    variant="destructive"
                    className="border-red-500/50 bg-red-500/10"
                >
                    <AlertCircle className="h-4 w-4 text-red-400" />
                    <AlertTitle className="text-red-400">
                        Acceso denegado
                    </AlertTitle>
                    <AlertDescription className="text-red-300">
                        No tienes acceso a esta conversacion.
                    </AlertDescription>
                </Alert>
                <Link
                    to="/crowdsourcing/mensajes"
                    className="flex items-center gap-2 text-[#94a3b8] hover:text-white text-sm mt-4 transition-colors"
                >
                    <ChevronLeft className="w-4 h-4" />
                    Volver a mensajes
                </Link>
            </div>
        )
    }

    if (isError) {
        return (
            <div className="max-w-3xl mx-auto px-3 md:px-4 py-4 md:py-8">
                <Alert
                    variant="destructive"
                    className="border-red-500/50 bg-red-500/10"
                >
                    <AlertCircle className="h-4 w-4 text-red-400" />
                    <AlertTitle className="text-red-400">Error</AlertTitle>
                    <AlertDescription className="text-red-300">
                        No se pudo cargar la conversacion.
                    </AlertDescription>
                </Alert>
                <Link
                    to="/crowdsourcing/mensajes"
                    className="flex items-center gap-2 text-[#94a3b8] hover:text-white text-sm mt-4 transition-colors"
                >
                    <ChevronLeft className="w-4 h-4" />
                    Volver a mensajes
                </Link>
            </div>
        )
    }

    return (
        <div className="max-w-3xl mx-auto px-3 md:px-4 py-4 md:py-8">
            <Link
                to="/crowdsourcing/mensajes"
                className="flex items-center gap-2 text-[#94a3b8] hover:text-white text-sm mb-4 transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#a855f7]"
            >
                <ChevronLeft className="w-4 h-4" />
                Volver a mensajes
            </Link>

            {conversacionInfo && (
                <ChatHeader
                    nombreOtraParte={conversacionInfo.nombreOtraParte}
                    imagenOtraParte={conversacionInfo.imagenOtraParte}
                    asunto={conversacionInfo.asunto}
                    contextoTipo={conversacionInfo.contextoTipo}
                    contextoTitulo={conversacionInfo.contextoTitulo}
                />
            )}

            <ChatMessageList
                mensajes={mensajesAcumulados}
                isLoading={isLoading}
                hasMore={!!hasMore}
                isLoadingMore={page > 1 && isLoading}
                onLoadMore={handleLoadMore}
            />

            <MessageInput
                conversacionId={conversacionId}
                onMensajeEnviado={handleMensajeEnviado}
            />
        </div>
    )
}
