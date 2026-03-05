import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@testing-library/react"
import { TemplateGallery } from "../../presentation/components/TemplateGallery"
import { mockTemplatesList } from "../../__mocks__/crowdsourcing.mock"

describe("TemplateGallery", () => {
    it("renders loading skeletons when isLoading", () => {
        const { container } = render(
            <TemplateGallery
                templates={[]}
                onSelectTemplate={vi.fn()}
                isLoading={true}
            />
        )

        // Should render skeleton elements (6 cards, each with multiple Skeleton divs)
        const skeletons = container.querySelectorAll(".animate-pulse")
        expect(skeletons.length).toBeGreaterThanOrEqual(6)
    })

    it("renders template cards when data is available", () => {
        render(
            <TemplateGallery
                templates={mockTemplatesList}
                onSelectTemplate={vi.fn()}
            />
        )

        expect(screen.getByText("Produccion de EP")).toBeInTheDocument()
        expect(screen.getByText("Produccion de Album")).toBeInTheDocument()
        expect(screen.getByText("Produccion de Sencillo")).toBeInTheDocument()
    })

    it("displays empty state when no templates", () => {
        render(
            <TemplateGallery
                templates={[]}
                onSelectTemplate={vi.fn()}
            />
        )

        expect(
            screen.getByText("No hay plantillas disponibles")
        ).toBeInTheDocument()
        expect(
            screen.getByText("Crear plantilla personalizada")
        ).toBeInTheDocument()
    })

    it("calls onSelectTemplate when a card is clicked", () => {
        const handleSelect = vi.fn()
        render(
            <TemplateGallery
                templates={mockTemplatesList}
                onSelectTemplate={handleSelect}
            />
        )

        fireEvent.click(screen.getAllByText("Seleccionar")[0])
        expect(handleSelect).toHaveBeenCalledWith("tpl-001")
    })

    it("renders correct number of cards", () => {
        render(
            <TemplateGallery
                templates={mockTemplatesList}
                onSelectTemplate={vi.fn()}
            />
        )

        const selectButtons = screen.getAllByText("Seleccionar")
        expect(selectButtons).toHaveLength(3)
    })

    it("does not show skeletons when not loading", () => {
        const { container } = render(
            <TemplateGallery
                templates={mockTemplatesList}
                onSelectTemplate={vi.fn()}
                isLoading={false}
            />
        )

        const skeletons = container.querySelectorAll(".animate-pulse")
        expect(skeletons.length).toBe(0)
    })
})
