import { useMutation } from "@tanstack/react-query"
import { useNavigate } from "react-router-dom"
import { useAuthStore } from "@/store/auth-store"
import { ROUTES } from "@/lib/constants"
import { authService } from "../infrastructure/auth.service"
import type { LoginCredentials, RegisterData } from "../domain"

export function useLogin() {
  const navigate = useNavigate()
  const { login } = useAuthStore()

  return useMutation({
    mutationFn: (credentials: LoginCredentials) => authService.login(credentials),
    onSuccess: (data) => {
      login(data.user, data.token)
      navigate(ROUTES.DASHBOARD)
    },
  })
}

export function useRegister() {
  const navigate = useNavigate()
  const { login } = useAuthStore()

  return useMutation({
    mutationFn: (data: RegisterData) => authService.register(data),
    onSuccess: (data) => {
      login(data.user, data.token)
      navigate(ROUTES.DASHBOARD)
    },
  })
}

export function useLogout() {
  const navigate = useNavigate()
  const { logout } = useAuthStore()

  return () => {
    logout()
    navigate(ROUTES.HOME)
  }
}
