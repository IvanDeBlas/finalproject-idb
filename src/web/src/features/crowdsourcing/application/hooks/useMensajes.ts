import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS, MENSAJERIA_POLLING_INTERVAL_MS } from "@shared/constants"
import { mensajeApi } from "../../infrastructure"

export function useMensajes(
    conversacionId: string,
    page: number = 1,
    pageSize: number = 50
) {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.conversaciones.mensajes(
            conversacionId,
            page
        ),
        queryFn: () =>
            mensajeApi.getByConversacion(conversacionId, { page, pageSize }),
        staleTime: 0,
        refetchInterval: MENSAJERIA_POLLING_INTERVAL_MS,
        refetchIntervalInBackground: false,
        enabled: !!conversacionId,
        retry: false,
    })
}
