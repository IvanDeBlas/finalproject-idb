import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@testing-library/react"
import { EntregableItem } from "../../presentation/components/EntregableItem"
import {
    mockEntregableEntregado,
    mockEntregableAprobado,
    mockEntregableRechazado,
} from "../../__mocks__/acuerdo.mock"

describe("EntregableItem", () => {
    it("renders entregable titulo", () => {
        render(
            <EntregableItem
                entregable={mockEntregableEntregado}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.getByText(mockEntregableEntregado.titulo)).toBeInTheDocument()
    })

    it("renders estado badge", () => {
        render(
            <EntregableItem
                entregable={mockEntregableEntregado}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.getByText(mockEntregableEntregado.estadoEntregableNombre)).toBeInTheDocument()
    })

    it("renders URL link when urlRecurso exists", () => {
        render(
            <EntregableItem
                entregable={mockEntregableEntregado}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        const link = screen.getByLabelText("Ver recurso")
        expect(link).toBeInTheDocument()
        expect(link).toHaveAttribute("href", mockEntregableEntregado.urlRecurso)
        expect(link).toHaveAttribute("target", "_blank")
    })

    it("does not render URL link when urlRecurso is undefined", () => {
        const entregableSinUrl = { ...mockEntregableEntregado, urlRecurso: undefined }

        render(
            <EntregableItem
                entregable={entregableSinUrl}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.queryByLabelText("Ver recurso")).not.toBeInTheDocument()
    })

    it("renders comentarioAprobacion when exists", () => {
        render(
            <EntregableItem
                entregable={mockEntregableAprobado}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.getByText(mockEntregableAprobado.comentarioAprobacion!)).toBeInTheDocument()
    })

    it("renders comentarioRechazo when exists", () => {
        render(
            <EntregableItem
                entregable={mockEntregableRechazado}
                miRol="Artista"
                estadoAcuerdoId={1}
            />
        )

        expect(screen.getByText(mockEntregableRechazado.comentarioRechazo!)).toBeInTheDocument()
    })

    it("shows approve/reject buttons for Artista with Entregado state in active acuerdo", () => {
        render(
            <EntregableItem
                entregable={mockEntregableEntregado}
                miRol="Artista"
                estadoAcuerdoId={1}
                onAprobar={vi.fn()}
                onRechazar={vi.fn()}
            />
        )

        expect(screen.getByText("Aprobar")).toBeInTheDocument()
        expect(screen.getByText("Rechazar")).toBeInTheDocument()
    })

    it("does NOT show approve/reject buttons for already approved entregable", () => {
        render(
            <EntregableItem
                entregable={mockEntregableAprobado}
                miRol="Artista"
                estadoAcuerdoId={1}
                onAprobar={vi.fn()}
                onRechazar={vi.fn()}
            />
        )

        expect(screen.queryByText("Aprobar")).not.toBeInTheDocument()
        expect(screen.queryByText("Rechazar")).not.toBeInTheDocument()
    })

    it("does NOT show approve/reject buttons for already rejected entregable", () => {
        render(
            <EntregableItem
                entregable={mockEntregableRechazado}
                miRol="Artista"
                estadoAcuerdoId={1}
                onAprobar={vi.fn()}
                onRechazar={vi.fn()}
            />
        )

        expect(screen.queryByText("Aprobar")).not.toBeInTheDocument()
        expect(screen.queryByText("Rechazar")).not.toBeInTheDocument()
    })

    it("does NOT show approve/reject buttons for Profesional", () => {
        render(
            <EntregableItem
                entregable={mockEntregableEntregado}
                miRol="Profesional"
                estadoAcuerdoId={1}
                onAprobar={vi.fn()}
                onRechazar={vi.fn()}
            />
        )

        expect(screen.queryByText("Aprobar")).not.toBeInTheDocument()
        expect(screen.queryByText("Rechazar")).not.toBeInTheDocument()
    })

    it("does NOT show approve/reject buttons in completed acuerdo", () => {
        render(
            <EntregableItem
                entregable={mockEntregableEntregado}
                miRol="Artista"
                estadoAcuerdoId={2}
                onAprobar={vi.fn()}
                onRechazar={vi.fn()}
            />
        )

        expect(screen.queryByText("Aprobar")).not.toBeInTheDocument()
        expect(screen.queryByText("Rechazar")).not.toBeInTheDocument()
    })

    it("calls onAprobar with entregable when clicking Aprobar", () => {
        const onAprobar = vi.fn()

        render(
            <EntregableItem
                entregable={mockEntregableEntregado}
                miRol="Artista"
                estadoAcuerdoId={1}
                onAprobar={onAprobar}
                onRechazar={vi.fn()}
            />
        )

        fireEvent.click(screen.getByText("Aprobar"))
        expect(onAprobar).toHaveBeenCalledWith(mockEntregableEntregado)
    })

    it("calls onRechazar with entregable when clicking Rechazar", () => {
        const onRechazar = vi.fn()

        render(
            <EntregableItem
                entregable={mockEntregableEntregado}
                miRol="Artista"
                estadoAcuerdoId={1}
                onAprobar={vi.fn()}
                onRechazar={onRechazar}
            />
        )

        fireEvent.click(screen.getByText("Rechazar"))
        expect(onRechazar).toHaveBeenCalledWith(mockEntregableEntregado)
    })
})
