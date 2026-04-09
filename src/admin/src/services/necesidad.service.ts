import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    NecesidadCrowdsourcingList,
    NecesidadCrowdsourcing,
    CreateNecesidadRequest,
    UpdateNecesidadRequest,
    CerrarNecesidadRequest,
    NecesidadCreateResult,
    NecesidadUpdateResult,
    CerrarNecesidadResult,
    ServiceResponse,
    PaginatedResponse,
} from "@shared/types"

interface GetMisNecesidadesParams {
    page?: number
    pageSize?: number
    estado?: number
    search?: string
}

class NecesidadService {
    async getMisNecesidades(
        params: GetMisNecesidadesParams = {}
    ): Promise<PaginatedResponse<NecesidadCrowdsourcingList>> {
        const queryParams = new URLSearchParams()
        if (params.page) queryParams.append("page", params.page.toString())
        if (params.pageSize) queryParams.append("pageSize", params.pageSize.toString())
        if (params.estado) queryParams.append("estado", params.estado.toString())
        if (params.search) queryParams.append("search", params.search)

        const qs = queryParams.toString()
        const url = `${API_ROUTES.crowdsourcing.necesidades.mis}${qs ? `?${qs}` : ""}`
        const response = await apiFetch<ServiceResponse<PaginatedResponse<NecesidadCrowdsourcingList>>>(url)
        return response.data
    }

    async getById(id: string): Promise<NecesidadCrowdsourcing | null> {
        try {
            const response = await apiFetch<ServiceResponse<NecesidadCrowdsourcing>>(
                API_ROUTES.crowdsourcing.necesidades.byId(id)
            )
            return response.data
        } catch {
            return null
        }
    }

    async create(data: CreateNecesidadRequest): Promise<NecesidadCreateResult> {
        const response = await apiFetch<ServiceResponse<NecesidadCreateResult>>(
            API_ROUTES.crowdsourcing.necesidades.base,
            {
                method: "POST",
                data,
            }
        )
        return response.data
    }

    async update(id: string, data: UpdateNecesidadRequest): Promise<NecesidadUpdateResult> {
        const response = await apiFetch<ServiceResponse<NecesidadUpdateResult>>(
            API_ROUTES.crowdsourcing.necesidades.byId(id),
            {
                method: "PUT",
                data,
            }
        )
        return response.data
    }

    async cerrar(id: string, data: CerrarNecesidadRequest): Promise<CerrarNecesidadResult> {
        const response = await apiFetch<ServiceResponse<CerrarNecesidadResult>>(
            API_ROUTES.crowdsourcing.necesidades.cerrar(id),
            {
                method: "PATCH",
                data,
            }
        )
        return response.data
    }
}

export const necesidadService = new NecesidadService()
