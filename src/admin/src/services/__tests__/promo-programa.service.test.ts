import { promoProgramaService } from "../promo-programa.service"
import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type { ServiceResponse } from "@shared/types"
import type {
    PromoProgramaListResult,
    PromoProgramaDetail,
    PromoProgramaCreatedResult,
    PromoProgramaUpdatedResult,
    PromoProgramaDesactivadoResult,
} from "@shared/types"
import {
    mockProgramaListResult,
    mockProgramaDetail,
    mockCreatedResult,
    mockUpdatedResult,
    mockDesactivadoResult,
} from "@/__mocks__/promo-programa.mock"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
    apiClient: {
        interceptors: {
            request: { use: vi.fn() },
            response: { use: vi.fn() },
        },
    },
}))

describe("promoProgramaService.getMisProgramas", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns paginated list on success", async () => {
        const response: ServiceResponse<PromoProgramaListResult> = {
            data: mockProgramaListResult,
            messages: [],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const result = await promoProgramaService.getMisProgramas()

        expect(result.items).toHaveLength(2)
        expect(result.totalCount).toBe(2)
    })

    it("calls correct URL without params", async () => {
        vi.mocked(apiFetch).mockResolvedValue({ data: mockProgramaListResult, messages: [] })

        await promoProgramaService.getMisProgramas()

        expect(apiFetch).toHaveBeenCalledWith(API_ROUTES.crowdpromotion.programas.mis)
    })

    it("appends query params when provided", async () => {
        vi.mocked(apiFetch).mockResolvedValue({ data: mockProgramaListResult, messages: [] })

        await promoProgramaService.getMisProgramas({ esActivo: true, page: 2, pageSize: 5 })

        const calledUrl = vi.mocked(apiFetch).mock.calls[0][0] as string
        expect(calledUrl).toContain("esActivo=true")
        expect(calledUrl).toContain("page=2")
        expect(calledUrl).toContain("pageSize=5")
    })
})

describe("promoProgramaService.getById", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns detail on success", async () => {
        const response: ServiceResponse<PromoProgramaDetail> = {
            data: mockProgramaDetail,
            messages: [],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const result = await promoProgramaService.getById("programa-1")

        expect(result?.titulo).toBe("Campana de Referidos Q1")
        expect(result?.tareas).toHaveLength(2)
    })

    it("returns null on error (404)", async () => {
        vi.mocked(apiFetch).mockRejectedValue(new Error("Not found"))

        const result = await promoProgramaService.getById("nonexistent")

        expect(result).toBeNull()
    })

    it("calls correct URL", async () => {
        vi.mocked(apiFetch).mockResolvedValue({ data: mockProgramaDetail, messages: [] })

        await promoProgramaService.getById("programa-1")

        expect(apiFetch).toHaveBeenCalledWith(
            API_ROUTES.crowdpromotion.programas.byId("programa-1")
        )
    })
})

describe("promoProgramaService.create", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("sends POST request with correct data", async () => {
        const response: ServiceResponse<PromoProgramaCreatedResult> = {
            data: mockCreatedResult,
            messages: [{ message: "Creado", errorCode: "0001" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const createData = { titulo: "Test", tipoPromoId: 1, monedaId: 1 }
        await promoProgramaService.create(createData)

        expect(apiFetch).toHaveBeenCalledWith(
            API_ROUTES.crowdpromotion.programas.base,
            { method: "POST", data: createData }
        )
    })

    it("returns created result on success", async () => {
        const response: ServiceResponse<PromoProgramaCreatedResult> = {
            data: mockCreatedResult,
            messages: [{ message: "OK", errorCode: "0001" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const result = await promoProgramaService.create({ titulo: "Test", tipoPromoId: 1, monedaId: 1 })

        expect(result.id).toBe("programa-new")
        expect(result.esActivo).toBe(true)
    })

    it("throws error when response has error messages", async () => {
        const response: ServiceResponse<PromoProgramaCreatedResult> = {
            data: null as unknown as PromoProgramaCreatedResult,
            messages: [{ message: "Comision requerida", errorCode: "1020" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        await expect(
            promoProgramaService.create({ titulo: "Test", tipoPromoId: 1, monedaId: 1 })
        ).rejects.toThrow()
    })

    it("propagates network error", async () => {
        vi.mocked(apiFetch).mockRejectedValue(new Error("Server error"))

        await expect(
            promoProgramaService.create({ titulo: "Test", tipoPromoId: 1, monedaId: 1 })
        ).rejects.toThrow("Server error")
    })
})

describe("promoProgramaService.update", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("sends PUT request with correct URL and body", async () => {
        const response: ServiceResponse<PromoProgramaUpdatedResult> = {
            data: mockUpdatedResult,
            messages: [{ message: "OK", errorCode: "0002" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const updateData = { titulo: "Updated", tipoPromoId: 1, monedaId: 1 }
        await promoProgramaService.update("programa-1", updateData)

        expect(apiFetch).toHaveBeenCalledWith(
            API_ROUTES.crowdpromotion.programas.byId("programa-1"),
            { method: "PUT", data: updateData }
        )
    })

    it("returns updated result on success", async () => {
        const response: ServiceResponse<PromoProgramaUpdatedResult> = {
            data: mockUpdatedResult,
            messages: [{ message: "OK", errorCode: "0002" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const result = await promoProgramaService.update("programa-1", { titulo: "X", tipoPromoId: 1, monedaId: 1 })

        expect(result.titulo).toBe("Programa Actualizado")
    })

    it("throws on error response", async () => {
        const response: ServiceResponse<PromoProgramaUpdatedResult> = {
            data: null as unknown as PromoProgramaUpdatedResult,
            messages: [{ message: "Not found", errorCode: "2016" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        await expect(
            promoProgramaService.update("x", { titulo: "X", tipoPromoId: 1, monedaId: 1 })
        ).rejects.toThrow()
    })
})

describe("promoProgramaService.desactivar", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("sends PATCH request with correct URL", async () => {
        const response: ServiceResponse<PromoProgramaDesactivadoResult> = {
            data: mockDesactivadoResult,
            messages: [{ message: "OK", errorCode: "0002" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        await promoProgramaService.desactivar("programa-1")

        expect(apiFetch).toHaveBeenCalledWith(
            API_ROUTES.crowdpromotion.programas.desactivar("programa-1"),
            { method: "PATCH" }
        )
    })

    it("returns result on success", async () => {
        const response: ServiceResponse<PromoProgramaDesactivadoResult> = {
            data: mockDesactivadoResult,
            messages: [{ message: "OK", errorCode: "0002" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const result = await promoProgramaService.desactivar("programa-1")

        expect(result.esActivo).toBe(false)
        expect(result.tareasDesactivadas).toBe(2)
    })

    it("throws on error response", async () => {
        const response: ServiceResponse<PromoProgramaDesactivadoResult> = {
            data: null as unknown as PromoProgramaDesactivadoResult,
            messages: [{ message: "Already inactive", errorCode: "4021" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        await expect(promoProgramaService.desactivar("programa-1")).rejects.toThrow()
    })

    it("propagates network error", async () => {
        vi.mocked(apiFetch).mockRejectedValue(new Error("Network error"))

        await expect(promoProgramaService.desactivar("programa-1")).rejects.toThrow("Network error")
    })
})
