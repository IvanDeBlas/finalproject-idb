import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@testing-library/react"
import { ConversacionRow } from "../../presentation/components/ConversacionRow"
import {
    mockConversacionConNoLeidos,
    mockConversacionSinNoLeidos,
    mockConversacionSobreAcuerdo,
    mockConversacionSinMensajes,
    makeConversacion,
} from "../../__mocks__/mensajeria.mock"

describe("ConversacionRow", () => {
    it("renders nombre de la otra parte", () => {
        render(
            <ConversacionRow
                conversacion={mockConversacionConNoLeidos}
                onClick={vi.fn()}
            />
        )

        expect(screen.getByText("Studio Mix Pro")).toBeInTheDocument()
    })

    it("renders contexto titulo", () => {
        render(
            <ConversacionRow
                conversacion={mockConversacionConNoLeidos}
                onClick={vi.fn()}
            />
        )

        expect(
            screen.getByText("Mezcla de pistas para EP")
        ).toBeInTheDocument()
    })

    it("shows preview of ultimo mensaje when it exists", () => {
        render(
            <ConversacionRow
                conversacion={mockConversacionConNoLeidos}
                onClick={vi.fn()}
            />
        )

        expect(
            screen.getByText(/te envio los stems manana/)
        ).toBeInTheDocument()
    })

    it("hides preview when ultimoMensaje is null", () => {
        render(
            <ConversacionRow
                conversacion={mockConversacionSinMensajes}
                onClick={vi.fn()}
            />
        )

        expect(
            screen.queryByText(/te envio los stems/)
        ).not.toBeInTheDocument()
    })

    it("shows badge when mensajesNoLeidos > 0", () => {
        render(
            <ConversacionRow
                conversacion={mockConversacionConNoLeidos}
                onClick={vi.fn()}
            />
        )

        expect(screen.getByText("2")).toBeInTheDocument()
    })

    it("hides badge when mensajesNoLeidos is 0", () => {
        render(
            <ConversacionRow
                conversacion={mockConversacionSinNoLeidos}
                onClick={vi.fn()}
            />
        )

        expect(screen.queryByText(/mensajes no leidos/)).not.toBeInTheDocument()
    })

    it("has different background when there are unread messages", () => {
        const { container } = render(
            <ConversacionRow
                conversacion={mockConversacionConNoLeidos}
                onClick={vi.fn()}
            />
        )

        const button = container.querySelector("button")
        expect(button?.className).toContain("bg-[#1e2a42]")
    })

    it("shows initials as fallback when no avatar image", () => {
        render(
            <ConversacionRow
                conversacion={mockConversacionSinNoLeidos}
                onClick={vi.fn()}
            />
        )

        // "Diseno Grafico Pro" -> first 2 chars "DI"
        expect(screen.getByText("DI")).toBeInTheDocument()
    })

    it("calls onClick when row is clicked", () => {
        const handleClick = vi.fn()
        render(
            <ConversacionRow
                conversacion={mockConversacionConNoLeidos}
                onClick={handleClick}
            />
        )

        fireEvent.click(screen.getByRole("button"))
        expect(handleClick).toHaveBeenCalledTimes(1)
    })

    it("badge has accessible aria-label", () => {
        render(
            <ConversacionRow
                conversacion={mockConversacionConNoLeidos}
                onClick={vi.fn()}
            />
        )

        expect(
            screen.getByLabelText("2 mensajes no leidos")
        ).toBeInTheDocument()
    })

    it("badge shows 99+ when mensajesNoLeidos > 99", () => {
        const conv = makeConversacion({ mensajesNoLeidos: 150 })
        render(
            <ConversacionRow conversacion={conv} onClick={vi.fn()} />
        )

        expect(screen.getByText("99+")).toBeInTheDocument()
    })

    it("shows acuerdo contexto titulo", () => {
        render(
            <ConversacionRow
                conversacion={mockConversacionSobreAcuerdo}
                onClick={vi.fn()}
            />
        )

        expect(
            screen.getByText("Sesion de fotos promo")
        ).toBeInTheDocument()
    })
})
