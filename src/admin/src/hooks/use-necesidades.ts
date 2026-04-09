import { useQuery } from "@tanstack/react-query"
import { necesidadService } from "@/services/necesidad.service"
import { QUERY_KEYS } from "@shared/constants"
import type { PaginatedResponse, NecesidadCrowdsourcingList, NecesidadCrowdsourcing } from "@shared/types"

interface UseMisNecesidadesParams {
    page?: number
    pageSize?: number
    estado?: number
    search?: string
}

export function useMisNecesidades({
    page = 1,
    pageSize = 12,
    estado,
    search,
}: UseMisNecesidadesParams = {}) {
    return useQuery<PaginatedResponse<NecesidadCrowdsourcingList>>({
        queryKey: [...QUERY_KEYS.crowdsourcing.necesidades.mis, { page, pageSize, estado, search }],
        queryFn: () => necesidadService.getMisNecesidades({ page, pageSize, estado, search }),
        staleTime: 2 * 60 * 1000,
        placeholderData: (previousData) => previousData,
    })
}

export function useNecesidad(id: string) {
    return useQuery<NecesidadCrowdsourcing | null>({
        queryKey: QUERY_KEYS.crowdsourcing.necesidades.byId(id),
        queryFn: () => necesidadService.getById(id),
        enabled: !!id,
        staleTime: 5 * 60 * 1000,
    })
}
