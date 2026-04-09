import type {
    TareaPendienteItem,
    TareasPendientesResponse,
    ValidarTareaResponse,
    RechazarTareaResponse,
} from "@shared/types/crowdpromotion"

// ─── Fixtures base ────────────────────────────────────────────────────────────

export const PROGRAMA_ID = "3fa85f64-5717-4562-b3fc-2c963f66afa6"

export const mockTareaPendienteItem: TareaPendienteItem = {
    tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    tareaId: "a1b2c3d4-e5f6-7890-ab12-cd34ef567890",
    tareaNombre: "Comparte en Instagram Stories",
    promotorId: "p1b2c3d4-e5f6-7890-ab12-cd34ef567890",
    promotorNombre: "Maria Lopez",
    promotorTipoNombre: "Influencer",
    urlPruebaCompletado: "https://instagram.com/stories/maria_promo_abc123",
    comentarioPromotor: "Comparto mi story con la campana tal como se indico",
    vecesCompletada: 1,
    fechaUltimaCompletada: "2026-03-20T15:30:00Z",
}

export const mockTareaPendienteItemSinComentario: TareaPendienteItem = {
    ...mockTareaPendienteItem,
    tareaPromotorId: "c3d4e5f6-a7b8-9012-cd34-ef5678901234",
    tareaId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    tareaNombre: "Publica un TikTok",
    promotorId: "p2c3d4e5-f6a7-8901-bc23-de45fa678901",
    promotorNombre: "Carlos Ruiz",
    promotorTipoNombre: undefined,
    urlPruebaCompletado: "https://tiktok.com/@carlosruiz/video/123456789",
    comentarioPromotor: undefined,
    vecesCompletada: 3,
    fechaUltimaCompletada: "2026-03-19T10:00:00Z",
}

export const mockTareasPendientesResponse: TareasPendientesResponse = {
    items: [mockTareaPendienteItem, mockTareaPendienteItemSinComentario],
    totalCount: 2,
    page: 1,
    pageSize: 10,
    totalPages: 1,
}

export const mockTareasPendientesResponsePaginada: TareasPendientesResponse = {
    items: Array.from({ length: 10 }, (_, i) => ({
        ...mockTareaPendienteItem,
        tareaPromotorId: `tarea-promotor-${i + 1}`,
        promotorNombre: `Promotor ${i + 1}`,
    })),
    totalCount: 25,
    page: 1,
    pageSize: 10,
    totalPages: 3,
}

export const mockTareasPendientesResponseVacia: TareasPendientesResponse = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 10,
    totalPages: 0,
}

export const mockValidarTareaResponse: ValidarTareaResponse = {
    tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    estadoTareaId: 3,
    estadoTareaNombre: "Validada",
    recompensaAcreditada: 5.00,
    monedaNombre: "EUR",
    puntosAcreditados: undefined,
}

export const mockValidarTareaResponseSinRecompensa: ValidarTareaResponse = {
    tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    estadoTareaId: 3,
    estadoTareaNombre: "Validada",
    recompensaAcreditada: undefined,
    monedaNombre: undefined,
    puntosAcreditados: undefined,
}

export const mockRechazarTareaResponse: RechazarTareaResponse = {
    tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
    estadoTareaId: 4,
    estadoTareaNombre: "Rechazada",
}

// ─── Helpers ──────────────────────────────────────────────────────────────────

export function buildTareaPendiente(
    overrides: Partial<TareaPendienteItem> = {}
): TareaPendienteItem {
    return { ...mockTareaPendienteItem, ...overrides }
}

export function buildTareasPendientesResponse(
    count: number,
    page = 1,
    pageSize = 10
): TareasPendientesResponse {
    return {
        items: Array.from({ length: Math.min(count, pageSize) }, (_, i) =>
            buildTareaPendiente({
                tareaPromotorId: `tarea-promotor-${(page - 1) * pageSize + i + 1}`,
                promotorNombre: `Promotor ${(page - 1) * pageSize + i + 1}`,
            })
        ),
        totalCount: count,
        page,
        pageSize,
        totalPages: Math.ceil(count / pageSize),
    }
}
