import { useQuery } from "@tanstack/react-query"
import { promoProgramaService } from "@/services/promo-programa.service"
import { QUERY_KEYS } from "@shared/constants"

interface MisProgramasParams {
    esActivo?: boolean
    page?: number
    pageSize?: number
}

export function useMisProgramas(params: MisProgramasParams = {}) {
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.programas.misFiltrados({
            esActivo: params.esActivo,
            page: params.page,
            pageSize: params.pageSize,
        }),
        queryFn: () => promoProgramaService.getMisProgramas(params),
        staleTime: 30_000,
    })
}

export function usePromoPrograma(id: string) {
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.programas.byId(id),
        queryFn: () => promoProgramaService.getById(id),
        enabled: !!id,
        retry: false,
        staleTime: 30_000,
    })
}
