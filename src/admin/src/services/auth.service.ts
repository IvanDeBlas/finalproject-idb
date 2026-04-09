import { apiFetch } from "@/lib/api-client"
import type { AuthResponse, LoginCredentials, RegisterRequest, User, ServiceResponse } from "@shared/types"

class AuthService {
  async login(credentials: LoginCredentials): Promise<AuthResponse> {
    const response = await apiFetch<ServiceResponse<{ token: string }>>("/auth/login", {
      method: "POST",
      data: credentials,
    })

    // Store token before fetching user so the interceptor can attach it
    if (typeof window !== "undefined" && response.data.token) {
      localStorage.setItem("token", response.data.token)
    }

    // Get user info after login
    const user = await this.getCurrentUser()
    if (!user) throw new Error("Failed to get user after login")

    return { user, token: response.data.token }
  }

  async register(data: RegisterRequest): Promise<AuthResponse> {
    const response = await apiFetch<ServiceResponse<{ token: string }>>("/auth/register", {
      method: "POST",
      data,
    })

    // Store token in localStorage
    if (typeof window !== "undefined" && response.data.token) {
      localStorage.setItem("token", response.data.token)
    }

    const user = await this.getCurrentUser()
    if (!user) throw new Error("Failed to get user after register")

    return { user, token: response.data.token }
  }

  async getCurrentUser(): Promise<User | null> {
    try {
      const response = await apiFetch<ServiceResponse<User>>("/auth/me")
      return response.data
    } catch {
      return null
    }
  }
}

export const authService = new AuthService()
