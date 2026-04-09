import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { inscripcionService } from "../../infrastructure/inscripcion.service"
import type { ExplorarProgramasFilters } from "../../domain"

export function useExplorarProgramas(filters?: ExplorarProgramasFilters) {
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.programas.explorar(filters),
        queryFn: () => inscripcionService.explorarProgramas(filters),
        staleTime: 1 * 60 * 1000,
        retry: 1,
    })
}
