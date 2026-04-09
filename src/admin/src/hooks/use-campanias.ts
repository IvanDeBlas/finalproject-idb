import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { campaniaService } from "@/services/campania.service"
import { QUERY_KEYS } from "@shared/constants"
import type { CreateCampaniaRequest, UpdateCampaniaRequest } from "@shared/types"

export function useCampanias() {
    return useQuery({
        queryKey: QUERY_KEYS.campanias.all,
        queryFn: () => campaniaService.getAll(),
    })
}

export function useCampania(id: string) {
    return useQuery({
        queryKey: QUERY_KEYS.campanias.byId(id),
        queryFn: () => campaniaService.getById(id),
        enabled: !!id,
    })
}

export function useMisCampanias() {
    return useQuery({
        queryKey: QUERY_KEYS.campanias.misCampanias,
        queryFn: () => campaniaService.getMisCampanias(),
    })
}

export function useCreateCampania() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: CreateCampaniaRequest) => campaniaService.create(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.campanias.all })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.campanias.misCampanias,
            })
        },
    })
}

export function useUpdateCampania() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: UpdateCampaniaRequest }) =>
            campaniaService.update(id, data),
        onSuccess: (_, { id }) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.campanias.byId(id),
            })
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.campanias.all })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.campanias.misCampanias,
            })
        },
    })
}

export function usePublicarCampania() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (id: string) => campaniaService.publicar(id),
        onSuccess: (_, id) => {
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.campanias.byId(id),
            })
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.campanias.all })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.campanias.misCampanias,
            })
        },
    })
}

export function useDeleteCampania() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (id: string) => campaniaService.delete(id),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: QUERY_KEYS.campanias.all })
            queryClient.invalidateQueries({
                queryKey: QUERY_KEYS.campanias.misCampanias,
            })
        },
    })
}
