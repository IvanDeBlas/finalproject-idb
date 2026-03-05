import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    ProgramasExplorarResponse,
    MisProgramasResponse,
    InscripcionCreada,
    ExplorarProgramasFilters,
} from "../domain"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}

function extractError(messages: Array<{ message: string; errorCode: string }>): Error {
    const firstError = messages.find(m => m.errorCode && !m.errorCode.startsWith("0"))
    const error = new Error(firstError?.message || "Error desconocido")
    ;(error as Error & { errorCode: string }).errorCode = firstError?.errorCode ?? "5000"
    return error
}

class InscripcionService {
    async explorarProgramas(filters?: ExplorarProgramasFilters): Promise<ProgramasExplorarResponse> {
        const params = new URLSearchParams()
        if (filters?.artistaNombre) params.set("artistaNombre", filters.artistaNombre)
        if (filters?.tipoPromoId !== undefined) params.set("tipoPromoId", String(filters.tipoPromoId))
        params.set("page", String(filters?.page ?? 1))
        params.set("pageSize", String(filters?.pageSize ?? 10))

        const url = `${API_ROUTES.crowdpromotion.programas.explorar}?${params.toString()}`
        const response = await apiFetch<ServiceResponse<ProgramasExplorarResponse>>(url)
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }

    async misProgramas(page: number = 1, pageSize: number = 50): Promise<MisProgramasResponse> {
        const params = new URLSearchParams()
        params.set("page", String(page))
        params.set("pageSize", String(pageSize))

        const url = `${API_ROUTES.crowdpromotion.promotorInscripciones.misProgramas}?${params.toString()}`
        const response = await apiFetch<ServiceResponse<MisProgramasResponse>>(url)
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }

    async solicitarInscripcion(programaId: string): Promise<InscripcionCreada> {
        const url = API_ROUTES.crowdpromotion.programas.inscripcion(programaId)
        const response = await apiFetch<ServiceResponse<InscripcionCreada>>(url, {
            method: "POST",
        })
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }
}

export const inscripcionService = new InscripcionService()
