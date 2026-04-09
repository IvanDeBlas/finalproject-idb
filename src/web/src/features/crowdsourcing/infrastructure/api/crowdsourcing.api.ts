import { apiFetch } from "@/lib/api-client"
import type {
    PlantillaProyectoList,
    PlantillaProyecto,
    GenerarNecesidadesRequest,
    GenerarNecesidadesResult,
    RolProfesionalConCategoria,
    CategoriaRol,
} from "../../domain"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}

class CrowdsourcingApiService {
    private readonly baseUrl = "/crowdsourcing"

    async getTemplates(): Promise<PlantillaProyectoList[]> {
        const response = await apiFetch<ServiceResponse<PlantillaProyectoList[]>>(
            `${this.baseUrl}/templates`
        )
        return response.data ?? []
    }

    async getTemplateById(id: string): Promise<PlantillaProyecto> {
        const response = await apiFetch<ServiceResponse<PlantillaProyecto>>(
            `${this.baseUrl}/templates/${id}`
        )
        if (!response.data) {
            throw new Error("Template not found")
        }
        return response.data
    }

    async generarNecesidades(
        templateId: string,
        data: GenerarNecesidadesRequest
    ): Promise<GenerarNecesidadesResult> {
        const response = await apiFetch<ServiceResponse<GenerarNecesidadesResult>>(
            `${this.baseUrl}/templates/${templateId}/generar`,
            {
                method: "POST",
                data,
            }
        )
        if (!response.data) {
            throw new Error("Failed to generate necesidades")
        }
        return response.data
    }

    async getRolesProfesionales(): Promise<RolProfesionalConCategoria[]> {
        const response = await apiFetch<ServiceResponse<RolProfesionalConCategoria[]>>(
            `${this.baseUrl}/maestras/roles-profesionales`
        )
        return response.data ?? []
    }

    async getCategoriasRol(): Promise<CategoriaRol[]> {
        const response = await apiFetch<ServiceResponse<CategoriaRol[]>>(
            `${this.baseUrl}/maestras/categorias-rol`
        )
        return response.data ?? []
    }
}

export const crowdsourcingApi = new CrowdsourcingApiService()
