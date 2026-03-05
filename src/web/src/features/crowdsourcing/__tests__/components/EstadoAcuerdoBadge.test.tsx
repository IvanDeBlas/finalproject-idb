import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { EstadoAcuerdoBadge } from "../../presentation/components/EstadoAcuerdoBadge"

describe("EstadoAcuerdoBadge", () => {
    it("renders badge text for Activo", () => {
        render(<EstadoAcuerdoBadge estadoId={1} estadoNombre="Activo" />)

        expect(screen.getByText("Activo")).toBeInTheDocument()
    })

    it("renders badge text for Completado", () => {
        render(<EstadoAcuerdoBadge estadoId={2} estadoNombre="Completado" />)

        expect(screen.getByText("Completado")).toBeInTheDocument()
    })

    it("renders badge text for Cancelado", () => {
        render(<EstadoAcuerdoBadge estadoId={3} estadoNombre="Cancelado" />)

        expect(screen.getByText("Cancelado")).toBeInTheDocument()
    })

    it("has correct aria-label", () => {
        render(<EstadoAcuerdoBadge estadoId={1} estadoNombre="Activo" />)

        expect(screen.getByLabelText("Estado: Activo")).toBeInTheDocument()
    })

    it("applies blue styling for Activo", () => {
        const { container } = render(
            <EstadoAcuerdoBadge estadoId={1} estadoNombre="Activo" />
        )
        const badge = container.querySelector("[class*='blue']")
        expect(badge).not.toBeNull()
    })

    it("applies green styling for Completado", () => {
        const { container } = render(
            <EstadoAcuerdoBadge estadoId={2} estadoNombre="Completado" />
        )
        const badge = container.querySelector("[class*='green']")
        expect(badge).not.toBeNull()
    })

    it("applies gray styling for Cancelado", () => {
        const { container } = render(
            <EstadoAcuerdoBadge estadoId={3} estadoNombre="Cancelado" />
        )
        const badge = container.querySelector("[class*='gray']")
        expect(badge).not.toBeNull()
    })
})
