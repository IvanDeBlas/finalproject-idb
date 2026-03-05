import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { useMensajes } from "../../application/hooks/useMensajes"
import { mockMensajeListResponse } from "../../__mocks__/mensajeria.mock"

vi.mock("../../infrastructure", () => ({
    mensajeApi: {
        getByConversacion: vi.fn(),
        create: vi.fn(),
        marcarLeidos: vi.fn(),
    },
}))

import { mensajeApi } from "../../infrastructure"

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

describe("useMensajes", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns messages for the given conversacionId", async () => {
        vi.mocked(mensajeApi.getByConversacion).mockResolvedValue(
            mockMensajeListResponse
        )

        const { result } = renderHook(() => useMensajes("conv-123"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.items).toHaveLength(3)
    })

    it("isLoading is true initially", () => {
        vi.mocked(mensajeApi.getByConversacion).mockReturnValue(
            new Promise(() => {})
        )

        const { result } = renderHook(() => useMensajes("conv-123"), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("isError is true when API fails", async () => {
        vi.mocked(mensajeApi.getByConversacion).mockRejectedValue(
            new Error("5000")
        )

        const { result } = renderHook(() => useMensajes("conv-123"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
    })

    it("calls service with correct conversacionId", async () => {
        vi.mocked(mensajeApi.getByConversacion).mockResolvedValue(
            mockMensajeListResponse
        )

        const { result } = renderHook(() => useMensajes("conv-abc"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(mensajeApi.getByConversacion).toHaveBeenCalledWith("conv-abc", {
            page: 1,
            pageSize: 50,
        })
    })

    it("calls service with page 1 by default", async () => {
        vi.mocked(mensajeApi.getByConversacion).mockResolvedValue(
            mockMensajeListResponse
        )

        const { result } = renderHook(() => useMensajes("conv-123"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(mensajeApi.getByConversacion).toHaveBeenCalledWith(
            "conv-123",
            expect.objectContaining({ page: 1 })
        )
    })

    it("calls service with specified page number", async () => {
        vi.mocked(mensajeApi.getByConversacion).mockResolvedValue(
            mockMensajeListResponse
        )

        const { result } = renderHook(() => useMensajes("conv-123", 3), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(mensajeApi.getByConversacion).toHaveBeenCalledWith(
            "conv-123",
            expect.objectContaining({ page: 3 })
        )
    })

    it("is disabled when conversacionId is empty", () => {
        const { result } = renderHook(() => useMensajes(""), {
            wrapper: createWrapper(),
        })

        expect(result.current.fetchStatus).toBe("idle")
        expect(mensajeApi.getByConversacion).not.toHaveBeenCalled()
    })
})
