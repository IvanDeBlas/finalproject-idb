import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { ConfirmationSummary } from "../../presentation/components/ConfirmationSummary"
import {
    mockTemplateDetail,
    mockNecesidadAlta,
    mockNecesidadMedia,
} from "../../__mocks__/crowdsourcing.mock"

describe("ConfirmationSummary", () => {
    const defaultProps = {
        template: mockTemplateDetail,
        selectedNecesidades: [mockNecesidadAlta, mockNecesidadMedia],
        presupuestos: new Map([
            ["nec-001", { min: 500, max: 1500 }],
            ["nec-002", { min: 800, max: 2000 }],
        ]),
        minTotal: 1300,
        maxTotal: 3500,
    }

    it("displays template name", () => {
        render(<ConfirmationSummary {...defaultProps} />)

        expect(screen.getByText("Produccion de EP")).toBeInTheDocument()
    })

    it("displays selected count out of total", () => {
        render(<ConfirmationSummary {...defaultProps} />)

        expect(screen.getByText(/2 de 3/)).toBeInTheDocument()
    })

    it("displays budget summary section", () => {
        render(<ConfirmationSummary {...defaultProps} />)

        expect(screen.getByText("Resumen Presupuestario")).toBeInTheDocument()
    })

    it("displays min total", () => {
        render(<ConfirmationSummary {...defaultProps} />)

        expect(screen.getByText(/1[.,]?300/)).toBeInTheDocument()
    })

    it("displays max total", () => {
        render(<ConfirmationSummary {...defaultProps} />)

        expect(screen.getByText(/3[.,]?500/)).toBeInTheDocument()
    })

    it("displays average budget", () => {
        render(<ConfirmationSummary {...defaultProps} />)

        // Average: (1300 + 3500) / 2 = 2400
        expect(screen.getByText(/2[.,]?400/)).toBeInTheDocument()
    })

    it("groups necesidades by fase", () => {
        render(<ConfirmationSummary {...defaultProps} />)

        expect(screen.getByText("Preproduccion")).toBeInTheDocument()
        expect(screen.getByText("Grabacion")).toBeInTheDocument()
    })

    it("shows necesidad titles in the list", () => {
        render(<ConfirmationSummary {...defaultProps} />)

        expect(screen.getByText("Productor Musical")).toBeInTheDocument()
        expect(screen.getByText("Ingeniero de Grabacion")).toBeInTheDocument()
    })

    it("shows priority badges", () => {
        render(<ConfirmationSummary {...defaultProps} />)

        expect(screen.getByText("Alta")).toBeInTheDocument()
        expect(screen.getByText("Media")).toBeInTheDocument()
    })

    it("displays Necesidades Seleccionadas heading", () => {
        render(<ConfirmationSummary {...defaultProps} />)

        expect(
            screen.getByText("Necesidades Seleccionadas")
        ).toBeInTheDocument()
    })
})
