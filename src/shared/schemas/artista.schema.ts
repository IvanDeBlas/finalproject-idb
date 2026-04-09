import { z } from "zod"

export const artistaSchema = z.object({
  nombreArtistico: z
    .string()
    .min(2, "El nombre artistico es obligatorio")
    .max(200, "El nombre artistico no puede superar los 200 caracteres"),
  descripcion: z
    .string()
    .max(2000, "La descripcion no puede superar los 2000 caracteres")
    .optional()
    .or(z.literal("")),
  pais: z
    .string()
    .max(100, "El pais no puede superar los 100 caracteres")
    .optional()
    .or(z.literal("")),
  ciudad: z
    .string()
    .max(100, "La ciudad no puede superar los 100 caracteres")
    .optional()
    .or(z.literal("")),
  imagenUrl: z
    .string()
    .url("Debe ser una URL valida")
    .optional()
    .or(z.literal("")),
  generoMusical: z.string().max(50).optional(),
  redesSociales: z
    .object({
      instagram: z.string().optional(),
      twitter: z.string().optional(),
      youtube: z.string().url().optional().or(z.literal("")),
      spotify: z.string().url().optional().or(z.literal("")),
      website: z.string().url().optional().or(z.literal("")),
    })
    .optional(),
})

export const createArtistaSchema = z.object({
  nombreArtistico: z
    .string()
    .min(1, "El nombre artistico es obligatorio")
    .max(200, "El nombre artistico no puede superar los 200 caracteres"),
  descripcion: z
    .string()
    .max(2000, "La descripcion no puede superar los 2000 caracteres")
    .optional()
    .or(z.literal("")),
  pais: z
    .string()
    .max(100, "El pais no puede superar los 100 caracteres")
    .optional()
    .or(z.literal("")),
  ciudad: z
    .string()
    .max(100, "La ciudad no puede superar los 100 caracteres")
    .optional()
    .or(z.literal("")),
  imagenUrl: z
    .string()
    .url("Debe ser una URL valida")
    .optional()
    .or(z.literal("")),
})

export type ArtistaFormData = z.infer<typeof artistaSchema>
export type CreateArtistaFormData = z.infer<typeof createArtistaSchema>
