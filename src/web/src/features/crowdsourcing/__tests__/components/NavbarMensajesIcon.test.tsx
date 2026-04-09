import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { MemoryRouter } from "react-router-dom"
import { NavbarMensajesIcon } from "../../presentation/components/NavbarMensajesIcon"

function renderIcon(totalNoLeidos: number) {
    return render(
        <MemoryRouter>
            <NavbarMensajesIcon totalNoLeidos={totalNoLeidos} />
        </MemoryRouter>
    )
}

describe("NavbarMensajesIcon", () => {
    it("renders the messaging icon", () => {
        renderIcon(0)
        const link = screen.getByRole("link")
        expect(link).toBeInTheDocument()
    })

    it("shows badge when totalNoLeidos > 0", () => {
        renderIcon(3)
        expect(screen.getByText("3")).toBeInTheDocument()
    })

    it("hides badge when totalNoLeidos is 0", () => {
        renderIcon(0)
        const badge = screen.getByLabelText("0 mensajes no leidos")
        expect(badge.classList.contains("hidden")).toBe(true)
    })

    it("shows exact number in badge (1-99)", () => {
        renderIcon(42)
        expect(screen.getByText("42")).toBeInTheDocument()
    })

    it("shows 99+ when totalNoLeidos > 99", () => {
        renderIcon(150)
        expect(screen.getByText("99+")).toBeInTheDocument()
    })

    it("badge has correct aria-label", () => {
        renderIcon(3)
        expect(
            screen.getByLabelText("3 mensajes no leidos")
        ).toBeInTheDocument()
    })

    it("link points to /crowdsourcing/mensajes", () => {
        renderIcon(0)
        const link = screen.getByRole("link")
        expect(link).toHaveAttribute("href", "/crowdsourcing/mensajes")
    })
})
