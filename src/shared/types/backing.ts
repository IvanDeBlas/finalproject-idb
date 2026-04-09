/**
 * Types para Backings (Aportes a Campanias)
 * Alineado con WePlayRises.Crowdfunding.Application.Dtos.BackingDto
 */

// ========== Response DTOs ==========

export interface Backing {
    id: string;
    campaniaId: string;
    userId?: string; // undefined para anonimos
    rewardId?: string; // undefined para aporte sin recompensa
    monto: number;
    mensaje?: string;
    esAnonimo: boolean;
    fechaCreacion: string; // ISO 8601
}

export interface BackingDto extends Backing {
    campaniaTitulo: string;
    userName?: string; // "Anonimo" si esAnonimo o userId undefined
    rewardNombre?: string;
    monedaSimbolo: string;
    estadoPedido: string;
}

export interface BackingPublicDto {
    id: string;
    nombreBacker: string; // "Anonimo" o nombre real
    monto: number;
    rewardNombre?: string;
    mensaje?: string;
    fechaCreacion: string; // ISO 8601
}

// ========== Request DTOs ==========

export interface CreateBackingRequest {
    rewardId?: string; // opcional, null para aporte sin recompensa
    monto: number; // requerido, >= 1.00 EUR
    mensaje?: string; // opcional, max 500 caracteres
    esAnonimo?: boolean; // default false
}

// ========== Stats ==========

export interface CampaniaStats {
    campaniaId: string;
    totalBackers: number;
    totalRecaudado: number;
    promedioAporte: number;
    aporteMinimo: number;
    aporteMaximo: number;
    diasRestantes: number;
}

// ========== Legacy (deprecated) ==========

/**
 * @deprecated Use BackingDto instead
 * Kept for backwards compatibility
 */
export interface BackingStats {
    totalBackings: number;
    montoPromedio: number;
    ultimosBacking: BackingDto[];
}
