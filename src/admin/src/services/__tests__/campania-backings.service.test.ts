import { campaniaService } from "../campania.service"
import { apiFetch } from "@/lib/api-client"
import type { BackingPublicDto, CampaniaStats, ServiceResponse } from "@shared/types"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
    apiClient: {
        interceptors: {
            request: { use: vi.fn() },
            response: { use: vi.fn() },
        },
    },
}))

const mockBackings: BackingPublicDto[] = [
    {
        id: "1",
        nombreBacker: "Maria Lopez",
        monto: 50,
        rewardNombre: "CD Fisico",
        mensaje: "Exito!",
        fechaCreacion: "2026-02-10T10:00:00Z",
    },
    {
        id: "2",
        nombreBacker: "Anonimo",
        monto: 25,
        rewardNombre: null,
        mensaje: null,
        fechaCreacion: "2026-02-09T09:00:00Z",
    },
]

const mockStats: CampaniaStats = {
    campaniaId: "campania-1",
    totalBackers: 42,
    totalRecaudado: 5000,
    promedioAporte: 119,
    aporteMinimo: 10,
    aporteMaximo: 500,
    diasRestantes: 30,
}

describe("campaniaService.getBackings", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns backings list for a campania", async () => {
        const response: ServiceResponse<BackingPublicDto[]> = {
            data: mockBackings,
            messages: [],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const result = await campaniaService.getBackings("campania-1")

        expect(result).toHaveLength(2)
        expect(result[0].nombreBacker).toBe("Maria Lopez")
        expect(result[1].monto).toBe(25)
    })

    it("calls apiFetch with correct URL without params", async () => {
        vi.mocked(apiFetch).mockResolvedValue({ data: [], messages: [] })

        await campaniaService.getBackings("campania-1")

        expect(apiFetch).toHaveBeenCalledWith("/campanias/campania-1/backings")
    })

    it("sends correct query params when provided", async () => {
        vi.mocked(apiFetch).mockResolvedValue({ data: [], messages: [] })

        await campaniaService.getBackings("campania-1", {
            pageNumber: 2,
            pageSize: 10,
        })

        expect(apiFetch).toHaveBeenCalledWith(
            "/campanias/campania-1/backings?pageNumber=2&pageSize=10"
        )
    })

    it("handles API error", async () => {
        vi.mocked(apiFetch).mockRejectedValue(new Error("Network error"))

        await expect(
            campaniaService.getBackings("invalid-id")
        ).rejects.toThrow("Network error")
    })
})

describe("campaniaService.getStats", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns stats for a campania", async () => {
        const response: ServiceResponse<CampaniaStats> = {
            data: mockStats,
            messages: [],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const result = await campaniaService.getStats("campania-1")

        expect(result.totalBackers).toBe(42)
        expect(result.totalRecaudado).toBe(5000)
        expect(result.promedioAporte).toBe(119)
    })

    it("calls apiFetch with correct URL", async () => {
        vi.mocked(apiFetch).mockResolvedValue({ data: mockStats, messages: [] })

        await campaniaService.getStats("campania-1")

        expect(apiFetch).toHaveBeenCalledWith("/campanias/campania-1/stats")
    })

    it("handles API error", async () => {
        vi.mocked(apiFetch).mockRejectedValue(new Error("Server error"))

        await expect(
            campaniaService.getStats("invalid-id")
        ).rejects.toThrow("Server error")
    })
})
