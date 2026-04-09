import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    CreateMensajeRequest,
    Mensaje,
    MensajeListResponse,
    MarcarLeidosResponse,
} from "../../domain"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}

class MensajeApiService {
    async getByConversacion(
        conversacionId: string,
        params?: { page?: number; pageSize?: number }
    ): Promise<MensajeListResponse> {
        const searchParams = new URLSearchParams()
        if (params?.page) {
            searchParams.set("page", String(params.page))
        }
        if (params?.pageSize) {
            searchParams.set("pageSize", String(params.pageSize))
        }

        const baseUrl = API_ROUTES.crowdsourcing.conversaciones.mensajes(conversacionId)
        const queryString = searchParams.toString()
        const url = queryString ? `${baseUrl}?${queryString}` : baseUrl

        const response = await apiFetch<ServiceResponse<MensajeListResponse>>(url)

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async create(
        conversacionId: string,
        data: CreateMensajeRequest
    ): Promise<Mensaje> {
        const response = await apiFetch<ServiceResponse<Mensaje>>(
            API_ROUTES.crowdsourcing.conversaciones.mensajes(conversacionId),
            { method: "POST", data }
        )

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async marcarLeidos(conversacionId: string): Promise<MarcarLeidosResponse> {
        const response = await apiFetch<ServiceResponse<MarcarLeidosResponse>>(
            API_ROUTES.crowdsourcing.conversaciones.marcarLeidos(conversacionId),
            { method: "PATCH" }
        )

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }
}

export const mensajeApi = new MensajeApiService()
