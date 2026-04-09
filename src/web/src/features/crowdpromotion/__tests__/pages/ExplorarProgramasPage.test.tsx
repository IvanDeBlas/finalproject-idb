import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import {
    mockPaginatedProgramas,
    mockEmptyPaginatedProgramas,
    mockInscripcionCreada,
    mockProgramaExplorarItem,
    mockProgramaBloqueado,
    mockProgramaAprobado,
} from "../../__mocks__/inscripcion.mock"

vi.mock("sonner", () => ({
    toast: { success: vi.fn(), error: vi.fn(), info: vi.fn() },
}))

vi.mock("@/store/auth-store", () => ({
    useAuthStore: vi.fn(() => ({
        isAuthenticated: true,
        user: { id: "user-test-001", email: "test@test.com", nombreCompleto: "Test User" },
        token: "mock-jwt-token",
    })),
}))

vi.mock("@/features/promotor/application", () => ({
    usePromotor: vi.fn(() => ({
        data: { id: "promotor-001", nombrePublico: "Test Promotor" },
        isLoading: false,
        isError: false,
    })),
}))

vi.mock("@/hooks/useDebounce", () => ({
    useDebounce: vi.fn((val: string) => val),
}))

vi.mock("../../infrastructure/inscripcion.service", () => ({
    inscripcionService: {
        explorarProgramas: vi.fn(),
        solicitarInscripcion: vi.fn(),
        misProgramas: vi.fn(),
    },
}))

import { inscripcionService } from "../../infrastructure/inscripcion.service"
import { toast } from "sonner"
import ExplorarProgramasPage from "../../presentation/pages/ExplorarProgramasPage"

function renderPage() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })

    return render(
        <QueryClientProvider client={queryClient}>
            <MemoryRouter initialEntries={["/crowdpromotion/explorar"]}>
                <ExplorarProgramasPage />
            </MemoryRouter>
        </QueryClientProvider>
    )
}

describe("ExplorarProgramasPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders paginated list of programas", async () => {
        vi.mocked(inscripcionService.explorarProgramas).mockResolvedValue(mockPaginatedProgramas)

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Promociona mi nuevo album")).toBeInTheDocument()
        })

        expect(screen.getByText("Luna Nova")).toBeInTheDocument()
        expect(screen.getByText(/5 de 5 programas/i)).toBeInTheDocument()
    })

    it("shows skeleton while loading", () => {
        vi.mocked(inscripcionService.explorarProgramas).mockReturnValue(
            new Promise(() => {})
        )

        renderPage()

        expect(screen.getByLabelText("Cargando programas")).toBeInTheDocument()
    })

    it("shows error state when service fails", async () => {
        vi.mocked(inscripcionService.explorarProgramas).mockRejectedValue(
            new Error("API Error")
        )

        renderPage()

        await waitFor(() => {
            expect(screen.getByText(/No se pudieron cargar los programas/i)).toBeInTheDocument()
        }, { timeout: 5000 })

        expect(screen.getByRole("button", { name: /Reintentar/i })).toBeInTheDocument()
    })

    it("shows empty state when no programas", async () => {
        vi.mocked(inscripcionService.explorarProgramas).mockResolvedValue(mockEmptyPaginatedProgramas)

        renderPage()

        await waitFor(() => {
            expect(screen.getByText(/No hay programas/i)).toBeInTheDocument()
        })
    })

    it("solicitar inscripcion shows success toast", async () => {
        const user = userEvent.setup()
        vi.mocked(inscripcionService.explorarProgramas).mockResolvedValue({
            ...mockPaginatedProgramas,
            items: [mockProgramaExplorarItem],
            totalCount: 1,
        })
        vi.mocked(inscripcionService.solicitarInscripcion).mockResolvedValue(mockInscripcionCreada)

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Promociona mi nuevo album")).toBeInTheDocument()
        })

        await user.click(screen.getByRole("button", { name: /Solicitar inscripcion/i }))

        await waitFor(() => {
            expect(toast.success).toHaveBeenCalledWith(
                expect.stringContaining("Solicitud enviada")
            )
        })
    })

    it("solicitar inscripcion shows error toast on 400", async () => {
        const user = userEvent.setup()
        vi.mocked(inscripcionService.explorarProgramas).mockResolvedValue({
            ...mockPaginatedProgramas,
            items: [mockProgramaExplorarItem],
            totalCount: 1,
        })
        const error = new Error("Ya estas inscrito en este programa")
        ;(error as Error & { errorCode: string }).errorCode = "4021"
        vi.mocked(inscripcionService.solicitarInscripcion).mockRejectedValue(error)

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Promociona mi nuevo album")).toBeInTheDocument()
        })

        await user.click(screen.getByRole("button", { name: /Solicitar inscripcion/i }))

        await waitFor(() => {
            expect(toast.info).toHaveBeenCalled()
        })
    })

    it("solicitar inscripcion shows error toast on 403", async () => {
        const user = userEvent.setup()
        vi.mocked(inscripcionService.explorarProgramas).mockResolvedValue({
            ...mockPaginatedProgramas,
            items: [mockProgramaExplorarItem],
            totalCount: 1,
        })
        const error = new Error("No puedes inscribirte en este programa")
        ;(error as Error & { errorCode: string }).errorCode = "4022"
        vi.mocked(inscripcionService.solicitarInscripcion).mockRejectedValue(error)

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Promociona mi nuevo album")).toBeInTheDocument()
        })

        await user.click(screen.getByRole("button", { name: /Solicitar inscripcion/i }))

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalled()
        })
    })

    it("programa con miEstado Aprobado does not show solicitar button", async () => {
        vi.mocked(inscripcionService.explorarProgramas).mockResolvedValue({
            ...mockPaginatedProgramas,
            items: [mockProgramaAprobado],
            totalCount: 1,
        })

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Comparte mi single viral")).toBeInTheDocument()
        })

        expect(screen.queryByRole("button", { name: /Solicitar inscripcion/i })).not.toBeInTheDocument()
        expect(screen.getByText("Aprobado")).toBeInTheDocument()
    })

    it("programa con miEstado Bloqueado shows blocked message", async () => {
        vi.mocked(inscripcionService.explorarProgramas).mockResolvedValue({
            ...mockPaginatedProgramas,
            items: [mockProgramaBloqueado],
            totalCount: 1,
        })

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Programa de influencers")).toBeInTheDocument()
        })

        expect(screen.getByText(/No puedes inscribirte en este programa/i)).toBeInTheDocument()
    })

    it("renders page title and subtitle", () => {
        vi.mocked(inscripcionService.explorarProgramas).mockReturnValue(
            new Promise(() => {})
        )

        renderPage()

        expect(screen.getByText("Programas de promocion disponibles")).toBeInTheDocument()
        expect(screen.getByText(/empieza a ganar comisiones/i)).toBeInTheDocument()
    })

    it("renders filter controls", () => {
        vi.mocked(inscripcionService.explorarProgramas).mockReturnValue(
            new Promise(() => {})
        )

        renderPage()

        expect(screen.getByPlaceholderText("Buscar artista...")).toBeInTheDocument()
    })
})
