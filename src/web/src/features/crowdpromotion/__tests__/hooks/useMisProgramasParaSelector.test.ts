import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import React from "react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"

vi.mock("../../infrastructure/inscripcion.service", () => ({
    inscripcionService: {
        misProgramas: vi.fn(),
        explorarProgramas: vi.fn(),
        solicitarInscripcion: vi.fn(),
    },
}))

import { inscripcionService } from "../../infrastructure/inscripcion.service"
import { useMisProgramasParaSelector } from "../../metricas/application/hooks/useMisProgramasParaSelector"

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(QueryClientProvider, { client: queryClient }, children)
    }
}

describe("useMisProgramasParaSelector", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns list of approved programs", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue({
            items: [
                {
                    id: "ins-001",
                    programaId: "prog-001",
                    programaTitulo: "Mi programa",
                    artistaNombre: "Artist",
                    tipoPromoNombre: "Social",
                    importeComisionPorcentaje: 10,
                    importeComisionFija: null,
                    monedaNombre: "EUR",
                    esAprobado: true,
                    esBloqueado: false,
                    codigoReferido: "ref-001",
                    urlTrackingPersonalizada: "https://example.com?ref=ref-001",
                    fechaAlta: "2026-01-01",
                    fechaBaja: null,
                    estado: "Aprobado" as const,
                },
            ],
            totalCount: 1,
            page: 1,
            pageSize: 50,
            totalPages: 1,
        })

        const { result } = renderHook(() => useMisProgramasParaSelector(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isLoading).toBe(false))
        expect(result.current.programas).toHaveLength(1)
        expect(result.current.programas[0].id).toBe("prog-001")
        expect(result.current.programas[0].nombrePrograma).toBe("Mi programa")
    })

    it("isLoading is true initially", () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue({
            items: [],
            totalCount: 0,
            page: 1,
            pageSize: 50,
            totalPages: 0,
        })

        const { result } = renderHook(() => useMisProgramasParaSelector(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("isError is true when service fails", async () => {
        vi.mocked(inscripcionService.misProgramas).mockRejectedValue(
            new Error("Server error")
        )

        const { result } = renderHook(() => useMisProgramasParaSelector(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true), { timeout: 5000 })
    })

    it("returns empty array when no approved programs", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue({
            items: [
                {
                    id: "ins-001",
                    programaId: "prog-001",
                    programaTitulo: "Programa Pendiente",
                    artistaNombre: "Artist",
                    tipoPromoNombre: "Social",
                    importeComisionPorcentaje: 10,
                    importeComisionFija: null,
                    monedaNombre: "EUR",
                    esAprobado: false,
                    esBloqueado: false,
                    codigoReferido: null,
                    urlTrackingPersonalizada: null,
                    fechaAlta: "2026-01-01",
                    fechaBaja: null,
                    estado: "Pendiente" as const,
                },
            ],
            totalCount: 1,
            page: 1,
            pageSize: 50,
            totalPages: 1,
        })

        const { result } = renderHook(() => useMisProgramasParaSelector(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isLoading).toBe(false))
        expect(result.current.programas).toHaveLength(0)
    })

    it("each item has codigoReferido and urlTrackingPersonalizada", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue({
            items: [
                {
                    id: "ins-001",
                    programaId: "prog-001",
                    programaTitulo: "Mi programa",
                    artistaNombre: "Artist",
                    tipoPromoNombre: "Social",
                    importeComisionPorcentaje: 10,
                    importeComisionFija: null,
                    monedaNombre: "EUR",
                    esAprobado: true,
                    esBloqueado: false,
                    codigoReferido: "my-code",
                    urlTrackingPersonalizada: "https://example.com?ref=my-code",
                    fechaAlta: "2026-01-01",
                    fechaBaja: null,
                    estado: "Aprobado" as const,
                },
            ],
            totalCount: 1,
            page: 1,
            pageSize: 50,
            totalPages: 1,
        })

        const { result } = renderHook(() => useMisProgramasParaSelector(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isLoading).toBe(false))
        expect(result.current.programas[0].codigoReferido).toBe("my-code")
        expect(result.current.programas[0].urlTrackingPersonalizada).toBe(
            "https://example.com?ref=my-code"
        )
    })
})
