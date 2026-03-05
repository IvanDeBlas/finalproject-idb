import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    RolProfesionalConCategoria,
    CategoriaRol,
    MaestraTipoNecesidad,
    MaestraModalidadTrabajo,
    MaestraMoneda,
    ServiceResponse,
} from "@shared/types"

class MaestrasService {
    async getRolesProfesionales(): Promise<RolProfesionalConCategoria[]> {
        const response = await apiFetch<ServiceResponse<RolProfesionalConCategoria[]>>(
            API_ROUTES.crowdsourcing.maestras.rolesProfesionales
        )
        return response.data
    }

    async getCategoriasRol(): Promise<CategoriaRol[]> {
        const response = await apiFetch<ServiceResponse<CategoriaRol[]>>(
            API_ROUTES.crowdsourcing.maestras.categoriasRol
        )
        return response.data
    }

    async getTiposNecesidad(): Promise<MaestraTipoNecesidad[]> {
        const response = await apiFetch<ServiceResponse<MaestraTipoNecesidad[]>>(
            API_ROUTES.crowdsourcing.maestras.tiposNecesidad
        )
        return response.data
    }

    async getModalidadesTrabajo(): Promise<MaestraModalidadTrabajo[]> {
        const response = await apiFetch<ServiceResponse<MaestraModalidadTrabajo[]>>(
            API_ROUTES.crowdsourcing.maestras.modalidadesTrabajo
        )
        return response.data
    }

    async getMonedas(): Promise<MaestraMoneda[]> {
        const response = await apiFetch<ServiceResponse<MaestraMoneda[]>>(
            API_ROUTES.crowdsourcing.maestras.monedas
        )
        return response.data
    }
}

export const maestrasService = new MaestrasService()
