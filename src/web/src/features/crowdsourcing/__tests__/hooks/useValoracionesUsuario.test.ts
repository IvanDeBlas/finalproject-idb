import { createElement } from "react"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { describe, it, expect, vi, beforeEach } from "vitest"
import { useValoracionesUsuario } from "../../application/hooks/useValoracionesUsuario"
import {
    MOCK_USER_ID,
    mockValoracionesUsuarioConDatos,
} from "../../__mocks__/valoracion.mock"

vi.mock("../../infrastructure", () => ({
    valoracionApi: {
        getByUser: vi.fn(),
    },
}))

import { valoracionApi } from "../../infrastructure"

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

describe("useValoracionesUsuario", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("fetches valoraciones data successfully", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        const { result } = renderHook(
            () => useValoracionesUsuario(MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data).toEqual(
            mockValoracionesUsuarioConDatos
        )
    })

    it("isLoading is true initially", () => {
        vi.mocked(valoracionApi.getByUser).mockImplementation(
            () => new Promise(() => {})
        )
        const { result } = renderHook(
            () => useValoracionesUsuario(MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        expect(result.current.isLoading).toBe(true)
    })

    it("isSuccess is true after data loads", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        const { result } = renderHook(
            () => useValoracionesUsuario(MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
    })

    it("data contains resumen and valoraciones list", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        const { result } = renderHook(
            () => useValoracionesUsuario(MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data?.resumen).toBeDefined()
        expect(result.current.data?.valoraciones).toBeDefined()
        expect(result.current.data?.valoraciones.items).toHaveLength(2)
    })

    it("calls API with correct userId", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        renderHook(
            () => useValoracionesUsuario(MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        await waitFor(() => {
            expect(valoracionApi.getByUser).toHaveBeenCalledWith(
                MOCK_USER_ID,
                { page: 1, pageSize: 10 }
            )
        })
    })

    it("calls API with page and pageSize params", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        renderHook(
            () => useValoracionesUsuario(MOCK_USER_ID, 2, 5),
            { wrapper: createWrapper() }
        )

        await waitFor(() => {
            expect(valoracionApi.getByUser).toHaveBeenCalledWith(
                MOCK_USER_ID,
                { page: 2, pageSize: 5 }
            )
        })
    })

    it("defaults to page 1 and pageSize 10", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        renderHook(
            () => useValoracionesUsuario(MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        await waitFor(() => {
            expect(valoracionApi.getByUser).toHaveBeenCalledWith(
                MOCK_USER_ID,
                { page: 1, pageSize: 10 }
            )
        })
    })

    it("isError is true when API fails", async () => {
        vi.mocked(valoracionApi.getByUser).mockRejectedValue(
            new Error("5000")
        )
        const { result } = renderHook(
            () => useValoracionesUsuario(MOCK_USER_ID),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true))
    })

    it("query is disabled when userId is empty string", () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        const { result } = renderHook(
            () => useValoracionesUsuario(""),
            { wrapper: createWrapper() }
        )

        expect(result.current.isLoading).toBe(false)
        expect(result.current.fetchStatus).toBe("idle")
        expect(valoracionApi.getByUser).not.toHaveBeenCalled()
    })

    it("re-fetches when page param changes", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        const { result, rerender } = renderHook(
            ({ page }: { page: number }) =>
                useValoracionesUsuario(MOCK_USER_ID, page, 10),
            {
                wrapper: createWrapper(),
                initialProps: { page: 1 },
            }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(valoracionApi.getByUser).toHaveBeenCalledTimes(1)

        rerender({ page: 2 })

        await waitFor(() => {
            expect(valoracionApi.getByUser).toHaveBeenCalledWith(
                MOCK_USER_ID,
                { page: 2, pageSize: 10 }
            )
        })
    })
})
