import { z } from "zod";
import type { RewardPublic } from "../types/campania";

/**
 * Schema de validacion para crear un backing
 * Alineado con CreateBackingCommand del backend
 */
export const createBackingSchema = z.object({
    rewardId: z
        .string()
        .uuid("ID de recompensa invalido")
        .optional(),
    monto: z
        .number({ invalid_type_error: "El monto debe ser un numero" })
        .min(1, "El monto minimo es 1 EUR")
        .max(100000, "El monto maximo es 100,000 EUR"),
    mensaje: z
        .string()
        .max(500, "El mensaje no puede superar los 500 caracteres")
        .optional()
        .or(z.literal("")), // permite strings vacios
    esAnonimo: z
        .boolean()
        .default(false),
});

export type CreateBackingFormData = z.infer<typeof createBackingSchema>;

// ========== Validaciones Adicionales (Client-Side) ==========

/**
 * Valida que el monto sea >= reward.importeMinimo
 * Retorna mensaje de error o null si es valido
 */
export const validateBackingAmount = (
    monto: number,
    reward?: RewardPublic
): string | null => {
    if (!reward) return null;

    if (monto < reward.importeMinimo) {
        return `El monto debe ser al menos ${reward.importeMinimo} EUR para esta recompensa`;
    }

    return null;
};

/**
 * Valida que la recompensa este disponible (stock)
 * Retorna mensaje de error o null si es valido
 */
export const validateRewardAvailability = (
    reward?: RewardPublic
): string | null => {
    if (!reward) return null;

    if (!reward.esActivo) {
        return "Esta recompensa ya no esta disponible";
    }

    if (!reward.disponible) {
        return "Esta recompensa esta agotada";
    }

    return null;
};

/**
 * Valida que la campania este activa y acepte backings
 * Retorna mensaje de error o null si es valido
 */
export const validateCampaniaActive = (
    estadoCampaniaId: number,
    fechaFin?: string
): string | null => {
    // PUBLICADA = 2
    if (estadoCampaniaId !== 2) {
        return "Esta campania no esta activa en este momento";
    }

    if (fechaFin) {
        const endDate = new Date(fechaFin);
        const now = new Date();
        if (now > endDate) {
            return "Esta campania ya ha finalizado";
        }
    }

    return null;
};

// ========== Legacy (deprecated) ==========

/**
 * @deprecated Use createBackingSchema instead
 */
export const backingSchema = createBackingSchema.extend({
    campaniaId: z.string().uuid("ID de campania invalido"),
});

/**
 * @deprecated Use CreateBackingFormData instead
 */
export type BackingFormData = z.infer<typeof backingSchema>;
