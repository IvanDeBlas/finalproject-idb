import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    MisTareasResponse,
    CompletarTareaRequest,
    CompletarTareaResponse,
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

class TareasService {
    async misTareas(programaId: string): Promise<MisTareasResponse> {
        const url = API_ROUTES.crowdpromotion.programas.misTareas(programaId)
        const response = await apiFetch<ServiceResponse<MisTareasResponse>>(url)
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }

    async completarTarea(
        programaId: string,
        tareaId: string,
        data: CompletarTareaRequest
    ): Promise<CompletarTareaResponse> {
        const url = API_ROUTES.crowdpromotion.programas.completarTarea(programaId, tareaId)
        const response = await apiFetch<ServiceResponse<CompletarTareaResponse>>(url, {
            method: "POST",
            data,
        })
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }
}

export const tareasService = new TareasService()
