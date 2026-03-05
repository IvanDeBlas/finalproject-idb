// Domain types - Pure business entities, no dependencies

export interface Campania {
    id: string
    artistaId: string
    proyectoArtisticoId?: string
    titulo: string
    subtitulo?: string
    descripcion: string
    descripcionCorta?: string
    videoPrincipalUrl?: string
    imagenPrincipalUrl?: string
    monedaId: number
    importeObjetivo: number
    importeMinimo?: number
    importePledgedActual: number
    tipoFinanciacionId: number
    estadoCampaniaId: number
    permiteAportacionesAnonimas: boolean
    permitePropinas: boolean
    porcentajeComisionPlataforma?: number
    backersCount: number
    fechaInicio?: string
    fechaFin?: string
    fechaPublicacion?: string
    fechaCierre?: string
    fechaCreacion: string
    fechaActualizacion?: string
    // Legacy compat
    estado: CampaniaEstado
    imagenUrl?: string
    fechaInicioDate: Date
    fechaFinDate: Date
    createdAt: Date
    updatedAt: Date
}

export interface CampaniaListItem {
    id: string
    artistaId: string
    titulo: string
    subtitulo?: string
    descripcionCorta?: string
    imagenPrincipalUrl?: string
    monedaId: number
    importeObjetivo: number
    importePledgedActual: number
    estadoCampaniaId: number
    backersCount: number
    fechaInicio?: string
    fechaFin?: string
    fechaCreacion: string
    // Legacy compat
    estado: CampaniaEstado
    imagenUrl?: string
    descripcion: string
}

export type CampaniaEstado = "borrador" | "activa" | "finalizada" | "cancelada"

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

export type { Reward, RewardListItem } from "@shared/types/reward"

export interface CampaniaFilters {
    searchTerm?: string
    artistaId?: string
    estadoCampaniaId?: number
}

export interface PaginationParams {
    pageNumber?: number
    pageSize?: number
}

// Repository interface - defines contract for infrastructure
export interface ICampaniaRepository {
    getAll(filters?: CampaniaFilters, pagination?: PaginationParams): Promise<CampaniaListItem[]>
    getById(id: string): Promise<Campania | null>
    getByArtistaId(artistaId: string): Promise<CampaniaListItem[]>
    create(data: CreateCampaniaData): Promise<Campania>
    update(id: string, data: UpdateCampaniaData): Promise<Campania>
    publicar(id: string): Promise<Campania>
}

export interface CreateCampaniaData {
    titulo: string
    descripcion: string
    importeObjetivo: number
    fechaFin: Date
    imagenUrl?: string
}

export interface UpdateCampaniaData {
    titulo?: string
    descripcion?: string
    importeObjetivo?: number
    fechaFin?: Date
    imagenUrl?: string
}
