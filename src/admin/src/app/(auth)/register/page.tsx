"use client"

import { useState } from "react"
import Link from "next/link"
import { useRouter } from "next/navigation"
import { toast } from "sonner"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { RegisterForm } from "@/components/auth/RegisterForm"
import { useAuthStore } from "@/store/auth-store"
import { authService } from "@/services/auth.service"
import { Music } from "lucide-react"
import type { RegisterFormData } from "@shared/schemas"

export default function RegisterPage() {
  const router = useRouter()
  const { login } = useAuthStore()
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [serverError, setServerError] = useState<string>()

  const handleSubmit = async (data: RegisterFormData) => {
    setIsSubmitting(true)
    setServerError(undefined)

    try {
      const response = await authService.register(data)
      login(response.user, response.token)
      toast.success("Cuenta creada exitosamente!")
      router.push("/artista/perfil/crear")
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : "Error al crear cuenta. Intenta nuevamente."
      setServerError(errorMessage)
      toast.error(errorMessage)
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <Card className="w-full max-w-md bg-gray-800 border-gray-700">
      <CardHeader className="text-center">
        <div className="flex justify-center mb-4">
          <div className="flex items-center gap-2">
            <Music className="h-8 w-8 text-primary" />
            <span className="text-2xl font-bold text-white">WePlay Rises</span>
          </div>
        </div>
        <CardTitle className="text-white">Crear Cuenta</CardTitle>
        <CardDescription className="text-gray-400">
          Registrate como artista para crear campañas
        </CardDescription>
      </CardHeader>
      <CardContent>
        <RegisterForm
          onSubmit={handleSubmit}
          isSubmitting={isSubmitting}
          serverError={serverError}
        />

        <p className="mt-4 text-center text-sm text-gray-400">
          ¿Ya tienes cuenta?{" "}
          <Link href="/login" className="text-primary hover:underline">
            Inicia sesión
          </Link>
        </p>
      </CardContent>
    </Card>
  )
}
