import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { dashboardService } from "@/services/dashboard.service"
import type { BackingsQueryParams } from "@shared/schemas"

export function useCampaniaBackings(
    campaniaId: string,
    params?: BackingsQueryParams
) {
    return useQuery({
        queryKey: QUERY_KEYS.dashboard.campaniaBackings(campaniaId, params),
        queryFn: () => dashboardService.getCampaniaBackings(campaniaId, params),
        enabled: !!campaniaId,
        placeholderData: (previousData) => previousData,
    })
}
