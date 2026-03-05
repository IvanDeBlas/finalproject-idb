import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import React from "react"
import { mockPaginatedMisProgramas, mockEmptyMisProgramas } from "../../__mocks__/inscripcion.mock"

vi.mock("../../infrastructure/inscripcion.service", () => ({
    inscripcionService: {
        explorarProgramas: vi.fn(),
        solicitarInscripcion: vi.fn(),
        misProgramas: vi.fn(),
    },
}))

import { inscripcionService } from "../../infrastructure/inscripcion.service"
import { useMisProgramas } from "../../application/hooks/useMisProgramas"

const createWrapper = () => {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
        },
    })

    return function Wrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(
            QueryClientProvider,
            { client: queryClient },
            children
        )
    }
}

describe("useMisProgramas", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns inscripciones list on success", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockPaginatedMisProgramas)

        const { result } = renderHook(
            () => useMisProgramas(),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.items).toHaveLength(4)
        expect(result.current.data?.totalCount).toBe(4)
    })

    it("isLoading true initially", () => {
        vi.mocked(inscripcionService.misProgramas).mockReturnValue(
            new Promise(() => {})
        )

        const { result } = renderHook(
            () => useMisProgramas(),
            { wrapper: createWrapper() }
        )

        expect(result.current.isLoading).toBe(true)
    })

    it("isError true when service fails", async () => {
        vi.mocked(inscripcionService.misProgramas).mockRejectedValue(
            new Error("API Error")
        )

        const { result } = renderHook(
            () => useMisProgramas(),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true), { timeout: 5000 })

        expect(result.current.data).toBeUndefined()
    })

    it("aprobado item has codigoReferido and urlTracking", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockPaginatedMisProgramas)

        const { result } = renderHook(
            () => useMisProgramas(),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        const aprobado = result.current.data?.items.find(i => i.estado === "Aprobado")
        expect(aprobado?.codigoReferido).toBe("album-2026-x7k9m")
        expect(aprobado?.urlTrackingPersonalizada).toContain("utm_source=weplay")
    })

    it("pendiente item has null codigoReferido", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockPaginatedMisProgramas)

        const { result } = renderHook(
            () => useMisProgramas(),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        const pendiente = result.current.data?.items.find(i => i.estado === "Pendiente")
        expect(pendiente?.codigoReferido).toBeNull()
        expect(pendiente?.urlTrackingPersonalizada).toBeNull()
    })

    it("returns empty items when promotor has no inscripciones", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockEmptyMisProgramas)

        const { result } = renderHook(
            () => useMisProgramas(),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.items).toHaveLength(0)
        expect(result.current.data?.totalCount).toBe(0)
    })
})
