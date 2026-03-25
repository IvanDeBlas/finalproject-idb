import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import React from "react"
import { useLogin, useRegister, useLogout } from "../application/useAuth"
import { authService } from "../infrastructure/auth.service"
import type { AuthResponse } from "../domain/types"

// Mock auth service
vi.mock("../infrastructure/auth.service", () => ({
    authService: {
        login: vi.fn(),
        register: vi.fn(),
        getCurrentUser: vi.fn(),
    },
}))

// Mock auth store
const mockStoreLogin = vi.fn()
const mockStoreLogout = vi.fn()
vi.mock("@/store/auth-store", () => ({
    useAuthStore: () => ({
        login: mockStoreLogin,
        logout: mockStoreLogout,
    }),
}))

// Mock navigate
const mockNavigate = vi.fn()
vi.mock("react-router-dom", async () => {
    const actual = await vi.importActual("react-router-dom")
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    }
})

const mockAuthResponse: AuthResponse = {
    user: {
        id: "user-123",
        email: "test@example.com",
        nombreCompleto: "Test User",
    },
    token: "mock-jwt-token",
}

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
        },
    })

    return function Wrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(
            QueryClientProvider,
            { client: queryClient },
            React.createElement(MemoryRouter, null, children)
        )
    }
}

describe("useLogin", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls authService.login with credentials", async () => {
        vi.mocked(authService.login).mockResolvedValue(mockAuthResponse)

        const { result } = renderHook(() => useLogin(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            email: "test@example.com",
            password: "123456",
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(authService.login).toHaveBeenCalledWith({
            email: "test@example.com",
            password: "123456",
        })
    })

    it("stores user and token on success", async () => {
        vi.mocked(authService.login).mockResolvedValue(mockAuthResponse)

        const { result } = renderHook(() => useLogin(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            email: "test@example.com",
            password: "123456",
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(mockStoreLogin).toHaveBeenCalledWith(
            mockAuthResponse.user,
            mockAuthResponse.token
        )
    })

    it("navigates to dashboard on success", async () => {
        vi.mocked(authService.login).mockResolvedValue(mockAuthResponse)

        const { result } = renderHook(() => useLogin(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            email: "test@example.com",
            password: "123456",
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(mockNavigate).toHaveBeenCalledWith("/dashboard")
    })

    it("handles login error", async () => {
        vi.mocked(authService.login).mockRejectedValue(
            new Error("Invalid credentials")
        )

        const { result } = renderHook(() => useLogin(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            email: "test@example.com",
            password: "wrong",
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeDefined()
        expect(mockStoreLogin).not.toHaveBeenCalled()
        expect(mockNavigate).not.toHaveBeenCalled()
    })
})

describe("useRegister", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls authService.register with data", async () => {
        vi.mocked(authService.register).mockResolvedValue(mockAuthResponse)

        const { result } = renderHook(() => useRegister(), {
            wrapper: createWrapper(),
        })

        const registerData = {
            email: "new@example.com",
            password: "123456",
            confirmPassword: "123456",
            nombreCompleto: "New User",
        }

        result.current.mutate(registerData)

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(authService.register).toHaveBeenCalledWith(registerData)
    })

    it("stores user and token on success", async () => {
        vi.mocked(authService.register).mockResolvedValue(mockAuthResponse)

        const { result } = renderHook(() => useRegister(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            email: "new@example.com",
            password: "123456",
            confirmPassword: "123456",
            nombreCompleto: "New User",
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(mockStoreLogin).toHaveBeenCalledWith(
            mockAuthResponse.user,
            mockAuthResponse.token
        )
    })

    it("navigates to dashboard on success", async () => {
        vi.mocked(authService.register).mockResolvedValue(mockAuthResponse)

        const { result } = renderHook(() => useRegister(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            email: "new@example.com",
            password: "123456",
            confirmPassword: "123456",
            nombreCompleto: "New User",
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(mockNavigate).toHaveBeenCalledWith("/dashboard")
    })

    it("handles registration error", async () => {
        vi.mocked(authService.register).mockRejectedValue(
            new Error("Email already exists")
        )

        const { result } = renderHook(() => useRegister(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            email: "existing@example.com",
            password: "123456",
            confirmPassword: "123456",
            nombreCompleto: "Existing User",
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeDefined()
        expect(mockStoreLogin).not.toHaveBeenCalled()
        expect(mockNavigate).not.toHaveBeenCalled()
    })
})

describe("useLogout", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls store logout and navigates to home", () => {
        const { result } = renderHook(() => useLogout(), {
            wrapper: createWrapper(),
        })

        // useLogout returns a function, call it
        result.current()

        expect(mockStoreLogout).toHaveBeenCalled()
        expect(mockNavigate).toHaveBeenCalledWith("/")
    })
})
