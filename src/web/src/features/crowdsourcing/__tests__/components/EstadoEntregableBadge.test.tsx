import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { EstadoEntregableBadge } from "../../presentation/components/EstadoEntregableBadge"

describe("EstadoEntregableBadge", () => {
    it("renders badge text for Entregado", () => {
        render(<EstadoEntregableBadge estadoId={1} estadoNombre="Entregado" />)

        expect(screen.getByText("Entregado")).toBeInTheDocument()
    })

    it("renders badge text for Aprobado", () => {
        render(<EstadoEntregableBadge estadoId={2} estadoNombre="Aprobado" />)

        expect(screen.getByText("Aprobado")).toBeInTheDocument()
    })

    it("renders badge text for Rechazado", () => {
        render(<EstadoEntregableBadge estadoId={3} estadoNombre="Rechazado" />)

        expect(screen.getByText("Rechazado")).toBeInTheDocument()
    })

    it("has correct aria-label", () => {
        render(<EstadoEntregableBadge estadoId={1} estadoNombre="Entregado" />)

        expect(screen.getByLabelText("Estado: Entregado")).toBeInTheDocument()
    })

    it("applies amber styling for Entregado", () => {
        const { container } = render(
            <EstadoEntregableBadge estadoId={1} estadoNombre="Entregado" />
        )
        const badge = container.querySelector("[class*='amber']")
        expect(badge).not.toBeNull()
    })

    it("applies green styling for Aprobado", () => {
        const { container } = render(
            <EstadoEntregableBadge estadoId={2} estadoNombre="Aprobado" />
        )
        const badge = container.querySelector("[class*='green']")
        expect(badge).not.toBeNull()
    })

    it("applies red styling for Rechazado", () => {
        const { container } = render(
            <EstadoEntregableBadge estadoId={3} estadoNombre="Rechazado" />
        )
        const badge = container.querySelector("[class*='red']")
        expect(badge).not.toBeNull()
    })
})
