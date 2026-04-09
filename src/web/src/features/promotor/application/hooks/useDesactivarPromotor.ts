import { useMutation, useQueryClient } from "@tanstack/react-query"
import { useNavigate } from "react-router-dom"
import { toast } from "sonner"
import { QUERY_KEYS, APP_ROUTES } from "@shared/constants"
import { getPromotorErrorMessage } from "@shared/utils/error-messages"
import { promotorService } from "../../infrastructure/promotor.service"

export function useDesactivarPromotor() {
    const queryClient = useQueryClient()
    const navigate = useNavigate()

    return useMutation({
        mutationFn: () => promotorService.desactivar(),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.promotor.me })
            toast.success("Perfil de promotor desactivado")
            navigate(APP_ROUTES.landing.home)
        },
        onError: (error: Error & { errorCode?: string }) => {
            const errorCode = error.errorCode ?? "5000"
            if (errorCode === "4019") {
                toast.info("Tu perfil de promotor ya estaba desactivado")
                queryClient.invalidateQueries({ queryKey: QUERY_KEYS.crowdpromotion.promotor.me })
                return
            }
            toast.error(getPromotorErrorMessage(errorCode))
        },
    })
}
