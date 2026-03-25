// DTOs - Data Transfer Objects from/to API

export interface CampaniaDto {
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
    // Legacy fields from old API
    estado?: string
    imagenUrl?: string
    createdAt?: string
    updatedAt?: string
}

export interface CampaniaListItemDto {
    id: string
    artistaId: string
    titulo: string
    subtitulo?: string
    descripcionCorta?: string
    descripcion?: string
    imagenPrincipalUrl?: string
    monedaId: number
    importeObjetivo: number
    importePledgedActual: number
    estadoCampaniaId: number
    backersCount: number
    fechaInicio?: string
    fechaFin?: string
    fechaCreacion: string
    proyectoArtisticoId?: string
    tieneCrowdsourcing?: boolean
    tieneCrowdpromotion?: boolean
    // Legacy fields
    estado?: string
    imagenUrl?: string
}

export interface CreateCampaniaDto {
    titulo: string
    descripcion: string
    importeObjetivo: number
    fechaFin: string
    imagenUrl?: string
}

export interface UpdateCampaniaDto {
    titulo?: string
    descripcion?: string
    importeObjetivo?: number
    fechaFin?: string
    imagenUrl?: string
}

export interface RewardApiDto {
    id: string
    campaniaId: string
    nombre: string
    descripcion?: string
    importeMinimo: number
    cantidadDisponible?: number
    cantidadReclamada: number
    fechaEntregaEstimada?: string
    // Legacy
    stockLimitado?: number
    stockDisponible?: number
}
