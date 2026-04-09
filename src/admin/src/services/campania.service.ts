import { apiFetch } from "@/lib/api-client"
import type {
    Campania,
    CampaniaListItem,
    CreateCampaniaRequest,
    UpdateCampaniaRequest,
    PublishCampaniaResponse,
    ServiceResponse,
    BackingPublicDto,
    CampaniaStats,
} from "@shared/types"

class CampaniaService {
    private readonly baseUrl = "/campanias"

    async getAll(): Promise<Campania[]> {
        const response = await apiFetch<ServiceResponse<Campania[]>>(this.baseUrl)
        return response.data
    }

    async getById(id: string): Promise<Campania | null> {
        try {
            const response = await apiFetch<ServiceResponse<Campania>>(
                `${this.baseUrl}/${id}`
            )
            return response.data
        } catch {
            return null
        }
    }

    async getMisCampanias(): Promise<CampaniaListItem[]> {
        const response = await apiFetch<ServiceResponse<CampaniaListItem[]>>(
            `${this.baseUrl}/mis-campanias`
        )
        return response.data
    }

    async create(data: CreateCampaniaRequest): Promise<Campania> {
        const response = await apiFetch<ServiceResponse<Campania>>(this.baseUrl, {
            method: "POST",
            data,
        })
        return response.data
    }

    async update(id: string, data: UpdateCampaniaRequest): Promise<Campania> {
        const response = await apiFetch<ServiceResponse<Campania>>(
            `${this.baseUrl}/${id}`,
            {
                method: "PUT",
                data,
            }
        )
        return response.data
    }

    async publicar(id: string): Promise<PublishCampaniaResponse> {
        const response = await apiFetch<ServiceResponse<PublishCampaniaResponse>>(
            `${this.baseUrl}/${id}/publicar`,
            { method: "POST" }
        )
        return response.data
    }

    async delete(id: string): Promise<void> {
        await apiFetch(`${this.baseUrl}/${id}`, { method: "DELETE" })
    }

    async getBackings(
        campaniaId: string,
        params?: { pageNumber?: number; pageSize?: number }
    ): Promise<BackingPublicDto[]> {
        const queryParams = new URLSearchParams()
        if (params?.pageNumber)
            queryParams.set("pageNumber", String(params.pageNumber))
        if (params?.pageSize)
            queryParams.set("pageSize", String(params.pageSize))

        const qs = queryParams.toString()
        const url = `${this.baseUrl}/${campaniaId}/backings${qs ? `?${qs}` : ""}`
        const response = await apiFetch<ServiceResponse<BackingPublicDto[]>>(url)
        return response.data
    }

    async getStats(campaniaId: string): Promise<CampaniaStats> {
        const response = await apiFetch<ServiceResponse<CampaniaStats>>(
            `${this.baseUrl}/${campaniaId}/stats`
        )
        return response.data
    }
}

export const campaniaService = new CampaniaService()
