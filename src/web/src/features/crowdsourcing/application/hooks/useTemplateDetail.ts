import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { crowdsourcingApi } from "../../infrastructure"

export function useTemplateDetail(id: string) {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.templates.byId(id),
        queryFn: () => crowdsourcingApi.getTemplateById(id),
        enabled: !!id,
        staleTime: 5 * 60 * 1000,
    })
}
