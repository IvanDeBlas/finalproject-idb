import { FC, useRef, useEffect } from "react"
import { ScrollArea } from "@/components/ui/scroll-area"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { Loader2 } from "lucide-react"
import type { Mensaje } from "../../domain"
import { MessageBubble } from "./MessageBubble"
import { DateSeparator } from "./DateSeparator"
import { isDifferentDay } from "./mensajeria.utils"

interface ChatMessageListProps {
    mensajes: Mensaje[]
    isLoading: boolean
    hasMore: boolean
    isLoadingMore: boolean
    onLoadMore: () => void
}

export const ChatMessageList: FC<ChatMessageListProps> = ({
    mensajes,
    isLoading,
    hasMore,
    isLoadingMore,
    onLoadMore,
}) => {
    const bottomRef = useRef<HTMLDivElement>(null)

    useEffect(() => {
        bottomRef.current?.scrollIntoView({ behavior: "smooth" })
    }, [mensajes.length])

    if (isLoading) {
        return (
            <div className="h-[calc(100vh-320px)] md:h-[calc(100vh-280px)] min-h-[300px] bg-[#0f1729] border border-[#334155] rounded-xl p-4">
                <div className="space-y-4">
                    {Array.from({ length: 5 }).map((_, i) => (
                        <div
                            key={i}
                            className={`flex items-start gap-2 mb-3 ${
                                i % 2 === 0 ? "" : "flex-row-reverse"
                            }`}
                        >
                            <Skeleton className="w-8 h-8 rounded-full bg-[#1e2a42]" />
                            <Skeleton
                                className={`h-16 ${
                                    i % 2 === 0 ? "w-48" : "w-40"
                                } ${
                                    i % 2 === 0
                                        ? "rounded-[1rem_1rem_1rem_0.25rem]"
                                        : "rounded-[1rem_1rem_0.25rem_1rem]"
                                } bg-[#1e2a42]`}
                            />
                        </div>
                    ))}
                </div>
            </div>
        )
    }

    return (
        <ScrollArea className="h-[calc(100vh-320px)] md:h-[calc(100vh-280px)] min-h-[300px] bg-[#0f1729] border border-[#334155] rounded-xl p-4">
            <div aria-live="polite" aria-busy={isLoading}>
                {hasMore && (
                    <div className="flex justify-center mb-4">
                        {isLoadingMore ? (
                            <Loader2 className="animate-spin w-4 h-4 text-[#64748b]" />
                        ) : (
                            <Button
                                variant="ghost"
                                className="text-xs text-[#64748b] hover:text-[#94a3b8]"
                                onClick={onLoadMore}
                            >
                                Cargar mensajes anteriores
                            </Button>
                        )}
                    </div>
                )}

                {mensajes.length === 0 && (
                    <p className="text-center text-[#64748b] py-8">
                        Aun no hay mensajes. Escribe el primero.
                    </p>
                )}

                {mensajes.map((mensaje, index) => {
                    const showDateSeparator =
                        index === 0 ||
                        isDifferentDay(
                            mensajes[index - 1].fechaCreacion,
                            mensaje.fechaCreacion
                        )

                    return (
                        <div key={mensaje.id}>
                            {showDateSeparator && (
                                <DateSeparator
                                    fecha={new Date(mensaje.fechaCreacion)}
                                />
                            )}
                            <MessageBubble mensaje={mensaje} />
                        </div>
                    )
                })}

                <div ref={bottomRef} className="h-1" />
            </div>
        </ScrollArea>
    )
}
