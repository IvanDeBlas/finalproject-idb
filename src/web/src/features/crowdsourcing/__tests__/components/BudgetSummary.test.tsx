import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { BudgetSummary } from "../../presentation/components/BudgetSummary"

describe("BudgetSummary", () => {
    it("displays min total", () => {
        render(
            <BudgetSummary
                minTotal={3000}
                maxTotal={8000}
                selectedCount={5}
                totalCount={8}
            />
        )

        // formatCurrency renders with locale-dependent separators
        expect(screen.getByText(/3[.,]?000/)).toBeInTheDocument()
    })

    it("displays max total", () => {
        render(
            <BudgetSummary
                minTotal={3000}
                maxTotal={8000}
                selectedCount={5}
                totalCount={8}
            />
        )

        expect(screen.getByText(/8[.,]?000/)).toBeInTheDocument()
    })

    it("displays average budget", () => {
        render(
            <BudgetSummary
                minTotal={3000}
                maxTotal={8000}
                selectedCount={5}
                totalCount={8}
            />
        )

        // Average: (3000 + 8000) / 2 = 5500
        expect(screen.getByText(/5[.,]?500/)).toBeInTheDocument()
    })

    it("shows selected count", () => {
        render(
            <BudgetSummary
                minTotal={1000}
                maxTotal={3000}
                selectedCount={3}
                totalCount={10}
            />
        )

        // "Necesidades seleccionadas: 3 de 10" is rendered as a single element
        expect(screen.getByText(/seleccionadas/i)).toBeInTheDocument()
    })

    it("displays warning when no selections", () => {
        render(
            <BudgetSummary
                minTotal={0}
                maxTotal={0}
                selectedCount={0}
                totalCount={8}
            />
        )

        expect(
            screen.getByText("Selecciona al menos una necesidad para continuar")
        ).toBeInTheDocument()
    })

    it("does not display warning when selections exist", () => {
        render(
            <BudgetSummary
                minTotal={500}
                maxTotal={1500}
                selectedCount={1}
                totalCount={8}
            />
        )

        expect(
            screen.queryByText("Selecciona al menos una necesidad para continuar")
        ).not.toBeInTheDocument()
    })

    it("displays zero totals when nothing selected", () => {
        render(
            <BudgetSummary
                minTotal={0}
                maxTotal={0}
                selectedCount={0}
                totalCount={5}
            />
        )

        const zeroValues = screen.getAllByText(/€\s*0/)
        expect(zeroValues.length).toBeGreaterThan(0)
    })
})
