import { useQuery } from "@tanstack/react-query"
import { maestrasService } from "@/services/maestras.service"
import { QUERY_KEYS } from "@shared/constants"

export function useMonedas() {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.maestras.monedas,
        queryFn: () => maestrasService.getMonedas(),
        staleTime: Infinity,
    })
}
