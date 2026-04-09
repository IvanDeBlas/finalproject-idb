import { z } from "zod";

// ========== Schema para Item de Necesidad Seleccionada ==========

export const necesidadSeleccionadaSchema = z
    .object({
        plantillaNecesidadId: z
            .string()
            .min(1, "La necesidad es obligatoria"),
        presupuestoMin: z
            .number()
            .nonnegative("El presupuesto minimo no puede ser negativo")
            .optional(),
        presupuestoMax: z
            .number()
            .nonnegative("El presupuesto maximo no puede ser negativo")
            .optional(),
        monedaId: z
            .number()
            .positive("La moneda es obligatoria"),
    })
    .refine(
        (data) => {
            if (data.presupuestoMin !== undefined && data.presupuestoMax !== undefined) {
                return data.presupuestoMax >= data.presupuestoMin;
            }
            return true;
        },
        {
            message: "El presupuesto maximo debe ser mayor o igual al minimo",
            path: ["presupuestoMax"],
        }
    );

// ========== Schema para Request de Generar Necesidades ==========

export const generarNecesidadesSchema = z.object({
    proyectoArtisticoId: z
        .string()
        .min(1, "El proyecto artistico es obligatorio"),
    necesidadesSeleccionadas: z
        .array(necesidadSeleccionadaSchema)
        .min(1, "Debe seleccionar al menos una necesidad"),
});

// ========== Types Inferidos ==========

export type NecesidadSeleccionadaFormData = z.infer<typeof necesidadSeleccionadaSchema>;
export type GenerarNecesidadesFormData = z.infer<typeof generarNecesidadesSchema>;

// ========== Schemas para Necesidades Crowdsourcing ==========

const necesidadBaseFields = {
    titulo: z
        .string()
        .min(1, "El titulo es obligatorio")
        .min(5, "El titulo debe tener al menos 5 caracteres")
        .max(200, "El titulo no puede superar los 200 caracteres"),
    descripcion: z
        .string()
        .max(4000, "La descripcion no puede superar los 4000 caracteres")
        .optional(),
    modalidadTrabajoId: z
        .number()
        .positive("La modalidad de trabajo es obligatoria"),
    presupuestoMin: z
        .number()
        .nonnegative("El presupuesto no puede ser negativo")
        .optional(),
    presupuestoMax: z
        .number()
        .nonnegative("El presupuesto no puede ser negativo")
        .optional(),
    monedaId: z
        .number()
        .positive("La moneda es obligatoria")
        .optional(),
    ubicacionCiudad: z.string().max(100).optional(),
    ubicacionPais: z.string().max(100).optional(),
    fechaLimitePropuestas: z.string().optional(),
    fechaInicioPrevista: z.string().optional(),
};

export const createNecesidadSchema = z
    .object({
        ...necesidadBaseFields,
        tipoNecesidadId: z
            .number()
            .positive("El tipo de necesidad es obligatorio"),
        proyectoArtisticoId: z
            .string()
            .min(1, "El proyecto artistico es obligatorio"),
    })
    .refine(
        (data) => {
            if (data.presupuestoMin !== undefined && data.presupuestoMax !== undefined) {
                return data.presupuestoMax >= data.presupuestoMin;
            }
            return true;
        },
        {
            message: "El presupuesto maximo debe ser mayor o igual al minimo",
            path: ["presupuestoMax"],
        }
    )
    .refine(
        (data) => {
            if (data.presupuestoMin !== undefined || data.presupuestoMax !== undefined) {
                return data.monedaId !== undefined;
            }
            return true;
        },
        {
            message: "La moneda es obligatoria cuando se especifica presupuesto",
            path: ["monedaId"],
        }
    )
    .refine(
        (data) => {
            if (data.modalidadTrabajoId === 1 || data.modalidadTrabajoId === 3) {
                return data.ubicacionCiudad && data.ubicacionCiudad.length > 0;
            }
            return true;
        },
        {
            message: "La ubicacion es obligatoria para modalidad Presencial o Hibrida",
            path: ["ubicacionCiudad"],
        }
    )
    .refine(
        (data) => {
            if (data.fechaLimitePropuestas) {
                const limite = new Date(data.fechaLimitePropuestas);
                const hoy = new Date();
                hoy.setHours(0, 0, 0, 0);
                return limite > hoy;
            }
            return true;
        },
        {
            message: "La fecha limite debe ser posterior a hoy",
            path: ["fechaLimitePropuestas"],
        }
    )
    .refine(
        (data) => {
            if (data.fechaInicioPrevista) {
                const inicio = new Date(data.fechaInicioPrevista);
                const hoy = new Date();
                hoy.setHours(0, 0, 0, 0);
                return inicio >= hoy;
            }
            return true;
        },
        {
            message: "La fecha de inicio debe ser igual o posterior a hoy",
            path: ["fechaInicioPrevista"],
        }
    );

export const updateNecesidadSchema = z
    .object({
        ...necesidadBaseFields,
    })
    .refine(
        (data) => {
            if (data.presupuestoMin !== undefined && data.presupuestoMax !== undefined) {
                return data.presupuestoMax >= data.presupuestoMin;
            }
            return true;
        },
        {
            message: "El presupuesto maximo debe ser mayor o igual al minimo",
            path: ["presupuestoMax"],
        }
    )
    .refine(
        (data) => {
            if (data.presupuestoMin !== undefined || data.presupuestoMax !== undefined) {
                return data.monedaId !== undefined;
            }
            return true;
        },
        {
            message: "La moneda es obligatoria cuando se especifica presupuesto",
            path: ["monedaId"],
        }
    )
    .refine(
        (data) => {
            if (data.modalidadTrabajoId === 1 || data.modalidadTrabajoId === 3) {
                return data.ubicacionCiudad && data.ubicacionCiudad.length > 0;
            }
            return true;
        },
        {
            message: "La ubicacion es obligatoria para modalidad Presencial o Hibrida",
            path: ["ubicacionCiudad"],
        }
    )
    .refine(
        (data) => {
            if (data.fechaLimitePropuestas) {
                const limite = new Date(data.fechaLimitePropuestas);
                const hoy = new Date();
                hoy.setHours(0, 0, 0, 0);
                return limite > hoy;
            }
            return true;
        },
        {
            message: "La fecha limite debe ser posterior a hoy",
            path: ["fechaLimitePropuestas"],
        }
    )
    .refine(
        (data) => {
            if (data.fechaInicioPrevista) {
                const inicio = new Date(data.fechaInicioPrevista);
                const hoy = new Date();
                hoy.setHours(0, 0, 0, 0);
                return inicio >= hoy;
            }
            return true;
        },
        {
            message: "La fecha de inicio debe ser igual o posterior a hoy",
            path: ["fechaInicioPrevista"],
        }
    );

export const cerrarNecesidadSchema = z.object({
    motivo: z
        .string()
        .max(500, "El motivo no puede superar los 500 caracteres")
        .optional(),
});

// ========== Types Inferidos - Necesidades ==========

export type CreateNecesidadFormData = z.infer<typeof createNecesidadSchema>;
export type UpdateNecesidadFormData = z.infer<typeof updateNecesidadSchema>;
export type CerrarNecesidadFormData = z.infer<typeof cerrarNecesidadSchema>;

// ========== Schemas para Explorar Propuestas - Vista Profesional (US-CS-03) ==========

export const createPropuestaSchema = z.object({
    precioPropuesto: z
        .number({
            required_error: 'El precio propuesto es obligatorio',
            invalid_type_error: 'El precio debe ser un numero',
        })
        .positive('El precio propuesto debe ser mayor a 0'),
    monedaId: z
        .number({
            required_error: 'La moneda es obligatoria',
            invalid_type_error: 'Selecciona una moneda valida',
        })
        .positive('La moneda es obligatoria'),
    diasEstimados: z
        .number()
        .int('Los dias estimados deben ser un numero entero')
        .positive('Los dias estimados deben ser mayor a 0')
        .max(365, 'Maximo 365 dias')
        .optional(),
    mensajePropuesta: z
        .string()
        .min(1, 'El mensaje de propuesta es obligatorio')
        .min(20, 'El mensaje debe tener al menos 20 caracteres')
        .max(2000, 'El mensaje no puede superar los 2000 caracteres'),
});

export const filterNecesidadesSchema = z.object({
    tipoNecesidadId: z.number().positive().optional(),
    modalidad: z.number().positive().optional(),
    presupuestoMin: z
        .number()
        .nonnegative('El presupuesto minimo no puede ser negativo')
        .optional(),
    presupuestoMax: z
        .number()
        .nonnegative('El presupuesto maximo no puede ser negativo')
        .optional(),
    pais: z.string().max(100).optional(),
    orderBy: z.enum(['recientes', 'mayor-presupuesto', 'fecha-limite']).optional(),
    search: z.string().max(200).optional(),
    page: z.number().positive().optional(),
    pageSize: z.number().positive().max(50).optional(),
}).refine(
    (data) => {
        if (data.presupuestoMin !== undefined && data.presupuestoMax !== undefined) {
            return data.presupuestoMax >= data.presupuestoMin;
        }
        return true;
    },
    {
        message: 'El presupuesto maximo debe ser mayor o igual al minimo',
        path: ['presupuestoMax'],
    }
);

// ========== Types Inferidos - Explorar Propuestas ==========

export type CreatePropuestaFormData = z.infer<typeof createPropuestaSchema>;
export type FilterNecesidadesFormData = z.infer<typeof filterNecesidadesSchema>;

// ========== Schemas para Acuerdos, Milestones y Entregables (US-CS-04) ==========

export const aceptarPropuestaSchema = z.object({
    tituloInterno: z
        .string()
        .min(1, 'El titulo interno es obligatorio')
        .max(200, 'El titulo no puede superar los 200 caracteres'),
    fechaInicio: z
        .string()
        .min(1, 'La fecha de inicio es obligatoria'),
    fechaFinPrevista: z
        .string()
        .optional(),
}).refine(
    (data) => {
        if (data.fechaFinPrevista && data.fechaFinPrevista.length > 0) {
            return data.fechaFinPrevista > data.fechaInicio;
        }
        return true;
    },
    {
        message: 'La fecha de fin prevista debe ser posterior a la fecha de inicio',
        path: ['fechaFinPrevista'],
    }
);

export const rechazarPropuestaSchema = z.object({
    motivo: z
        .string()
        .max(500, 'El motivo no puede superar los 500 caracteres')
        .optional(),
});

export const createMilestoneSchema = z.object({
    titulo: z
        .string()
        .min(1, 'El titulo es obligatorio')
        .min(3, 'El titulo debe tener al menos 3 caracteres')
        .max(200, 'El titulo no puede superar los 200 caracteres'),
    descripcion: z
        .string()
        .max(1000, 'La descripcion no puede superar los 1000 caracteres')
        .optional(),
    importeParcial: z
        .number({
            required_error: 'El importe parcial es obligatorio',
            invalid_type_error: 'El importe debe ser un numero',
        })
        .positive('El importe parcial debe ser mayor a 0'),
    fechaLimite: z
        .string()
        .optional(),
});

export const createEntregableSchema = z.object({
    titulo: z
        .string()
        .min(1, 'El titulo es obligatorio')
        .min(3, 'El titulo debe tener al menos 3 caracteres')
        .max(200, 'El titulo no puede superar los 200 caracteres'),
    descripcion: z
        .string()
        .max(1000, 'La descripcion no puede superar los 1000 caracteres')
        .optional(),
    urlRecurso: z
        .string()
        .url('Debe ser una URL valida')
        .optional()
        .or(z.literal('')),
    milestoneId: z
        .string()
        .uuid('ID de milestone invalido')
        .optional(),
});

export const aprobarEntregableSchema = z.object({
    comentario: z
        .string()
        .max(500, 'El comentario no puede superar los 500 caracteres')
        .optional(),
});

export const rechazarEntregableSchema = z.object({
    comentario: z
        .string()
        .min(1, 'El comentario es obligatorio al rechazar')
        .min(10, 'Minimo 10 caracteres explicando que debe corregirse')
        .max(500, 'El comentario no puede superar los 500 caracteres'),
});

export const cancelarAcuerdoSchema = z.object({
    motivo: z
        .string()
        .min(1, 'El motivo es obligatorio')
        .min(20, 'El motivo debe tener al menos 20 caracteres')
        .max(1000, 'El motivo no puede superar los 1000 caracteres'),
});

// ========== Types Inferidos - Acuerdos ==========

export type AceptarPropuestaFormData = z.infer<typeof aceptarPropuestaSchema>;
export type RechazarPropuestaFormData = z.infer<typeof rechazarPropuestaSchema>;
export type CreateMilestoneFormData = z.infer<typeof createMilestoneSchema>;
export type CreateEntregableFormData = z.infer<typeof createEntregableSchema>;
export type AprobarEntregableFormData = z.infer<typeof aprobarEntregableSchema>;
export type RechazarEntregableFormData = z.infer<typeof rechazarEntregableSchema>;
export type CancelarAcuerdoFormData = z.infer<typeof cancelarAcuerdoSchema>;

// ========== Schemas para Mensajeria Crowdsourcing (US-CS-05) ==========

export const createConversacionSchema = z.object({
    necesidadId: z
        .string()
        .uuid('ID de necesidad invalido')
        .optional(),
    acuerdoId: z
        .string()
        .uuid('ID de acuerdo invalido')
        .optional(),
    userIdDestinatario: z
        .string()
        .min(1, 'El destinatario es obligatorio'),
    asunto: z
        .string()
        .min(1, 'El asunto es obligatorio')
        .max(200, 'El asunto no puede superar los 200 caracteres'),
});

export const createMensajeSchema = z.object({
    contenido: z
        .string()
        .min(1, 'El mensaje no puede estar vacio')
        .max(5000, 'El mensaje no puede superar los 5000 caracteres'),
    urlAdjunto: z
        .string()
        .url('Debe ser una URL valida')
        .optional()
        .or(z.literal('')),
});

// ========== Types Inferidos - Mensajeria ==========

export type CreateConversacionFormData = z.infer<typeof createConversacionSchema>;
export type CreateMensajeFormData = z.infer<typeof createMensajeSchema>;

// ========== Schemas para Valoraciones Bidireccionales (US-CS-06) ==========

export const createValoracionSchema = z.object({
    puntuacion: z
        .number({
            required_error: 'La puntuacion es obligatoria',
            invalid_type_error: 'La puntuacion debe ser un numero',
        })
        .int('La puntuacion debe ser un numero entero')
        .gte(1, 'Minimo 1 estrella')
        .lte(5, 'Maximo 5 estrellas'),
    comentario: z
        .string()
        .max(1000, 'El comentario no puede superar los 1000 caracteres')
        .optional(),
});

export const valoracionesQuerySchema = z.object({
    page: z
        .number()
        .int()
        .gte(1, 'La pagina debe ser mayor a 0')
        .optional()
        .default(1),
    pageSize: z
        .number()
        .int()
        .gte(1)
        .lte(50, 'El tamano de pagina no puede superar 50')
        .optional()
        .default(10),
});

// ========== Types Inferidos - Valoraciones ==========

export type CreateValoracionFormData = z.infer<typeof createValoracionSchema>;
export type ValoracionesQueryParams = z.infer<typeof valoracionesQuerySchema>;
