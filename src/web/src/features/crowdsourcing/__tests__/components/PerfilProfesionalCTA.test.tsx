import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { BrowserRouter } from "react-router-dom"
import { PerfilProfesionalCTA } from "../../presentation/components/PerfilProfesionalCTA"

function renderWithRouter(ui: React.ReactElement) {
    return render(<BrowserRouter>{ui}</BrowserRouter>)
}

describe("PerfilProfesionalCTA", () => {
    it("renders informational text", () => {
        renderWithRouter(<PerfilProfesionalCTA />)

        expect(
            screen.getByText("Necesitas un perfil profesional para enviar propuestas")
        ).toBeInTheDocument()
    })

    it("renders create profile button", () => {
        renderWithRouter(<PerfilProfesionalCTA />)

        expect(screen.getByText("Crear perfil profesional")).toBeInTheDocument()
    })

    it("links to perfil profesional creation page", () => {
        renderWithRouter(<PerfilProfesionalCTA />)

        const link = screen.getByRole("link")
        expect(link).toHaveAttribute("href", "/perfil-profesional/crear")
    })
})
