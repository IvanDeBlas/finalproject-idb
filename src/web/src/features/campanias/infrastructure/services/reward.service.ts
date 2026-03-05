import { apiFetch } from "@/lib/api-client"
import type { Reward } from "@shared/types/reward"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
    isSuccess?: boolean
}

interface RewardFilters {
    esActivo?: boolean
    esAddOn?: boolean
    campaniaId?: string
}

class RewardService {
    private readonly baseUrl = "/rewards"

    async getAll(filters?: RewardFilters): Promise<Reward[]> {
        const params = new URLSearchParams()

        if (filters?.campaniaId) {
            params.append("campaniaId", filters.campaniaId)
        }
        if (filters?.esActivo !== undefined) {
            params.append("esActivo", filters.esActivo.toString())
        }
        if (filters?.esAddOn !== undefined) {
            params.append("esAddOn", filters.esAddOn.toString())
        }

        const queryString = params.toString()
        const url = queryString ? `${this.baseUrl}?${queryString}` : this.baseUrl

        const response = await apiFetch<ServiceResponse<Reward[]>>(url)
        return response.data ?? []
    }

    async getByCampaniaId(
        campaniaId: string,
        filters?: Omit<RewardFilters, "campaniaId">
    ): Promise<Reward[]> {
        return this.getAll({ ...filters, campaniaId })
    }

    async getById(id: string): Promise<Reward | null> {
        try {
            const response = await apiFetch<ServiceResponse<Reward>>(
                `${this.baseUrl}/${id}`
            )
            return response.data ?? null
        } catch {
            return null
        }
    }
}

export const rewardService = new RewardService()
