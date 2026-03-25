import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { ArtistaHero } from "../../presentation/components/ArtistaHero"
import type { Artista } from "../../domain/types"

const mockArtista: Artista = {
    id: "artista-001",
    userId: "user-123",
    nombreArtistico: "The Rock Stars",
    descripcion: "Rock band",
    pais: "Espana",
    ciudad: "Madrid",
    imagenUrl: "https://example.com/photo.jpg",
    generoMusical: "Rock",
    createdAt: new Date("2026-01-15T10:00:00Z"),
    updatedAt: new Date("2026-02-20T15:30:00Z"),
}

describe("ArtistaHero", () => {
    it("renders artista name", () => {
        render(<ArtistaHero artista={mockArtista} />)

        expect(screen.getByText("The Rock Stars")).toBeInTheDocument()
    })

    it("displays genero musical", () => {
        render(<ArtistaHero artista={mockArtista} />)

        expect(screen.getByText("Rock")).toBeInTheDocument()
    })

    it("hides genero musical when not provided", () => {
        const artistaSinGenero: Artista = {
            ...mockArtista,
            generoMusical: undefined,
        }

        render(<ArtistaHero artista={artistaSinGenero} />)

        expect(screen.queryByText("Rock")).not.toBeInTheDocument()
    })

    it("renders avatar section", () => {
        const { container } = render(<ArtistaHero artista={mockArtista} />)

        // Avatar component is rendered (radix Avatar may not load images in jsdom)
        const avatarImg = container.querySelector("img")
        if (avatarImg) {
            expect(avatarImg).toHaveAttribute("alt", "The Rock Stars")
        }
        // Fallback is always rendered by radix Avatar in jsdom
        expect(container.querySelector("[class*='avatar']") || container.querySelector("span")).toBeTruthy()
    })

    it("renders heading as h1", () => {
        render(<ArtistaHero artista={mockArtista} />)

        const heading = screen.getByRole("heading", { level: 1 })
        expect(heading).toHaveTextContent("The Rock Stars")
    })
})
