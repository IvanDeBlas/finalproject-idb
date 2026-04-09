import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { tareasService } from "../../infrastructure/tareas.service"

export function useMisTareas(programaId: string) {
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.tareas.mis(programaId),
        queryFn: () => tareasService.misTareas(programaId),
        enabled: !!programaId,
        staleTime: 30 * 1000,
        retry: 1,
    })
}
