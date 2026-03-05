import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { QUERY_KEYS } from "@shared/constants"
import { getAcuerdoErrorMessage } from "@shared/utils/error-messages"
import { milestoneApi } from "../../infrastructure"

export function useDeleteMilestone(acuerdoId: string) {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (milestoneId: string) =>
            milestoneApi.delete(acuerdoId, milestoneId),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdsourcing.acuerdos.byId(acuerdoId),
            })
            toast.success("Milestone eliminado")
        },
        onError: (error: Error) => {
            toast.error(getAcuerdoErrorMessage(error.message))
        },
    })
}
