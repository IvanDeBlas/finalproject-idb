import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { AcuerdoCabecera } from "../../presentation/components/AcuerdoCabecera"
import {
    mockAcuerdoActivo,
    mockAcuerdoCompletado,
} from "../../__mocks__/acuerdo.mock"

describe("AcuerdoCabecera", () => {
    it("renders tituloInterno", () => {
        render(<AcuerdoCabecera acuerdo={mockAcuerdoActivo} />)

        expect(screen.getByText("Mezcla EP Los Rockeros")).toBeInTheDocument()
    })

    it("renders necesidad titulo", () => {
        render(<AcuerdoCabecera acuerdo={mockAcuerdoActivo} />)

        expect(
            screen.getByText(/Mezcla de pistas para EP/)
        ).toBeInTheDocument()
    })

    it("renders artista nombre", () => {
        render(<AcuerdoCabecera acuerdo={mockAcuerdoActivo} />)

        expect(screen.getByText("Los Rockeros")).toBeInTheDocument()
    })

    it("renders profesional nombre", () => {
        render(<AcuerdoCabecera acuerdo={mockAcuerdoActivo} />)

        expect(screen.getByText("Studio Mix Pro")).toBeInTheDocument()
    })

    it("renders importe total formatted with moneda", () => {
        render(<AcuerdoCabecera acuerdo={mockAcuerdoActivo} />)

        expect(screen.getByText(/EUR/)).toBeInTheDocument()
        expect(screen.getByText(/450/)).toBeInTheDocument()
    })

    it("renders estado badge", () => {
        render(<AcuerdoCabecera acuerdo={mockAcuerdoActivo} />)

        expect(screen.getByText("Activo")).toBeInTheDocument()
    })

    it("renders fecha inicio", () => {
        render(<AcuerdoCabecera acuerdo={mockAcuerdoActivo} />)

        // formatDate uses toLocaleDateString("es-ES", { day: "2-digit", month: "short", year: "numeric" })
        // 2026-03-01 => "01 mar 2026" in es-ES
        expect(screen.getByText(/Inicio:/)).toBeInTheDocument()
    })

    it("does NOT render fecha fin real when undefined", () => {
        render(<AcuerdoCabecera acuerdo={mockAcuerdoActivo} />)

        expect(screen.queryByText(/Fin real:/)).not.toBeInTheDocument()
    })

    it("renders fecha fin real when present", () => {
        render(<AcuerdoCabecera acuerdo={mockAcuerdoCompletado} />)

        expect(screen.getByText(/Fin real:/)).toBeInTheDocument()
    })
})
