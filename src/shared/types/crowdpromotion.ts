// ========== DTOs de Request ==========

export interface CreatePromotorRequest {
    nombrePublico: string;
    tipoPromotorId: number;
    emailContacto?: string;
    urlSitioWeb?: string;
    urlInstagram?: string;
    urlTikTok?: string;
    urlYouTube?: string;
    urlTwitter?: string;
}

export interface UpdatePromotorRequest {
    nombrePublico: string;
    emailContacto?: string;
    urlSitioWeb?: string;
    urlInstagram?: string;
    urlTikTok?: string;
    urlYouTube?: string;
    urlTwitter?: string;
}

// ========== DTOs de Response ==========

export interface PromotorCreatedResult {
    id: string;
    nombrePublico: string;
    tipoPromotorNombre: string;
    esActivo: boolean;
    fechaCreacion: string;
}

export interface Promotor {
    id: string;
    nombrePublico: string;
    tipoPromotorId: number;
    tipoPromotorNombre: string;
    emailContacto: string | null;
    urlSitioWeb: string | null;
    urlInstagram: string | null;
    urlTikTok: string | null;
    urlYouTube: string | null;
    urlTwitter: string | null;
    esActivo: boolean;
    fechaCreacion: string;
    totalProgramasActivos: number;
    totalComisionesGanadas: number;
    monedaComisiones: string;
}

export interface PromotorUpdatedResult {
    id: string;
    nombrePublico: string;
    fechaActualizacion: string;
}

export interface PromotorDesactivadoResult {
    id: string;
    esActivo: boolean;
    programasDadosDeBaja: number;
}

// ========== Maestras ==========

export interface TipoPromotor {
    id: number;
    nombre: string;
    descripcion: string;
}

// ========== Union Types ==========

export type PromotorEstado = 'activo' | 'inactivo';

// ========== PromoPrograma - Requests ==========

export interface CreatePromoTareaItem {
    titulo: string;
    descripcion?: string;
    tipoEventoPromoId: number;
    tipoRewardId: number;
    importeRecompensa?: number;
    monedaId?: number;
    puntosRecompensa?: number;
    urlInstrucciones?: string;
    esRepetible: boolean;
    maxRepeticiones?: number;
    fechaInicio?: string;
    fechaFin?: string;
}

export interface UpdatePromoTareaItem extends CreatePromoTareaItem {
    id?: string;
    esActivo?: boolean;
}

export interface CreatePromoProgramaRequest {
    titulo: string;
    descripcion?: string;
    tipoPromoId: number;
    campaniaCrowdfundingId?: string;
    proyectoArtisticoId?: string;
    urlLanding?: string;
    codigoTrackingBase?: string;
    monedaId: number;
    importeComisionPorcentaje?: number;
    importeComisionFija?: number;
    fechaInicio?: string;
    fechaFin?: string;
    tareas?: CreatePromoTareaItem[];
}

export interface UpdatePromoProgramaRequest extends Omit<CreatePromoProgramaRequest, 'tareas'> {
    tareas?: UpdatePromoTareaItem[];
}

// ========== PromoPrograma - Results ==========

export interface PromoProgramaCreatedResult {
    id: string;
    titulo: string;
    tipoPromoNombre: string;
    esActivo: boolean;
    tareasCreadas: number;
    fechaCreacion: string;
}

export interface PromoProgramaListItem {
    id: string;
    titulo: string;
    tipoPromoId: number;
    tipoPromoNombre: string;
    campaniaTitulo: string | null;
    esActivo: boolean;
    importeComisionPorcentaje: number | null;
    importeComisionFija: number | null;
    monedaNombre: string;
    numeroPromotores: number;
    numeroTareas: number;
    fechaInicio: string | null;
    fechaFin: string | null;
    fechaCreacion: string;
}

export interface PromoProgramaListResult {
    items: PromoProgramaListItem[];
    totalCount: number;
    page: number;
    pageSize: number;
}

export interface PromoTareaDetail {
    id: string;
    titulo: string;
    descripcion: string | null;
    tipoEventoPromoId: number;
    tipoEventoPromoNombre: string;
    tipoRewardId: number;
    tipoRewardNombre: string;
    importeRecompensa: number | null;
    monedaId: number | null;
    monedaNombre: string | null;
    puntosRecompensa: number | null;
    urlInstrucciones: string | null;
    esRepetible: boolean;
    maxRepeticiones: number | null;
    orden: number;
    esActivo: boolean;
    fechaInicio: string | null;
    fechaFin: string | null;
    completadosPorPromotores: number;
}

export interface PromoProgramaPromotorSummary {
    id: string;
    promotorNombre: string;
    tipoPromotorNombre: string;
    esAprobado: boolean;
    esBloqueado: boolean;
    fechaAlta: string;
}

export interface PromoProgramaResumen {
    totalPromotoresAprobados: number;
    totalPromotoresPendientes: number;
    totalEventos: number;
    totalConversiones: number;
    valorTotalGenerado: number;
}

export interface PromoProgramaDetail {
    id: string;
    titulo: string;
    descripcion: string | null;
    tipoPromoId: number;
    tipoPromoNombre: string;
    campaniaCrowdfundingId: string | null;
    campaniaTitulo: string | null;
    proyectoArtisticoId: string | null;
    urlLanding: string | null;
    codigoTrackingBase: string | null;
    monedaId: number;
    monedaNombre: string;
    importeComisionPorcentaje: number | null;
    importeComisionFija: number | null;
    esActivo: boolean;
    fechaInicio: string | null;
    fechaFin: string | null;
    fechaCreacion: string;
    fechaActualizacion: string | null;
    tareas: PromoTareaDetail[];
    promotores: PromoProgramaPromotorSummary[];
    resumen: PromoProgramaResumen;
}

export interface PromoProgramaUpdatedResult {
    id: string;
    titulo: string;
    fechaActualizacion: string;
}

export interface PromoProgramaDesactivadoResult {
    id: string;
    esActivo: boolean;
    tareasDesactivadas: number;
}

// ========== Maestras Crowdpromotion ==========

export interface TipoPromo {
    id: number;
    nombre: string;
    descripcion: string;
}

export interface TipoEventoPromo {
    id: number;
    nombre: string;
    descripcion: string;
}

export interface TipoRewardPromo {
    id: number;
    nombre: string;
    descripcion: string;
}

// ========== Union Types PromoPrograma ==========

export type PromoProgramaEstado = 'activo' | 'inactivo';

// ========== Inscripcion a Programa - US-CP-03 ==========

/**
 * Estado calculado server-side de una inscripcion.
 * Derivacion:
 *   esBloqueado=true          -> 'Bloqueado'
 *   fechaBaja != null         -> 'DadoDeBaja'
 *   esAprobado=true           -> 'Aprobado'
 *   default                   -> 'Pendiente'
 */
export type InscripcionEstado = 'Pendiente' | 'Aprobado' | 'Bloqueado' | 'DadoDeBaja';

// ========== DTOs de Response - Vista Promotor (Explorar) ==========

export interface ProgramaExplorarItem {
    id: string;
    titulo: string;
    artistaNombre: string;
    tipoPromoId: number;
    tipoPromoNombre: string;
    importeComisionPorcentaje: number | null;
    importeComisionFija: number | null;
    monedaNombre: string | null;
    numeroTareas: number;
    campaniaTitulo: string | null;
    fechaInicio: string | null;
    fechaFin: string | null;
    miEstado: InscripcionEstado | null;
}

export interface ProgramasExplorarResponse {
    items: ProgramaExplorarItem[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

// ========== DTOs de Response - Solicitar Inscripcion (Promotor) ==========

export interface InscripcionCreada {
    id: string;
    programaId: string;
    programaTitulo: string;
    esAprobado: boolean;
    esBloqueado: boolean;
    fechaAlta: string;
}

// ========== DTOs de Response - Mis Programas (Promotor) ==========

export interface TareaResumen {
    id: string;
    titulo: string;
    descripcion: string | null;
    tipoEventoPromoNombre: string;
    importeRecompensa: number | null;
    monedaNombre: string | null;
    esRepetible: boolean;
    maxRepeticiones: number | null;
    orden: number;
}

export interface MiInscripcion {
    id: string;
    programaId: string;
    programaTitulo: string;
    artistaNombre: string;
    tipoPromoNombre: string;
    importeComisionPorcentaje: number | null;
    importeComisionFija: number | null;
    monedaNombre: string | null;
    esAprobado: boolean;
    esBloqueado: boolean;
    codigoReferido: string | null;
    urlTrackingPersonalizada: string | null;
    fechaAlta: string;
    fechaBaja: string | null;
    estado: InscripcionEstado;
    tareas: TareaResumen[];
}

export interface MisProgramasResponse {
    items: MiInscripcion[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

// ========== DTOs de Response - Gestion de Inscripciones (Artista) ==========

export interface InscripcionAprobada {
    id: string;
    promotorNombre: string;
    esAprobado: boolean;
    esBloqueado: boolean;
    codigoReferido: string;
    urlTrackingPersonalizada: string | null;
}

export interface InscripcionRechazada {
    inscripcionId: string;
    promotorNombre: string;
}

export interface InscripcionBloqueada {
    id: string;
    promotorNombre: string;
    esBloqueado: boolean;
    esAprobado: boolean;
}

export interface InscripcionDadaDeBaja {
    id: string;
    promotorNombre: string;
    esAprobado: boolean;
    fechaBaja: string;
}

export interface InscripcionListItem {
    id: string;
    promotorId: string;
    promotorNombre: string;
    tipoPromotorNombre: string;
    promotorEmailContacto: string | null;
    promotorUrlInstagram: string | null;
    promotorUrlTikTok: string | null;
    promotorUrlSitioWeb: string | null;
    esAprobado: boolean;
    esBloqueado: boolean;
    codigoReferido: string | null;
    fechaAlta: string;
    fechaBaja: string | null;
    estado: InscripcionEstado;
}

export interface InscripcionesListResponse {
    items: InscripcionListItem[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

// ========== DTOs de Filtros (Request params) ==========

export interface ExplorarProgramasFilters {
    artistaNombre?: string;
    tipoPromoId?: number;
    page?: number;
    pageSize?: number;
}

export interface InscripcionesFilters {
    estado?: InscripcionEstado;
    page?: number;
    pageSize?: number;
}

// ========== Tareas de Promocion - US-CP-04 ==========

export type EstadoTareaPromo = 1 | 2 | 3 | 4;

export interface MiEstadoTarea {
    tareaPromotorId: string;
    estadoTareaId: EstadoTareaPromo;
    estadoTareaNombre: string;
    vecesCompletada: number;
    fechaPrimeraCompletada: string | undefined;
    fechaUltimaCompletada: string | undefined;
    urlPruebaCompletado: string | undefined;
    comentarioValidacion: string | undefined;
}

export interface MisTareasItem {
    tareaId: string;
    nombre: string;
    descripcion: string | undefined;
    instruccionesUrl: string | undefined;
    tipoEventoPromoNombre: string;
    tipoRewardNombre: string | undefined;
    importeRecompensa: number | undefined;
    monedaNombre: string | undefined;
    puntosRecompensa: number | undefined;
    esRepetible: boolean;
    maxRepeticiones: number | undefined;
    orden: number;
    miEstado: MiEstadoTarea | undefined;
}

export interface MisTareasResponse {
    programaId: string;
    programaTitulo: string;
    items: MisTareasItem[];
}

export interface CompletarTareaRequest {
    urlPruebaCompletado: string;
    comentarioPromotor?: string;
}

export interface CompletarTareaResponse {
    tareaPromotorId: string;
    estadoTareaId: EstadoTareaPromo;
    estadoTareaNombre: string;
    vecesCompletada: number;
    fechaUltimaCompletada: string | undefined;
}

export interface TareaPendienteItem {
    tareaPromotorId: string;
    tareaId: string;
    tareaNombre: string;
    promotorId: string;
    promotorNombre: string;
    promotorTipoNombre: string | undefined;
    urlPruebaCompletado: string | undefined;
    comentarioPromotor: string | undefined;
    vecesCompletada: number;
    fechaUltimaCompletada: string | undefined;
}

export interface TareasPendientesResponse {
    items: TareaPendienteItem[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

export interface ValidarTareaRequest {
    comentarioValidacion?: string;
}

export interface ValidarTareaResponse {
    tareaPromotorId: string;
    estadoTareaId: EstadoTareaPromo;
    estadoTareaNombre: string;
    recompensaAcreditada: number | undefined;
    monedaNombre: string | undefined;
    puntosAcreditados: number | undefined;
}

export interface RechazarTareaRequest {
    comentarioValidacion: string;
}

export interface RechazarTareaResponse {
    tareaPromotorId: string;
    estadoTareaId: EstadoTareaPromo;
    estadoTareaNombre: string;
}

// ========== Tracking y Metricas - US-CP-05 ==========

// --- DTOs de Request (Tracking) ---

/** Body para POST /tracking/evento. Endpoint publico, sin auth. */
export interface RegistrarEventoRequest {
    codigoReferido?: string;
    /** ID del tipo de evento: 1=Click, 2=PageView, 3=Signup, 4=Backing, 5=Share */
    tipoEventoPromoId: number;
    campaniaCrowdfundingId?: string;
    urlOrigen?: string;
    urlReferer?: string;
    utmSource?: string;
    utmMedium?: string;
    utmCampaign?: string;
}

/** Body para POST /tracking/conversion. Uso interno entre modulos (Crowdfunding -> Crowdpromotion). */
export interface RegistrarConversionRequest {
    codigoReferido: string;
    campaniaCrowdfundingId: string;
    aportacionCrowdfundingId: string;
    valorMonetario: number;
    monedaId: number;
    userIdAfectado: string;
}

// --- DTOs de Response (Tracking) ---

export interface RegistrarEventoResponse {
    eventoId: string;
    registrado: boolean;
}

export interface RegistrarConversionResponse {
    eventoId: string;
    comisionCalculada: number;
    monedaNombre: string | undefined;
    walletTransaccionId: string | undefined;
    comisionAcreditada: boolean;
}

// --- Dashboard del Artista (GET /programas/{id}/metricas) ---

export interface ProgramaMetricasKpis {
    totalClicks: number;
    totalPageViews: number;
    totalSignups: number;
    totalConversiones: number;
    valorTotalGenerado: number;
    monedaNombre: string | null;
    tasaConversion: number;
    comisionesTotales: number;
}

export interface RankingPromotorItem {
    promotorId: string;
    promotorNombre: string;
    tipoPromotorNombre: string | null;
    clicks: number;
    pageViews: number;
    signups: number;
    conversiones: number;
    valorGenerado: number;
    comisionAcumulada: number;
}

export interface EventosPorDiaItem {
    fecha: string;
    clicks: number;
    pageViews: number;
    signups: number;
    conversiones: number;
}

export interface ProgramaMetricasResponse {
    programaId: string;
    programaTitulo: string;
    fechaDesde: string;
    fechaHasta: string;
    kpis: ProgramaMetricasKpis;
    rankingPromotores: RankingPromotorItem[];
    eventosPorDia: EventosPorDiaItem[];
}

// --- Dashboard del Promotor (GET /promotor/metricas) ---

export interface PromotorMetricasKpis {
    misClicks: number;
    misPageViews: number;
    misSignups: number;
    misConversiones: number;
    miValorGenerado: number;
    miComisionAcumulada: number;
    monedaNombre: string | null;
    miTasaConversion: number;
}

export interface EventoReciente {
    id: string;
    tipoEventoId: number;
    tipoEventoNombre: string;
    valorMonetario: number;
    comisionGenerada: number | null;
    monedaNombre: string | null;
    fechaEvento: string;
}

export interface PromotorMetricasResponse {
    promotorId: string;
    promotorNombre: string;
    programaId: string | null;
    programaTitulo: string | null;
    fechaDesde: string;
    fechaHasta: string;
    kpis: PromotorMetricasKpis;
    eventosRecientes: EventoReciente[];
}

// --- Filtros de Metricas (Query params) ---

export interface FiltroFechas {
    fechaDesde?: string;
    fechaHasta?: string;
}

export interface FiltroMetricasPromotor extends FiltroFechas {
    programaId?: string;
}

// ========== Wallet de Promotor - US-CP-06 ==========

// --- DTOs de Response ---

export interface PromotorWallet {
    walletId: string;
    monedaId: number;
    monedaNombre: string;
    saldoDisponible: number;
    saldoPendiente: number;
    totalGanado: number;
    totalRetirado: number;
    minimoRetiro: number;
}

export interface WalletTransaccionItem {
    id: string;
    esCredito: boolean;
    importe: number;
    descripcion: string | null;
    concepto: string | null;
    estadoTransaccionId: number;
    estadoTransaccionNombre: string;
    tipoRewardId: number | null;
    tipoRewardNombre: string | null;
    promoEventoId: string | null;
    fechaCreacion: string;
    fechaProcesado: string | null;
}

export interface WalletTransaccionesPagedResponse {
    items: WalletTransaccionItem[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
}

// --- DTOs de Request ---

export interface SolicitarCobroRequest {
    importe: number;
    descripcion?: string;
}

export interface SolicitarCobroResponse {
    transaccionId: string;
    importe: number;
    monedaNombre: string;
    estadoTransaccionNombre: string;
    saldoRestante: number;
    fechaCreacion: string;
}

// --- Filtros (Query Params) ---

export interface WalletTransaccionesFilters {
    esCredito?: boolean;
    estadoTransaccionId?: number;
    fechaDesde?: string;
    fechaHasta?: string;
    page?: number;
    pageSize?: number;
}
