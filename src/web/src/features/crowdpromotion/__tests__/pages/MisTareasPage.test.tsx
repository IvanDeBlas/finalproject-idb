import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter, Route, Routes } from "react-router-dom"
import {
    mockMisTareasResponse_ConTareas,
    mockMisTareasResponse_Vacia,
    mockCompletarTareaResponse,
} from "../../__mocks__/tareas.mock"

vi.mock("sonner", () => ({
    toast: { success: vi.fn(), error: vi.fn() },
}))

vi.mock("../../tareas/infrastructure/tareas.service", () => ({
    tareasService: {
        misTareas: vi.fn(),
        completarTarea: vi.fn(),
    },
}))

import { tareasService } from "../../tareas/infrastructure/tareas.service"
import MisTareasPage from "../../tareas/presentation/pages/MisTareasPage"

function renderPage(programaId = "prog-001") {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })

    return render(
        <QueryClientProvider client={queryClient}>
            <MemoryRouter
                initialEntries={[
                    `/promotor/programas/${programaId}/tareas`,
                ]}
            >
                <Routes>
                    <Route
                        path="/promotor/programas/:programaId/tareas"
                        element={<MisTareasPage />}
                    />
                    <Route
                        path="/promotor/mis-programas"
                        element={<div>Mis Programas Redirect</div>}
                    />
                </Routes>
            </MemoryRouter>
        </QueryClientProvider>
    )
}

describe("MisTareasPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders list of tasks on successful fetch", async () => {
        vi.mocked(tareasService.misTareas).mockResolvedValue(
            mockMisTareasResponse_ConTareas
        )

        renderPage()

        await waitFor(
            () => {
                expect(
                    screen.getByText("Comparte en Instagram Stories")
                ).toBeInTheDocument()
            },
            { timeout: 5000 }
        )

        expect(
            screen.getByText("Escribe una resena en Spotify")
        ).toBeInTheDocument()
        expect(
            screen.getByText("Menciona el album en tu blog")
        ).toBeInTheDocument()
    })

    it("shows skeleton during loading", () => {
        vi.mocked(tareasService.misTareas).mockReturnValue(
            new Promise(() => {})
        )

        renderPage()

        expect(
            screen.getByLabelText("Cargando tareas")
        ).toBeInTheDocument()
    })

    it("shows empty state when no tasks", async () => {
        vi.mocked(tareasService.misTareas).mockResolvedValue(
            mockMisTareasResponse_Vacia
        )

        renderPage()

        await waitFor(
            () => {
                expect(
                    screen.getByText(
                        /Este programa no tiene tareas activas/i
                    )
                ).toBeInTheDocument()
            },
            { timeout: 5000 }
        )
    })

    it("shows error state when fetch fails", async () => {
        vi.mocked(tareasService.misTareas).mockRejectedValue(
            new Error("API Error")
        )

        renderPage()

        await waitFor(
            () => {
                expect(
                    screen.getByText(
                        /No se pudieron cargar las tareas/i
                    )
                ).toBeInTheDocument()
            },
            { timeout: 5000 }
        )

        expect(
            screen.getByRole("button", { name: /Reintentar/i })
        ).toBeInTheDocument()
    })

    it("renders program name in the header", async () => {
        vi.mocked(tareasService.misTareas).mockResolvedValue(
            mockMisTareasResponse_ConTareas
        )

        renderPage()

        await waitFor(() => {
            expect(
                screen.getByRole("heading", {
                    name: /Promociona mi nuevo album/,
                })
            ).toBeInTheDocument()
        })
    })

    it("renders multiple TareaCards for each item", async () => {
        vi.mocked(tareasService.misTareas).mockResolvedValue(
            mockMisTareasResponse_ConTareas
        )

        renderPage()

        await waitFor(() => {
            expect(
                screen.getByText("Comparte en Instagram Stories")
            ).toBeInTheDocument()
        })

        const listItems = screen.getAllByRole("listitem")
        expect(listItems.length).toBe(3)
    })

    it("opens CompletarTareaDialog when clicking Completar button", async () => {
        vi.mocked(tareasService.misTareas).mockResolvedValue(
            mockMisTareasResponse_ConTareas
        )

        renderPage()

        await waitFor(() => {
            expect(
                screen.getByText("Comparte en Instagram Stories")
            ).toBeInTheDocument()
        })

        const user = userEvent.setup()
        await user.click(
            screen.getByRole("button", { name: /Completar tarea/i })
        )

        await waitFor(() => {
            expect(screen.getByRole("dialog")).toBeInTheDocument()
        })
    })

    it("renders breadcrumb navigation", async () => {
        vi.mocked(tareasService.misTareas).mockResolvedValue(
            mockMisTareasResponse_ConTareas
        )

        renderPage()

        await waitFor(() => {
            expect(
                screen.getByText("Mis programas")
            ).toBeInTheDocument()
        })

        expect(
            screen.getByLabelText("Breadcrumb")
        ).toBeInTheDocument()
        expect(
            screen.getByText("Tareas")
        ).toBeInTheDocument()
    })

    it("renders subtitle text", () => {
        vi.mocked(tareasService.misTareas).mockReturnValue(
            new Promise(() => {})
        )

        renderPage()

        expect(
            screen.getByText(
                /Completa las tareas y acumula recompensas/i
            )
        ).toBeInTheDocument()
    })
})
