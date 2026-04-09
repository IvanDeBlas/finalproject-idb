import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@/test-utils"
import { PropuestaCard } from "../PropuestaCard"
import type { PropuestaCrowdsourcing } from "@shared/types"

const mockPropuesta: PropuestaCrowdsourcing = {
    id: "prop-1",
    profesionalId: "prof-1",
    profesionalNombre: "Juan Perez",
    precioPropuesto: 600,
    monedaId: 1,
    tiempoEstimadoDias: 14,
    mensaje: "Tengo 8 anos de experiencia mezclando indie rock y folk.",
    estadoPropuestaId: 1,
    estadoPropuestaNombre: "Pendiente",
    fechaCreacion: "2026-02-17T09:15:00Z",
}

describe("PropuestaCard", () => {
    it("renders propuesta data correctly", () => {
        render(<PropuestaCard propuesta={mockPropuesta} />)

        expect(screen.getByText("Juan Perez")).toBeInTheDocument()
        expect(screen.getByText("Pendiente")).toBeInTheDocument()
        expect(screen.getByText("14 dias")).toBeInTheDocument()
        expect(
            screen.getByText(/Tengo 8 anos de experiencia/i)
        ).toBeInTheDocument()
    })

    it("displays profesional avatar initials", () => {
        render(<PropuestaCard propuesta={mockPropuesta} />)

        expect(screen.getByText("J")).toBeInTheDocument()
    })

    it("truncates long messages and shows Leer mas", () => {
        const longMessage = "A".repeat(200)
        const propuestaLarga = { ...mockPropuesta, mensaje: longMessage }

        render(<PropuestaCard propuesta={propuestaLarga} />)

        expect(screen.getByText(/Leer mas/i)).toBeInTheDocument()
    })

    it("expands message on Leer mas click", () => {
        const longMessage = "A".repeat(200)
        const propuestaLarga = { ...mockPropuesta, mensaje: longMessage }

        render(<PropuestaCard propuesta={propuestaLarga} />)

        fireEvent.click(screen.getByText(/Leer mas/i))

        expect(screen.getByText(/Leer menos/i)).toBeInTheDocument()
    })

    it("shows action buttons when not readonly", () => {
        render(
            <PropuestaCard
                propuesta={mockPropuesta}
                onVerPerfil={vi.fn()}
                onAceptar={vi.fn()}
                onRechazar={vi.fn()}
            />
        )

        expect(screen.getByText("Ver Perfil")).toBeInTheDocument()
        expect(screen.getByText("Aceptar")).toBeInTheDocument()
        expect(screen.getByText("Rechazar")).toBeInTheDocument()
    })

    it("hides action buttons when readonly", () => {
        render(
            <PropuestaCard
                propuesta={mockPropuesta}
                readonly={true}
            />
        )

        expect(screen.queryByText("Aceptar")).not.toBeInTheDocument()
        expect(screen.queryByText("Rechazar")).not.toBeInTheDocument()
    })

    it("calls onAceptar when Aceptar button clicked", () => {
        const handleAceptar = vi.fn()
        render(
            <PropuestaCard
                propuesta={mockPropuesta}
                onAceptar={handleAceptar}
            />
        )

        fireEvent.click(screen.getByText("Aceptar"))

        expect(handleAceptar).toHaveBeenCalled()
    })

    it("calls onRechazar when Rechazar button clicked", () => {
        const handleRechazar = vi.fn()
        render(
            <PropuestaCard
                propuesta={mockPropuesta}
                onRechazar={handleRechazar}
            />
        )

        fireEvent.click(screen.getByText("Rechazar"))

        expect(handleRechazar).toHaveBeenCalled()
    })
})
