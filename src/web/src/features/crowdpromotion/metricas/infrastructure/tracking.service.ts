import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    RegistrarEventoRequest,
    RegistrarEventoResponse,
    PromotorMetricasResponse,
} from "../domain"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}

function extractError(messages: Array<{ message: string; errorCode: string }>): Error {
    const firstError = messages.find(m => m.errorCode && !m.errorCode.startsWith("0"))
    const error = new Error(firstError?.message || "Error desconocido")
    ;(error as Error & { errorCode: string }).errorCode = firstError?.errorCode ?? "5000"
    return error
}

class TrackingService {
    async registrarEvento(data: RegistrarEventoRequest): Promise<RegistrarEventoResponse> {
        const url = API_ROUTES.crowdpromotion.tracking.evento
        const response = await apiFetch<ServiceResponse<RegistrarEventoResponse>>(url, {
            method: "POST",
            data,
        })
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }

    async getMetricasPromotor(programaId?: string): Promise<PromotorMetricasResponse> {
        const params = new URLSearchParams()
        if (programaId) params.set("programaId", programaId)

        const query = params.toString()
        const url = query
            ? `${API_ROUTES.crowdpromotion.promotorMetricas}?${query}`
            : API_ROUTES.crowdpromotion.promotorMetricas
        const response = await apiFetch<ServiceResponse<PromotorMetricasResponse>>(url)
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }
}

export const trackingService = new TrackingService()
