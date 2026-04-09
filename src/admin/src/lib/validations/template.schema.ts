import { z } from "zod"

export const necesidadTemplateSchema = z
    .object({
        fase: z.string().min(1, "La fase es obligatoria"),
        titulo: z.string().min(1, "El titulo es obligatorio").max(200, "Maximo 200 caracteres"),
        descripcion: z.string().optional(),
        rolProfesionalId: z.number().positive("Selecciona un rol profesional"),
        precioMinOrientativo: z.number().nonnegative("Debe ser mayor o igual a 0").optional(),
        precioMaxOrientativo: z.number().nonnegative("Debe ser mayor o igual a 0").optional(),
        prioridad: z.enum(["Alta", "Media", "Baja"], {
            errorMap: () => ({ message: "Selecciona una prioridad" }),
        }),
        orden: z.number().int().nonnegative(),
    })
    .refine(
        (data) => {
            if (
                data.precioMinOrientativo !== undefined &&
                data.precioMaxOrientativo !== undefined
            ) {
                return data.precioMaxOrientativo >= data.precioMinOrientativo
            }
            return true
        },
        {
            message: "El precio maximo debe ser mayor o igual al minimo",
            path: ["precioMaxOrientativo"],
        }
    )

export const createTemplateSchema = z.object({
    nombre: z.string().min(1, "El nombre es obligatorio").max(200, "Maximo 200 caracteres"),
    descripcion: z.string().max(500, "Maximo 500 caracteres").optional(),
    icono: z.string().min(1, "Selecciona un icono"),
    orden: z.number().int().nonnegative("El orden debe ser mayor o igual a 0"),
    activo: z.boolean().default(true),
    necesidades: z
        .array(necesidadTemplateSchema)
        .min(1, "Debe agregar al menos una necesidad"),
})

export type CreateTemplateFormData = z.infer<typeof createTemplateSchema>
export type NecesidadFormData = z.infer<typeof necesidadTemplateSchema>
