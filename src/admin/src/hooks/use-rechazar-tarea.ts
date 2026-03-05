import { useMutation, useQueryClient } from "@tanstack/react-query"
import { tareasService } from "@/services/tareas.service"
import { QUERY_KEYS } from "@shared/constants"
import type { RechazarTareaRequest, RechazarTareaResponse } from "@shared/types/crowdpromotion"

interface RechazarTareaParams {
    programaId: string
    tareaPromotorId: string
    data: RechazarTareaRequest
}

export function useRechazarTarea() {
    const queryClient = useQueryClient()

    return useMutation<RechazarTareaResponse, Error, RechazarTareaParams>({
        mutationFn: (params: RechazarTareaParams) =>
            tareasService.rechazarTarea(params.programaId, params.tareaPromotorId, params.data),
        onSuccess: (_data, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.tareas.pendientes(variables.programaId),
            })
        },
    })
}
