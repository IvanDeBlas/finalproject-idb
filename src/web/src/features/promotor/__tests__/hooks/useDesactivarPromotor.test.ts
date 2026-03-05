import { createElement } from "react"
import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import { describe, it, expect, vi, beforeEach } from "vitest"
import { useDesactivarPromotor } from "../../application/hooks/useDesactivarPromotor"
import { mockPromotorDesactivadoResult } from "../../__mocks__/promotor.mock"

vi.mock("../../infrastructure/promotor.service", () => ({
    promotorService: {
        desactivar: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
        info: vi.fn(),
    },
}))

const mockNavigate = vi.fn()

vi.mock("react-router-dom", async () => {
    const actual = await vi.importActual("react-router-dom")
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    }
})

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
        return createElement(
            QueryClientProvider,
            { client },
            createElement(MemoryRouter, null, children)
        )
    }
}

describe("useDesactivarPromotor", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("deactivates promotor successfully", async () => {
        vi.mocked(promotorService.desactivar).mockResolvedValue(
            mockPromotorDesactivadoResult
        )
        const { result } = renderHook(() => useDesactivarPromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate()
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
    })

    it("shows success toast on deactivation", async () => {
        vi.mocked(promotorService.desactivar).mockResolvedValue(
            mockPromotorDesactivadoResult
        )
        const { result } = renderHook(() => useDesactivarPromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate()
        })

        await waitFor(() => {
            expect(toast.success).toHaveBeenCalledWith(
                "Perfil de promotor desactivado"
            )
        })
    })

    it("navigates to home page after deactivation", async () => {
        vi.mocked(promotorService.desactivar).mockResolvedValue(
            mockPromotorDesactivadoResult
        )
        const { result } = renderHook(() => useDesactivarPromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate()
        })

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith("/")
        })
    })

    it("invalidates promotor query on success", async () => {
        vi.mocked(promotorService.desactivar).mockResolvedValue(
            mockPromotorDesactivadoResult
        )
        const queryClient = createTestQueryClient()
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const { result } = renderHook(() => useDesactivarPromotor(), {
            wrapper: createWrapper(queryClient),
        })

        act(() => {
            result.current.mutate()
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(invalidateSpy).toHaveBeenCalled()
    })

    it("shows error toast when already inactive (errorCode 4019)", async () => {
        const error = new Error("Ya esta desactivado") as Error & { errorCode: string }
        error.errorCode = "4019"
        vi.mocked(promotorService.desactivar).mockRejectedValue(error)

        const { result } = renderHook(() => useDesactivarPromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate()
        })

        await waitFor(() => {
            expect(toast.info).toHaveBeenCalledWith(
                "Tu perfil de promotor ya estaba desactivado"
            )
        })
    })

    it("does NOT navigate on failure", async () => {
        vi.mocked(promotorService.desactivar).mockRejectedValue(
            new Error("Error de red")
        )
        const { result } = renderHook(() => useDesactivarPromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate()
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(mockNavigate).not.toHaveBeenCalled()
    })
})
