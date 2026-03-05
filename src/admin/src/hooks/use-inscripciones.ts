import { useQuery } from "@tanstack/react-query"
import { inscripcionService } from "@/services/inscripcion.service"
import { QUERY_KEYS } from "@shared/constants"
import type { InscripcionesFilters } from "@shared/types"

export function useInscripcionesPendientes(programaId: string) {
    const filters: InscripcionesFilters = { estado: "Pendiente" }
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.programas.inscripciones(programaId, filters),
        queryFn: () => inscripcionService.getInscripciones(programaId, filters),
        enabled: !!programaId,
        staleTime: 0,
        retry: false,
    })
}

export function useInscripcionesAprobadas(programaId: string) {
    const filters: InscripcionesFilters = { estado: "Aprobado" }
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.programas.inscripciones(programaId, filters),
        queryFn: () => inscripcionService.getInscripciones(programaId, filters),
        enabled: !!programaId,
        staleTime: 30_000,
        retry: false,
    })
}

export function useInscripcionesBloqueadas(programaId: string) {
    const filters: InscripcionesFilters = { estado: "Bloqueado" }
    return useQuery({
        queryKey: QUERY_KEYS.crowdpromotion.programas.inscripciones(programaId, filters),
        queryFn: () => inscripcionService.getInscripciones(programaId, filters),
        enabled: !!programaId,
        staleTime: 60_000,
        retry: false,
    })
}
