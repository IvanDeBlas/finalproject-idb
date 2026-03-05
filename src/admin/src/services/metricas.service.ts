import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    FiltroFechas,
    ProgramaMetricasResponse,
} from "@shared/types/crowdpromotion"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}

class MetricasService {
    async getProgramaMetricas(
        programaId: string,
        filtro?: FiltroFechas
    ): Promise<ProgramaMetricasResponse> {
        const searchParams = new URLSearchParams()
        if (filtro?.fechaDesde && filtro.fechaDesde !== "") {
            searchParams.set("fechaDesde", filtro.fechaDesde)
        }
        if (filtro?.fechaHasta && filtro.fechaHasta !== "") {
            searchParams.set("fechaHasta", filtro.fechaHasta)
        }

        const queryString = searchParams.toString()
        const baseUrl = API_ROUTES.crowdpromotion.programaMetricas(programaId)
        const url = queryString ? `${baseUrl}?${queryString}` : baseUrl

        const response = await apiFetch<ServiceResponse<ProgramaMetricasResponse>>(url)

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorMsg = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )
            throw new Error(errorMsg?.message ?? "Error al obtener metricas")
        }

        return response.data
    }
}

export const metricasService = new MetricasService()
