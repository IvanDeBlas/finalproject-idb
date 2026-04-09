import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@testing-library/react"
import { MemoryRouter } from "react-router-dom"
import { CampaniaList } from "../../presentation/components/CampaniaList"
import {
    mockCampaniasList,
    mockCampaniaListItem,
} from "../../__mocks__/campania.mock"

vi.mock("react-router-dom", async () => {
    const actual = await vi.importActual("react-router-dom")
    return {
        ...actual,
        useNavigate: () => vi.fn(),
    }
})

function renderWithRouter(ui: React.ReactElement) {
    return render(<MemoryRouter>{ui}</MemoryRouter>)
}

describe("CampaniaList", () => {
    it("renders list of campanias", () => {
        renderWithRouter(
            <CampaniaList campanias={mockCampaniasList} />
        )

        expect(screen.getByText(mockCampaniasList[0].titulo)).toBeInTheDocument()
        expect(screen.getByText(mockCampaniasList[1].titulo)).toBeInTheDocument()
        expect(screen.getByText(mockCampaniasList[2].titulo)).toBeInTheDocument()
    })

    it("renders correct number of cards", () => {
        renderWithRouter(
            <CampaniaList campanias={mockCampaniasList} />
        )

        const articles = screen.getAllByRole("article")
        expect(articles).toHaveLength(mockCampaniasList.length)
    })

    it("displays loading skeletons when isLoading is true", () => {
        renderWithRouter(
            <CampaniaList campanias={[]} isLoading={true} />
        )

        // CampaniaListSkeleton renders 6 skeleton cards by default
        expect(screen.queryByRole("article")).not.toBeInTheDocument()
    })

    it("does not render cards when loading", () => {
        renderWithRouter(
            <CampaniaList campanias={mockCampaniasList} isLoading={true} />
        )

        expect(screen.queryByText(mockCampaniasList[0].titulo)).not.toBeInTheDocument()
    })

    it("displays empty state when no campanias and no filters", () => {
        renderWithRouter(
            <CampaniaList campanias={[]} isLoading={false} />
        )

        expect(screen.getByText("No hay campanas disponibles")).toBeInTheDocument()
        expect(screen.getByText(/Aun no hay proyectos musicales/)).toBeInTheDocument()
    })

    it("displays filtered empty state with clear button when hasFilters", () => {
        const handleClearFilters = vi.fn()

        renderWithRouter(
            <CampaniaList
                campanias={[]}
                isLoading={false}
                hasFilters={true}
                onClearFilters={handleClearFilters}
            />
        )

        expect(screen.getByText("No se encontraron campanas")).toBeInTheDocument()
        expect(screen.getByText(/Intenta ajustar tus filtros/)).toBeInTheDocument()

        const clearButton = screen.getByRole("button", { name: /Ver todas las campanas/i })
        expect(clearButton).toBeInTheDocument()

        fireEvent.click(clearButton)
        expect(handleClearFilters).toHaveBeenCalledOnce()
    })

    it("displays error state with retry button", () => {
        const handleRetry = vi.fn()
        const error = new Error("Network error")

        renderWithRouter(
            <CampaniaList
                campanias={[]}
                error={error}
                onRetry={handleRetry}
            />
        )

        expect(screen.getByText("Error al cargar campanas")).toBeInTheDocument()

        const retryButton = screen.getByRole("button", { name: /Reintentar/i })
        fireEvent.click(retryButton)
        expect(handleRetry).toHaveBeenCalledOnce()
    })

    it("renders grid layout by default", () => {
        const { container } = renderWithRouter(
            <CampaniaList campanias={mockCampaniasList} />
        )

        const grid = container.querySelector(".grid")
        expect(grid).toBeInTheDocument()
        expect(grid?.className).toContain("grid-cols-1")
        expect(grid?.className).toContain("md:grid-cols-2")
        expect(grid?.className).toContain("lg:grid-cols-3")
    })

    it("renders list layout with compact cards when variant is list", () => {
        const { container } = renderWithRouter(
            <CampaniaList campanias={mockCampaniasList} variant="list" />
        )

        const list = container.querySelector(".space-y-4")
        expect(list).toBeInTheDocument()
    })

    it("does not show clear filters button when no onClearFilters provided", () => {
        renderWithRouter(
            <CampaniaList campanias={[]} hasFilters={true} />
        )

        expect(screen.queryByRole("button", { name: /Ver todas/i })).not.toBeInTheDocument()
    })

    it("applies custom className", () => {
        const { container } = renderWithRouter(
            <CampaniaList campanias={mockCampaniasList} className="custom-class" />
        )

        const grid = container.querySelector(".custom-class")
        expect(grid).toBeInTheDocument()
    })

    it("renders single campania correctly", () => {
        renderWithRouter(
            <CampaniaList campanias={[mockCampaniaListItem]} />
        )

        const articles = screen.getAllByRole("article")
        expect(articles).toHaveLength(1)
        expect(screen.getByText(mockCampaniaListItem.titulo)).toBeInTheDocument()
    })
})
