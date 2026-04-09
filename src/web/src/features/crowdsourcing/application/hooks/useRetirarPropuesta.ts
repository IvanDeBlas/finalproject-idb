import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { QUERY_KEYS } from "@shared/constants"
import { getPropuestaErrorMessage } from "@shared/utils/error-messages"
import { propuestaApi } from "../../infrastructure"

export function useRetirarPropuesta() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (propuestaId: string) => propuestaApi.retirar(propuestaId),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.propuestas.mis,
            })
            toast.success("Propuesta retirada correctamente")
        },
        onError: (error: Error) => {
            toast.error(getPropuestaErrorMessage(error.message))
        },
    })
}
