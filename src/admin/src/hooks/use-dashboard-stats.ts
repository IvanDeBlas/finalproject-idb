import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { dashboardService } from "@/services/dashboard.service"

export function useDashboardStats() {
    return useQuery({
        queryKey: QUERY_KEYS.dashboard.resumen,
        queryFn: () => dashboardService.getResumen(),
        staleTime: 30_000,
    })
}
