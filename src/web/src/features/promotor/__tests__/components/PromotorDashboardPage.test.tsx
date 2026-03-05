import { createElement } from "react"
import { render, screen } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import { describe, it, expect, vi, beforeEach } from "vitest"
import PromotorDashboardPage from "../../presentation/pages/PromotorDashboardPage"
import { mockPromotor, mockPromotorMinimo, mockPromotorInactivo } from "../../__mocks__/promotor.mock"

vi.mock("../../infrastructure/promotor.service", () => ({
    promotorService: {
        getMe: vi.fn(),
    },
}))

import { promotorService } from "../../infrastructure/promotor.service"

function createTestQueryClient() {
    return new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })
}

function renderWithProviders(queryClient?: QueryClient) {
    const client = queryClient ?? createTestQueryClient()
    return render(
        createElement(
            QueryClientProvider,
            { client },
            createElement(MemoryRouter, null, createElement(PromotorDashboardPage))
        )
    )
}

describe("PromotorDashboardPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("shows loading state while data is fetching", () => {
        vi.mocked(promotorService.getMe).mockImplementation(
            () => new Promise(() => {})
        )
        renderWithProviders()
        expect(screen.getByText("Cargando...")).toBeInTheDocument()
    })

    it("shows promotor nombre in welcome message", async () => {
        vi.mocked(promotorService.getMe).mockResolvedValue(mockPromotor)
        renderWithProviders()

        expect(
            await screen.findByText(/Hola, DJ Marketing Pro/)
        ).toBeInTheDocument()
    })

    it("shows totalProgramasActivos KPI", async () => {
        vi.mocked(promotorService.getMe).mockResolvedValue(mockPromotor)
        renderWithProviders()

        await screen.findByText(/Hola, DJ Marketing Pro/)
        expect(screen.getByText("Programas activos")).toBeInTheDocument()
        expect(screen.getByText("3")).toBeInTheDocument()
    })

    it("shows comisiones ganadas KPI", async () => {
        vi.mocked(promotorService.getMe).mockResolvedValue(mockPromotor)
        renderWithProviders()

        expect(
            await screen.findByText("Comisiones ganadas")
        ).toBeInTheDocument()
    })

    it("shows link to edit profile", async () => {
        vi.mocked(promotorService.getMe).mockResolvedValue(mockPromotor)
        renderWithProviders()

        const editLink = await screen.findByRole("link", { name: /editar perfil/i })
        expect(editLink).toBeInTheDocument()
        expect(editLink).toHaveAttribute("href", "/promotor/perfil")
    })

    it("shows inactive banner when esActivo is false", async () => {
        vi.mocked(promotorService.getMe).mockResolvedValue(mockPromotorInactivo)
        renderWithProviders()

        expect(
            await screen.findByText(/perfil de promotor esta desactivado/i)
        ).toBeInTheDocument()
    })

    it("does NOT show inactive banner when esActivo is true", async () => {
        vi.mocked(promotorService.getMe).mockResolvedValue(mockPromotor)
        renderWithProviders()

        await screen.findByText(/Hola, DJ Marketing Pro/)
        expect(
            screen.queryByText(/perfil de promotor esta desactivado/i)
        ).not.toBeInTheDocument()
    })

    it("shows zero state for KPIs when promotor has no activity", async () => {
        vi.mocked(promotorService.getMe).mockResolvedValue(mockPromotorMinimo)
        renderWithProviders()

        await screen.findByText(/Hola, Fan Embajador Test/)
        expect(screen.getByText("0")).toBeInTheDocument()
    })
})
