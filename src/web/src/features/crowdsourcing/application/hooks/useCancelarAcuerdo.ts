import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { QUERY_KEYS } from "@shared/constants"
import { getAcuerdoErrorMessage } from "@shared/utils/error-messages"
import { acuerdoApi } from "../../infrastructure"
import type { CancelarAcuerdoRequest } from "../../domain"

export function useCancelarAcuerdo(
    acuerdoId: string,
    onSuccess?: () => void
) {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: CancelarAcuerdoRequest) =>
            acuerdoApi.cancelar(acuerdoId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId),
            })
            toast.success("Acuerdo cancelado.")
            onSuccess?.()
        },
        onError: (error: Error) => {
            toast.error(getAcuerdoErrorMessage(error.message))
        },
    })
}
