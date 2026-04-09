import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import React from "react"
import LoginPage from "../../presentation/pages/LoginPage"

// Mock useLogin
const mockMutate = vi.fn()
const mockLoginMutation = {
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
        useLogin: () => mockLoginMutation,
        loginSchema: schemas.loginSchema,
    }
})

vi.mock("react-router-dom", async () => {
    const actual = await vi.importActual("react-router-dom")
    return {
        ...actual,
        useNavigate: () => vi.fn(),
    }
})

function renderLoginPage() {
    const queryClient = new QueryClient({
        defaultOptions: { queries: { retry: false } },
    })

    return render(
        React.createElement(
            QueryClientProvider,
            { client: queryClient },
            React.createElement(MemoryRouter, null, React.createElement(LoginPage))
        )
    )
}

describe("LoginPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
        mockLoginMutation.error = null
        mockLoginMutation.isError = false
        mockLoginMutation.isPending = false
    })

    it("renders login form with title", () => {
        renderLoginPage()

        expect(screen.getByRole("heading", { name: "Iniciar sesion" })).toBeInTheDocument()
        expect(screen.getByLabelText("Email")).toBeInTheDocument()
        expect(screen.getByLabelText("Contrasena")).toBeInTheDocument()
    })

    it("renders submit button", () => {
        renderLoginPage()

        expect(
            screen.getByRole("button", { name: "Iniciar sesion" })
        ).toBeInTheDocument()
    })

    it("renders link to register page", () => {
        renderLoginPage()

        expect(screen.getByText("Registrate")).toBeInTheDocument()
    })

    it("shows email validation error for empty submit", async () => {
        const user = userEvent.setup()
        renderLoginPage()

        const submitButton = screen.getByRole("button", { name: /Iniciar sesion/ })
        await user.click(submitButton)

        await waitFor(() => {
            expect(screen.getByText("Email invalido")).toBeInTheDocument()
        })
    })

    it("shows password validation error for short password", async () => {
        const user = userEvent.setup()
        renderLoginPage()

        const emailInput = screen.getByLabelText("Email")
        const passwordInput = screen.getByLabelText("Contrasena")
        const submitButton = screen.getByRole("button", { name: /Iniciar sesion/ })

        await user.type(emailInput, "test@example.com")
        await user.type(passwordInput, "123")
        await user.click(submitButton)

        await waitFor(() => {
            expect(
                screen.getByText(/al menos 6 caracteres/)
            ).toBeInTheDocument()
        })
    })

    it("calls mutate with form data on valid submit", async () => {
        const user = userEvent.setup()
        renderLoginPage()

        const emailInput = screen.getByLabelText("Email")
        const passwordInput = screen.getByLabelText("Contrasena")
        const submitButton = screen.getByRole("button", { name: /Iniciar sesion/ })

        await user.type(emailInput, "test@example.com")
        await user.type(passwordInput, "123456")
        await user.click(submitButton)

        await waitFor(() => {
            expect(mockMutate).toHaveBeenCalledWith({
                email: "test@example.com",
                password: "123456",
            })
        })
    })

    it("shows error message when login mutation fails", () => {
        mockLoginMutation.error = new Error("Invalid credentials")
        mockLoginMutation.isError = true

        renderLoginPage()

        expect(
            screen.getByText("Credenciales invalidas. Intenta de nuevo.")
        ).toBeInTheDocument()
    })

    it("renders description text", () => {
        renderLoginPage()

        expect(
            screen.getByText("Ingresa tus credenciales para acceder a tu cuenta")
        ).toBeInTheDocument()
    })
})
