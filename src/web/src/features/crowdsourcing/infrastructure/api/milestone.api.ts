import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    CreateMilestoneRequest,
    MilestoneCreatedResult,
} from "../../domain"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}

class MilestoneApiService {
    async create(
        acuerdoId: string,
        data: CreateMilestoneRequest
    ): Promise<MilestoneCreatedResult> {
        const response = await apiFetch<
            ServiceResponse<MilestoneCreatedResult>
        >(API_ROUTES.crowdsourcing.acuerdos.milestones(acuerdoId), {
            method: "POST",
            data,
        })

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async update(
        acuerdoId: string,
        milestoneId: string,
        data: CreateMilestoneRequest
    ): Promise<MilestoneCreatedResult> {
        const response = await apiFetch<
            ServiceResponse<MilestoneCreatedResult>
        >(API_ROUTES.crowdsourcing.acuerdos.milestoneById(acuerdoId, milestoneId), {
            method: "PUT",
            data,
        })

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async delete(acuerdoId: string, milestoneId: string): Promise<void> {
        const response = await apiFetch<
            ServiceResponse<null>
        >(API_ROUTES.crowdsourcing.acuerdos.milestoneById(acuerdoId, milestoneId), {
            method: "DELETE",
        })

        const hasError = response.messages?.some(
            (m) => m.errorCode && !m.errorCode.startsWith("0")
        )
        if (hasError) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }
    }
}

export const milestoneApi = new MilestoneApiService()
