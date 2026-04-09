import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import { getInscripcionErrorMessage } from "@shared/utils/error-messages"
import type {
    InscripcionesListResponse,
    InscripcionesFilters,
    InscripcionAprobada,
    InscripcionRechazada,
    InscripcionBloqueada,
    InscripcionDadaDeBaja,
    ServiceResponse,
} from "@shared/types"

class InscripcionService {
    async getInscripciones(
        programaId: string,
        filters?: InscripcionesFilters
    ): Promise<InscripcionesListResponse> {
        const searchParams = new URLSearchParams()
        if (filters?.estado) {
            searchParams.set("estado", filters.estado)
        }
        if (filters?.page) {
            searchParams.set("page", String(filters.page))
        }
        if (filters?.pageSize) {
            searchParams.set("pageSize", String(filters.pageSize))
        }

        const queryString = searchParams.toString()
        const baseUrl = API_ROUTES.crowdpromotion.programas.inscripciones(programaId)
        const url = `${baseUrl}${queryString ? `?${queryString}` : ""}`

        const response = await apiFetch<ServiceResponse<InscripcionesListResponse>>(url)

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorCode = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )?.errorCode
            throw new Error(getInscripcionErrorMessage(errorCode ?? "5000"))
        }

        return response.data
    }

    async aprobar(params: {
        programaId: string
        inscripcionId: string
    }): Promise<InscripcionAprobada> {
        const response = await apiFetch<ServiceResponse<InscripcionAprobada>>(
            API_ROUTES.crowdpromotion.programas.aprobar(params.programaId, params.inscripcionId),
            { method: "PATCH" }
        )

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorCode = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )?.errorCode
            throw new Error(getInscripcionErrorMessage(errorCode ?? "5000"))
        }

        return response.data
    }

    async rechazar(params: {
        programaId: string
        inscripcionId: string
    }): Promise<InscripcionRechazada> {
        const response = await apiFetch<ServiceResponse<InscripcionRechazada>>(
            API_ROUTES.crowdpromotion.programas.rechazar(params.programaId, params.inscripcionId),
            { method: "PATCH" }
        )

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorCode = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )?.errorCode
            throw new Error(getInscripcionErrorMessage(errorCode ?? "5000"))
        }

        return response.data
    }

    async bloquear(params: {
        programaId: string
        inscripcionId: string
    }): Promise<InscripcionBloqueada> {
        const response = await apiFetch<ServiceResponse<InscripcionBloqueada>>(
            API_ROUTES.crowdpromotion.programas.bloquear(params.programaId, params.inscripcionId),
            { method: "PATCH" }
        )

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorCode = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )?.errorCode
            throw new Error(getInscripcionErrorMessage(errorCode ?? "5000"))
        }

        return response.data
    }

    async darDeBaja(params: {
        programaId: string
        inscripcionId: string
    }): Promise<InscripcionDadaDeBaja> {
        const response = await apiFetch<ServiceResponse<InscripcionDadaDeBaja>>(
            API_ROUTES.crowdpromotion.programas.darDeBaja(params.programaId, params.inscripcionId),
            { method: "PATCH" }
        )

        if (response.messages?.some((m) => m.errorCode && !m.errorCode.startsWith("0"))) {
            const errorCode = response.messages.find(
                (m) => m.errorCode && !m.errorCode.startsWith("0")
            )?.errorCode
            throw new Error(getInscripcionErrorMessage(errorCode ?? "5000"))
        }

        return response.data
    }
}

export const inscripcionService = new InscripcionService()
