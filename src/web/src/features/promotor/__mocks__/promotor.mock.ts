import type {
    Promotor,
    PromotorCreatedResult,
    PromotorUpdatedResult,
    PromotorDesactivadoResult,
    CreatePromotorRequest,
    UpdatePromotorRequest,
} from "@shared/types/crowdpromotion"

export const mockPromotor: Promotor = {
    id: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    nombrePublico: "DJ Marketing Pro",
    tipoPromotorId: 2,
    tipoPromotorNombre: "Influencer",
    emailContacto: "contacto@djmarketing.com",
    urlSitioWeb: "https://djmarketing.com",
    urlInstagram: "https://instagram.com/djmarketing",
    urlTikTok: "https://tiktok.com/@djmarketing",
    urlYouTube: null,
    urlTwitter: null,
    esActivo: true,
    fechaCreacion: "2026-02-25T10:00:00Z",
    totalProgramasActivos: 3,
    totalComisionesGanadas: 150.5,
    monedaComisiones: "EUR",
}

export const mockPromotorMinimo: Promotor = {
    id: "4ab96g75-6828-5673-c4gd-3d074g77bgb7",
    nombrePublico: "Fan Embajador Test",
    tipoPromotorId: 1,
    tipoPromotorNombre: "Fan Embajador",
    emailContacto: null,
    urlSitioWeb: null,
    urlInstagram: null,
    urlTikTok: null,
    urlYouTube: null,
    urlTwitter: null,
    esActivo: true,
    fechaCreacion: "2026-02-25T11:00:00Z",
    totalProgramasActivos: 0,
    totalComisionesGanadas: 0,
    monedaComisiones: "EUR",
}

export const mockPromotorInactivo: Promotor = {
    ...mockPromotor,
    esActivo: false,
    totalProgramasActivos: 0,
}

export const mockPromotorCreatedResult: PromotorCreatedResult = {
    id: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    nombrePublico: "DJ Marketing Pro",
    tipoPromotorNombre: "Influencer",
    esActivo: true,
    fechaCreacion: "2026-02-25T10:00:00Z",
}

export const mockPromotorUpdatedResult: PromotorUpdatedResult = {
    id: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    nombrePublico: "DJ Marketing Pro (Updated)",
    fechaActualizacion: "2026-02-25T12:00:00Z",
}

export const mockPromotorDesactivadoResult: PromotorDesactivadoResult = {
    id: "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    esActivo: false,
    programasDadosDeBaja: 2,
}

export const mockCreatePromotorRequest: CreatePromotorRequest = {
    nombrePublico: "DJ Marketing Pro",
    tipoPromotorId: 2,
    emailContacto: "contacto@djmarketing.com",
    urlSitioWeb: "https://djmarketing.com",
    urlInstagram: "https://instagram.com/djmarketing",
    urlTikTok: "https://tiktok.com/@djmarketing",
}

export const mockUpdatePromotorRequest: UpdatePromotorRequest = {
    nombrePublico: "DJ Marketing Pro (Updated)",
    emailContacto: "nuevo@djmarketing.com",
    urlSitioWeb: "https://djmarketing.com",
    urlInstagram: "https://instagram.com/djmarketing",
    urlYouTube: "https://youtube.com/@djmarketing",
}
