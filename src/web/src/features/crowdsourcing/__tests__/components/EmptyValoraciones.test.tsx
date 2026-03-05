import { render, screen } from "@testing-library/react"
import { describe, it, expect } from "vitest"
import { EmptyValoraciones } from "../../presentation/components/EmptyValoraciones"

describe("EmptyValoraciones", () => {
    it("renders empty state message", () => {
        render(<EmptyValoraciones />)
        expect(
            screen.getByText("Este usuario aun no tiene valoraciones")
        ).toBeInTheDocument()
    })

    it("renders exact text 'Este usuario aun no tiene valoraciones'", () => {
        render(<EmptyValoraciones />)
        expect(
            screen.getByText("Este usuario aun no tiene valoraciones")
        ).toBeVisible()
    })

    it("renders secondary helper text", () => {
        render(<EmptyValoraciones />)
        expect(
            screen.getByText(
                "Completa un acuerdo para recibir tu primera valoracion"
            )
        ).toBeInTheDocument()
    })

    it("renders star icon as visual indicator", () => {
        const { container } = render(<EmptyValoraciones />)
        const svgs = container.querySelectorAll("svg")
        expect(svgs.length).toBeGreaterThanOrEqual(1)
    })

    it("has accessible role status", () => {
        render(<EmptyValoraciones />)
        expect(screen.getByRole("status")).toBeInTheDocument()
    })
})
