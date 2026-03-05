import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    PromotorWallet,
    WalletTransaccionesPagedResponse,
    SolicitarCobroRequest,
    SolicitarCobroResponse,
    WalletTransaccionesFilters,
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

class WalletService {
    async getWalletResumen(): Promise<PromotorWallet> {
        const url = API_ROUTES.crowdpromotion.promotorWallet.resumen
        const response = await apiFetch<ServiceResponse<PromotorWallet>>(url)
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }

    async getTransacciones(filters: WalletTransaccionesFilters): Promise<WalletTransaccionesPagedResponse> {
        const params = new URLSearchParams()
        if (filters.esCredito !== undefined) params.set("esCredito", String(filters.esCredito))
        if (filters.estadoTransaccionId !== undefined) params.set("estadoTransaccionId", String(filters.estadoTransaccionId))
        if (filters.fechaDesde) params.set("fechaDesde", filters.fechaDesde)
        if (filters.fechaHasta) params.set("fechaHasta", filters.fechaHasta)
        params.set("page", String(filters.page ?? 1))
        params.set("pageSize", String(filters.pageSize ?? 10))

        const query = params.toString()
        const url = `${API_ROUTES.crowdpromotion.promotorWallet.transacciones}?${query}`
        const response = await apiFetch<ServiceResponse<WalletTransaccionesPagedResponse>>(url)
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }

    async solicitarCobro(data: SolicitarCobroRequest): Promise<SolicitarCobroResponse> {
        const url = API_ROUTES.crowdpromotion.promotorWallet.cobro
        const response = await apiFetch<ServiceResponse<SolicitarCobroResponse>>(url, {
            method: "POST",
            data,
        })
        if (response.messages?.some(m => m.errorCode && !m.errorCode.startsWith("0"))) {
            throw extractError(response.messages)
        }
        return response.data
    }
}

export const walletService = new WalletService()
