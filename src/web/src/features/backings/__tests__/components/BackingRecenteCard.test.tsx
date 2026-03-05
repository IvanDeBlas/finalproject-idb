import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { BackingRecenteCard } from "@/features/campanias/presentation/components/BackingRecenteCard"
import type { BackingPublicDto } from "@shared/types/backing"

const mockBacking: BackingPublicDto = {
    id: "700e8400-e29b-41d4-a716-446655440001",
    nombreBacker: "Maria Lopez",
    monto: 25,
    rewardNombre: "CD Fisico Firmado",
    mensaje: "Mucha suerte con el proyecto!",
    fechaCreacion: new Date().toISOString(),
}

const mockAnonymousBacking: BackingPublicDto = {
    id: "800e8400-e29b-41d4-a716-446655440002",
    nombreBacker: "Anonimo",
    monto: 50,
    rewardNombre: undefined,
    mensaje: undefined,
    fechaCreacion: new Date().toISOString(),
}

describe("BackingRecenteCard", () => {
    it("renders backer name", () => {
        render(<BackingRecenteCard backing={mockBacking} />)

        expect(screen.getByText("Maria Lopez")).toBeInTheDocument()
    })

    it("renders amount", () => {
        render(<BackingRecenteCard backing={mockBacking} />)

        expect(screen.getByText(/25/)).toBeInTheDocument()
    })

    it("renders reward name when available", () => {
        render(<BackingRecenteCard backing={mockBacking} />)

        expect(screen.getByText(/CD Fisico Firmado/)).toBeInTheDocument()
    })

    it("renders message when available", () => {
        render(<BackingRecenteCard backing={mockBacking} />)

        expect(screen.getByText(/Mucha suerte con el proyecto/)).toBeInTheDocument()
    })

    it("shows 'Anonimo' for anonymous backer", () => {
        render(<BackingRecenteCard backing={mockAnonymousBacking} />)

        expect(screen.getByText("Anonimo")).toBeInTheDocument()
    })

    it("does not show reward name when undefined", () => {
        render(<BackingRecenteCard backing={mockAnonymousBacking} />)

        expect(screen.queryByText("CD Fisico Firmado")).not.toBeInTheDocument()
    })

    it("does not show message when undefined", () => {
        render(<BackingRecenteCard backing={mockAnonymousBacking} />)

        // No message should be shown - the component doesn't render the blockquote
        const allText = screen.queryByText(/"/i)
        expect(allText).not.toBeInTheDocument()
    })

    it("shows relative time for fecha", () => {
        render(<BackingRecenteCard backing={mockBacking} />)

        // Since fechaCreacion is now, should show something like "hace unos segundos" or "ahora"
        expect(screen.getByText(/hace|ahora/i)).toBeInTheDocument()
    })
})
