import { apiFetch } from "@/lib/api-client"
import type { ArtistaDto, CreateArtistaDto, UpdateArtistaDto, ServiceResponse } from "@shared/types"

class ArtistaService {
  private readonly baseUrl = "/artistas"

  async getMyProfile(): Promise<ArtistaDto | null> {
    try {
      const response = await apiFetch<ServiceResponse<ArtistaDto>>(`${this.baseUrl}/me`)
      return response.data
    } catch {
      return null
    }
  }

  async create(data: CreateArtistaDto): Promise<ArtistaDto> {
    const response = await apiFetch<ServiceResponse<ArtistaDto>>(this.baseUrl, {
      method: "POST",
      data,
    })
    return response.data
  }

  async update(id: string, data: UpdateArtistaDto): Promise<ArtistaDto> {
    const response = await apiFetch<ServiceResponse<ArtistaDto>>(`${this.baseUrl}/${id}`, {
      method: "PUT",
      data,
    })
    return response.data
  }
}

export const artistaService = new ArtistaService()
