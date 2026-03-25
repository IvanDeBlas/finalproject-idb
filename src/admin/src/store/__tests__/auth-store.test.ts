import { describe, it, expect, beforeEach } from "vitest"
import { useAuthStore } from "../auth-store"
import { act } from "@testing-library/react"
import type { User } from "@shared/types"

const mockUser: User = {
    id: "user-1",
    email: "test@mail.com",
    nombreCompleto: "Test User",
    roles: ["Fan"],
}

describe("useAuthStore", () => {
    beforeEach(() => {
        localStorage.clear()
        act(() => {
            useAuthStore.setState({
                user: null,
                token: null,
                isAuthenticated: false,
            })
        })
    })

    it("initializes with unauthenticated state", () => {
        const state = useAuthStore.getState()

        expect(state.user).toBeNull()
        expect(state.token).toBeNull()
        expect(state.isAuthenticated).toBe(false)
    })

    it("login sets user, token, and isAuthenticated", () => {
        act(() => {
            useAuthStore.getState().login(mockUser, "jwt-token")
        })

        const state = useAuthStore.getState()
        expect(state.user).toEqual(mockUser)
        expect(state.token).toBe("jwt-token")
        expect(state.isAuthenticated).toBe(true)
    })

    it("login stores token in localStorage", () => {
        act(() => {
            useAuthStore.getState().login(mockUser, "stored-token")
        })

        expect(localStorage.getItem("token")).toBe("stored-token")
    })

    it("logout clears user, token, and isAuthenticated", () => {
        act(() => {
            useAuthStore.getState().login(mockUser, "jwt-token")
        })

        act(() => {
            useAuthStore.getState().logout()
        })

        const state = useAuthStore.getState()
        expect(state.user).toBeNull()
        expect(state.token).toBeNull()
        expect(state.isAuthenticated).toBe(false)
    })

    it("logout removes token from localStorage", () => {
        act(() => {
            useAuthStore.getState().login(mockUser, "jwt-token")
        })

        act(() => {
            useAuthStore.getState().logout()
        })

        expect(localStorage.getItem("token")).toBeNull()
    })

    it("setUser updates only the user without changing token or auth status", () => {
        act(() => {
            useAuthStore.getState().login(mockUser, "jwt-token")
        })

        const updatedUser: User = {
            ...mockUser,
            nombreCompleto: "Updated Name",
        }

        act(() => {
            useAuthStore.getState().setUser(updatedUser)
        })

        const state = useAuthStore.getState()
        expect(state.user?.nombreCompleto).toBe("Updated Name")
        expect(state.token).toBe("jwt-token")
        expect(state.isAuthenticated).toBe(true)
    })
})
