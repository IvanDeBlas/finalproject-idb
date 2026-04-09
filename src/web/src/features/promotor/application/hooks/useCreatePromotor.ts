import { useMutation, useQueryClient } from "@tanstack/react-query"
import { useNavigate } from "react-router-dom"
import { toast } from "sonner"
import { QUERY_KEYS, APP_ROUTES } from "@shared/constants"
import { getPromotorErrorMessage } from "@shared/utils/error-messages"
import { promotorService } from "../../infrastructure/promotor.service"
import type { CreatePromotorRequest } from "../../domain"

export function useCreatePromotor() {
    const queryClient = useQueryClient()
    const navigate = useNavigate()

    return useMutation({
        mutationFn: (data: CreatePromotorRequest) => promotorService.create(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.promotor.me })
            toast.success("Perfil de promotor creado correctamente")
            navigate(APP_ROUTES.landing.promotor.dashboard)
        },
        onError: (error: Error & { errorCode?: string }) => {
            const errorCode = error.errorCode ?? "5000"
            if (errorCode === "4018") {
                toast.info("Ya tienes un perfil de promotor creado")
                navigate(APP_ROUTES.landing.promotor.dashboard)
                return
            }
            toast.error(getPromotorErrorMessage(errorCode))
        },
    })
}
