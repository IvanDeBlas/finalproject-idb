import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { QUERY_KEYS } from "@shared/constants"
import { getAcuerdoErrorMessage } from "@shared/utils/error-messages"
import { entregableApi } from "../../infrastructure"
import type { RechazarEntregableRequest } from "../../domain"

export function useRechazarEntregable(
    acuerdoId: string,
    onSuccess?: () => void
) {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: ({
            entregableId,
            data,
        }: {
            entregableId: string
            data: RechazarEntregableRequest
        }) => entregableApi.rechazar(entregableId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId),
            })
            toast.success(
                "Entregable rechazado. El profesional sera notificado."
            )
            onSuccess?.()
        },
        onError: (error: Error) => {
            toast.error(getAcuerdoErrorMessage(error.message))
        },
    })
}
