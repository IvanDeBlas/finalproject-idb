// Auth domain types

export interface User {
  id: string
  email: string
  nombreCompleto: string
}

export interface LoginCredentials {
  email: string
  password: string
}

export interface RegisterData {
  email: string
  password: string
  confirmPassword: string
  nombreCompleto: string
}

export interface AuthResponse {
  user: User
  token: string
}

export interface IAuthRepository {
  login(credentials: LoginCredentials): Promise<AuthResponse>
  register(data: RegisterData): Promise<AuthResponse>
  getCurrentUser(): Promise<User | null>
}
