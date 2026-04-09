import { FC, useState, useEffect } from "react"
import { useNavigate } from "react-router-dom"
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Skeleton } from "@/components/ui/skeleton"
import { Alert, AlertTitle, AlertDescription } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { MessageSquare, AlertCircle, Loader2 } from "lucide-react"
import { FILTRO_CONVERSACION } from "@shared/constants"
import { useConversaciones } from "../../application/hooks/useConversaciones"
import type { FiltroConversacion, ConversacionListItem } from "../../domain"
import { ConversacionRow } from "./ConversacionRow"

interface ConversacionListProps {
    filtro: FiltroConversacion
    onFiltroChange: (filtro: FiltroConversacion) => void
}

export const ConversacionList: FC<ConversacionListProps> = ({
    filtro,
    onFiltroChange,
}) => {
    const navigate = useNavigate()
    const [page, setPage] = useState(1)
    const [conversaciones, setConversaciones] = useState<
        ConversacionListItem[]
    >([])

    const { data, isLoading, isError, refetch, isFetching } =
        useConversaciones(filtro, page)

    useEffect(() => {
        setPage(1)
        setConversaciones([])
    }, [filtro])

    useEffect(() => {
        if (data?.items) {
            if (page === 1) {
                setConversaciones(data.items)
            } else {
                setConversaciones((prev) => {
                    const existingIds = new Set(prev.map((c) => c.id))
                    const newItems = data.items.filter(
                        (c) => !existingIds.has(c.id)
                    )
                    return [...prev, ...newItems]
                })
            }
        }
    }, [data, page])

    const hasMore =
        data && conversaciones.length < data.totalCount

    const handleLoadMore = () => {
        setPage((prev) => prev + 1)
    }

    const handleTabChange = (value: string) => {
        onFiltroChange(value as FiltroConversacion)
    }

    return (
        <div>
            <Tabs
                value={filtro}
                onValueChange={handleTabChange}
                className="mb-4"
            >
                <TabsList className="bg-[#1e2a42] border border-[#334155]">
                    <TabsTrigger
                        value={FILTRO_CONVERSACION.TODAS}
                        className="data-[state=active]:bg-[#a855f7] data-[state=active]:text-white text-[#94a3b8] text-sm"
                    >
                        Todas
                    </TabsTrigger>
                    <TabsTrigger
                        value={FILTRO_CONVERSACION.NECESIDADES}
                        className="data-[state=active]:bg-[#a855f7] data-[state=active]:text-white text-[#94a3b8] text-sm"
                    >
                        Necesidades
                    </TabsTrigger>
                    <TabsTrigger
                        value={FILTRO_CONVERSACION.ACUERDOS}
                        className="data-[state=active]:bg-[#a855f7] data-[state=active]:text-white text-[#94a3b8] text-sm"
                    >
                        Acuerdos
                    </TabsTrigger>
                </TabsList>
            </Tabs>

            {isLoading && (
                <div className="space-y-3">
                    {Array.from({ length: 5 }).map((_, i) => (
                        <Skeleton
                            key={i}
                            className="h-20 rounded-xl w-full bg-[#1e2a42]"
                            data-testid="conversacion-skeleton"
                        />
                    ))}
                </div>
            )}

            {isError && (
                <Alert
                    variant="destructive"
                    className="border-red-500/50 bg-red-500/10"
                >
                    <AlertCircle className="h-4 w-4 text-red-400" />
                    <AlertTitle className="text-red-400">
                        Error al cargar conversaciones
                    </AlertTitle>
                    <AlertDescription className="text-red-300">
                        No se pudieron cargar las conversaciones. Intenta de
                        nuevo.
                    </AlertDescription>
                    <Button
                        variant="outline"
                        size="sm"
                        className="mt-3 border-red-500/50 text-red-400 hover:bg-red-500/10"
                        onClick={() => refetch()}
                    >
                        Reintentar
                    </Button>
                </Alert>
            )}

            {!isLoading && !isError && conversaciones.length === 0 && (
                <div className="text-center py-16">
                    <MessageSquare className="w-12 h-12 mx-auto mb-3 opacity-30 text-[#64748b]" />
                    <p className="text-base text-[#64748b]">
                        No tienes conversaciones activas
                    </p>
                </div>
            )}

            {!isLoading && !isError && conversaciones.length > 0 && (
                <div className="space-y-3">
                    {conversaciones.map((conversacion) => (
                        <ConversacionRow
                            key={conversacion.id}
                            conversacion={conversacion}
                            onClick={() =>
                                navigate(
                                    `/crowdsourcing/mensajes/${conversacion.id}`
                                )
                            }
                        />
                    ))}

                    {hasMore && (
                        <Button
                            variant="outline"
                            className="w-full mt-4 border-[#334155] text-[#94a3b8] hover:bg-[#1e2a42] hover:text-white"
                            onClick={handleLoadMore}
                            disabled={isFetching}
                        >
                            {isFetching && page > 1 ? (
                                <Loader2 className="animate-spin w-5 h-5 text-[#64748b]" />
                            ) : (
                                "Cargar mas conversaciones"
                            )}
                        </Button>
                    )}
                </div>
            )}
        </div>
    )
}
