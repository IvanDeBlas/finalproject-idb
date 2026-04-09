import React from "react"
import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import { GraficoTemporal } from "../GraficoTemporal"
import { mockEventosPorDia } from "@/__mocks__/cp-tracking-metricas.mock"

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

describe("GraficoTemporal", () => {
    it("renders LineChart container when datos has items", () => {
        render(<GraficoTemporal datos={mockEventosPorDia} />)
        expect(screen.getByTestId("line-chart")).toBeInTheDocument()
    })

    it("renders empty state when datos is empty", () => {
        render(<GraficoTemporal datos={[]} />)
        expect(
            screen.getByText("No hay datos para el periodo seleccionado")
        ).toBeInTheDocument()
    })

    it("does not render empty state when datos has items", () => {
        render(<GraficoTemporal datos={mockEventosPorDia} />)
        expect(
            screen.queryByText("No hay datos para el periodo seleccionado")
        ).not.toBeInTheDocument()
    })

    it("renders accessible sr-only description", () => {
        const { container } = render(<GraficoTemporal datos={mockEventosPorDia} />)
        const srOnly = container.querySelector(".sr-only")
        expect(srOnly).toBeInTheDocument()
        expect(srOnly?.textContent).toContain("Grafico de lineas")
    })

    it("renders section title", () => {
        render(<GraficoTemporal datos={mockEventosPorDia} />)
        expect(screen.getByText("Eventos por dia")).toBeInTheDocument()
    })
})
