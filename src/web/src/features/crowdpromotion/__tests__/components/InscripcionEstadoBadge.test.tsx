import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { InscripcionEstadoBadge } from "../../presentation/components/InscripcionEstadoBadge"

describe("InscripcionEstadoBadge", () => {
    it("renders Pendiente badge", () => {
        render(<InscripcionEstadoBadge estado="Pendiente" />)

        expect(screen.getByText("Pendiente de aprobacion")).toBeInTheDocument()
        expect(screen.getByLabelText("Estado: Pendiente de aprobacion")).toBeInTheDocument()
    })

    it("renders Aprobado badge", () => {
        render(<InscripcionEstadoBadge estado="Aprobado" />)

        expect(screen.getByText("Aprobado")).toBeInTheDocument()
        expect(screen.getByLabelText("Estado: Aprobado")).toBeInTheDocument()
    })

    it("renders Bloqueado badge", () => {
        render(<InscripcionEstadoBadge estado="Bloqueado" />)

        expect(screen.getByText("Bloqueado")).toBeInTheDocument()
        expect(screen.getByLabelText("Estado: Bloqueado")).toBeInTheDocument()
    })

    it("renders DadoDeBaja badge", () => {
        render(<InscripcionEstadoBadge estado="DadoDeBaja" />)

        expect(screen.getByText("Dado de baja")).toBeInTheDocument()
    })

    it("renders amber styling for Pendiente", () => {
        const { container } = render(<InscripcionEstadoBadge estado="Pendiente" />)
        const badge = container.querySelector("[class*='amber']")
        expect(badge).not.toBeNull()
    })

    it("renders green styling for Aprobado", () => {
        const { container } = render(<InscripcionEstadoBadge estado="Aprobado" />)
        const badge = container.querySelector("[class*='green']")
        expect(badge).not.toBeNull()
    })

    it("renders red styling for Bloqueado", () => {
        const { container } = render(<InscripcionEstadoBadge estado="Bloqueado" />)
        const badge = container.querySelector("[class*='red']")
        expect(badge).not.toBeNull()
    })
})
