import { useQuery } from "@tanstack/react-query"
import { campaniaService } from "@/services/campania.service"
import { QUERY_KEYS } from "@shared/constants"

export function useCampaignBackings(campaniaId: string) {
    return useQuery({
        queryKey: QUERY_KEYS.backings.byCampania(campaniaId),
        queryFn: () => campaniaService.getBackings(campaniaId),
        enabled: !!campaniaId,
    })
}

export function useCampaignStats(campaniaId: string) {
    return useQuery({
        queryKey: QUERY_KEYS.campanias.stats(campaniaId),
        queryFn: () => campaniaService.getStats(campaniaId),
        enabled: !!campaniaId,
    })
}
