import type { NecesidadPublicaList, NecesidadPublica } from "../domain/types"

export const mockNecesidadUrgente: NecesidadPublicaList = {
    id: "550e8400-e29b-41d4-a716-446655440000",
    titulo: "Mezcla de pistas para EP de 5 canciones",
    descripcion: "Buscamos un ingeniero de mezcla experimentado para un EP de rock alternativo...",
    tipoNecesidadId: 2,
    tipoNecesidadNombre: "Post-produccion",
    presupuestoMin: 150,
    presupuestoMax: 800,
    monedaId: 1,
    monedaNombre: "EUR",
    modalidadTrabajoId: 2,
    modalidadTrabajoNombre: "Remoto",
    ubicacionCiudad: undefined,
    ubicacionPais: undefined,
    artistaNombre: "Los Rockeros",
    fechaCreacion: "2026-02-15T10:30:00Z",
    fechaLimitePropuestas: new Date(Date.now() + 2 * 86400000).toISOString(),
    esUrgente: true,
    numeroPropuestas: 3,
}

export const mockNecesidadNormal: NecesidadPublicaList = {
    id: "660e8400-e29b-41d4-a716-446655440001",
    titulo: "Diseno de portada para EP",
    descripcion: "Necesitamos un disenador grafico para la portada de nuestro primer EP...",
    tipoNecesidadId: 5,
    tipoNecesidadNombre: "Diseno",
    presupuestoMin: 200,
    presupuestoMax: 500,
    monedaId: 1,
    monedaNombre: "EUR",
    modalidadTrabajoId: 1,
    modalidadTrabajoNombre: "Presencial",
    ubicacionCiudad: "Madrid",
    ubicacionPais: "Espana",
    artistaNombre: "Indie Band",
    fechaCreacion: "2026-02-12T10:30:00Z",
    fechaLimitePropuestas: new Date(Date.now() + 20 * 86400000).toISOString(),
    esUrgente: false,
    numeroPropuestas: 1,
}

export const mockNecesidadesList: NecesidadPublicaList[] = [
    mockNecesidadUrgente,
    mockNecesidadNormal,
]

export const mockNecesidadDetalle: NecesidadPublica = {
    id: mockNecesidadUrgente.id,
    titulo: "Mezcla de pistas para EP de 5 canciones",
    descripcion: "Buscamos un ingeniero de mezcla experimentado para un EP de 5 canciones de rock alternativo.",
    tipoNecesidadId: 2,
    tipoNecesidadNombre: "Post-produccion",
    estadoNecesidadId: 1,
    estadoNecesidadNombre: "Abierta",
    modalidadTrabajoId: 2,
    modalidadTrabajoNombre: "Remoto",
    presupuestoMin: 150,
    presupuestoMax: 800,
    monedaId: 1,
    monedaNombre: "EUR",
    ubicacionCiudad: undefined,
    ubicacionPais: undefined,
    fechaCreacion: "2026-02-15T10:30:00Z",
    fechaLimitePropuestas: new Date(Date.now() + 2 * 86400000).toISOString(),
    fechaInicioPrevista: "2026-04-01",
    numeroPropuestas: 3,
    artista: {
        id: "artista-001",
        nombreArtistico: "Los Rockeros",
        imagenUrl: "https://example.com/artista.jpg",
    },
    yaPropuso: false,
    esPropietario: false,
    tienePerfilProfesional: true,
}

export const mockNecesidadDetalleYaPropuso: NecesidadPublica = {
    ...mockNecesidadDetalle,
    yaPropuso: true,
}

export const mockNecesidadDetallePropietario: NecesidadPublica = {
    ...mockNecesidadDetalle,
    esPropietario: true,
}

export const mockNecesidadDetalleSinPerfil: NecesidadPublica = {
    ...mockNecesidadDetalle,
    tienePerfilProfesional: false,
}

export const mockNecesidadesPaginadas = {
    items: mockNecesidadesList,
    totalCount: 2,
    page: 1,
    pageSize: 12,
    totalPages: 1,
}
