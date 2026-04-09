/**
 * Types para Recompensas (Rewards)
 * Alineado con WePlayRises.Crowdfunding.Application.Dtos.RewardDto
 */

// ========== Response DTOs ==========

export interface Reward {
    id: string;
    campaniaId: string;
    tipoRewardId: number;
    nombre: string;
    descripcion?: string;
    importeMinimo: number;
    monedaId: number;
    esAddOn: boolean;
    cantidadMaxima?: number; // null = ilimitado
    cantidadPorBacker?: number;
    incluyeEnvioFisico: boolean;
    tiempoEntregaEstimado?: string; // Texto libre, ej: "30 dias", "Inmediato"
    orden: number;
    esActivo: boolean;
    fechaCreacion: string; // ISO 8601
    fechaActualizacion?: string; // ISO 8601
}

export interface RewardListItem {
    id: string;
    campaniaId: string;
    nombre: string;
    descripcion?: string;
    importeMinimo: number;
    esAddOn: boolean;
    cantidadMaxima?: number;
    incluyeEnvioFisico: boolean;
    orden: number;
    esActivo: boolean;
}

// ========== Request DTOs ==========

export interface CreateRewardRequest {
    campaniaId: string;
    tipoRewardId: number;
    nombre: string;
    descripcion?: string;
    importeMinimo: number;
    monedaId: number;
    esAddOn: boolean;
    cantidadMaxima?: number;
    cantidadPorBacker?: number;
    incluyeEnvioFisico: boolean;
    tiempoEntregaEstimado?: string;
    orden: number;
}

export interface UpdateRewardRequest {
    id: string;
    nombre?: string;
    descripcion?: string;
    importeMinimo?: number;
    tipoRewardId?: number;
    monedaId?: number;
    esAddOn?: boolean;
    cantidadMaxima?: number;
    cantidadPorBacker?: number;
    incluyeEnvioFisico?: boolean;
    tiempoEntregaEstimado?: string;
    orden?: number;
    esActivo?: boolean;
}

export interface ReorderRewardsRequest {
    campaniaId: string;
    rewardOrders: RewardOrder[];
}

export interface RewardOrder {
    rewardId: string;
    orden: number;
}

// ========== Enums ==========

export enum TipoReward {
    Digital = 1,
    Fisico = 2,
    Experiencia = 3,
    Otro = 4,
}
