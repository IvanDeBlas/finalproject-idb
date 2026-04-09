import { apiFetch } from "@/lib/api-client"
import type {
    Reward,
    CreateRewardRequest,
    UpdateRewardRequest,
    ReorderRewardsRequest,
    ServiceResponse,
} from "@shared/types"

class RewardService {
    private readonly baseUrl = "/rewards"

    async getByCampania(campaniaId: string): Promise<Reward[]> {
        const response = await apiFetch<ServiceResponse<Reward[]>>(
            `${this.baseUrl}?campaniaId=${campaniaId}`
        )
        return response.data ?? []
    }

    async getById(id: string): Promise<Reward | null> {
        try {
            const response = await apiFetch<ServiceResponse<Reward>>(
                `${this.baseUrl}/${id}`
            )
            return response.data
        } catch {
            return null
        }
    }

    async create(data: CreateRewardRequest): Promise<Reward> {
        const response = await apiFetch<ServiceResponse<Reward>>(
            this.baseUrl,
            {
                method: "POST",
                data,
            }
        )
        return response.data
    }

    async update(id: string, data: UpdateRewardRequest): Promise<Reward> {
        const response = await apiFetch<ServiceResponse<Reward>>(
            `${this.baseUrl}/${id}`,
            {
                method: "PUT",
                data,
            }
        )
        return response.data
    }

    async delete(id: string): Promise<void> {
        await apiFetch(`${this.baseUrl}/${id}`, { method: "DELETE" })
    }

    async reorder(data: ReorderRewardsRequest): Promise<void> {
        await apiFetch(`${this.baseUrl}/reorder`, {
            method: "PUT",
            data,
        })
    }
}

export const rewardService = new RewardService()
