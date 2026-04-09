import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    Acuerdo,
    AceptarPropuestaRequest,
    AceptarPropuestaResult,
    RechazarPropuestaRequest,
    RechazarPropuestaResult,
    CancelarAcuerdoRequest,
    CancelarAcuerdoResult,
    CompletarAcuerdoResult,
} from "../../domain"

interface ServiceResponse<T> {
    data: T
    messages: Array<{ message: string; errorCode: string }>
}

class AcuerdoApiService {
    async getById(id: string): Promise<Acuerdo> {
        const response = await apiFetch<ServiceResponse<Acuerdo>>(
            API_ROUTES.crowdsourcing.acuerdos.byId(id)
        )

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async completar(id: string): Promise<CompletarAcuerdoResult> {
        const response = await apiFetch<ServiceResponse<CompletarAcuerdoResult>>(
            API_ROUTES.crowdsourcing.acuerdos.completar(id),
            { method: "PATCH" }
        )

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async cancelar(
        id: string,
        data: CancelarAcuerdoRequest
    ): Promise<CancelarAcuerdoResult> {
        const response = await apiFetch<ServiceResponse<CancelarAcuerdoResult>>(
            API_ROUTES.crowdsourcing.acuerdos.cancelar(id),
            { method: "PATCH", data }
        )

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async aceptarPropuesta(
        propuestaId: string,
        data: AceptarPropuestaRequest
    ): Promise<AceptarPropuestaResult> {
        const response = await apiFetch<
            ServiceResponse<AceptarPropuestaResult>
        >(API_ROUTES.crowdsourcing.propuestas.aceptar(propuestaId), {
            method: "POST",
            data,
        })

        if (!response.data) {
            const errorCode = response.messages?.[0]?.errorCode ?? "5000"
            throw new Error(errorCode)
        }

        return response.data
    }

    async rechazarPropuesta(
        propuestaId: string,
        data: RechazarPropuestaRequest
    ): Promise<RechazarPropuestaResult> {
        const response = await apiFetch<
            ServiceResponse<RechazarPropuestaResult>
        >(API_ROUTES.crowdsourcing.propuestas.rechazar(propuestaId), {
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

export const acuerdoApi = new AcuerdoApiService()
