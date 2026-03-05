import { useMutation, useQueryClient } from "@tanstack/react-query"
import { tareasService } from "@/services/tareas.service"
import { QUERY_KEYS } from "@shared/constants"
import type { ValidarTareaRequest, ValidarTareaResponse } from "@shared/types/crowdpromotion"

interface ValidarTareaParams {
    programaId: string
    tareaPromotorId: string
    data: ValidarTareaRequest
}

export function useValidarTarea() {
    const queryClient = useQueryClient()

    return useMutation<ValidarTareaResponse, Error, ValidarTareaParams>({
        mutationFn: (params: ValidarTareaParams) =>
            tareasService.validarTarea(params.programaId, params.tareaPromotorId, params.data),
        onSuccess: (_data, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.tareas.pendientes(variables.programaId),
            })
        },
    })
}
