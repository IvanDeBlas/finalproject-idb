import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { ProgramaCard } from "../../presentation/components/ProgramaCard"
import {
    mockProgramaExplorarItem,
    mockProgramaPendiente,
    mockProgramaAprobado,
    mockProgramaBloqueado,
    mockProgramaDadoDeBaja,
} from "../../__mocks__/inscripcion.mock"
import type { ProgramaExplorarItem } from "../../domain"

const defaultProps = {
    isSolicitando: false,
    onSolicitar: vi.fn(),
    onVerMisDatos: vi.fn(),
}

describe("ProgramaCard", () => {
    it("renders programa titulo and artista", () => {
        render(<ProgramaCard programa={mockProgramaExplorarItem} {...defaultProps} />)

        expect(screen.getByText("Promociona mi nuevo album")).toBeInTheDocument()
        expect(screen.getByText("Luna Nova")).toBeInTheDocument()
    })

    it("renders tipo de programa", () => {
        render(<ProgramaCard programa={mockProgramaExplorarItem} {...defaultProps} />)

        expect(screen.getByText("Referral")).toBeInTheDocument()
    })

    it("renders comision porcentaje when present", () => {
        render(<ProgramaCard programa={mockProgramaExplorarItem} {...defaultProps} />)

        expect(screen.getByText(/10%/i)).toBeInTheDocument()
    })

    it("renders comision fija when present", () => {
        const programaFija: ProgramaExplorarItem = {
            ...mockProgramaExplorarItem,
            importeComisionPorcentaje: null,
            importeComisionFija: 5,
        }
        render(<ProgramaCard programa={programaFija} {...defaultProps} />)

        expect(screen.getByText(/5 EUR/)).toBeInTheDocument()
    })

    it("renders numero de tareas", () => {
        render(<ProgramaCard programa={mockProgramaExplorarItem} {...defaultProps} />)

        expect(screen.getByText(/3 tareas/)).toBeInTheDocument()
    })

    it("renders campaniaTitulo when present", () => {
        render(<ProgramaCard programa={mockProgramaExplorarItem} {...defaultProps} />)

        expect(screen.getByText("Mi Album Debut")).toBeInTheDocument()
    })

    it("does not render campania section when null", () => {
        const programaSinCampania: ProgramaExplorarItem = {
            ...mockProgramaExplorarItem,
            campaniaTitulo: null,
        }
        render(<ProgramaCard programa={programaSinCampania} {...defaultProps} />)

        expect(screen.queryByText("Mi Album Debut")).not.toBeInTheDocument()
    })

    it("shows solicitar button when miEstado is null", () => {
        render(<ProgramaCard programa={mockProgramaExplorarItem} {...defaultProps} />)

        expect(screen.getByRole("button", { name: /Solicitar inscripcion/i })).toBeInTheDocument()
    })

    it("hides solicitar button when miEstado is Pendiente", () => {
        render(<ProgramaCard programa={mockProgramaPendiente} {...defaultProps} />)

        expect(screen.queryByRole("button", { name: /Solicitar inscripcion/i })).not.toBeInTheDocument()
        expect(screen.getByText("Pendiente de aprobacion")).toBeInTheDocument()
    })

    it("shows Aprobado badge and Ver mis datos link when miEstado is Aprobado", () => {
        render(<ProgramaCard programa={mockProgramaAprobado} {...defaultProps} />)

        expect(screen.queryByRole("button", { name: /Solicitar inscripcion/i })).not.toBeInTheDocument()
        expect(screen.getByText("Aprobado")).toBeInTheDocument()
        expect(screen.getByText(/Ver mis datos/i)).toBeInTheDocument()
    })

    it("shows blocked message when miEstado is Bloqueado", () => {
        render(<ProgramaCard programa={mockProgramaBloqueado} {...defaultProps} />)

        expect(screen.queryByRole("button", { name: /Solicitar inscripcion/i })).not.toBeInTheDocument()
        expect(screen.getByText(/No puedes inscribirte en este programa/i)).toBeInTheDocument()
    })

    it("shows DadoDeBaja badge when miEstado is DadoDeBaja", () => {
        render(<ProgramaCard programa={mockProgramaDadoDeBaja} {...defaultProps} />)

        expect(screen.queryByRole("button", { name: /Solicitar inscripcion/i })).not.toBeInTheDocument()
        expect(screen.getByText(/Dado de baja/i)).toBeInTheDocument()
    })

    it("calls onSolicitar when solicitar button clicked", async () => {
        const handleSolicitar = vi.fn()
        const user = userEvent.setup()

        render(
            <ProgramaCard
                programa={mockProgramaExplorarItem}
                isSolicitando={false}
                onSolicitar={handleSolicitar}
                onVerMisDatos={vi.fn()}
            />
        )

        await user.click(screen.getByRole("button", { name: /Solicitar inscripcion/i }))

        expect(handleSolicitar).toHaveBeenCalledWith(mockProgramaExplorarItem.id)
    })

    it("disables button while solicitar is pending", () => {
        render(
            <ProgramaCard
                programa={mockProgramaExplorarItem}
                isSolicitando={true}
                onSolicitar={vi.fn()}
                onVerMisDatos={vi.fn()}
            />
        )

        expect(screen.getByRole("button", { name: /Enviando/i })).toBeDisabled()
    })

    it("calls onVerMisDatos when Aprobado link clicked", async () => {
        const handleVerMisDatos = vi.fn()
        const user = userEvent.setup()

        render(
            <ProgramaCard
                programa={mockProgramaAprobado}
                isSolicitando={false}
                onSolicitar={vi.fn()}
                onVerMisDatos={handleVerMisDatos}
            />
        )

        await user.click(screen.getByText(/Ver mis datos/i))

        expect(handleVerMisDatos).toHaveBeenCalledWith(mockProgramaAprobado.id)
    })
})
