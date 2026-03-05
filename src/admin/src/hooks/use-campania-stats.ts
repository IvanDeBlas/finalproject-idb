import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { dashboardService } from "@/services/dashboard.service"

export function useCampaniaStats(campaniaId: string) {
    return useQuery({
        queryKey: QUERY_KEYS.dashboard.campaniaStats(campaniaId),
        queryFn: () => dashboardService.getCampaniaStats(campaniaId),
        enabled: !!campaniaId,
        staleTime: 60_000,
    })
}
