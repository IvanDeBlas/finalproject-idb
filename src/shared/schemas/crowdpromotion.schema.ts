import { z } from 'zod';

// Helper interno: no exportar
const urlOpcionalSchema = (nombreCampo: string) =>
    z
        .string()
        .url(`La URL de ${nombreCampo} no tiene formato valido`)
        .max(300, 'Maximo 300 caracteres')
        .optional()
        .or(z.literal(''));

// ========== Schema de Creacion ==========

export const createPromotorSchema = z.object({
    nombrePublico: z
        .string({
            required_error: 'El nombre publico es obligatorio',
        })
        .min(1, 'El nombre publico es obligatorio')
        .min(3, 'El nombre debe tener al menos 3 caracteres')
        .max(200, 'Maximo 200 caracteres'),

    tipoPromotorId: z
        .number({
            required_error: 'El tipo de promotor es obligatorio',
            invalid_type_error: 'El tipo de promotor es obligatorio',
        })
        .int()
        .min(1, 'El tipo de promotor es obligatorio'),

    emailContacto: z
        .string()
        .email('El email de contacto no tiene formato valido')
        .max(200, 'Maximo 200 caracteres')
        .optional()
        .or(z.literal('')),

    urlSitioWeb: urlOpcionalSchema('sitio web'),
    urlInstagram: urlOpcionalSchema('Instagram'),
    urlTikTok: urlOpcionalSchema('TikTok'),
    urlYouTube: urlOpcionalSchema('YouTube'),
    urlTwitter: urlOpcionalSchema('Twitter/X'),
});

// ========== Schema de Edicion ==========

export const updatePromotorSchema = z.object({
    nombrePublico: z
        .string({
            required_error: 'El nombre publico es obligatorio',
        })
        .min(1, 'El nombre publico es obligatorio')
        .min(3, 'El nombre debe tener al menos 3 caracteres')
        .max(200, 'Maximo 200 caracteres'),

    emailContacto: z
        .string()
        .email('El email de contacto no tiene formato valido')
        .max(200, 'Maximo 200 caracteres')
        .optional()
        .or(z.literal('')),

    urlSitioWeb: urlOpcionalSchema('sitio web'),
    urlInstagram: urlOpcionalSchema('Instagram'),
    urlTikTok: urlOpcionalSchema('TikTok'),
    urlYouTube: urlOpcionalSchema('YouTube'),
    urlTwitter: urlOpcionalSchema('Twitter/X'),
});

// ========== Types Inferidos ==========

export type CreatePromotorFormData = z.infer<typeof createPromotorSchema>;
export type UpdatePromotorFormData = z.infer<typeof updatePromotorSchema>;

// ========== PromoPrograma - Schemas de Tarea ==========

// Helper interno para URLs con max 500 (programa requiere mas que promotor)
const urlOpcional500Schema = (nombreCampo: string) =>
    z
        .string()
        .url(`La URL de ${nombreCampo} no tiene formato valido`)
        .max(500, 'Maximo 500 caracteres')
        .optional()
        .or(z.literal(''));

export const createPromoTareaSchema = z.object({
    titulo: z
        .string({ required_error: 'El titulo de la tarea es obligatorio' })
        .min(1, 'El titulo de la tarea es obligatorio')
        .min(3, 'Al menos 3 caracteres')
        .max(200, 'Maximo 200 caracteres'),

    descripcion: z.string().max(4000, 'Maximo 4000 caracteres').optional().or(z.literal('')),

    tipoEventoPromoId: z
        .number({
            required_error: 'El tipo de evento es obligatorio',
            invalid_type_error: 'El tipo de evento es obligatorio',
        })
        .int()
        .min(1, 'El tipo de evento es obligatorio'),

    tipoRewardId: z
        .number({
            required_error: 'El tipo de recompensa es obligatorio',
            invalid_type_error: 'El tipo de recompensa es obligatorio',
        })
        .int()
        .min(1, 'El tipo de recompensa es obligatorio'),

    importeRecompensa: z.number().min(0, 'No puede ser negativo').optional(),

    monedaId: z.number().int().min(1).optional(),

    puntosRecompensa: z.number().int().min(0, 'No puede ser negativo').optional(),

    urlInstrucciones: urlOpcional500Schema('instrucciones'),

    esRepetible: z.boolean().default(true),

    maxRepeticiones: z.number().int().min(1, 'Requiere al menos 1 repeticion').optional(),

    fechaInicio: z.string().optional(),

    fechaFin: z.string().optional(),
})
    .refine(
        (data) => !data.esRepetible || data.maxRepeticiones != null,
        { message: 'Las tareas repetibles requieren max repeticiones >= 1', path: ['maxRepeticiones'] }
    )
    .refine(
        (data) => {
            if (data.tipoRewardId === 1 || data.tipoRewardId === 3) {
                return data.importeRecompensa != null;
            }
            return true;
        },
        { message: 'El importe de recompensa es requerido para recompensas monetarias', path: ['importeRecompensa'] }
    )
    .refine(
        (data) => {
            if (data.tipoRewardId === 2 || data.tipoRewardId === 3) {
                return data.puntosRecompensa != null;
            }
            return true;
        },
        { message: 'Los puntos de recompensa son requeridos para recompensas de puntos', path: ['puntosRecompensa'] }
    );

export const updatePromoTareaSchema = z.object({
    titulo: z
        .string({ required_error: 'El titulo de la tarea es obligatorio' })
        .min(1, 'El titulo de la tarea es obligatorio')
        .min(3, 'Al menos 3 caracteres')
        .max(200, 'Maximo 200 caracteres'),

    descripcion: z.string().max(4000, 'Maximo 4000 caracteres').optional().or(z.literal('')),

    tipoEventoPromoId: z
        .number({
            required_error: 'El tipo de evento es obligatorio',
            invalid_type_error: 'El tipo de evento es obligatorio',
        })
        .int()
        .min(1, 'El tipo de evento es obligatorio'),

    tipoRewardId: z
        .number({
            required_error: 'El tipo de recompensa es obligatorio',
            invalid_type_error: 'El tipo de recompensa es obligatorio',
        })
        .int()
        .min(1, 'El tipo de recompensa es obligatorio'),

    importeRecompensa: z.number().min(0, 'No puede ser negativo').optional(),

    monedaId: z.number().int().min(1).optional(),

    puntosRecompensa: z.number().int().min(0, 'No puede ser negativo').optional(),

    urlInstrucciones: urlOpcional500Schema('instrucciones'),

    esRepetible: z.boolean().default(true),

    maxRepeticiones: z.number().int().min(1, 'Requiere al menos 1 repeticion').optional(),

    fechaInicio: z.string().optional(),

    fechaFin: z.string().optional(),

    id: z.string().uuid().optional(),

    esActivo: z.boolean().optional(),
})
    .refine(
        (data) => !data.esRepetible || data.maxRepeticiones != null,
        { message: 'Las tareas repetibles requieren max repeticiones >= 1', path: ['maxRepeticiones'] }
    )
    .refine(
        (data) => {
            if (data.tipoRewardId === 1 || data.tipoRewardId === 3) {
                return data.importeRecompensa != null;
            }
            return true;
        },
        { message: 'El importe de recompensa es requerido para recompensas monetarias', path: ['importeRecompensa'] }
    )
    .refine(
        (data) => {
            if (data.tipoRewardId === 2 || data.tipoRewardId === 3) {
                return data.puntosRecompensa != null;
            }
            return true;
        },
        { message: 'Los puntos de recompensa son requeridos para recompensas de puntos', path: ['puntosRecompensa'] }
    );

// ========== PromoPrograma - Schema Base (sin tareas ni refines) ==========

const promoProgramaBaseFields = {
    titulo: z
        .string({ required_error: 'El titulo es obligatorio' })
        .min(1, 'El titulo es obligatorio')
        .min(5, 'El titulo debe tener al menos 5 caracteres')
        .max(200, 'Maximo 200 caracteres'),

    descripcion: z.string().max(4000, 'Maximo 4000 caracteres').optional().or(z.literal('')),

    tipoPromoId: z
        .number({
            required_error: 'El tipo de programa es obligatorio',
            invalid_type_error: 'El tipo de programa es obligatorio',
        })
        .int()
        .min(1, 'El tipo de programa es obligatorio'),

    campaniaCrowdfundingId: z.string().uuid().optional().or(z.literal('')),

    proyectoArtisticoId: z.string().uuid().optional().or(z.literal('')),

    urlLanding: urlOpcional500Schema('landing'),

    codigoTrackingBase: z
        .string()
        .max(50, 'Maximo 50 caracteres')
        .regex(/^[a-zA-Z0-9-]*$/, 'Solo letras, numeros y guiones')
        .optional()
        .or(z.literal('')),

    monedaId: z
        .number({
            required_error: 'La moneda es obligatoria',
            invalid_type_error: 'La moneda es obligatoria',
        })
        .int()
        .min(1, 'La moneda es obligatoria'),

    importeComisionPorcentaje: z
        .number()
        .min(0, 'No puede ser negativo')
        .max(100, 'No puede superar el 100%')
        .optional(),

    importeComisionFija: z.number().min(0, 'No puede ser negativa').optional(),

    fechaInicio: z.string().optional(),

    fechaFin: z.string().optional(),
};

// Factory que aplica los refines compartidos entre create y update
const applyProgramaRefines = <T extends z.ZodTypeAny>(schema: T) =>
    schema
        .refine(
            (data: Record<string, unknown>) =>
                data.importeComisionPorcentaje != null || data.importeComisionFija != null,
            {
                message: 'Debe definir al menos una comision (porcentaje o fija)',
                path: ['importeComisionPorcentaje'],
            }
        )
        .refine(
            (data: Record<string, unknown>) => {
                if (data.fechaFin && data.fechaInicio) {
                    return (data.fechaFin as string) > (data.fechaInicio as string);
                }
                return true;
            },
            { message: 'La fecha fin debe ser posterior a la fecha inicio', path: ['fechaFin'] }
        );

// ========== PromoPrograma - Schemas Exportados ==========

export const createPromoProgramaSchema = applyProgramaRefines(
    z.object({
        ...promoProgramaBaseFields,
        tareas: z.array(createPromoTareaSchema).optional().default([]),
    })
);

export const updatePromoProgramaSchema = applyProgramaRefines(
    z.object({
        ...promoProgramaBaseFields,
        tareas: z.array(updatePromoTareaSchema).optional().default([]),
    })
);

// ========== PromoPrograma - Types Inferidos ==========

export type CreatePromoProgramaFormData = z.infer<typeof createPromoProgramaSchema>;
export type UpdatePromoProgramaFormData = z.infer<typeof updatePromoProgramaSchema>;

// ========== Inscripcion a Programa - Schemas de Filtros (US-CP-03) ==========

export const explorarProgramasFiltersSchema = z.object({
    artistaNombre: z
        .string()
        .max(200, 'Maximo 200 caracteres')
        .optional(),

    tipoPromoId: z
        .number()
        .int()
        .positive('El tipo de programa debe ser un numero positivo')
        .optional(),

    page: z
        .number()
        .int()
        .min(1, 'La pagina debe ser mayor a 0')
        .optional(),

    pageSize: z
        .number()
        .int()
        .min(1, 'El tamano de pagina debe ser mayor a 0')
        .max(50, 'El tamano de pagina no puede superar 50')
        .optional(),
});

export const inscripcionesFiltersSchema = z.object({
    estado: z
        .enum(['Pendiente', 'Aprobado', 'Bloqueado', 'DadoDeBaja'])
        .optional(),

    page: z
        .number()
        .int()
        .min(1, 'La pagina debe ser mayor a 0')
        .optional(),

    pageSize: z
        .number()
        .int()
        .min(1, 'El tamano de pagina debe ser mayor a 0')
        .max(50, 'El tamano de pagina no puede superar 50')
        .optional(),
});

// ========== Inscripcion - Types Inferidos ==========

export type ExplorarProgramasFiltersData = z.infer<typeof explorarProgramasFiltersSchema>;
export type InscripcionesFiltersData = z.infer<typeof inscripcionesFiltersSchema>;

// ========== Tareas de Promocion - US-CP-04 ==========

export const completarTareaSchema = z.object({
    urlPruebaCompletado: z
        .string()
        .min(1, 'La URL de prueba es obligatoria')
        .max(2048, 'La URL no puede superar los 2048 caracteres')
        .url('La URL de prueba no tiene formato valido'),

    comentarioPromotor: z
        .string()
        .max(500, 'El comentario no puede superar los 500 caracteres')
        .optional(),
});

export const validarTareaSchema = z.object({
    comentarioValidacion: z
        .string()
        .max(500, 'El comentario no puede superar los 500 caracteres')
        .optional(),
});

export const rechazarTareaSchema = z.object({
    comentarioValidacion: z
        .string()
        .min(1, 'El motivo de rechazo es obligatorio')
        .max(500, 'El motivo de rechazo no puede superar los 500 caracteres'),
});

// ========== Tareas de Promocion - Types Inferidos ==========

export type CompletarTareaFormData = z.infer<typeof completarTareaSchema>;
export type ValidarTareaFormData = z.infer<typeof validarTareaSchema>;
export type RechazarTareaFormData = z.infer<typeof rechazarTareaSchema>;

// ========== Tracking y Metricas - US-CP-05 ==========

// Base sin refine para poder extender sin conflictos en Zod v3
const filtroFechasBaseFields = {
    fechaDesde: z.string().optional(),
    fechaHasta: z.string().optional(),
};

const fechasRefinement = (data: { fechaDesde?: string; fechaHasta?: string }) =>
    !data.fechaDesde || !data.fechaHasta || data.fechaDesde <= data.fechaHasta;

const fechasRefinementConfig = {
    message: 'La fecha de inicio no puede ser posterior a la fecha fin',
    path: ['fechaDesde'] as string[],
};

export const filtroFechasSchema = z
    .object(filtroFechasBaseFields)
    .refine(fechasRefinement, fechasRefinementConfig);

export type FiltroFechasFormData = z.infer<typeof filtroFechasSchema>;

export const filtroMetricasPromotorSchema = z
    .object({
        ...filtroFechasBaseFields,
        programaId: z.string().uuid('ID de programa invalido').optional(),
    })
    .refine(fechasRefinement, fechasRefinementConfig);

export type FiltroMetricasPromotorFormData = z.infer<typeof filtroMetricasPromotorSchema>;

// ========== Wallet - US-CP-06 ==========

export const solicitarCobroSchema = z.object({
    importe: z
        .number({
            required_error: 'El importe es obligatorio',
            invalid_type_error: 'El importe debe ser un numero',
        })
        .positive('El importe debe ser mayor que cero'),

    descripcion: z
        .string()
        .max(500, 'La descripcion no puede superar los 500 caracteres')
        .optional(),
});

export type SolicitarCobroFormData = z.infer<typeof solicitarCobroSchema>;

export const walletTransaccionesFiltersSchema = z
    .object({
        esCredito: z.boolean().optional(),

        estadoTransaccionId: z
            .number()
            .int()
            .min(1)
            .max(4, 'Estado invalido')
            .optional(),

        fechaDesde: z.string().optional(),

        fechaHasta: z.string().optional(),

        page: z
            .number()
            .int()
            .min(1, 'La pagina debe ser mayor a 0')
            .optional(),

        pageSize: z
            .number()
            .int()
            .min(1, 'El tamano de pagina debe ser mayor a 0')
            .max(50, 'El tamano de pagina no puede superar 50')
            .optional(),
    })
    .refine(fechasRefinement, fechasRefinementConfig);

export type WalletTransaccionesFiltersFormData = z.infer<typeof walletTransaccionesFiltersSchema>;
