"use client"

import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { registerSchema, type RegisterFormData } from "@shared/schemas"

interface RegisterFormProps {
  onSubmit: (data: RegisterFormData) => Promise<void>
  isSubmitting?: boolean
  serverError?: string
}

export function RegisterForm({ onSubmit, isSubmitting, serverError }: RegisterFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
  })

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div className="space-y-2">
        <Label htmlFor="email" className="text-white">Email</Label>
        <Input
          id="email"
          type="email"
          placeholder="tu@email.com"
          className="bg-gray-900 border-gray-700 text-white placeholder:text-gray-500"
          {...register("email")}
        />
        {errors.email && (
          <p className="text-sm text-destructive">{errors.email.message}</p>
        )}
      </div>

      <div className="space-y-2">
        <Label htmlFor="password" className="text-white">Contraseña</Label>
        <Input
          id="password"
          type="password"
          placeholder="Mínimo 6 caracteres"
          className="bg-gray-900 border-gray-700 text-white placeholder:text-gray-500"
          {...register("password")}
        />
        {errors.password && (
          <p className="text-sm text-destructive">{errors.password.message}</p>
        )}
      </div>

      <div className="space-y-2">
        <Label htmlFor="confirmPassword" className="text-white">Confirmar Contraseña</Label>
        <Input
          id="confirmPassword"
          type="password"
          placeholder="Repite tu contraseña"
          className="bg-gray-900 border-gray-700 text-white placeholder:text-gray-500"
          {...register("confirmPassword")}
        />
        {errors.confirmPassword && (
          <p className="text-sm text-destructive">{errors.confirmPassword.message}</p>
        )}
      </div>

      {serverError && (
        <div className="p-3 text-sm text-destructive bg-destructive/10 border border-destructive/20 rounded-md">
          {serverError}
        </div>
      )}

      <Button
        type="submit"
        className="w-full bg-primary hover:bg-primary/90"
        disabled={isSubmitting}
      >
        {isSubmitting ? "Registrando..." : "Crear cuenta"}
      </Button>
    </form>
  )
}
