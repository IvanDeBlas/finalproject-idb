import { createElement } from "react"
import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import { describe, it, expect, vi, beforeEach } from "vitest"
import { useCreatePromotor } from "../../application/hooks/useCreatePromotor"
import {
    mockCreatePromotorRequest,
    mockPromotorCreatedResult,
} from "../../__mocks__/promotor.mock"

vi.mock("../../infrastructure/promotor.service", () => ({
    promotorService: {
        create: vi.fn(),
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

describe("useCreatePromotor", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("creates promotor successfully", async () => {
        vi.mocked(promotorService.create).mockResolvedValue(
            mockPromotorCreatedResult
        )
        const { result } = renderHook(() => useCreatePromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(mockCreatePromotorRequest)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
    })

    it("passes correct data to service", async () => {
        vi.mocked(promotorService.create).mockResolvedValue(
            mockPromotorCreatedResult
        )
        const { result } = renderHook(() => useCreatePromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(mockCreatePromotorRequest)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(promotorService.create).toHaveBeenCalledWith(
            mockCreatePromotorRequest
        )
    })

    it("shows success toast on success", async () => {
        vi.mocked(promotorService.create).mockResolvedValue(
            mockPromotorCreatedResult
        )
        const { result } = renderHook(() => useCreatePromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(mockCreatePromotorRequest)
        })

        await waitFor(() => {
            expect(toast.success).toHaveBeenCalledWith(
                "Perfil de promotor creado correctamente"
            )
        })
    })

    it("navigates to /promotor/dashboard on success", async () => {
        vi.mocked(promotorService.create).mockResolvedValue(
            mockPromotorCreatedResult
        )
        const { result } = renderHook(() => useCreatePromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(mockCreatePromotorRequest)
        })

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith("/promotor/dashboard")
        })
    })

    it("invalidates promotor query on success", async () => {
        vi.mocked(promotorService.create).mockResolvedValue(
            mockPromotorCreatedResult
        )
        const queryClient = createTestQueryClient()
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const { result } = renderHook(() => useCreatePromotor(), {
            wrapper: createWrapper(queryClient),
        })

        act(() => {
            result.current.mutate(mockCreatePromotorRequest)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(invalidateSpy).toHaveBeenCalled()
    })

    it("shows error toast on failure", async () => {
        vi.mocked(promotorService.create).mockRejectedValue(
            new Error("Error de red")
        )
        const { result } = renderHook(() => useCreatePromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(mockCreatePromotorRequest)
        })

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalled()
        })
    })

    it("does NOT navigate on failure", async () => {
        vi.mocked(promotorService.create).mockRejectedValue(
            new Error("Error de red")
        )
        const { result } = renderHook(() => useCreatePromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(mockCreatePromotorRequest)
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(mockNavigate).not.toHaveBeenCalled()
    })

    it("handles errorCode 4018 (already exists) with info toast and navigate", async () => {
        const error = new Error("Ya existe un promotor") as Error & { errorCode: string }
        error.errorCode = "4018"
        vi.mocked(promotorService.create).mockRejectedValue(error)

        const { result } = renderHook(() => useCreatePromotor(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(mockCreatePromotorRequest)
        })

        await waitFor(() => {
            expect(toast.info).toHaveBeenCalledWith(
                "Ya tienes un perfil de promotor creado"
            )
        })
        expect(mockNavigate).toHaveBeenCalledWith("/promotor/dashboard")
    })
})
