import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@testing-library/react"
import { NecesidadList } from "../../presentation/components/NecesidadList"
import {
    mockNecesidadAlta,
    mockNecesidadMedia,
    mockNecesidadBaja,
} from "../../__mocks__/crowdsourcing.mock"
import { TooltipProvider } from "@/components/ui/tooltip"

function renderWithTooltip(ui: React.ReactElement) {
    return render(<TooltipProvider>{ui}</TooltipProvider>)
}

const necesidades = [mockNecesidadAlta, mockNecesidadMedia, mockNecesidadBaja]

describe("NecesidadList", () => {
    it("groups necesidades by fase", () => {
        renderWithTooltip(
            <NecesidadList
                necesidades={necesidades}
                selectedNecesidades={new Map()}
                onToggleNecesidad={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        expect(screen.getByText("Preproduccion")).toBeInTheDocument()
        expect(screen.getByText("Grabacion")).toBeInTheDocument()
        expect(screen.getByText("Mezcla y Master")).toBeInTheDocument()
    })

    it("renders all necesidad items", () => {
        renderWithTooltip(
            <NecesidadList
                necesidades={necesidades}
                selectedNecesidades={new Map()}
                onToggleNecesidad={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        expect(screen.getByText("Productor Musical")).toBeInTheDocument()
        expect(screen.getByText("Ingeniero de Grabacion")).toBeInTheDocument()
        expect(screen.getByText("Ingeniero de Mezcla")).toBeInTheDocument()
    })

    it("marks selected items as checked", () => {
        const selected = new Map([
            ["nec-001", { min: 500, max: 1500 }],
        ])

        renderWithTooltip(
            <NecesidadList
                necesidades={necesidades}
                selectedNecesidades={selected}
                onToggleNecesidad={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        const checkboxes = screen.getAllByRole("checkbox")
        expect(checkboxes[0]).toBeChecked()
        expect(checkboxes[1]).not.toBeChecked()
        expect(checkboxes[2]).not.toBeChecked()
    })

    it("renders 3 checkboxes for 3 necesidades", () => {
        renderWithTooltip(
            <NecesidadList
                necesidades={necesidades}
                selectedNecesidades={new Map()}
                onToggleNecesidad={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        const checkboxes = screen.getAllByRole("checkbox")
        expect(checkboxes).toHaveLength(3)
    })

    it("renders empty when no necesidades", () => {
        const { container } = renderWithTooltip(
            <NecesidadList
                necesidades={[]}
                selectedNecesidades={new Map()}
                onToggleNecesidad={vi.fn()}
                onBudgetChange={vi.fn()}
            />
        )

        expect(container.querySelectorAll("[role='checkbox']")).toHaveLength(0)
    })
})
