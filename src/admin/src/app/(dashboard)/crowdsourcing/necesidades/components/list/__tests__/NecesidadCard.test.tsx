import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@/test-utils"
import { NecesidadCard } from "../NecesidadCard"
import { ESTADO_NECESIDAD } from "@shared/constants"
import type { NecesidadCrowdsourcingList } from "@shared/types"

const mockNecesidadAbierta: NecesidadCrowdsourcingList = {
    id: "nec-1",
    titulo: "Mezcla de pistas para EP",
    estadoNecesidadId: ESTADO_NECESIDAD.ABIERTA,
    estadoNecesidadNombre: "Abierta",
    tipoNecesidadId: 3,
    tipoNecesidadNombre: "Ingenieria de Audio",
    presupuestoMin: 150,
    presupuestoMax: 800,
    monedaId: 1,
    monedaNombre: "EUR",
    modalidadTrabajoId: 2,
    modalidadTrabajoNombre: "Remoto",
    numeroPropuestas: 3,
    fechaCreacion: "2026-02-16T10:00:00Z",
    fechaLimitePropuestas: "2026-03-15T00:00:00Z",
    fechaActualizacion: null,
}

const mockNecesidadCerrada: NecesidadCrowdsourcingList = {
    ...mockNecesidadAbierta,
    id: "nec-2",
    estadoNecesidadId: ESTADO_NECESIDAD.CERRADA,
    estadoNecesidadNombre: "Cerrada",
}

const mockNecesidadEnProgreso: NecesidadCrowdsourcingList = {
    ...mockNecesidadAbierta,
    id: "nec-3",
    estadoNecesidadId: ESTADO_NECESIDAD.EN_PROGRESO,
    estadoNecesidadNombre: "En Progreso",
}

describe("NecesidadCard", () => {
    it("renders correctly with all data", () => {
        render(<NecesidadCard necesidad={mockNecesidadAbierta} />)

        expect(screen.getByText("Mezcla de pistas para EP")).toBeInTheDocument()
        expect(screen.getByText("Abierta")).toBeInTheDocument()
        expect(screen.getByText("Ingenieria de Audio")).toBeInTheDocument()
        expect(screen.getByText("3 propuestas")).toBeInTheDocument()
    })

    it("shows Editar button only when estado is Abierta", () => {
        render(<NecesidadCard necesidad={mockNecesidadAbierta} onEdit={vi.fn()} />)

        expect(screen.getByText("Editar")).toBeInTheDocument()
    })

    it("hides Editar button when estado is Cerrada", () => {
        render(<NecesidadCard necesidad={mockNecesidadCerrada} onEdit={vi.fn()} />)

        expect(screen.queryByText("Editar")).not.toBeInTheDocument()
    })

    it("shows Cerrar button for Abierta and En Progreso", () => {
        const { rerender } = render(
            <NecesidadCard necesidad={mockNecesidadAbierta} onCerrar={vi.fn()} />
        )
        expect(screen.getByText("Cerrar")).toBeInTheDocument()

        rerender(<NecesidadCard necesidad={mockNecesidadEnProgreso} onCerrar={vi.fn()} />)
        expect(screen.getByText("Cerrar")).toBeInTheDocument()
    })

    it("hides Cerrar button when estado is Cerrada", () => {
        render(<NecesidadCard necesidad={mockNecesidadCerrada} onCerrar={vi.fn()} />)

        expect(screen.queryByText("Cerrar")).not.toBeInTheDocument()
    })

    it("calls onClick when card is clicked", () => {
        const handleClick = vi.fn()
        render(<NecesidadCard necesidad={mockNecesidadAbierta} onClick={handleClick} />)

        fireEvent.click(screen.getByRole("article"))

        expect(handleClick).toHaveBeenCalled()
    })

    it("calls onEdit when Editar button is clicked", () => {
        const handleEdit = vi.fn()
        render(<NecesidadCard necesidad={mockNecesidadAbierta} onEdit={handleEdit} />)

        fireEvent.click(screen.getByText("Editar"))

        expect(handleEdit).toHaveBeenCalled()
    })

    it("displays Urgente badge when fecha limite < 3 days", () => {
        const urgentNecesidad = {
            ...mockNecesidadAbierta,
            fechaLimitePropuestas: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000).toISOString(),
        }

        render(<NecesidadCard necesidad={urgentNecesidad} />)

        expect(screen.getByText("Urgente")).toBeInTheDocument()
    })

    it("displays Cierra pronto badge when fecha limite < 7 days", () => {
        const closingSoon = {
            ...mockNecesidadAbierta,
            fechaLimitePropuestas: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000).toISOString(),
        }

        render(<NecesidadCard necesidad={closingSoon} />)

        expect(screen.getByText("Cierra pronto")).toBeInTheDocument()
    })

    it("does not display urgency badges when fecha limite is far", () => {
        render(<NecesidadCard necesidad={mockNecesidadAbierta} />)

        expect(screen.queryByText("Urgente")).not.toBeInTheDocument()
        expect(screen.queryByText("Cierra pronto")).not.toBeInTheDocument()
    })
})
