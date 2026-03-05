import { z } from "zod"

export const loginSchema = z.object({
  email: z.string().min(1, "El email es obligatorio").email("Formato de email invalido"),
  password: z.string().min(1, "La contrasena es obligatoria").min(6, "La contrasena debe tener al menos 6 caracteres"),
})

export const registerSchema = z.object({
  email: z
    .string()
    .min(1, "El email es obligatorio")
    .email("Formato de email invalido"),
  password: z
    .string()
    .min(1, "La contrasena es obligatoria")
    .min(6, "La contrasena debe tener al menos 6 caracteres"),
  confirmPassword: z
    .string()
    .min(1, "Confirme su contrasena"),
}).refine((data) => data.password === data.confirmPassword, {
  message: "Las contrasenas no coinciden",
  path: ["confirmPassword"],
})

export type LoginFormData = z.infer<typeof loginSchema>
export type RegisterFormData = z.infer<typeof registerSchema>
