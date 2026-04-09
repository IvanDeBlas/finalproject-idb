import { createElement } from "react"
import { render, screen, fireEvent, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { describe, it, expect, vi, beforeEach } from "vitest"
import { ValoracionesSection } from "../../presentation/components/ValoracionesSection"
import {
    MOCK_USER_ID,
    mockValoracionesUsuarioConDatos,
    mockValoracionesUsuarioVacio,
} from "../../__mocks__/valoracion.mock"

// jsdom doesn't implement scrollIntoView
Element.prototype.scrollIntoView = vi.fn()

vi.mock("../../infrastructure", () => ({
    valoracionApi: {
        getByUser: vi.fn(),
    },
}))

import { valoracionApi } from "../../infrastructure"

function createTestQueryClient() {
    return new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
        },
    })
}

function renderSection(userId = MOCK_USER_ID) {
    const queryClient = createTestQueryClient()
    return render(
        createElement(
            QueryClientProvider,
            { client: queryClient },
            createElement(ValoracionesSection, { userId })
        )
    )
}

describe("ValoracionesSection", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders loading skeletons while fetching", () => {
        vi.mocked(valoracionApi.getByUser).mockImplementation(
            () => new Promise(() => {})
        )
        const { container } = renderSection()
        const skeletons = container.querySelectorAll(".animate-pulse")
        expect(skeletons.length).toBeGreaterThanOrEqual(2)
    })

    it('renders section title "Valoraciones"', async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        renderSection()
        await waitFor(() => {
            expect(screen.getByText("Valoraciones")).toBeInTheDocument()
        })
    })

    it("renders resumen with puntuacionMedia when data loads", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        renderSection()
        await waitFor(() => {
            expect(screen.getByText("4.5")).toBeInTheDocument()
        })
    })

    it("renders totalValoraciones count", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        renderSection()
        await waitFor(() => {
            expect(
                screen.getByText("12 valoraciones")
            ).toBeInTheDocument()
        })
    })

    it("renders RatingHistogram component", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        renderSection()
        await waitFor(() => {
            expect(screen.getByText("Distribucion")).toBeInTheDocument()
        })
    })

    it("renders list of ValoracionListItem", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        renderSection()
        await waitFor(() => {
            expect(screen.getByText("Los Rockeros")).toBeInTheDocument()
            expect(screen.getByText("Indie Band")).toBeInTheDocument()
        })
    })

    it("renders EmptyValoraciones when no ratings exist", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioVacio
        )
        renderSection()
        await waitFor(() => {
            expect(
                screen.getByText(
                    "Este usuario aun no tiene valoraciones"
                )
            ).toBeInTheDocument()
        })
    })

    it('renders empty state message "Este usuario aun no tiene valoraciones"', async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioVacio
        )
        renderSection()
        await waitFor(() => {
            expect(
                screen.getByText(
                    "Este usuario aun no tiene valoraciones"
                )
            ).toBeVisible()
        })
    })

    it("renders error alert when API fails", async () => {
        vi.mocked(valoracionApi.getByUser).mockRejectedValue(
            new Error("5000")
        )
        renderSection()
        await waitFor(() => {
            expect(
                screen.getByText(
                    "No se pudieron cargar las valoraciones."
                )
            ).toBeInTheDocument()
        })
    })

    it("renders pagination controls when totalCount > pageSize", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos // totalCount=12, pageSize=10 -> 2 pages
        )
        renderSection()
        await waitFor(() => {
            expect(
                screen.getByLabelText("Pagina anterior")
            ).toBeInTheDocument()
            expect(
                screen.getByLabelText("Pagina siguiente")
            ).toBeInTheDocument()
        })
    })

    it("previous button is disabled on first page", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        renderSection()
        await waitFor(() => {
            expect(
                screen.getByLabelText("Pagina anterior")
            ).toBeDisabled()
        })
    })

    it("next button is disabled on last page", async () => {
        // First render page 1
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos // 12 items, 10 per page = 2 pages
        )
        renderSection()
        await waitFor(() => {
            expect(
                screen.getByLabelText("Pagina siguiente")
            ).toBeEnabled()
        })

        // Navigate to page 2 (last page)
        vi.mocked(valoracionApi.getByUser).mockResolvedValue({
            ...mockValoracionesUsuarioConDatos,
            valoraciones: {
                ...mockValoracionesUsuarioConDatos.valoraciones,
                page: 2,
                items: [],
            },
        })
        fireEvent.click(screen.getByLabelText("Pagina siguiente"))
        await waitFor(() => {
            expect(
                screen.getByLabelText("Pagina siguiente")
            ).toBeDisabled()
        })
    })

    it("clicking next page button fetches page 2", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue(
            mockValoracionesUsuarioConDatos
        )
        renderSection()
        await waitFor(() => {
            expect(
                screen.getByLabelText("Pagina siguiente")
            ).toBeEnabled()
        })

        vi.mocked(valoracionApi.getByUser).mockResolvedValue({
            ...mockValoracionesUsuarioConDatos,
            valoraciones: {
                ...mockValoracionesUsuarioConDatos.valoraciones,
                page: 2,
            },
        })
        fireEvent.click(screen.getByLabelText("Pagina siguiente"))

        await waitFor(() => {
            expect(valoracionApi.getByUser).toHaveBeenCalledWith(
                MOCK_USER_ID,
                { page: 2, pageSize: 10 }
            )
        })
    })

    it("pagination controls are not rendered when totalCount <= pageSize", async () => {
        vi.mocked(valoracionApi.getByUser).mockResolvedValue({
            ...mockValoracionesUsuarioConDatos,
            valoraciones: {
                ...mockValoracionesUsuarioConDatos.valoraciones,
                totalCount: 5,
            },
        })
        renderSection()
        await waitFor(() => {
            expect(screen.getByText("Los Rockeros")).toBeInTheDocument()
        })
        expect(
            screen.queryByLabelText("Pagina anterior")
        ).not.toBeInTheDocument()
        expect(
            screen.queryByLabelText("Pagina siguiente")
        ).not.toBeInTheDocument()
    })
})
