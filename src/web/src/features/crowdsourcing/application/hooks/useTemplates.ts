import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { crowdsourcingApi } from "../../infrastructure"

export function useTemplates() {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.templates.all,
        queryFn: () => crowdsourcingApi.getTemplates(),
        staleTime: 5 * 60 * 1000,
    })
}
