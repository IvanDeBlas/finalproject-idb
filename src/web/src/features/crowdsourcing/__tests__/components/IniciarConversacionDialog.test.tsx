import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor, fireEvent } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import { IniciarConversacionDialog } from "../../presentation/components/IniciarConversacionDialog"
import { mockCreateConversacionResult } from "../../__mocks__/mensajeria.mock"

const mockNavigate = vi.fn()
vi.mock("react-router-dom", async () => {
    const actual = await vi.importActual("react-router-dom")
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    }
})

// Mock the hook directly for reliable testing
const mockMutate = vi.fn()
vi.mock("../../application/hooks/useCreateConversacion", () => ({
    useCreateConversacion: () => ({
        mutate: mockMutate,
        isPending: false,
    }),
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
        info: vi.fn(),
    },
}))

import { toast } from "sonner"

const defaultProps = {
    isOpen: true,
    onClose: vi.fn(),
    destinatarioId: "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    destinatarioNombre: "Studio Mix Pro",
    contextoId: "f2a3b4c5-d6e7-8f9a-0b1c-2d3e4f5a6b7c",
    contextoTipo: "necesidad" as const,
    contextoTitulo: "Mezcla de pistas para EP",
}

function renderDialog(props = {}) {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
        },
    })

    return render(
        <QueryClientProvider client={queryClient}>
            <MemoryRouter>
                <IniciarConversacionDialog {...defaultProps} {...props} />
            </MemoryRouter>
        </QueryClientProvider>
    )
}

describe("IniciarConversacionDialog", () => {
    beforeEach(() => {
        vi.clearAllMocks()
        // Default: mutate calls onSuccess
        mockMutate.mockImplementation((_data, callbacks) => {
            callbacks?.onSuccess?.(mockCreateConversacionResult)
        })
    })

    it("renders dialog when isOpen is true", () => {
        renderDialog()

        expect(screen.getByRole("dialog")).toBeInTheDocument()
        expect(
            screen.getByRole("heading", { name: /iniciar conversacion/i })
        ).toBeInTheDocument()
    })

    it("does not render content when isOpen is false", () => {
        renderDialog({ isOpen: false })

        expect(screen.queryByRole("dialog")).not.toBeInTheDocument()
    })

    it("shows destinatario name in header", () => {
        renderDialog()

        expect(
            screen.getByText(/Con:.*Studio Mix Pro/)
        ).toBeInTheDocument()
    })

    it("shows contexto titulo", () => {
        renderDialog()

        expect(
            screen.getByText("Mezcla de pistas para EP")
        ).toBeInTheDocument()
    })

    it("shows character counter for asunto", async () => {
        renderDialog()

        const input = screen.getByPlaceholderText(
            "Breve descripcion del motivo..."
        )
        await userEvent.type(input, "Test")

        expect(screen.getByText("4 / 200")).toBeInTheDocument()
    })

    it("shows error when asunto is empty on submit", async () => {
        mockMutate.mockImplementation(() => {})

        renderDialog()

        const form = screen.getByRole("dialog").querySelector("form")
        fireEvent.submit(form!)

        await waitFor(() => {
            expect(
                screen.getByText("El asunto es obligatorio")
            ).toBeInTheDocument()
        })
    })

    it("successful submit navigates to chat", async () => {
        renderDialog()

        const input = screen.getByPlaceholderText(
            "Breve descripcion del motivo..."
        )
        await userEvent.type(input, "Consulta sobre la propuesta")

        const form = screen.getByRole("dialog").querySelector("form")
        fireEvent.submit(form!)

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith(
                `/crowdsourcing/mensajes/${mockCreateConversacionResult.id}`
            )
        })
    })

    it("successful submit shows toast success", async () => {
        renderDialog()

        const input = screen.getByPlaceholderText(
            "Breve descripcion del motivo..."
        )
        await userEvent.type(input, "Consulta")

        const form = screen.getByRole("dialog").querySelector("form")
        fireEvent.submit(form!)

        await waitFor(() => {
            expect(toast.success).toHaveBeenCalledWith(
                "Conversacion iniciada correctamente."
            )
        })
    })

    it("successful submit calls onClose", async () => {
        const onClose = vi.fn()
        renderDialog({ onClose })

        const input = screen.getByPlaceholderText(
            "Breve descripcion del motivo..."
        )
        await userEvent.type(input, "Consulta")

        const form = screen.getByRole("dialog").querySelector("form")
        fireEvent.submit(form!)

        await waitFor(() => {
            expect(onClose).toHaveBeenCalled()
        })
    })

    it("error 4015 redirects to mensajes page", async () => {
        mockMutate.mockImplementation((_data, callbacks) => {
            callbacks?.onError?.(new Error("4015"))
        })

        renderDialog()

        const input = screen.getByPlaceholderText(
            "Breve descripcion del motivo..."
        )
        await userEvent.type(input, "Consulta")

        const form = screen.getByRole("dialog").querySelector("form")
        fireEvent.submit(form!)

        await waitFor(() => {
            expect(mockNavigate).toHaveBeenCalledWith(
                "/crowdsourcing/mensajes"
            )
        })
    })

    it("error 4015 shows info toast", async () => {
        mockMutate.mockImplementation((_data, callbacks) => {
            callbacks?.onError?.(new Error("4015"))
        })

        renderDialog()

        const input = screen.getByPlaceholderText(
            "Breve descripcion del motivo..."
        )
        await userEvent.type(input, "Consulta")

        const form = screen.getByRole("dialog").querySelector("form")
        fireEvent.submit(form!)

        await waitFor(() => {
            expect(toast.info).toHaveBeenCalled()
        })
    })

    it("generic error shows error toast, dialog stays open", async () => {
        mockMutate.mockImplementation((_data, callbacks) => {
            callbacks?.onError?.(new Error("5000"))
        })

        renderDialog()

        const input = screen.getByPlaceholderText(
            "Breve descripcion del motivo..."
        )
        await userEvent.type(input, "Consulta")

        const form = screen.getByRole("dialog").querySelector("form")
        fireEvent.submit(form!)

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalled()
        })

        expect(screen.getByRole("dialog")).toBeInTheDocument()
    })

    it("cancel button calls onClose", async () => {
        const onClose = vi.fn()
        renderDialog({ onClose })

        await userEvent.click(
            screen.getByRole("button", { name: /cancelar/i })
        )

        expect(onClose).toHaveBeenCalled()
    })

    it("shows acuerdo context type correctly", () => {
        renderDialog({
            contextoId: "b1c2d3e4-f5a6-7890-bcde-f12345678901",
            contextoTipo: "acuerdo",
            contextoTitulo: "Sesion de fotos",
        })

        expect(
            screen.getByText("Acuerdo de crowdsourcing")
        ).toBeInTheDocument()
    })
})
