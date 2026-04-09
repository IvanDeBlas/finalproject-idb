import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import { getPromotorErrorMessage } from "@shared/utils/error-messages"
import type {
    Promotor,
    UpdatePromotorRequest,
    PromotorUpdatedResult,
    PromotorDesactivadoResult,
} from "@shared/types"
import type { ServiceResponse } from "@shared/types"

class PromotorService {
    async getMe(): Promise<Promotor | null> {
        try {
            const response = await apiFetch<ServiceResponse<Promotor>>(
                API_ROUTES.crowdpromotion.promotor.me
            )
            return response.data
        } catch {
            return null
        }
    }

    async update(data: UpdatePromotorRequest): Promise<PromotorUpdatedResult> {
        const response = await apiFetch<ServiceResponse<PromotorUpdatedResult>>(
            API_ROUTES.crowdpromotion.promotor.me,
            {
                method: "PUT",
                data,
            }
        )

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorCode = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )?.errorCode
            throw new Error(getPromotorErrorMessage(errorCode ?? "5000"))
        }

        return response.data
    }

    async desactivar(): Promise<PromotorDesactivadoResult> {
        const response = await apiFetch<ServiceResponse<PromotorDesactivadoResult>>(
            API_ROUTES.crowdpromotion.promotor.desactivar,
            {
                method: "PATCH",
            }
        )

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorCode = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )?.errorCode
            throw new Error(getPromotorErrorMessage(errorCode ?? "5000"))
        }

        return response.data
    }
}

export const promotorService = new PromotorService()
