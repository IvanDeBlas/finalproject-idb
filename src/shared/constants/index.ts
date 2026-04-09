import type { MisCampaniasQueryParams, BackingsQueryParams } from '../schemas/dashboard.schema';
import type { ExplorarProgramasFilters, InscripcionesFilters, WalletTransaccionesFilters } from '../types/crowdpromotion';

export const APP_NAME = "WePlay Rises";
export const APP_DESCRIPTION = "Plataforma de crowdfunding para artistas musicales";

// API - Each app defines its own API_BASE_URL in its api-client.ts
export const API_BASE_URL = "/api";

// Query Keys for TanStack Query (hierarchical for granular invalidation)
export const QUERY_KEYS = {
    // Auth
    auth: {
        currentUser: ["auth", "current-user"] as const,
    },

    // Artistas
    artistas: {
        all: ["artistas"] as const,
        byId: (id: string) => ["artistas", id] as const,
        byUserId: (userId: string) => ["artistas", "user", userId] as const,
    },

    // Campanias
    campanias: {
        all: ["campanias"] as const,
        byId: (id: string) => ["campanias", id] as const,
        byArtista: (artistaId: string) => ["campanias", "artista", artistaId] as const,
        stats: (id: string) => ["campanias", id, "stats"] as const,
        misCampanias: ["campanias", "mis-campanias"] as const,
        filtered: (filters: Record<string, unknown>) => ["campanias", "filtered", filters] as const,
    },

    // Rewards
    rewards: {
        all: ["rewards"] as const,
        byId: (id: string) => ["rewards", id] as const,
        byCampania: (campaniaId: string) => ["rewards", "campania", campaniaId] as const,
        filtered: (filters: Record<string, unknown>) => ["rewards", "filtered", filters] as const,
    },

    // Backings
    backings: {
        all: ["backings"] as const,
        byId: (id: string) => ["backings", id] as const,
        byCampania: (campaniaId: string) => ["backings", "campania", campaniaId] as const,
        me: ["backings", "me"] as const,
    },

    // Crowdsourcing
    crowdsourcing: {
        templates: {
            all: ['crowdsourcing', 'templates'] as const,
            byId: (id: string) => ['crowdsourcing', 'templates', id] as const,
        },
        maestras: {
            rolesProfesionales: ['crowdsourcing', 'maestras', 'roles'] as const,
            categoriasRol: ['crowdsourcing', 'maestras', 'categorias'] as const,
            tiposNecesidad: ['crowdsourcing', 'maestras', 'tipos-necesidad'] as const,
            modalidadesTrabajo: ['crowdsourcing', 'maestras', 'modalidades'] as const,
            monedas: ['crowdsourcing', 'maestras', 'monedas'] as const,
        },
        necesidades: {
            mis: ['crowdsourcing', 'necesidades', 'mis'] as const,
            byId: (id: string) => ['crowdsourcing', 'necesidades', id] as const,
            publicas: ['crowdsourcing', 'necesidades', 'publicas'] as const,
            publicaById: (id: string) => ['crowdsourcing', 'necesidades', 'publica', id] as const,
        },
        propuestas: {
            mis: ['crowdsourcing', 'propuestas', 'mis'] as const,
        },
        acuerdos: {
            byId: (id: string) => ['crowdsourcing', 'acuerdos', id] as const,
        },
        valoraciones: {
            byUserId: (userId: string) => ['crowdsourcing', 'valoraciones', userId] as const,
        },
        conversaciones: {
            lista: (filtro?: string) =>
                ['crowdsourcing', 'conversaciones', filtro ?? 'todas'] as const,
            mensajes: (id: string, page?: number) =>
                ['crowdsourcing', 'conversaciones', id, 'mensajes', page ?? 1] as const,
            noLeidos: ['crowdsourcing', 'conversaciones', 'no-leidos'] as const,
        },
    },

    // Crowdpromotion
    crowdpromotion: {
        promotor: {
            me: ['crowdpromotion', 'promotor', 'me'] as const,
        },
        maestras: {
            tiposPromotor: ['crowdpromotion', 'maestras', 'tipos-promotor'] as const,
            tiposPromo: ['crowdpromotion', 'maestras', 'tipos-promo'] as const,
            tiposEventoPromo: ['crowdpromotion', 'maestras', 'tipos-evento-promo'] as const,
            tiposRewardPromo: ['crowdpromotion', 'maestras', 'tipos-reward-promo'] as const,
        },
        programas: {
            mis: ['crowdpromotion', 'programas', 'mis'] as const,
            byId: (id: string) => ['crowdpromotion', 'programas', id] as const,
            misFiltrados: (filters: Record<string, unknown>) =>
                ['crowdpromotion', 'programas', 'mis', filters] as const,
            explorar: (filters?: ExplorarProgramasFilters) =>
                ['crowdpromotion', 'programas', 'explorar', filters] as const,
            inscripciones: (programaId: string, filters?: InscripcionesFilters) =>
                ['crowdpromotion', 'programas', programaId, 'inscripciones', filters] as const,
        },
        inscripciones: {
            misProgramas: (page?: number) =>
                ['crowdpromotion', 'inscripciones', 'mis-programas', page] as const,
        },
        tareas: {
            mis: (programaId: string) =>
                ['crowdpromotion', 'tareas', programaId, 'mis'] as const,
            pendientes: (programaId: string, params?: Record<string, unknown>) =>
                ['crowdpromotion', 'tareas', programaId, 'pendientes', params] as const,
        },
        metricas: {
            programa: (programaId: string, fechaDesde?: string, fechaHasta?: string) =>
                ['crowdpromotion', 'metricas', 'programa', programaId, fechaDesde, fechaHasta] as const,
            promotor: (programaId?: string, fechaDesde?: string, fechaHasta?: string) =>
                ['crowdpromotion', 'metricas', 'promotor', programaId, fechaDesde, fechaHasta] as const,
        },
        wallet: {
            resumen: ['crowdpromotion', 'wallet', 'resumen'] as const,
            transacciones: (filters?: WalletTransaccionesFilters) =>
                ['crowdpromotion', 'wallet', 'transacciones', filters] as const,
        },
    },

    // Dashboard
    dashboard: {
        resumen: ['dashboard', 'resumen'] as const,
        misCampanias: (params?: MisCampaniasQueryParams) =>
            ['dashboard', 'mis-campanias', params] as const,
        campaniaBackings: (campaniaId: string, params?: BackingsQueryParams) =>
            ['dashboard', 'campanias', campaniaId, 'backings', params] as const,
        campaniaStats: (campaniaId: string) =>
            ['dashboard', 'campanias', campaniaId, 'stats'] as const,
    },

    // Legacy keys (for backwards compatibility)
    AUTH_USER: "auth-user",
    CAMPANIAS: "campanias",
    CAMPANIA: "campania",
    CAMPANIAS_ARTISTA: "campanias-artista",
    CAMPANIA_STATS: "campania-stats",
    ARTISTAS: "artistas",
    ARTISTA: "artista",
    ARTISTA_ME: "artista-me",
    REWARDS: "rewards",
    REWARD: "reward",
    BACKINGS: "backings",
    BACKING: "backing",
    BACKINGS_ME: "backings-me",
} as const;

// API Routes (backend endpoints)
export const API_ROUTES = {
    auth: {
        register: "/auth/register",
        login: "/auth/login",
    },
    artistas: {
        base: "/artistas",
        byId: (id: string) => `/artistas/${id}`,
        byUserId: (userId: string) => `/artistas/by-user/${userId}`,
    },
    campanias: {
        base: "/campanias",
        byId: (id: string) => `/campanias/${id}`,
        publicar: (id: string) => `/campanias/${id}/publicar`,
        misCampanias: "/campanias/mis-campanias",
        stats: (id: string) => `/campanias/${id}/stats`,
        backings: (id: string) => `/campanias/${id}/backings`,
    },
    rewards: {
        base: "/rewards",
        byId: (id: string) => `/rewards/${id}`,
        reorder: "/rewards/reorder",
    },
    backings: {
        base: "/backings",
        byId: (id: string) => `/backings/${id}`,
        create: (campaniaId: string) => `/campanias/${campaniaId}/backings`,
        byCampania: (campaniaId: string) => `/campanias/${campaniaId}/backings`,
    },
    crowdsourcing: {
        templates: {
            base: '/crowdsourcing/templates',
            byId: (id: string) => `/crowdsourcing/templates/${id}`,
            generar: (id: string) => `/crowdsourcing/templates/${id}/generar`,
        },
        maestras: {
            rolesProfesionales: '/crowdsourcing/maestras/roles-profesionales',
            categoriasRol: '/crowdsourcing/maestras/categorias-rol',
            tiposNecesidad: '/crowdsourcing/maestras/tipos-necesidad',
            modalidadesTrabajo: '/crowdsourcing/maestras/modalidades-trabajo',
            monedas: '/crowdsourcing/maestras/monedas',
        },
        necesidades: {
            base: '/crowdsourcing/necesidades',
            mis: '/crowdsourcing/necesidades/mis-necesidades',
            byId: (id: string) => `/crowdsourcing/necesidades/${id}`,
            cerrar: (id: string) => `/crowdsourcing/necesidades/${id}/cerrar`,
            propuestas: (necesidadId: string) =>
                `/crowdsourcing/necesidades/${necesidadId}/propuestas`,
        },
        propuestas: {
            mis: '/crowdsourcing/propuestas/mis-propuestas',
            retirar: (id: string) => `/crowdsourcing/propuestas/${id}/retirar`,
            aceptar: (id: string) => `/crowdsourcing/propuestas/${id}/aceptar`,
            rechazar: (id: string) => `/crowdsourcing/propuestas/${id}/rechazar`,
        },
        acuerdos: {
            byId: (id: string) => `/crowdsourcing/acuerdos/${id}`,
            completar: (id: string) => `/crowdsourcing/acuerdos/${id}/completar`,
            cancelar: (id: string) => `/crowdsourcing/acuerdos/${id}/cancelar`,
            milestones: (acuerdoId: string) => `/crowdsourcing/acuerdos/${acuerdoId}/milestones`,
            milestoneById: (acuerdoId: string, id: string) => `/crowdsourcing/acuerdos/${acuerdoId}/milestones/${id}`,
            entregables: (acuerdoId: string) => `/crowdsourcing/acuerdos/${acuerdoId}/entregables`,
        },
        entregables: {
            aprobar: (id: string) => `/crowdsourcing/entregables/${id}/aprobar`,
            rechazar: (id: string) => `/crowdsourcing/entregables/${id}/rechazar`,
        },
        valoraciones: {
            create: (acuerdoId: string) =>
                `/crowdsourcing/acuerdos/${acuerdoId}/valoraciones`,
            byUser: (userId: string) =>
                `/crowdsourcing/usuarios/${userId}/valoraciones`,
        },
        conversaciones: {
            base: '/crowdsourcing/conversaciones',
            noLeidos: '/crowdsourcing/conversaciones/no-leidos',
            mensajes: (id: string) =>
                `/crowdsourcing/conversaciones/${id}/mensajes`,
            marcarLeidos: (id: string) =>
                `/crowdsourcing/conversaciones/${id}/marcar-leidos`,
        },
    },
    crowdpromotion: {
        promotor: {
            base: '/crowdpromotion/promotor',
            me: '/crowdpromotion/promotor/me',
            desactivar: '/crowdpromotion/promotor/me/desactivar',
        },
        maestras: {
            tiposPromotor: '/crowdpromotion/maestras/tipos-promotor',
            tiposPromo: '/crowdpromotion/maestras/tipos-promo',
            tiposEventoPromo: '/crowdpromotion/maestras/tipos-evento-promo',
            tiposRewardPromo: '/crowdpromotion/maestras/tipos-reward-promo',
        },
        programas: {
            base: '/crowdpromotion/programas',
            mis: '/crowdpromotion/programas/mis-programas',
            byId: (id: string) => `/crowdpromotion/programas/${id}`,
            desactivar: (id: string) => `/crowdpromotion/programas/${id}/desactivar`,
            explorar: '/crowdpromotion/programas/explorar',
            inscripcion: (programaId: string) => `/crowdpromotion/programas/${programaId}/inscripcion`,
            inscripciones: (programaId: string) => `/crowdpromotion/programas/${programaId}/inscripciones`,
            aprobar: (programaId: string, inscripcionId: string) => `/crowdpromotion/programas/${programaId}/inscripciones/${inscripcionId}/aprobar`,
            rechazar: (programaId: string, inscripcionId: string) => `/crowdpromotion/programas/${programaId}/inscripciones/${inscripcionId}/rechazar`,
            bloquear: (programaId: string, inscripcionId: string) => `/crowdpromotion/programas/${programaId}/inscripciones/${inscripcionId}/bloquear`,
            darDeBaja: (programaId: string, inscripcionId: string) => `/crowdpromotion/programas/${programaId}/inscripciones/${inscripcionId}/dar-de-baja`,
            misTareas: (programaId: string) => `/crowdpromotion/programas/${programaId}/mis-tareas`,
            completarTarea: (programaId: string, tareaId: string) => `/crowdpromotion/programas/${programaId}/tareas/${tareaId}/completar`,
            tareasPendientes: (programaId: string) => `/crowdpromotion/programas/${programaId}/tareas-pendientes`,
            validarTarea: (programaId: string, tareaPromotorId: string) => `/crowdpromotion/programas/${programaId}/tareas-promotor/${tareaPromotorId}/validar`,
            rechazarTarea: (programaId: string, tareaPromotorId: string) => `/crowdpromotion/programas/${programaId}/tareas-promotor/${tareaPromotorId}/rechazar`,
        },
        promotorInscripciones: {
            misProgramas: '/crowdpromotion/promotor/mis-programas',
        },
        tracking: {
            evento: '/crowdpromotion/tracking/evento',
        },
        programaMetricas: (programaId: string) => `/crowdpromotion/programas/${programaId}/metricas`,
        promotorMetricas: '/crowdpromotion/promotor/metricas',
        promotorWallet: {
            resumen: '/crowdpromotion/promotor/wallet',
            transacciones: '/crowdpromotion/promotor/wallet/transacciones',
            cobro: '/crowdpromotion/promotor/wallet/cobro',
        },
    },
    dashboard: {
        resumen: '/dashboard/resumen',
        misCampanias: '/campanias/mis-campanias',
        campaniaBackings: (campaniaId: string) => `/campanias/${campaniaId}/backings`,
        campaniaStats: (campaniaId: string) => `/campanias/${campaniaId}/stats`,
    },
} as const;

// App Routes (frontend navigation)
export const APP_ROUTES = {
    auth: {
        register: "/auth/register",
        login: "/auth/login",
    },
    artista: {
        crearPerfil: "/artista/perfil/crear",
    },
    dashboard: {
        root: "/dashboard",
        campanias: {
            list: "/dashboard/campanias",
            nueva: "/dashboard/campanias/nueva",
            editar: (id: string) => `/dashboard/campanias/${id}/editar`,
            detalle: (id: string) => `/dashboard/campanias/${id}`,
            backings: (id: string) => `/dashboard/campanias/${id}/backings`,
        },
        crowdsourcing: {
            templates: '/dashboard/crowdsourcing/templates',
            templateDetail: (id: string) => `/dashboard/crowdsourcing/templates/${id}`,
            wizard: (projectId: string) => `/dashboard/crowdsourcing/wizard/${projectId}`,
            necesidades: '/dashboard/crowdsourcing/necesidades',
            nuevaNecesidad: '/dashboard/crowdsourcing/necesidades/nueva',
            necesidadDetail: (id: string) => `/dashboard/crowdsourcing/necesidades/${id}`,
            editarNecesidad: (id: string) => `/dashboard/crowdsourcing/necesidades/${id}/editar`,
        },
        rewards: {
            list: (campaniaId: string) => `/dashboard/campanias/${campaniaId}/recompensas`,
            nueva: (campaniaId: string) => `/dashboard/campanias/${campaniaId}/recompensas/nueva`,
            editar: (campaniaId: string, rewardId: string) =>
                `/dashboard/campanias/${campaniaId}/recompensas/${rewardId}/editar`,
        },
        crowdpromotion: {
            programas: {
                list: '/dashboard/crowdpromotion/programas',
                nuevo: '/dashboard/crowdpromotion/programas/nuevo',
                detalle: (id: string) => `/dashboard/crowdpromotion/programas/${id}`,
                editar: (id: string) => `/dashboard/crowdpromotion/programas/${id}/editar`,
                inscripciones: (programaId: string) => `/dashboard/crowdpromotion/programas/${programaId}/inscripciones`,
            },
        },
    },
    landing: {
        home: "/",
        explorar: "/explorar",
        artistaById: (id: string) => `/artistas/${id}`,
        campaniaById: (id: string) => `/campanias/${id}`,
        campanias: {
            list: "/campanias",
            detail: (id: string) => `/campanias/${id}`,
            apoyar: (id: string) => `/campanias/${id}/apoyar`,
        },
        promotor: {
            registro: '/promotor/registro',
            dashboard: '/promotor/dashboard',
            perfil: '/promotor/perfil',
            wallet: '/promotor/wallet',
        },
        crowdpromotion: {
            explorar: '/crowdpromotion/explorar',
            misProgramas: '/promotor/mis-programas',
            inscripcionDetalle: (inscripcionId: string) => `/promotor/mis-programas/${inscripcionId}`,
        },
        crowdsourcing: {
            templates: '/crowdsourcing/nuevo-proyecto',
            wizard: '/crowdsourcing/nuevo-proyecto',
            necesidades: '/crowdsourcing/necesidades',
            necesidadDetail: (id: string) => `/crowdsourcing/necesidades/${id}`,
            misPropuestas: '/crowdsourcing/mis-propuestas',
            acuerdoDetail: (id: string) => `/crowdsourcing/acuerdos/${id}`,
            mensajes: '/crowdsourcing/mensajes',
            mensajeDetail: (id: string) => `/crowdsourcing/mensajes/${id}`,
        },
    },
} as const;

// Campania estados (numeric, aligned with backend)
export const CAMPANIA_ESTADOS = {
    BORRADOR: 1,
    PUBLICADA: 2,
    FINALIZADA: 3,
    CANCELADA: 4,
    PAUSADA: 5,
} as const;

export const CAMPANIA_ESTADOS_LABELS: Record<number, string> = {
    1: "Borrador",
    2: "Publicada",
    3: "Finalizada",
    4: "Cancelada",
    5: "Pausada",
};

export const CAMPANIA_ESTADO_COLORS: Record<number, string> = {
    1: "yellow",
    2: "green",
    3: "blue",
    4: "red",
    5: "gray",
};

// Estado de pedido (backing order status)
export const ESTADO_PEDIDO = {
    PENDIENTE: 1,
    PROCESANDO: 2,
    COMPLETADO: 3,
    FALLIDO: 4,
    CANCELADO: 5,
} as const;

export const ESTADO_PEDIDO_LABELS: Record<number, string> = {
    1: "Pendiente",
    2: "Procesando",
    3: "Completado",
    4: "Fallido",
    5: "Cancelado",
};

// Tipo financiacion
export const TIPO_FINANCIACION = {
    TODO_O_NADA: 1,
    FLEXIBLE: 2,
} as const;

export const TIPO_FINANCIACION_LABELS: Record<number, string> = {
    1: "Todo o Nada",
    2: "Meta Flexible",
};

export const TIPO_FINANCIACION_DESCRIPTIONS: Record<number, string> = {
    1: "Solo recibiras los fondos si alcanzas tu meta",
    2: "Recibiras los fondos recaudados aunque no alcances tu meta",
};

// Tipos de recompensa (aligned with backend TipoReward catalog)
export const TIPO_REWARD = {
    DIGITAL: 1,
    FISICO: 2,
    EXPERIENCIA: 3,
    OTRO: 4,
} as const;

export const TIPO_REWARD_LABELS: Record<number, string> = {
    1: "Digital",
    2: "Fisico",
    3: "Experiencia",
    4: "Otro",
};

export const TIPO_REWARD_DESCRIPTIONS: Record<number, string> = {
    1: "Descarga digital, streaming, acceso online",
    2: "CD, vinilo, merchandising, productos fisicos",
    3: "Conciertos privados, meet & greet, workshops",
    4: "Otras recompensas personalizadas",
};

// Monedas
export const MONEDAS = {
    EUR: 1,
    USD: 2,
} as const;

export const MONEDA_SYMBOLS: Record<number, string> = {
    1: "€",
    2: "$",
};

export const MONEDA_CODES: Record<number, string> = {
    1: "EUR",
    2: "USD",
};

// Validation limits
export const VALIDATION = {
    // Artista
    NOMBRE_ARTISTICO_MAX: 200,
    DESCRIPCION_ARTISTA_MAX: 2000,
    PAIS_MAX: 100,
    CIUDAD_MAX: 100,

    // Campania
    TITULO_MAX: 200,
    SUBTITULO_MAX: 300,
    DESCRIPCION_CORTA_MAX: 500,
    DESCRIPCION_MAX: 5000,
    URL_MAX: 500,
    IMPORTE_MIN: 100,
    IMPORTE_MAX: 1000000,

    // Auth
    PASSWORD_MIN: 6,
    EMAIL_MAX: 256,

    // Promotor
    NOMBRE_PUBLICO_MIN: 3,
    NOMBRE_PUBLICO_MAX: 200,
    EMAIL_CONTACTO_MAX: 200,
    URL_PROMOTOR_MAX: 300,

    // Programa de Promocion
    PROGRAMA_TITULO_MIN: 5,
    PROGRAMA_TITULO_MAX: 200,
    PROGRAMA_DESCRIPCION_MAX: 4000,
    PROGRAMA_URL_LANDING_MAX: 500,
    PROGRAMA_CODIGO_TRACKING_MAX: 50,
    TAREA_TITULO_MIN: 3,
    TAREA_TITULO_MAX: 200,
    TAREA_DESCRIPCION_MAX: 4000,
    TAREA_URL_INSTRUCCIONES_MAX: 500,
    COMISION_PORCENTAJE_MAX: 100,
    PROGRAMAS_PAGE_SIZE_MAX: 50,

    // Inscripcion a Programa
    INSCRIPCION_NOMBRE_ARTISTA_FILTRO_MAX: 200,
    INSCRIPCION_PAGE_SIZE_MAX: 50,

    // Tareas de Promocion (US-CP-04)
    TAREA_URL_PRUEBA_MAX: 2048,
    TAREA_COMENTARIO_PROMOTOR_MAX: 500,
    TAREA_COMENTARIO_VALIDACION_MAX: 500,

    // Tracking y Metricas (US-CP-05)
    TRACKING_CODIGO_REFERIDO_MAX: 50,
    TRACKING_URL_MAX: 2048,
    TRACKING_UTM_MAX: 100,
    METRICAS_EVENTOS_RECIENTES_MAX: 20,

    // Wallet y Cobros (US-CP-06)
    WALLET_DESCRIPCION_COBRO_MAX: 500,
    WALLET_TRANSACCIONES_PAGE_SIZE_MAX: 50,
} as const;

// Generos musicales
export const GENEROS_MUSICALES = [
    "Rock",
    "Pop",
    "Hip Hop",
    "Electronica",
    "Jazz",
    "Clasica",
    "Folk",
    "Indie",
    "Metal",
    "Reggaeton",
    "Flamenco",
    "Otro",
] as const;

// ========== Crowdsourcing Domain Constants ==========

export const PRIORIDAD_NECESIDAD = {
    ALTA: 'Alta',
    MEDIA: 'Media',
    BAJA: 'Baja',
} as const;

export const PRIORIDAD_NECESIDAD_LABELS: Record<string, string> = {
    Alta: 'Alta prioridad',
    Media: 'Prioridad media',
    Baja: 'Prioridad baja',
};

export const PRIORIDAD_NECESIDAD_COLORS: Record<string, string> = {
    Alta: 'red',
    Media: 'yellow',
    Baja: 'blue',
};

export const MODALIDAD_COBRO = {
    POR_PROYECTO: 'Por proyecto',
    POR_DIA: 'Por dia',
    POR_HORA: 'Por hora',
    POR_CANCION: 'Por cancion',
    POR_MES: 'Por mes',
    POR_SESION: 'Por sesion',
    POR_VIDEO: 'Por video',
} as const;

export const FASES_PROYECTO = [
    'Preproduccion',
    'Grabacion',
    'Mezcla y Master',
    'Diseno y Produccion',
    'Promocion',
    'Distribucion',
] as const;

// ========== Estado de Necesidad (int IDs from maestras) ==========

export const ESTADO_NECESIDAD = {
    ABIERTA: 1,
    EN_PROGRESO: 2,
    CERRADA: 3,
    CANCELADA: 4,
} as const;

export const ESTADO_NECESIDAD_LABELS: Record<number, string> = {
    1: 'Abierta',
    2: 'En Progreso',
    3: 'Cerrada',
    4: 'Cancelada',
};

export const ESTADO_NECESIDAD_BADGES: Record<number, string> = {
    1: 'green',
    2: 'blue',
    3: 'gray',
    4: 'red',
};

// ========== Estado de Propuesta (int IDs from maestras) ==========

export const ESTADO_PROPUESTA = {
    PENDIENTE: 1,
    ACEPTADA: 2,
    RECHAZADA: 3,
    RETIRADA: 4,
} as const;

export const ESTADO_PROPUESTA_LABELS: Record<number, string> = {
    1: 'Pendiente',
    2: 'Aceptada',
    3: 'Rechazada',
    4: 'Retirada',
};

export const ESTADO_PROPUESTA_BADGES: Record<number, string> = {
    1: 'yellow',   // Pendiente
    2: 'green',    // Aceptada
    3: 'red',      // Rechazada
    4: 'gray',     // Retirada
};

// ========== Ordenamiento de Necesidades Publicas ==========

export const ORDER_BY_NECESIDADES = {
    RECIENTES: 'recientes',
    MAYOR_PRESUPUESTO: 'mayor-presupuesto',
    FECHA_LIMITE: 'fecha-limite',
} as const;

export const ORDER_BY_NECESIDADES_LABELS: Record<string, string> = {
    recientes: 'Mas recientes',
    'mayor-presupuesto': 'Mayor presupuesto',
    'fecha-limite': 'Fecha limite proxima',
};

/** Dias restantes desde hoy para considerar una necesidad como urgente */
export const URGENCIA_DIAS_UMBRAL = 3;

// ========== Modalidad de Trabajo (int IDs from maestras) ==========

export const MODALIDAD_TRABAJO = {
    PRESENCIAL: 1,
    REMOTO: 2,
    HIBRIDO: 3,
} as const;

export const MODALIDAD_TRABAJO_LABELS: Record<number, string> = {
    1: 'Presencial',
    2: 'Remoto',
    3: 'Hibrido',
};

// ========== Monedas para Necesidades ==========

export const MONEDA = {
    EUR: 1,
    USD: 2,
    GBP: 3,
} as const;

export const MONEDA_SIMBOLOS: Record<number, string> = {
    1: '\u20AC',
    2: '$',
    3: '\u00A3',
};

// ========== Estado de Acuerdo (US-CS-04) ==========

export const ESTADO_ACUERDO = {
    ACTIVO: 1,
    COMPLETADO: 2,
    CANCELADO: 3,
} as const;

export const ESTADO_ACUERDO_LABELS: Record<number, string> = {
    1: 'Activo',
    2: 'Completado',
    3: 'Cancelado',
};

export const ESTADO_ACUERDO_BADGES: Record<number, string> = {
    1: 'blue',
    2: 'green',
    3: 'gray',
};

// ========== Estado de Entregable (US-CS-04) ==========

export const ESTADO_ENTREGABLE = {
    ENTREGADO: 1,
    APROBADO: 2,
    RECHAZADO: 3,
} as const;

export const ESTADO_ENTREGABLE_LABELS: Record<number, string> = {
    1: 'Entregado',
    2: 'Aprobado',
    3: 'Rechazado',
};

export const ESTADO_ENTREGABLE_BADGES: Record<number, string> = {
    1: 'yellow',
    2: 'green',
    3: 'red',
};

/** @deprecated Use CAMPANIA_ESTADOS with numeric values instead */
export const CAMPANIA_ESTADOS_LEGACY = {
    BORRADOR: "borrador",
    ACTIVA: "activa",
    FINALIZADA: "finalizada",
    CANCELADA: "cancelada",
} as const;

// ========== Mensajeria Crowdsourcing (US-CS-05) ==========

/** Intervalo de polling para mensajeria (MVP sin WebSocket). En milisegundos. */
export const MENSAJERIA_POLLING_INTERVAL_MS = 10000;

/** Longitud maxima del preview del ultimo mensaje en el listado de conversaciones. */
export const MENSAJERIA_PREVIEW_MAX_LENGTH = 80;

export const FILTRO_CONVERSACION = {
    TODAS: 'todas',
    NECESIDADES: 'necesidades',
    ACUERDOS: 'acuerdos',
} as const;

// ========== Crowdpromotion - Tipo de Promotor ==========
// Valores de seed de MaestraTipoPromotor. IDs fijos en MVP.

export const TIPO_PROMOTOR = {
    FAN_EMBAJADOR:           1,
    INFLUENCER:              2,
    MEDIO_BLOG:              3,
    PROFESIONAL_MARKETING:   4,
} as const;

export const TIPO_PROMOTOR_LABELS: Record<number, string> = {
    1: 'Fan Embajador',
    2: 'Influencer',
    3: 'Medio / Blog',
    4: 'Profesional Marketing',
};

export const TIPO_PROMOTOR_DESCRIPTIONS: Record<number, string> = {
    1: 'Fan que promueve artistas por pasion y por recompensas',
    2: 'Creador de contenido con audiencia en redes sociales',
    3: 'Medio de comunicacion, blog o podcast musical',
    4: 'Profesional del marketing digital o musical',
};

// ========== Crowdpromotion - Tipo de Programa de Promocion ==========

export const TIPO_PROMO = {
    REFERRAL:   1,
    AFILIADO:   2,
    INFLUENCER: 3,
    MIXTO:      4,
} as const;

export const TIPO_PROMO_LABELS: Record<number, string> = {
    1: 'Referral',
    2: 'Afiliado',
    3: 'Influencer',
    4: 'Mixto',
};

export const TIPO_PROMO_DESCRIPTIONS: Record<number, string> = {
    1: 'Programa de referidos: comision por cada nuevo backer referido',
    2: 'Programa de afiliados: comision por ventas generadas',
    3: 'Programa para influencers: tareas de contenido con recompensa',
    4: 'Combinacion de referral + tareas de contenido',
};

// ========== Crowdpromotion - Tipo de Evento de Promocion ==========

export const TIPO_EVENTO_PROMO = {
    CLICK:     1,
    PAGE_VIEW: 2,
    SHARE:     3,
    POST:      4,
    SIGNUP:    5,
    BACKING:   6,
} as const;

export const TIPO_EVENTO_PROMO_LABELS: Record<number, string> = {
    1: 'Click',
    2: 'PageView',
    3: 'Share',
    4: 'Post',
    5: 'Signup',
    6: 'Backing',
};

export const TIPO_EVENTO_PROMO_DESCRIPTIONS: Record<number, string> = {
    1: 'Click en enlace de referido',
    2: 'Visita a la pagina de la campana',
    3: 'Compartir en redes sociales',
    4: 'Publicacion original sobre la campana',
    5: 'Registro de nuevo usuario referido',
    6: 'Aportacion/backing a la campana (conversion)',
};

// ========== Crowdpromotion - Tipo de Reward de Promocion ==========
// Nota: TIPO_REWARD_PROMO (no TIPO_REWARD) para no colisionar con el
// TIPO_REWARD existente del modulo de campanias (Digital, Fisico, Experiencia, Otro).

export const TIPO_REWARD_PROMO = {
    DINERO: 1,
    PUNTOS: 2,
    MIXTO:  3,
} as const;

export const TIPO_REWARD_PROMO_LABELS: Record<number, string> = {
    1: 'Dinero',
    2: 'Puntos',
    3: 'Mixto',
};

export const TIPO_REWARD_PROMO_DESCRIPTIONS: Record<number, string> = {
    1: 'Recompensa monetaria',
    2: 'Recompensa en puntos canjeables',
    3: 'Dinero + puntos',
};

// ========== Crowdpromotion - Estado de Inscripcion (US-CP-03) ==========

export const INSCRIPCION_ESTADO = {
    PENDIENTE: 'Pendiente',
    APROBADO: 'Aprobado',
    BLOQUEADO: 'Bloqueado',
    DADO_DE_BAJA: 'DadoDeBaja',
} as const;

export const INSCRIPCION_ESTADO_LABELS: Record<string, string> = {
    Pendiente: 'Pendiente de aprobacion',
    Aprobado: 'Aprobado',
    Bloqueado: 'Bloqueado',
    DadoDeBaja: 'Dado de baja',
};

export const INSCRIPCION_ESTADO_BADGE_VARIANT: Record<string, string> = {
    Pendiente: 'secondary',
    Aprobado: 'default',
    Bloqueado: 'destructive',
    DadoDeBaja: 'outline',
};

// ========== Crowdpromotion - Estado de Tarea de Promocion (US-CP-04) ==========

export const ESTADO_TAREA_PROMO = {
    PENDIENTE: 1,
    COMPLETADA: 2,
    VALIDADA: 3,
    RECHAZADA: 4,
} as const;

export const ESTADO_TAREA_PROMO_LABELS: Record<number, string> = {
    1: 'Pendiente',
    2: 'Completada',
    3: 'Validada',
    4: 'Rechazada',
};

export const ESTADO_TAREA_PROMO_BADGES: Record<number, string> = {
    1: 'secondary',
    2: 'warning',
    3: 'success',
    4: 'destructive',
};

export const TAREAS_PENDIENTES_DEFAULT_PAGE_SIZE = 10;
export const TAREAS_PENDIENTES_MAX_PAGE_SIZE = 50;

// ========== Crowdpromotion - Estado de Transaccion de Wallet (US-CP-06) ==========

export const ESTADO_WALLET_TRANSACCION = {
    PENDIENTE: 1,
    PROCESADA: 2,
    PAGADA: 3,
    CANCELADA: 4,
} as const;

export const ESTADO_WALLET_TRANSACCION_LABELS: Record<number, string> = {
    1: 'Pendiente',
    2: 'Procesada',
    3: 'Pagada',
    4: 'Cancelada',
};

export const ESTADO_WALLET_TRANSACCION_BADGES: Record<number, string> = {
    1: 'secondary',
    2: 'default',
    3: 'success',
    4: 'destructive',
};

export const MIN_RETIRO_WALLET = 10.00;
export const WALLET_DEFAULT_PAGE_SIZE = 10;

// ========== Tracking y Metricas - Re-exports (US-CP-05) ==========

export * from './tracking';
