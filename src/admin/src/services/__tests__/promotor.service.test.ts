import { promotorService } from "../promotor.service"
import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type { ServiceResponse } from "@shared/types"
import type { Promotor, PromotorUpdatedResult, PromotorDesactivadoResult } from "@shared/types"
import {
    mockPromotor,
    mockPromotorUpdatedResult,
    mockDesactivadoConProgramas,
} from "@/__mocks__/promotor.mock"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
    apiClient: {
        interceptors: {
            request: { use: vi.fn() },
            response: { use: vi.fn() },
        },
    },
}))

describe("promotorService.getMe", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns promotor profile on success", async () => {
        const response: ServiceResponse<Promotor> = {
            data: mockPromotor,
            messages: [],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const result = await promotorService.getMe()

        expect(result).toEqual(mockPromotor)
        expect(result?.nombrePublico).toBe("DJ Promo Star")
    })

    it("returns null on error", async () => {
        vi.mocked(apiFetch).mockRejectedValue(new Error("Network error"))

        const result = await promotorService.getMe()

        expect(result).toBeNull()
    })

    it("calls apiFetch with correct URL", async () => {
        vi.mocked(apiFetch).mockResolvedValue({ data: mockPromotor, messages: [] })

        await promotorService.getMe()

        expect(apiFetch).toHaveBeenCalledWith(API_ROUTES.crowdpromotion.promotor.me)
    })
})

describe("promotorService.update", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("sends PUT request with correct URL and body", async () => {
        const response: ServiceResponse<PromotorUpdatedResult> = {
            data: mockPromotorUpdatedResult,
            messages: [{ message: "Actualizado", errorCode: "0002" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const updateData = { nombrePublico: "Updated Name" }
        await promotorService.update(updateData)

        expect(apiFetch).toHaveBeenCalledWith(
            API_ROUTES.crowdpromotion.promotor.me,
            { method: "PUT", data: updateData }
        )
    })

    it("returns result on success", async () => {
        const response: ServiceResponse<PromotorUpdatedResult> = {
            data: mockPromotorUpdatedResult,
            messages: [{ message: "OK", errorCode: "0002" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const result = await promotorService.update({ nombrePublico: "Test" })

        expect(result.id).toBe("promotor-1")
        expect(result.nombrePublico).toBe("DJ Promo Star Updated")
    })

    it("throws error when response has error messages", async () => {
        const response: ServiceResponse<PromotorUpdatedResult> = {
            data: null as unknown as PromotorUpdatedResult,
            messages: [{ message: "Error", errorCode: "2015" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        await expect(
            promotorService.update({ nombrePublico: "Test" })
        ).rejects.toThrow()
    })

    it("propagates network error", async () => {
        vi.mocked(apiFetch).mockRejectedValue(new Error("Server error"))

        await expect(
            promotorService.update({ nombrePublico: "Test" })
        ).rejects.toThrow("Server error")
    })
})

describe("promotorService.desactivar", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("sends PATCH request with correct URL", async () => {
        const response: ServiceResponse<PromotorDesactivadoResult> = {
            data: mockDesactivadoConProgramas,
            messages: [{ message: "OK", errorCode: "0002" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        await promotorService.desactivar()

        expect(apiFetch).toHaveBeenCalledWith(
            API_ROUTES.crowdpromotion.promotor.desactivar,
            { method: "PATCH" }
        )
    })

    it("returns result on success", async () => {
        const response: ServiceResponse<PromotorDesactivadoResult> = {
            data: mockDesactivadoConProgramas,
            messages: [{ message: "OK", errorCode: "0002" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const result = await promotorService.desactivar()

        expect(result.esActivo).toBe(false)
        expect(result.programasDadosDeBaja).toBe(3)
    })

    it("throws error when response has error messages", async () => {
        const response: ServiceResponse<PromotorDesactivadoResult> = {
            data: null as unknown as PromotorDesactivadoResult,
            messages: [{ message: "Already inactive", errorCode: "4019" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        await expect(promotorService.desactivar()).rejects.toThrow()
    })

    it("propagates network error", async () => {
        vi.mocked(apiFetch).mockRejectedValue(new Error("Network error"))

        await expect(promotorService.desactivar()).rejects.toThrow("Network error")
    })
})
