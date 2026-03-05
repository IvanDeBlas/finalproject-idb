import { z } from "zod"

export const createCampaniaSchema = z.object({
  titulo: z
    .string()
    .min(1, "El titulo es obligatorio")
    .max(200, "El titulo no puede exceder 200 caracteres"),
  descripcion: z
    .string()
    .min(1, "La descripcion es obligatoria")
    .max(2000, "La descripcion no puede exceder 2000 caracteres"),
  importeObjetivo: z
    .number()
    .min(100, "El importe minimo es 100 EUR")
    .max(1000000, "El importe maximo es 1.000.000 EUR"),
  fechaFin: z
    .date()
    .min(new Date(), "La fecha de fin debe ser futura"),
  imagenUrl: z.string().url().optional(),
})

export const updateCampaniaSchema = createCampaniaSchema.partial()

export type CreateCampaniaFormData = z.infer<typeof createCampaniaSchema>
export type UpdateCampaniaFormData = z.infer<typeof updateCampaniaSchema>
