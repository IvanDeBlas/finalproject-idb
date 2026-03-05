import type {
    ValoracionCreatedResult,
    ValoracionListItem,
    ValoracionResumen,
    ValoracionesUsuario,
} from "../domain"

// --- IDs reutilizables ---
export const MOCK_ACUERDO_ID = "e1f2a3b4-c5d6-7e8f-9a0b-1c2d3e4f5a6b"
export const MOCK_USER_ID = "profesional-user-001"

// --- ValoracionCreatedResult ---
export const mockValoracionCreada: ValoracionCreatedResult = {
    id: "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
    puntuacion: 5,
    comentario:
        "Excelente trabajo, muy profesional y puntual. Las mezclas quedaron increibles.",
    fechaCreacion: "2026-03-16T10:00:00Z",
}

export const mockValoracionCreadaSinComentario: ValoracionCreatedResult = {
    id: "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e",
    puntuacion: 4,
    comentario: undefined,
    fechaCreacion: "2026-03-16T11:00:00Z",
}

// --- ValoracionListItem ---
export const mockValoracionItem: ValoracionListItem = {
    id: "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
    puntuacion: 5,
    comentario:
        "Excelente trabajo, muy profesional y puntual. Las mezclas quedaron increibles.",
    autorNombre: "Los Rockeros",
    autorImagenUrl: "https://storage.example.com/imagenes/los-rockeros.jpg",
    acuerdoTituloInterno: "Mezcla EP Los Rockeros",
    fechaCreacion: "2026-03-16T10:00:00Z",
}

export const mockValoracionItemSinComentario: ValoracionListItem = {
    id: "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e",
    puntuacion: 4,
    comentario: undefined,
    autorNombre: "Indie Band",
    autorImagenUrl: null,
    acuerdoTituloInterno: "Mastering Single",
    fechaCreacion: "2026-02-28T16:00:00Z",
}

export const mockValoracionItemSinImagen: ValoracionListItem = {
    ...mockValoracionItem,
    id: "c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f",
    autorNombre: "Studio Mix",
    autorImagenUrl: null,
}

// --- ValoracionResumen ---
export const mockResumenConValoraciones: ValoracionResumen = {
    puntuacionMedia: 4.5,
    totalValoraciones: 12,
    distribucion: {
        5: 7,
        4: 3,
        3: 1,
        2: 1,
        1: 0,
    },
}

export const mockResumenSinValoraciones: ValoracionResumen = {
    puntuacionMedia: null,
    totalValoraciones: 0,
    distribucion: {
        5: 0,
        4: 0,
        3: 0,
        2: 0,
        1: 0,
    },
}

// --- ValoracionesUsuario ---
export const mockValoracionesUsuarioConDatos: ValoracionesUsuario = {
    resumen: mockResumenConValoraciones,
    valoraciones: {
        items: [mockValoracionItem, mockValoracionItemSinComentario],
        totalCount: 12,
        page: 1,
        pageSize: 10,
    },
}

export const mockValoracionesUsuarioVacio: ValoracionesUsuario = {
    resumen: mockResumenSinValoraciones,
    valoraciones: {
        items: [],
        totalCount: 0,
        page: 1,
        pageSize: 10,
    },
}

export const mockValoracionesUsuarioPagina2: ValoracionesUsuario = {
    resumen: mockResumenConValoraciones,
    valoraciones: {
        items: [mockValoracionItemSinImagen],
        totalCount: 12,
        page: 2,
        pageSize: 10,
    },
}
