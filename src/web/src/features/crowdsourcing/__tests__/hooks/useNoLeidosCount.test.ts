import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { useNoLeidosCount } from "../../application/hooks/useNoLeidosCount"
import {
    mockNoLeidosCountResponse,
    mockNoLeidosCountCero,
} from "../../__mocks__/mensajeria.mock"

vi.mock("../../infrastructure", () => ({
    conversacionApi: {
        getAll: vi.fn(),
        create: vi.fn(),
        getNoLeidosCount: vi.fn(),
    },
}))

vi.mock("@/store/auth-store", () => ({
    useAuthStore: vi.fn(() => ({
        isAuthenticated: true,
    })),
}))

import { conversacionApi } from "../../infrastructure"

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
        },
    })
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return createElement(QueryClientProvider, { client: queryClient }, children)
    }
}

describe("useNoLeidosCount", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns total no leidos count", async () => {
        vi.mocked(conversacionApi.getNoLeidosCount).mockResolvedValue(
            mockNoLeidosCountResponse
        )

        const { result } = renderHook(() => useNoLeidosCount(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.totalNoLeidos).toBe(5)
    })

    it("returns 0 when no unread messages", async () => {
        vi.mocked(conversacionApi.getNoLeidosCount).mockResolvedValue(
            mockNoLeidosCountCero
        )

        const { result } = renderHook(() => useNoLeidosCount(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.totalNoLeidos).toBe(0)
    })

    it("isLoading is true initially", () => {
        vi.mocked(conversacionApi.getNoLeidosCount).mockReturnValue(
            new Promise(() => {})
        )

        const { result } = renderHook(() => useNoLeidosCount(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("isError is true when API fails", async () => {
        vi.mocked(conversacionApi.getNoLeidosCount).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(() => useNoLeidosCount(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
    })
})
