import type { Campania, CampaniaListItem, CampaniaEstado, Reward, CreateCampaniaData, UpdateCampaniaData } from "../domain"
import type { CampaniaDto, CampaniaListItemDto, CreateCampaniaDto, UpdateCampaniaDto, RewardApiDto } from "./dtos"

function mapEstadoIdToString(estadoCampaniaId: number): CampaniaEstado {
    switch (estadoCampaniaId) {
        case 1: return "borrador"
        case 2: return "activa"
        case 3: return "finalizada"
        case 4: return "cancelada"
        default: return "borrador"
    }
}

// Map full DTO from API to Domain entity
export function mapCampaniaDtoToDomain(dto: CampaniaDto): Campania {
    const estadoCampaniaId = dto.estadoCampaniaId ?? 1
    const fechaInicio = dto.fechaInicio || dto.createdAt || dto.fechaCreacion
    const fechaFin = dto.fechaFin || dto.fechaCreacion

    return {
        id: dto.id,
        artistaId: dto.artistaId,
        proyectoArtisticoId: dto.proyectoArtisticoId,
        titulo: dto.titulo,
        subtitulo: dto.subtitulo,
        descripcion: dto.descripcion,
        descripcionCorta: dto.descripcionCorta,
        videoPrincipalUrl: dto.videoPrincipalUrl,
        imagenPrincipalUrl: dto.imagenPrincipalUrl ?? dto.imagenUrl,
        monedaId: dto.monedaId ?? 1,
        importeObjetivo: dto.importeObjetivo,
        importeMinimo: dto.importeMinimo,
        importePledgedActual: dto.importePledgedActual ?? 0,
        tipoFinanciacionId: dto.tipoFinanciacionId ?? 1,
        estadoCampaniaId,
        permiteAportacionesAnonimas: dto.permiteAportacionesAnonimas ?? false,
        permitePropinas: dto.permitePropinas ?? false,
        porcentajeComisionPlataforma: dto.porcentajeComisionPlataforma,
        backersCount: dto.backersCount ?? 0,
        fechaInicio: dto.fechaInicio,
        fechaFin: dto.fechaFin,
        fechaPublicacion: dto.fechaPublicacion,
        fechaCierre: dto.fechaCierre,
        fechaCreacion: dto.fechaCreacion ?? dto.createdAt ?? new Date().toISOString(),
        fechaActualizacion: dto.fechaActualizacion ?? dto.updatedAt,
        // Legacy compat
        estado: dto.estado as CampaniaEstado ?? mapEstadoIdToString(estadoCampaniaId),
        imagenUrl: dto.imagenPrincipalUrl ?? dto.imagenUrl,
        fechaInicioDate: new Date(fechaInicio),
        fechaFinDate: new Date(fechaFin),
        createdAt: new Date(dto.fechaCreacion ?? dto.createdAt ?? new Date().toISOString()),
        updatedAt: new Date(dto.fechaActualizacion ?? dto.updatedAt ?? new Date().toISOString()),
    }
}

// Map list item DTO to domain list item
export function mapCampaniaListItemDtoToDomain(dto: CampaniaListItemDto): CampaniaListItem {
    const estadoCampaniaId = dto.estadoCampaniaId ?? 1

    return {
        id: dto.id,
        artistaId: dto.artistaId,
        titulo: dto.titulo,
        subtitulo: dto.subtitulo,
        descripcionCorta: dto.descripcionCorta ?? dto.descripcion ?? "",
        imagenPrincipalUrl: dto.imagenPrincipalUrl ?? dto.imagenUrl,
        monedaId: dto.monedaId ?? 1,
        importeObjetivo: dto.importeObjetivo,
        importePledgedActual: dto.importePledgedActual ?? 0,
        estadoCampaniaId,
        backersCount: dto.backersCount ?? 0,
        fechaInicio: dto.fechaInicio,
        fechaFin: dto.fechaFin,
        fechaCreacion: dto.fechaCreacion,
        proyectoArtisticoId: dto.proyectoArtisticoId,
        tieneCrowdsourcing: dto.tieneCrowdsourcing ?? false,
        tieneCrowdpromotion: dto.tieneCrowdpromotion ?? false,
        // Legacy compat
        estado: dto.estado as CampaniaEstado ?? mapEstadoIdToString(estadoCampaniaId),
        imagenUrl: dto.imagenPrincipalUrl ?? dto.imagenUrl,
        descripcion: dto.descripcionCorta ?? dto.descripcion ?? "",
    }
}

// Map reward DTO to domain
export function mapRewardDtoToDomain(dto: RewardApiDto): Reward {
    return {
        id: dto.id,
        campaniaId: dto.campaniaId,
        nombre: dto.nombre,
        descripcion: dto.descripcion,
        importeMinimo: dto.importeMinimo,
        cantidadDisponible: dto.cantidadDisponible ?? dto.stockLimitado,
        cantidadReclamada: dto.cantidadReclamada ?? 0,
        fechaEntregaEstimada: dto.fechaEntregaEstimada,
        stockLimitado: dto.stockLimitado ?? dto.cantidadDisponible,
        stockDisponible: dto.stockDisponible,
    }
}

// Map Domain data to DTO for API
export function mapCreateCampaniaToDto(data: CreateCampaniaData): CreateCampaniaDto {
    return {
        titulo: data.titulo,
        descripcion: data.descripcion,
        importeObjetivo: data.importeObjetivo,
        fechaFin: data.fechaFin.toISOString(),
        imagenUrl: data.imagenUrl,
    }
}

export function mapUpdateCampaniaToDto(data: UpdateCampaniaData): UpdateCampaniaDto {
    return {
        titulo: data.titulo,
        descripcion: data.descripcion,
        importeObjetivo: data.importeObjetivo,
        fechaFin: data.fechaFin?.toISOString(),
        imagenUrl: data.imagenUrl,
    }
}
