import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import {
    mockPaginatedMisProgramas,
    mockEmptyMisProgramas,
    mockMiPrograma_Aprobado,
    mockMiPrograma_Pendiente,
    mockMiPrograma_Bloqueado,
    mockMiPrograma_DadoDeBaja,
} from "../../__mocks__/inscripcion.mock"

vi.mock("sonner", () => ({
    toast: { success: vi.fn(), error: vi.fn() },
}))

vi.mock("@/features/promotor/application", () => ({
    usePromotor: vi.fn(() => ({
        data: { id: "promotor-001", nombrePublico: "Test Promotor" },
        isLoading: false,
        isError: false,
    })),
}))

vi.mock("../../infrastructure/inscripcion.service", () => ({
    inscripcionService: {
        explorarProgramas: vi.fn(),
        solicitarInscripcion: vi.fn(),
        misProgramas: vi.fn(),
    },
}))

import { inscripcionService } from "../../infrastructure/inscripcion.service"
import MisProgramasPage from "../../presentation/pages/MisProgramasPage"

function renderPage() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })

    return render(
        <QueryClientProvider client={queryClient}>
            <MemoryRouter initialEntries={["/promotor/mis-programas"]}>
                <MisProgramasPage />
            </MemoryRouter>
        </QueryClientProvider>
    )
}

describe("MisProgramasPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
        Object.defineProperty(navigator, "clipboard", {
            value: { writeText: vi.fn().mockResolvedValue(undefined) },
            writable: true,
            configurable: true,
        })
    })

    it("renders lista de inscripciones con todos los estados", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockPaginatedMisProgramas)

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Promociona mi nuevo album")).toBeInTheDocument()
        })

        expect(screen.getByText("Difunde mi EP de verano")).toBeInTheDocument()
        expect(screen.getByText("Programa de influencers")).toBeInTheDocument()
        expect(screen.getByText("Campana de lanzamiento")).toBeInTheDocument()
    })

    it("shows skeleton while loading", () => {
        vi.mocked(inscripcionService.misProgramas).mockReturnValue(
            new Promise(() => {})
        )

        renderPage()

        expect(screen.getByLabelText("Cargando inscripciones")).toBeInTheDocument()
    })

    it("shows empty state when no inscripciones", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockEmptyMisProgramas)

        renderPage()

        await waitFor(() => {
            expect(screen.getByText(/No estas inscrito en ningun programa/i)).toBeInTheDocument()
        }, { timeout: 5000 })

        // Two "Explorar programas" buttons exist: one in header, one in empty state CTA
        const explorarButtons = screen.getAllByRole("button", { name: /Explorar programas/i })
        expect(explorarButtons.length).toBeGreaterThanOrEqual(2)
    })

    it("shows error state when service fails", async () => {
        vi.mocked(inscripcionService.misProgramas).mockRejectedValue(
            new Error("API Error")
        )

        renderPage()

        await waitFor(() => {
            expect(screen.getByText(/No se pudieron cargar tus inscripciones/i)).toBeInTheDocument()
        }, { timeout: 5000 })

        expect(screen.getByRole("button", { name: /Reintentar/i })).toBeInTheDocument()
    })

    it("inscripcion aprobada shows detalle with codigoReferido", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue({
            ...mockPaginatedMisProgramas,
            items: [mockMiPrograma_Aprobado],
            totalCount: 1,
        })

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Promociona mi nuevo album")).toBeInTheDocument()
        })

        expect(screen.getByText("album-2026-x7k9m")).toBeInTheDocument()
    })

    it("inscripcion pendiente does not show codigoReferido", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue({
            ...mockPaginatedMisProgramas,
            items: [mockMiPrograma_Pendiente],
            totalCount: 1,
        })

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Difunde mi EP de verano")).toBeInTheDocument()
        })

        expect(screen.queryByText("album-2026-x7k9m")).not.toBeInTheDocument()
    })

    it("inscripcion bloqueada shows Bloqueado badge", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue({
            ...mockPaginatedMisProgramas,
            items: [mockMiPrograma_Bloqueado],
            totalCount: 1,
        })

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Programa de influencers")).toBeInTheDocument()
        })

        expect(screen.getByLabelText("Estado: Bloqueado")).toBeInTheDocument()
        expect(screen.getByText(/Tu acceso a este programa ha sido bloqueado/i)).toBeInTheDocument()
    })

    it("inscripcion DadoDeBaja shows DadoDeBaja badge", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue({
            ...mockPaginatedMisProgramas,
            items: [mockMiPrograma_DadoDeBaja],
            totalCount: 1,
        })

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Campana de lanzamiento")).toBeInTheDocument()
        })

        expect(screen.getByText("Dado de baja")).toBeInTheDocument()
        expect(screen.getByText(/Tu inscripcion fue dada de baja/i)).toBeInTheDocument()
    })

    it("shows tareas for inscripcion aprobada when expanded", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue({
            ...mockPaginatedMisProgramas,
            items: [mockMiPrograma_Aprobado],
            totalCount: 1,
        })

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Promociona mi nuevo album")).toBeInTheDocument()
        })

        expect(screen.getByRole("button", { name: /Ver tareas y estadisticas/i })).toBeInTheDocument()
    })

    it("renders page header", () => {
        vi.mocked(inscripcionService.misProgramas).mockReturnValue(
            new Promise(() => {})
        )

        renderPage()

        expect(screen.getByText("Mis programas")).toBeInTheDocument()
        expect(screen.getByText(/Gestiona tus inscripciones/i)).toBeInTheDocument()
    })

    it("renders Explorar programas button in header", () => {
        vi.mocked(inscripcionService.misProgramas).mockReturnValue(
            new Promise(() => {})
        )

        renderPage()

        expect(screen.getByRole("button", { name: /Explorar programas/i })).toBeInTheDocument()
    })
})
