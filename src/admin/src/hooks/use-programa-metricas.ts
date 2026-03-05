import { useQuery } from "@tanstack/react-query"
import { metricasService } from "@/services/metricas.service"
import { QUERY_KEYS } from "@shared/constants"
import type { FiltroFechas, ProgramaMetricasResponse } from "@shared/types/crowdpromotion"

export function useProgramaMetricas(programaId: string, filtro: FiltroFechas = {}) {
    return useQuery<ProgramaMetricasResponse>({
        queryKey: QUERY_KEYS.crowdpromotion.metricas.programa(
            programaId,
            filtro.fechaDesde,
            filtro.fechaHasta
        ),
        queryFn: () => metricasService.getProgramaMetricas(programaId, filtro),
        enabled: !!programaId,
        staleTime: 0,
        retry: false,
    })
}
