import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@/test-utils"
import { EmptyStateNecesidades } from "../EmptyStateNecesidades"

describe("EmptyStateNecesidades", () => {
    it("renders with no filters applied", () => {
        render(
            <EmptyStateNecesidades
                hasFilters={false}
                onNuevaNecesidad={vi.fn()}
                onUsarPlantilla={vi.fn()}
            />
        )

        expect(screen.getByText("No tienes necesidades publicadas")).toBeInTheDocument()
        expect(screen.getByText(/Publica lo que necesitas/i)).toBeInTheDocument()
        expect(screen.getByText("Publicar necesidad")).toBeInTheDocument()
        expect(screen.getByText("Usar plantilla")).toBeInTheDocument()
    })

    it("renders with filters applied", () => {
        render(
            <EmptyStateNecesidades
                hasFilters={true}
                onClearFilters={vi.fn()}
            />
        )

        expect(screen.getByText("No se encontraron necesidades")).toBeInTheDocument()
        expect(screen.getByText(/Intenta ajustar los filtros/i)).toBeInTheDocument()
        expect(screen.getByText("Limpiar filtros")).toBeInTheDocument()
    })

    it("calls onNuevaNecesidad when Publicar button clicked", () => {
        const handleNueva = vi.fn()
        render(
            <EmptyStateNecesidades
                hasFilters={false}
                onNuevaNecesidad={handleNueva}
                onUsarPlantilla={vi.fn()}
            />
        )

        fireEvent.click(screen.getByText("Publicar necesidad"))

        expect(handleNueva).toHaveBeenCalled()
    })

    it("calls onUsarPlantilla when Usar plantilla button clicked", () => {
        const handlePlantilla = vi.fn()
        render(
            <EmptyStateNecesidades
                hasFilters={false}
                onNuevaNecesidad={vi.fn()}
                onUsarPlantilla={handlePlantilla}
            />
        )

        fireEvent.click(screen.getByText("Usar plantilla"))

        expect(handlePlantilla).toHaveBeenCalled()
    })

    it("calls onClearFilters when Limpiar filtros button clicked", () => {
        const handleClear = vi.fn()
        render(
            <EmptyStateNecesidades
                hasFilters={true}
                onClearFilters={handleClear}
            />
        )

        fireEvent.click(screen.getByText("Limpiar filtros"))

        expect(handleClear).toHaveBeenCalled()
    })
})
