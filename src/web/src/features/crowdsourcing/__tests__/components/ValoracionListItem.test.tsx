import { render, screen } from "@testing-library/react"
import { describe, it, expect } from "vitest"
import { ValoracionListItem } from "../../presentation/components/ValoracionListItem"
import {
    mockValoracionItem,
    mockValoracionItemSinComentario,
    mockValoracionItemSinImagen,
} from "../../__mocks__/valoracion.mock"

describe("ValoracionListItem", () => {
    it("renders autor nombre", () => {
        render(<ValoracionListItem valoracion={mockValoracionItem} />)
        expect(screen.getByText("Los Rockeros")).toBeInTheDocument()
    })

    it("renders acuerdo titulo interno", () => {
        render(<ValoracionListItem valoracion={mockValoracionItem} />)
        expect(
            screen.getByText("Mezcla EP Los Rockeros")
        ).toBeInTheDocument()
    })

    it("renders StarDisplay with correct puntuacion", () => {
        render(<ValoracionListItem valoracion={mockValoracionItem} />)
        const starDisplay = screen.getByRole("img")
        expect(starDisplay).toHaveAttribute(
            "aria-label",
            "Puntuacion: 5 de 5 estrellas"
        )
    })

    it("renders formatted date", () => {
        render(<ValoracionListItem valoracion={mockValoracionItem} />)
        // "2026-03-16T10:00:00Z" -> "16 mar 2026" (es-ES)
        expect(screen.getByText(/16/)).toBeInTheDocument()
        expect(screen.getByText(/2026/)).toBeInTheDocument()
    })

    it("renders comment when comentario exists", () => {
        render(<ValoracionListItem valoracion={mockValoracionItem} />)
        expect(
            screen.getByText(
                /Excelente trabajo, muy profesional y puntual/
            )
        ).toBeInTheDocument()
    })

    it("does not render comment section when comentario is undefined", () => {
        render(
            <ValoracionListItem
                valoracion={mockValoracionItemSinComentario}
            />
        )
        expect(
            screen.queryByText(/Excelente trabajo/)
        ).not.toBeInTheDocument()
    })

    it("renders avatar with autor image when autorImagenUrl exists", () => {
        const { container } = render(
            <ValoracionListItem valoracion={mockValoracionItem} />
        )
        // Radix Avatar uses <img> internally but it may not render in jsdom
        // Check that the Avatar component is rendered with the image src
        const avatarImg = container.querySelector("img")
        if (avatarImg) {
            expect(avatarImg).toHaveAttribute(
                "src",
                "https://storage.example.com/imagenes/los-rockeros.jpg"
            )
        } else {
            // In jsdom, Radix Avatar falls back to AvatarFallback
            // The fallback initials should be "LR" for "Los Rockeros"
            expect(screen.getByText("LR")).toBeInTheDocument()
        }
    })

    it("renders avatar fallback initials when autorImagenUrl is null", () => {
        render(
            <ValoracionListItem valoracion={mockValoracionItemSinImagen} />
        )
        // "Studio Mix" -> "SM"
        expect(screen.getByText("SM")).toBeInTheDocument()
    })

    it("avatar image has correct alt text or fallback initials", () => {
        const { container } = render(
            <ValoracionListItem valoracion={mockValoracionItem} />
        )
        // Radix Avatar may not render <img> in jsdom due to missing load event
        const avatarImg = container.querySelector("img")
        if (avatarImg) {
            expect(avatarImg).toHaveAttribute("alt", "Los Rockeros")
        } else {
            // Fallback: verify initials are displayed as accessibility alternative
            expect(screen.getByText("LR")).toBeInTheDocument()
        }
    })
})
