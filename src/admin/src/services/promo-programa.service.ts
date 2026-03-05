import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import { getPromoProgramaErrorMessage } from "@shared/utils/error-messages"
import type {
    CreatePromoProgramaRequest,
    UpdatePromoProgramaRequest,
    PromoProgramaCreatedResult,
    PromoProgramaListResult,
    PromoProgramaDetail,
    PromoProgramaUpdatedResult,
    PromoProgramaDesactivadoResult,
    ServiceResponse,
} from "@shared/types"

interface MisProgramasParams {
    esActivo?: boolean
    page?: number
    pageSize?: number
}

class PromoProgramaService {
    async getMisProgramas(params: MisProgramasParams = {}): Promise<PromoProgramaListResult> {
        const searchParams = new URLSearchParams()
        if (params.esActivo !== undefined) {
            searchParams.set("esActivo", String(params.esActivo))
        }
        if (params.page) {
            searchParams.set("page", String(params.page))
        }
        if (params.pageSize) {
            searchParams.set("pageSize", String(params.pageSize))
        }

        const queryString = searchParams.toString()
        const url = `${API_ROUTES.crowdpromotion.programas.mis}${queryString ? `?${queryString}` : ""}`

        const response = await apiFetch<ServiceResponse<PromoProgramaListResult>>(url)
        return response.data
    }

    async getById(id: string): Promise<PromoProgramaDetail | null> {
        try {
            const response = await apiFetch<ServiceResponse<PromoProgramaDetail>>(
                API_ROUTES.crowdpromotion.programas.byId(id)
            )
            return response.data
        } catch {
            return null
        }
    }

    async create(data: CreatePromoProgramaRequest): Promise<PromoProgramaCreatedResult> {
        const response = await apiFetch<ServiceResponse<PromoProgramaCreatedResult>>(
            API_ROUTES.crowdpromotion.programas.base,
            { method: "POST", data }
        )

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorCode = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )?.errorCode
            throw new Error(getPromoProgramaErrorMessage(errorCode ?? "5000"))
        }

        return response.data
    }

    async update(id: string, data: UpdatePromoProgramaRequest): Promise<PromoProgramaUpdatedResult> {
        const response = await apiFetch<ServiceResponse<PromoProgramaUpdatedResult>>(
            API_ROUTES.crowdpromotion.programas.byId(id),
            { method: "PUT", data }
        )

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorCode = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )?.errorCode
            throw new Error(getPromoProgramaErrorMessage(errorCode ?? "5000"))
        }

        return response.data
    }

    async desactivar(id: string): Promise<PromoProgramaDesactivadoResult> {
        const response = await apiFetch<ServiceResponse<PromoProgramaDesactivadoResult>>(
            API_ROUTES.crowdpromotion.programas.desactivar(id),
            { method: "PATCH" }
        )

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorCode = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )?.errorCode
            throw new Error(getPromoProgramaErrorMessage(errorCode ?? "5000"))
        }

        return response.data
    }
}

export const promoProgramaService = new PromoProgramaService()
