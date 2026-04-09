import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement, type ReactNode } from "react"
import { useDashboardStats } from "../use-dashboard-stats"
import { dashboardService } from "@/services/dashboard.service"
import { mockDashboardResumen } from "@/__mocks__/dashboard.mock"

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

describe("useDashboardStats", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns resumen data on success", async () => {
        vi.mocked(dashboardService.getResumen).mockResolvedValue(
            mockDashboardResumen
        )

        const { result } = renderHook(() => useDashboardStats(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data).toEqual(mockDashboardResumen)
        expect(result.current.data?.totalRecaudado).toBe(15340.5)
        expect(result.current.data?.totalBackers).toBe(487)
    })

    it("handles loading state", () => {
        vi.mocked(dashboardService.getResumen).mockImplementation(
            () => new Promise(() => {})
        )

        const { result } = renderHook(() => useDashboardStats(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
        expect(result.current.data).toBeUndefined()
    })

    it("handles error state", async () => {
        vi.mocked(dashboardService.getResumen).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(() => useDashboardStats(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(result.current.error).toBeDefined()
    })

    it("returns correct data structure", async () => {
        vi.mocked(dashboardService.getResumen).mockResolvedValue(
            mockDashboardResumen
        )

        const { result } = renderHook(() => useDashboardStats(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toHaveProperty("artistaId")
        expect(result.current.data).toHaveProperty("totalRecaudado")
        expect(result.current.data).toHaveProperty("totalBackers")
        expect(result.current.data).toHaveProperty("campaniasActivas")
        expect(result.current.data).toHaveProperty("campaniasCompletadas")
        expect(result.current.data).toHaveProperty("monedaSimbolo")
    })

    it("calls getResumen service method", async () => {
        vi.mocked(dashboardService.getResumen).mockResolvedValue(
            mockDashboardResumen
        )

        renderHook(() => useDashboardStats(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(dashboardService.getResumen).toHaveBeenCalledTimes(1)
        })
    })
})
