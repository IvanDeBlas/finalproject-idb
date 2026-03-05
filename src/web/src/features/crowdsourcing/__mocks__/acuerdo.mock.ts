import type {
    Acuerdo,
    Milestone,
    Entregable,
    AcuerdoTimelineEvento,
    AceptarPropuestaResult,
    MilestoneCreatedResult,
    EntregableCreatedResult,
    AprobarEntregableResult,
    RechazarEntregableResult,
    CancelarAcuerdoResult,
    CompletarAcuerdoResult,
} from "../domain/types"

// === Entregables in all states ===

export const mockEntregableEntregado: Entregable = {
    id: "entr-001-entregado",
    titulo: "Mezcla cancion 1 - v1",
    descripcion: "Primera version de la mezcla",
    urlRecurso: "https://drive.google.com/file/xyz",
    estadoEntregableId: 1,
    estadoEntregableNombre: "Entregado",
    comentarioAprobacion: undefined,
    comentarioRechazo: undefined,
    fechaAprobacion: undefined,
    fechaCreacion: "2026-03-05T14:00:00Z",
}

export const mockEntregableAprobado: Entregable = {
    ...mockEntregableEntregado,
    id: "entr-002-aprobado",
    titulo: "Mezcla cancion 1 - v2",
    estadoEntregableId: 2,
    estadoEntregableNombre: "Aprobado",
    comentarioAprobacion: "Excelente resultado",
    fechaAprobacion: "2026-03-06T10:00:00Z",
}

export const mockEntregableRechazado: Entregable = {
    ...mockEntregableEntregado,
    id: "entr-003-rechazado",
    titulo: "Mezcla cancion 2 - v1",
    estadoEntregableId: 3,
    estadoEntregableNombre: "Rechazado",
    comentarioRechazo: "La voz esta demasiado baja en el coro, necesita mas presencia.",
}

// === Milestones ===

export const mockMilestoneConEntregables: Milestone = {
    id: "mile-001",
    titulo: "Mezcla de pistas 1-3",
    descripcion: "Mezcla de las primeras 3 canciones del EP",
    orden: 1,
    importeParcial: 270.00,
    porcentajeParcial: 60.00,
    fechaLimite: "2026-03-08T00:00:00Z",
    fechaCompletado: undefined,
    entregables: [mockEntregableEntregado],
}

export const mockMilestoneCompletado: Milestone = {
    ...mockMilestoneConEntregables,
    id: "mile-002",
    titulo: "Mezcla de pistas 4-5",
    fechaCompletado: "2026-03-07T18:00:00Z",
    entregables: [mockEntregableAprobado],
}

export const mockMilestoneSinEntregables: Milestone = {
    id: "mile-003",
    titulo: "Masterizacion final",
    descripcion: undefined,
    orden: 2,
    importeParcial: 180.00,
    porcentajeParcial: 40.00,
    fechaLimite: undefined,
    fechaCompletado: undefined,
    entregables: [],
}

// === Timeline ===

export const mockTimeline: AcuerdoTimelineEvento[] = [
    {
        accion: "Acuerdo creado",
        fecha: "2026-03-01T10:00:00Z",
        actor: "Los Rockeros",
    },
    {
        accion: "Milestone agregado: Mezcla de pistas 1-3",
        fecha: "2026-03-02T09:00:00Z",
        actor: "Los Rockeros",
    },
    {
        accion: "Entregable subido: Mezcla cancion 1 - v1",
        fecha: "2026-03-05T14:00:00Z",
        actor: "Studio Mix Pro",
    },
]

// === Acuerdos ===

export const mockAcuerdoActivo: Acuerdo = {
    id: "e1f2a3b4-c5d6-7e8f-9a0b-1c2d3e4f5a6b",
    tituloInterno: "Mezcla EP Los Rockeros",
    estadoAcuerdoId: 1,
    estadoAcuerdoNombre: "Activo",
    importeTotalPactado: 450.00,
    monedaNombre: "EUR",
    fechaInicio: "2026-03-01T00:00:00Z",
    fechaFinPrevista: "2026-03-15T00:00:00Z",
    fechaFinReal: undefined,
    artista: { id: "artista-001", nombreArtistico: "Los Rockeros" },
    profesional: {
        userId: "profesional-user-001",
        perfilProfesionalId: "perfil-001",
        nombre: "Studio Mix Pro",
    },
    necesidad: { id: "necesidad-001", titulo: "Mezcla de pistas para EP" },
    conversacionId: "conv-001",
    milestones: [mockMilestoneConEntregables],
    importeAsignado: 270.00,
    porcentajeAsignado: 60.00,
    miRol: "Artista",
    timeline: mockTimeline,
}

export const mockAcuerdoActivoProfesional: Acuerdo = {
    ...mockAcuerdoActivo,
    miRol: "Profesional",
}

export const mockAcuerdoCompletado: Acuerdo = {
    ...mockAcuerdoActivo,
    estadoAcuerdoId: 2,
    estadoAcuerdoNombre: "Completado",
    fechaFinReal: "2026-03-14T16:00:00Z",
}

export const mockAcuerdoCancelado: Acuerdo = {
    ...mockAcuerdoActivo,
    estadoAcuerdoId: 3,
    estadoAcuerdoNombre: "Cancelado",
    fechaFinReal: "2026-03-10T12:00:00Z",
}

export const mockAcuerdoSinMilestones: Acuerdo = {
    ...mockAcuerdoActivo,
    milestones: [],
    importeAsignado: 0,
    porcentajeAsignado: 0,
}

export const mockAcuerdoConEntregablesPendientes: Acuerdo = {
    ...mockAcuerdoActivo,
    milestones: [
        {
            ...mockMilestoneConEntregables,
            entregables: [mockEntregableEntregado, mockEntregableRechazado],
        },
    ],
}

// === Mutation Results ===

export const mockAceptarPropuestaResult: AceptarPropuestaResult = {
    acuerdoId: "e1f2a3b4-c5d6-7e8f-9a0b-1c2d3e4f5a6b",
    tituloInterno: "Mezcla EP Los Rockeros",
    estadoAcuerdoNombre: "Activo",
    importeTotalPactado: 450.00,
    monedaNombre: "EUR",
    conversacionId: "conv-001",
    propuestasRechazadas: 2,
}

export const mockMilestoneCreatedResult: MilestoneCreatedResult = {
    id: "mile-004-new",
    titulo: "Nuevo Milestone",
    orden: 2,
    importeParcial: 180.00,
    porcentajeParcial: 40.00,
    importeAsignadoTotal: 450.00,
}

export const mockEntregableCreatedResult: EntregableCreatedResult = {
    id: "entr-004-new",
    titulo: "Nuevo Entregable",
    estadoEntregableNombre: "Entregado",
    fechaCreacion: "2026-03-06T10:00:00Z",
}

export const mockAprobarEntregableResult: AprobarEntregableResult = {
    id: mockEntregableEntregado.id,
    estadoEntregableNombre: "Aprobado",
    fechaAprobacion: "2026-03-06T10:00:00Z",
    todosAprobadosEnMilestone: true,
}

export const mockAprobarEntregableResultParcial: AprobarEntregableResult = {
    ...mockAprobarEntregableResult,
    todosAprobadosEnMilestone: false,
}

export const mockRechazarEntregableResult: RechazarEntregableResult = {
    id: mockEntregableEntregado.id,
    estadoEntregableNombre: "Rechazado",
}

export const mockCancelarAcuerdoResult: CancelarAcuerdoResult = {
    id: mockAcuerdoActivo.id,
    estadoAcuerdoNombre: "Cancelado",
    fechaFinReal: "2026-03-10T12:00:00Z",
    necesidadEstadoNombre: "Abierta",
}

export const mockCompletarAcuerdoResult: CompletarAcuerdoResult = {
    id: mockAcuerdoActivo.id,
    estadoAcuerdoNombre: "Completado",
    fechaFinReal: "2026-03-14T16:00:00Z",
}
