import { useQuery } from "@tanstack/react-query"
import { maestrasService } from "@/services/maestras.service"
import { QUERY_KEYS } from "@shared/constants"

export function useModalidadesTrabajo() {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.maestras.modalidadesTrabajo,
        queryFn: () => maestrasService.getModalidadesTrabajo(),
        staleTime: Infinity,
    })
}
