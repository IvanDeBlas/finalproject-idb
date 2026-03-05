import type { CampaniaListItem, Campania } from "../domain/types"

export const mockCampaniaListItem: CampaniaListItem = {
    id: "550e8400-e29b-41d4-a716-446655440000",
    artistaId: "660e8400-e29b-41d4-a716-446655440001",
    titulo: "Mi Primer Album - Rock Alternativo",
    subtitulo: "Un viaje por el rock indie",
    descripcionCorta: "Proyecto musical que necesita tu apoyo",
    imagenPrincipalUrl: "https://example.com/imagen.jpg",
    monedaId: 1,
    importeObjetivo: 5000,
    importePledgedActual: 1250,
    estadoCampaniaId: 2,
    backersCount: 15,
    fechaInicio: "2026-03-01T00:00:00Z",
    fechaFin: "2026-04-30T23:59:59Z",
    fechaCreacion: "2026-02-12T10:30:00Z",
    estado: "activa",
    imagenUrl: "https://example.com/imagen.jpg",
    descripcion: "Proyecto musical que necesita tu apoyo",
}

export const mockCampaniaListItemNoImage: CampaniaListItem = {
    ...mockCampaniaListItem,
    id: "550e8400-e29b-41d4-a716-446655440001",
    titulo: "Tour Nacional 2026",
    imagenPrincipalUrl: undefined,
    imagenUrl: undefined,
    importeObjetivo: 15000,
    importePledgedActual: 500,
    backersCount: 3,
}

export const mockCampaniaListItemFinalizada: CampaniaListItem = {
    ...mockCampaniaListItem,
    id: "550e8400-e29b-41d4-a716-446655440002",
    titulo: "Campania Finalizada Exitosa",
    estadoCampaniaId: 3,
    estado: "finalizada",
    importePledgedActual: 6000,
    backersCount: 42,
    fechaFin: "2026-01-30T23:59:59Z",
}

export const mockCampaniaListItemOverfunded: CampaniaListItem = {
    ...mockCampaniaListItem,
    id: "550e8400-e29b-41d4-a716-446655440003",
    titulo: "Campania SuperFinanciada",
    importeObjetivo: 5000,
    importePledgedActual: 8000,
    backersCount: 65,
}

export const mockCampaniaListItemBorrador: CampaniaListItem = {
    ...mockCampaniaListItem,
    id: "550e8400-e29b-41d4-a716-446655440004",
    titulo: "Campania en Borrador",
    estadoCampaniaId: 1,
    estado: "borrador",
    importePledgedActual: 0,
    backersCount: 0,
}

export const mockCampaniaListItemCancelada: CampaniaListItem = {
    ...mockCampaniaListItem,
    id: "550e8400-e29b-41d4-a716-446655440005",
    titulo: "Campania Cancelada",
    estadoCampaniaId: 4,
    estado: "cancelada",
    importePledgedActual: 100,
    backersCount: 2,
}

export const mockCampaniasList: CampaniaListItem[] = [
    mockCampaniaListItem,
    mockCampaniaListItemFinalizada,
    mockCampaniaListItemNoImage,
]

export const mockCampaniaFull: Campania = {
    id: "550e8400-e29b-41d4-a716-446655440000",
    artistaId: "660e8400-e29b-41d4-a716-446655440001",
    titulo: "Mi Primer Album - Rock Alternativo",
    subtitulo: "Un viaje por el rock indie",
    descripcion: "Descripcion completa de la campania con detalles del proyecto",
    descripcionCorta: "Proyecto musical que necesita tu apoyo",
    videoPrincipalUrl: "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
    imagenPrincipalUrl: "https://example.com/imagen.jpg",
    monedaId: 1,
    importeObjetivo: 5000,
    importeMinimo: 100,
    importePledgedActual: 1250,
    tipoFinanciacionId: 1,
    estadoCampaniaId: 2,
    permiteAportacionesAnonimas: false,
    permitePropinas: true,
    porcentajeComisionPlataforma: 5,
    backersCount: 15,
    fechaInicio: "2026-03-01T00:00:00Z",
    fechaFin: "2026-04-30T23:59:59Z",
    fechaPublicacion: "2026-02-15T10:00:00Z",
    fechaCreacion: "2026-02-12T10:30:00Z",
    fechaActualizacion: "2026-02-14T08:00:00Z",
    estado: "activa",
    imagenUrl: "https://example.com/imagen.jpg",
    fechaInicioDate: new Date("2026-03-01T00:00:00Z"),
    fechaFinDate: new Date("2026-04-30T23:59:59Z"),
    createdAt: new Date("2026-02-12T10:30:00Z"),
    updatedAt: new Date("2026-02-14T08:00:00Z"),
}
