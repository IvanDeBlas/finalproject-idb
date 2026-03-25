import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import LoginPage from "../page"

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
        login: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

// Import after mocks
const { authService } = await import("@/services/auth.service")
const { toast } = await import("sonner")

describe("LoginPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders the login form with branding", () => {
        render(<LoginPage />)

        expect(screen.getByText("WePlay Rises")).toBeInTheDocument()
        expect(screen.getByText("Panel de Artista")).toBeInTheDocument()
        expect(
            screen.getByText("Ingresa tus credenciales para acceder")
        ).toBeInTheDocument()
    })

    it("renders email and password fields", () => {
        render(<LoginPage />)

        expect(screen.getByLabelText("Email")).toBeInTheDocument()
        expect(screen.getByLabelText("Contrasena")).toBeInTheDocument()
    })

    it("renders submit button", () => {
        render(<LoginPage />)

        expect(
            screen.getByRole("button", { name: "Iniciar sesion" })
        ).toBeInTheDocument()
    })

    it("renders link to register page", () => {
        render(<LoginPage />)

        const registerLink = screen.getByRole("link", { name: "Registrate" })
        expect(registerLink).toBeInTheDocument()
        expect(registerLink).toHaveAttribute("href", "/register")
    })

    it("shows validation error for empty email on submit", async () => {
        const user = userEvent.setup()
        render(<LoginPage />)

        await user.click(
            screen.getByRole("button", { name: "Iniciar sesion" })
        )

        await waitFor(() => {
            expect(
                screen.getByText("El email es obligatorio")
            ).toBeInTheDocument()
        })
    })

    it("shows validation error for short password", async () => {
        const user = userEvent.setup()
        render(<LoginPage />)

        await user.type(screen.getByLabelText("Email"), "test@mail.com")
        await user.type(screen.getByLabelText("Contrasena"), "12")
        await user.click(
            screen.getByRole("button", { name: "Iniciar sesion" })
        )

        await waitFor(() => {
            expect(
                screen.getByText(
                    "La contrasena debe tener al menos 6 caracteres"
                )
            ).toBeInTheDocument()
        })
    })

    it("calls authService.login and navigates to dashboard on success", async () => {
        const mockResponse = {
            user: { id: "1", email: "test@mail.com", roles: ["Fan"] },
            token: "jwt-token",
        }
        vi.mocked(authService.login).mockResolvedValue(mockResponse)

        const user = userEvent.setup()
        render(<LoginPage />)

        await user.type(screen.getByLabelText("Email"), "test@mail.com")
        await user.type(screen.getByLabelText("Contrasena"), "123456")
        await user.click(
            screen.getByRole("button", { name: "Iniciar sesion" })
        )

        await waitFor(() => {
            expect(authService.login).toHaveBeenCalledWith({
                email: "test@mail.com",
                password: "123456",
            })
        })

        await waitFor(() => {
            expect(mockLogin).toHaveBeenCalledWith(
                mockResponse.user,
                mockResponse.token
            )
        })

        expect(toast.success).toHaveBeenCalledWith("Bienvenido!")
        expect(mockPush).toHaveBeenCalledWith("/dashboard")
    })

    it("shows error toast on login failure", async () => {
        vi.mocked(authService.login).mockRejectedValue(
            new Error("Invalid credentials")
        )

        const user = userEvent.setup()
        render(<LoginPage />)

        await user.type(screen.getByLabelText("Email"), "test@mail.com")
        await user.type(screen.getByLabelText("Contrasena"), "123456")
        await user.click(
            screen.getByRole("button", { name: "Iniciar sesion" })
        )

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalledWith(
                "Credenciales invalidas"
            )
        })

        expect(mockPush).not.toHaveBeenCalled()
    })
})
