import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query"
import { artistaService } from "@/services/artista.service"
import { QUERY_KEYS } from "@shared/constants"
import type { CreateArtistaDto, UpdateArtistaDto } from "@shared/types"

export function useMyArtistProfile() {
  return useQuery({
    queryKey: [QUERY_KEYS.ARTISTA_ME],
    queryFn: () => artistaService.getMyProfile(),
  })
}

export function useCreateArtista() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: (data: CreateArtistaDto) => artistaService.create(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.ARTISTA_ME] })
    },
  })
}

export function useUpdateArtista() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateArtistaDto }) =>
      artistaService.update(id, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [QUERY_KEYS.ARTISTA_ME] })
    },
  })
}
