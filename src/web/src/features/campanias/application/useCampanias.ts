import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { QUERY_KEYS } from "@/lib/constants"
import { campaniaApi } from "../infrastructure/campania.api"
import type { CreateCampaniaData, UpdateCampaniaData, CampaniaFilters, PaginationParams } from "../domain"

// Query: Get all campanias with optional filters and pagination
export function useCampanias(filters?: CampaniaFilters, pagination?: PaginationParams) {
    return useQuery({
        queryKey: [QUERY_KEYS.CAMPANIAS, "filtered", { ...filters, ...pagination }],
        queryFn: () => campaniaApi.getAll(filters, pagination),
        staleTime: 5 * 60 * 1000, // 5 min
    })
}

// Query: Get campania by id
export function useCampania(id: string) {
    return useQuery({
        queryKey: [QUERY_KEYS.CAMPANIA, id],
        queryFn: () => campaniaApi.getById(id),
        enabled: !!id,
        staleTime: 5 * 60 * 1000,
    })
}

// Query: Get campanias by artista
export function useMisCampanias(artistaId: string) {
    return useQuery({
        queryKey: [QUERY_KEYS.CAMPANIAS, "artista", artistaId],
        queryFn: () => campaniaApi.getByArtistaId(artistaId),
        enabled: !!artistaId,
    })
}

// Mutation: Create campania
export function useCreateCampania() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (data: CreateCampaniaData) => campaniaApi.create(data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.CAMPANIAS] })
        },
    })
}

// Mutation: Update campania
export function useUpdateCampania() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: ({ id, data }: { id: string; data: UpdateCampaniaData }) =>
            campaniaApi.update(id, data),
        onSuccess: (_, { id }) => {
            queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.CAMPANIA, id] })
            queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.CAMPANIAS] })
        },
    })
}

// Mutation: Publicar campania
export function usePublicarCampania() {
    const queryClient = useQueryClient()

    return useMutation({
        mutationFn: (id: string) => campaniaApi.publicar(id),
        onSuccess: (_, id) => {
            queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.CAMPANIA, id] })
            queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.CAMPANIAS] })
        },
    })
}
