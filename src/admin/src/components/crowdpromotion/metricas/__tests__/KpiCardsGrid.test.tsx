import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import { KpiCardsGrid } from "../KpiCardsGrid"
import { mockKpisConDatos, mockKpisVacios } from "@/__mocks__/cp-tracking-metricas.mock"

describe("KpiCardsGrid", () => {
    it("renders 4 primary KPI labels", () => {
        render(<KpiCardsGrid kpis={mockKpisConDatos} />)
        expect(screen.getByText("Clicks totales")).toBeInTheDocument()
        expect(screen.getByText("Registros")).toBeInTheDocument()
        expect(screen.getByText("Backings")).toBeInTheDocument()
        expect(screen.getByText("Valor generado")).toBeInTheDocument()
    })

    it("renders 2 secondary KPI labels", () => {
        render(<KpiCardsGrid kpis={mockKpisConDatos} />)
        expect(screen.getByText("Tasa de conversion")).toBeInTheDocument()
        expect(screen.getByText("Comisiones")).toBeInTheDocument()
    })

    it("renders totalClicks formatted", () => {
        render(<KpiCardsGrid kpis={mockKpisConDatos} />)
        // toLocaleString may or may not add thousands separator depending on env
        expect(screen.getByText(/1[.,]?250/)).toBeInTheDocument()
    })

    it("renders totalConversiones", () => {
        render(<KpiCardsGrid kpis={mockKpisConDatos} />)
        expect(screen.getByText("12")).toBeInTheDocument()
    })

    it("renders valorTotalGenerado in amber color", () => {
        render(<KpiCardsGrid kpis={mockKpisConDatos} />)
        // Value 1200 might be formatted as "1.200", "1,200", or "1200"
        const valorElement = screen.getByText(/1[.,]?200/)
        expect(valorElement.className).toContain("text-[#f59e0b]")
    })

    it("renders monedaNombre as suffix", () => {
        render(<KpiCardsGrid kpis={mockKpisConDatos} />)
        const eurElements = screen.getAllByText("EUR")
        expect(eurElements.length).toBeGreaterThanOrEqual(1)
    })

    it("renders EUR fallback when monedaNombre is null", () => {
        render(<KpiCardsGrid kpis={mockKpisVacios} />)
        const eurElements = screen.getAllByText("EUR")
        expect(eurElements.length).toBeGreaterThanOrEqual(1)
    })

    it("renders tasaConversion formatted", () => {
        render(<KpiCardsGrid kpis={mockKpisConDatos} />)
        expect(screen.getByText("0.96%")).toBeInTheDocument()
    })

    it("renders comisionesTotales", () => {
        render(<KpiCardsGrid kpis={mockKpisConDatos} />)
        expect(screen.getByText("120")).toBeInTheDocument()
    })

    it("renders zeros correctly for empty kpis", () => {
        render(<KpiCardsGrid kpis={mockKpisVacios} />)
        expect(screen.getByText("0.00%")).toBeInTheDocument()
        const zeroElements = screen.getAllByText("0")
        expect(zeroElements.length).toBeGreaterThanOrEqual(4)
    })
})
