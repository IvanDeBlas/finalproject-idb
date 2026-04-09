import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { QUERY_KEYS } from "@shared/constants"
import { getAcuerdoErrorMessage } from "@shared/utils/error-messages"
import { acuerdoApi } from "../../infrastructure"

export function useCompletarAcuerdo(
    acuerdoId: string,
    onSuccess?: () => void
) {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: () => acuerdoApi.completar(acuerdoId),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId),
            })
            toast.success(
                "Acuerdo completado. Puedes dejar una valoracion al profesional."
            )
            onSuccess?.()
        },
        onError: (error: Error) => {
            toast.error(getAcuerdoErrorMessage(error.message))
        },
    })
}
