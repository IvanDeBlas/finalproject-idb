import type {
    PromoProgramaListItem,
    PromoProgramaListResult,
    PromoProgramaDetail,
    PromoProgramaCreatedResult,
    PromoProgramaUpdatedResult,
    PromoProgramaDesactivadoResult,
    PromoTareaDetail,
    PromoProgramaPromotorSummary,
    PromoProgramaResumen,
} from "@shared/types"

export const mockTareaDetail: PromoTareaDetail = {
    id: "tarea-1",
    titulo: "Compartir en Instagram",
    descripcion: "Publica una story mencionando la campana",
    tipoEventoPromoId: 3,
    tipoEventoPromoNombre: "Share",
    tipoRewardId: 1,
    tipoRewardNombre: "Dinero",
    importeRecompensa: 10,
    monedaId: 1,
    monedaNombre: "EUR",
    puntosRecompensa: null,
    urlInstrucciones: "https://example.com/instrucciones",
    esRepetible: false,
    maxRepeticiones: null,
    orden: 1,
    esActivo: true,
    fechaInicio: null,
    fechaFin: null,
    completadosPorPromotores: 5,
}

export const mockTareaDetail2: PromoTareaDetail = {
    id: "tarea-2",
    titulo: "Publicar review en YouTube",
    descripcion: null,
    tipoEventoPromoId: 4,
    tipoEventoPromoNombre: "Post",
    tipoRewardId: 2,
    tipoRewardNombre: "Puntos",
    importeRecompensa: null,
    monedaId: null,
    monedaNombre: null,
    puntosRecompensa: 100,
    urlInstrucciones: null,
    esRepetible: true,
    maxRepeticiones: 3,
    orden: 2,
    esActivo: true,
    fechaInicio: "2026-03-01",
    fechaFin: "2026-06-01",
    completadosPorPromotores: 0,
}

export const mockPromotorSummary: PromoProgramaPromotorSummary = {
    id: "promotor-1",
    promotorNombre: "DJ Promo Star",
    tipoPromotorNombre: "Influencer",
    esAprobado: true,
    esBloqueado: false,
    fechaAlta: "2026-02-10T10:00:00Z",
}

export const mockPromotorSummary2: PromoProgramaPromotorSummary = {
    id: "promotor-2",
    promotorNombre: "Fan Embajador X",
    tipoPromotorNombre: "Fan Embajador",
    esAprobado: false,
    esBloqueado: false,
    fechaAlta: "2026-02-15T08:00:00Z",
}

export const mockResumen: PromoProgramaResumen = {
    totalPromotoresAprobados: 5,
    totalPromotoresPendientes: 2,
    totalEventos: 120,
    totalConversiones: 15,
    valorTotalGenerado: 2500.50,
}

export const mockProgramaListItem: PromoProgramaListItem = {
    id: "programa-1",
    titulo: "Campana de Referidos Q1",
    tipoPromoId: 1,
    tipoPromoNombre: "Referral",
    campaniaTitulo: "Mi Album Debut",
    esActivo: true,
    importeComisionPorcentaje: 10,
    importeComisionFija: null,
    monedaNombre: "EUR",
    numeroPromotores: 5,
    numeroTareas: 2,
    fechaInicio: "2026-01-01",
    fechaFin: "2026-06-30",
    fechaCreacion: "2025-12-20T10:00:00Z",
}

export const mockProgramaListItemInactivo: PromoProgramaListItem = {
    id: "programa-2",
    titulo: "Programa Influencers Verano",
    tipoPromoId: 3,
    tipoPromoNombre: "Influencer",
    campaniaTitulo: null,
    esActivo: false,
    importeComisionPorcentaje: null,
    importeComisionFija: 25,
    monedaNombre: "EUR",
    numeroPromotores: 0,
    numeroTareas: 3,
    fechaInicio: null,
    fechaFin: null,
    fechaCreacion: "2025-11-15T08:00:00Z",
}

export const mockProgramaListResult: PromoProgramaListResult = {
    items: [mockProgramaListItem, mockProgramaListItemInactivo],
    totalCount: 2,
    page: 1,
    pageSize: 10,
}

export const mockProgramaDetail: PromoProgramaDetail = {
    id: "programa-1",
    titulo: "Campana de Referidos Q1",
    descripcion: "Programa de referidos para el primer trimestre del ano",
    tipoPromoId: 1,
    tipoPromoNombre: "Referral",
    campaniaCrowdfundingId: "campania-1",
    campaniaTitulo: "Mi Album Debut",
    proyectoArtisticoId: null,
    urlLanding: "https://example.com/referidos",
    codigoTrackingBase: "ref-q1-2026",
    monedaId: 1,
    monedaNombre: "EUR",
    importeComisionPorcentaje: 10,
    importeComisionFija: null,
    esActivo: true,
    fechaInicio: "2026-01-01",
    fechaFin: "2026-06-30",
    fechaCreacion: "2025-12-20T10:00:00Z",
    fechaActualizacion: "2026-01-05T14:00:00Z",
    tareas: [mockTareaDetail, mockTareaDetail2],
    promotores: [mockPromotorSummary, mockPromotorSummary2],
    resumen: mockResumen,
}

export const mockProgramaDetailInactivo: PromoProgramaDetail = {
    ...mockProgramaDetail,
    id: "programa-2",
    esActivo: false,
}

export const mockCreatedResult: PromoProgramaCreatedResult = {
    id: "programa-new",
    titulo: "Nuevo Programa",
    tipoPromoNombre: "Referral",
    esActivo: true,
    tareasCreadas: 1,
    fechaCreacion: "2026-02-25T12:00:00Z",
}

export const mockUpdatedResult: PromoProgramaUpdatedResult = {
    id: "programa-1",
    titulo: "Programa Actualizado",
    fechaActualizacion: "2026-02-25T14:00:00Z",
}

export const mockDesactivadoResult: PromoProgramaDesactivadoResult = {
    id: "programa-1",
    esActivo: false,
    tareasDesactivadas: 2,
}
