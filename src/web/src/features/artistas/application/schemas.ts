import { z } from "zod"

export const artistaSchema = z.object({
  nombreArtistico: z
    .string()
    .min(2, "El nombre artistico es obligatorio")
    .max(100, "Maximo 100 caracteres"),
  descripcion: z.string().max(1000).optional(),
  imagenUrl: z.string().url("URL invalida").optional().or(z.literal("")),
  generoMusical: z.string().max(50).optional(),
})

export type ArtistaFormData = z.infer<typeof artistaSchema>
