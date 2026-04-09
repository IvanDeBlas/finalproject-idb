import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@/lib/constants"
import { rewardService } from "../../infrastructure/services/reward.service"
import type { Reward } from "@shared/types/reward"

interface UseRewardsByCampaniaOptions {
    enabled?: boolean
}

export function useRewardsByCampania(
    campaniaId: string,
    options?: UseRewardsByCampaniaOptions
) {
    return useQuery<Reward[], Error>({
        queryKey: [QUERY_KEYS.REWARDS, "campania", campaniaId, { esActivo: true }],
        queryFn: () => rewardService.getByCampaniaId(campaniaId, { esActivo: true }),
        enabled: options?.enabled !== false && !!campaniaId,
        staleTime: 5 * 60 * 1000,
        select: (data) => {
            return [...data].sort((a, b) => a.importeMinimo - b.importeMinimo)
        },
    })
}
