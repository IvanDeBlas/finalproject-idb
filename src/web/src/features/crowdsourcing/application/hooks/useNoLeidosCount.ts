import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { useAuthStore } from "@/store/auth-store"
import { conversacionApi } from "../../infrastructure"

export function useNoLeidosCount() {
    const { isAuthenticated } = useAuthStore()

    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.conversaciones.noLeidos,
        queryFn: () => conversacionApi.getNoLeidosCount(),
        staleTime: 0,
        refetchInterval: 60_000,
        refetchIntervalInBackground: false,
        enabled: isAuthenticated,
    })
}
