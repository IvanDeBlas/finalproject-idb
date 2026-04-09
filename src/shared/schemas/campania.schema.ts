import { z } from "zod";

// ========== STEP 1: Informacion Basica ==========
export const campaniaBasicInfoSchema = z.object({
    titulo: z
        .string()
        .min(1, "El titulo es obligatorio")
        .max(200, "El titulo no puede superar los 200 caracteres"),
    subtitulo: z
        .string()
        .max(300, "El subtitulo no puede superar los 300 caracteres")
        .optional()
        .or(z.literal("")),
    descripcionCorta: z
        .string()
        .max(500, "La descripcion corta no puede superar los 500 caracteres")
        .optional()
        .or(z.literal("")),
});

// ========== STEP 2: Meta Financiera ==========
export const campaniaFundingSchema = z
    .object({
        importeObjetivo: z
            .number({ invalid_type_error: "Debe ser un numero" })
            .positive("El importe objetivo debe ser mayor a 0")
            .min(100, "El importe minimo es 100 EUR"),
        importeMinimo: z
            .number()
            .positive("El importe minimo debe ser mayor a 0")
            .optional(),
        monedaId: z.number().default(1),
        tipoFinanciacionId: z
            .number()
            .positive("Debe seleccionar un tipo de financiacion"),
        permiteAportacionesAnonimas: z.boolean().default(false),
        permitePropinas: z.boolean().default(false),
    })
    .refine(
        (data) => {
            if (data.importeMinimo) {
                return data.importeMinimo <= data.importeObjetivo;
            }
            return true;
        },
        {
            message: "El importe minimo no puede ser mayor al objetivo",
            path: ["importeMinimo"],
        }
    );

// ========== STEP 3: Duracion ==========
export const campaniaDurationSchema = z
    .object({
        fechaInicio: z
            .string()
            .datetime("Formato de fecha invalido")
            .optional(),
        fechaFin: z
            .string()
            .datetime("Formato de fecha invalido")
            .refine(
                (fecha) => {
                    if (!fecha) return true;
                    const minDate = new Date();
                    minDate.setDate(minDate.getDate() + 7);
                    return new Date(fecha) >= minDate;
                },
                {
                    message: "La campania debe durar minimo 7 dias",
                }
            )
            .optional(),
    })
    .refine(
        (data) => {
            if (data.fechaInicio && data.fechaFin) {
                return new Date(data.fechaFin) > new Date(data.fechaInicio);
            }
            return true;
        },
        {
            message: "La fecha de fin debe ser posterior a la fecha de inicio",
            path: ["fechaFin"],
        }
    );

// ========== STEP 4: Multimedia ==========
export const campaniaMediaSchema = z.object({
    imagenPrincipalUrl: z
        .string()
        .url("Debe ser una URL valida")
        .optional()
        .or(z.literal("")),
    videoPrincipalUrl: z
        .string()
        .url("Debe ser una URL valida")
        .optional()
        .or(z.literal("")),
    proyectoArtisticoId: z.string().uuid().optional(),
});

// ========== Schema Completo para Crear Campania ==========
export const createCampaniaSchema = campaniaBasicInfoSchema
    .merge(
        z.object({
            importeObjetivo: z
                .number({ invalid_type_error: "Debe ser un numero" })
                .positive("El importe objetivo debe ser mayor a 0")
                .min(100, "El importe minimo es 100 EUR"),
            importeMinimo: z
                .number()
                .positive("El importe minimo debe ser mayor a 0")
                .optional(),
            monedaId: z.number().default(1),
            tipoFinanciacionId: z
                .number()
                .positive("Debe seleccionar un tipo de financiacion"),
            permiteAportacionesAnonimas: z.boolean().default(false),
            permitePropinas: z.boolean().default(false),
            fechaInicio: z
                .string()
                .datetime("Formato de fecha invalido")
                .optional(),
            fechaFin: z
                .string()
                .datetime("Formato de fecha invalido")
                .optional(),
        })
    )
    .merge(campaniaMediaSchema);

export type CreateCampaniaFormData = z.infer<typeof createCampaniaSchema>;

// ========== Schema para Actualizar Campania ==========
export const updateCampaniaSchema = z
    .object({
        id: z.string().uuid("ID invalido"),
        titulo: z
            .string()
            .max(200, "El titulo no puede superar los 200 caracteres")
            .optional(),
        subtitulo: z
            .string()
            .max(300, "El subtitulo no puede superar los 300 caracteres")
            .optional(),
        descripcionCorta: z
            .string()
            .max(500, "La descripcion corta no puede superar los 500 caracteres")
            .optional(),
        videoPrincipalUrl: z.string().url("URL invalida").optional(),
        imagenPrincipalUrl: z.string().url("URL invalida").optional(),
        importeObjetivo: z.number().positive().optional(),
        importeMinimo: z.number().positive().optional(),
        tipoFinanciacionId: z.number().positive().optional(),
        permiteAportacionesAnonimas: z.boolean().optional(),
        permitePropinas: z.boolean().optional(),
        fechaInicio: z.string().datetime().optional(),
        fechaFin: z.string().datetime().optional(),
    })
    .refine(
        (data) => {
            if (data.importeMinimo && data.importeObjetivo) {
                return data.importeMinimo <= data.importeObjetivo;
            }
            return true;
        },
        {
            message: "El importe minimo no puede ser mayor al objetivo",
            path: ["importeMinimo"],
        }
    )
    .refine(
        (data) => {
            if (data.fechaInicio && data.fechaFin) {
                return new Date(data.fechaFin) > new Date(data.fechaInicio);
            }
            return true;
        },
        {
            message: "La fecha de fin debe ser posterior a la fecha de inicio",
            path: ["fechaFin"],
        }
    );

export type UpdateCampaniaFormData = z.infer<typeof updateCampaniaSchema>;

// ========== Schema para Validar Publicacion ==========
export const publishCampaniaSchema = z.object({
    titulo: z.string().min(1, "El titulo es obligatorio para publicar"),
    importeObjetivo: z.number().positive("El importe objetivo es obligatorio"),
    monedaId: z.number().positive(),
    tipoFinanciacionId: z
        .number()
        .positive("El tipo de financiacion es obligatorio"),
    fechaFin: z
        .string()
        .datetime()
        .refine(
            (fecha) => {
                const minDate = new Date();
                minDate.setDate(minDate.getDate() + 7);
                return new Date(fecha) >= minDate;
            },
            {
                message: "La fecha de fin debe ser minimo 7 dias desde hoy",
            }
        ),
});

export type PublishCampaniaValidation = z.infer<typeof publishCampaniaSchema>;

// ========== Legacy Schemas (mantener por compatibilidad) ==========
/** @deprecated Use createCampaniaSchema instead */
export const createCampaniaSchemaLegacy = z.object({
    titulo: z.string().min(1, "El titulo es obligatorio").max(200),
    descripcion: z.string().min(1, "La descripcion es obligatoria").max(5000),
    importeObjetivo: z.number().min(100).max(1000000),
    fechaFin: z.date().refine((date) => date > new Date(), {
        message: "La fecha de fin debe ser futura",
    }),
    imagenUrl: z.string().url("URL invalida").optional().or(z.literal("")),
    videoUrl: z.string().url("URL invalida").optional().or(z.literal("")),
});

/** @deprecated Use updateCampaniaSchema instead */
export const updateCampaniaSchemaLegacy = createCampaniaSchemaLegacy.partial();
