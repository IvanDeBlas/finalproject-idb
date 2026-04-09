import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { QUERY_KEYS } from "@shared/constants"
import { getTareaPromocionErrorMessage } from "@shared/utils/error-messages"
import { tareasService } from "../../infrastructure/tareas.service"
import type { CompletarTareaRequest, CompletarTareaResponse } from "../../domain"

interface CompletarTareaVars {
    programaId: string
    tareaId: string
    data: CompletarTareaRequest
    esReenvio: boolean
}

export function useCompletarTarea() {
    const queryClient = useQueryClient()

    return useMutation<CompletarTareaResponse, Error & { errorCode?: string }, CompletarTareaVars>({
        mutationFn: ({ programaId, tareaId, data }) =>
            tareasService.completarTarea(programaId, tareaId, data),
        onSuccess: (_data, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.tareas.mis(variables.programaId),
            })
            if (variables.esReenvio) {
                toast.success("Prueba re-enviada para validacion.")
            } else {
                toast.success("Tarea enviada. El artista revisara tu prueba.")
            }
        },
        onError: (error: Error & { errorCode?: string }) => {
            const errorCode = error.errorCode ?? "5000"
            if (errorCode === "4027") {
                toast.error("Esta tarea ya fue completada y no es repetible.")
                return
            }
            if (errorCode === "4028") {
                toast.error("Alcanzaste el maximo de repeticiones para esta tarea.")
                return
            }
            if (errorCode === "4024") {
                toast.error("Este programa no esta activo. No puedes completar tareas.")
                return
            }
            toast.error(getTareaPromocionErrorMessage(errorCode))
        },
    })
}
