import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@testing-library/react"
import { MilestoneCard } from "../../presentation/components/MilestoneCard"
import {
    mockMilestoneConEntregables,
    mockMilestoneCompletado,
    mockMilestoneSinEntregables,
} from "../../__mocks__/acuerdo.mock"

const defaultProps = {
    importeTotal: 450,
    monedaNombre: "EUR",
    onAprobarEntregable: vi.fn(),
    onRechazarEntregable: vi.fn(),
    onSubirEntregable: vi.fn(),
}

describe("MilestoneCard", () => {
    it("renders milestone titulo", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneConEntregables}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.getByText(mockMilestoneConEntregables.titulo)).toBeInTheDocument()
    })

    it("renders importe parcial text", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneConEntregables}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.getByText(/270,00/)).toBeInTheDocument()
        expect(screen.getByText(/EUR/)).toBeInTheDocument()
    })

    it("renders Pendiente badge when fechaCompletado is null", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneConEntregables}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.getByText("Pendiente")).toBeInTheDocument()
    })

    it("renders Completado badge when fechaCompletado exists", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneCompletado}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.getByText("Completado")).toBeInTheDocument()
    })

    it("renders fechaLimite when exists", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneConEntregables}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.getByText(/Limite:/)).toBeInTheDocument()
    })

    it("does not render fechaLimite when undefined", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneSinEntregables}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.queryByText(/Limite:/)).not.toBeInTheDocument()
    })

    it("renders entregable items when entregables exist", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneConEntregables}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.getByText(mockMilestoneConEntregables.entregables[0].titulo)).toBeInTheDocument()
    })

    it("renders Sin entregables when entregables is empty", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneSinEntregables}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.getByText("Sin entregables")).toBeInTheDocument()
    })

    it("shows edit button for Artista in active acuerdo with pending milestone", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneConEntregables}
                miRol="Artista"
                estadoAcuerdoId={1}
                onEditar={vi.fn()}
            />
        )

        expect(screen.getByLabelText("Editar milestone")).toBeInTheDocument()
    })

    it("does NOT show edit button for completed milestone", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneCompletado}
                miRol="Artista"
                estadoAcuerdoId={1}
                onEditar={vi.fn()}
            />
        )

        expect(screen.queryByLabelText("Editar milestone")).not.toBeInTheDocument()
    })

    it("does NOT show edit button for Profesional", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneConEntregables}
                miRol="Profesional"
                estadoAcuerdoId={1}
                onEditar={vi.fn()}
            />
        )

        expect(screen.queryByLabelText("Editar milestone")).not.toBeInTheDocument()
    })

    it("does NOT show edit button in completed acuerdo", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneConEntregables}
                miRol="Artista"
                estadoAcuerdoId={2}
                onEditar={vi.fn()}
            />
        )

        expect(screen.queryByLabelText("Editar milestone")).not.toBeInTheDocument()
    })

    it("shows delete button for Artista in active acuerdo with milestone without entregables", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneSinEntregables}
                miRol="Artista"
                estadoAcuerdoId={1}
                onEliminar={vi.fn()}
            />
        )

        expect(screen.getByLabelText("Eliminar milestone")).toBeInTheDocument()
    })

    it("does NOT show delete button when milestone has entregables", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneConEntregables}
                miRol="Artista"
                estadoAcuerdoId={1}
                onEliminar={vi.fn()}
            />
        )

        expect(screen.queryByLabelText("Eliminar milestone")).not.toBeInTheDocument()
    })

    it("calls onEditar when clicking edit button", () => {
        const onEditar = vi.fn()

        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneConEntregables}
                miRol="Artista"
                estadoAcuerdoId={1}
                onEditar={onEditar}
            />
        )

        fireEvent.click(screen.getByLabelText("Editar milestone"))
        expect(onEditar).toHaveBeenCalledWith(mockMilestoneConEntregables)
    })

    it("shows upload button for Profesional in active acuerdo", () => {
        render(
            <MilestoneCard
                {...defaultProps}
                milestone={mockMilestoneConEntregables}
                miRol="Profesional"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.getByText("Subir entregable")).toBeInTheDocument()
    })
})
