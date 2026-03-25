import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import React from "react"
import RegisterPage from "../../presentation/pages/RegisterPage"

// Mock useRegister
const mockMutate = vi.fn()
const mockRegisterMutation = {
    mutate: mockMutate,
    error: null as Error | null,
    isError: false,
    isPending: false,
}

vi.mock("../../application", async () => {
    const schemas = await vi.importActual<typeof import("../../application/schemas")>(
        "../../application/schemas"
    )
    return {
        useRegister: () => mockRegisterMutation,
        registerSchema: schemas.registerSchema,
    }
})

vi.mock("react-router-dom", async () => {
    const actual = await vi.importActual("react-router-dom")
    return {
        ...actual,
        useNavigate: () => vi.fn(),
    }
})

function renderRegisterPage() {
    const queryClient = new QueryClient({
        defaultOptions: { queries: { retry: false } },
    })

    return render(
        React.createElement(
            QueryClientProvider,
            { client: queryClient },
            React.createElement(
                MemoryRouter,
                null,
                React.createElement(RegisterPage)
            )
        )
    )
}

describe("RegisterPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
        mockRegisterMutation.error = null
        mockRegisterMutation.isError = false
        mockRegisterMutation.isPending = false
    })

    it("renders registration form with title", () => {
        renderRegisterPage()

        expect(screen.getByRole("heading", { name: "Crear cuenta" })).toBeInTheDocument()
        expect(screen.getByLabelText("Nombre completo")).toBeInTheDocument()
        expect(screen.getByLabelText("Email")).toBeInTheDocument()
        expect(screen.getByLabelText("Contrasena")).toBeInTheDocument()
        expect(screen.getByLabelText("Confirmar contrasena")).toBeInTheDocument()
    })

    it("renders submit button", () => {
        renderRegisterPage()

        expect(
            screen.getByRole("button", { name: "Crear cuenta" })
        ).toBeInTheDocument()
    })

    it("renders link to login page", () => {
        renderRegisterPage()

        expect(screen.getByText("Inicia sesion")).toBeInTheDocument()
    })

    it("renders description text", () => {
        renderRegisterPage()

        expect(
            screen.getByText("Registrate para empezar a apoyar o crear campanias")
        ).toBeInTheDocument()
    })

    it("shows validation errors for empty fields", async () => {
        const user = userEvent.setup()
        renderRegisterPage()

        const submitButton = screen.getByRole("button", { name: "Crear cuenta" })
        await user.click(submitButton)

        await waitFor(() => {
            expect(screen.getByText(/Email invalido/)).toBeInTheDocument()
        })
    })

    it("shows error for short nombre", async () => {
        const user = userEvent.setup()
        renderRegisterPage()

        const nombreInput = screen.getByLabelText("Nombre completo")
        const emailInput = screen.getByLabelText("Email")
        const passwordInput = screen.getByLabelText("Contrasena")
        const confirmInput = screen.getByLabelText("Confirmar contrasena")
        const submitButton = screen.getByRole("button", { name: "Crear cuenta" })

        await user.type(nombreInput, "A")
        await user.type(emailInput, "test@example.com")
        await user.type(passwordInput, "123456")
        await user.type(confirmInput, "123456")
        await user.click(submitButton)

        await waitFor(() => {
            expect(screen.getByText(/obligatorio/)).toBeInTheDocument()
        })
    })

    it("shows error for mismatched passwords", async () => {
        const user = userEvent.setup()
        renderRegisterPage()

        const nombreInput = screen.getByLabelText("Nombre completo")
        const emailInput = screen.getByLabelText("Email")
        const passwordInput = screen.getByLabelText("Contrasena")
        const confirmInput = screen.getByLabelText("Confirmar contrasena")
        const submitButton = screen.getByRole("button", { name: "Crear cuenta" })

        await user.type(nombreInput, "Test User")
        await user.type(emailInput, "test@example.com")
        await user.type(passwordInput, "123456")
        await user.type(confirmInput, "654321")
        await user.click(submitButton)

        await waitFor(() => {
            expect(screen.getByText(/no coinciden/)).toBeInTheDocument()
        })
    })

    it("calls mutate with form data on valid submit", async () => {
        const user = userEvent.setup()
        renderRegisterPage()

        const nombreInput = screen.getByLabelText("Nombre completo")
        const emailInput = screen.getByLabelText("Email")
        const passwordInput = screen.getByLabelText("Contrasena")
        const confirmInput = screen.getByLabelText("Confirmar contrasena")
        const submitButton = screen.getByRole("button", { name: "Crear cuenta" })

        await user.type(nombreInput, "Juan Perez")
        await user.type(emailInput, "juan@example.com")
        await user.type(passwordInput, "123456")
        await user.type(confirmInput, "123456")
        await user.click(submitButton)

        await waitFor(() => {
            expect(mockMutate).toHaveBeenCalledWith({
                email: "juan@example.com",
                password: "123456",
                confirmPassword: "123456",
                nombreCompleto: "Juan Perez",
            })
        })
    })

    it("shows error message when register mutation fails", () => {
        mockRegisterMutation.error = new Error("Email already exists")
        mockRegisterMutation.isError = true

        renderRegisterPage()

        expect(
            screen.getByText("Error al registrar. Intenta de nuevo.")
        ).toBeInTheDocument()
    })
})
