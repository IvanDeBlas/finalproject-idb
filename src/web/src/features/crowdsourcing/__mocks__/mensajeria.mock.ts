import type {
    ConversacionListItem,
    ConversacionListResponse,
    Mensaje,
    MensajeListResponse,
    CreateConversacionResult,
    MarcarLeidosResponse,
    NoLeidosCountResponse,
} from "../domain"

// --- Conversaciones ---

export const mockConversacionConNoLeidos: ConversacionListItem = {
    id: "f2a3b4c5-d6e7-8f9a-0b1c-2d3e4f5a6b7c",
    asunto: "Consulta sobre la mezcla de pistas",
    nombreOtraParte: "Studio Mix Pro",
    imagenOtraParte: "https://storage.weplay.com/avatars/studio-mix-pro.jpg",
    contextoTipo: "necesidad",
    contextoTitulo: "Mezcla de pistas para EP",
    ultimoMensaje:
        "Perfecto, te envio los stems manana por la manana...",
    fechaUltimoMensaje: "2026-03-02T15:30:00Z",
    mensajesNoLeidos: 2,
}

export const mockConversacionSinNoLeidos: ConversacionListItem = {
    id: "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6e",
    asunto: "Portada del album",
    nombreOtraParte: "Diseno Grafico Pro",
    imagenOtraParte: null,
    contextoTipo: "necesidad",
    contextoTitulo: "Portada del album debut",
    ultimoMensaje: "He preparado 3 bocetos para que elijas...",
    fechaUltimoMensaje: "2026-03-02T10:00:00Z",
    mensajesNoLeidos: 0,
}

export const mockConversacionSobreAcuerdo: ConversacionListItem = {
    id: "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7f",
    asunto: "Coordinacion sesion de grabacion",
    nombreOtraParte: "Fotografo Madrid",
    imagenOtraParte: null,
    contextoTipo: "acuerdo",
    contextoTitulo: "Sesion de fotos promo",
    ultimoMensaje: "Perfecto, confirmamos el sabado",
    fechaUltimoMensaje: "2026-03-01T20:00:00Z",
    mensajesNoLeidos: 0,
}

export const mockConversacionSinMensajes: ConversacionListItem = {
    id: "c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e80",
    asunto: "Primer contacto",
    nombreOtraParte: "Productor Nuevo",
    imagenOtraParte: null,
    contextoTipo: "necesidad",
    contextoTitulo: "Produccion musical EP",
    ultimoMensaje: null,
    fechaUltimoMensaje: null,
    mensajesNoLeidos: 0,
}

export const mockConversacionListResponse: ConversacionListResponse = {
    items: [
        mockConversacionConNoLeidos,
        mockConversacionSinNoLeidos,
        mockConversacionSobreAcuerdo,
    ],
    totalCount: 3,
    totalNoLeidos: 2,
    page: 1,
    pageSize: 20,
}

export const mockConversacionListResponseEmpty: ConversacionListResponse = {
    items: [],
    totalCount: 0,
    totalNoLeidos: 0,
    page: 1,
    pageSize: 20,
}

export const mockCreateConversacionResult: CreateConversacionResult = {
    id: "f2a3b4c5-d6e7-8f9a-0b1c-2d3e4f5a6b7c",
    asunto: "Consulta sobre la mezcla de pistas",
    nombreDestinatario: "Studio Mix Pro",
    contextoTipo: "necesidad",
    contextoTitulo: "Mezcla de pistas para EP",
    fechaCreacion: "2026-03-01T10:00:00Z",
}

// --- Mensajes ---

export const mockMensajeAjeno: Mensaje = {
    id: "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
    contenido:
        "Hola, me interesa tu propuesta. Podrias contarme mas sobre tu experiencia?",
    urlAdjunto: null,
    remitenteNombre: "Los Rockeros",
    esPropio: false,
    leido: true,
    fechaCreacion: "2026-03-01T10:05:00Z",
}

export const mockMensajePropio: Mensaje = {
    id: "b2c3d4e5-f6a7-8b9c-0d1e-2f3a4b5c6d7e",
    contenido: "Claro! He trabajado con bandas como...",
    urlAdjunto: "https://drive.google.com/portfolio",
    remitenteNombre: "Studio Mix Pro",
    esPropio: true,
    leido: true,
    fechaCreacion: "2026-03-01T10:15:00Z",
}

export const mockMensajePropioSinAdjunto: Mensaje = {
    id: "c3d4e5f6-a7b8-9c0d-1e2f-3a4b5c6d7e8f",
    contenido: "Perfecto, te envio los stems manana",
    urlAdjunto: null,
    remitenteNombre: "Studio Mix Pro",
    esPropio: true,
    leido: false,
    fechaCreacion: "2026-03-02T15:30:00Z",
}

export const mockMensajeListResponse: MensajeListResponse = {
    items: [mockMensajeAjeno, mockMensajePropio, mockMensajePropioSinAdjunto],
    totalCount: 3,
    page: 1,
    pageSize: 50,
}

export const mockMensajeListResponseEmpty: MensajeListResponse = {
    items: [],
    totalCount: 0,
    page: 1,
    pageSize: 50,
}

export const mockMensajeNuevo: Mensaje = {
    id: "d4e5f6a7-b8c9-0d1e-2f3a-4b5c6d7e8f9a",
    contenido: "Nuevo mensaje enviado correctamente",
    urlAdjunto: null,
    remitenteNombre: "Studio Mix Pro",
    esPropio: true,
    leido: false,
    fechaCreacion: "2026-03-02T16:00:00Z",
}

export const mockMarcarLeidosResponse: MarcarLeidosResponse = {
    mensajesMarcados: 2,
}

export const mockMarcarLeidosResponseCero: MarcarLeidosResponse = {
    mensajesMarcados: 0,
}

export const mockNoLeidosCountResponse: NoLeidosCountResponse = {
    totalNoLeidos: 5,
}

export const mockNoLeidosCountCero: NoLeidosCountResponse = {
    totalNoLeidos: 0,
}

// --- Factories ---

export const makeConversacion = (
    overrides: Partial<ConversacionListItem> = {}
): ConversacionListItem => ({
    ...mockConversacionConNoLeidos,
    id: `conv-${Math.random().toString(36).substring(2, 9)}`,
    ...overrides,
})

export const makeMensaje = (overrides: Partial<Mensaje> = {}): Mensaje => ({
    ...mockMensajeAjeno,
    id: `msg-${Math.random().toString(36).substring(2, 9)}`,
    fechaCreacion: new Date().toISOString(),
    ...overrides,
})
