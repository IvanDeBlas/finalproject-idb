import type {
    Promotor,
    PromotorUpdatedResult,
    PromotorDesactivadoResult,
} from "@shared/types"

export const mockPromotor: Promotor = {
    id: "promotor-1",
    nombrePublico: "DJ Promo Star",
    tipoPromotorId: 2,
    tipoPromotorNombre: "Influencer",
    emailContacto: "promo@example.com",
    urlSitioWeb: "https://promostar.com",
    urlInstagram: "https://instagram.com/promostar",
    urlTikTok: "https://tiktok.com/@promostar",
    urlYouTube: "https://youtube.com/@promostar",
    urlTwitter: "https://x.com/promostar",
    esActivo: true,
    fechaCreacion: "2026-01-15T10:00:00Z",
    totalProgramasActivos: 3,
    totalComisionesGanadas: 450.50,
    monedaComisiones: "EUR",
}

export const mockPromotorMinimo: Promotor = {
    id: "promotor-2",
    nombrePublico: "Fan Embajador",
    tipoPromotorId: 1,
    tipoPromotorNombre: "Fan Embajador",
    emailContacto: null,
    urlSitioWeb: null,
    urlInstagram: null,
    urlTikTok: null,
    urlYouTube: null,
    urlTwitter: null,
    esActivo: true,
    fechaCreacion: "2026-02-01T08:00:00Z",
    totalProgramasActivos: 0,
    totalComisionesGanadas: 0,
    monedaComisiones: "EUR",
}

export const mockPromotorInactivo: Promotor = {
    ...mockPromotor,
    id: "promotor-3",
    esActivo: false,
    totalProgramasActivos: 0,
}

export const mockPromotorUpdatedResult: PromotorUpdatedResult = {
    id: "promotor-1",
    nombrePublico: "DJ Promo Star Updated",
    fechaActualizacion: "2026-02-25T12:00:00Z",
}

export const mockDesactivadoConProgramas: PromotorDesactivadoResult = {
    id: "promotor-1",
    esActivo: false,
    programasDadosDeBaja: 3,
}

export const mockDesactivadoSinProgramas: PromotorDesactivadoResult = {
    id: "promotor-2",
    esActivo: false,
    programasDadosDeBaja: 0,
}
