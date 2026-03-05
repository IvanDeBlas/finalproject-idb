import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { MemoryRouter } from "react-router-dom"
import { EventosRecientesList } from "../../metricas/presentation/components/EventosRecientesList"
import {
    mockEventosRecientes,
    mockEventoReciente_Backing,
    mockEventoReciente_Click,
} from "../../__mocks__/tracking.mock"

function renderWithRouter(ui: React.ReactElement) {
    return render(<MemoryRouter>{ui}</MemoryRouter>)
}

describe("EventosRecientesList", () => {
    it("renders list of events correctly", () => {
        renderWithRouter(
            <EventosRecientesList eventos={mockEventosRecientes} programaId="prog-001" />
        )
        expect(screen.getAllByText("Backing").length).toBeGreaterThanOrEqual(1)
        expect(screen.getAllByText("Click").length).toBeGreaterThanOrEqual(1)
    })

    it("shows badge for each event type", () => {
        renderWithRouter(
            <EventosRecientesList eventos={mockEventosRecientes} programaId="prog-001" />
        )
        expect(screen.getAllByText("Backing").length).toBeGreaterThanOrEqual(1)
        expect(screen.getAllByText("Click").length).toBeGreaterThanOrEqual(1)
    })

    it("shows monetary value and commission for Backing events", () => {
        renderWithRouter(
            <EventosRecientesList eventos={[mockEventoReciente_Backing]} programaId="prog-001" />
        )
        expect(screen.getByText(/100.*EUR/)).toBeInTheDocument()
        expect(screen.getByText(/10.*EUR.*comision/)).toBeInTheDocument()
    })

    it("does not show monetary value for non-Backing events", () => {
        renderWithRouter(
            <EventosRecientesList eventos={[mockEventoReciente_Click]} programaId="prog-001" />
        )
        expect(screen.queryByText(/comision/)).not.toBeInTheDocument()
    })

    it("shows formatted date for each event", () => {
        renderWithRouter(
            <EventosRecientesList eventos={[mockEventoReciente_Backing]} programaId="prog-001" />
        )
        // Date format: "20 mar 2026, 14:30" or similar localized format
        expect(screen.getByText(/2026/)).toBeInTheDocument()
    })

    it("shows empty state when events array is empty", () => {
        renderWithRouter(
            <EventosRecientesList eventos={[]} programaId="prog-001" />
        )
        expect(screen.getByText(/Aun no hay eventos registrados/)).toBeInTheDocument()
    })

    it("shows share message in empty state", () => {
        renderWithRouter(
            <EventosRecientesList eventos={[]} programaId="prog-001" />
        )
        expect(screen.getByText(/Comparte tu enlace/)).toBeInTheDocument()
    })

    it('renders the "Ver todos" link', () => {
        renderWithRouter(
            <EventosRecientesList eventos={mockEventosRecientes} programaId="prog-001" />
        )
        expect(screen.getByText("Ver todos")).toBeInTheDocument()
    })

    it("renders header title", () => {
        renderWithRouter(
            <EventosRecientesList eventos={mockEventosRecientes} programaId="prog-001" />
        )
        expect(screen.getByText("Eventos recientes")).toBeInTheDocument()
    })
})
