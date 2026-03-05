import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    NecesidadPublicaList,
    NecesidadPublica,
    NecesidadesPublicasFilter,
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

class NecesidadPublicaApiService {
    async getAll(
        filters: NecesidadesPublicasFilter
    ): Promise<PaginatedResponse<NecesidadPublicaList>> {
        const params = new URLSearchParams()

        if (filters.search) params.set("search", filters.search)
        if (filters.tipoNecesidadId !== undefined)
            params.set("tipoNecesidadId", String(filters.tipoNecesidadId))
        if (filters.modalidad !== undefined)
            params.set("modalidad", String(filters.modalidad))
        if (filters.presupuestoMin !== undefined)
            params.set("presupuestoMin", String(filters.presupuestoMin))
        if (filters.presupuestoMax !== undefined)
            params.set("presupuestoMax", String(filters.presupuestoMax))
        if (filters.pais) params.set("pais", filters.pais)
        if (filters.orderBy) params.set("orderBy", filters.orderBy)
        if (filters.page !== undefined) params.set("page", String(filters.page))
        if (filters.pageSize !== undefined)
            params.set("pageSize", String(filters.pageSize))

        const queryString = params.toString()
        const url = queryString
            ? `${API_ROUTES.crowdsourcing.necesidades.base}?${queryString}`
            : API_ROUTES.crowdsourcing.necesidades.base

        const response = await apiFetch<
            ServiceResponse<PaginatedResponse<NecesidadPublicaList>>
        >(url)

        if (!response.data) {
            return { items: [], totalCount: 0, page: 1, pageSize: 12, totalPages: 0 }
        }

        return response.data
    }

    async getById(id: string): Promise<NecesidadPublica> {
        const response = await apiFetch<ServiceResponse<NecesidadPublica>>(
            API_ROUTES.crowdsourcing.necesidades.byId(id)
        )

        if (!response.data) {
            const errorMsg =
                response.messages?.[0]?.message ?? "Necesidad no encontrada"
            throw new Error(errorMsg)
        }

        return response.data
    }
}

export const necesidadPublicaApi = new NecesidadPublicaApiService()
