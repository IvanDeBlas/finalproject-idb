import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { useAuthStore } from "@/store/auth-store"
import { conversacionApi } from "../../infrastructure"
import type { FiltroConversacion } from "../../domain"

export function useConversaciones(
    filtro: FiltroConversacion = "todas",
    page: number = 1,
    pageSize: number = 20
) {
    const { isAuthenticated } = useAuthStore()

    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.conversaciones.lista(filtro),
        queryFn: () =>
            conversacionApi.getAll({ contexto: filtro, page, pageSize }),
        staleTime: 0,
        refetchInterval: 30_000,
        refetchIntervalInBackground: false,
        enabled: isAuthenticated,
    })
}
