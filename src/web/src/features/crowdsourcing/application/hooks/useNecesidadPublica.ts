import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { necesidadPublicaApi } from "../../infrastructure"

export function useNecesidadPublica(id: string) {
    return useQuery({
        queryKey: QUERY_KEYS.crowdsourcing.necesidades.publicaById(id),
        queryFn: () => necesidadPublicaApi.getById(id),
        staleTime: 30_000,
        enabled: !!id,
    })
}
