import { useMutation } from "@tanstack/react-query"
import { authService } from "@/services/auth.service"
import type { RegisterFormData } from "@shared/schemas"
import type { AuthResponse } from "@shared/types"

export function useRegister() {
  return useMutation<AuthResponse, Error, RegisterFormData>({
    mutationFn: (data: RegisterFormData) => authService.register(data),
  })
}
