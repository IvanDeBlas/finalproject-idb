import type { BackingPublicDto } from "./backing";

// DTOs alineados con contracts.md
export interface Campania {
    id: string;
    artistaId: string;
    proyectoArtisticoId?: string;
    titulo: string;
    subtitulo?: string;
    descripcionCorta?: string;
    videoPrincipalUrl?: string;
    imagenPrincipalUrl?: string;
    monedaId: number;
    importeObjetivo: number;
    importeMinimo?: number;
    importePledgedActual: number;
    tipoFinanciacionId: number;
    estadoCampaniaId: number;
    permiteAportacionesAnonimas: boolean;
    permitePropinas: boolean;
    porcentajeComisionPlataforma?: number;
    fechaInicio?: string; // ISO 8601
    fechaFin?: string; // ISO 8601
    fechaPublicacion?: string; // ISO 8601
    fechaCierre?: string; // ISO 8601
    fechaCreacion: string; // ISO 8601
    fechaActualizacion?: string; // ISO 8601
}

export interface CampaniaListItem {
    id: string;
    artistaId: string;
    titulo: string;
    subtitulo?: string;
    descripcionCorta?: string;
    imagenPrincipalUrl?: string;
    importeObjetivo: number;
    importePledgedActual: number;
    estadoCampaniaId: number;
    fechaInicio?: string;
    fechaFin?: string;
    fechaCreacion: string;
}

export interface CreateCampaniaRequest {
    proyectoArtisticoId?: string;
    titulo: string;
    subtitulo?: string;
    descripcionCorta?: string;
    videoPrincipalUrl?: string;
    imagenPrincipalUrl?: string;
    monedaId: number;
    importeObjetivo: number;
    importeMinimo?: number;
    tipoFinanciacionId: number;
    permiteAportacionesAnonimas: boolean;
    permitePropinas: boolean;
    fechaInicio?: string; // ISO 8601
    fechaFin?: string; // ISO 8601
}

export interface UpdateCampaniaRequest {
    id: string;
    titulo?: string;
    subtitulo?: string;
    descripcionCorta?: string;
    videoPrincipalUrl?: string;
    imagenPrincipalUrl?: string;
    importeObjetivo?: number;
    importeMinimo?: number;
    tipoFinanciacionId?: number;
    permiteAportacionesAnonimas?: boolean;
    permitePropinas?: boolean;
    fechaInicio?: string;
    fechaFin?: string;
}

export interface PublishCampaniaResponse {
    id: string;
    estadoCampaniaId: number;
    fechaPublicacion: string;
    message: string;
}

// Enums (alineados con backend)
export enum EstadoCampania {
    Borrador = 1,
    Publicada = 2,
    Finalizada = 3,
    Cancelada = 4,
}

export enum TipoFinanciacion {
    TodoONada = 1,
    FlexibleGoal = 2,
}

// Legacy types (mantener por compatibilidad temporal)
/** @deprecated Use EstadoCampania enum instead */
export type CampaniaEstado = "borrador" | "activa" | "finalizada" | "cancelada";

/** @deprecated Use Campania interface instead */
export interface CampaniaDto {
    id: string;
    artistaId: string;
    titulo: string;
    descripcion: string;
    importeObjetivo: number;
    importePledgedActual: number;
    fechaInicio: string;
    fechaFin: string;
    estado: string;
    imagenUrl?: string;
    videoUrl?: string;
    createdAt: string;
    updatedAt: string;
}

/** @deprecated Use CreateCampaniaRequest instead */
export interface CreateCampaniaDto {
    titulo: string;
    descripcion: string;
    importeObjetivo: number;
    fechaFin: string;
    imagenUrl?: string;
}

/** @deprecated Use UpdateCampaniaRequest instead */
export interface UpdateCampaniaDto {
    titulo?: string;
    descripcion?: string;
    importeObjetivo?: number;
    fechaFin?: string;
    imagenUrl?: string;
}

/**
 * @deprecated Use CampaniaStats from backing.ts instead
 * Kept for backwards compatibility
 */
export interface CampaniaStatsLegacy {
    totalBackers: number;
    totalRecaudado: number;
    porcentajeCompletado: number;
    diasRestantes: number;
}

// ========== Detalle de Campania con Rewards y Backings ==========

export interface CampaniaDetail {
    id: string;
    artistaId: string;
    titulo: string;
    subtitulo?: string;
    descripcionCorta?: string;
    videoPrincipalUrl?: string;
    imagenPrincipalUrl?: string;
    importeObjetivo: number;
    importeMinimo?: number;
    importePledgedActual: number;
    porcentajeProgreso: number; // calculado en backend
    monedaId: number;
    monedaSimbolo: string; // EUR, USD, etc.
    estadoCampaniaId: number;
    estadoCampaniaNombre: string; // Publicada, Finalizada, etc.
    permiteAportacionesAnonimas: boolean;
    permitePropinas: boolean;
    fechaInicio?: string; // ISO 8601
    fechaFin?: string; // ISO 8601
    diasRestantes: number; // calculado en backend
    artistaNombre: string;
    artistaImagenUrl?: string;
    rewards: RewardPublic[];
    backingsRecientes: BackingPublicDto[];
    totalBackers: number;
    fechaCreacion: string; // ISO 8601
}

export interface RewardPublic {
    id: string;
    nombre: string;
    descripcion?: string;
    importeMinimo: number;
    cantidadMaxima?: number; // null = ilimitado
    cantidadVendida: number; // calculado desde PedidoCrowdfundingLinea
    disponible: boolean; // cantidadMaxima == null || cantidadVendida < cantidadMaxima
    incluyeEnvioFisico: boolean;
    tiempoEntregaEstimado?: string;
    orden: number;
    esActivo: boolean;
}
