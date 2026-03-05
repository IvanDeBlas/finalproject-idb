import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@testing-library/react"
import { EmptyStateNecesidades } from "../../presentation/components/EmptyStateNecesidades"

describe("EmptyStateNecesidades", () => {
    it("renders 'Sin necesidades abiertas' when no filters active", () => {
        render(<EmptyStateNecesidades hasActiveFilters={false} />)

        expect(screen.getByText("Sin necesidades abiertas")).toBeInTheDocument()
        expect(
            screen.getByText("Vuelve pronto para ver nuevas oportunidades de trabajo")
        ).toBeInTheDocument()
    })

    it("renders 'Sin resultados' when filters active", () => {
        render(
            <EmptyStateNecesidades hasActiveFilters onClearFilters={vi.fn()} />
        )

        expect(screen.getByText("Sin resultados")).toBeInTheDocument()
        expect(
            screen.getByText(/No hay necesidades que coincidan/)
        ).toBeInTheDocument()
    })

    it("shows clear filters button when filters active", () => {
        const onClear = vi.fn()
        render(
            <EmptyStateNecesidades hasActiveFilters onClearFilters={onClear} />
        )

        const btn = screen.getByText("Limpiar filtros")
        fireEvent.click(btn)
        expect(onClear).toHaveBeenCalledOnce()
    })

    it("does not show clear filters button when no filters active", () => {
        render(<EmptyStateNecesidades hasActiveFilters={false} />)

        expect(screen.queryByText("Limpiar filtros")).not.toBeInTheDocument()
    })

    it("has role=status for accessibility", () => {
        render(<EmptyStateNecesidades hasActiveFilters={false} />)

        expect(screen.getByRole("status")).toBeInTheDocument()
    })
})
