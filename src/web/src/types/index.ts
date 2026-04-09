// Global types - re-export domain types for convenience
export type { Campania, CampaniaEstado, Reward } from "@/features/campanias"
export type { User, LoginCredentials, RegisterData, AuthResponse } from "@/features/auth"
export type { Artista } from "@/features/artistas"
export type { Backing } from "@/features/backings"

// API Response wrapper (matches backend ServiceResponse<T>)
export interface ServiceResponse<T> {
  data: T
  messages: ServiceResponseMessage[]
}

export interface ServiceResponseMessage {
  message: string
  errorCode: string
}
