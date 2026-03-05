import { apiFetch } from "@/lib/api-client"
import type {
    BackingDto,
    BackingPublicDto,
    CreateBackingRequest,
} from "@shared/types/backing"
import type { PaginationParams } from "../domain/types"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}

class BackingService {
    async create(
        data: CreateBackingRequest & { campaniaId: string }
    ): Promise<BackingDto> {
        const { campaniaId, ...backingData } = data

        const response = await apiFetch<ServiceResponse<BackingDto>>(
            `/campanias/${campaniaId}/backings`,
            {
                method: "POST",
                data: backingData,
            }
        )

        if (!response.data) {
            const errorMsg = response.messages?.[0]?.message || "Error al crear backing"
            throw new Error(errorMsg)
        }

        return response.data
    }

    async getByCampaniaId(
        campaniaId: string,
        options?: PaginationParams
    ): Promise<BackingPublicDto[]> {
        const params = new URLSearchParams()
        if (options?.pageNumber) params.append("pageNumber", String(options.pageNumber))
        if (options?.pageSize) params.append("pageSize", String(options.pageSize))

        const queryString = params.toString() ? `?${params.toString()}` : ""

        const response = await apiFetch<ServiceResponse<BackingPublicDto[]>>(
            `/campanias/${campaniaId}/backings${queryString}`
        )

        return response.data || []
    }

    async getMyBackings(): Promise<BackingDto[]> {
        const response = await apiFetch<ServiceResponse<BackingDto[]>>(
            "/backings/me"
        )

        return response.data || []
    }
}

export const backingService = new BackingService()
