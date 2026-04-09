import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { useNavigate } from "react-router-dom"
import { toast } from "sonner"
import { QUERY_KEYS } from "@/lib/constants"
import { backingService } from "../infrastructure/backing.service"
import type { CreateBackingRequest } from "@shared/types/backing"

export function useCampaniaBackings(campaniaId: string) {
    return useQuery({
        queryKey: [QUERY_KEYS.BACKINGS, "campania", campaniaId],
        queryFn: () => backingService.getByCampaniaId(campaniaId),
        enabled: !!campaniaId,
        staleTime: 1 * 60 * 1000,
    })
}

export function useMyBackings() {
    return useQuery({
        queryKey: [QUERY_KEYS.BACKINGS, "me"],
        queryFn: () => backingService.getMyBackings(),
    })
}

export function useCreateBacking() {
    const queryClient = useQueryClient()
    const navigate = useNavigate()

    return useMutation({
        mutationFn: (data: CreateBackingRequest & { campaniaId: string }) =>
            backingService.create(data),

        onSuccess: (result, variables) => {
            queryClient.invalidateQueries({
                queryKey: [QUERY_KEYS.BACKINGS],
            })
            queryClient.invalidateQueries({
                queryKey: [QUERY_KEYS.CAMPANIA, variables.campaniaId],
            })
            queryClient.invalidateQueries({
                queryKey: [QUERY_KEYS.CAMPANIAS],
            })

            toast.success("Apoyo confirmado!", {
                description: "Gracias por tu contribucion",
            })

            navigate(
                `/campanias/${variables.campaniaId}/confirmacion?backingId=${result.id}`
            )
        },

        onError: (error: Error) => {
            const message = error.message || "Intenta de nuevo mas tarde"
            toast.error("Error al procesar apoyo", {
                description: message,
            })
        },
    })
}
