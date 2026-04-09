import { PaginatedResponse } from './api';

/**
 * Metricas resumidas del dashboard del artista
 * Endpoint: GET /api/dashboard/resumen
 */
export interface DashboardResumen {
    artistaId: string;
    nombreArtistico: string;
    totalRecaudado: number;
    totalBackers: number;
    campaniasActivas: number;
    campaniasCompletadas: number;
    totalCampanias: number;
    monedaSimbolo: string;
    fechaUltimoAporte?: string;
}

/**
 * Item de campania en lista de "Mis Campanias" con metricas
 * Endpoint: GET /api/campanias/mis-campanias
 */
export interface MiCampaniaListItem {
    id: string;
    titulo: string;
    imagenPrincipalUrl?: string;
    estadoCampaniaId: number;
    estadoCampaniaNombre: string;
    importeObjetivo: number;
    importeRecaudado: number;
    porcentajeProgreso: number;
    numBackers: number;
    diasRestantes?: number;
    fechaFin?: string;
    fechaCreacion: string;
}

/**
 * Respuesta completa de backings con stats y lista paginada
 * Endpoint: GET /api/campanias/{id}/backings
 */
export interface CampaniaBackingList {
    campaniaId: string;
    campaniaTitulo: string;
    stats: CampaniaBackingStats;
    backings: PaginatedResponse<CampaniaBackingItem>;
}

/**
 * Estadisticas agregadas de backings
 */
export interface CampaniaBackingStats {
    totalRecaudado: number;
    backingPromedio: number;
    totalBackers: number;
    rewardMasPopular?: string;
    ultimoBacking?: UltimoBacking;
}

/**
 * Datos del ultimo backing recibido
 */
export interface UltimoBacking {
    nombreBacker: string;
    monto: number;
    fechaCreacion: string;
}

/**
 * Item individual de backing
 */
export interface CampaniaBackingItem {
    id: string;
    nombreBacker: string;
    email?: string;
    monto: number;
    rewardNombre?: string;
    mensaje?: string;
    esAnonimo: boolean;
    estadoPedido: string;
    fechaCreacion: string;
}

/**
 * Estadisticas detalladas de campania con proyecciones
 * Endpoint: GET /api/campanias/{id}/stats
 */
export interface CampaniaStatsDetail {
    campaniaId: string;
    campaniaTitulo: string;
    importeObjetivo: number;
    importeRecaudado: number;
    porcentajeProgreso: number;
    numBackers: number;
    backingPromedio: number;
    diasRestantes?: number;
    diasTranscurridos: number;
    totalDiasCampania: number;
    proyeccionFinal?: number;
    velocidadDiaria: number;
    rewardStats: RewardStat[];
    progressoPorDia: ProgressoDia[];
}

/**
 * Estadisticas de distribucion de recompensas
 */
export interface RewardStat {
    rewardId?: string;
    rewardNombre: string;
    cantidadVendida: number;
    totalRecaudado: number;
    porcentajeDelTotal: number;
}

/**
 * Serie temporal de progreso diario
 */
export interface ProgressoDia {
    fecha: string;
    numBackings: number;
    totalRecaudado: number;
    acumulado: number;
}
