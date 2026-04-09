import type {
    MiEstadoTarea,
    MisTareasItem,
    MisTareasResponse,
    CompletarTareaResponse,
} from "../tareas/domain"

// ========== Estados del Promotor ==========

export const mockMiEstado_Completada: MiEstadoTarea = {
    tareaPromotorId: "ttp-001",
    estadoTareaId: 2,
    estadoTareaNombre: "Completada",
    vecesCompletada: 1,
    fechaPrimeraCompletada: "2026-03-01T10:00:00Z",
    fechaUltimaCompletada: "2026-03-01T10:00:00Z",
    urlPruebaCompletado: "https://instagram.com/stories/test",
    comentarioValidacion: undefined,
}

export const mockMiEstado_Validada: MiEstadoTarea = {
    tareaPromotorId: "ttp-002",
    estadoTareaId: 3,
    estadoTareaNombre: "Validada",
    vecesCompletada: 1,
    fechaPrimeraCompletada: "2026-03-01T10:00:00Z",
    fechaUltimaCompletada: "2026-03-01T10:00:00Z",
    urlPruebaCompletado: "https://instagram.com/stories/test",
    comentarioValidacion: undefined,
}

export const mockMiEstado_Rechazada: MiEstadoTarea = {
    tareaPromotorId: "ttp-003",
    estadoTareaId: 4,
    estadoTareaNombre: "Rechazada",
    vecesCompletada: 0,
    fechaPrimeraCompletada: "2026-03-01T10:00:00Z",
    fechaUltimaCompletada: "2026-03-01T10:00:00Z",
    urlPruebaCompletado: "https://instagram.com/stories/old",
    comentarioValidacion: "La URL no muestra el contenido requerido",
}

// ========== Tareas ==========

export const mockTarea_Pendiente: MisTareasItem = {
    tareaId: "tarea-001",
    nombre: "Comparte en Instagram Stories",
    descripcion: "Sube una story mencionando la campana de lanzamiento",
    instruccionesUrl: "https://weplay.com/instrucciones/instagram",
    tipoEventoPromoNombre: "Share",
    tipoRewardNombre: "Monetaria",
    importeRecompensa: 5.0,
    monedaNombre: "EUR",
    puntosRecompensa: undefined,
    esRepetible: true,
    maxRepeticiones: 10,
    orden: 1,
    miEstado: undefined,
}

export const mockTarea_Completada_NoRepetible: MisTareasItem = {
    tareaId: "tarea-002",
    nombre: "Escribe una resena en Spotify",
    descripcion: "Deja una resena en la pagina del artista",
    instruccionesUrl: undefined,
    tipoEventoPromoNombre: "Review",
    tipoRewardNombre: "Monetaria",
    importeRecompensa: 3.0,
    monedaNombre: "EUR",
    puntosRecompensa: undefined,
    esRepetible: false,
    maxRepeticiones: undefined,
    orden: 2,
    miEstado: mockMiEstado_Completada,
}

export const mockTarea_Validada_NoRepetible: MisTareasItem = {
    tareaId: "tarea-003",
    nombre: "Publica un video en TikTok",
    descripcion: "Publica un video mencionando el album",
    instruccionesUrl: undefined,
    tipoEventoPromoNombre: "Post",
    tipoRewardNombre: "Monetaria",
    importeRecompensa: 8.0,
    monedaNombre: "EUR",
    puntosRecompensa: undefined,
    esRepetible: false,
    maxRepeticiones: undefined,
    orden: 3,
    miEstado: mockMiEstado_Validada,
}

export const mockTarea_Rechazada: MisTareasItem = {
    tareaId: "tarea-004",
    nombre: "Menciona el album en tu blog",
    descripcion: "Escribe un articulo en tu blog sobre el album",
    instruccionesUrl: undefined,
    tipoEventoPromoNombre: "Post",
    tipoRewardNombre: "Monetaria",
    importeRecompensa: 10.0,
    monedaNombre: "EUR",
    puntosRecompensa: undefined,
    esRepetible: false,
    maxRepeticiones: undefined,
    orden: 4,
    miEstado: mockMiEstado_Rechazada,
}

export const mockTarea_RepetibleLimiteAlcanzado: MisTareasItem = {
    tareaId: "tarea-005",
    nombre: "Comparte en Twitter/X",
    descripcion: "Tuitea sobre el album con el hashtag oficial",
    instruccionesUrl: undefined,
    tipoEventoPromoNombre: "Share",
    tipoRewardNombre: "Monetaria",
    importeRecompensa: 2.0,
    monedaNombre: "EUR",
    puntosRecompensa: undefined,
    esRepetible: true,
    maxRepeticiones: 3,
    orden: 5,
    miEstado: {
        tareaPromotorId: "ttp-004",
        estadoTareaId: 3,
        estadoTareaNombre: "Validada",
        vecesCompletada: 3,
        fechaPrimeraCompletada: "2026-02-20T10:00:00Z",
        fechaUltimaCompletada: "2026-03-01T10:00:00Z",
        urlPruebaCompletado: "https://twitter.com/test",
        comentarioValidacion: undefined,
    },
}

export const mockTarea_RepetibleConCupo: MisTareasItem = {
    tareaId: "tarea-006",
    nombre: "Comenta en YouTube",
    descripcion: "Deja un comentario en el video del artista",
    instruccionesUrl: undefined,
    tipoEventoPromoNombre: "Comment",
    tipoRewardNombre: "Monetaria",
    importeRecompensa: 1.5,
    monedaNombre: "EUR",
    puntosRecompensa: undefined,
    esRepetible: true,
    maxRepeticiones: 5,
    orden: 6,
    miEstado: {
        tareaPromotorId: "ttp-005",
        estadoTareaId: 3,
        estadoTareaNombre: "Validada",
        vecesCompletada: 2,
        fechaPrimeraCompletada: "2026-02-25T10:00:00Z",
        fechaUltimaCompletada: "2026-03-01T10:00:00Z",
        urlPruebaCompletado: "https://youtube.com/watch?v=test",
        comentarioValidacion: undefined,
    },
}

export const mockTarea_SoloPuntos: MisTareasItem = {
    tareaId: "tarea-007",
    nombre: "Comparte en Facebook",
    descripcion: "Publica en tu muro de Facebook",
    instruccionesUrl: undefined,
    tipoEventoPromoNombre: "Share",
    tipoRewardNombre: "Puntos",
    importeRecompensa: undefined,
    monedaNombre: undefined,
    puntosRecompensa: 100,
    esRepetible: false,
    maxRepeticiones: undefined,
    orden: 7,
    miEstado: undefined,
}

// ========== Responses ==========

export const mockMisTareasResponse_ConTareas: MisTareasResponse = {
    programaId: "prog-001",
    programaTitulo: "Promociona mi nuevo album",
    items: [
        mockTarea_Pendiente,
        mockTarea_Completada_NoRepetible,
        mockTarea_Rechazada,
    ],
}

export const mockMisTareasResponse_Vacia: MisTareasResponse = {
    programaId: "prog-002",
    programaTitulo: "Programa sin tareas activas",
    items: [],
}

export const mockCompletarTareaResponse: CompletarTareaResponse = {
    tareaPromotorId: "ttp-new-001",
    estadoTareaId: 2,
    estadoTareaNombre: "Completada",
    vecesCompletada: 1,
    fechaUltimaCompletada: "2026-03-01T12:00:00Z",
}
