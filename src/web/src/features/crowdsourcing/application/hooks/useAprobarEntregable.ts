import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { QUERY_KEYS } from "@shared/constants"
import { getAcuerdoErrorMessage } from "@shared/utils/error-messages"
import { entregableApi } from "../../infrastructure"
import type { AprobarEntregableRequest } from "../../domain"

export function useAprobarEntregable(
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
            data: AprobarEntregableRequest
        }) => entregableApi.aprobar(entregableId, data),
        onSuccess: (result) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId),
            })
            toast.success("Entregable aprobado")
            if (result.todosAprobadosEnMilestone) {
                toast.info(
                    "Todos los entregables del milestone fueron aprobados. Puedes marcarlo como completado."
                )
            }
            onSuccess?.()
        },
        onError: (error: Error) => {
            toast.error(getAcuerdoErrorMessage(error.message))
        },
    })
}
