import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { QUERY_KEYS } from "@shared/constants"
import { getAcuerdoErrorMessage } from "@shared/utils/error-messages"
import { acuerdoApi } from "../../infrastructure"
import type { RechazarPropuestaRequest } from "../../domain"

export function useRechazarPropuesta(necesidadId: string, onSuccess?: () => void) {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: ({
            propuestaId,
            data,
        }: {
            propuestaId: string
            data: RechazarPropuestaRequest
        }) => acuerdoApi.rechazarPropuesta(propuestaId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.necesidades.byId(necesidadId),
            })
            toast.success("Propuesta rechazada")
            onSuccess?.()
        },
        onError: (error: Error) => {
            toast.error(getAcuerdoErrorMessage(error.message))
        },
    })
}
