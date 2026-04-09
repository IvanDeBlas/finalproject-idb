import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { useConversaciones } from "../../application/hooks/useConversaciones"
import { mockConversacionListResponse } from "../../__mocks__/mensajeria.mock"

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

describe("useConversaciones", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns data correctly on success", async () => {
        vi.mocked(conversacionApi.getAll).mockResolvedValue(
            mockConversacionListResponse
        )

        const { result } = renderHook(() => useConversaciones(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.items).toHaveLength(3)
        expect(result.current.data?.totalNoLeidos).toBe(2)
    })

    it("isLoading is true initially", () => {
        vi.mocked(conversacionApi.getAll).mockReturnValue(new Promise(() => {}))

        const { result } = renderHook(() => useConversaciones(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("isError is true when API fails", async () => {
        vi.mocked(conversacionApi.getAll).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(() => useConversaciones(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
    })

    it("calls service with default filter 'todas'", async () => {
        vi.mocked(conversacionApi.getAll).mockResolvedValue(
            mockConversacionListResponse
        )

        const { result } = renderHook(() => useConversaciones(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(conversacionApi.getAll).toHaveBeenCalledWith({
            contexto: "todas",
            page: 1,
            pageSize: 20,
        })
    })

    it("calls service with filter 'necesidades'", async () => {
        vi.mocked(conversacionApi.getAll).mockResolvedValue(
            mockConversacionListResponse
        )

        const { result } = renderHook(
            () => useConversaciones("necesidades"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(conversacionApi.getAll).toHaveBeenCalledWith(
            expect.objectContaining({ contexto: "necesidades" })
        )
    })

    it("calls service with filter 'acuerdos'", async () => {
        vi.mocked(conversacionApi.getAll).mockResolvedValue(
            mockConversacionListResponse
        )

        const { result } = renderHook(
            () => useConversaciones("acuerdos"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(conversacionApi.getAll).toHaveBeenCalledWith(
            expect.objectContaining({ contexto: "acuerdos" })
        )
    })

    it("calls service with correct page number", async () => {
        vi.mocked(conversacionApi.getAll).mockResolvedValue(
            mockConversacionListResponse
        )

        const { result } = renderHook(
            () => useConversaciones("todas", 3),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(conversacionApi.getAll).toHaveBeenCalledWith(
            expect.objectContaining({ page: 3 })
        )
    })
})
