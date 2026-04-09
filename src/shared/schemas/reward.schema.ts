import { z } from "zod";

/**
 * Schema Zod para crear una recompensa
 * Alineado con CreateRewardCommandValidator del backend
 */
export const createRewardSchema = z.object({
    campaniaId: z
        .string()
        .uuid("ID de campania invalido"),
    tipoRewardId: z
        .number()
        .int()
        .positive("Debe seleccionar un tipo de recompensa"),
    nombre: z
        .string()
        .min(1, "El nombre es obligatorio")
        .max(200, "El nombre no puede superar los 200 caracteres"),
    descripcion: z
        .string()
        .max(2000, "La descripcion no puede superar los 2000 caracteres")
        .optional()
        .or(z.literal("")),
    importeMinimo: z
        .number({ invalid_type_error: "Debe ser un numero" })
        .positive("El importe debe ser mayor a 0")
        .min(1, "El importe minimo es 1 EUR"),
    monedaId: z
        .number()
        .int()
        .positive("La moneda es obligatoria")
        .default(1), // EUR por defecto
    esAddOn: z.boolean().default(false),
    cantidadMaxima: z
        .number()
        .int()
        .positive("La cantidad maxima debe ser mayor a 0")
        .optional()
        .nullable(),
    cantidadPorBacker: z
        .number()
        .int()
        .positive("La cantidad por backer debe ser mayor a 0")
        .optional()
        .nullable(),
    incluyeEnvioFisico: z.boolean().default(false),
    tiempoEntregaEstimado: z
        .string()
        .max(200, "El tiempo de entrega no puede superar los 200 caracteres")
        .optional()
        .or(z.literal("")),
    orden: z
        .number()
        .int()
        .min(0, "El orden debe ser mayor o igual a 0")
        .default(0),
});

export type CreateRewardFormData = z.infer<typeof createRewardSchema>;

/**
 * Schema Zod para actualizar una recompensa
 * Alineado con UpdateRewardCommandValidator del backend
 * Todos los campos son opcionales excepto id
 */
export const updateRewardSchema = z.object({
    id: z.string().uuid("ID invalido"),
    nombre: z
        .string()
        .min(1, "El nombre no puede estar vacio")
        .max(200, "El nombre no puede superar los 200 caracteres")
        .optional(),
    descripcion: z
        .string()
        .max(2000, "La descripcion no puede superar los 2000 caracteres")
        .optional(),
    importeMinimo: z
        .number()
        .positive("El importe debe ser mayor a 0")
        .optional(),
    tipoRewardId: z
        .number()
        .int()
        .positive()
        .optional(),
    monedaId: z
        .number()
        .int()
        .positive()
        .optional(),
    esAddOn: z.boolean().optional(),
    cantidadMaxima: z
        .number()
        .int()
        .positive()
        .optional()
        .nullable(),
    cantidadPorBacker: z
        .number()
        .int()
        .positive()
        .optional()
        .nullable(),
    incluyeEnvioFisico: z.boolean().optional(),
    tiempoEntregaEstimado: z
        .string()
        .max(200)
        .optional(),
    orden: z
        .number()
        .int()
        .min(0)
        .optional(),
    esActivo: z.boolean().optional(),
});

export type UpdateRewardFormData = z.infer<typeof updateRewardSchema>;

/**
 * Schema Zod para reordenar recompensas
 * Alineado con ReorderRewardsCommandValidator del backend
 */
export const reorderRewardsSchema = z.object({
    campaniaId: z.string().uuid("ID de campania invalido"),
    rewardOrders: z
        .array(
            z.object({
                rewardId: z.string().uuid("ID de reward invalido"),
                orden: z.number().int().min(0, "El orden debe ser mayor o igual a 0"),
            })
        )
        .min(1, "Debe proporcionar al menos una recompensa"),
});

export type ReorderRewardsFormData = z.infer<typeof reorderRewardsSchema>;
