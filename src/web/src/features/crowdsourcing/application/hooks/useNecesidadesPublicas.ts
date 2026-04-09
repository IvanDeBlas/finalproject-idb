import { useQuery, keepPreviousData } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { necesidadPublicaApi } from "../../infrastructure"
import type { NecesidadesPublicasFilter } from "../../domain"

export function useNecesidadesPublicas(filters: NecesidadesPublicasFilter) {
    return useQuery({
        queryKey: [...QUERY_KEYS.crowdsourcing.necesidades.publicas, filters],
        queryFn: () => necesidadPublicaApi.getAll(filters),
        staleTime: 30_000,
        placeholderData: keepPreviousData,
    })
}
