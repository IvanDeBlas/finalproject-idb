export const APP_NAME = "WePlay Rises"
export const APP_DESCRIPTION = "Plataforma de crowdfunding para artistas musicales"

export const ROUTES = {
    HOME: "/",
    LOGIN: "/auth/login",
    REGISTER: "/auth/register",
    DASHBOARD: "/dashboard",
    CAMPANIAS: "/campanias",
    CAMPANIA_DETAIL: "/campanias/:id",
    BACKING_CONFIRMATION: "/campanias/:id/confirmacion",
    CAMPANIA_NEW: "/dashboard/campanias/nueva",
    EXPLORAR: "/explorar",
    ARTISTA_PERFIL: "/artista/perfil",
    CROWDSOURCING_NECESIDADES: "/crowdsourcing/necesidades",
    CROWDSOURCING_NECESIDAD_DETAIL: "/crowdsourcing/necesidades/:id",
    CROWDSOURCING_MIS_PROPUESTAS: "/crowdsourcing/mis-propuestas",
    CROWDSOURCING_ACUERDO_DETAIL: "/crowdsourcing/acuerdos/:id",
} as const

export const QUERY_KEYS = {
    CAMPANIAS: "campanias",
    CAMPANIA: "campania",
    ARTISTAS: "artistas",
    ARTISTA: "artista",
    BACKINGS: "backings",
    REWARDS: "rewards",
    AUTH_USER: "auth-user",
} as const

export const CAMPANIA_ESTADOS = {
    BORRADOR: 1,
    PUBLICADA: 2,
    FINALIZADA: 3,
    CANCELADA: 4,
} as const

export const CAMPANIA_ESTADOS_LABELS: Record<number, string> = {
    1: "Borrador",
    2: "Activa",
    3: "Finalizada",
    4: "Cancelada",
}

export const TIPO_FINANCIACION_LABELS: Record<number, string> = {
    1: "Todo o Nada",
    2: "Meta Flexible",
}

export const MONEDA_SYMBOLS: Record<number, string> = {
    1: "\u20AC",
    2: "$",
}

export const MONEDA_CODES: Record<number, string> = {
    1: "EUR",
    2: "USD",
}

export const DEFAULT_PAGE_SIZE = 12
