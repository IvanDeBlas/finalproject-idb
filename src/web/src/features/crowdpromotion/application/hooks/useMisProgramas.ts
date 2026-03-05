import { useQuery } from "@tanstack/react-query"
import { QUERY_KEYS } from "@shared/constants"
import { inscripcionService } from "../../infrastructure/inscripcion.service"

export function useMisProgramas(page: number = 1, pageSize: number = 50) {
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.inscripciones.misProgramas(page),
        queryFn: () => inscripcionService.misProgramas(page, pageSize),
        staleTime: 30 * 1000,
        retry: 1,
    })
}
