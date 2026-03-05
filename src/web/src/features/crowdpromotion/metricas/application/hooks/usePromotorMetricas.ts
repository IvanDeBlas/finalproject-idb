import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { trackingService } from "../../infrastructure/tracking.service"

export function usePromotorMetricas(programaId: string | undefined) {
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.metricas.promotor(programaId, undefined, undefined),
        queryFn: () => trackingService.getMetricasPromotor(programaId),
        enabled: !!programaId,
        staleTime: 60 * 1000,
        retry: 1,
    })
}
