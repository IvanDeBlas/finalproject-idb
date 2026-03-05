import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    CreateValoracionRequest,
    ValoracionCreatedResult,
    ValoracionesUsuario,
} from "../../domain"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}

class ValoracionApiService {
    async create(
        acuerdoId: string,
        data: CreateValoracionRequest
    ): Promise<ValoracionCreatedResult> {
        const response = await apiFetch<ServiceResponse<ValoracionCreatedResult>>(
            API_ROUTES.crowdsourcing.valoraciones.create(acuerdoId),
            { method: "POST", data }
        )

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async getByUser(
        userId: string,
        params?: { page?: number; pageSize?: number }
    ): Promise<ValoracionesUsuario> {
        const searchParams = new URLSearchParams()
        if (params?.page && params.page !== 1) {
            searchParams.set("page", String(params.page))
        }
        if (params?.pageSize && params.pageSize !== 10) {
            searchParams.set("pageSize", String(params.pageSize))
        }

        const queryString = searchParams.toString()
        const url = queryString
            ? `${API_ROUTES.crowdsourcing.valoraciones.byUser(userId)}?${queryString}`
            : API_ROUTES.crowdsourcing.valoraciones.byUser(userId)

        const response = await apiFetch<ServiceResponse<ValoracionesUsuario>>(url)

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }
}

export const valoracionApi = new ValoracionApiService()
