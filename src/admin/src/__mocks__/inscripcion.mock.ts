import type {
    InscripcionListItem,
    InscripcionesListResponse,
    InscripcionAprobada,
    InscripcionRechazada,
    InscripcionBloqueada,
    InscripcionDadaDeBaja,
} from "@shared/types"

export const mockInscripcionPendiente: InscripcionListItem = {
    id: "inscripcion-1",
    promotorId: "promotor-1",
    promotorNombre: "DJ Marketing Pro",
    tipoPromotorNombre: "Influencer",
    promotorEmailContacto: "contacto@djmarketing.com",
    promotorUrlInstagram: "https://instagram.com/djmarketing",
    promotorUrlTikTok: null,
    promotorUrlSitioWeb: "https://djmarketing.com",
    esAprobado: false,
    esBloqueado: false,
    codigoReferido: null,
    fechaAlta: "2026-03-05T14:00:00Z",
    fechaBaja: null,
    estado: "Pendiente",
}

export const mockInscripcionAprobada: InscripcionListItem = {
    id: "inscripcion-2",
    promotorId: "promotor-2",
    promotorNombre: "MusicBlog.es",
    tipoPromotorNombre: "Medio / Blog",
    promotorEmailContacto: "info@musicblog.es",
    promotorUrlInstagram: null,
    promotorUrlTikTok: null,
    promotorUrlSitioWeb: "https://musicblog.es",
    esAprobado: true,
    esBloqueado: false,
    codigoReferido: "album-2026-m3k2n",
    fechaAlta: "2026-03-06T09:00:00Z",
    fechaBaja: null,
    estado: "Aprobado",
}

export const mockInscripcionBloqueada: InscripcionListItem = {
    id: "inscripcion-3",
    promotorId: "promotor-3",
    promotorNombre: "Spammer123",
    tipoPromotorNombre: "Fan Embajador",
    promotorEmailContacto: null,
    promotorUrlInstagram: null,
    promotorUrlTikTok: null,
    promotorUrlSitioWeb: null,
    esAprobado: false,
    esBloqueado: true,
    codigoReferido: null,
    fechaAlta: "2026-03-07T10:00:00Z",
    fechaBaja: null,
    estado: "Bloqueado",
}

export const mockInscripcionesPendientesResponse: InscripcionesListResponse = {
    items: [mockInscripcionPendiente],
    totalCount: 1,
    page: 1,
    pageSize: 20,
    totalPages: 1,
}

export const mockInscripcionesAprobadasResponse: InscripcionesListResponse = {
    items: [mockInscripcionAprobada],
    totalCount: 1,
    page: 1,
    pageSize: 20,
    totalPages: 1,
}

export const mockInscripcionesBloqueadasResponse: InscripcionesListResponse = {
    items: [mockInscripcionBloqueada],
    totalCount: 1,
    page: 1,
    pageSize: 20,
    totalPages: 1,
}

export const mockInscripcionesVaciaResponse: InscripcionesListResponse = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 20,
    totalPages: 0,
}

export const mockAprobadaResult: InscripcionAprobada = {
    id: "inscripcion-1",
    promotorNombre: "DJ Marketing Pro",
    esAprobado: true,
    esBloqueado: false,
    codigoReferido: "album-2026-x7k9m",
    urlTrackingPersonalizada: null,
}

export const mockRechazadaResult: InscripcionRechazada = {
    inscripcionId: "inscripcion-1",
    promotorNombre: "DJ Marketing Pro",
}

export const mockBloqueadaResult: InscripcionBloqueada = {
    id: "inscripcion-1",
    promotorNombre: "Spammer123",
    esBloqueado: true,
    esAprobado: false,
}

export const mockDadaDeBajaResult: InscripcionDadaDeBaja = {
    id: "inscripcion-2",
    promotorNombre: "MusicBlog.es",
    esAprobado: false,
    fechaBaja: "2026-04-01T12:00:00Z",
}
