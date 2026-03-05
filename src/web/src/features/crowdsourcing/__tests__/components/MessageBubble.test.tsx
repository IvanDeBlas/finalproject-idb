import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { MessageBubble } from "../../presentation/components/MessageBubble"
import {
    mockMensajeAjeno,
    mockMensajePropio,
    mockMensajePropioSinAdjunto,
} from "../../__mocks__/mensajeria.mock"

describe("MessageBubble", () => {
    it("renders message content", () => {
        render(<MessageBubble mensaje={mockMensajeAjeno} />)

        expect(
            screen.getByText(/me interesa tu propuesta/)
        ).toBeInTheDocument()
    })

    it("own message is aligned to the right (flex-row-reverse)", () => {
        const { container } = render(
            <MessageBubble mensaje={mockMensajePropio} />
        )

        const wrapper = container.firstElementChild
        expect(wrapper?.className).toContain("flex-row-reverse")
    })

    it("other message is aligned to the left", () => {
        const { container } = render(
            <MessageBubble mensaje={mockMensajeAjeno} />
        )

        const wrapper = container.firstElementChild
        expect(wrapper?.className).not.toContain("flex-row-reverse")
    })

    it("own bubble has gradient class", () => {
        const { container } = render(
            <MessageBubble mensaje={mockMensajePropio} />
        )

        const bubble = container.querySelector("[class*='bg-gradient']")
        expect(bubble).not.toBeNull()
    })

    it("other bubble has neutral background", () => {
        const { container } = render(
            <MessageBubble mensaje={mockMensajeAjeno} />
        )

        const bubble = container.querySelector("[class*='bg-\\[#16213e\\]']")
        expect(bubble).not.toBeNull()
    })

    it("shows sender name for other messages", () => {
        render(<MessageBubble mensaje={mockMensajeAjeno} />)

        expect(screen.getByText("Los Rockeros")).toBeInTheDocument()
    })

    it("hides sender name for own messages", () => {
        render(<MessageBubble mensaje={mockMensajePropio} />)

        expect(
            screen.queryByText("Studio Mix Pro")
        ).not.toBeInTheDocument()
    })

    it("renders attachment link when urlAdjunto exists", () => {
        render(<MessageBubble mensaje={mockMensajePropio} />)

        const link = screen.getByRole("link")
        expect(link).toHaveAttribute(
            "href",
            "https://drive.google.com/portfolio"
        )
    })

    it("attachment link opens in new tab", () => {
        render(<MessageBubble mensaje={mockMensajePropio} />)

        const link = screen.getByRole("link")
        expect(link).toHaveAttribute("target", "_blank")
    })

    it("attachment link has rel noopener noreferrer", () => {
        render(<MessageBubble mensaje={mockMensajePropio} />)

        const link = screen.getByRole("link")
        expect(link).toHaveAttribute("rel", "noopener noreferrer")
    })

    it("does not render attachment when urlAdjunto is null", () => {
        render(<MessageBubble mensaje={mockMensajeAjeno} />)

        expect(screen.queryByRole("link")).not.toBeInTheDocument()
    })

    it("does not render attachment for own message without url", () => {
        render(<MessageBubble mensaje={mockMensajePropioSinAdjunto} />)

        expect(screen.queryByRole("link")).not.toBeInTheDocument()
    })
})
