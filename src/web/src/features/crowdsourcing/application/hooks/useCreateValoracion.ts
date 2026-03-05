import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { QUERY_KEYS } from "@shared/constants"
import { getValoracionErrorMessage } from "@shared/utils/error-messages"
import { valoracionApi } from "../../infrastructure"
import type { CreateValoracionRequest, ValoracionCreatedResult } from "../../domain"

export function useCreateValoracion(
    acuerdoId: string,
    userIdValorado: string,
    onSuccess?: (valoracion: ValoracionCreatedResult) => void
) {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: CreateValoracionRequest) =>
            valoracionApi.create(acuerdoId, data),
        onSuccess: (valoracion) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId),
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.valoraciones.byUserId(userIdValorado),
            })
            toast.success("Valoracion enviada. Gracias por tu feedback.")
            onSuccess?.(valoracion)
        },
        onError: (error: Error) => {
            toast.error(getValoracionErrorMessage(error.message))
        },
    })
}
