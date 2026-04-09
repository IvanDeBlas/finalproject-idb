import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, fireEvent, waitFor } from "@testing-library/react"
import { MemoryRouter, Route, Routes } from "react-router-dom"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { TooltipProvider } from "@/components/ui/tooltip"
import NuevoProyectoPage from "../../presentation/pages/NuevoProyectoPage"
import { mockTemplatesList, mockTemplateDetail } from "../../__mocks__/crowdsourcing.mock"

vi.mock("../../infrastructure", () => ({
    crowdsourcingApi: {
        getTemplates: vi.fn(),
        getTemplateById: vi.fn(),
        generarNecesidades: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

import { crowdsourcingApi } from "../../infrastructure"

function renderPage(initialEntry = "/crowdsourcing/nuevo-proyecto?step=1") {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
        },
    })

    return render(
        <QueryClientProvider client={queryClient}>
            <MemoryRouter initialEntries={[initialEntry]}>
                <Routes>
                    <Route
                        path="/crowdsourcing/nuevo-proyecto"
                        element={<NuevoProyectoPage />}
                    />
                </Routes>
            </MemoryRouter>
        </QueryClientProvider>
    )
}

describe("NuevoProyectoPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
        vi.mocked(crowdsourcingApi.getTemplates).mockResolvedValue(mockTemplatesList)
        vi.mocked(crowdsourcingApi.getTemplateById).mockResolvedValue(mockTemplateDetail)
    })

    describe("Step 1 - Template Gallery", () => {
        it("renders step 1 title", async () => {
            renderPage()

            await waitFor(() => {
                expect(
                    screen.getByText("Selecciona tu tipo de proyecto")
                ).toBeInTheDocument()
            })
        })

        it("renders wizard stepper", async () => {
            renderPage()

            await waitFor(() => {
                expect(screen.getByText("Seleccionar")).toBeInTheDocument()
                expect(screen.getByText("Personalizar")).toBeInTheDocument()
                expect(screen.getByText("Confirmar")).toBeInTheDocument()
            })
        })

        it("renders templates from API", async () => {
            renderPage()

            await waitFor(() => {
                expect(
                    screen.getByText("Produccion de EP")
                ).toBeInTheDocument()
            })
        })

        it("renders all template cards with select buttons", async () => {
            renderPage()

            await waitFor(() => {
                expect(
                    screen.getByText("Produccion de EP")
                ).toBeInTheDocument()
            })

            // All 3 template card buttons rendered (role="button" with aria-label containing "Template:")
            const templateCards = screen.getAllByRole("button", { name: /Template:/i })
            expect(templateCards).toHaveLength(3)
        })
    })

    describe("Step 2 - Customize Needs", () => {
        it("renders template name and needs", async () => {
            renderPage(
                "/crowdsourcing/nuevo-proyecto?step=2&template=tpl-001"
            )

            await waitFor(() => {
                expect(
                    screen.getByText("Produccion de EP")
                ).toBeInTheDocument()
            })

            expect(screen.getByText("Productor Musical")).toBeInTheDocument()
        })

        it("renders budget summary", async () => {
            renderPage(
                "/crowdsourcing/nuevo-proyecto?step=2&template=tpl-001"
            )

            await waitFor(() => {
                expect(
                    screen.getByText("Resumen Presupuestario")
                ).toBeInTheDocument()
            })
        })

        it("renders back and next buttons", async () => {
            renderPage(
                "/crowdsourcing/nuevo-proyecto?step=2&template=tpl-001"
            )

            await waitFor(() => {
                expect(screen.getByText("Produccion de EP")).toBeInTheDocument()
            })

            expect(screen.getByText(/Atras/)).toBeInTheDocument()
            expect(screen.getByText(/Siguiente/)).toBeInTheDocument()
        })

        it("disables next button when no needs selected", async () => {
            renderPage(
                "/crowdsourcing/nuevo-proyecto?step=2&template=tpl-001"
            )

            await waitFor(() => {
                expect(screen.getByText("Produccion de EP")).toBeInTheDocument()
            })

            // Auto-selected Alta needs - uncheck all to test disabled state
            const checkboxes = screen.getAllByRole("checkbox")
            checkboxes.forEach((cb) => {
                if ((cb as HTMLInputElement).getAttribute("aria-checked") === "true") {
                    fireEvent.click(cb)
                }
            })

            const nextButton = screen.getByText(/Siguiente/)
            expect(nextButton.closest("button")).toBeDisabled()
        })

        it("shows error when template not found", async () => {
            vi.mocked(crowdsourcingApi.getTemplateById).mockResolvedValue(
                undefined as never
            )

            renderPage(
                "/crowdsourcing/nuevo-proyecto?step=2&template=invalid"
            )

            await waitFor(() => {
                expect(
                    screen.getByText(/no se encontro/i)
                ).toBeInTheDocument()
            })
        })
    })

    describe("Step 3 - Confirmation", () => {
        it("renders confirmation title", async () => {
            renderPage(
                "/crowdsourcing/nuevo-proyecto?step=3&template=tpl-001"
            )

            await waitFor(() => {
                expect(
                    screen.getByText("Resumen de tu proyecto")
                ).toBeInTheDocument()
            })
        })

        it("renders confirm button", async () => {
            renderPage(
                "/crowdsourcing/nuevo-proyecto?step=3&template=tpl-001"
            )

            await waitFor(() => {
                expect(
                    screen.getByText("Confirmar y publicar")
                ).toBeInTheDocument()
            })
        })

        it("renders proyecto selector", async () => {
            renderPage(
                "/crowdsourcing/nuevo-proyecto?step=3&template=tpl-001"
            )

            await waitFor(() => {
                expect(
                    screen.getByText("Proyecto Artistico (opcional)")
                ).toBeInTheDocument()
            })
        })

        it("renders back button in step 3", async () => {
            renderPage(
                "/crowdsourcing/nuevo-proyecto?step=3&template=tpl-001"
            )

            await waitFor(() => {
                expect(screen.getByText(/Atras/)).toBeInTheDocument()
            })
        })
    })
})
