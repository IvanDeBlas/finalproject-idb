import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { ArtistaBio } from "../../presentation/components/ArtistaBio"
import type { Artista } from "../../domain/types"

const mockArtista: Artista = {
    id: "artista-001",
    userId: "user-123",
    nombreArtistico: "Test Artist",
    descripcion: "Una banda de rock alternativo de Madrid",
    pais: "Espana",
    ciudad: "Madrid",
    imagenUrl: "https://example.com/photo.jpg",
    generoMusical: "Rock Alternativo",
    createdAt: new Date("2026-01-15T10:00:00Z"),
    updatedAt: new Date("2026-02-20T15:30:00Z"),
}

describe("ArtistaBio", () => {
    it("renders card title", () => {
        render(<ArtistaBio artista={mockArtista} />)

        expect(screen.getByText("Sobre el Artista")).toBeInTheDocument()
    })

    it("displays artista descripcion", () => {
        render(<ArtistaBio artista={mockArtista} />)

        expect(
            screen.getByText("Una banda de rock alternativo de Madrid")
        ).toBeInTheDocument()
    })

    it("shows fallback text when no descripcion", () => {
        const artistaSinDescripcion: Artista = {
            ...mockArtista,
            descripcion: undefined,
        }

        render(<ArtistaBio artista={artistaSinDescripcion} />)

        expect(
            screen.getByText("No hay descripcion disponible")
        ).toBeInTheDocument()
    })

    it("displays ciudad and pais", () => {
        render(<ArtistaBio artista={mockArtista} />)

        expect(screen.getByText("Madrid, Espana")).toBeInTheDocument()
    })

    it("displays only ciudad when pais is missing", () => {
        const artistaSoloCiudad: Artista = {
            ...mockArtista,
            pais: undefined,
        }

        render(<ArtistaBio artista={artistaSoloCiudad} />)

        expect(screen.getByText("Madrid")).toBeInTheDocument()
    })

    it("displays only pais when ciudad is missing", () => {
        const artistaSoloPais: Artista = {
            ...mockArtista,
            ciudad: undefined,
        }

        render(<ArtistaBio artista={artistaSoloPais} />)

        expect(screen.getByText("Espana")).toBeInTheDocument()
    })

    it("hides location when neither ciudad nor pais", () => {
        const artistaSinUbicacion: Artista = {
            ...mockArtista,
            ciudad: undefined,
            pais: undefined,
        }

        render(<ArtistaBio artista={artistaSinUbicacion} />)

        expect(screen.queryByText("Madrid")).not.toBeInTheDocument()
        expect(screen.queryByText("Espana")).not.toBeInTheDocument()
    })

    it("displays genero musical", () => {
        render(<ArtistaBio artista={mockArtista} />)

        expect(screen.getByText("Rock Alternativo")).toBeInTheDocument()
    })

    it("hides genero musical when not provided", () => {
        const artistaSinGenero: Artista = {
            ...mockArtista,
            generoMusical: undefined,
        }

        render(<ArtistaBio artista={artistaSinGenero} />)

        expect(
            screen.queryByText("Rock Alternativo")
        ).not.toBeInTheDocument()
    })

    it("shows redes sociales placeholder", () => {
        render(<ArtistaBio artista={mockArtista} />)

        expect(screen.getByText("Redes Sociales")).toBeInTheDocument()
        expect(screen.getByText("Proximamente")).toBeInTheDocument()
    })
})
