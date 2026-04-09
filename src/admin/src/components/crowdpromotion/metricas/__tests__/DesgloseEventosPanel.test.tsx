import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import { DesgloseEventosPanel } from "../DesgloseEventosPanel"
import { mockKpisConDatos, mockKpisVacios } from "@/__mocks__/cp-tracking-metricas.mock"

describe("DesgloseEventosPanel", () => {
    it("renders Clicks label and count", () => {
        render(<DesgloseEventosPanel kpis={mockKpisConDatos} />)
        expect(screen.getByText("Clicks")).toBeInTheDocument()
        expect(screen.getByText(/1[.,]?250/)).toBeInTheDocument()
    })

    it("renders Page Views label and count", () => {
        render(<DesgloseEventosPanel kpis={mockKpisConDatos} />)
        expect(screen.getByText("Page Views")).toBeInTheDocument()
        expect(screen.getByText("890")).toBeInTheDocument()
    })

    it("renders Signups label and count", () => {
        render(<DesgloseEventosPanel kpis={mockKpisConDatos} />)
        expect(screen.getByText("Signups")).toBeInTheDocument()
        expect(screen.getByText("45")).toBeInTheDocument()
    })

    it("renders Conversiones label and count", () => {
        render(<DesgloseEventosPanel kpis={mockKpisConDatos} />)
        expect(screen.getByText("Conversiones")).toBeInTheDocument()
        expect(screen.getByText("12")).toBeInTheDocument()
    })

    it("renders progress bars (one per event type)", () => {
        const { container } = render(<DesgloseEventosPanel kpis={mockKpisConDatos} />)
        const bars = container.querySelectorAll("[data-testid^='bar-']")
        expect(bars.length).toBe(4)
    })

    it("clicks bar has largest width", () => {
        const { container } = render(<DesgloseEventosPanel kpis={mockKpisConDatos} />)
        const clicksBar = container.querySelector("[data-testid='bar-totalClicks']") as HTMLElement
        const signupsBar = container.querySelector("[data-testid='bar-totalSignups']") as HTMLElement

        const clicksWidth = parseFloat(clicksBar.style.width)
        const signupsWidth = parseFloat(signupsBar.style.width)
        expect(clicksWidth).toBeGreaterThan(signupsWidth)
    })

    it("renders bars with zero width when all kpis are zero", () => {
        const { container } = render(<DesgloseEventosPanel kpis={mockKpisVacios} />)
        const bars = container.querySelectorAll("[data-testid^='bar-']")
        bars.forEach((bar) => {
            expect((bar as HTMLElement).style.width).toBe("0%")
        })
    })

    it("calculates percentage correctly", () => {
        const { container } = render(<DesgloseEventosPanel kpis={mockKpisConDatos} />)
        const clicksBar = container.querySelector("[data-testid='bar-totalClicks']") as HTMLElement
        // Total = 1250 + 890 + 45 + 12 = 2197
        // Clicks percentage = 1250/2197 * 100 ≈ 56.9%
        const width = parseFloat(clicksBar.style.width)
        expect(width).toBeCloseTo(56.9, 0)
    })
})
