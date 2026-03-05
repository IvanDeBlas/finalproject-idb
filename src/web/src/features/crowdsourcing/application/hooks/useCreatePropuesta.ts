import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { QUERY_KEYS } from "@shared/constants"
import { getPropuestaErrorMessage } from "@shared/utils/error-messages"
import { propuestaApi } from "../../infrastructure"
import type { CreatePropuestaRequest } from "../../domain"

export function useCreatePropuesta(
    necesidadId: string,
    onSuccess?: () => void
) {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: CreatePropuestaRequest) =>
            propuestaApi.create(necesidadId, data),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.necesidades.publicaById(necesidadId),
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.propuestas.mis,
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.necesidades.publicas,
            })
            toast.success(
                "Propuesta enviada correctamente. El artista sera notificado."
            )
            onSuccess?.()
        },
        onError: (error: Error) => {
            toast.error(getPropuestaErrorMessage(error.message))
        },
    })
}
