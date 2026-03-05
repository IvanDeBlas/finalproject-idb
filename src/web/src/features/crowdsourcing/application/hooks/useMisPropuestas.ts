import { useQuery, keepPreviousData } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { propuestaApi } from "../../infrastructure"

export function useMisPropuestas(filters: {
    estado?: number
    page?: number
    pageSize?: number
}) {
    return useQuery({
        queryKey: [...QUERY_KEYS.crowdsourcing.propuestas.mis, filters],
        queryFn: () => propuestaApi.getMisPropuestas(filters),
        staleTime: 30_000,
        placeholderData: keepPreviousData,
    })
}
