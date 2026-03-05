import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type {
    DashboardResumen,
    CampaniaStatsDetail,
    CampaniaBackingList,
    ServiceResponse,
} from "@shared/types"
import type { BackingsQueryParams } from "@shared/schemas"

export const dashboardService = {
    async getResumen(): Promise<DashboardResumen> {
        const response = await apiFetch<ServiceResponse<DashboardResumen>>(
            API_ROUTES.dashboard.resumen
        )
        return response.data
    },

    async getCampaniaStats(campaniaId: string): Promise<CampaniaStatsDetail> {
        const response = await apiFetch<ServiceResponse<CampaniaStatsDetail>>(
            API_ROUTES.dashboard.campaniaStats(campaniaId)
        )
        return response.data
    },

    async getCampaniaBackings(
        campaniaId: string,
        params?: BackingsQueryParams
    ): Promise<CampaniaBackingList> {
        const queryParams = new URLSearchParams()
        if (params?.page) queryParams.set("page", String(params.page))
        if (params?.pageSize) queryParams.set("pageSize", String(params.pageSize))

        const qs = queryParams.toString()
        const url = `${API_ROUTES.dashboard.campaniaBackings(campaniaId)}${qs ? `?${qs}` : ""}`
        const response = await apiFetch<ServiceResponse<CampaniaBackingList>>(url)
        return response.data
    },

    async exportBackingsCSV(campaniaId: string): Promise<Blob> {
        const url = `${API_ROUTES.dashboard.campaniaBackings(campaniaId)}/export`
        const response = await apiFetch<Blob>(url, { responseType: "blob" })
        return response
    },
}
