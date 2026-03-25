import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement, type ReactNode } from "react"
import { useRegister } from "../useRegister"
import type { AuthResponse } from "@shared/types"

vi.mock("@/services/auth.service", () => ({
    authService: {
        register: vi.fn(),
    },
}))

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
    apiClient: {
        interceptors: {
            request: { use: vi.fn() },
            response: { use: vi.fn() },
        },
    },
}))

const { authService } = await import("@/services/auth.service")

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: ReactNode }) {
        return createElement(
            QueryClientProvider,
            { client: queryClient },
            children
        )
    }
}

describe("useRegister", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls authService.register with provided data", async () => {
        const mockResponse: AuthResponse = {
            user: { id: "1", email: "test@mail.com", roles: ["Fan"] },
            token: "jwt-token",
        }
        vi.mocked(authService.register).mockResolvedValue(mockResponse)

        const { result } = renderHook(() => useRegister(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            email: "test@mail.com",
            password: "123456",
            confirmPassword: "123456",
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(authService.register).toHaveBeenCalledWith({
            email: "test@mail.com",
            password: "123456",
            confirmPassword: "123456",
        })
        expect(result.current.data).toEqual(mockResponse)
    })

    it("sets error state on failure", async () => {
        vi.mocked(authService.register).mockRejectedValue(
            new Error("Registration failed")
        )

        const { result } = renderHook(() => useRegister(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            email: "test@mail.com",
            password: "123456",
            confirmPassword: "123456",
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })

        expect(result.current.error?.message).toBe("Registration failed")
    })

    it("starts in idle state", () => {
        const { result } = renderHook(() => useRegister(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isIdle).toBe(true)
        expect(result.current.isPending).toBe(false)
    })
})
