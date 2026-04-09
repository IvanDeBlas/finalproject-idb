import { useQuery } from "@tanstack/react-query"
import { maestrasService } from "@/services/maestras.service"
import { QUERY_KEYS } from "@shared/constants"

export function useTiposNecesidad() {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.maestras.tiposNecesidad,
        queryFn: () => maestrasService.getTiposNecesidad(),
        staleTime: Infinity,
    })
}
