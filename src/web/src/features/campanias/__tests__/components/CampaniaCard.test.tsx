import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@testing-library/react"
import { MemoryRouter } from "react-router-dom"
import { CampaniaCard } from "../../presentation/components/CampaniaCard"
import {
    mockCampaniaListItem,
    mockCampaniaListItemNoImage,
    mockCampaniaListItemFinalizada,
    mockCampaniaListItemOverfunded,
    mockCampaniaListItemBorrador,
    mockCampaniaListItemCancelada,
} from "../../__mocks__/campania.mock"

const mockNavigate = vi.fn()

vi.mock("react-router-dom", async () => {
    const actual = await vi.importActual("react-router-dom")
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    }
})

function renderWithRouter(ui: React.ReactElement) {
    return render(<MemoryRouter>{ui}</MemoryRouter>)
}

describe("CampaniaCard", () => {
    beforeEach(() => {
        vi.useFakeTimers()
        vi.setSystemTime(new Date("2026-03-15T12:00:00Z"))
    })

    afterEach(() => {
        vi.useRealTimers()
        vi.clearAllMocks()
    })

    it("renders campania title", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItem} />)

        expect(screen.getByText(mockCampaniaListItem.titulo)).toBeInTheDocument()
    })

    it("displays funding amounts", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItem} />)

        // importePledgedActual = 1250, importeObjetivo = 5000
        // Locale formatting may vary in test environments
        expect(screen.getByText(/1250/)).toBeInTheDocument()
        expect(screen.getByText(/5000/)).toBeInTheDocument()
    })

    it("calculates percentage correctly (25%)", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItem} />)

        // 1250 / 5000 * 100 = 25%
        const progressBar = screen.getByRole("progressbar")
        expect(progressBar).toHaveAttribute("aria-valuenow", "25")
    })

    it("caps percentage at 100 when over-funded", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItemOverfunded} />)

        // 8000 / 5000 = 160% -> capped at 100
        const progressBar = screen.getByRole("progressbar")
        expect(progressBar).toHaveAttribute("aria-valuenow", "100")
    })

    it("displays image when provided", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItem} />)

        const img = screen.getByAltText(`Imagen de portada de ${mockCampaniaListItem.titulo}`)
        expect(img).toBeInTheDocument()
        expect(img).toHaveAttribute("src", mockCampaniaListItem.imagenPrincipalUrl)
    })

    it("shows fallback when no image provided", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItemNoImage} />)

        // Should show first letter of title as fallback
        expect(screen.getByText("T")).toBeInTheDocument()
        expect(screen.queryByAltText(/Imagen de portada/)).not.toBeInTheDocument()
    })

    it("shows estado badge 'Activa' for estadoCampaniaId 2", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItem} />)

        expect(screen.getByText("Activa")).toBeInTheDocument()
    })

    it("shows estado badge 'Finalizada' for estadoCampaniaId 3", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItemFinalizada} />)

        expect(screen.getByText("Finalizada")).toBeInTheDocument()
    })

    it("shows estado badge 'Borrador' for estadoCampaniaId 1", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItemBorrador} />)

        expect(screen.getByText("Borrador")).toBeInTheDocument()
    })

    it("shows estado badge 'Cancelada' for estadoCampaniaId 4", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItemCancelada} />)

        expect(screen.getByText("Cancelada")).toBeInTheDocument()
    })

    it("displays backers count", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItem} />)

        expect(screen.getByText("15")).toBeInTheDocument()
    })

    it("displays days remaining when fechaFin is future", () => {
        // System time: 2026-03-15, fechaFin: 2026-04-30
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItem} />)

        expect(screen.getByText(/dias restantes/)).toBeInTheDocument()
    })

    it("shows 'Campana finalizada' when fechaFin is past", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItemFinalizada} />)

        // fechaFin: 2026-01-30, system time: 2026-03-15 -> 0 days
        expect(screen.getByText("Campana finalizada")).toBeInTheDocument()
    })

    it("displays descripcionCorta when available", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItem} />)

        expect(screen.getByText(mockCampaniaListItem.descripcionCorta!)).toBeInTheDocument()
    })

    it("navigates to detail on click", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItem} />)

        fireEvent.click(screen.getByRole("article"))
        expect(mockNavigate).toHaveBeenCalledWith(`/campanias/${mockCampaniaListItem.id}`)
    })

    it("navigates to detail on Enter key press", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItem} />)

        fireEvent.keyDown(screen.getByRole("article"), { key: "Enter" })
        expect(mockNavigate).toHaveBeenCalledWith(`/campanias/${mockCampaniaListItem.id}`)
    })

    it("has accessible article role and aria-label", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItem} />)

        const article = screen.getByRole("article")
        expect(article).toHaveAttribute("aria-label", `Campana: ${mockCampaniaListItem.titulo}`)
    })

    it("progress bar has accessible aria attributes", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItem} />)

        const progressBar = screen.getByRole("progressbar")
        expect(progressBar).toHaveAttribute("aria-valuemin", "0")
        expect(progressBar).toHaveAttribute("aria-valuemax", "100")
        expect(progressBar).toHaveAttribute("aria-label", "Progreso de financiacion: 25%")
    })

    it("renders compact variant correctly", () => {
        renderWithRouter(
            <CampaniaCard campania={mockCampaniaListItem} variant="compact" />
        )

        expect(screen.getByText(mockCampaniaListItem.titulo)).toBeInTheDocument()
        expect(screen.getByText("25% financiado")).toBeInTheDocument()
    })

    it("applies green gradient for over-funded progress bar", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItemOverfunded} />)

        const progressBar = screen.getByRole("progressbar")
        const indicator = progressBar.querySelector("div")
        expect(indicator?.className).toContain("from-green-400")
    })

    it("image has lazy loading attribute", () => {
        renderWithRouter(<CampaniaCard campania={mockCampaniaListItem} />)

        const img = screen.getByAltText(`Imagen de portada de ${mockCampaniaListItem.titulo}`)
        expect(img).toHaveAttribute("loading", "lazy")
    })
})
