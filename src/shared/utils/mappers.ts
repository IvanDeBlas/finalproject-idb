import type {
    Campania,
    CampaniaDto,
    Artista,
    ArtistaDto,
    ArtistaListItem,
    Backing,
    BackingDto,
} from "../types"
import type {
    CreatePromotorRequest,
    UpdatePromotorRequest,
    Promotor,
    PromotorEstado,
    InscripcionEstado,
    MisTareasItem,
} from "../types/crowdpromotion"
import { INSCRIPCION_ESTADO_BADGE_VARIANT, ESTADO_TAREA_PROMO_BADGES, ESTADO_WALLET_TRANSACCION_BADGES } from "../constants"
import type {
    CreatePromotorFormData,
    UpdatePromotorFormData,
} from "../schemas/crowdpromotion.schema"

/**
 * @deprecated CampaniaDto is deprecated. Use Campania directly from the API.
 * This mapper is kept for backwards compatibility.
 */
export function mapCampaniaDtoToDomain(dto: CampaniaDto): Campania {
    return {
        id: dto.id,
        artistaId: dto.artistaId,
        titulo: dto.titulo,
        importeObjetivo: dto.importeObjetivo,
        importePledgedActual: dto.importePledgedActual,
        monedaId: 1,
        tipoFinanciacionId: 1,
        estadoCampaniaId: 1,
        permiteAportacionesAnonimas: false,
        permitePropinas: false,
        fechaInicio: dto.fechaInicio,
        fechaFin: dto.fechaFin,
        fechaCreacion: dto.createdAt,
        fechaActualizacion: dto.updatedAt,
        imagenPrincipalUrl: dto.imagenUrl,
        videoPrincipalUrl: dto.videoUrl,
    }
}

export function mapCampaniaToDomain(campanias: CampaniaDto[]): Campania[] {
    return campanias.map(mapCampaniaDtoToDomain)
}

// Artista mappers
export function mapArtistaDtoToDomain(dto: ArtistaDto): Artista {
    return {
        ...dto,
        fechaCreacion: dto.fechaCreacion,
        fechaActualizacion: dto.fechaActualizacion,
    }
}

export function artistaToListItem(artista: Artista): ArtistaListItem {
    return {
        id: artista.id,
        nombreArtistico: artista.nombreArtistico,
        imagenUrl: artista.imagenUrl,
        ciudad: artista.ciudad,
        pais: artista.pais,
    }
}

export function artistasToListItems(artistas: Artista[]): ArtistaListItem[] {
    return artistas.map(artistaToListItem)
}

// Backing mappers
/**
 * @deprecated BackingDto now extends Backing directly.
 * This mapper is kept for backwards compatibility.
 */
export function mapBackingDtoToDomain(dto: BackingDto): Backing {
    return {
        id: dto.id,
        campaniaId: dto.campaniaId,
        userId: dto.userId,
        rewardId: dto.rewardId,
        monto: dto.monto,
        mensaje: dto.mensaje,
        esAnonimo: dto.esAnonimo,
        fechaCreacion: dto.fechaCreacion,
    }
}

// ========== Crowdpromotion - Promotor Mappers ==========

/**
 * Convierte datos del formulario de creacion al payload del API.
 * Los strings vacios ('') se convierten a undefined para no enviar
 * campos opcionales al backend y evitar errores de validacion de URL/email.
 */
export function mapCreateFormToRequest(
    formData: CreatePromotorFormData
): CreatePromotorRequest {
    return {
        nombrePublico: formData.nombrePublico,
        tipoPromotorId: formData.tipoPromotorId,
        emailContacto: formData.emailContacto || undefined,
        urlSitioWeb: formData.urlSitioWeb || undefined,
        urlInstagram: formData.urlInstagram || undefined,
        urlTikTok: formData.urlTikTok || undefined,
        urlYouTube: formData.urlYouTube || undefined,
        urlTwitter: formData.urlTwitter || undefined,
    };
}

/**
 * Convierte datos del formulario de edicion al payload del API.
 * Misma logica de conversion de strings vacios que en creacion.
 */
export function mapUpdateFormToRequest(
    formData: UpdatePromotorFormData
): UpdatePromotorRequest {
    return {
        nombrePublico: formData.nombrePublico,
        emailContacto: formData.emailContacto || undefined,
        urlSitioWeb: formData.urlSitioWeb || undefined,
        urlInstagram: formData.urlInstagram || undefined,
        urlTikTok: formData.urlTikTok || undefined,
        urlYouTube: formData.urlYouTube || undefined,
        urlTwitter: formData.urlTwitter || undefined,
    };
}

/**
 * Convierte el perfil completo del promotor a los valores iniciales
 * del formulario de edicion. Los null del backend se convierten a ''
 * para que React Hook Form pueda controlar los inputs como strings.
 */
export function mapPromotorToUpdateForm(
    promotor: Promotor
): UpdatePromotorFormData {
    return {
        nombrePublico: promotor.nombrePublico,
        emailContacto: promotor.emailContacto ?? '',
        urlSitioWeb: promotor.urlSitioWeb ?? '',
        urlInstagram: promotor.urlInstagram ?? '',
        urlTikTok: promotor.urlTikTok ?? '',
        urlYouTube: promotor.urlYouTube ?? '',
        urlTwitter: promotor.urlTwitter ?? '',
    };
}

/**
 * Devuelve el estado del promotor como string legible.
 */
export function getPromotorEstado(esActivo: boolean): PromotorEstado {
    return esActivo ? 'activo' : 'inactivo';
}

// ========== Crowdpromotion - Inscripcion Mappers (US-CP-03) ==========

/**
 * Deriva el estado de la inscripcion a partir de las flags booleanas.
 * Precedencia: Bloqueado > DadoDeBaja > Aprobado > Pendiente
 * Util para operaciones optimistas donde el frontend necesita
 * computar el estado antes de confirmar con el servidor.
 */
export function mapInscripcionEstado(data: {
    esAprobado: boolean;
    esBloqueado: boolean;
    fechaBaja: string | null;
}): InscripcionEstado {
    if (data.esBloqueado) return 'Bloqueado';
    if (data.fechaBaja != null) return 'DadoDeBaja';
    if (data.esAprobado) return 'Aprobado';
    return 'Pendiente';
}

/**
 * Devuelve el variant del Badge de shadcn/ui para el estado dado.
 * Valores: 'secondary' | 'default' | 'destructive' | 'outline'
 */
export function getInscripcionBadgeVariant(estado: InscripcionEstado): string {
    return INSCRIPCION_ESTADO_BADGE_VARIANT[estado] ?? 'secondary';
}

// ========== Crowdpromotion - Tareas de Promocion Mappers (US-CP-04) ==========

/**
 * Devuelve el variant del Badge de shadcn/ui para el estado de tarea dado.
 * Valores: 'secondary' (Pendiente), 'warning' (Completada), 'success' (Validada), 'destructive' (Rechazada)
 */
export function mapEstadoTareaPromoToBadge(estadoTareaId: number): string {
    return ESTADO_TAREA_PROMO_BADGES[estadoTareaId] ?? 'secondary';
}

/**
 * Determina si el promotor puede completar una tarea.
 * Encapsula las reglas de negocio RN-04 y RN-05:
 * - Tarea no repetible: solo puede si fue rechazada (re-envio)
 * - Tarea repetible con limite: puede si vecesCompletada < maxRepeticiones
 * - Tarea repetible sin limite: siempre puede
 * - Sin miEstado (nunca completada): siempre puede
 */
export function puedeCompletarTarea(item: MisTareasItem): boolean {
    if (!item.miEstado) return true;
    if (!item.esRepetible) {
        return item.miEstado.estadoTareaId === 4; // Solo puede si fue rechazada (re-envio)
    }
    if (item.maxRepeticiones != null) {
        return item.miEstado.vecesCompletada < item.maxRepeticiones;
    }
    return true; // Repetible sin limite
}

// ========== Crowdpromotion - Tracking y Metricas Mappers (US-CP-05) ==========

/** Labels de UI para tipos de evento de tracking (en espanol) */
const TIPO_EVENTO_TRACKING_LABELS: Record<number, string> = {
    1: 'Click',
    2: 'Vista',
    3: 'Registro',
    4: 'Backing',
    5: 'Share',
};

/**
 * Devuelve el label de UI para el tipo de evento de tracking.
 * Para tipos desconocidos devuelve 'Evento' como fallback.
 */
export function mapTipoEventoPromoToLabel(tipoEventoId: number): string {
    return TIPO_EVENTO_TRACKING_LABELS[tipoEventoId] ?? 'Evento';
}

/** Clases Tailwind para badges de tipo de evento (design system ui-ux.md) */
const TIPO_EVENTO_BADGE_CLASSES: Record<number, string> = {
    1: 'bg-blue-950/50 text-[#3b82f6] border border-blue-800/50 text-xs',
    2: 'bg-slate-900/50 text-[#94a3b8] border border-slate-700/50 text-xs',
    3: 'bg-green-950/50 text-[#10b981] border border-green-800/50 text-xs',
    4: 'bg-purple-950/50 text-[#a855f7] border border-purple-800/50 text-xs',
    5: 'bg-amber-950/50 text-[#f59e0b] border border-amber-800/50 text-xs',
};

const BADGE_CLASS_FALLBACK = 'bg-slate-900/50 text-[#94a3b8] border border-slate-700/50 text-xs';

/**
 * Devuelve la clase CSS Tailwind para el badge del tipo de evento.
 * Cubre los 5 tipos de tracking mas un fallback gris para desconocidos.
 */
export function mapTipoEventoPromoToBadgeClass(tipoEventoId: number): string {
    return TIPO_EVENTO_BADGE_CLASSES[tipoEventoId] ?? BADGE_CLASS_FALLBACK;
}

// ========== Crowdpromotion - Wallet Mappers (US-CP-06) ==========

/**
 * Devuelve el variant del Badge de shadcn/ui para el estado de transaccion.
 * Valores: 'secondary' (Pendiente), 'default' (Procesada), 'success' (Pagada), 'destructive' (Cancelada)
 */
export function mapEstadoWalletTransaccionToBadge(estadoTransaccionId: number): string {
    return ESTADO_WALLET_TRANSACCION_BADGES[estadoTransaccionId] ?? 'secondary';
}

/**
 * Devuelve el icono/direccion visual para una transaccion de wallet.
 * 'credit' para ingresos (esCredito=true), 'debit' para retiros (esCredito=false).
 * Los componentes usan esto para aplicar color verde/rojo y signo +/-.
 */
export function mapTransaccionTipoToDisplayProps(esCredito: boolean): {
    tipo: 'credit' | 'debit';
    signo: '+' | '-';
    colorClass: string;
} {
    return esCredito
        ? { tipo: 'credit', signo: '+', colorClass: 'text-green-500' }
        : { tipo: 'debit', signo: '-', colorClass: 'text-red-500' };
}
