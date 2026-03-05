// ========== Tracking y Metricas - Constantes de Sesion (US-CP-05) ==========

/** Nombres de parametros de URL que useTrackingInterceptor lee de window.location.search */
export const TRACKING_PARAMS = {
    ref: 'ref',
    utmSource: 'utm_source',
    utmMedium: 'utm_medium',
    utmCampaign: 'utm_campaign',
} as const;

/** Claves de sessionStorage para persistir contexto de tracking durante la sesion */
export const TRACKING_STORAGE_KEYS = {
    ref: 'wp_ref',
    utmSource: 'wp_utm_source',
    utmMedium: 'wp_utm_medium',
    utmCampaign: 'wp_utm_campaign',
    campaniaId: 'wp_campania_id',
} as const;

/** Nombre de la cookie de 30 min para persistir codigoReferido entre tabs */
export const TRACKING_COOKIE_KEY = 'wp_ref';

/** TTL de la cookie en minutos */
export const TRACKING_COOKIE_TTL_MINUTES = 30;

/** Valor fijo de utm_source para todos los eventos de la plataforma */
export const UTM_SOURCE_WEPLAY = 'weplay';

/** Valor fijo de utm_medium */
export const UTM_MEDIUM_REFERRAL = 'referral';
