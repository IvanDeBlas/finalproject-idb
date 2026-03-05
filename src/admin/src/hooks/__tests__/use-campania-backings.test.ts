import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement, type ReactNode } from "react"
import { useCampaniaBackings } from "../use-campania-backings"
import { dashboardService } from "@/services/dashboard.service"
import { mockCampaniaBackings } from "@/__mocks__/dashboard.mock"

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

describe("useCampaniaBackings", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns backings data on success", async () => {
        vi.mocked(dashboardService.getCampaniaBackings).mockResolvedValue(
            mockCampaniaBackings
        )

        const { result } = renderHook(
            () => useCampaniaBackings("campania-1"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data?.backings.items).toHaveLength(3)
        expect(result.current.data?.stats.totalBackers).toBe(78)
    })

    it("handles loading state", () => {
        vi.mocked(dashboardService.getCampaniaBackings).mockImplementation(
            () => new Promise(() => {})
        )

        const { result } = renderHook(
            () => useCampaniaBackings("campania-1"),
            { wrapper: createWrapper() }
        )

        expect(result.current.isLoading).toBe(true)
        expect(result.current.data).toBeUndefined()
    })

    it("handles error state", async () => {
        vi.mocked(dashboardService.getCampaniaBackings).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(
            () => useCampaniaBackings("campania-1"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(result.current.error).toBeDefined()
    })

    it("does not fetch when campaniaId is empty", () => {
        const { result } = renderHook(
            () => useCampaniaBackings(""),
            { wrapper: createWrapper() }
        )

        expect(result.current.fetchStatus).toBe("idle")
        expect(dashboardService.getCampaniaBackings).not.toHaveBeenCalled()
    })

    it("passes pagination params to service", async () => {
        vi.mocked(dashboardService.getCampaniaBackings).mockResolvedValue(
            mockCampaniaBackings
        )

        renderHook(
            () => useCampaniaBackings("campania-1", { page: 2, pageSize: 10 }),
            { wrapper: createWrapper() }
        )

        await waitFor(() => {
            expect(dashboardService.getCampaniaBackings).toHaveBeenCalledWith(
                "campania-1",
                { page: 2, pageSize: 10 }
            )
        })
    })

    it("calls service without params when none provided", async () => {
        vi.mocked(dashboardService.getCampaniaBackings).mockResolvedValue(
            mockCampaniaBackings
        )

        renderHook(
            () => useCampaniaBackings("campania-1"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => {
            expect(dashboardService.getCampaniaBackings).toHaveBeenCalledWith(
                "campania-1",
                undefined
            )
        })
    })
})
