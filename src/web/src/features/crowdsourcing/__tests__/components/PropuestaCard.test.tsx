import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@testing-library/react"
import { BrowserRouter } from "react-router-dom"
import { PropuestaCard } from "../../presentation/components/PropuestaCard"
import {
    mockPropuestaPendiente,
    mockPropuestaAceptada,
    mockPropuestaRechazada,
} from "../../__mocks__/propuesta.mock"

function renderWithRouter(ui: React.ReactElement) {
    return render(<BrowserRouter>{ui}</BrowserRouter>)
}

describe("PropuestaCard", () => {
    it("renders necesidad titulo", () => {
        renderWithRouter(<PropuestaCard propuesta={mockPropuestaPendiente} />)

        expect(screen.getByText("Mezcla de pistas para EP de 5 canciones")).toBeInTheDocument()
    })

    it("renders artista nombre", () => {
        renderWithRouter(<PropuestaCard propuesta={mockPropuestaPendiente} />)

        expect(screen.getByText("Los Rockeros")).toBeInTheDocument()
    })

    it("renders precio and moneda", () => {
        renderWithRouter(<PropuestaCard propuesta={mockPropuestaPendiente} />)

        expect(screen.getByText(/450/)).toBeInTheDocument()
        expect(screen.getByText(/EUR/)).toBeInTheDocument()
    })

    it("renders estado badge", () => {
        renderWithRouter(<PropuestaCard propuesta={mockPropuestaPendiente} />)

        expect(screen.getByText("Pendiente")).toBeInTheDocument()
    })

    it("shows retirar button for PENDIENTE estado", () => {
        const onRetirar = vi.fn()
        renderWithRouter(
            <PropuestaCard propuesta={mockPropuestaPendiente} onRetirar={onRetirar} />
        )

        const btn = screen.getByText("Retirar propuesta")
        expect(btn).toBeInTheDocument()
        fireEvent.click(btn)
        expect(onRetirar).toHaveBeenCalledWith(mockPropuestaPendiente)
    })

    it("shows ver acuerdo button for ACEPTADA estado with acuerdoId", () => {
        renderWithRouter(<PropuestaCard propuesta={mockPropuestaAceptada} />)

        expect(screen.getByText("Ver acuerdo")).toBeInTheDocument()
    })

    it("does not show retirar button for RECHAZADA estado", () => {
        renderWithRouter(<PropuestaCard propuesta={mockPropuestaRechazada} />)

        expect(screen.queryByText("Retirar propuesta")).not.toBeInTheDocument()
    })

    it("does not show retirar button when onRetirar is not provided", () => {
        renderWithRouter(<PropuestaCard propuesta={mockPropuestaPendiente} />)

        expect(screen.queryByText("Retirar propuesta")).not.toBeInTheDocument()
    })

    it("renders fecha creacion", () => {
        renderWithRouter(<PropuestaCard propuesta={mockPropuestaPendiente} />)

        expect(screen.getByText(/Enviada:/)).toBeInTheDocument()
    })

    it("renders fecha actualizacion when present", () => {
        renderWithRouter(<PropuestaCard propuesta={mockPropuestaAceptada} />)

        expect(screen.getByText(/Actualizada:/)).toBeInTheDocument()
    })
})
