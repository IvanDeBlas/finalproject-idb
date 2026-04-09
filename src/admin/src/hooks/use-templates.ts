import { useQuery } from "@tanstack/react-query"
import { templateService } from "@/services/template.service"
import { QUERY_KEYS } from "@shared/constants"

export function useTemplates() {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.templates.all,
        queryFn: () => templateService.getAll(),
        staleTime: 5 * 60 * 1000,
    })
}

export function useTemplate(id: string) {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.templates.byId(id),
        queryFn: () => templateService.getById(id),
        enabled: !!id,
        staleTime: 5 * 60 * 1000,
    })
}
