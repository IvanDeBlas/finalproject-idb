import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement, type ReactNode } from "react"
import { useCampaniaStats } from "../use-campania-stats"
import { dashboardService } from "@/services/dashboard.service"
import { mockCampaniaStats } from "@/__mocks__/dashboard.mock"

vi.mock("@/services/dashboard.service", () => ({
    dashboardService: {
        getResumen: vi.fn(),
        getCampaniaStats: vi.fn(),
        getCampaniaBackings: vi.fn(),
        exportBackingsCSV: vi.fn(),
    },
}))

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

describe("useCampaniaStats", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns stats on success", async () => {
        vi.mocked(dashboardService.getCampaniaStats).mockResolvedValue(
            mockCampaniaStats
        )

        const { result } = renderHook(
            () => useCampaniaStats("campania-1"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data?.numBackers).toBe(78)
        expect(result.current.data?.importeRecaudado).toBe(2340.5)
        expect(result.current.data?.porcentajeProgreso).toBe(46.81)
    })

    it("handles loading state", () => {
        vi.mocked(dashboardService.getCampaniaStats).mockImplementation(
            () => new Promise(() => {})
        )

        const { result } = renderHook(
            () => useCampaniaStats("campania-1"),
            { wrapper: createWrapper() }
        )

        expect(result.current.isLoading).toBe(true)
        expect(result.current.data).toBeUndefined()
    })

    it("handles error state", async () => {
        vi.mocked(dashboardService.getCampaniaStats).mockRejectedValue(
            new Error("Server error")
        )

        const { result } = renderHook(
            () => useCampaniaStats("campania-1"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(result.current.error).toBeDefined()
    })

    it("does not fetch when campaniaId is empty", () => {
        const { result } = renderHook(
            () => useCampaniaStats(""),
            { wrapper: createWrapper() }
        )

        expect(result.current.fetchStatus).toBe("idle")
        expect(dashboardService.getCampaniaStats).not.toHaveBeenCalled()
    })

    it("calls service with correct campaniaId", async () => {
        vi.mocked(dashboardService.getCampaniaStats).mockResolvedValue(
            mockCampaniaStats
        )

        renderHook(
            () => useCampaniaStats("campania-42"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => {
            expect(dashboardService.getCampaniaStats).toHaveBeenCalledWith(
                "campania-42"
            )
        })
    })
})
