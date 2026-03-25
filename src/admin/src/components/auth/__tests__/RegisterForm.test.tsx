import { describe, it, expect, vi } from "vitest"
import { render, screen, waitFor } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { RegisterForm } from "../RegisterForm"

describe("RegisterForm", () => {
    const mockOnSubmit = vi.fn()

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders all form fields", () => {
        render(<RegisterForm onSubmit={mockOnSubmit} />)

        expect(screen.getByLabelText("Email")).toBeInTheDocument()
        expect(screen.getByLabelText("Contraseña")).toBeInTheDocument()
        expect(screen.getByLabelText("Confirmar Contraseña")).toBeInTheDocument()
    })

    it("renders submit button with default text", () => {
        render(<RegisterForm onSubmit={mockOnSubmit} />)

        expect(
            screen.getByRole("button", { name: "Crear cuenta" })
        ).toBeInTheDocument()
    })

    it("shows loading text when isSubmitting is true", () => {
        render(<RegisterForm onSubmit={mockOnSubmit} isSubmitting />)

        expect(
            screen.getByRole("button", { name: "Registrando..." })
        ).toBeInTheDocument()
    })

    it("disables submit button when isSubmitting is true", () => {
        render(<RegisterForm onSubmit={mockOnSubmit} isSubmitting />)

        expect(
            screen.getByRole("button", { name: "Registrando..." })
        ).toBeDisabled()
    })

    it("displays server error when provided", () => {
        render(
            <RegisterForm
                onSubmit={mockOnSubmit}
                serverError="Email ya registrado"
            />
        )

        expect(screen.getByText("Email ya registrado")).toBeInTheDocument()
    })

    it("does not display server error when not provided", () => {
        render(<RegisterForm onSubmit={mockOnSubmit} />)

        expect(
            screen.queryByText("Email ya registrado")
        ).not.toBeInTheDocument()
    })

    it("shows validation error for empty email on submit", async () => {
        const user = userEvent.setup()
        render(<RegisterForm onSubmit={mockOnSubmit} />)

        await user.click(screen.getByRole("button", { name: "Crear cuenta" }))

        await waitFor(() => {
            expect(
                screen.getByText("El email es obligatorio")
            ).toBeInTheDocument()
        })
        expect(mockOnSubmit).not.toHaveBeenCalled()
    })

    it("does not submit form with invalid email", async () => {
        const user = userEvent.setup()
        render(<RegisterForm onSubmit={mockOnSubmit} />)

        await user.type(screen.getByLabelText("Email"), "invalid-email")
        await user.type(screen.getByLabelText("Contraseña"), "123456")
        await user.type(
            screen.getByLabelText("Confirmar Contraseña"),
            "123456"
        )
        await user.click(screen.getByRole("button", { name: "Crear cuenta" }))

        await waitFor(() => {
            expect(mockOnSubmit).not.toHaveBeenCalled()
        })
    })

    it("shows validation error for short password", async () => {
        const user = userEvent.setup()
        render(<RegisterForm onSubmit={mockOnSubmit} />)

        await user.type(screen.getByLabelText("Email"), "test@mail.com")
        await user.type(screen.getByLabelText("Contraseña"), "123")
        await user.type(screen.getByLabelText("Confirmar Contraseña"), "123")
        await user.click(screen.getByRole("button", { name: "Crear cuenta" }))

        await waitFor(() => {
            expect(
                screen.getByText(
                    "La contrasena debe tener al menos 6 caracteres"
                )
            ).toBeInTheDocument()
        })
        expect(mockOnSubmit).not.toHaveBeenCalled()
    })

    it("shows validation error when passwords do not match", async () => {
        const user = userEvent.setup()
        render(<RegisterForm onSubmit={mockOnSubmit} />)

        await user.type(screen.getByLabelText("Email"), "test@mail.com")
        await user.type(screen.getByLabelText("Contraseña"), "123456")
        await user.type(
            screen.getByLabelText("Confirmar Contraseña"),
            "654321"
        )
        await user.click(screen.getByRole("button", { name: "Crear cuenta" }))

        await waitFor(() => {
            expect(
                screen.getByText("Las contrasenas no coinciden")
            ).toBeInTheDocument()
        })
        expect(mockOnSubmit).not.toHaveBeenCalled()
    })

    it("calls onSubmit with valid form data", async () => {
        const user = userEvent.setup()
        render(<RegisterForm onSubmit={mockOnSubmit} />)

        await user.type(screen.getByLabelText("Email"), "test@mail.com")
        await user.type(screen.getByLabelText("Contraseña"), "123456")
        await user.type(
            screen.getByLabelText("Confirmar Contraseña"),
            "123456"
        )
        await user.click(screen.getByRole("button", { name: "Crear cuenta" }))

        await waitFor(() => {
            expect(mockOnSubmit).toHaveBeenCalledWith(
                {
                    email: "test@mail.com",
                    password: "123456",
                    confirmPassword: "123456",
                },
                expect.anything()
            )
        })
    })
})
