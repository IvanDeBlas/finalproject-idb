import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { useMarcarLeidos } from "../../application/hooks/useMarcarLeidos"
import {
    mockMarcarLeidosResponse,
    mockMarcarLeidosResponseCero,
} from "../../__mocks__/mensajeria.mock"

vi.mock("../../infrastructure", () => ({
    mensajeApi: {
        getByConversacion: vi.fn(),
        create: vi.fn(),
        marcarLeidos: vi.fn(),
    },
}))

import { mensajeApi } from "../../infrastructure"

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

describe("useMarcarLeidos", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("mutation succeeds and returns mensajesMarcados", async () => {
        vi.mocked(mensajeApi.marcarLeidos).mockResolvedValue(
            mockMarcarLeidosResponse
        )

        const { result } = renderHook(
            () => useMarcarLeidos("conv-123"),
            { wrapper: createWrapper() }
        )

        await act(async () => {
            result.current.mutate()
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data?.mensajesMarcados).toBe(2)
    })

    it("invalidates noLeidos query on success", async () => {
        vi.mocked(mensajeApi.marcarLeidos).mockResolvedValue(
            mockMarcarLeidosResponse
        )
        const queryClient = createTestQueryClient()
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const { result } = renderHook(
            () => useMarcarLeidos("conv-123"),
            { wrapper: createWrapper(queryClient) }
        )

        await act(async () => {
            result.current.mutate()
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(invalidateSpy).toHaveBeenCalled()
    })

    it("invalidates conversaciones query on success", async () => {
        vi.mocked(mensajeApi.marcarLeidos).mockResolvedValue(
            mockMarcarLeidosResponse
        )
        const queryClient = createTestQueryClient()
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const { result } = renderHook(
            () => useMarcarLeidos("conv-123"),
            { wrapper: createWrapper(queryClient) }
        )

        await act(async () => {
            result.current.mutate()
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        const calls = invalidateSpy.mock.calls.map((c) => c[0])
        const hasConversacionesInvalidation = calls.some(
            (call) =>
                JSON.stringify(call).includes("conversaciones")
        )
        expect(hasConversacionesInvalidation).toBe(true)
    })

    it("calls service with correct conversacionId", async () => {
        vi.mocked(mensajeApi.marcarLeidos).mockResolvedValue(
            mockMarcarLeidosResponse
        )

        const { result } = renderHook(
            () => useMarcarLeidos("conv-xyz"),
            { wrapper: createWrapper() }
        )

        await act(async () => {
            result.current.mutate()
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(mensajeApi.marcarLeidos).toHaveBeenCalledWith("conv-xyz")
    })

    it("does not error when mensajesMarcados is 0", async () => {
        vi.mocked(mensajeApi.marcarLeidos).mockResolvedValue(
            mockMarcarLeidosResponseCero
        )

        const { result } = renderHook(
            () => useMarcarLeidos("conv-123"),
            { wrapper: createWrapper() }
        )

        await act(async () => {
            result.current.mutate()
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.isError).toBe(false)
        expect(result.current.data?.mensajesMarcados).toBe(0)
    })

    it("isError is true when API fails", async () => {
        vi.mocked(mensajeApi.marcarLeidos).mockRejectedValue(
            new Error("Server error")
        )

        const { result } = renderHook(
            () => useMarcarLeidos("conv-123"),
            { wrapper: createWrapper() }
        )

        await act(async () => {
            result.current.mutate()
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
    })
})
