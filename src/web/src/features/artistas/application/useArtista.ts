import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { QUERY_KEYS } from "@/lib/constants"
import { artistaService } from "../infrastructure/artista.service"
import type { CreateArtistaData, UpdateArtistaData } from "../domain"

export function useMyArtistProfile() {
  return useQuery({
    queryKey: [QUERY_KEYS.ARTISTA, "me"],
    queryFn: () => artistaService.getMyProfile(),
  })
}

export function useCreateArtista() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (data: CreateArtistaData) => artistaService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.ARTISTA, "me"] })
    },
  })
}

export function useUpdateArtista() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateArtistaData }) =>
      artistaService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.ARTISTA, "me"] })
    },
  })
}
