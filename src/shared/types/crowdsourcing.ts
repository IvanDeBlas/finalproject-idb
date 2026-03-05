// ========== DTOs de Response ==========

export interface PlantillaProyectoList {
    id: string;
    nombre: string;
    descripcion?: string;
    icono?: string;
    orden: number;
    precioMinTotal: number;
    precioMaxTotal: number;
    moneda: number;
    cantidadNecesidades: number;
    fases: string[];
}

export interface PlantillaProyecto {
    id: string;
    nombre: string;
    descripcion?: string;
    icono?: string;
    orden: number;
    necesidades: PlantillaProyectoNecesidad[];
    resumen: PlantillaResumen;
}

export interface PlantillaProyectoNecesidad {
    id: string;
    fase: string;
    titulo: string;
    descripcion?: string;
    rolProfesional: RolProfesional;
    precioMinOrientativo?: number;
    precioMaxOrientativo?: number;
    moneda: number;
    prioridad: PrioridadNecesidad;
    orden: number;
}

export interface PlantillaResumen {
    precioMinTotal: number;
    precioMaxTotal: number;
    moneda: number;
    cantidadNecesidadesAlta: number;
    cantidadNecesidadesMedia: number;
    cantidadNecesidadesBaja: number;
}

export interface RolProfesional {
    id: number;
    nombre: string;
    descripcion?: string;
    categoriaRolId: number;
    modalidadCobro?: string;
}

export interface RolProfesionalConCategoria {
    id: number;
    nombre: string;
    descripcion?: string;
    categoriaRol: CategoriaRol;
    modalidadCobro?: string;
    activo: boolean;
}

export interface CategoriaRol {
    id: number;
    nombre: string;
    icono?: string;
    orden: number;
}

export interface GenerarNecesidadesResult {
    necesidadesCreadas: number;
    necesidadIds: string[];
    presupuestoTotalMin: number;
    presupuestoTotalMax: number;
    moneda: number;
}

// ========== DTOs de Request ==========

export interface GenerarNecesidadesRequest {
    proyectoArtisticoId: string;
    necesidadesSeleccionadas: NecesidadSeleccionada[];
}

export interface NecesidadSeleccionada {
    plantillaNecesidadId: string;
    presupuestoMin?: number;
    presupuestoMax?: number;
    monedaId: number;
}

// ========== Union Types ==========

export type PrioridadNecesidad = "Alta" | "Media" | "Baja";

// ========== Necesidades Crowdsourcing - Union Types ==========

export type EstadoNecesidad = "Abierta" | "En Progreso" | "Cerrada" | "Cancelada";

export type ModalidadTrabajo = "Presencial" | "Remoto" | "Hibrido";

// ========== Necesidades Crowdsourcing - DTOs de Response ==========

export interface NecesidadCrowdsourcingList {
    id: string;
    titulo: string;
    estadoNecesidadId: number;
    estadoNecesidadNombre: string;
    tipoNecesidadId: number;
    tipoNecesidadNombre: string;
    presupuestoMin?: number;
    presupuestoMax?: number;
    monedaId?: number;
    monedaNombre?: string;
    modalidadTrabajoId: number;
    modalidadTrabajoNombre: string;
    numeroPropuestas: number;
    fechaCreacion: string;
    fechaLimitePropuestas?: string;
    fechaActualizacion?: string;
}

export interface NecesidadCrowdsourcing {
    id: string;
    titulo: string;
    descripcion?: string;
    tipoNecesidadId: number;
    tipoNecesidadNombre: string;
    estadoNecesidadId: number;
    estadoNecesidadNombre: string;
    modalidadTrabajoId: number;
    modalidadTrabajoNombre: string;
    presupuestoMin?: number;
    presupuestoMax?: number;
    monedaId?: number;
    monedaNombre?: string;
    ubicacionCiudad?: string;
    ubicacionPais?: string;
    fechaLimitePropuestas?: string;
    fechaInicioPrevista?: string;
    fechaCreacion: string;
    fechaActualizacion?: string;
    proyectoArtisticoId: string;
    proyectoArtisticoNombre: string;
    propuestas: PropuestaCrowdsourcing[];
}

export interface PropuestaCrowdsourcing {
    id: string;
    profesionalId: string;
    profesionalNombre: string;
    precioPropuesto: number;
    monedaId: number;
    tiempoEstimadoDias?: number;
    mensaje: string;
    estadoPropuestaId: number;
    estadoPropuestaNombre: string;
    fechaCreacion: string;
}

export interface NecesidadCreateResult {
    id: string;
    titulo: string;
    estadoNecesidadId: number;
    estadoNecesidadNombre: string;
    fechaCreacion: string;
}

export interface NecesidadUpdateResult {
    id: string;
    titulo: string;
    estadoNecesidadId: number;
    estadoNecesidadNombre: string;
    fechaActualizacion?: string;
}

export interface CerrarNecesidadResult {
    id: string;
    estadoNecesidadNombre: string;
    propuestasRechazadas: number;
}

// ========== Necesidades Crowdsourcing - DTOs de Request ==========

export interface CreateNecesidadRequest {
    titulo: string;
    descripcion?: string;
    tipoNecesidadId: number;
    modalidadTrabajoId: number;
    presupuestoMin?: number;
    presupuestoMax?: number;
    monedaId?: number;
    ubicacionCiudad?: string;
    ubicacionPais?: string;
    fechaLimitePropuestas?: string;
    fechaInicioPrevista?: string;
    proyectoArtisticoId: string;
}

export interface UpdateNecesidadRequest {
    titulo: string;
    descripcion?: string;
    modalidadTrabajoId: number;
    presupuestoMin?: number;
    presupuestoMax?: number;
    monedaId?: number;
    ubicacionCiudad?: string;
    ubicacionPais?: string;
    fechaLimitePropuestas?: string;
    fechaInicioPrevista?: string;
}

export interface CerrarNecesidadRequest {
    motivo?: string;
}

// ========== Maestras Crowdsourcing ==========

export interface MaestraTipoNecesidad {
    id: number;
    nombre: string;
}

export interface MaestraModalidadTrabajo {
    id: number;
    nombre: string;
}

export interface MaestraMoneda {
    id: number;
    nombre: string;
    simbolo: string;
}

// ========== Explorar Propuestas - Vista Profesional (US-CS-03) ==========

export interface NecesidadPublicaList {
    id: string;
    titulo: string;
    /** Truncada a 150 caracteres por el backend */
    descripcion?: string;
    tipoNecesidadId: number;
    tipoNecesidadNombre: string;
    presupuestoMin?: number;
    presupuestoMax?: number;
    monedaId?: number;
    monedaNombre?: string;
    modalidadTrabajoId: number;
    modalidadTrabajoNombre: string;
    ubicacionCiudad?: string;
    ubicacionPais?: string;
    artistaNombre: string;
    fechaCreacion: string;
    fechaLimitePropuestas?: string;
    /** Calculado en backend: fechaLimitePropuestas existe y es menor a 3 dias desde ahora */
    esUrgente: boolean;
    numeroPropuestas: number;
}

export interface ArtistaPublico {
    id: string;
    nombreArtistico: string;
    imagenUrl?: string;
}

export interface NecesidadPublica {
    id: string;
    titulo: string;
    descripcion?: string;
    tipoNecesidadId: number;
    tipoNecesidadNombre: string;
    estadoNecesidadId: number;
    estadoNecesidadNombre: string;
    modalidadTrabajoId: number;
    modalidadTrabajoNombre: string;
    presupuestoMin?: number;
    presupuestoMax?: number;
    monedaId?: number;
    monedaNombre?: string;
    ubicacionCiudad?: string;
    ubicacionPais?: string;
    fechaCreacion: string;
    fechaLimitePropuestas?: string;
    fechaInicioPrevista?: string;
    numeroPropuestas: number;
    artista: ArtistaPublico;
    /** true si el usuario autenticado ya envio una propuesta no-Retirada */
    yaPropuso: boolean;
    /** true si el usuario autenticado es el artista propietario de la necesidad */
    esPropietario: boolean;
    /** true si existe PerfilProfesional para el usuario autenticado */
    tienePerfilProfesional: boolean;
}

export interface CreatePropuestaRequest {
    precioPropuesto: number;
    monedaId: number;
    diasEstimados?: number;
    mensajePropuesta: string;
}

export interface PropuestaCreatedResult {
    id: string;
    necesidadTitulo: string;
    precioPropuesto: number;
    estadoPropuestaNombre: string;
    fechaCreacion: string;
}

export interface MiPropuestaList {
    id: string;
    necesidadTitulo: string;
    artistaNombre: string;
    precioPropuesto: number;
    monedaId: number;
    monedaNombre: string;
    estadoPropuestaId: number;
    estadoPropuestaNombre: string;
    fechaCreacion: string;
    fechaActualizacion?: string;
    /** Nulo hasta que la propuesta sea aceptada y se genere un acuerdo */
    acuerdoId?: string;
}

export interface RetirarPropuestaResult {
    id: string;
    estadoPropuestaNombre: string;
}

export type OrderByNecesidades = 'recientes' | 'mayor-presupuesto' | 'fecha-limite';

export interface NecesidadesPublicasFilter {
    tipoNecesidadId?: number;
    modalidad?: number;
    presupuestoMin?: number;
    presupuestoMax?: number;
    pais?: string;
    orderBy?: OrderByNecesidades;
    page?: number;
    pageSize?: number;
    search?: string;
}

export type EstadoPropuesta = 'Pendiente' | 'Aceptada' | 'Rechazada' | 'Retirada';

// ========== Acuerdos, Milestones y Entregables (US-CS-04) ==========

// --- Union Types ---

export type EstadoAcuerdo = 'Activo' | 'Completado' | 'Cancelado';

export type EstadoEntregable = 'Entregado' | 'Aprobado' | 'Rechazado';

export type RolAcuerdo = 'Artista' | 'Profesional';

// --- Request Interfaces ---

export interface AceptarPropuestaRequest {
    tituloInterno: string;
    fechaInicio: string;
    fechaFinPrevista?: string;
}

export interface RechazarPropuestaRequest {
    motivo?: string;
}

export interface CreateMilestoneRequest {
    titulo: string;
    descripcion?: string;
    importeParcial: number;
    fechaLimite?: string;
}

export interface CreateEntregableRequest {
    titulo: string;
    descripcion?: string;
    urlRecurso?: string;
    milestoneId?: string;
}

export interface AprobarEntregableRequest {
    comentario?: string;
}

export interface RechazarEntregableRequest {
    comentario: string;
}

export interface CancelarAcuerdoRequest {
    motivo: string;
}

// --- Result Interfaces ---

export interface AceptarPropuestaResult {
    acuerdoId: string;
    tituloInterno: string;
    estadoAcuerdoNombre: string;
    importeTotalPactado: number;
    monedaNombre: string;
    conversacionId: string;
    propuestasRechazadas: number;
}

export interface RechazarPropuestaResult {
    id: string;
    estadoPropuestaNombre: string;
}

export interface MilestoneCreatedResult {
    id: string;
    titulo: string;
    orden: number;
    importeParcial: number;
    porcentajeParcial: number;
    importeAsignadoTotal: number;
}

export interface EntregableCreatedResult {
    id: string;
    titulo: string;
    estadoEntregableNombre: string;
    fechaCreacion: string;
}

export interface AprobarEntregableResult {
    id: string;
    estadoEntregableNombre: string;
    fechaAprobacion: string;
    todosAprobadosEnMilestone: boolean;
}

export interface RechazarEntregableResult {
    id: string;
    estadoEntregableNombre: string;
}

export interface CompletarAcuerdoResult {
    id: string;
    estadoAcuerdoNombre: string;
    fechaFinReal: string;
}

export interface CancelarAcuerdoResult {
    id: string;
    estadoAcuerdoNombre: string;
    fechaFinReal: string;
    necesidadEstadoNombre: string;
}

// --- Detail DTOs ---

export interface AcuerdoArtista {
    id: string;
    nombreArtistico: string;
}

export interface AcuerdoProfesional {
    userId: string;
    perfilProfesionalId: string;
    nombre: string;
}

export interface AcuerdoNecesidad {
    id: string;
    titulo: string;
}

export interface AcuerdoTimelineEvento {
    accion: string;
    fecha: string;
    actor: string;
}

export interface Entregable {
    id: string;
    titulo: string;
    descripcion?: string;
    urlRecurso?: string;
    estadoEntregableId: number;
    estadoEntregableNombre: string;
    comentarioAprobacion?: string;
    comentarioRechazo?: string;
    fechaAprobacion?: string;
    fechaCreacion: string;
}

export interface Milestone {
    id: string;
    titulo: string;
    descripcion?: string;
    orden: number;
    importeParcial: number;
    porcentajeParcial: number;
    fechaLimite?: string;
    fechaCompletado?: string;
    entregables: Entregable[];
}

export interface Acuerdo {
    id: string;
    tituloInterno: string;
    estadoAcuerdoId: number;
    estadoAcuerdoNombre: string;
    importeTotalPactado: number;
    monedaNombre: string;
    fechaInicio: string;
    fechaFinPrevista?: string;
    fechaFinReal?: string;
    artista: AcuerdoArtista;
    profesional: AcuerdoProfesional;
    necesidad: AcuerdoNecesidad;
    conversacionId?: string;
    milestones: Milestone[];
    importeAsignado: number;
    porcentajeAsignado: number;
    miRol: RolAcuerdo;
    timeline: AcuerdoTimelineEvento[];
}

// ========== Mensajeria Crowdsourcing (US-CS-05) ==========

// --- Union Types ---

export type ContextoConversacion = 'necesidad' | 'acuerdo';
export type FiltroConversacion = 'todas' | 'necesidades' | 'acuerdos';

// --- DTOs de Request ---

export interface CreateConversacionRequest {
    /** FK a NecesidadCrowdsourcing. Mutuamente excluyente con acuerdoId. */
    necesidadId?: string;
    /** FK a AcuerdoCrowdsourcing. Mutuamente excluyente con necesidadId. */
    acuerdoId?: string;
    /** UserId del destinatario (Identity User string). Requerido. */
    userIdDestinatario: string;
    /** Asunto de la conversacion. Min 1, max 200 chars. Requerido. */
    asunto: string;
}

export interface CreateMensajeRequest {
    /** Contenido del mensaje. Min 1, max 5000 chars. Requerido. */
    contenido: string;
    /** URL de adjunto externo. Opcional. Debe ser URL valida si se proporciona. */
    urlAdjunto?: string;
}

// --- DTOs de Response (POST results) ---

export interface CreateConversacionResult {
    id: string;
    asunto: string;
    nombreDestinatario: string;
    contextoTipo: ContextoConversacion;
    contextoTitulo: string;
    /** ISO 8601 string */
    fechaCreacion: string;
}

// --- DTOs de Response (listados) ---

export interface ConversacionListItem {
    id: string;
    asunto: string;
    nombreOtraParte: string;
    /** URL de imagen de perfil de la otra parte. Undefined si no tiene foto. */
    imagenOtraParte?: string;
    contextoTipo: ContextoConversacion;
    contextoTitulo: string;
    /** Truncado a 80 chars por el backend. Undefined si la conversacion no tiene mensajes. */
    ultimoMensaje?: string;
    /** ISO 8601 string. Undefined si la conversacion no tiene mensajes. */
    fechaUltimoMensaje?: string;
    mensajesNoLeidos: number;
}

export interface ConversacionListResponse {
    items: ConversacionListItem[];
    totalCount: number;
    /** Suma de mensajes no leidos en TODAS las conversaciones del usuario (no paginado). */
    totalNoLeidos: number;
    page: number;
    pageSize: number;
}

export interface Mensaje {
    id: string;
    contenido: string;
    /** URL de adjunto. Undefined si no tiene adjunto. */
    urlAdjunto?: string;
    remitenteNombre: string;
    /** true si UserIdRemitente == UserId del usuario autenticado. Calculado en backend. */
    esPropio: boolean;
    leido: boolean;
    /** ISO 8601 string */
    fechaCreacion: string;
}

export interface MensajeListResponse {
    items: Mensaje[];
    totalCount: number;
    page: number;
    pageSize: number;
}

export interface MarcarLeidosResponse {
    /** Numero de mensajes que pasaron de Leido=false a Leido=true. 0 si no habia no leidos. */
    mensajesMarcados: number;
}

export interface NoLeidosCountResponse {
    /** Total de mensajes no leidos del usuario en todas sus conversaciones. */
    totalNoLeidos: number;
}

// ========== Valoraciones Bidireccionales (US-CS-06) ==========

// --- DTOs de Request ---

export interface CreateValoracionRequest {
    puntuacion: number;       // entero, 1-5 inclusive
    comentario?: string;      // max 1000 chars
}

// --- DTOs de Response (POST result) ---

export interface ValoracionCreatedResult {
    id: string;
    puntuacion: number;
    comentario?: string;
    fechaCreacion: string;    // ISO 8601 datetime string
}

// --- DTOs de Response (GET resumen + lista) ---

export interface ValoracionResumen {
    puntuacionMedia: number | null;   // null si totalValoraciones === 0; 1 decimal
    totalValoraciones: number;
    distribucion: {
        5: number;
        4: number;
        3: number;
        2: number;
        1: number;
    };
}

export interface ValoracionListItem {
    id: string;
    puntuacion: number;
    comentario?: string;
    autorNombre: string;
    autorImagenUrl: string | null;    // null explicito, no undefined
    acuerdoTituloInterno: string;
    fechaCreacion: string;            // ISO 8601 datetime string
}

export interface PaginatedResult<T> {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
}

export interface ValoracionesUsuario {
    resumen: ValoracionResumen;
    valoraciones: PaginatedResult<ValoracionListItem>;
}
