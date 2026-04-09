import { useMutation, useQueryClient } from "@tanstack/react-query"
import { promotorService } from "@/services/promotor.service"
import { QUERY_KEYS } from "@shared/constants"
import { mapUpdateFormToRequest } from "@shared/utils/mappers"
import { toast } from "sonner"
import type { UpdatePromotorFormData } from "@shared/schemas"

export function useUpdatePromotor() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (formData: UpdatePromotorFormData) => {
            const request = mapUpdateFormToRequest(formData)
            return promotorService.update(request)
        },
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.promotor.me,
            })
            toast.success("Perfil de promotor actualizado correctamente")
        },
        onError: (error: Error) => {
            toast.error(error.message)
        },
    })
}

export function useDesactivarPromotor() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: () => promotorService.desactivar(),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.promotor.me,
            })
            toast.success("Tu perfil de promotor ha sido desactivado")
        },
        onError: (error: Error) => {
            toast.error(error.message)
        },
    })
}
