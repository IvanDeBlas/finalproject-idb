import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { dashboardService } from "@/services/dashboard.service"

export function useRecentBackings(campaniaId?: string) {
    return useQuery({
        queryKey: QUERY_KEYS.dashboard.campaniaBackings(campaniaId ?? "", { page: 1, pageSize: 5 }),
        queryFn: () => dashboardService.getCampaniaBackings(campaniaId!, { page: 1, pageSize: 5 }),
        enabled: !!campaniaId,
        staleTime: 30_000,
        select: (data) => data.backings.items,
    })
}
