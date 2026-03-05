import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@testing-library/react"
import { NecesidadCard } from "../../presentation/components/NecesidadCard"
import { mockNecesidadUrgente, mockNecesidadNormal } from "../../__mocks__/necesidad.mock"

describe("NecesidadCard", () => {
    it("renders titulo and descripcion", () => {
        render(<NecesidadCard necesidad={mockNecesidadUrgente} />)

        expect(screen.getByText("Mezcla de pistas para EP de 5 canciones")).toBeInTheDocument()
        expect(screen.getByText(/Buscamos un ingeniero/)).toBeInTheDocument()
    })

    it("renders tipo and modalidad badges", () => {
        render(<NecesidadCard necesidad={mockNecesidadUrgente} />)

        expect(screen.getByText("Post-produccion")).toBeInTheDocument()
        expect(screen.getByText("Remoto")).toBeInTheDocument()
    })

    it("renders urgencia badge when esUrgente is true", () => {
        render(<NecesidadCard necesidad={mockNecesidadUrgente} />)

        expect(screen.getByText("URGENTE")).toBeInTheDocument()
    })

    it("does not render urgencia badge when esUrgente is false", () => {
        render(<NecesidadCard necesidad={mockNecesidadNormal} />)

        expect(screen.queryByText("URGENTE")).not.toBeInTheDocument()
    })

    it("renders artista nombre", () => {
        render(<NecesidadCard necesidad={mockNecesidadUrgente} />)

        expect(screen.getByText("Los Rockeros")).toBeInTheDocument()
    })

    it("renders presupuesto range", () => {
        render(<NecesidadCard necesidad={mockNecesidadUrgente} />)

        expect(screen.getByText(/150 - 800 EUR/)).toBeInTheDocument()
    })

    it("renders propuestas count", () => {
        render(<NecesidadCard necesidad={mockNecesidadUrgente} />)

        expect(screen.getByText("3 propuesta(s)")).toBeInTheDocument()
    })

    it("renders 'Sin propuestas aun' when numeroPropuestas is 0", () => {
        render(
            <NecesidadCard
                necesidad={{ ...mockNecesidadNormal, numeroPropuestas: 0 }}
            />
        )

        expect(screen.getByText("Sin propuestas aun")).toBeInTheDocument()
    })

    it("calls onClick when card is clicked", () => {
        const handleClick = vi.fn()
        render(<NecesidadCard necesidad={mockNecesidadUrgente} onClick={handleClick} />)

        fireEvent.click(screen.getByRole("article"))
        expect(handleClick).toHaveBeenCalledOnce()
    })

    it("calls onClick on Enter key press", () => {
        const handleClick = vi.fn()
        render(<NecesidadCard necesidad={mockNecesidadUrgente} onClick={handleClick} />)

        fireEvent.keyDown(screen.getByRole("article"), { key: "Enter" })
        expect(handleClick).toHaveBeenCalledOnce()
    })

    it("calls onClick on Space key press", () => {
        const handleClick = vi.fn()
        render(<NecesidadCard necesidad={mockNecesidadUrgente} onClick={handleClick} />)

        fireEvent.keyDown(screen.getByRole("article"), { key: " " })
        expect(handleClick).toHaveBeenCalledOnce()
    })

    it("has correct aria-label", () => {
        render(<NecesidadCard necesidad={mockNecesidadUrgente} />)

        const card = screen.getByRole("article")
        expect(card.getAttribute("aria-label")).toContain("Mezcla de pistas")
        expect(card.getAttribute("aria-label")).toContain("Post-produccion")
    })

    it("renders fecha limite when provided", () => {
        render(<NecesidadCard necesidad={mockNecesidadUrgente} />)

        expect(screen.getByText(/Limite:/)).toBeInTheDocument()
    })

    it("renders ubicacion badge for presencial modalidad", () => {
        render(<NecesidadCard necesidad={mockNecesidadNormal} />)

        expect(screen.getByText("Presencial")).toBeInTheDocument()
    })
})
