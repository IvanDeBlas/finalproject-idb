import { createElement } from "react"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { describe, it, expect, vi, beforeEach } from "vitest"
import { usePromotor } from "../../application/hooks/usePromotor"
import { mockPromotor } from "../../__mocks__/promotor.mock"

vi.mock("../../infrastructure/promotor.service", () => ({
    promotorService: {
        getMe: vi.fn(),
    },
}))

import { promotorService } from "../../infrastructure/promotor.service"

function createTestQueryClient() {
    return new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })
}

function createWrapper(queryClient?: QueryClient) {
    const client = queryClient ?? createTestQueryClient()
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return createElement(QueryClientProvider, { client }, children)
    }
}

describe("usePromotor", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns promotor data on success", async () => {
        vi.mocked(promotorService.getMe).mockResolvedValue(mockPromotor)
        const { result } = renderHook(() => usePromotor(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data).toEqual(mockPromotor)
    })

    it("handles loading state initially", () => {
        vi.mocked(promotorService.getMe).mockImplementation(
            () => new Promise(() => {})
        )
        const { result } = renderHook(() => usePromotor(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
        expect(result.current.data).toBeUndefined()
    })

    it("handles error state when API fails", async () => {
        vi.mocked(promotorService.getMe).mockRejectedValue(
            new Error("Network error")
        )
        const { result } = renderHook(() => usePromotor(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(result.current.data).toBeUndefined()
    })

    it("handles 404 not found (no profile yet)", async () => {
        const error = new Error("Promotor no encontrado") as Error & { errorCode: string }
        error.errorCode = "2010"
        vi.mocked(promotorService.getMe).mockRejectedValue(error)

        const { result } = renderHook(() => usePromotor(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
    })

    it("does NOT retry on failure", async () => {
        vi.mocked(promotorService.getMe).mockRejectedValue(
            new Error("Error")
        )
        const { result } = renderHook(() => usePromotor(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(promotorService.getMe).toHaveBeenCalledTimes(1)
    })

    it("calls promotorService.getMe", async () => {
        vi.mocked(promotorService.getMe).mockResolvedValue(mockPromotor)
        renderHook(() => usePromotor(), {
            wrapper: createWrapper(),
        })

        await waitFor(() =>
            expect(promotorService.getMe).toHaveBeenCalledTimes(1)
        )
    })
})
