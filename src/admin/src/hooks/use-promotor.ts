import { useQuery } from "@tanstack/react-query"
import { promotorService } from "@/services/promotor.service"
import { QUERY_KEYS } from "@shared/constants"

export function usePromotor() {
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.promotor.me,
        queryFn: () => promotorService.getMe(),
        retry: false,
        staleTime: 30_000,
    })
}
