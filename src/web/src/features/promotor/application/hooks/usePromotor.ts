import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { promotorService } from "../../infrastructure/promotor.service"

export function usePromotor() {
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.promotor.me,
        queryFn: () => promotorService.getMe(),
        retry: false,
        staleTime: 2 * 60 * 1000,
    })
}
