import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import React from "react"
import { mockPaginatedProgramas } from "../../__mocks__/inscripcion.mock"

vi.mock("../../infrastructure/inscripcion.service", () => ({
    inscripcionService: {
        explorarProgramas: vi.fn(),
        solicitarInscripcion: vi.fn(),
        misProgramas: vi.fn(),
    },
}))

import { inscripcionService } from "../../infrastructure/inscripcion.service"
import { useExplorarProgramas } from "../../application/hooks/useExplorarProgramas"

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

describe("useExplorarProgramas", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns paginated programs on success", async () => {
        vi.mocked(inscripcionService.explorarProgramas).mockResolvedValue(mockPaginatedProgramas)

        const { result } = renderHook(
            () => useExplorarProgramas(),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.items).toHaveLength(5)
        expect(result.current.data?.totalCount).toBe(5)
    })

    it("isLoading true initially", () => {
        vi.mocked(inscripcionService.explorarProgramas).mockReturnValue(
            new Promise(() => {})
        )

        const { result } = renderHook(
            () => useExplorarProgramas(),
            { wrapper: createWrapper() }
        )

        expect(result.current.isLoading).toBe(true)
    })

    it("isError true when service fails", async () => {
        vi.mocked(inscripcionService.explorarProgramas).mockRejectedValue(
            new Error("API Error")
        )

        const { result } = renderHook(
            () => useExplorarProgramas(),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true), { timeout: 5000 })

        expect(result.current.data).toBeUndefined()
    })

    it("passes artistaNombre filter to service", async () => {
        vi.mocked(inscripcionService.explorarProgramas).mockResolvedValue(mockPaginatedProgramas)

        const { result } = renderHook(
            () => useExplorarProgramas({ artistaNombre: "Luna Nova" }),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(inscripcionService.explorarProgramas).toHaveBeenCalledWith(
            expect.objectContaining({ artistaNombre: "Luna Nova" })
        )
    })

    it("passes tipoPromoId filter to service", async () => {
        vi.mocked(inscripcionService.explorarProgramas).mockResolvedValue(mockPaginatedProgramas)

        const { result } = renderHook(
            () => useExplorarProgramas({ tipoPromoId: 2 }),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(inscripcionService.explorarProgramas).toHaveBeenCalledWith(
            expect.objectContaining({ tipoPromoId: 2 })
        )
    })

    it("updates when filters change", async () => {
        vi.mocked(inscripcionService.explorarProgramas).mockResolvedValue(mockPaginatedProgramas)

        const { result, rerender } = renderHook(
            ({ filters }) => useExplorarProgramas(filters),
            {
                wrapper: createWrapper(),
                initialProps: { filters: undefined as Record<string, unknown> | undefined },
            }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        rerender({ filters: { artistaNombre: "Luna Nova" } })

        await waitFor(() =>
            expect(inscripcionService.explorarProgramas).toHaveBeenCalledTimes(2)
        )

        expect(inscripcionService.explorarProgramas).toHaveBeenLastCalledWith(
            expect.objectContaining({ artistaNombre: "Luna Nova" })
        )
    })
})
