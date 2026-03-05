import type {
    ProgramaMetricasKpis,
    ProgramaMetricasResponse,
    RankingPromotorItem,
    EventosPorDiaItem,
    FiltroFechas,
} from "@shared/types/crowdpromotion"

// ─── Constante de identificadores ────────────────────────────────────────────

export const PROGRAMA_ID = "3fa85f64-5717-4562-b3fc-2c963f66afa6"

// ─── KPIs con datos reales ────────────────────────────────────────────────────

export const mockKpisConDatos: ProgramaMetricasKpis = {
    totalClicks: 1250,
    totalPageViews: 890,
    totalSignups: 45,
    totalConversiones: 12,
    valorTotalGenerado: 1200,
    monedaNombre: "EUR",
    tasaConversion: 0.96,
    comisionesTotales: 120,
}

export const mockKpisVacios: ProgramaMetricasKpis = {
    totalClicks: 0,
    totalPageViews: 0,
    totalSignups: 0,
    totalConversiones: 0,
    valorTotalGenerado: 0,
    monedaNombre: null,
    tasaConversion: 0,
    comisionesTotales: 0,
}

// ─── Ranking promotores ────────────────────────────────────────────────────────

export const mockRankingItem1: RankingPromotorItem = {
    promotorId: "promotor-1",
    promotorNombre: "DJ Mark",
    tipoPromotorNombre: "Influencer",
    clicks: 450,
    pageViews: 320,
    signups: 20,
    conversiones: 5,
    valorGenerado: 500,
    comisionAcumulada: 50,
}

export const mockRankingItem2: RankingPromotorItem = {
    promotorId: "promotor-2",
    promotorNombre: "MusicBlog.es",
    tipoPromotorNombre: "Medio / Blog",
    clicks: 380,
    pageViews: 280,
    signups: 15,
    conversiones: 4,
    valorGenerado: 400,
    comisionAcumulada: 40,
}

export const mockRankingItem3: RankingPromotorItem = {
    promotorId: "promotor-3",
    promotorNombre: "FanLuna",
    tipoPromotorNombre: null,
    clicks: 300,
    pageViews: 200,
    signups: 8,
    conversiones: 3,
    valorGenerado: 300,
    comisionAcumulada: 30,
}

export const mockRankingItems: RankingPromotorItem[] = [
    mockRankingItem1,
    mockRankingItem2,
    mockRankingItem3,
]

// ─── Eventos por dia (serie temporal) ────────────────────────────────────────

export const mockEventosPorDia: EventosPorDiaItem[] = [
    { fecha: "2026-03-01", clicks: 45, pageViews: 32, signups: 5, conversiones: 1 },
    { fecha: "2026-03-02", clicks: 78, pageViews: 55, signups: 8, conversiones: 2 },
    { fecha: "2026-03-03", clicks: 62, pageViews: 41, signups: 6, conversiones: 1 },
    { fecha: "2026-03-04", clicks: 91, pageViews: 67, signups: 11, conversiones: 3 },
]

// ─── Response completo ─────────────────────────────────────────────────────────

export const mockProgramaMetricasResponse: ProgramaMetricasResponse = {
    programaId: PROGRAMA_ID,
    programaTitulo: "Promociona mi nuevo album",
    fechaDesde: "2026-03-01",
    fechaHasta: "2026-03-31",
    kpis: mockKpisConDatos,
    rankingPromotores: mockRankingItems,
    eventosPorDia: mockEventosPorDia,
}

export const mockProgramaMetricasResponseVacia: ProgramaMetricasResponse = {
    programaId: PROGRAMA_ID,
    programaTitulo: "Promociona mi nuevo album",
    fechaDesde: "2026-03-01",
    fechaHasta: "2026-03-31",
    kpis: mockKpisVacios,
    rankingPromotores: [],
    eventosPorDia: [],
}

// ─── Filtros ──────────────────────────────────────────────────────────────────

export const mockFiltroConFechas: FiltroFechas = {
    fechaDesde: "2026-03-01",
    fechaHasta: "2026-03-31",
}

export const mockFiltroVacio: FiltroFechas = {}

// ─── Builder helper ───────────────────────────────────────────────────────────

export function buildRankingItem(
    overrides: Partial<RankingPromotorItem> = {}
): RankingPromotorItem {
    return { ...mockRankingItem1, ...overrides }
}
