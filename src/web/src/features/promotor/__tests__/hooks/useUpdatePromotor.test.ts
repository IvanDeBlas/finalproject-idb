import { createElement } from "react"
import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { describe, it, expect, vi, beforeEach } from "vitest"
import { useUpdatePromotor } from "../../application/hooks/useUpdatePromotor"
import {
    mockUpdatePromotorRequest,
    mockPromotorUpdatedResult,
} from "../../__mocks__/promotor.mock"

vi.mock("../../infrastructure/promotor.service", () => ({
    promotorService: {
        update: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

import { promotorService } from "../../infrastructure/promotor.service"
import { toast } from "sonner"

function createTestQueryClient() {
    return new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
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

describe("useUpdatePromotor", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("updates promotor successfully", async () => {
        vi.mocked(promotorService.update).mockResolvedValue(
            mockPromotorUpdatedResult
        )
        const { result } = renderHook(() => useUpdatePromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(mockUpdatePromotorRequest)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
    })

    it("passes correct data to service (without tipoPromotorId)", async () => {
        vi.mocked(promotorService.update).mockResolvedValue(
            mockPromotorUpdatedResult
        )
        const { result } = renderHook(() => useUpdatePromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(mockUpdatePromotorRequest)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(promotorService.update).toHaveBeenCalledWith(
            mockUpdatePromotorRequest
        )
        const callArg = vi.mocked(promotorService.update).mock.calls[0][0]
        expect("tipoPromotorId" in callArg).toBe(false)
    })

    it("shows success toast on success", async () => {
        vi.mocked(promotorService.update).mockResolvedValue(
            mockPromotorUpdatedResult
        )
        const { result } = renderHook(() => useUpdatePromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(mockUpdatePromotorRequest)
        })

        await waitFor(() => {
            expect(toast.success).toHaveBeenCalledWith(
                "Perfil actualizado correctamente"
            )
        })
    })

    it("invalidates promotor query on success", async () => {
        vi.mocked(promotorService.update).mockResolvedValue(
            mockPromotorUpdatedResult
        )
        const queryClient = createTestQueryClient()
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const { result } = renderHook(() => useUpdatePromotor(), {
            wrapper: createWrapper(queryClient),
        })

        act(() => {
            result.current.mutate(mockUpdatePromotorRequest)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(invalidateSpy).toHaveBeenCalled()
    })

    it("shows error toast on failure", async () => {
        vi.mocked(promotorService.update).mockRejectedValue(
            new Error("Error de red")
        )
        const { result } = renderHook(() => useUpdatePromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(mockUpdatePromotorRequest)
        })

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalled()
        })
    })
})
