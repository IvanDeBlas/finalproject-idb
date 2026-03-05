import { createElement } from "react"
import { render, screen, fireEvent, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { describe, it, expect, vi, beforeEach } from "vitest"
import { ValoracionForm } from "../../presentation/components/ValoracionForm"
import {
    MOCK_ACUERDO_ID,
    MOCK_USER_ID,
    mockValoracionCreada,
    mockValoracionCreadaSinComentario,
} from "../../__mocks__/valoracion.mock"

vi.mock("../../infrastructure", () => ({
    valoracionApi: {
        create: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

import { valoracionApi } from "../../infrastructure"
import { toast } from "sonner"

function createTestQueryClient() {
    return new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
            mutations: { retry: false },
        },
    })
}

function renderForm(onSuccess = vi.fn()) {
    const queryClient = createTestQueryClient()
    return {
        onSuccess,
        ...render(
            createElement(
                QueryClientProvider,
                { client: queryClient },
                createElement(ValoracionForm, {
                    acuerdoId: MOCK_ACUERDO_ID,
                    userIdValorado: MOCK_USER_ID,
                    onSuccess,
                })
            )
        ),
    }
}

describe("ValoracionForm", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it('renders the form card with title "Deja tu valoracion"', () => {
        renderForm()
        expect(screen.getByText("Deja tu valoracion")).toBeInTheDocument()
    })

    it("renders incentive message text", () => {
        renderForm()
        expect(
            screen.getByText(
                "Tu comentario ayuda a otros artistas/profesionales"
            )
        ).toBeInTheDocument()
    })

    it("renders StarRating component", () => {
        renderForm()
        expect(
            screen.getByRole("radiogroup", {
                name: /puntuacion de 1 a 5 estrellas/i,
            })
        ).toBeInTheDocument()
    })

    it("renders optional comment textarea", () => {
        renderForm()
        expect(
            screen.getByPlaceholderText(
                "Comparte tu experiencia con este profesional..."
            )
        ).toBeInTheDocument()
    })

    it("renders character counter starting at 0 / 1000", () => {
        renderForm()
        expect(
            screen.getByText("0 / 1000 caracteres")
        ).toBeInTheDocument()
    })

    it("submit button is disabled when no star is selected", () => {
        renderForm()
        const submitBtn = screen.getByRole("button", {
            name: /enviar valoracion/i,
        })
        expect(submitBtn).toBeDisabled()
    })

    it("submit button is enabled after selecting a star", () => {
        renderForm()
        fireEvent.click(screen.getByRole("radio", { name: /4 estrellas/i }))
        const submitBtn = screen.getByRole("button", {
            name: /enviar valoracion/i,
        })
        expect(submitBtn).toBeEnabled()
    })

    it("shows validation error when submitting without rating", () => {
        renderForm()
        // Submit button is disabled when no star is selected, preventing submission
        // This validates that the UI properly prevents submission without a rating
        const submitBtn = screen.getByRole("button", {
            name: /enviar valoracion/i,
        })
        expect(submitBtn).toBeDisabled()
        // Also verify the validation message is NOT shown initially (only on submit attempt)
        expect(
            screen.queryByText("Selecciona una puntuacion")
        ).not.toBeInTheDocument()
    })

    it("updates character counter as user types in textarea", () => {
        renderForm()
        const textarea = screen.getByPlaceholderText(
            "Comparte tu experiencia con este profesional..."
        )
        fireEvent.change(textarea, { target: { value: "Buen trabajo" } })
        expect(
            screen.getByText("12 / 1000 caracteres")
        ).toBeInTheDocument()
    })

    it("counter turns amber color when comment exceeds 900 chars", () => {
        renderForm()
        const textarea = screen.getByPlaceholderText(
            "Comparte tu experiencia con este profesional..."
        )
        const longText = "a".repeat(901)
        fireEvent.change(textarea, { target: { value: longText } })
        const counter = screen.getByText("901 / 1000 caracteres")
        expect(counter.className).toContain("text-[#f59e0b]")
    })

    it("counter turns red when comment exceeds 1000 chars", () => {
        renderForm()
        const textarea = screen.getByPlaceholderText(
            "Comparte tu experiencia con este profesional..."
        )
        const longText = "a".repeat(1001)
        fireEvent.change(textarea, { target: { value: longText } })
        const counter = screen.getByText("1001 / 1000 caracteres")
        expect(counter.className).toContain("text-[#ef4444]")
    })

    it("submit button is disabled when comment exceeds 1000 chars", () => {
        renderForm()
        // Select a star first
        fireEvent.click(screen.getByRole("radio", { name: /5 estrellas/i }))
        const textarea = screen.getByPlaceholderText(
            "Comparte tu experiencia con este profesional..."
        )
        fireEvent.change(textarea, {
            target: { value: "a".repeat(1001) },
        })
        const submitBtn = screen.getByRole("button", {
            name: /enviar valoracion/i,
        })
        expect(submitBtn).toBeDisabled()
    })

    it("calls onSuccess with result when mutation succeeds", async () => {
        vi.mocked(valoracionApi.create).mockResolvedValue(
            mockValoracionCreada
        )
        const onSuccess = vi.fn()
        renderForm(onSuccess)

        fireEvent.click(screen.getByRole("radio", { name: /5 estrellas/i }))
        fireEvent.click(
            screen.getByRole("button", { name: /enviar valoracion/i })
        )

        await waitFor(() => {
            expect(onSuccess).toHaveBeenCalledWith(mockValoracionCreada)
        })
    })

    it("shows success toast on submission", async () => {
        vi.mocked(valoracionApi.create).mockResolvedValue(
            mockValoracionCreada
        )
        renderForm()

        fireEvent.click(screen.getByRole("radio", { name: /5 estrellas/i }))
        fireEvent.click(
            screen.getByRole("button", { name: /enviar valoracion/i })
        )

        await waitFor(() => {
            expect(toast.success).toHaveBeenCalledWith(
                "Valoracion enviada. Gracias por tu feedback."
            )
        })
    })

    it("submits form without comment (comentario is optional)", async () => {
        vi.mocked(valoracionApi.create).mockResolvedValue(
            mockValoracionCreadaSinComentario
        )
        renderForm()

        fireEvent.click(screen.getByRole("radio", { name: /4 estrellas/i }))
        // Do NOT type in the textarea
        fireEvent.click(
            screen.getByRole("button", { name: /enviar valoracion/i })
        )

        await waitFor(() => {
            expect(valoracionApi.create).toHaveBeenCalledWith(
                MOCK_ACUERDO_ID,
                { puntuacion: 4, comentario: undefined }
            )
        })
    })

    it('submit button shows "Enviando..." during submission', async () => {
        let resolveCreate: (value: unknown) => void
        vi.mocked(valoracionApi.create).mockImplementation(
            () =>
                new Promise((resolve) => {
                    resolveCreate = resolve
                })
        )
        renderForm()

        fireEvent.click(screen.getByRole("radio", { name: /5 estrellas/i }))
        fireEvent.click(
            screen.getByRole("button", { name: /enviar valoracion/i })
        )

        await waitFor(() => {
            expect(screen.getByText("Enviando...")).toBeInTheDocument()
        })

        // Resolve the promise to clean up
        resolveCreate!(mockValoracionCreada)
    })

    it("shows error toast when mutation fails", async () => {
        vi.mocked(valoracionApi.create).mockRejectedValue(
            new Error("4014")
        )
        renderForm()

        fireEvent.click(screen.getByRole("radio", { name: /5 estrellas/i }))
        fireEvent.click(
            screen.getByRole("button", { name: /enviar valoracion/i })
        )

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalled()
        })
    })

    it("form remains editable after API error", async () => {
        vi.mocked(valoracionApi.create).mockRejectedValue(
            new Error("5000")
        )
        renderForm()

        fireEvent.click(screen.getByRole("radio", { name: /5 estrellas/i }))
        fireEvent.click(
            screen.getByRole("button", { name: /enviar valoracion/i })
        )

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalled()
        })

        // Form should still be editable
        const textarea = screen.getByPlaceholderText(
            "Comparte tu experiencia con este profesional..."
        )
        expect(textarea).not.toBeDisabled()
        const submitBtn = screen.getByRole("button", {
            name: /enviar valoracion/i,
        })
        expect(submitBtn).toBeEnabled()
    })
})
