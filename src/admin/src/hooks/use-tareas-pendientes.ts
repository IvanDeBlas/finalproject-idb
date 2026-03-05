import { useQuery } from "@tanstack/react-query"
import { tareasService } from "@/services/tareas.service"
import { QUERY_KEYS, TAREAS_PENDIENTES_DEFAULT_PAGE_SIZE } from "@shared/constants"
import type { TareasPendientesResponse } from "@shared/types/crowdpromotion"

interface UseTareasPendientesOptions {
    page?: number
    pageSize?: number
}

export function useTareasPendientes(
    programaId: string,
    options?: UseTareasPendientesOptions
) {
    const page = options?.page ?? 1
    const pageSize = options?.pageSize ?? TAREAS_PENDIENTES_DEFAULT_PAGE_SIZE

    return useQuery<TareasPendientesResponse>({
        queryKey: QUERY_KEYS.crowdpromotion.tareas.pendientes(programaId, { page, pageSize }),
        queryFn: () => tareasService.getTareasPendientes(programaId, { page, pageSize }),
        enabled: !!programaId,
        staleTime: 0,
        retry: false,
    })
}
