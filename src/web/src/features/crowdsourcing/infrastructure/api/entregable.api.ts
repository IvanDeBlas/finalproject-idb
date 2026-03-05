import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    CreateEntregableRequest,
    EntregableCreatedResult,
    AprobarEntregableRequest,
    AprobarEntregableResult,
    RechazarEntregableRequest,
    RechazarEntregableResult,
} from "../../domain"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}

class EntregableApiService {
    async create(
        acuerdoId: string,
        data: CreateEntregableRequest
    ): Promise<EntregableCreatedResult> {
        const response = await apiFetch<
            ServiceResponse<EntregableCreatedResult>
        >(API_ROUTES.crowdsourcing.acuerdos.entregables(acuerdoId), {
            method: "POST",
            data,
        })

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async aprobar(
        entregableId: string,
        data: AprobarEntregableRequest
    ): Promise<AprobarEntregableResult> {
        const response = await apiFetch<
            ServiceResponse<AprobarEntregableResult>
        >(API_ROUTES.crowdsourcing.entregables.aprobar(entregableId), {
            method: "PATCH",
            data,
        })

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async rechazar(
        entregableId: string,
        data: RechazarEntregableRequest
    ): Promise<RechazarEntregableResult> {
        const response = await apiFetch<
            ServiceResponse<RechazarEntregableResult>
        >(API_ROUTES.crowdsourcing.entregables.rechazar(entregableId), {
            method: "PATCH",
            data,
        })

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }
}

export const entregableApi = new EntregableApiService()
