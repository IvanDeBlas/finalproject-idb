import { z } from 'zod';

/**
 * Schema de validacion para query params de GET /api/campanias/mis-campanias
 */
export const misCampaniasQuerySchema = z.object({
    estadoCampaniaId: z
        .number()
        .int()
        .min(1, 'Estado invalido')
        .max(5, 'Estado invalido')
        .optional(),
    page: z
        .number()
        .int()
        .min(1, 'La pagina debe ser mayor o igual a 1')
        .default(1),
    pageSize: z
        .number()
        .int()
        .min(1, 'El tamano de pagina debe ser mayor o igual a 1')
        .max(100, 'El tamano de pagina no puede superar 100')
        .default(10),
});

export type MisCampaniasQueryParams = z.infer<typeof misCampaniasQuerySchema>;

/**
 * Schema de validacion para query params de GET /api/campanias/{id}/backings
 */
export const backingsQuerySchema = z.object({
    page: z
        .number()
        .int()
        .min(1, 'La pagina debe ser mayor o igual a 1')
        .default(1),
    pageSize: z
        .number()
        .int()
        .min(1, 'El tamano de pagina debe ser mayor o igual a 1')
        .max(100, 'El tamano de pagina no puede superar 100')
        .default(20),
});

export type BackingsQueryParams = z.infer<typeof backingsQuerySchema>;
