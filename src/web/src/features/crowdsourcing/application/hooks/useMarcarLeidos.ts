import { useMutation, useQueryClient } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { mensajeApi } from "../../infrastructure"

export function useMarcarLeidos(conversacionId: string) {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: () => mensajeApi.marcarLeidos(conversacionId),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.conversaciones.noLeidos,
            })
            queryClient.invalidateQueries({
                queryKey: ["crowdsourcing", "conversaciones"],
            })
        },
    })
}
