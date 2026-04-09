import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@testing-library/react"
import { NecesidadItem } from "../../presentation/components/NecesidadItem"
import {
    mockNecesidadAlta,
    mockNecesidadMedia,
    mockNecesidadBaja,
} from "../../__mocks__/crowdsourcing.mock"

// Wrap with TooltipProvider for tooltip tests
import { TooltipProvider } from "@/components/ui/tooltip"

function renderWithTooltip(ui: React.ReactElement) {
    return render(<TooltipProvider>{ui}</TooltipProvider>)
}

describe("NecesidadItem", () => {
    it("renders necesidad title", () => {
        renderWithTooltip(
            <NecesidadItem
                necesidad={mockNecesidadAlta}
                isSelected={false}
                onToggle={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        expect(screen.getByText("Productor Musical")).toBeInTheDocument()
    })

    it("renders rol profesional name", () => {
        renderWithTooltip(
            <NecesidadItem
                necesidad={mockNecesidadAlta}
                isSelected={false}
                onToggle={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        expect(screen.getByText(/Rol: Productor Musical/)).toBeInTheDocument()
    })

    it("shows ESENCIAL badge for Alta priority", () => {
        renderWithTooltip(
            <NecesidadItem
                necesidad={mockNecesidadAlta}
                isSelected={false}
                onToggle={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        expect(screen.getByText("ESENCIAL")).toBeInTheDocument()
    })

    it("shows RECOMENDADO badge for Media priority", () => {
        renderWithTooltip(
            <NecesidadItem
                necesidad={mockNecesidadMedia}
                isSelected={false}
                onToggle={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        expect(screen.getByText("RECOMENDADO")).toBeInTheDocument()
    })

    it("shows OPCIONAL badge for Baja priority", () => {
        renderWithTooltip(
            <NecesidadItem
                necesidad={mockNecesidadBaja}
                isSelected={false}
                onToggle={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        expect(screen.getByText("OPCIONAL")).toBeInTheDocument()
    })

    it("checkbox is checked when isSelected is true", () => {
        renderWithTooltip(
            <NecesidadItem
                necesidad={mockNecesidadAlta}
                isSelected={true}
                onToggle={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        const checkbox = screen.getByRole("checkbox")
        expect(checkbox).toBeChecked()
    })

    it("checkbox is unchecked when isSelected is false", () => {
        renderWithTooltip(
            <NecesidadItem
                necesidad={mockNecesidadAlta}
                isSelected={false}
                onToggle={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        const checkbox = screen.getByRole("checkbox")
        expect(checkbox).not.toBeChecked()
    })

    it("calls onToggle when checkbox is clicked", () => {
        const handleToggle = vi.fn()
        renderWithTooltip(
            <NecesidadItem
                necesidad={mockNecesidadAlta}
                isSelected={false}
                onToggle={handleToggle}
                onBudgetChange={vi.fn()}
            />
        )

        fireEvent.click(screen.getByRole("checkbox"))
        expect(handleToggle).toHaveBeenCalledWith("nec-001")
    })

    it("shows budget inputs when selected", () => {
        renderWithTooltip(
            <NecesidadItem
                necesidad={mockNecesidadAlta}
                isSelected={true}
                presupuestoMin={500}
                presupuestoMax={1500}
                onToggle={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        expect(screen.getByLabelText("Presupuesto minimo")).toBeInTheDocument()
        expect(screen.getByLabelText("Presupuesto maximo")).toBeInTheDocument()
    })

    it("hides budget inputs when not selected", () => {
        renderWithTooltip(
            <NecesidadItem
                necesidad={mockNecesidadAlta}
                isSelected={false}
                onToggle={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        expect(screen.queryByLabelText("Presupuesto minimo")).not.toBeInTheDocument()
        expect(screen.queryByLabelText("Presupuesto maximo")).not.toBeInTheDocument()
    })

    it("has accessible checkbox label", () => {
        renderWithTooltip(
            <NecesidadItem
                necesidad={mockNecesidadAlta}
                isSelected={false}
                onToggle={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        expect(
            screen.getByRole("checkbox", { name: /Seleccionar Productor Musical/i })
        ).toBeInTheDocument()
    })
})
