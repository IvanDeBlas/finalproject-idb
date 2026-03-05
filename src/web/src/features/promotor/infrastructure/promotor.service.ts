import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    Promotor,
    PromotorCreatedResult,
    PromotorUpdatedResult,
    PromotorDesactivadoResult,
    CreatePromotorRequest,
    UpdatePromotorRequest,
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

class PromotorService {
    async getMe(): Promise<Promotor> {
        const response = await apiFetch<ServiceResponse<Promotor>>(
            API_ROUTES.crowdpromotion.promotor.me
        )
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }

    async create(data: CreatePromotorRequest): Promise<PromotorCreatedResult> {
        const response = await apiFetch<ServiceResponse<PromotorCreatedResult>>(
            API_ROUTES.crowdpromotion.promotor.base,
            { method: "POST", data }
        )
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }

    async update(data: UpdatePromotorRequest): Promise<PromotorUpdatedResult> {
        const response = await apiFetch<ServiceResponse<PromotorUpdatedResult>>(
            API_ROUTES.crowdpromotion.promotor.me,
            { method: "PUT", data }
        )
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }

    async desactivar(): Promise<PromotorDesactivadoResult> {
        const response = await apiFetch<ServiceResponse<PromotorDesactivadoResult>>(
            API_ROUTES.crowdpromotion.promotor.desactivar,
            { method: "PATCH" }
        )
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }
}

export const promotorService = new PromotorService()
