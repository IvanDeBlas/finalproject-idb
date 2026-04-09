import type {
    ProgramaExplorarItem,
    ProgramasExplorarResponse,
    InscripcionCreada,
    MiInscripcion,
    MisProgramasResponse,
    TareaResumen,
} from "../domain"

// ========== Programas para Explorar ==========

export const mockProgramaExplorarItem: ProgramaExplorarItem = {
    id: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    titulo: "Promociona mi nuevo album",
    artistaNombre: "Luna Nova",
    tipoPromoId: 1,
    tipoPromoNombre: "Referral",
    importeComisionPorcentaje: 10.0,
    importeComisionFija: null,
    monedaNombre: "EUR",
    numeroTareas: 3,
    campaniaTitulo: "Mi Album Debut",
    fechaInicio: "2026-03-01",
    fechaFin: "2026-06-01",
    miEstado: null,
}

export const mockProgramaPendiente: ProgramaExplorarItem = {
    ...mockProgramaExplorarItem,
    id: "4fb96f75-6828-5673-c4gd-3d074g77bga7",
    titulo: "Difunde mi EP de verano",
    artistaNombre: "Sol Naciente",
    miEstado: "Pendiente",
}

export const mockProgramaAprobado: ProgramaExplorarItem = {
    ...mockProgramaExplorarItem,
    id: "5gc07g86-7939-6784-d5he-4e185h88chb8",
    titulo: "Comparte mi single viral",
    artistaNombre: "Beat Master",
    miEstado: "Aprobado",
}

export const mockProgramaBloqueado: ProgramaExplorarItem = {
    ...mockProgramaExplorarItem,
    id: "6hd18h97-8040-7895-e6if-5f296i99dic9",
    titulo: "Programa de influencers",
    artistaNombre: "Ritmo Urbano",
    miEstado: "Bloqueado",
}

export const mockProgramaDadoDeBaja: ProgramaExplorarItem = {
    ...mockProgramaExplorarItem,
    id: "7ie29i08-9151-8906-f7jg-6g307j00ejd0",
    titulo: "Campana de lanzamiento",
    artistaNombre: "Melodia",
    miEstado: "DadoDeBaja",
}

export const mockProgramasList: ProgramaExplorarItem[] = [
    mockProgramaExplorarItem,
    mockProgramaPendiente,
    mockProgramaAprobado,
    mockProgramaBloqueado,
    mockProgramaDadoDeBaja,
]

export const mockPaginatedProgramas: ProgramasExplorarResponse = {
    items: mockProgramasList,
    totalCount: 5,
    page: 1,
    pageSize: 10,
    totalPages: 1,
}

export const mockEmptyPaginatedProgramas: ProgramasExplorarResponse = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 10,
    totalPages: 0,
}

// ========== Inscripcion Creada ==========

export const mockInscripcionCreada: InscripcionCreada = {
    id: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    programaId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    programaTitulo: "Promociona mi nuevo album",
    esAprobado: false,
    esBloqueado: false,
    fechaAlta: "2026-03-05T10:00:00Z",
}

// ========== Tareas ==========

export const mockTarea: TareaResumen = {
    id: "a1b2c3d4-e5f6-7890-ab12-cd34ef567890",
    titulo: "Comparte en Instagram Stories",
    descripcion: "Sube una story mencionando la campana...",
    tipoEventoPromoNombre: "Share",
    importeRecompensa: 5.0,
    monedaNombre: "EUR",
    esRepetible: true,
    maxRepeticiones: 10,
    orden: 1,
}

// ========== Mis Programas (Inscripciones) ==========

export const mockMiPrograma_Aprobado: MiInscripcion = {
    id: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    programaId: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    programaTitulo: "Promociona mi nuevo album",
    artistaNombre: "Luna Nova",
    tipoPromoNombre: "Referral",
    importeComisionPorcentaje: 10.0,
    importeComisionFija: null,
    monedaNombre: "EUR",
    esAprobado: true,
    esBloqueado: false,
    codigoReferido: "album-2026-x7k9m",
    urlTrackingPersonalizada: "https://weplay.com/campanias/mi-album?utm_source=weplay&utm_medium=referral&utm_campaign=album-2026&ref=album-2026-x7k9m",
    fechaAlta: "2026-03-05T10:00:00Z",
    fechaBaja: null,
    estado: "Aprobado",
    tareas: [mockTarea],
}

export const mockMiPrograma_Pendiente: MiInscripcion = {
    id: "c3d4e5f6-a7b8-9012-cd34-ef56ab789012",
    programaId: "4fb96f75-6828-5673-c4gd-3d074g77bga7",
    programaTitulo: "Difunde mi EP de verano",
    artistaNombre: "Sol Naciente",
    tipoPromoNombre: "Referral",
    importeComisionPorcentaje: 8.0,
    importeComisionFija: null,
    monedaNombre: "EUR",
    esAprobado: false,
    esBloqueado: false,
    codigoReferido: null,
    urlTrackingPersonalizada: null,
    fechaAlta: "2026-03-10T14:00:00Z",
    fechaBaja: null,
    estado: "Pendiente",
    tareas: [],
}

export const mockMiPrograma_Bloqueado: MiInscripcion = {
    id: "d4e5f6a7-b8c9-0123-de45-fg67bc890123",
    programaId: "6hd18h97-8040-7895-e6if-5f296i99dic9",
    programaTitulo: "Programa de influencers",
    artistaNombre: "Ritmo Urbano",
    tipoPromoNombre: "Referral",
    importeComisionPorcentaje: 12.0,
    importeComisionFija: null,
    monedaNombre: "EUR",
    esAprobado: false,
    esBloqueado: true,
    codigoReferido: null,
    urlTrackingPersonalizada: null,
    fechaAlta: "2026-02-20T09:00:00Z",
    fechaBaja: null,
    estado: "Bloqueado",
    tareas: [],
}

export const mockMiPrograma_DadoDeBaja: MiInscripcion = {
    id: "e5f6a7b8-c9d0-1234-ef56-gh78cd901234",
    programaId: "7ie29i08-9151-8906-f7jg-6g307j00ejd0",
    programaTitulo: "Campana de lanzamiento",
    artistaNombre: "Melodia",
    tipoPromoNombre: "Referral",
    importeComisionPorcentaje: 5.0,
    importeComisionFija: null,
    monedaNombre: "EUR",
    esAprobado: false,
    esBloqueado: false,
    codigoReferido: null,
    urlTrackingPersonalizada: null,
    fechaAlta: "2026-01-15T11:00:00Z",
    fechaBaja: "2026-02-01T16:00:00Z",
    estado: "DadoDeBaja",
    tareas: [],
}

export const mockMisProgramasList: MiInscripcion[] = [
    mockMiPrograma_Aprobado,
    mockMiPrograma_Pendiente,
    mockMiPrograma_Bloqueado,
    mockMiPrograma_DadoDeBaja,
]

export const mockPaginatedMisProgramas: MisProgramasResponse = {
    items: mockMisProgramasList,
    totalCount: 4,
    page: 1,
    pageSize: 50,
    totalPages: 1,
}

export const mockEmptyMisProgramas: MisProgramasResponse = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 50,
    totalPages: 0,
}
