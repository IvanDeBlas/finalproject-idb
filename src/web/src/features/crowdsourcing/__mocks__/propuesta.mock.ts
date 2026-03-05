import type { MiPropuestaList, CreatePropuestaRequest, PropuestaCreatedResult } from "../domain/types"

export const mockPropuestaPendiente: MiPropuestaList = {
    id: "880e8400-e29b-41d4-a716-446655440003",
    necesidadTitulo: "Mezcla de pistas para EP de 5 canciones",
    artistaNombre: "Los Rockeros",
    precioPropuesto: 450,
    monedaId: 1,
    monedaNombre: "EUR",
    estadoPropuestaId: 1,
    estadoPropuestaNombre: "Pendiente",
    fechaCreacion: "2026-02-20T14:00:00Z",
    fechaActualizacion: undefined,
    acuerdoId: undefined,
}

export const mockPropuestaAceptada: MiPropuestaList = {
    id: "990e8400-e29b-41d4-a716-446655440004",
    necesidadTitulo: "Diseno de portada para EP",
    artistaNombre: "Indie Band",
    precioPropuesto: 300,
    monedaId: 1,
    monedaNombre: "EUR",
    estadoPropuestaId: 2,
    estadoPropuestaNombre: "Aceptada",
    fechaCreacion: "2026-02-15T10:00:00Z",
    fechaActualizacion: "2026-02-18T09:30:00Z",
    acuerdoId: "acuerdo-001",
}

export const mockPropuestaRechazada: MiPropuestaList = {
    ...mockPropuestaPendiente,
    id: "aaa0e8400-e29b-41d4-a716-446655440005",
    estadoPropuestaId: 3,
    estadoPropuestaNombre: "Rechazada",
    fechaActualizacion: "2026-02-19T11:00:00Z",
}

export const mockPropuestaRetirada: MiPropuestaList = {
    ...mockPropuestaPendiente,
    id: "bbb0e8400-e29b-41d4-a716-446655440006",
    estadoPropuestaId: 4,
    estadoPropuestaNombre: "Retirada",
    fechaActualizacion: "2026-02-21T16:00:00Z",
}

export const mockPropuestasList: MiPropuestaList[] = [
    mockPropuestaPendiente,
    mockPropuestaAceptada,
    mockPropuestaRechazada,
    mockPropuestaRetirada,
]

export const mockCreatePropuestaRequest: CreatePropuestaRequest = {
    precioPropuesto: 450,
    monedaId: 1,
    diasEstimados: 14,
    mensajePropuesta: "Soy ingeniero de mezcla con 10 anos de experiencia en rock alternativo.",
}

export const mockPropuestaCreated: PropuestaCreatedResult = {
    id: mockPropuestaPendiente.id,
    necesidadTitulo: mockPropuestaPendiente.necesidadTitulo,
    precioPropuesto: 450,
    estadoPropuestaNombre: "Pendiente",
    fechaCreacion: "2026-02-20T14:00:00Z",
}

export const mockPropuestasPaginadas = {
    items: mockPropuestasList,
    totalCount: 4,
    page: 1,
    pageSize: 12,
    totalPages: 1,
}
