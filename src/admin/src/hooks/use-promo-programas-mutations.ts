import { useMutation, useQueryClient } from "@tanstack/react-query"
import { promoProgramaService } from "@/services/promo-programa.service"
import { QUERY_KEYS } from "@shared/constants"
import { toast } from "sonner"
import type {
    CreatePromoProgramaRequest,
    UpdatePromoProgramaRequest,
} from "@shared/types"

export function useCreatePromoPrograma() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: CreatePromoProgramaRequest) =>
            promoProgramaService.create(data),
        onSuccess: () => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.mis,
            })
        },
        onError: (error: Error) => {
            toast.error(error.message)
        },
    })
}

export function useUpdatePromoPrograma() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: UpdatePromoProgramaRequest }) =>
            promoProgramaService.update(id, data),
        onSuccess: (_data, variables) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.byId(variables.id),
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.mis,
            })
        },
        onError: (error: Error) => {
            toast.error(error.message)
        },
    })
}

export function useDesactivarPromoPrograma() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (id: string) => promoProgramaService.desactivar(id),
        onSuccess: (_data, id) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.byId(id),
            })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.crowdpromotion.programas.mis,
            })
            toast.success("Programa desactivado correctamente")
        },
        onError: (error: Error) => {
            toast.error(error.message)
        },
    })
}
