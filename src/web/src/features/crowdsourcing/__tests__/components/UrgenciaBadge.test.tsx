import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { UrgenciaBadge } from "../../presentation/components/UrgenciaBadge"

describe("UrgenciaBadge", () => {
    it("renders URGENTE when fecha limite is less than 3 days", () => {
        const fechaLimite = new Date(Date.now() + 1 * 86400000).toISOString()
        render(<UrgenciaBadge fechaLimite={fechaLimite} />)

        expect(screen.getByText("URGENTE")).toBeInTheDocument()
    })

    it("returns null when fecha limite is more than 3 days away", () => {
        const fechaLimite = new Date(Date.now() + 10 * 86400000).toISOString()
        const { container } = render(<UrgenciaBadge fechaLimite={fechaLimite} />)

        expect(container.innerHTML).toBe("")
    })

    it("has correct aria-label", () => {
        const fechaLimite = new Date(Date.now() + 1 * 86400000).toISOString()
        render(<UrgenciaBadge fechaLimite={fechaLimite} />)

        expect(
            screen.getByLabelText(/Urgente: menos de 3 dias para el cierre/)
        ).toBeInTheDocument()
    })

    it("renders at exactly 2 days remaining", () => {
        const fechaLimite = new Date(Date.now() + 2 * 86400000).toISOString()
        render(<UrgenciaBadge fechaLimite={fechaLimite} />)

        expect(screen.getByText("URGENTE")).toBeInTheDocument()
    })

    it("does not render at exactly 3 days remaining", () => {
        const fechaLimite = new Date(Date.now() + 3 * 86400000).toISOString()
        const { container } = render(<UrgenciaBadge fechaLimite={fechaLimite} />)

        expect(container.innerHTML).toBe("")
    })
})
