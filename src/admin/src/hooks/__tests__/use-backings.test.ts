import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement, type ReactNode } from "react"
import { useCampaignBackings, useCampaignStats } from "../use-backings"
import { campaniaService } from "@/services/campania.service"
import type { BackingPublicDto, CampaniaStats } from "@shared/types"

vi.mock("@/services/campania.service", () => ({
    campaniaService: {
        getAll: vi.fn(),
        getById: vi.fn(),
        getMisCampanias: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        publicar: vi.fn(),
        delete: vi.fn(),
        getBackings: vi.fn(),
        getStats: vi.fn(),
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

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: ReactNode }) {
        return createElement(QueryClientProvider, { client: queryClient }, children)
    }
}

describe("useCampaignBackings", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns data on success", async () => {
        vi.mocked(campaniaService.getBackings).mockResolvedValue(mockBackings)

        const { result } = renderHook(
            () => useCampaignBackings("campania-1"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data).toHaveLength(2)
        expect(result.current.data?.[0].nombreBacker).toBe("Maria Lopez")
    })

    it("handles loading state", () => {
        vi.mocked(campaniaService.getBackings).mockImplementation(
            () => new Promise(() => {})
        )

        const { result } = renderHook(
            () => useCampaignBackings("campania-1"),
            { wrapper: createWrapper() }
        )

        expect(result.current.isLoading).toBe(true)
    })

    it("handles error state", async () => {
        vi.mocked(campaniaService.getBackings).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(
            () => useCampaignBackings("campania-1"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(result.current.error).toBeDefined()
    })

    it("does not fetch when campaniaId is empty", () => {
        const { result } = renderHook(() => useCampaignBackings(""), {
            wrapper: createWrapper(),
        })

        expect(result.current.fetchStatus).toBe("idle")
        expect(campaniaService.getBackings).not.toHaveBeenCalled()
    })
})

describe("useCampaignStats", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns stats on success", async () => {
        vi.mocked(campaniaService.getStats).mockResolvedValue(mockStats)

        const { result } = renderHook(
            () => useCampaignStats("campania-1"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data?.totalBackers).toBe(42)
        expect(result.current.data?.totalRecaudado).toBe(5000)
    })

    it("handles error state", async () => {
        vi.mocked(campaniaService.getStats).mockRejectedValue(
            new Error("Server error")
        )

        const { result } = renderHook(
            () => useCampaignStats("campania-1"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(result.current.error).toBeDefined()
    })

    it("does not fetch when campaniaId is empty", () => {
        const { result } = renderHook(() => useCampaignStats(""), {
            wrapper: createWrapper(),
        })

        expect(result.current.fetchStatus).toBe("idle")
        expect(campaniaService.getStats).not.toHaveBeenCalled()
    })
})
