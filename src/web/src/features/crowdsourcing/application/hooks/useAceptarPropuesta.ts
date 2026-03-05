import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { QUERY_KEYS } from "@shared/constants"
import { getAcuerdoErrorMessage } from "@shared/utils/error-messages"
import { acuerdoApi } from "../../infrastructure"
import type { AceptarPropuestaRequest } from "../../domain"

export function useAceptarPropuesta(
    propuestaId: string,
    onSuccess?: (acuerdoId: string) => void
) {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: AceptarPropuestaRequest) =>
            acuerdoApi.aceptarPropuesta(propuestaId, data),
        onSuccess: (result) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.propuestas.mis,
            })
            toast.success(
                "Acuerdo creado correctamente. Ya puedes comunicarte con el profesional."
            )
            onSuccess?.(result.acuerdoId)
        },
        onError: (error: Error) => {
            toast.error(getAcuerdoErrorMessage(error.message))
        },
    })
}
