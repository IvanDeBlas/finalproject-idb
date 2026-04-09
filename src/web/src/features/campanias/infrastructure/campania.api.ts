import { apiFetch } from "@/lib/api-client"
import { CAMPANIA_ESTADOS } from "@/lib/constants"
import type {
    Campania,
    CampaniaListItem,
    ICampaniaRepository,
    CreateCampaniaData,
    UpdateCampaniaData,
    CampaniaFilters,
    PaginationParams,
} from "../domain"
import type { CampaniaDto, CampaniaListItemDto } from "./dtos"
import {
    mapCampaniaDtoToDomain,
    mapCampaniaListItemDtoToDomain,
    mapCreateCampaniaToDto,
    mapUpdateCampaniaToDto,
} from "./mappers"

// ServiceResponse wrapper from backend
interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
    isSuccess?: boolean
}

class CampaniaApiService implements ICampaniaRepository {
    private readonly baseUrl = "/campanias"

    async getAll(
        filters?: CampaniaFilters,
        pagination?: PaginationParams
    ): Promise<CampaniaListItem[]> {
        const params = new URLSearchParams()

        if (filters?.searchTerm) {
            params.append("searchTerm", filters.searchTerm)
        }
        if (filters?.artistaId) {
            params.append("artistaId", filters.artistaId)
        }
        if (filters?.estadoCampaniaId !== undefined) {
            params.append("estadoCampaniaId", filters.estadoCampaniaId.toString())
        } else {
            params.append("estadoCampaniaId", CAMPANIA_ESTADOS.PUBLICADA.toString())
        }

        if (pagination?.pageNumber) {
            params.append("pageNumber", pagination.pageNumber.toString())
        }
        if (pagination?.pageSize) {
            params.append("pageSize", pagination.pageSize.toString())
        }

        const queryString = params.toString()
        const url = queryString ? `${this.baseUrl}?${queryString}` : this.baseUrl

        try {
            const response = await apiFetch<ServiceResponse<CampaniaListItemDto[]>>(url)
            const items = response.data ?? []
            return items.map(mapCampaniaListItemDtoToDomain)
        } catch {
            // Fallback: try fetching without filters for backward compat
            const response = await apiFetch<ServiceResponse<CampaniaDto[]>>(this.baseUrl)
            const items = response.data ?? []
            return items.map((dto) => mapCampaniaListItemDtoToDomain(dto as unknown as CampaniaListItemDto))
        }
    }

    async getById(id: string): Promise<Campania | null> {
        try {
            const response = await apiFetch<ServiceResponse<CampaniaDto>>(`${this.baseUrl}/${id}`)
            if (!response.data) return null
            return mapCampaniaDtoToDomain(response.data)
        } catch {
            return null
        }
    }

    async getByArtistaId(artistaId: string): Promise<CampaniaListItem[]> {
        const response = await apiFetch<ServiceResponse<CampaniaListItemDto[]>>(
            `${this.baseUrl}?artistaId=${artistaId}`
        )
        const items = response.data ?? []
        return items.map(mapCampaniaListItemDtoToDomain)
    }

    async create(data: CreateCampaniaData): Promise<Campania> {
        const dto = mapCreateCampaniaToDto(data)
        const response = await apiFetch<ServiceResponse<CampaniaDto>>(this.baseUrl, {
            method: "POST",
            data: dto,
        })
        return mapCampaniaDtoToDomain(response.data)
    }

    async update(id: string, data: UpdateCampaniaData): Promise<Campania> {
        const dto = mapUpdateCampaniaToDto(data)
        const response = await apiFetch<ServiceResponse<CampaniaDto>>(`${this.baseUrl}/${id}`, {
            method: "PUT",
            data: dto,
        })
        return mapCampaniaDtoToDomain(response.data)
    }

    async publicar(id: string): Promise<Campania> {
        const response = await apiFetch<ServiceResponse<CampaniaDto>>(
            `${this.baseUrl}/${id}/publicar`,
            { method: "POST" }
        )
        return mapCampaniaDtoToDomain(response.data)
    }
}

export const campaniaApi = new CampaniaApiService()

// Backward compat alias
export const campaniaService = campaniaApi
