import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor, fireEvent } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import { ConversacionList } from "../../presentation/components/ConversacionList"
import {
    mockConversacionListResponse,
    mockConversacionListResponseEmpty,
} from "../../__mocks__/mensajeria.mock"

const mockNavigate = vi.fn()
vi.mock("react-router-dom", async () => {
    const actual = await vi.importActual("react-router-dom")
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    }
})

vi.mock("../../infrastructure", () => ({
    conversacionApi: {
        getAll: vi.fn(),
        create: vi.fn(),
        getNoLeidosCount: vi.fn(),
    },
}))

vi.mock("@/store/auth-store", () => ({
    useAuthStore: vi.fn(() => ({
        isAuthenticated: true,
    })),
}))

import { conversacionApi } from "../../infrastructure"

function renderList(
    filtro: "todas" | "necesidades" | "acuerdos" = "todas",
    onFiltroChange = vi.fn()
) {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
        },
    })

    return render(
        <QueryClientProvider client={queryClient}>
            <MemoryRouter>
                <ConversacionList
                    filtro={filtro}
                    onFiltroChange={onFiltroChange}
                />
            </MemoryRouter>
        </QueryClientProvider>
    )
}

describe("ConversacionList", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("shows skeleton during initial loading", () => {
        vi.mocked(conversacionApi.getAll).mockReturnValue(
            new Promise(() => {})
        )

        renderList()

        const skeletons = screen.getAllByTestId("conversacion-skeleton")
        expect(skeletons.length).toBeGreaterThan(0)
    })

    it("shows list of conversations on success", async () => {
        vi.mocked(conversacionApi.getAll).mockResolvedValue(
            mockConversacionListResponse
        )

        renderList()

        await waitFor(() => {
            expect(screen.getByText("Studio Mix Pro")).toBeInTheDocument()
        })
        expect(
            screen.getByText("Diseno Grafico Pro")
        ).toBeInTheDocument()
        expect(
            screen.getByText("Fotografo Madrid")
        ).toBeInTheDocument()
    })

    it("shows empty state when no conversations", async () => {
        vi.mocked(conversacionApi.getAll).mockResolvedValue(
            mockConversacionListResponseEmpty
        )

        renderList()

        await waitFor(() => {
            expect(
                screen.getByText("No tienes conversaciones activas")
            ).toBeInTheDocument()
        })
    })

    it("shows error alert when API fails", async () => {
        vi.mocked(conversacionApi.getAll).mockRejectedValue(
            new Error("Network error")
        )

        renderList()

        await waitFor(() => {
            expect(
                screen.getByText("Error al cargar conversaciones")
            ).toBeInTheDocument()
        })
    })

    it("renders Todas tab as active by default", () => {
        vi.mocked(conversacionApi.getAll).mockReturnValue(
            new Promise(() => {})
        )

        renderList("todas")

        const todasTab = screen.getByRole("tab", { name: "Todas" })
        expect(todasTab).toHaveAttribute("data-state", "active")
    })

    it("calls onFiltroChange when Necesidades tab is clicked", async () => {
        vi.mocked(conversacionApi.getAll).mockReturnValue(
            new Promise(() => {})
        )
        const onFiltroChange = vi.fn()

        renderList("todas", onFiltroChange)

        await userEvent.click(
            screen.getByRole("tab", { name: "Necesidades" })
        )

        expect(onFiltroChange).toHaveBeenCalledWith("necesidades")
    })

    it("calls onFiltroChange when Acuerdos tab is clicked", async () => {
        vi.mocked(conversacionApi.getAll).mockReturnValue(
            new Promise(() => {})
        )
        const onFiltroChange = vi.fn()

        renderList("todas", onFiltroChange)

        await userEvent.click(
            screen.getByRole("tab", { name: "Acuerdos" })
        )

        expect(onFiltroChange).toHaveBeenCalledWith("acuerdos")
    })

    it("shows load more button when there are more pages", async () => {
        vi.mocked(conversacionApi.getAll).mockResolvedValue({
            ...mockConversacionListResponse,
            totalCount: 30,
        })

        renderList()

        await waitFor(() => {
            expect(
                screen.getByText("Cargar mas conversaciones")
            ).toBeInTheDocument()
        })
    })

    it("hides load more button when no more pages", async () => {
        vi.mocked(conversacionApi.getAll).mockResolvedValue(
            mockConversacionListResponse
        )

        renderList()

        await waitFor(() => {
            expect(screen.getByText("Studio Mix Pro")).toBeInTheDocument()
        })

        expect(
            screen.queryByText("Cargar mas conversaciones")
        ).not.toBeInTheDocument()
    })

    it("navigates to chat when a row is clicked", async () => {
        vi.mocked(conversacionApi.getAll).mockResolvedValue(
            mockConversacionListResponse
        )

        renderList()

        await waitFor(() => {
            expect(screen.getByText("Studio Mix Pro")).toBeInTheDocument()
        })

        fireEvent.click(screen.getByText("Studio Mix Pro"))

        expect(mockNavigate).toHaveBeenCalledWith(
            "/crowdsourcing/mensajes/f2a3b4c5-d6e7-8f9a-0b1c-2d3e4f5a6b7c"
        )
    })
})
