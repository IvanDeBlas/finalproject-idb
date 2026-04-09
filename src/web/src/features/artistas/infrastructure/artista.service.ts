import { apiFetch } from "@/lib/api-client"
import type { Artista, IArtistaRepository, CreateArtistaData, UpdateArtistaData } from "../domain"

interface ServiceResponse<T> {
  data: T
  messages: Array<{ message: string; errorCode: string }>
}

interface ArtistaDto {
  id: string
  userId: string
  nombreArtistico: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
  generoMusical?: string
  createdAt: string
  updatedAt: string
}

function mapDtoToDomain(dto: ArtistaDto): Artista {
  return {
    ...dto,
    createdAt: new Date(dto.createdAt),
    updatedAt: new Date(dto.updatedAt),
  }
}

class ArtistaService implements IArtistaRepository {
  private readonly baseUrl = "/artistas"

  async getById(id: string): Promise<Artista> {
    const response = await apiFetch<ServiceResponse<ArtistaDto>>(`${this.baseUrl}/${id}`)
    return mapDtoToDomain(response.data)
  }

  async getMyProfile(): Promise<Artista | null> {
    try {
      const response = await apiFetch<ServiceResponse<ArtistaDto>>(`${this.baseUrl}/me`)
      return mapDtoToDomain(response.data)
    } catch {
      return null
    }
  }

  async create(data: CreateArtistaData): Promise<Artista> {
    const response = await apiFetch<ServiceResponse<ArtistaDto>>(this.baseUrl, {
      method: "POST",
      data,
    })
    return mapDtoToDomain(response.data)
  }

  async update(id: string, data: UpdateArtistaData): Promise<Artista> {
    const response = await apiFetch<ServiceResponse<ArtistaDto>>(`${this.baseUrl}/${id}`, {
      method: "PUT",
      data,
    })
    return mapDtoToDomain(response.data)
  }
}

export const artistaService = new ArtistaService()
