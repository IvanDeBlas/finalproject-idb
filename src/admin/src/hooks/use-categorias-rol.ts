import { useQuery } from "@tanstack/react-query"
import { maestrasService } from "@/services/maestras.service"
import { QUERY_KEYS } from "@shared/constants"

export function useCategoriasRol() {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.maestras.categoriasRol,
        queryFn: () => maestrasService.getCategoriasRol(),
        staleTime: 10 * 60 * 1000,
    })
}
