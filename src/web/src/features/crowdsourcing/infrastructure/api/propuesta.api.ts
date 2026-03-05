import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    CreatePropuestaRequest,
    PropuestaCreatedResult,
    MiPropuestaList,
    RetirarPropuestaResult,
} from "../../domain"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}

interface PaginatedResponse<T> {
    items: T[]
    totalCount: number
    page: number
    pageSize: number
    totalPages: number
}

class PropuestaApiService {
    async create(
        necesidadId: string,
        data: CreatePropuestaRequest
    ): Promise<PropuestaCreatedResult> {
        const response = await apiFetch<
            ServiceResponse<PropuestaCreatedResult>
        >(API_ROUTES.crowdsourcing.necesidades.propuestas(necesidadId), {
            method: "POST",
            data,
        })

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async getMisPropuestas(filters: {
        estado?: number
        page?: number
        pageSize?: number
    }): Promise<PaginatedResponse<MiPropuestaList>> {
        const params = new URLSearchParams()

        if (filters.estado !== undefined)
            params.set("estado", String(filters.estado))
        if (filters.page !== undefined) params.set("page", String(filters.page))
        if (filters.pageSize !== undefined)
            params.set("pageSize", String(filters.pageSize))

        const queryString = params.toString()
        const url = queryString
            ? `${API_ROUTES.crowdsourcing.propuestas.mis}?${queryString}`
            : API_ROUTES.crowdsourcing.propuestas.mis

        const response = await apiFetch<
            ServiceResponse<PaginatedResponse<MiPropuestaList>>
        >(url)

        if (!response.data) {
            return { items: [], totalCount: 0, page: 1, pageSize: 10, totalPages: 0 }
        }

        return response.data
    }

    async retirar(propuestaId: string): Promise<RetirarPropuestaResult> {
        const response = await apiFetch<
            ServiceResponse<RetirarPropuestaResult>
        >(API_ROUTES.crowdsourcing.propuestas.retirar(propuestaId), {
            method: "PATCH",
        })

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }
}

export const propuestaApi = new PropuestaApiService()
