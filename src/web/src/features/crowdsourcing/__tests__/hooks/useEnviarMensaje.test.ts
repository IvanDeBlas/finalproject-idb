import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { useEnviarMensaje } from "../../application/hooks/useEnviarMensaje"
import { mockMensajeNuevo } from "../../__mocks__/mensajeria.mock"

vi.mock("../../infrastructure", () => ({
    mensajeApi: {
        getByConversacion: vi.fn(),
        create: vi.fn(),
        marcarLeidos: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
        info: vi.fn(),
    },
}))

import { mensajeApi } from "../../infrastructure"
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

describe("useEnviarMensaje", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("mutation succeeds and returns the created message", async () => {
        vi.mocked(mensajeApi.create).mockResolvedValue(mockMensajeNuevo)

        const { result } = renderHook(
            () => useEnviarMensaje("conv-123"),
            { wrapper: createWrapper() }
        )

        await act(async () => {
            result.current.mutate({
                contenido: "Hola mundo",
            })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data).toEqual(mockMensajeNuevo)
    })

    it("invalidates mensajes query on success", async () => {
        vi.mocked(mensajeApi.create).mockResolvedValue(mockMensajeNuevo)
        const queryClient = createTestQueryClient()
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const { result } = renderHook(
            () => useEnviarMensaje("conv-123"),
            { wrapper: createWrapper(queryClient) }
        )

        await act(async () => {
            result.current.mutate({ contenido: "Test" })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(invalidateSpy).toHaveBeenCalled()
    })

    it("invalidates conversaciones query on success", async () => {
        vi.mocked(mensajeApi.create).mockResolvedValue(mockMensajeNuevo)
        const queryClient = createTestQueryClient()
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const { result } = renderHook(
            () => useEnviarMensaje("conv-123"),
            { wrapper: createWrapper(queryClient) }
        )

        await act(async () => {
            result.current.mutate({ contenido: "Test" })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        const calls = invalidateSpy.mock.calls.map((c) => c[0])
        const hasConversacionesInvalidation = calls.some(
            (call) =>
                JSON.stringify(call).includes("conversaciones")
        )
        expect(hasConversacionesInvalidation).toBe(true)
    })

    it("calls service with conversacionId and data", async () => {
        vi.mocked(mensajeApi.create).mockResolvedValue(mockMensajeNuevo)

        const { result } = renderHook(
            () => useEnviarMensaje("conv-abc"),
            { wrapper: createWrapper() }
        )

        await act(async () => {
            result.current.mutate({
                contenido: "Mensaje de prueba",
                urlAdjunto: "https://example.com",
            })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(mensajeApi.create).toHaveBeenCalledWith("conv-abc", {
            contenido: "Mensaje de prueba",
            urlAdjunto: "https://example.com",
        })
    })

    it("isError is true when API fails", async () => {
        vi.mocked(mensajeApi.create).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(
            () => useEnviarMensaje("conv-123"),
            { wrapper: createWrapper() }
        )

        await act(async () => {
            result.current.mutate({ contenido: "Test" })
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
    })

    it("shows toast error when API fails", async () => {
        vi.mocked(mensajeApi.create).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(
            () => useEnviarMensaje("conv-123"),
            { wrapper: createWrapper() }
        )

        await act(async () => {
            result.current.mutate({ contenido: "Test" })
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(toast.error).toHaveBeenCalledWith(
            "No se pudo enviar el mensaje. Intenta de nuevo."
        )
    })

    it("accepts message without urlAdjunto", async () => {
        vi.mocked(mensajeApi.create).mockResolvedValue(mockMensajeNuevo)

        const { result } = renderHook(
            () => useEnviarMensaje("conv-123"),
            { wrapper: createWrapper() }
        )

        await act(async () => {
            result.current.mutate({ contenido: "Solo texto" })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(mensajeApi.create).toHaveBeenCalledWith("conv-123", {
            contenido: "Solo texto",
        })
    })
})
