import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    CreateConversacionRequest,
    CreateConversacionResult,
    ConversacionListResponse,
    NoLeidosCountResponse,
} from "../../domain"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}

class ConversacionApiService {
    async getAll(params?: {
        contexto?: string
        page?: number
        pageSize?: number
    }): Promise<ConversacionListResponse> {
        const searchParams = new URLSearchParams()
        if (params?.contexto && params.contexto !== "todas") {
            searchParams.set("contexto", params.contexto)
        }
        if (params?.page) {
            searchParams.set("page", String(params.page))
        }
        if (params?.pageSize) {
            searchParams.set("pageSize", String(params.pageSize))
        }

        const queryString = searchParams.toString()
        const url = queryString
            ? `${API_ROUTES.crowdsourcing.conversaciones.base}?${queryString}`
            : API_ROUTES.crowdsourcing.conversaciones.base

        const response = await apiFetch<ServiceResponse<ConversacionListResponse>>(url)

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async create(data: CreateConversacionRequest): Promise<CreateConversacionResult> {
        const response = await apiFetch<ServiceResponse<CreateConversacionResult>>(
            API_ROUTES.crowdsourcing.conversaciones.base,
            { method: "POST", data }
        )

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async getNoLeidosCount(): Promise<NoLeidosCountResponse> {
        const response = await apiFetch<ServiceResponse<NoLeidosCountResponse>>(
            API_ROUTES.crowdsourcing.conversaciones.noLeidos
        )

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }
}

export const conversacionApi = new ConversacionApiService()
