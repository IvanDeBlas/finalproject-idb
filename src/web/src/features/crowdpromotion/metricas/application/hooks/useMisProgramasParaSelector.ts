import { useMemo } from "react"
import { INSCRIPCION_ESTADO } from "@shared/constants"
import { useMisProgramas } from "../../../application/hooks/useMisProgramas"

export interface ProgramaSelectorItem {
    id: string
    nombrePrograma: string
    codigoReferido: string | null
    urlTrackingPersonalizada: string | null
}

export function useMisProgramasParaSelector() {
    const query = useMisProgramas()

    const programas = useMemo<ProgramaSelectorItem[]>(() => {
        if (!query.data?.items) return []
        return query.data.items
            .filter(item => item.estado === INSCRIPCION_ESTADO.APROBADO)
            .map(item => ({
                id: item.programaId,
                nombrePrograma: item.programaTitulo,
                codigoReferido: item.codigoReferido,
                urlTrackingPersonalizada: item.urlTrackingPersonalizada,
            }))
    }, [query.data?.items])

    return {
        programas,
        isLoading: query.isLoading,
        isError: query.isError,
    }
}
