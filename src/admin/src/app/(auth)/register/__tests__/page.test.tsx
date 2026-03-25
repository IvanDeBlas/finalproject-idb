import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import RegisterPage from "../page"

const mockPush = vi.fn()
const mockLogin = vi.fn()

vi.mock("next/navigation", () => ({
    useRouter: () => ({
        push: mockPush,
        replace: vi.fn(),
        prefetch: vi.fn(),
        back: vi.fn(),
    }),
}))

vi.mock("@/store/auth-store", () => ({
    useAuthStore: () => ({
        login: mockLogin,
    }),
}))

vi.mock("@/services/auth.service", () => ({
    authService: {
        register: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

const { authService } = await import("@/services/auth.service")
const { toast } = await import("sonner")

describe("RegisterPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders the registration form with branding", () => {
        render(<RegisterPage />)

        expect(screen.getByText("WePlay Rises")).toBeInTheDocument()
        expect(screen.getByText("Crear Cuenta")).toBeInTheDocument()
        expect(
            screen.getByText("Registrate como artista para crear campañas")
        ).toBeInTheDocument()
    })

    it("renders link to login page", () => {
        render(<RegisterPage />)

        const loginLink = screen.getByRole("link", {
            name: "Inicia sesión",
        })
        expect(loginLink).toBeInTheDocument()
        expect(loginLink).toHaveAttribute("href", "/login")
    })

    it("calls authService.register and navigates on success", async () => {
        const mockResponse = {
            user: { id: "1", email: "new@mail.com", roles: ["Fan"] },
            token: "new-token",
        }
        vi.mocked(authService.register).mockResolvedValue(mockResponse)

        const user = userEvent.setup()
        render(<RegisterPage />)

        await user.type(screen.getByLabelText("Email"), "new@mail.com")
        await user.type(screen.getByLabelText("Contraseña"), "123456")
        await user.type(
            screen.getByLabelText("Confirmar Contraseña"),
            "123456"
        )
        await user.click(
            screen.getByRole("button", { name: "Crear cuenta" })
        )

        await waitFor(() => {
            expect(authService.register).toHaveBeenCalledWith({
                email: "new@mail.com",
                password: "123456",
                confirmPassword: "123456",
            })
        })

        await waitFor(() => {
            expect(mockLogin).toHaveBeenCalledWith(
                mockResponse.user,
                mockResponse.token
            )
        })

        expect(toast.success).toHaveBeenCalledWith(
            "Cuenta creada exitosamente!"
        )
        expect(mockPush).toHaveBeenCalledWith("/artista/perfil/crear")
    })

    it("shows error toast with specific message on registration failure", async () => {
        vi.mocked(authService.register).mockRejectedValue(
            new Error("Email already exists")
        )

        const user = userEvent.setup()
        render(<RegisterPage />)

        await user.type(screen.getByLabelText("Email"), "existing@mail.com")
        await user.type(screen.getByLabelText("Contraseña"), "123456")
        await user.type(
            screen.getByLabelText("Confirmar Contraseña"),
            "123456"
        )
        await user.click(
            screen.getByRole("button", { name: "Crear cuenta" })
        )

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalledWith("Email already exists")
        })

        expect(mockPush).not.toHaveBeenCalled()
    })

    it("shows generic error message on non-Error failure", async () => {
        vi.mocked(authService.register).mockRejectedValue("unknown error")

        const user = userEvent.setup()
        render(<RegisterPage />)

        await user.type(screen.getByLabelText("Email"), "test@mail.com")
        await user.type(screen.getByLabelText("Contraseña"), "123456")
        await user.type(
            screen.getByLabelText("Confirmar Contraseña"),
            "123456"
        )
        await user.click(
            screen.getByRole("button", { name: "Crear cuenta" })
        )

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalledWith(
                "Error al crear cuenta. Intenta nuevamente."
            )
        })
    })

    it("displays server error in the form area", async () => {
        vi.mocked(authService.register).mockRejectedValue(
            new Error("Server error message")
        )

        const user = userEvent.setup()
        render(<RegisterPage />)

        await user.type(screen.getByLabelText("Email"), "test@mail.com")
        await user.type(screen.getByLabelText("Contraseña"), "123456")
        await user.type(
            screen.getByLabelText("Confirmar Contraseña"),
            "123456"
        )
        await user.click(
            screen.getByRole("button", { name: "Crear cuenta" })
        )

        await waitFor(() => {
            expect(
                screen.getByText("Server error message")
            ).toBeInTheDocument()
        })
    })
})
