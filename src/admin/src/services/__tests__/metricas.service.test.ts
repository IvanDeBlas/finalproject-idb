import { describe, it, expect, vi, beforeEach } from "vitest"
import { metricasService } from "../metricas.service"
import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import {
    mockProgramaMetricasResponse,
    mockFiltroConFechas,
    PROGRAMA_ID,
} from "@/__mocks__/cp-tracking-metricas.mock"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
}))

const mockedApiFetch = vi.mocked(apiFetch)

describe("MetricasService", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls correct URL without date filters", async () => {
        mockedApiFetch.mockResolvedValue({
            data: mockProgramaMetricasResponse,
            messages: [],
        })

        await metricasService.getProgramaMetricas(PROGRAMA_ID)

        const calledUrl = mockedApiFetch.mock.calls[0][0] as string
        expect(calledUrl).toBe(API_ROUTES.crowdpromotion.programaMetricas(PROGRAMA_ID))
        expect(calledUrl).not.toContain("?")
    })

    it("appends fechaDesde when provided", async () => {
        mockedApiFetch.mockResolvedValue({
            data: mockProgramaMetricasResponse,
            messages: [],
        })

        await metricasService.getProgramaMetricas(PROGRAMA_ID, { fechaDesde: "2026-03-01" })

        const calledUrl = mockedApiFetch.mock.calls[0][0] as string
        expect(calledUrl).toContain("fechaDesde=2026-03-01")
    })

    it("appends fechaHasta when provided", async () => {
        mockedApiFetch.mockResolvedValue({
            data: mockProgramaMetricasResponse,
            messages: [],
        })

        await metricasService.getProgramaMetricas(PROGRAMA_ID, { fechaHasta: "2026-03-31" })

        const calledUrl = mockedApiFetch.mock.calls[0][0] as string
        expect(calledUrl).toContain("fechaHasta=2026-03-31")
    })

    it("appends both date params when both provided", async () => {
        mockedApiFetch.mockResolvedValue({
            data: mockProgramaMetricasResponse,
            messages: [],
        })

        await metricasService.getProgramaMetricas(PROGRAMA_ID, mockFiltroConFechas)

        const calledUrl = mockedApiFetch.mock.calls[0][0] as string
        expect(calledUrl).toContain("fechaDesde=2026-03-01")
        expect(calledUrl).toContain("fechaHasta=2026-03-31")
    })

    it("returns ProgramaMetricasResponse on success", async () => {
        mockedApiFetch.mockResolvedValue({
            data: mockProgramaMetricasResponse,
            messages: [],
        })

        const result = await metricasService.getProgramaMetricas(PROGRAMA_ID)

        expect(result).toEqual(mockProgramaMetricasResponse)
    })

    it("throws on 401 error response", async () => {
        mockedApiFetch.mockResolvedValue({
            data: null,
            messages: [{ message: "No autorizado", errorCode: "3001" }],
        })

        await expect(metricasService.getProgramaMetricas(PROGRAMA_ID)).rejects.toThrow("No autorizado")
    })

    it("throws on 403 error response", async () => {
        mockedApiFetch.mockResolvedValue({
            data: null,
            messages: [{ message: "No tienes acceso a este programa", errorCode: "3002" }],
        })

        await expect(metricasService.getProgramaMetricas(PROGRAMA_ID)).rejects.toThrow(
            "No tienes acceso a este programa"
        )
    })

    it("throws on 404 error response", async () => {
        mockedApiFetch.mockResolvedValue({
            data: null,
            messages: [{ message: "Programa no encontrado", errorCode: "2000" }],
        })

        await expect(metricasService.getProgramaMetricas(PROGRAMA_ID)).rejects.toThrow(
            "Programa no encontrado"
        )
    })

    it("throws on 500 error response", async () => {
        mockedApiFetch.mockResolvedValue({
            data: null,
            messages: [{ message: "Error inesperado", errorCode: "5000" }],
        })

        await expect(metricasService.getProgramaMetricas(PROGRAMA_ID)).rejects.toThrow(
            "Error inesperado"
        )
    })

    it("propagates network error", async () => {
        mockedApiFetch.mockRejectedValue(new Error("Network error"))

        await expect(metricasService.getProgramaMetricas(PROGRAMA_ID)).rejects.toThrow(
            "Network error"
        )
    })

    it("does not append empty string fecha params", async () => {
        mockedApiFetch.mockResolvedValue({
            data: mockProgramaMetricasResponse,
            messages: [],
        })

        await metricasService.getProgramaMetricas(PROGRAMA_ID, { fechaDesde: "", fechaHasta: "" })

        const calledUrl = mockedApiFetch.mock.calls[0][0] as string
        expect(calledUrl).not.toContain("fechaDesde=")
        expect(calledUrl).not.toContain("fechaHasta=")
    })

    it("uses API_ROUTES.crowdpromotion.programaMetricas", async () => {
        mockedApiFetch.mockResolvedValue({
            data: mockProgramaMetricasResponse,
            messages: [],
        })

        await metricasService.getProgramaMetricas(PROGRAMA_ID)

        const calledUrl = mockedApiFetch.mock.calls[0][0] as string
        expect(calledUrl).toContain(`/crowdpromotion/programas/${PROGRAMA_ID}/metricas`)
    })
})
