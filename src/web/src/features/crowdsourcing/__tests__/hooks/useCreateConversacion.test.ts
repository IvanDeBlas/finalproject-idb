import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { useCreateConversacion } from "../../application/hooks/useCreateConversacion"
import { mockCreateConversacionResult } from "../../__mocks__/mensajeria.mock"

vi.mock("../../infrastructure", () => ({
    conversacionApi: {
        getAll: vi.fn(),
        create: vi.fn(),
        getNoLeidosCount: vi.fn(),
    },
}))

import { conversacionApi } from "../../infrastructure"

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return createElement(QueryClientProvider, { client: queryClient }, children)
    }
}

describe("useCreateConversacion", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("mutation succeeds and returns CreateConversacionResult", async () => {
        vi.mocked(conversacionApi.create).mockResolvedValue(
            mockCreateConversacionResult
        )

        const { result } = renderHook(() => useCreateConversacion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                asunto: "Test",
                userIdDestinatario: "user-abc",
            })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data?.id).toBe(
            mockCreateConversacionResult.id
        )
    })

    it("calls service with correct data", async () => {
        vi.mocked(conversacionApi.create).mockResolvedValue(
            mockCreateConversacionResult
        )

        const { result } = renderHook(() => useCreateConversacion(), {
            wrapper: createWrapper(),
        })

        const requestData = {
            asunto: "Consulta sobre propuesta",
            userIdDestinatario: "user-xyz",
            necesidadId: "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
        }

        await act(async () => {
            result.current.mutate(requestData)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(conversacionApi.create).toHaveBeenCalledWith(requestData)
    })

    it("isError is true when API fails with error 4015", async () => {
        vi.mocked(conversacionApi.create).mockRejectedValue(
            new Error("4015")
        )

        const { result } = renderHook(() => useCreateConversacion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                asunto: "Test",
                userIdDestinatario: "user-abc",
            })
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(result.current.error?.message).toBe("4015")
    })

    it("isError is true when API fails with error 3002", async () => {
        vi.mocked(conversacionApi.create).mockRejectedValue(
            new Error("3002")
        )

        const { result } = renderHook(() => useCreateConversacion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                asunto: "Test",
                userIdDestinatario: "user-abc",
            })
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(result.current.error?.message).toBe("3002")
    })

    it("isError is true on generic API failure", async () => {
        vi.mocked(conversacionApi.create).mockRejectedValue(
            new Error("5000")
        )

        const { result } = renderHook(() => useCreateConversacion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                asunto: "Test",
                userIdDestinatario: "user-abc",
            })
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
    })

    it("isPending is true during mutation", async () => {
        let resolvePromise: (value: typeof mockCreateConversacionResult) => void
        vi.mocked(conversacionApi.create).mockReturnValue(
            new Promise((resolve) => {
                resolvePromise = resolve
            })
        )

        const { result } = renderHook(() => useCreateConversacion(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate({
                asunto: "Test",
                userIdDestinatario: "user-abc",
            })
        })

        await waitFor(() => expect(result.current.isPending).toBe(true))

        await act(async () => {
            resolvePromise!(mockCreateConversacionResult)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
    })
})
