import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import { getTareaPromocionErrorMessage } from "@shared/utils/error-messages"
import type {
    TareasPendientesResponse,
    ValidarTareaRequest,
    ValidarTareaResponse,
    RechazarTareaRequest,
    RechazarTareaResponse,
} from "@shared/types/crowdpromotion"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}

class TareasService {
    async getTareasPendientes(
        programaId: string,
        params?: { page?: number; pageSize?: number }
    ): Promise<TareasPendientesResponse> {
        const searchParams = new URLSearchParams()
        if (params?.page) searchParams.set("page", String(params.page))
        if (params?.pageSize) searchParams.set("pageSize", String(params.pageSize))

        const queryString = searchParams.toString()
        const baseUrl = API_ROUTES.crowdpromotion.programas.tareasPendientes(programaId)
        const url = queryString ? `${baseUrl}?${queryString}` : baseUrl

        const response = await apiFetch<ServiceResponse<TareasPendientesResponse>>(url)

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorCode = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )?.errorCode
            throw new Error(getTareaPromocionErrorMessage(errorCode ?? "5000"))
        }

        return response.data
    }

    async validarTarea(
        programaId: string,
        tareaPromotorId: string,
        data: ValidarTareaRequest
    ): Promise<ValidarTareaResponse> {
        const url = API_ROUTES.crowdpromotion.programas.validarTarea(programaId, tareaPromotorId)

        const response = await apiFetch<ServiceResponse<ValidarTareaResponse>>(url, {
            method: "PATCH",
            data,
        })

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorCode = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )?.errorCode
            throw new Error(getTareaPromocionErrorMessage(errorCode ?? "5000"))
        }

        return response.data
    }

    async rechazarTarea(
        programaId: string,
        tareaPromotorId: string,
        data: RechazarTareaRequest
    ): Promise<RechazarTareaResponse> {
        const url = API_ROUTES.crowdpromotion.programas.rechazarTarea(programaId, tareaPromotorId)

        const response = await apiFetch<ServiceResponse<RechazarTareaResponse>>(url, {
            method: "PATCH",
            data,
        })

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorCode = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )?.errorCode
            throw new Error(getTareaPromocionErrorMessage(errorCode ?? "5000"))
        }

        return response.data
    }
}

export const tareasService = new TareasService()
