export interface User {
  id: string
  email: string
  nombreCompleto?: string
  roles?: string[]
}

export interface LoginCredentials {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  password: string
  confirmPassword: string
}

export interface RegisterResponse {
  userId: string
  email: string
  token: string
}

export interface AuthResponse {
  token: string
  user: User
}

export interface JwtPayload {
  sub: string
  email: string
  name?: string
  roles: string[]
  exp: number
  iat: number
}

export interface CreateArtistaRequest {
  nombreArtistico: string
  descripcion?: string
  pais?: string
  ciudad?: string
  imagenUrl?: string
}
