/**
 * Mapeo de codigos de error backend a mensajes amigables para el usuario
 * Alineado con ServiceResponseMessageType del backend
 */

// Numeric error code mappings (aligned with backend ServiceResponseMessageType)
export const ERROR_CODE_MESSAGES: Record<string, string> = {
    // Success codes (0000-0999)
    "0000": "Operacion exitosa",
    "0001": "Creado exitosamente",
    "0002": "Actualizado exitosamente",
    "0003": "Eliminado exitosamente",

    // Validation errors (1000-1999)
    "1001": "Este campo es obligatorio",
    "1002": "El valor supera el maximo de caracteres permitido",
    "1003": "El valor no cumple con el minimo requerido",
    "1004": "El formato del valor no es valido",
    "1005": "El formato del email no es valido",
    "1006": "La URL proporcionada no es valida",
    "1007": "El valor esta fuera del rango permitido",
    "1008": "Este nombre ya esta en uso",
    "1009": "Este email ya esta registrado",
    "1010": "La referencia proporcionada no existe",
    "1011": "El monto no es valido",
    "1012": "La fecha no es valida",
    "1013": "La URL proporcionada no es valida",
    "1020": "Debe definir al menos una comision (porcentaje o fija)",
    "1021": "El valor esta fuera del rango permitido",
    "1022": "La fecha fin debe ser posterior a la fecha inicio",
    "1023": "El formato del campo no es valido",
    "1024": "Este codigo de tracking ya esta en uso. Elige otro",
    "1025": "Las tareas repetibles deben tener un numero maximo de repeticiones",
    "1026": "El importe de recompensa es requerido para recompensas monetarias",
    "1027": "Los puntos de recompensa son requeridos para recompensas de puntos",

    // Not Found errors (2000-2999)
    "2000": "Recurso no encontrado",
    "2002": "Artista no encontrado",
    "2003": "Campania no encontrada",
    "2004": "Recompensa no encontrada",
    "2005": "Aporte no encontrado",
    "2011": "Acuerdo no encontrado",
    "2012": "Milestone no encontrado",
    "2013": "Entregable no encontrado",
    "2014": "La conversacion no fue encontrada",
    "2015": "No tienes un perfil de promotor. Registrate primero.",
    "2016": "No tienes un perfil de artista. Crea tu perfil primero",
    "2019": "El programa de promocion no existe o fue eliminado",
    "2020": "La inscripcion no existe.",
    "2021": "La tarea no existe o no pertenece a este programa.",
    "2022": "El completado no existe o no pertenece a este programa.",
    "2030": "No tienes un wallet asociado. Completa tu primera actividad para activarlo.",

    // Auth errors (3000-3999)
    "3001": "No autorizado. Por favor, inicia sesion nuevamente",
    "3002": "No tienes permiso para realizar esta accion",
    "3003": "Tu sesion ha expirado. Por favor, inicia sesion nuevamente",
    "3004": "Token invalido",
    "3005": "Debes iniciar sesion para continuar",

    // Business Rule errors (4000-4999)
    "4001": "Este registro ya existe",
    "4002": "El estado actual no permite esta operacion",
    "4003": "Esta operacion no esta permitida",
    "4004": "Se ha excedido el limite permitido",
    "4005": "Fondos insuficientes",
    "4006": "La campania no esta activa",
    "4007": "La campania ha finalizado",
    "4009": "Solo se pueden editar campanias en estado borrador",
    "4010": "No se puede eliminar una recompensa con aportes existentes",
    "4011": "Esta recompensa esta agotada",
    "4012": "El monto esta por debajo del minimo requerido para esta recompensa",
    "4013": "Esta campania no permite aportes anonimos",
    "4014": "Ya has enviado una valoracion para este acuerdo",
    "4015": "Ya existe una conversacion para este contexto",
    "4016": "No tienes relacion con este destinatario",
    "4018": "Ya tienes un perfil de promotor creado.",
    "4019": "Tu perfil de promotor ya esta desactivado.",
    "4020": "Este programa ya esta desactivado",
    "4021": "No se puede desactivar una tarea que ya tiene completados registrados",
    "4022": "No puedes inscribirte en este programa.",
    "4023": "Tu perfil de promotor esta desactivado. Reactiva tu perfil para inscribirte.",
    "4024": "Este programa no esta activo en este momento.",
    "4025": "La inscripcion no se puede modificar en su estado actual.",
    "4026": "No tienes permiso para gestionar las inscripciones de este programa.",
    "4027": "Ya completaste esta tarea. No se puede volver a completar porque no es repetible.",
    "4028": "Has alcanzado el numero maximo de veces que puedes completar esta tarea.",
    "4029": "Esta tarea ya no esta disponible.",
    "4030": "El plazo para completar esta tarea ha finalizado.",
    "4031": "Este completado no puede ser procesado porque ya fue validado o rechazado.",

    // Crowdpromotion - Tracking y Metricas (US-CP-05)
    "1033": "Tipo de evento no reconocido.",
    "1034": "Este tipo de evento no puede registrarse por esta via.",
    "1035": "El valor de la aportacion debe ser mayor que cero.",
    "4032": "Demasiadas peticiones en poco tiempo. Espera unos minutos e intenta de nuevo.",
    "4040": "Saldo insuficiente para realizar el cobro solicitado.",
    "4041": "El saldo disponible es inferior al minimo de retiro.",
    "4042": "Ya existe una solicitud de cobro pendiente. Espera a que sea procesada.",

    // Internal errors (5000-5999)
    "5000": "Ha ocurrido un error inesperado. Por favor, intenta nuevamente",
    "5001": "Error de base de datos",
    "5002": "Error de servicio externo",
    "5003": "Error de configuracion",
} as const;

// Legacy string-based error messages (keep for backwards compatibility)
export const ERROR_MESSAGES: Record<string, string> = {
    // Auth errors
    AUTH_EMAIL_INVALID: "El formato del email no es valido",
    AUTH_EMAIL_EXISTS: "Este email ya esta registrado. Quieres iniciar sesion?",
    AUTH_PASSWORD_MIN_LENGTH: "La contrasena debe tener al menos 8 caracteres",
    AUTH_PASSWORD_MISMATCH: "Las contrasenas no coinciden",
    AUTH_UNAUTHORIZED: "Tu sesion ha expirado. Por favor, inicia sesion nuevamente",
    AUTH_INVALID_CREDENTIALS: "Email o contrasena incorrectos",

    // Artista errors
    ARTISTA_NOMBRE_REQUERIDO: "El nombre artistico es obligatorio",
    ARTISTA_NOMBRE_MAX_LENGTH: "El nombre artistico no puede superar los 200 caracteres",
    ARTISTA_DESC_MAX_LENGTH: "La descripcion no puede superar los 2000 caracteres",
    ARTISTA_PAIS_MAX_LENGTH: "El pais no puede superar los 100 caracteres",
    ARTISTA_CIUDAD_MAX_LENGTH: "La ciudad no puede superar los 100 caracteres",
    ARTISTA_IMAGEN_URL_INVALIDA: "La URL de la imagen no es valida. Debe comenzar con http:// o https://",
    ARTISTA_ALREADY_EXISTS: "Ya tienes un perfil de artista creado",
    ARTISTA_NOT_FOUND: "Artista no encontrado",

    // Validation errors
    VALIDATION_REQUIRED: "Este campo es obligatorio",
    VALIDATION_MAX_LENGTH: "El campo supera la longitud maxima permitida",
    VALIDATION_MIN_LENGTH: "El campo no cumple la longitud minima requerida",
    VALIDATION_DUPLICATE: "Ya existe un registro con estos datos",

    // Generic errors
    ERROR_UNEXPECTED: "Ha ocurrido un error inesperado. Por favor, intenta nuevamente",
    NOT_FOUND: "Recurso no encontrado",
    NETWORK_ERROR: "Error de conexion. Verifica tu conexion a internet",

    // Include numeric codes for unified lookup
    ...ERROR_CODE_MESSAGES,
} as const;

export type ErrorCode = keyof typeof ERROR_MESSAGES;

export const getErrorMessage = (errorCode: string | undefined | null): string => {
    if (!errorCode) {
        return ERROR_MESSAGES.ERROR_UNEXPECTED;
    }
    return ERROR_MESSAGES[errorCode] || ERROR_CODE_MESSAGES[errorCode] || ERROR_MESSAGES.ERROR_UNEXPECTED;
};

export const getCampaniaErrorMessage = (errorCode: string): string => {
    const customMessages: Record<string, string> = {
        "1001": "Completa todos los campos obligatorios para continuar",
        "1011": "El importe debe ser mayor a 0. Ingresa un monto valido",
        "1012": "La fecha de fin debe ser posterior a la fecha de inicio",
        "2003": "No encontramos esta campania. Puede haber sido eliminada",
        "3002": "No tienes permiso para modificar esta campania",
        "4009": "No puedes editar una campania que ya ha sido publicada",
    };

    return customMessages[errorCode] || getErrorMessage(errorCode);
};

/**
 * Mensajes de error especificos para Recompensas
 */
export const getRewardErrorMessage = (errorCode: string): string => {
    const customMessages: Record<string, string> = {
        "1001": "Completa todos los campos obligatorios para crear la recompensa",
        "1011": "El importe debe ser al menos 1 EUR",
        "2004": "Esta recompensa no existe o ha sido eliminada",
        "3002": "Solo el creador de la campania puede modificar sus recompensas",
        "4010": "Esta recompensa ya tiene aportes y no puede ser eliminada. Puedes desactivarla en su lugar.",
    };

    return customMessages[errorCode] || getErrorMessage(errorCode);
};

/**
 * Mensajes de error especificos para recompensas con contexto adicional
 * Util para tooltips, modals de confirmacion, etc.
 */
export const getRewardSpecificErrorMessage = (errorCode: string): string => {
    const specificMessages: Record<string, string> = {
        "1001": "Los campos Nombre, Importe Minimo y Tipo de Recompensa son obligatorios. Completa todos los datos antes de continuar.",
        "1002": "Has superado el limite de caracteres permitido. Revisa los campos marcados.",
        "1007": "Los valores numericos deben ser positivos. Corrige los campos marcados.",
        "1011": "El importe minimo debe ser mayor a 0 EUR. Ingresa un monto valido para la recompensa.",
        "2003": "La campania asociada no existe. Verifica que la campania este activa.",
        "2004": "No pudimos encontrar esta recompensa. Puede haber sido eliminada por el creador de la campania.",
        "3001": "Tu sesion ha expirado. Inicia sesion nuevamente para continuar gestionando recompensas.",
        "3002": "Solo el artista creador de la campania puede crear, editar o eliminar sus recompensas.",
        "4010": "Esta recompensa ya tiene aportes de fans y no puede ser eliminada para preservar los compromisos. Puedes desactivarla para ocultarla de nuevos backers, pero los aportes existentes se mantendran.",
    };

    return specificMessages[errorCode] || getRewardErrorMessage(errorCode);
};

export const formatErrorMessages = (
    messages: Array<{ message: string; errorCode: string }>
): string[] => {
    return messages
        .filter((m) => m.errorCode && !m.errorCode.startsWith("0"))
        .map((m) => getErrorMessage(m.errorCode));
};

export const isSuccessCode = (errorCode: string): boolean => {
    return errorCode.startsWith("0");
};

export const requiresReAuth = (errorCode: string): boolean => {
    return ["3001", "3003", "3004"].includes(errorCode);
};

// ========== Backing Error Messages ==========

/**
 * Mensajes de error especificos para Backings
 * Alineado con ErrorCodes del backend (contracts.md)
 */
export const BACKING_ERROR_MESSAGES: Record<string, string> = {
    // Success codes
    "0000": "Operacion exitosa",
    "0001": "Aporte realizado con exito. Gracias por tu apoyo!",

    // Validation errors (1000-1999)
    "1001": "Este campo es obligatorio",
    "1002": "El mensaje no puede superar los 500 caracteres",
    "1011": "El monto debe ser al menos 1 EUR",
    "1012": "El ID de recompensa no es valido",
    "4012": "El monto esta por debajo del minimo requerido para esta recompensa",

    // Not Found errors (2000-2999)
    "2003": "No encontramos esta campania",
    "2004": "No encontramos esta recompensa",

    // Auth errors (3000-3999)
    "3001": "Tu sesion ha expirado. Por favor, inicia sesion nuevamente",
    "3005": "Debes iniciar sesion para hacer un aporte",

    // Business Rule errors (4000-4999)
    "4006": "Esta campania no esta activa en este momento",
    "4007": "Esta campania ya ha finalizado",
    "4011": "Esta recompensa esta agotada. Por favor, selecciona otra",
    "4013": "Esta campania no permite aportes anonimos. Por favor, inicia sesion",

    // Internal errors (5000-5999)
    "5000": "Ha ocurrido un error al procesar tu aporte. Por favor, intenta nuevamente",
} as const;

/**
 * Obtiene mensaje de error para backings
 */
export const getBackingErrorMessage = (errorCode: string): string => {
    return BACKING_ERROR_MESSAGES[errorCode] || BACKING_ERROR_MESSAGES["5000"];
};

/**
 * Obtiene mensaje de error contextual con datos de reward
 * Util para errores que requieren informacion dinamica
 */
export const getBackingContextualError = (
    errorCode: string,
    context?: { rewardNombre?: string; importeMinimo?: number }
): string => {
    if (errorCode === "4012" && context?.importeMinimo) {
        return `El monto debe ser al menos ${context.importeMinimo} EUR para "${context.rewardNombre}"`;
    }

    if (errorCode === "4011" && context?.rewardNombre) {
        return `La recompensa "${context.rewardNombre}" esta agotada. Selecciona otra opcion`;
    }

    return getBackingErrorMessage(errorCode);
};

// ========== Crowdsourcing Template Error Messages ==========

export const CROWDSOURCING_ERROR_MESSAGES: Record<string, string> = {
    // NotFound errors (2000-2999)
    TEMPLATE_NOT_FOUND: 'La plantilla seleccionada no existe',
    PROYECTO_ARTISTICO_NOT_FOUND: 'El proyecto artistico no fue encontrado',
    PLANTILLA_NECESIDAD_NOT_FOUND: 'Una o mas necesidades de la plantilla no fueron encontradas',

    // Authorization errors (3000-3999)
    PROYECTO_NO_PERTENECE_ARTISTA: 'No tienes permiso para modificar este proyecto',

    // Validation errors (1000-1999)
    VALIDATION_PRESUPUESTO_MIN_NEGATIVO: 'El presupuesto minimo no puede ser negativo',
    VALIDATION_PRESUPUESTO_MAX_MENOR_MIN: 'El presupuesto maximo debe ser mayor o igual al minimo',
    VALIDATION_NECESIDADES_REQUERIDAS: 'Debe seleccionar al menos una necesidad',
    VALIDATION_PROYECTO_REQUERIDO: 'Debe seleccionar un proyecto artistico',
} as const;

export const getCrowdsourcingErrorMessage = (errorCode: string): string => {
    return CROWDSOURCING_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};

// ========== Dashboard Error Messages ==========

/**
 * Mensajes de error especificos para Dashboard de Artista
 * Alineado con contracts.md de dashboard-artista
 */
export const DASHBOARD_ERROR_MESSAGES: Record<string, string> = {
    // Success codes
    '0000': 'Operacion exitosa',

    // Validation errors (1000-1999)
    '1007': 'Valor fuera de rango',

    // Not Found errors (2000-2999)
    '2002': 'No tienes un perfil de artista. Por favor, crea tu perfil primero',
    '2003': 'Campania no encontrada',

    // Auth errors (3000-3999)
    '3001': 'Tu sesion ha expirado. Por favor, inicia sesion nuevamente',
    '3002': 'No tienes permiso para acceder a este recurso',

    // Internal errors (5000-5999)
    '5000': 'Ha ocurrido un error inesperado. Por favor, intenta nuevamente',
} as const;

/**
 * Obtiene mensaje de error para dashboard
 * Usa mensajes especificos de dashboard si existen, sino fallback a getErrorMessage
 */
export const getDashboardErrorMessage = (errorCode: string): string => {
    return DASHBOARD_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};

// ========== Crowdsourcing Necesidades Error Messages ==========

export const NECESIDAD_ERROR_MESSAGES: Record<string, string> = {
    // Validation errors (1000-1999)
    '1011': 'El titulo debe tener al menos 5 caracteres',
    '1012': 'La fecha no es valida',

    // NotFound errors (2000-2999)
    '2009': 'La necesidad no fue encontrada',

    // Business Rule errors (4000-4999)
    '4001': 'Solo se pueden editar necesidades en estado Abierta',
    '4002': 'Solo se pueden cerrar necesidades en estado Abierta o En Progreso',

    // Friendly context messages (semantic keys)
    NECESIDAD_NOT_FOUND: 'La necesidad seleccionada no existe',
    NECESIDAD_NOT_EDITABLE: 'Solo se pueden editar necesidades en estado Abierta',
    NECESIDAD_NOT_CLOSEABLE: 'Solo se pueden cerrar necesidades en estado Abierta o En Progreso',
    PROYECTO_ARTISTICO_NOT_FOUND: 'El proyecto artistico no fue encontrado',
    PROYECTO_NO_PERTENECE_ARTISTA: 'No tienes permiso para crear necesidades en este proyecto',
    VALIDATION_TITULO_MIN_LENGTH: 'El titulo debe tener al menos 5 caracteres',
    VALIDATION_FECHA_INVALIDA: 'La fecha ingresada no es valida',
    VALIDATION_UBICACION_REQUERIDA: 'La ubicacion es obligatoria para modalidad Presencial o Hibrida',
    VALIDATION_PRESUPUESTO_MONEDA_REQUERIDA: 'Debe especificar la moneda cuando indica presupuesto',
    VALIDATION_PRESUPUESTO_MAX_MENOR_MIN: 'El presupuesto maximo debe ser mayor o igual al minimo',
} as const;

export const getNecesidadErrorMessage = (errorCode: string): string => {
    return NECESIDAD_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};

// ========== Propuestas Crowdsourcing Error Messages (US-CS-03) ==========

export const PROPUESTA_ERROR_MESSAGES: Record<string, string> = {
    // NotFound errors (2000-2999)
    '2009': 'La necesidad no fue encontrada',
    '2010': 'La propuesta no fue encontrada',

    // Business Rule errors (4000-4999)
    '4003': 'Ya tienes una propuesta enviada para esta necesidad',
    '4004': 'No puedes enviar propuesta a tu propia necesidad',
    '4005': 'Solo se pueden retirar propuestas en estado Pendiente',
    '4006': 'Debes crear un perfil profesional para poder enviar propuestas',

    // Semantic keys para uso interno en componentes
    PROPUESTA_NOT_FOUND: 'La propuesta no fue encontrada',
    ALREADY_PROPOSED: 'Ya tienes una propuesta enviada para esta necesidad',
    CANNOT_PROPOSE_SELF: 'No puedes enviar propuesta a tu propia necesidad',
    PROPUESTA_NOT_RETIRABLE: 'Solo se pueden retirar propuestas en estado Pendiente',
    NO_PROFESSIONAL_PROFILE: 'Debes crear un perfil profesional para poder enviar propuestas',
    VALIDATION_PRECIO_REQUERIDO: 'El precio propuesto debe ser mayor a 0',
    VALIDATION_MENSAJE_MIN_LENGTH: 'El mensaje debe tener al menos 20 caracteres',
    VALIDATION_DIAS_ESTIMADOS_RANGO: 'Los dias estimados deben ser entre 1 y 365',
} as const;

/**
 * Obtiene mensaje de error para el flujo de propuestas (enviar / retirar)
 * Usa mensajes especificos de propuestas si existen, sino fallback a getErrorMessage
 */
export const getPropuestaErrorMessage = (errorCode: string): string => {
    return PROPUESTA_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};

// ========== Acuerdos, Milestones y Entregables Error Messages (US-CS-04) ==========

export const ACUERDO_ERROR_MESSAGES: Record<string, string> = {
    // Numeric error codes (override global codes for acuerdos context)
    '1011': 'El campo no cumple la longitud minima requerida',
    '1013': 'La URL del recurso no es valida. Usa un enlace de Dropbox, Drive o similar',
    '2011': 'El acuerdo no fue encontrado',
    '2012': 'El milestone no fue encontrado',
    '2013': 'El entregable no fue encontrado',
    '4007': 'La propuesta no esta disponible para esta accion',
    '4008': 'Ya existe un acuerdo activo para esta necesidad',
    '4009': 'La suma de importes de los milestones supera el total pactado',
    '4010': 'Esta accion solo esta disponible cuando el acuerdo esta activo',
    '4011': 'No se puede modificar un milestone que ya fue completado',
    '4012': 'No se puede eliminar un milestone que tiene entregables asociados',
    '4013': 'Solo se pueden revisar entregables en estado Entregado',

    // Semantic keys for internal component usage
    ACUERDO_NOT_FOUND: 'El acuerdo no fue encontrado',
    MILESTONE_NOT_FOUND: 'El milestone no fue encontrado',
    ENTREGABLE_NOT_FOUND: 'El entregable no fue encontrado',
    PROPUESTA_NOT_ACCEPTABLE: 'La propuesta no esta en estado Pendiente',
    ACUERDO_ALREADY_EXISTS: 'Ya existe un acuerdo activo para esta necesidad',
    MILESTONE_IMPORTE_EXCEEDED: 'La suma de importes supera el total pactado del acuerdo',
    ACUERDO_NOT_ACTIVE: 'Esta accion solo esta disponible para acuerdos activos',
    MILESTONE_COMPLETED: 'El milestone ya fue completado y no puede modificarse',
    MILESTONE_HAS_ENTREGABLES: 'El milestone tiene entregables asociados y no puede eliminarse',
    ENTREGABLE_NOT_REVIEWABLE: 'El entregable no esta en estado Entregado',
    VALIDATION_COMENTARIO_RECHAZO_MIN: 'El comentario de rechazo debe tener al menos 10 caracteres',
    VALIDATION_MOTIVO_CANCELACION_MIN: 'El motivo de cancelacion debe tener al menos 20 caracteres',
} as const;

/**
 * Obtiene mensaje de error para el flujo de acuerdos, milestones y entregables
 * Usa mensajes especificos de acuerdos si existen, sino fallback a getErrorMessage
 */
export const getAcuerdoErrorMessage = (errorCode: string): string => {
    return ACUERDO_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};

// ========== Mensajeria Crowdsourcing Error Messages (US-CS-05) ==========

export const MENSAJERIA_ERROR_MESSAGES: Record<string, string> = {
    // Numeric error codes (override global codes for mensajeria context)
    '2014': 'La conversacion no fue encontrada',
    '4015': 'Ya existe una conversacion con esta persona para el mismo tema. Seras redirigido a la conversacion existente.',
    '4016': 'No puedes iniciar una conversacion con este usuario. Debes tener una propuesta o acuerdo en comun.',

    // Semantic keys para uso interno en componentes
    CONVERSACION_NOT_FOUND: 'La conversacion no fue encontrada',
    CONVERSACION_DUPLICADA: 'Ya existe una conversacion con esta persona para el mismo tema. Seras redirigido a la conversacion existente.',
    CONVERSACION_NO_RELACION: 'No puedes iniciar una conversacion con este usuario. Debes tener una propuesta o acuerdo en comun.',
    MENSAJE_CONTENIDO_VACIO: 'El mensaje no puede estar vacio',
    MENSAJE_CONTENIDO_MAX: 'El mensaje es demasiado largo (maximo 5000 caracteres)',
    MENSAJE_URL_INVALIDA: 'La URL adjunta no es valida. Verifica que sea una URL completa (ej: https://...)',
} as const;

/**
 * Obtiene mensaje de error para el flujo de mensajeria entre partes.
 * Caso especial: error 4015 indica conversacion duplicada; el componente
 * debe detectar este codigo y redirigir al chat existente en lugar de mostrar error.
 * Usa mensajes especificos de mensajeria si existen, sino fallback a getErrorMessage.
 */
export const getMensajeriaErrorMessage = (errorCode: string): string => {
    return MENSAJERIA_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};

// ========== Valoraciones Crowdsourcing Error Messages (US-CS-06) ==========

export const VALORACION_ERROR_MESSAGES: Record<string, string> = {
    // Numeric error codes (override global codes for valoraciones context)
    '1014': 'La puntuacion debe ser entre 1 y 5 estrellas',
    '2011': 'El acuerdo no fue encontrado',
    '3002': 'No eres participante de este acuerdo',
    '4007': 'Solo puedes valorar acuerdos que han sido completados',
    '4014': 'Ya has enviado una valoracion para este acuerdo',

    // Semantic keys para uso interno en componentes y hooks
    VALORACION_DUPLICATE: 'Ya has enviado una valoracion para este acuerdo',
    VALORACION_ACUERDO_NOT_COMPLETED: 'Solo puedes valorar acuerdos que han sido completados',
    VALORACION_PUNTUACION_INVALID: 'La puntuacion debe ser entre 1 y 5 estrellas',
    VALORACION_COMENTARIO_MAX: 'El comentario no puede superar los 1000 caracteres',
} as const;

/**
 * Obtiene mensaje de error para el flujo de valoraciones.
 * Caso especial: el codigo 4007 tiene significado distinto en valoraciones
 * respecto a acuerdos (US-CS-04). Esta funcion devuelve el mensaje correcto
 * para el contexto de valoraciones.
 * Usa mensajes especificos de valoraciones si existen, sino fallback a getErrorMessage.
 */
export const getValoracionErrorMessage = (errorCode: string): string => {
    return VALORACION_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};

// ========== Crowdpromotion - Perfil de Promotor Error Messages ==========

export const PROMOTOR_ERROR_MESSAGES: Record<string, string> = {
    // Numeric error codes (override global codes for promotor context)
    '1001': 'Completa los campos obligatorios: nombre publico y tipo de promotor',
    '1002': 'El valor supera el maximo de caracteres permitido',
    '1003': 'El email de contacto no tiene formato valido',
    '1010': 'El tipo de promotor seleccionado no existe',
    '1011': 'El nombre debe tener al menos 3 caracteres',
    '1013': 'La URL indicada no tiene formato valido. Usa una URL completa (ej: https://...)',
    '2015': 'No tienes un perfil de promotor. Registrate primero.',
    '3001': 'Tu sesion ha expirado. Por favor, inicia sesion nuevamente',
    '4018': 'Ya tienes un perfil de promotor creado.',
    '4019': 'Tu perfil de promotor ya esta desactivado.',
    '5000': 'Ha ocurrido un error inesperado. Por favor, intenta nuevamente',

    // Semantic keys para uso interno en hooks y componentes
    PROMOTOR_NOT_FOUND:          'No tienes un perfil de promotor. Registrate primero.',
    PROMOTOR_ALREADY_EXISTS:     'Ya tienes un perfil de promotor creado.',
    PROMOTOR_ALREADY_INACTIVE:   'Tu perfil de promotor ya esta desactivado.',
    PROMOTOR_TIPO_NOT_FOUND:     'El tipo de promotor seleccionado no existe.',
    PROMOTOR_NOMBRE_REQUIRED:    'El nombre publico es obligatorio.',
    PROMOTOR_NOMBRE_MIN:         'El nombre debe tener al menos 3 caracteres.',
    PROMOTOR_NOMBRE_MAX:         'El nombre publico no puede superar los 200 caracteres.',
    PROMOTOR_EMAIL_INVALID:      'El email de contacto no tiene un formato valido.',
    PROMOTOR_URL_INVALID:        'La URL indicada no tiene un formato valido.',
} as const;

export const getPromotorErrorMessage = (errorCode: string): string => {
    return PROMOTOR_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};

// ========== Crowdpromotion - Programa de Promocion Error Messages ==========

export const PROMO_PROGRAMA_ERROR_MESSAGES: Record<string, string> = {
    // Numeric codes (override global codes for this context)
    '1001': 'Completa los campos obligatorios: titulo, tipo de programa y moneda',
    '1011': 'El titulo debe tener al menos 5 caracteres',
    '1020': 'Debes definir al menos una comision (porcentaje o fija)',
    '1021': 'El valor esta fuera del rango permitido',
    '1022': 'La fecha fin debe ser posterior a la fecha inicio',
    '1023': 'El formato del campo no es valido (solo letras, numeros y guiones)',
    '1024': 'Este codigo de tracking ya esta en uso para otro programa. Elige otro',
    '1025': 'Las tareas repetibles deben tener un numero maximo de repeticiones definido',
    '1026': 'El importe de recompensa es requerido para recompensas monetarias',
    '1027': 'Los puntos de recompensa son requeridos para recompensas de puntos',
    '2016': 'No tienes un perfil de artista. Crea tu perfil de artista primero',
    '2019': 'El programa de promocion no existe o fue eliminado',
    '3002': 'No tienes permiso para realizar esta accion sobre este programa',
    '4020': 'Este programa ya esta desactivado',
    '4021': 'No se puede desactivar esta tarea porque ya tiene completados registrados',
    '5000': 'Ha ocurrido un error inesperado al procesar el programa. Por favor, intenta nuevamente',

    // Semantic keys para uso interno en hooks y componentes
    PROGRAMA_NOT_FOUND:              'El programa de promocion no existe o fue eliminado',
    PROGRAMA_ALREADY_INACTIVE:       'Este programa ya esta desactivado',
    PROGRAMA_TAREA_CON_COMPLETADOS:  'No se puede desactivar una tarea que ya tiene completados registrados',
    PROGRAMA_COMISION_REQUERIDA:     'Debes definir al menos una comision (porcentaje o fija)',
    PROGRAMA_CODIGO_TRACKING_DUPLICADO: 'Este codigo de tracking ya esta en uso. Elige otro',
    ARTISTA_NOT_FOUND:               'No tienes un perfil de artista. Crea tu perfil primero',
    CAMPANA_NOT_BELONGS_TO_ARTISTA:  'No tienes permiso para vincular esta campana a tu programa',
} as const;

export const getPromoProgramaErrorMessage = (errorCode: string): string => {
    return PROMO_PROGRAMA_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};

// ========== Crowdpromotion - Inscripcion a Programa Error Messages (US-CP-03) ==========

export const INSCRIPCION_ERROR_MESSAGES: Record<string, string> = {
    // Numeric error codes (override global codes for inscripcion context)
    '2020': 'La inscripcion no existe.',
    '4021': 'Ya estas inscrito en este programa.',
    '4022': 'No puedes inscribirte en este programa.',
    '4023': 'Tu perfil de promotor esta desactivado. Reactiva tu perfil para inscribirte.',
    '4024': 'Este programa no esta activo en este momento.',
    '4025': 'La inscripcion no se puede modificar en su estado actual.',
    '4026': 'No tienes permiso para gestionar las inscripciones de este programa.',

    // Semantic keys para uso interno en hooks y componentes
    INSCRIPCION_NOT_FOUND: 'La inscripcion no existe.',
    INSCRIPCION_ALREADY_EXISTS: 'Ya estas inscrito en este programa.',
    INSCRIPCION_PROMOTOR_BLOCKED: 'No puedes inscribirte en este programa.',
    INSCRIPCION_PROMOTOR_INACTIVE: 'Tu perfil de promotor esta desactivado. Reactiva tu perfil para inscribirte.',
    INSCRIPCION_PROGRAMA_INACTIVE: 'Este programa no esta activo en este momento.',
    INSCRIPCION_ESTADO_INVALIDO: 'La inscripcion no se puede modificar en su estado actual.',
    INSCRIPCION_NOT_OWNER: 'No tienes permiso para gestionar las inscripciones de este programa.',
} as const;

export const getInscripcionErrorMessage = (errorCode: string): string => {
    return INSCRIPCION_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};

// ========== Crowdpromotion - Tareas de Promocion Error Messages (US-CP-04) ==========

export const TAREA_PROMOCION_ERROR_MESSAGES: Record<string, string> = {
    // Numeric codes (override global codes for tareas context)
    '1001': 'La URL de prueba es obligatoria.',
    '1002': 'El campo supera el maximo de caracteres permitido.',
    '1013': 'La URL de prueba no tiene formato valido. Usa una URL completa (ej: https://...)',
    '2015': 'No tienes un perfil de promotor. Registrate primero.',
    '2016': 'No tienes un perfil de artista.',
    '2019': 'El programa de promocion no existe o fue eliminado.',
    '2021': 'La tarea no existe o no pertenece a este programa.',
    '2022': 'El completado no existe o no pertenece a este programa.',
    '3001': 'Tu sesion ha expirado. Por favor, inicia sesion nuevamente.',
    '4024': 'Este programa no esta activo en este momento.',
    '4026': 'No tienes permiso para acceder a este programa.',
    '4027': 'Ya completaste esta tarea. No se puede volver a completar porque no es repetible.',
    '4028': 'Has alcanzado el numero maximo de veces que puedes completar esta tarea.',
    '4029': 'Esta tarea ya no esta disponible.',
    '4030': 'El plazo para completar esta tarea ha finalizado.',
    '4031': 'Este completado no puede ser procesado porque ya fue validado o rechazado.',
    '5000': 'Ha ocurrido un error inesperado. Por favor, intenta nuevamente.',

    // Semantic keys para uso interno en hooks y componentes
    TAREA_NO_REPETIBLE: 'Ya completaste esta tarea. No se puede volver a completar porque no es repetible.',
    MAX_REPETICIONES_ALCANZADO: 'Has alcanzado el numero maximo de veces que puedes completar esta tarea.',
    TAREA_INACTIVA: 'Esta tarea ya no esta disponible.',
    TAREA_FUERA_FECHA: 'El plazo para completar esta tarea ha finalizado.',
    COMPLETADO_ESTADO_INVALIDO: 'Este completado no puede ser procesado porque ya fue validado o rechazado.',
    TAREA_NOT_FOUND: 'La tarea no existe o no pertenece a este programa.',
    COMPLETADO_NOT_FOUND: 'El completado no existe o no pertenece a este programa.',
} as const;

export const getTareaPromocionErrorMessage = (errorCode: string): string => {
    return TAREA_PROMOCION_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};

// ========== Crowdpromotion - Wallet y Cobros Error Messages (US-CP-06) ==========

export const WALLET_ERROR_MESSAGES: Record<string, string> = {
    // Codigos numericos (override del ERROR_CODE_MESSAGES global para contexto wallet)
    '1001': 'El importe es obligatorio.',
    '1002': 'La descripcion no puede superar los 500 caracteres.',
    '1021': 'El importe debe ser mayor que cero.',
    '1036': 'La fecha de inicio no puede ser posterior a la fecha fin.',
    '2015': 'No tienes un perfil de promotor. Registrate primero.',
    '2030': 'No tienes un wallet asociado. Completa tu primera actividad de promocion para activarlo.',
    '3001': 'Tu sesion ha expirado. Por favor, inicia sesion nuevamente.',
    '3002': 'No tienes permiso para realizar esta accion.',
    '4040': 'Saldo insuficiente. El importe solicitado supera tu saldo disponible.',
    '4041': 'El saldo disponible es inferior al minimo de retiro (10.00 EUR). Acumula mas comisiones antes de solicitar un cobro.',
    '4042': 'Ya tienes una solicitud de cobro en proceso. Espera a que sea procesada antes de crear otra.',
    '5000': 'Ha ocurrido un error inesperado al procesar tu solicitud. Por favor, intenta nuevamente.',

    // Claves semanticas para uso interno en hooks y componentes
    WALLET_NOT_FOUND: 'No tienes un wallet asociado. Completa tu primera actividad de promocion para activarlo.',
    PROMOTOR_NOT_FOUND: 'No tienes un perfil de promotor. Registrate primero.',
    SALDO_INSUFICIENTE: 'Saldo insuficiente. El importe solicitado supera tu saldo disponible.',
    SALDO_BAJO_MINIMO: 'El saldo disponible es inferior al minimo de retiro (10.00 EUR).',
    COBRO_CONCURRENTE: 'Ya tienes una solicitud de cobro en proceso. Espera a que sea procesada.',
    IMPORTE_INVALIDO: 'El importe debe ser mayor que cero.',
    DESCRIPCION_MAX: 'La descripcion no puede superar los 500 caracteres.',
} as const;

export const getWalletErrorMessage = (errorCode: string): string => {
    return WALLET_ERROR_MESSAGES[errorCode] || getErrorMessage(errorCode);
};
