import { describe, it, expect, vi, beforeEach } from "vitest"
import { authService } from "../auth.service"
import { apiFetch } from "@/lib/api-client"
import type { ServiceResponse, User } from "@shared/types"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
    apiClient: {
        interceptors: {
            request: { use: vi.fn() },
            response: { use: vi.fn() },
        },
    },
}))

const mockUser: User = {
    id: "user-1",
    email: "test@mail.com",
    nombreCompleto: "Test User",
    roles: ["Fan"],
}

describe("authService.login", () => {
    beforeEach(() => {
        vi.clearAllMocks()
        localStorage.clear()
    })

    it("returns user and token on successful login", async () => {
        const loginResponse: ServiceResponse<{ token: string }> = {
            data: { token: "jwt-token-123" },
            messages: [],
        }
        const meResponse: ServiceResponse<User> = {
            data: mockUser,
            messages: [],
        }

        vi.mocked(apiFetch)
            .mockResolvedValueOnce(loginResponse)
            .mockResolvedValueOnce(meResponse)

        const result = await authService.login({
            email: "test@mail.com",
            password: "123456",
        })

        expect(result.token).toBe("jwt-token-123")
        expect(result.user).toEqual(mockUser)
    })

    it("calls POST /auth/login with credentials", async () => {
        const loginResponse: ServiceResponse<{ token: string }> = {
            data: { token: "jwt-token" },
            messages: [],
        }
        const meResponse: ServiceResponse<User> = {
            data: mockUser,
            messages: [],
        }

        vi.mocked(apiFetch)
            .mockResolvedValueOnce(loginResponse)
            .mockResolvedValueOnce(meResponse)

        await authService.login({
            email: "test@mail.com",
            password: "123456",
        })

        expect(apiFetch).toHaveBeenCalledWith("/auth/login", {
            method: "POST",
            data: { email: "test@mail.com", password: "123456" },
        })
    })

    it("stores token in localStorage after login", async () => {
        const loginResponse: ServiceResponse<{ token: string }> = {
            data: { token: "stored-token" },
            messages: [],
        }
        const meResponse: ServiceResponse<User> = {
            data: mockUser,
            messages: [],
        }

        vi.mocked(apiFetch)
            .mockResolvedValueOnce(loginResponse)
            .mockResolvedValueOnce(meResponse)

        await authService.login({
            email: "test@mail.com",
            password: "123456",
        })

        expect(localStorage.getItem("token")).toBe("stored-token")
    })

    it("throws error when getCurrentUser returns null after login", async () => {
        const loginResponse: ServiceResponse<{ token: string }> = {
            data: { token: "jwt-token" },
            messages: [],
        }

        vi.mocked(apiFetch)
            .mockResolvedValueOnce(loginResponse)
            .mockRejectedValueOnce(new Error("Unauthorized"))

        await expect(
            authService.login({ email: "test@mail.com", password: "123456" })
        ).rejects.toThrow("Failed to get user after login")
    })

    it("propagates API error on login failure", async () => {
        vi.mocked(apiFetch).mockRejectedValue(new Error("Invalid credentials"))

        await expect(
            authService.login({ email: "bad@mail.com", password: "wrong" })
        ).rejects.toThrow("Invalid credentials")
    })
})

describe("authService.register", () => {
    beforeEach(() => {
        vi.clearAllMocks()
        localStorage.clear()
    })

    it("returns user and token on successful registration", async () => {
        const registerResponse: ServiceResponse<{ token: string }> = {
            data: { token: "new-token" },
            messages: [],
        }
        const meResponse: ServiceResponse<User> = {
            data: mockUser,
            messages: [],
        }

        vi.mocked(apiFetch)
            .mockResolvedValueOnce(registerResponse)
            .mockResolvedValueOnce(meResponse)

        const result = await authService.register({
            email: "new@mail.com",
            password: "123456",
            confirmPassword: "123456",
        })

        expect(result.token).toBe("new-token")
        expect(result.user).toEqual(mockUser)
    })

    it("calls POST /auth/register with data", async () => {
        const registerResponse: ServiceResponse<{ token: string }> = {
            data: { token: "token" },
            messages: [],
        }
        const meResponse: ServiceResponse<User> = {
            data: mockUser,
            messages: [],
        }

        vi.mocked(apiFetch)
            .mockResolvedValueOnce(registerResponse)
            .mockResolvedValueOnce(meResponse)

        const registerData = {
            email: "new@mail.com",
            password: "123456",
            confirmPassword: "123456",
        }

        await authService.register(registerData)

        expect(apiFetch).toHaveBeenCalledWith("/auth/register", {
            method: "POST",
            data: registerData,
        })
    })

    it("stores token in localStorage after register", async () => {
        const registerResponse: ServiceResponse<{ token: string }> = {
            data: { token: "register-token" },
            messages: [],
        }
        const meResponse: ServiceResponse<User> = {
            data: mockUser,
            messages: [],
        }

        vi.mocked(apiFetch)
            .mockResolvedValueOnce(registerResponse)
            .mockResolvedValueOnce(meResponse)

        await authService.register({
            email: "new@mail.com",
            password: "123456",
            confirmPassword: "123456",
        })

        expect(localStorage.getItem("token")).toBe("register-token")
    })

    it("throws error when getCurrentUser returns null after register", async () => {
        const registerResponse: ServiceResponse<{ token: string }> = {
            data: { token: "token" },
            messages: [],
        }

        vi.mocked(apiFetch)
            .mockResolvedValueOnce(registerResponse)
            .mockRejectedValueOnce(new Error("Unauthorized"))

        await expect(
            authService.register({
                email: "new@mail.com",
                password: "123456",
                confirmPassword: "123456",
            })
        ).rejects.toThrow("Failed to get user after register")
    })
})

describe("authService.getCurrentUser", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns user when authenticated", async () => {
        const meResponse: ServiceResponse<User> = {
            data: mockUser,
            messages: [],
        }
        vi.mocked(apiFetch).mockResolvedValue(meResponse)

        const result = await authService.getCurrentUser()

        expect(result).toEqual(mockUser)
        expect(apiFetch).toHaveBeenCalledWith("/auth/me")
    })

    it("returns null when API call fails", async () => {
        vi.mocked(apiFetch).mockRejectedValue(new Error("Unauthorized"))

        const result = await authService.getCurrentUser()

        expect(result).toBeNull()
    })
})
