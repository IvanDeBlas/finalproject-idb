import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { acuerdoApi } from "../../infrastructure"

export function useAcuerdo(id: string) {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.acuerdos.byId(id),
        queryFn: () => acuerdoApi.getById(id),
        staleTime: 30_000,
        enabled: !!id,
        retry: false,
    })
}
