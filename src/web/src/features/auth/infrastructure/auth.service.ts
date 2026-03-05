import { apiFetch } from "@/lib/api-client"
import type { AuthResponse, IAuthRepository, LoginCredentials, RegisterData, User } from "../domain"

interface ServiceResponse<T> {
  data: T
  messages: Array<{ message: string; errorCode: string }>
}

interface AuthApiResponse {
  userId: string
  email: string
  token: string
  roles: string[]
}

class AuthService implements IAuthRepository {
  async login(credentials: LoginCredentials): Promise<AuthResponse> {
    const response = await apiFetch<ServiceResponse<AuthApiResponse>>("/auth/login", {
      method: "POST",
      data: credentials,
    })

    const user: User = {
      id: response.data.userId,
      email: response.data.email,
      nombreCompleto: response.data.email,
    }

    return { user, token: response.data.token }
  }

  async register(data: RegisterData): Promise<AuthResponse> {
    const response = await apiFetch<ServiceResponse<AuthApiResponse>>("/auth/register", {
      method: "POST",
      data,
    })

    const user: User = {
      id: response.data.userId,
      email: response.data.email,
      nombreCompleto: data.nombreCompleto,
    }

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
