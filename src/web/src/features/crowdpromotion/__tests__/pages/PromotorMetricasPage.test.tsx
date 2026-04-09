import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter, Route, Routes } from "react-router-dom"
import {
    mockPromotorMetricasResponse,
    mockPromotorMetricasResponse_SinEventos,
} from "../../__mocks__/tracking.mock"

vi.mock("sonner", () => ({
    toast: { success: vi.fn(), error: vi.fn() },
}))

vi.mock("../../metricas/infrastructure/tracking.service", () => ({
    trackingService: {
        registrarEvento: vi.fn(),
        getMetricasPromotor: vi.fn(),
    },
}))

vi.mock("../../infrastructure/inscripcion.service", () => ({
    inscripcionService: {
        misProgramas: vi.fn(),
        explorarProgramas: vi.fn(),
        solicitarInscripcion: vi.fn(),
    },
}))

import { trackingService } from "../../metricas/infrastructure/tracking.service"
import { inscripcionService } from "../../infrastructure/inscripcion.service"
import PromotorMetricasPage from "../../metricas/presentation/pages/PromotorMetricasPage"

const mockProgramasAprobados = {
    items: [
        {
            id: "ins-001",
            programaId: "prog-001",
            programaTitulo: "Promociona mi album",
            artistaNombre: "Artist",
            tipoPromoNombre: "Social",
            importeComisionPorcentaje: 10,
            importeComisionFija: null,
            monedaNombre: "EUR",
            esAprobado: true,
            esBloqueado: false,
            codigoReferido: "album-2026-x7k9m",
            urlTrackingPersonalizada: "https://weplay.com/campanias/xxx?ref=album-2026-x7k9m",
            fechaAlta: "2026-01-01",
            fechaBaja: null,
            estado: "Aprobado" as const,
        },
        {
            id: "ins-002",
            programaId: "prog-002",
            programaTitulo: "Difunde mi EP",
            artistaNombre: "Artist 2",
            tipoPromoNombre: "Social",
            importeComisionPorcentaje: 15,
            importeComisionFija: null,
            monedaNombre: "EUR",
            esAprobado: true,
            esBloqueado: false,
            codigoReferido: "ep-verano-2026",
            urlTrackingPersonalizada: "https://weplay.com/campanias/yyy?ref=ep-verano-2026",
            fechaAlta: "2026-01-15",
            fechaBaja: null,
            estado: "Aprobado" as const,
        },
    ],
    totalCount: 2,
    page: 1,
    pageSize: 50,
    totalPages: 1,
}

const mockProgramasVacio = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 50,
    totalPages: 0,
}

function renderPage(programaId?: string) {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })
    const initialPath = programaId
        ? `/promotor/metricas?programaId=${programaId}`
        : "/promotor/metricas"

    return render(
        <QueryClientProvider client={queryClient}>
            <MemoryRouter initialEntries={[initialPath]}>
                <Routes>
                    <Route path="/promotor/metricas" element={<PromotorMetricasPage />} />
                    <Route
                        path="/promotor/mis-programas"
                        element={<div>Mis Programas Page</div>}
                    />
                </Routes>
            </MemoryRouter>
        </QueryClientProvider>
    )
}

describe("PromotorMetricasPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renderiza el titulo de la pagina", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockProgramasAprobados)
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        renderPage("prog-001")

        expect(screen.getByText("Mis metricas de promocion")).toBeInTheDocument()
    })

    it("muestra skeletons durante la carga inicial", () => {
        vi.mocked(inscripcionService.misProgramas).mockReturnValue(new Promise(() => {}))

        renderPage()

        expect(screen.getByLabelText("Cargando datos")).toBeInTheDocument()
    })

    it("renderiza KPI cards con datos del promotor tras carga", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockProgramasAprobados)
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        renderPage("prog-001")

        await waitFor(() => {
            expect(screen.getByText("450")).toBeInTheDocument()
        })
        expect(screen.getByText("5")).toBeInTheDocument()
        expect(screen.getByText("Comision")).toBeInTheDocument()
    })

    it("muestra tasa de conversion correctamente", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockProgramasAprobados)
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        renderPage("prog-001")

        await waitFor(() => {
            expect(screen.getByText(/1\.11%/)).toBeInTheDocument()
        })
    })

    it("muestra el selector de programa con opciones cargadas", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockProgramasAprobados)
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        renderPage("prog-001")

        await waitFor(() => {
            expect(screen.getByLabelText("Selecciona un programa de promocion")).toBeInTheDocument()
        })
    })

    it("renderiza la seccion EnlaceReferido con la URL correcta", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockProgramasAprobados)
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        renderPage("prog-001")

        await waitFor(() => {
            expect(screen.getByDisplayValue("https://weplay.com/campanias/xxx?ref=album-2026-x7k9m")).toBeInTheDocument()
        })
    })

    it("renderiza la lista EventosRecientesList con eventos", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockProgramasAprobados)
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        renderPage("prog-001")

        await waitFor(() => {
            expect(screen.getByText("Eventos recientes")).toBeInTheDocument()
        })
        expect(screen.getAllByText("Backing").length).toBeGreaterThanOrEqual(1)
        expect(screen.getAllByText("Click").length).toBeGreaterThanOrEqual(1)
    })

    it("muestra empty state cuando no tiene programas", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockProgramasVacio)

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Sin programas activos")).toBeInTheDocument()
        })
        expect(screen.getByText("Ver programas disponibles")).toBeInTheDocument()
    })

    it("muestra empty state de eventos cuando eventosRecientes esta vacio", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockProgramasAprobados)
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse_SinEventos
        )

        renderPage("prog-001")

        await waitFor(() => {
            expect(screen.getByText(/Aun no hay eventos registrados/)).toBeInTheDocument()
        })
    })

    it("muestra estado de error con boton Reintentar", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockProgramasAprobados)
        vi.mocked(trackingService.getMetricasPromotor).mockRejectedValue(
            new Error("Server error")
        )

        renderPage("prog-001")

        await waitFor(() => {
            expect(screen.getByText("No se pudieron cargar las metricas")).toBeInTheDocument()
        }, { timeout: 5000 })
        expect(screen.getByText("Reintentar")).toBeInTheDocument()
    })

    it("el boton Reintentar hace refetch de las metricas", async () => {
        const user = userEvent.setup()
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockProgramasAprobados)
        vi.mocked(trackingService.getMetricasPromotor).mockRejectedValue(
            new Error("Server error")
        )

        renderPage("prog-001")

        await waitFor(() => {
            expect(screen.getByText("Reintentar")).toBeInTheDocument()
        }, { timeout: 5000 })

        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        await user.click(screen.getByText("Reintentar"))

        await waitFor(() => {
            expect(screen.getByText("450")).toBeInTheDocument()
        }, { timeout: 5000 })
    })

    it("llama al servicio con el programaId correcto", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockProgramasAprobados)
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        renderPage("prog-001")

        await waitFor(() => {
            expect(trackingService.getMetricasPromotor).toHaveBeenCalledWith("prog-001")
        })
    })

    it("auto-selecciona el primer programa cuando no hay programaId en URL", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockProgramasAprobados)
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        renderPage()

        await waitFor(() => {
            expect(trackingService.getMetricasPromotor).toHaveBeenCalledWith("prog-001")
        })
    })

    it("el enlace 'Ver programas disponibles' navega correctamente", async () => {
        vi.mocked(inscripcionService.misProgramas).mockResolvedValue(mockProgramasVacio)

        renderPage()

        await waitFor(() => {
            expect(screen.getByText("Ver programas disponibles")).toBeInTheDocument()
        })

        const link = screen.getByText("Ver programas disponibles")
        expect(link.closest("a")).toHaveAttribute("href", "/promotor/mis-programas")
    })
})
