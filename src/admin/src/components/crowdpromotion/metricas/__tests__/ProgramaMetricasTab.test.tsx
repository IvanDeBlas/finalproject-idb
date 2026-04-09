import React from "react"
import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { ProgramaMetricasTab } from "../ProgramaMetricasTab"
import { metricasService } from "@/services/metricas.service"
import {
    PROGRAMA_ID,
    mockProgramaMetricasResponse,
    mockProgramaMetricasResponseVacia,
} from "@/__mocks__/cp-tracking-metricas.mock"

vi.mock("@/services/metricas.service", () => ({
    metricasService: {
        getProgramaMetricas: vi.fn(),
    },
}))

vi.mock("recharts", () => ({
    ResponsiveContainer: ({ children }: { children: React.ReactNode }) => children,
    LineChart: ({ children }: { children: React.ReactNode }) =>
        React.createElement("div", { "data-testid": "line-chart" }, children),
    Line: () => null,
    XAxis: () => null,
    YAxis: () => null,
    CartesianGrid: () => null,
    Tooltip: () => null,
    Legend: () => null,
}))

const mockReplace = vi.fn()
const mockGet = vi.fn(() => null)

vi.mock("next/navigation", () => ({
    useSearchParams: vi.fn(() => ({
        get: mockGet,
        toString: vi.fn(() => ""),
    })),
    useRouter: vi.fn(() => ({
        replace: mockReplace,
        push: vi.fn(),
    })),
}))

const mockedGetProgramaMetricas = vi.mocked(metricasService.getProgramaMetricas)

describe("ProgramaMetricasTab", () => {
    beforeEach(() => {
        vi.clearAllMocks()
        mockGet.mockReturnValue(null)
    })

    it("renders KPI values when data loads successfully", async () => {
        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponse)

        render(<ProgramaMetricasTab programaId={PROGRAMA_ID} />)

        // Wait for data by checking a locale-independent text
        await waitFor(() => {
            expect(screen.getByText("Clicks totales")).toBeInTheDocument()
        })
        expect(screen.getByText("Registros")).toBeInTheDocument()
        expect(screen.getByText("Backings")).toBeInTheDocument()
        expect(screen.getByText("Valor generado")).toBeInTheDocument()
    })

    it("renders ranking promotor names", async () => {
        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponse)

        render(<ProgramaMetricasTab programaId={PROGRAMA_ID} />)

        await waitFor(() => {
            expect(screen.getByText("DJ Mark")).toBeInTheDocument()
        })
        expect(screen.getByText("MusicBlog.es")).toBeInTheDocument()
    })

    it("renders desglose panel labels", async () => {
        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponse)

        render(<ProgramaMetricasTab programaId={PROGRAMA_ID} />)

        await waitFor(() => {
            expect(screen.getByText("Desglose por tipo")).toBeInTheDocument()
        })
        expect(screen.getByText("Page Views")).toBeInTheDocument()
        expect(screen.getByText("Signups")).toBeInTheDocument()
    })

    it("renders grafico container", async () => {
        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponse)

        render(<ProgramaMetricasTab programaId={PROGRAMA_ID} />)

        await waitFor(() => {
            expect(screen.getByTestId("line-chart")).toBeInTheDocument()
        })
    })

    it("shows skeleton while loading", () => {
        mockedGetProgramaMetricas.mockReturnValue(new Promise(() => {}))

        const { container } = render(<ProgramaMetricasTab programaId={PROGRAMA_ID} />)

        const skeletons = container.querySelectorAll(".animate-pulse")
        expect(skeletons.length).toBeGreaterThan(0)
    })

    it("shows error state on service failure", async () => {
        mockedGetProgramaMetricas.mockRejectedValue(new Error("Fail"))

        render(<ProgramaMetricasTab programaId={PROGRAMA_ID} />)

        await waitFor(() => {
            expect(screen.getByText("No se pudieron cargar las metricas")).toBeInTheDocument()
        })
    })

    it("shows retry button on error", async () => {
        mockedGetProgramaMetricas.mockRejectedValue(new Error("Fail"))

        render(<ProgramaMetricasTab programaId={PROGRAMA_ID} />)

        await waitFor(() => {
            expect(screen.getByRole("button", { name: /Reintentar/i })).toBeInTheDocument()
        })
    })

    it("renders FiltroFechas component", async () => {
        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponse)

        render(<ProgramaMetricasTab programaId={PROGRAMA_ID} />)

        await waitFor(() => {
            expect(screen.getByLabelText("Fecha de inicio")).toBeInTheDocument()
        })
        expect(screen.getByLabelText("Fecha de fin")).toBeInTheDocument()
    })

    it("clicking Aplicar updates URL search params", async () => {
        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponse)

        render(<ProgramaMetricasTab programaId={PROGRAMA_ID} />)

        await waitFor(() => {
            expect(screen.getByText("Clicks totales")).toBeInTheDocument()
        })

        await userEvent.click(screen.getByRole("button", { name: /Aplicar/i }))

        expect(mockReplace).toHaveBeenCalled()
    })

    it("clicking Limpiar removes date params from URL", async () => {
        mockGet.mockImplementation((key: string) => {
            if (key === "fechaDesde") return "2026-03-01"
            if (key === "fechaHasta") return "2026-03-31"
            return null
        })

        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponse)

        render(<ProgramaMetricasTab programaId={PROGRAMA_ID} />)

        await waitFor(() => {
            expect(screen.getByText("Clicks totales")).toBeInTheDocument()
        })

        const limpiarBtn = screen.getByRole("button", { name: /Limpiar/i })
        await userEvent.click(limpiarBtn)

        expect(mockReplace).toHaveBeenCalled()
    })

    it("renders empty state for ranking when rankingPromotores is empty", async () => {
        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponseVacia)

        render(<ProgramaMetricasTab programaId={PROGRAMA_ID} />)

        await waitFor(() => {
            expect(
                screen.getByText("Sin promotores con actividad en este periodo")
            ).toBeInTheDocument()
        })
    })

    it("renders empty state for grafico when eventosPorDia is empty", async () => {
        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponseVacia)

        render(<ProgramaMetricasTab programaId={PROGRAMA_ID} />)

        await waitFor(() => {
            expect(
                screen.getByText("No hay datos para el periodo seleccionado")
            ).toBeInTheDocument()
        })
    })

    it("reads initial filter from URL search params", async () => {
        mockGet.mockImplementation((key: string) => {
            if (key === "fechaDesde") return "2026-03-01"
            return null
        })

        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponse)

        render(<ProgramaMetricasTab programaId={PROGRAMA_ID} />)

        await waitFor(() => {
            expect(mockedGetProgramaMetricas).toHaveBeenCalledWith(
                PROGRAMA_ID,
                expect.objectContaining({ fechaDesde: "2026-03-01" })
            )
        })
    })
})
