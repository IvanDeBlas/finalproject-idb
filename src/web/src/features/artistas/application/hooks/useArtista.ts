import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { artistaService } from "../../infrastructure/artista.service"

export function useArtista(id: string) {
  return useQuery({
    queryKey: QUERY_KEYS.artistas.byId(id),
    queryFn: () => artistaService.getById(id),
    enabled: !!id,
  })
}
