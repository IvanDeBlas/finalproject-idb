import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { AcuerdoTimeline } from "../../presentation/components/AcuerdoTimeline"
import { mockTimeline } from "../../__mocks__/acuerdo.mock"

describe("AcuerdoTimeline", () => {
    it("renders title 'Actividad'", () => {
        render(<AcuerdoTimeline timeline={mockTimeline} />)

        expect(screen.getByText("Actividad")).toBeInTheDocument()
    })

    it("renders all timeline events", () => {
        render(<AcuerdoTimeline timeline={mockTimeline} />)

        expect(screen.getByText("Acuerdo creado")).toBeInTheDocument()
        expect(screen.getByText(/Milestone agregado/)).toBeInTheDocument()
        expect(screen.getByText(/Entregable subido/)).toBeInTheDocument()
    })

    it("renders event accion text", () => {
        render(<AcuerdoTimeline timeline={mockTimeline} />)

        expect(screen.getByText("Acuerdo creado")).toBeInTheDocument()
    })

    it("renders event actor text", () => {
        render(<AcuerdoTimeline timeline={mockTimeline} />)

        const losRockeros = screen.getAllByText(/Los Rockeros/)
        expect(losRockeros.length).toBeGreaterThanOrEqual(1)
        expect(screen.getByText(/Studio Mix Pro/)).toBeInTheDocument()
    })

    it("renders empty message when timeline is empty", () => {
        render(<AcuerdoTimeline timeline={[]} />)

        expect(screen.getByText("No hay actividad registrada")).toBeInTheDocument()
    })
})
