import type {
    PromotorMetricasKpis,
    EventoReciente,
    PromotorMetricasResponse,
    TipoEventoPromo,
} from '@shared/types/crowdpromotion'

// ========== KPIs del Promotor ==========

export const mockPromotorMetricasKpis: PromotorMetricasKpis = {
    misClicks: 450,
    misPageViews: 320,
    misSignups: 18,
    misConversiones: 5,
    miValorGenerado: 500.0,
    miComisionAcumulada: 50.0,
    monedaNombre: 'EUR',
    miTasaConversion: 1.11,
}

export const mockPromotorMetricasKpis_SinActividad: PromotorMetricasKpis = {
    misClicks: 0,
    misPageViews: 0,
    misSignups: 0,
    misConversiones: 0,
    miValorGenerado: 0,
    miComisionAcumulada: 0,
    monedaNombre: null,
    miTasaConversion: 0,
}

// ========== Eventos Recientes ==========

export const mockEventoReciente_Backing: EventoReciente = {
    id: 'evt-001',
    tipoEventoId: 4 as TipoEventoPromo,
    tipoEventoNombre: 'Backing',
    valorMonetario: 100.0,
    comisionGenerada: 10.0,
    monedaNombre: 'EUR',
    fechaEvento: '2026-03-20T14:30:00Z',
}

export const mockEventoReciente_Click: EventoReciente = {
    id: 'evt-002',
    tipoEventoId: 1 as TipoEventoPromo,
    tipoEventoNombre: 'Click',
    valorMonetario: 0,
    comisionGenerada: null,
    monedaNombre: null,
    fechaEvento: '2026-03-20T12:15:00Z',
}

export const mockEventoReciente_PageView: EventoReciente = {
    id: 'evt-003',
    tipoEventoId: 2 as TipoEventoPromo,
    tipoEventoNombre: 'PageView',
    valorMonetario: 0,
    comisionGenerada: null,
    monedaNombre: null,
    fechaEvento: '2026-03-19T18:42:00Z',
}

export const mockEventoReciente_Signup: EventoReciente = {
    id: 'evt-004',
    tipoEventoId: 3 as TipoEventoPromo,
    tipoEventoNombre: 'Signup',
    valorMonetario: 0,
    comisionGenerada: null,
    monedaNombre: null,
    fechaEvento: '2026-03-18T09:00:00Z',
}

export const mockEventoReciente_Share: EventoReciente = {
    id: 'evt-005',
    tipoEventoId: 5 as TipoEventoPromo,
    tipoEventoNombre: 'Share',
    valorMonetario: 0,
    comisionGenerada: null,
    monedaNombre: null,
    fechaEvento: '2026-03-17T11:00:00Z',
}

export const mockEventosRecientes: EventoReciente[] = [
    mockEventoReciente_Backing,
    mockEventoReciente_Click,
    mockEventoReciente_PageView,
]

// ========== Response Completa del Promotor ==========

export const mockPromotorMetricasResponse: PromotorMetricasResponse = {
    promotorId: 'promotor-001',
    promotorNombre: 'DJ Mark',
    programaId: 'prog-001',
    programaTitulo: 'Promociona mi nuevo album',
    fechaDesde: '2026-01-01',
    fechaHasta: '2026-03-31',
    kpis: mockPromotorMetricasKpis,
    eventosRecientes: mockEventosRecientes,
}

export const mockPromotorMetricasResponse_SinEventos: PromotorMetricasResponse = {
    promotorId: 'promotor-001',
    promotorNombre: 'DJ Mark',
    programaId: 'prog-001',
    programaTitulo: 'Promociona mi nuevo album',
    fechaDesde: '2026-01-01',
    fechaHasta: '2026-03-31',
    kpis: mockPromotorMetricasKpis_SinActividad,
    eventosRecientes: [],
}

// ========== Programas para el selector ==========

export const mockMiProgramaParaSelector = {
    programaId: 'prog-001',
    programaTitulo: 'Promociona mi nuevo album',
    codigoReferido: 'album-2026-x7k9m',
    urlTrackingPersonalizada:
        'https://weplay.com/campanias/xxx?ref=album-2026-x7k9m&utm_source=weplay&utm_medium=referral&utm_campaign=album-2026',
    estado: 'Aprobado',
}

export const mockMiProgramaSegundo = {
    programaId: 'prog-002',
    programaTitulo: 'Difunde mi EP de verano',
    codigoReferido: 'ep-verano-2026',
    urlTrackingPersonalizada:
        'https://weplay.com/campanias/yyy?ref=ep-verano-2026&utm_source=weplay&utm_medium=referral&utm_campaign=ep-verano',
    estado: 'Aprobado',
}

export const mockListaProgramasPromotor = [
    mockMiProgramaParaSelector,
    mockMiProgramaSegundo,
]

// ========== Respuesta del servicio de tracking (POST) ==========

export const mockRegistrarEventoResponse = {
    eventoId: 'b2c3d4e5-f6a7-8901-bc23-de45fa678901',
    registrado: true,
}
