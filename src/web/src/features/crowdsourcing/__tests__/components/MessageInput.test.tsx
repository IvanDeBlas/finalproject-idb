import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, fireEvent, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import { MessageInput } from "../../presentation/components/MessageInput"
import { mockMensajeNuevo } from "../../__mocks__/mensajeria.mock"

vi.mock("../../infrastructure", () => ({
    mensajeApi: {
        getByConversacion: vi.fn(),
        create: vi.fn(),
        marcarLeidos: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
        info: vi.fn(),
    },
}))

import { mensajeApi } from "../../infrastructure"

function renderMessageInput(
    conversacionId = "conv-123",
    onMensajeEnviado = vi.fn()
) {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
        },
    })

    return render(
        <QueryClientProvider client={queryClient}>
            <MemoryRouter>
                <MessageInput
                    conversacionId={conversacionId}
                    onMensajeEnviado={onMensajeEnviado}
                />
            </MemoryRouter>
        </QueryClientProvider>
    )
}

describe("MessageInput", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders textarea and send button", () => {
        renderMessageInput()

        expect(
            screen.getByLabelText("Escribe un mensaje")
        ).toBeInTheDocument()
        expect(
            screen.getByRole("button", { name: /enviar/i })
        ).toBeInTheDocument()
    })

    it("send button is disabled when textarea is empty", () => {
        renderMessageInput()

        expect(
            screen.getByRole("button", { name: /enviar/i })
        ).toBeDisabled()
    })

    it("send button is enabled when there is content", async () => {
        renderMessageInput()

        const textarea = screen.getByLabelText("Escribe un mensaje")
        await userEvent.type(textarea, "Hola mundo")

        expect(
            screen.getByRole("button", { name: /enviar/i })
        ).not.toBeDisabled()
    })

    it("toggle shows URL input on click", async () => {
        renderMessageInput()

        const toggleBtn = screen.getByText("Adjuntar URL")
        await userEvent.click(toggleBtn)

        expect(screen.getByPlaceholderText("https://...")).toBeInTheDocument()
    })

    it("toggle hides URL input on second click", async () => {
        renderMessageInput()

        const toggleBtn = screen.getByText("Adjuntar URL")
        await userEvent.click(toggleBtn)

        expect(screen.getByPlaceholderText("https://...")).toBeInTheDocument()

        await userEvent.click(screen.getByText("Quitar adjunto"))

        expect(
            screen.queryByPlaceholderText("https://...")
        ).not.toBeInTheDocument()
    })

    it("shows character counter when over 4000 chars", async () => {
        renderMessageInput()

        const textarea = screen.getByLabelText("Escribe un mensaje")
        const longText = "A".repeat(4001)
        fireEvent.change(textarea, { target: { value: longText } })

        await waitFor(() => {
            expect(screen.getByText("4001/5000")).toBeInTheDocument()
        })
    })

    it("calls mutation with correct data on send click", async () => {
        const onMensajeEnviado = vi.fn()
        vi.mocked(mensajeApi.create).mockResolvedValue(mockMensajeNuevo)

        renderMessageInput("conv-123", onMensajeEnviado)

        const textarea = screen.getByLabelText("Escribe un mensaje")
        await userEvent.type(textarea, "Mensaje de prueba")
        await userEvent.click(
            screen.getByRole("button", { name: /enviar/i })
        )

        await waitFor(() => {
            expect(mensajeApi.create).toHaveBeenCalledWith("conv-123", {
                contenido: "Mensaje de prueba",
                urlAdjunto: undefined,
            })
        })
    })

    it("clears textarea after successful send", async () => {
        const onMensajeEnviado = vi.fn()
        vi.mocked(mensajeApi.create).mockResolvedValue(mockMensajeNuevo)

        renderMessageInput("conv-123", onMensajeEnviado)

        const textarea = screen.getByLabelText(
            "Escribe un mensaje"
        ) as HTMLTextAreaElement
        await userEvent.type(textarea, "Mensaje de prueba")
        await userEvent.click(
            screen.getByRole("button", { name: /enviar/i })
        )

        await waitFor(() => {
            expect(textarea.value).toBe("")
        })
    })

    it("send button has accessible aria-label", () => {
        renderMessageInput()

        expect(
            screen.getByRole("button", { name: "Enviar mensaje" })
        ).toBeInTheDocument()
    })
})
