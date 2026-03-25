import { describe, it, expect, vi, beforeEach } from "vitest"
import { authService } from "../infrastructure/auth.service"

// Mock the api-client
const mockApiFetch = vi.fn()
vi.mock("@/lib/api-client", () => ({
    apiFetch: (...args: unknown[]) => mockApiFetch(...args),
}))

describe("AuthService", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    describe("login", () => {
        it("calls POST /auth/login with credentials", async () => {
            mockApiFetch.mockResolvedValue({
                data: {
                    userId: "user-123",
                    email: "test@example.com",
                    token: "jwt-token",
                    roles: ["Fan"],
                },
                messages: [],
            })

            await authService.login({
                email: "test@example.com",
                password: "123456",
            })

            expect(mockApiFetch).toHaveBeenCalledWith("/auth/login", {
                method: "POST",
                data: { email: "test@example.com", password: "123456" },
            })
        })

        it("maps API response to AuthResponse", async () => {
            mockApiFetch.mockResolvedValue({
                data: {
                    userId: "user-123",
                    email: "test@example.com",
                    token: "jwt-token",
                    roles: ["Fan"],
                },
                messages: [],
            })

            const result = await authService.login({
                email: "test@example.com",
                password: "123456",
            })

            expect(result.user.id).toBe("user-123")
            expect(result.user.email).toBe("test@example.com")
            expect(result.token).toBe("jwt-token")
        })

        it("propagates API errors", async () => {
            mockApiFetch.mockRejectedValue(new Error("Unauthorized"))

            await expect(
                authService.login({
                    email: "test@example.com",
                    password: "wrong",
                })
            ).rejects.toThrow("Unauthorized")
        })
    })

    describe("register", () => {
        it("calls POST /auth/register with data", async () => {
            mockApiFetch.mockResolvedValue({
                data: {
                    userId: "user-456",
                    email: "new@example.com",
                    token: "new-jwt-token",
                    roles: ["Fan"],
                },
                messages: [],
            })

            await authService.register({
                email: "new@example.com",
                password: "123456",
                confirmPassword: "123456",
                nombreCompleto: "New User",
            })

            expect(mockApiFetch).toHaveBeenCalledWith("/auth/register", {
                method: "POST",
                data: {
                    email: "new@example.com",
                    password: "123456",
                    confirmPassword: "123456",
                    nombreCompleto: "New User",
                },
            })
        })

        it("maps API response to AuthResponse with nombreCompleto from input", async () => {
            mockApiFetch.mockResolvedValue({
                data: {
                    userId: "user-456",
                    email: "new@example.com",
                    token: "new-jwt-token",
                    roles: ["Fan"],
                },
                messages: [],
            })

            const result = await authService.register({
                email: "new@example.com",
                password: "123456",
                confirmPassword: "123456",
                nombreCompleto: "New User",
            })

            expect(result.user.id).toBe("user-456")
            expect(result.user.email).toBe("new@example.com")
            expect(result.user.nombreCompleto).toBe("New User")
            expect(result.token).toBe("new-jwt-token")
        })

        it("propagates API errors", async () => {
            mockApiFetch.mockRejectedValue(new Error("Email already in use"))

            await expect(
                authService.register({
                    email: "existing@example.com",
                    password: "123456",
                    confirmPassword: "123456",
                    nombreCompleto: "Test",
                })
            ).rejects.toThrow("Email already in use")
        })
    })

    describe("getCurrentUser", () => {
        it("calls GET /auth/me", async () => {
            mockApiFetch.mockResolvedValue({
                data: {
                    id: "user-123",
                    email: "test@example.com",
                    nombreCompleto: "Test User",
                },
                messages: [],
            })

            const result = await authService.getCurrentUser()

            expect(mockApiFetch).toHaveBeenCalledWith("/auth/me")
            expect(result).toEqual({
                id: "user-123",
                email: "test@example.com",
                nombreCompleto: "Test User",
            })
        })

        it("returns null on error", async () => {
            mockApiFetch.mockRejectedValue(new Error("Unauthorized"))

            const result = await authService.getCurrentUser()

            expect(result).toBeNull()
        })
    })
})
