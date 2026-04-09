import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { QUERY_KEYS } from "@shared/constants"
import { mensajeApi } from "../../infrastructure"
import type { CreateMensajeRequest } from "../../domain"

export function useEnviarMensaje(conversacionId: string) {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: CreateMensajeRequest) =>
            mensajeApi.create(conversacionId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.conversaciones.mensajes(
                    conversacionId,
                    1
                ),
            })
            queryClient.invalidateQueries({
                queryKey: ["crowdsourcing", "conversaciones"],
            })
        },
        onError: () => {
            toast.error("No se pudo enviar el mensaje. Intenta de nuevo.")
        },
    })
}
