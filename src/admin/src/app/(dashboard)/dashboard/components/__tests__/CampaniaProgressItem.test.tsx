import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import { CampaniaProgressItem } from "../CampaniaProgressItem"
import type { MiCampaniaListItem } from "@shared/types"

const mockCampania: MiCampaniaListItem = {
    id: "campania-1",
    titulo: "Mi Album Debut",
    imagenPrincipalUrl: "https://example.com/album.jpg",
    estadoCampaniaId: 2,
    estadoCampaniaNombre: "Publicada",
    importeObjetivo: 5000.0,
    importeRecaudado: 2340.5,
    porcentajeProgreso: 46.81,
    numBackers: 78,
    diasRestantes: 46,
    fechaFin: "2026-03-31T23:59:59Z",
    fechaCreacion: "2026-01-15T12:00:00Z",
}

describe("CampaniaProgressItem", () => {
    it("renders campania title", () => {
        render(<CampaniaProgressItem campania={mockCampania} />)

        expect(screen.getByText("Mi Album Debut")).toBeInTheDocument()
    })

    it("renders estado badge", () => {
        render(<CampaniaProgressItem campania={mockCampania} />)

        expect(screen.getByText("Publicada")).toBeInTheDocument()
    })

    it("displays progress percentage", () => {
        render(<CampaniaProgressItem campania={mockCampania} />)

        expect(screen.getByText("47%")).toBeInTheDocument()
    })

    it("displays number of backers", () => {
        render(<CampaniaProgressItem campania={mockCampania} />)

        expect(screen.getByText("78 backers")).toBeInTheDocument()
    })

    it("renders progress bar", () => {
        render(<CampaniaProgressItem campania={mockCampania} />)

        const progressBar = screen.getByRole("progressbar")
        expect(progressBar).toBeInTheDocument()
    })

    it("renders as a link to campania detail", () => {
        render(<CampaniaProgressItem campania={mockCampania} />)

        const link = screen.getByRole("link")
        expect(link).toHaveAttribute("href", "/campanias/campania-1")
    })

    it("handles borrador estado", () => {
        const borrador: MiCampaniaListItem = {
            ...mockCampania,
            estadoCampaniaId: 1,
            estadoCampaniaNombre: "Borrador",
            porcentajeProgreso: 0,
            numBackers: 0,
        }

        render(<CampaniaProgressItem campania={borrador} />)

        expect(screen.getByText("Borrador")).toBeInTheDocument()
        expect(screen.getByText("0 backers")).toBeInTheDocument()
    })
})
