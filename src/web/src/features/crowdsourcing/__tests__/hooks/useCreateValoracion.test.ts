import { createElement } from "react"
import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { describe, it, expect, vi, beforeEach } from "vitest"
import { useCreateValoracion } from "../../application/hooks/useCreateValoracion"
import {
    MOCK_ACUERDO_ID,
    MOCK_USER_ID,
    mockValoracionCreada,
} from "../../__mocks__/valoracion.mock"

vi.mock("../../infrastructure", () => ({
    valoracionApi: {
        create: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

import { valoracionApi } from "../../infrastructure"
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

describe("useCreateValoracion", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls valoracionApi.create with acuerdoId and request data", async () => {
        vi.mocked(valoracionApi.create).mockResolvedValue(
            mockValoracionCreada
        )
        const { result } = renderHook(
            () =>
                useCreateValoracion(MOCK_ACUERDO_ID, MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        act(() => {
            result.current.mutate({
                puntuacion: 5,
                comentario: "Excelente",
            })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(valoracionApi.create).toHaveBeenCalledWith(
            MOCK_ACUERDO_ID,
            { puntuacion: 5, comentario: "Excelente" }
        )
    })

    it("isSuccess is true after successful mutation", async () => {
        vi.mocked(valoracionApi.create).mockResolvedValue(
            mockValoracionCreada
        )
        const { result } = renderHook(
            () =>
                useCreateValoracion(MOCK_ACUERDO_ID, MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        act(() => {
            result.current.mutate({ puntuacion: 5 })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
    })

    it("returns created valoracion data on success", async () => {
        vi.mocked(valoracionApi.create).mockResolvedValue(
            mockValoracionCreada
        )
        const { result } = renderHook(
            () =>
                useCreateValoracion(MOCK_ACUERDO_ID, MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        act(() => {
            result.current.mutate({ puntuacion: 5 })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data).toEqual(mockValoracionCreada)
    })

    it("shows success toast with message", async () => {
        vi.mocked(valoracionApi.create).mockResolvedValue(
            mockValoracionCreada
        )
        const { result } = renderHook(
            () =>
                useCreateValoracion(MOCK_ACUERDO_ID, MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        act(() => {
            result.current.mutate({ puntuacion: 5 })
        })

        await waitFor(() => {
            expect(toast.success).toHaveBeenCalledWith(
                "Valoracion enviada. Gracias por tu feedback."
            )
        })
    })

    it("invalidates acuerdo query on success", async () => {
        vi.mocked(valoracionApi.create).mockResolvedValue(
            mockValoracionCreada
        )
        const queryClient = createTestQueryClient()
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const { result } = renderHook(
            () =>
                useCreateValoracion(MOCK_ACUERDO_ID, MOCK_USER_ID),
            { wrapper: createWrapper(queryClient) }
        )

        act(() => {
            result.current.mutate({ puntuacion: 5 })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(invalidateSpy).toHaveBeenCalledWith(
            expect.objectContaining({
                queryKey: expect.arrayContaining([
                    "crowdsourcing",
                    "acuerdos",
                    MOCK_ACUERDO_ID,
                ]),
            })
        )
    })

    it("isError is true when API fails", async () => {
        vi.mocked(valoracionApi.create).mockRejectedValue(
            new Error("5000")
        )
        const { result } = renderHook(
            () =>
                useCreateValoracion(MOCK_ACUERDO_ID, MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        act(() => {
            result.current.mutate({ puntuacion: 5 })
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
    })

    it("shows error toast when API fails", async () => {
        vi.mocked(valoracionApi.create).mockRejectedValue(
            new Error("4014")
        )
        const { result } = renderHook(
            () =>
                useCreateValoracion(MOCK_ACUERDO_ID, MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        act(() => {
            result.current.mutate({ puntuacion: 5 })
        })

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalled()
        })
    })

    it("accepts request without comentario", async () => {
        vi.mocked(valoracionApi.create).mockResolvedValue(
            mockValoracionCreada
        )
        const { result } = renderHook(
            () =>
                useCreateValoracion(MOCK_ACUERDO_ID, MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        act(() => {
            result.current.mutate({ puntuacion: 4 })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(valoracionApi.create).toHaveBeenCalledWith(
            MOCK_ACUERDO_ID,
            { puntuacion: 4 }
        )
    })

    it("isPending is true during mutation execution", async () => {
        let resolveCreate: (value: unknown) => void
        vi.mocked(valoracionApi.create).mockImplementation(
            () =>
                new Promise((resolve) => {
                    resolveCreate = resolve
                })
        )
        const { result } = renderHook(
            () =>
                useCreateValoracion(MOCK_ACUERDO_ID, MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        act(() => {
            result.current.mutate({ puntuacion: 5 })
        })

        await waitFor(() =>
            expect(result.current.isPending).toBe(true)
        )

        // Clean up
        await act(async () => {
            resolveCreate!(mockValoracionCreada)
        })
    })

    it("calls onSuccess callback when provided", async () => {
        vi.mocked(valoracionApi.create).mockResolvedValue(
            mockValoracionCreada
        )
        const onSuccess = vi.fn()
        const { result } = renderHook(
            () =>
                useCreateValoracion(
                    MOCK_ACUERDO_ID,
                    MOCK_USER_ID,
                    onSuccess
                ),
            { wrapper: createWrapper() }
        )

        act(() => {
            result.current.mutate({ puntuacion: 5 })
        })

        await waitFor(() => {
            expect(onSuccess).toHaveBeenCalledWith(mockValoracionCreada)
        })
    })
})
