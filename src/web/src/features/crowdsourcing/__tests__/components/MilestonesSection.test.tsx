import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@testing-library/react"
import { MilestonesSection } from "../../presentation/components/MilestonesSection"
import {
    mockMilestoneConEntregables,
    mockMilestoneSinEntregables,
} from "../../__mocks__/acuerdo.mock"

const defaultProps = {
    milestones: [mockMilestoneConEntregables, mockMilestoneSinEntregables],
    importeTotal: 450,
    importeAsignado: 270,
    porcentajeAsignado: 60,
    monedaNombre: "EUR",
    miRol: "Artista" as const,
    estadoAcuerdoId: 1,
    onAgregarMilestone: vi.fn(),
    onEditarMilestone: vi.fn(),
    onEliminarMilestone: vi.fn(),
    onAprobarEntregable: vi.fn(),
    onRechazarEntregable: vi.fn(),
    onSubirEntregable: vi.fn(),
}

describe("MilestonesSection", () => {
    it("renders Milestones heading", () => {
        render(<MilestonesSection {...defaultProps} />)

        expect(screen.getByText("Milestones")).toBeInTheDocument()
    })

    it("renders MilestoneCard for each milestone", () => {
        render(<MilestonesSection {...defaultProps} />)

        expect(
            screen.getByText(mockMilestoneConEntregables.titulo)
        ).toBeInTheDocument()
        expect(
            screen.getByText(mockMilestoneSinEntregables.titulo)
        ).toBeInTheDocument()
    })

    it("shows empty state when no milestones", () => {
        render(
            <MilestonesSection
                {...defaultProps}
                milestones={[]}
            />
        )

        expect(
            screen.getByText("No hay milestones definidos")
        ).toBeInTheDocument()
    })

    it("shows Agregar milestone button for Artista in active acuerdo", () => {
        render(
            <MilestonesSection
                {...defaultProps}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.getByText("Agregar milestone")).toBeInTheDocument()
    })

    it("does NOT show Agregar milestone button for Profesional", () => {
        render(
            <MilestonesSection
                {...defaultProps}
                miRol="Profesional"
                estadoAcuerdoId={1}
            />
        )

        expect(
            screen.queryByText("Agregar milestone")
        ).not.toBeInTheDocument()
    })

    it("does NOT show Agregar milestone button in completed acuerdo", () => {
        render(
            <MilestonesSection
                {...defaultProps}
                miRol="Artista"
                estadoAcuerdoId={2}
            />
        )

        expect(
            screen.queryByText("Agregar milestone")
        ).not.toBeInTheDocument()
    })

    it("calls onAgregarMilestone when clicking the button", () => {
        const onAgregarMilestone = vi.fn()
        render(
            <MilestonesSection
                {...defaultProps}
                onAgregarMilestone={onAgregarMilestone}
            />
        )

        fireEvent.click(screen.getByText("Agregar milestone"))

        expect(onAgregarMilestone).toHaveBeenCalledOnce()
    })
})
