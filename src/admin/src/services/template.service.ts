import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    PlantillaProyectoList,
    PlantillaProyecto,
    ServiceResponse,
    GenerarNecesidadesRequest,
    GenerarNecesidadesResult,
} from "@shared/types"

export interface CreateTemplateRequest {
    nombre: string
    descripcion?: string
    icono: string
    orden: number
    activo: boolean
    necesidades: CreateNecesidadRequest[]
}

export interface CreateNecesidadRequest {
    fase: string
    titulo: string
    descripcion?: string
    rolProfesionalId: number
    precioMinOrientativo?: number
    precioMaxOrientativo?: number
    prioridad: string
    orden: number
}

export type UpdateTemplateRequest = CreateTemplateRequest

class TemplateService {
    private readonly baseUrl = API_ROUTES.crowdsourcing.templates.base

    async getAll(): Promise<PlantillaProyectoList[]> {
        const response = await apiFetch<ServiceResponse<PlantillaProyectoList[]>>(
            this.baseUrl
        )
        return response.data
    }

    async getById(id: string): Promise<PlantillaProyecto | null> {
        try {
            const response = await apiFetch<ServiceResponse<PlantillaProyecto>>(
                API_ROUTES.crowdsourcing.templates.byId(id)
            )
            return response.data
        } catch {
            return null
        }
    }

    async create(data: CreateTemplateRequest): Promise<string> {
        const response = await apiFetch<ServiceResponse<string>>(this.baseUrl, {
            method: "POST",
            data,
        })
        return response.data
    }

    async update(id: string, data: UpdateTemplateRequest): Promise<void> {
        await apiFetch<ServiceResponse<string>>(
            API_ROUTES.crowdsourcing.templates.byId(id),
            {
                method: "PUT",
                data,
            }
        )
    }

    async delete(id: string): Promise<void> {
        await apiFetch(API_ROUTES.crowdsourcing.templates.byId(id), {
            method: "DELETE",
        })
    }

    async toggleStatus(id: string): Promise<void> {
        await apiFetch(
            `${API_ROUTES.crowdsourcing.templates.byId(id)}/toggle-status`,
            { method: "PATCH" }
        )
    }

    async generarNecesidades(
        templateId: string,
        data: GenerarNecesidadesRequest
    ): Promise<GenerarNecesidadesResult> {
        const response = await apiFetch<ServiceResponse<GenerarNecesidadesResult>>(
            API_ROUTES.crowdsourcing.templates.generar(templateId),
            {
                method: "POST",
                data,
            }
        )
        return response.data
    }
}

export const templateService = new TemplateService()
