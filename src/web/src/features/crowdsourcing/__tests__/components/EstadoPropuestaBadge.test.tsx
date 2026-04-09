import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { EstadoPropuestaBadge } from "../../presentation/components/EstadoPropuestaBadge"

describe("EstadoPropuestaBadge", () => {
    it("renders estado nombre text", () => {
        render(<EstadoPropuestaBadge estadoId={1} estadoNombre="Pendiente" />)

        expect(screen.getByText("Pendiente")).toBeInTheDocument()
    })

    it("has correct aria-label", () => {
        render(<EstadoPropuestaBadge estadoId={2} estadoNombre="Aceptada" />)

        expect(screen.getByLabelText("Estado: Aceptada")).toBeInTheDocument()
    })

    it("renders amber styling for PENDIENTE", () => {
        const { container } = render(
            <EstadoPropuestaBadge estadoId={1} estadoNombre="Pendiente" />
        )
        const badge = container.querySelector("[class*='amber']")
        expect(badge).not.toBeNull()
    })

    it("renders green styling for ACEPTADA", () => {
        const { container } = render(
            <EstadoPropuestaBadge estadoId={2} estadoNombre="Aceptada" />
        )
        const badge = container.querySelector("[class*='green']")
        expect(badge).not.toBeNull()
    })

    it("renders red styling for RECHAZADA", () => {
        const { container } = render(
            <EstadoPropuestaBadge estadoId={3} estadoNombre="Rechazada" />
        )
        const badge = container.querySelector("[class*='red']")
        expect(badge).not.toBeNull()
    })

    it("renders gray styling for RETIRADA", () => {
        const { container } = render(
            <EstadoPropuestaBadge estadoId={4} estadoNombre="Retirada" />
        )
        const badge = container.querySelector("[class*='gray']")
        expect(badge).not.toBeNull()
    })

    it("renders gray styling for unknown estado", () => {
        const { container } = render(
            <EstadoPropuestaBadge estadoId={99} estadoNombre="Desconocido" />
        )
        const badge = container.querySelector("[class*='gray']")
        expect(badge).not.toBeNull()
    })
})
