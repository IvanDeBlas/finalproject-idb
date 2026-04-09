import { useMutation, useQueryClient } from "@tanstack/react-query"
import { toast } from "sonner"
import { QUERY_KEYS } from "@shared/constants"
import { getPromotorErrorMessage } from "@shared/utils/error-messages"
import { promotorService } from "../../infrastructure/promotor.service"
import type { UpdatePromotorRequest } from "../../domain"

export function useUpdatePromotor() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: UpdatePromotorRequest) => promotorService.update(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.promotor.me })
            toast.success("Perfil actualizado correctamente")
        },
        onError: (error: Error & { errorCode?: string }) => {
            const errorCode = error.errorCode ?? "5000"
            toast.error(getPromotorErrorMessage(errorCode))
        },
    })
}
