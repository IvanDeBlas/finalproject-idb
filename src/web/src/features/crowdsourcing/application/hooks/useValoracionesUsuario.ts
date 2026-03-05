import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { valoracionApi } from "../../infrastructure"

export function useValoracionesUsuario(
    userId: string,
    page: number = 1,
    pageSize: number = 10
) {
    return useQuery({
        queryKey: [
            ...QUERY_KEYS.crowdsourcing.valoraciones.byUserId(userId),
            page,
            pageSize,
        ],
        queryFn: () => valoracionApi.getByUser(userId, { page, pageSize }),
        staleTime: 60_000,
        enabled: !!userId,
    })
}
