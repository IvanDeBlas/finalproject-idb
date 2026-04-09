// Currency formatting
export function formatCurrency(amount: number, currency = "EUR"): string {
  return new Intl.NumberFormat("es-ES", {
    style: "currency",
    currency,
    minimumFractionDigits: 0,
    maximumFractionDigits: 0,
  }).format(amount)
}

// Date formatting
export function formatDate(date: Date | string): string {
  const d = typeof date === "string" ? new Date(date) : date
  return new Intl.DateTimeFormat("es-ES", {
    day: "numeric",
    month: "long",
    year: "numeric",
  }).format(d)
}

export function formatDateShort(date: Date | string): string {
  const d = typeof date === "string" ? new Date(date) : date
  return new Intl.DateTimeFormat("es-ES", {
    day: "2-digit",
    month: "2-digit",
    year: "numeric",
  }).format(d)
}

export function formatRelativeDate(date: Date | string): string {
  const d = typeof date === "string" ? new Date(date) : date
  const now = new Date()
  const diffMs = d.getTime() - now.getTime()
  const diffDays = Math.ceil(diffMs / (1000 * 60 * 60 * 24))

  if (diffDays < 0) {
    return `Hace ${Math.abs(diffDays)} dias`
  } else if (diffDays === 0) {
    return "Hoy"
  } else if (diffDays === 1) {
    return "Manana"
  } else {
    return `En ${diffDays} dias`
  }
}

// Number formatting
export function formatNumber(num: number): string {
  return new Intl.NumberFormat("es-ES").format(num)
}

export function formatCompactNumber(num: number): string {
  if (num >= 1000000) {
    return `${(num / 1000000).toFixed(1)}M`
  } else if (num >= 1000) {
    return `${(num / 1000).toFixed(1)}K`
  }
  return num.toString()
}

// Percentage
export function formatPercentage(value: number, decimals = 0): string {
  return `${value.toFixed(decimals)}%`
}

export function calculatePercentage(current: number, total: number): number {
  if (total === 0) return 0
  return Math.min(Math.round((current / total) * 100), 100)
}

// Days remaining
export function getDaysRemaining(endDate: Date | string): number {
  const end = typeof endDate === "string" ? new Date(endDate) : endDate
  const now = new Date()
  const diffMs = end.getTime() - now.getTime()
  return Math.max(0, Math.ceil(diffMs / (1000 * 60 * 60 * 24)))
}

// Currency formatting with monedaId
const MONEDA_CODES_MAP: Record<number, string> = {
  1: "EUR",
  2: "USD",
}

const MONEDA_SYMBOLS_MAP: Record<number, string> = {
  1: "€",
  2: "$",
}

export function formatCurrencyWithSymbol(
  amount: number,
  monedaId: number = 1
): string {
  const currencyCode = MONEDA_CODES_MAP[monedaId] || "EUR"
  const locale = monedaId === 1 ? "es-ES" : "en-US"

  return new Intl.NumberFormat(locale, {
    style: "currency",
    currency: currencyCode,
    minimumFractionDigits: 0,
    maximumFractionDigits: 0,
  }).format(amount)
}

export function getCurrencySymbol(monedaId: number = 1): string {
  return MONEDA_SYMBOLS_MAP[monedaId] || "€"
}

// ========== Backing Helpers ==========

/**
 * Obtiene el nombre a mostrar de un backer
 * Logica: si esAnonimo o userName vacio/null → "Anonimo", sino userName
 */
export function getBackerDisplayName(
  esAnonimo: boolean,
  userName?: string
): string {
  if (esAnonimo || !userName) {
    return "Anonimo"
  }
  return userName
}

// ========== Dashboard Metrics Helpers ==========

/**
 * Calcula el porcentaje de progreso de una campania
 * Retorna valor redondeado a 2 decimales, max 100%
 */
export function calculateCampaniaProgress(
    importeRecaudado: number,
    importeObjetivo: number
): number {
    if (importeObjetivo === 0) return 0;
    const percentage = (importeRecaudado / importeObjetivo) * 100;
    return Math.min(Math.round(percentage * 100) / 100, 100);
}

/**
 * Calcula los dias restantes de una campania
 * Retorna undefined si fechaFin es null/undefined
 */
export function calculateDiasRestantes(fechaFin?: string): number | undefined {
    if (!fechaFin) return undefined;

    const end = new Date(fechaFin);
    const now = new Date();
    const diffMs = end.getTime() - now.getTime();
    const diffDays = Math.ceil(diffMs / (1000 * 60 * 60 * 24));

    return Math.max(0, diffDays);
}

/**
 * Calcula la proyeccion final de recaudacion basada en velocidad diaria
 */
export function calculateProyeccionFinal(
    importeRecaudado: number,
    velocidadDiaria: number,
    diasRestantes: number
): number {
    return importeRecaudado + (velocidadDiaria * diasRestantes);
}

/**
 * Calcula la velocidad diaria de recaudacion
 * Retorna 0 si diasTranscurridos es 0 o negativo
 */
export function calculateVelocidadDiaria(
    importeRecaudado: number,
    diasTranscurridos: number
): number {
    if (diasTranscurridos <= 0) return 0;
    return importeRecaudado / diasTranscurridos;
}

/**
 * Calcula el promedio de backing
 * Retorna 0 si numBackers es 0
 */
export function calculateBackingPromedio(
    totalRecaudado: number,
    numBackers: number
): number {
    if (numBackers === 0) return 0;
    return totalRecaudado / numBackers;
}

/**
 * Formatea un numero con decimales
 */
export function formatDecimal(value: number, decimals: number = 2): string {
    return value.toFixed(decimals);
}

// ========== Presupuesto Formatting ==========

const PRESUPUESTO_MONEDA_SIMBOLOS: Record<number, string> = {
    1: '\u20AC',
    2: '$',
    3: '\u00A3',
};

/**
 * Formatea el rango de presupuesto con moneda
 * @param min - Presupuesto minimo (opcional)
 * @param max - Presupuesto maximo (opcional)
 * @param monedaId - ID de moneda (1=EUR, 2=USD, 3=GBP)
 * @returns String formateado: "€150 - €800", "Desde €150", "Hasta €800", "No especificado"
 */
export function formatPresupuesto(
    min?: number,
    max?: number,
    monedaId: number = 1
): string {
    if (min === undefined && max === undefined) return 'No especificado';

    const simbolo = PRESUPUESTO_MONEDA_SIMBOLOS[monedaId] ?? '\u20AC';

    if (min !== undefined && max !== undefined) {
        return `${simbolo}${min} - ${simbolo}${max}`;
    }
    if (min !== undefined) {
        return `Desde ${simbolo}${min}`;
    }
    if (max !== undefined) {
        return `Hasta ${simbolo}${max}`;
    }

    return 'No especificado';
}

// ========== Crowdpromotion - Promotor Formatters ==========

/**
 * Formatea el total de comisiones ganadas con la moneda indicada.
 * @param total - Importe de comisiones
 * @param monedaCodigo - Codigo ISO de la moneda (ej: "EUR")
 * @returns String formateado con moneda
 */
export function formatComisionesGanadas(
    total: number,
    monedaCodigo: string = 'EUR'
): string {
    return new Intl.NumberFormat('es-ES', {
        style: 'currency',
        currency: monedaCodigo,
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
    }).format(total);
}

// ========== Crowdpromotion - Tracking y Metricas Formatters (US-CP-05) ==========

/**
 * Formatea la tasa de conversion como porcentaje con 2 decimales.
 * @param tasa - Tasa de conversion (ej: 1.11)
 * @returns String formateado (ej: '1.11%'). Devuelve '0.00%' si tasa es 0 o falsy.
 */
export function formatTasaConversion(tasa: number): string {
    if (!tasa) return '0.00%';
    return `${tasa.toFixed(2)}%`;
}

// ========== Crowdpromotion - Wallet Formatters (US-CP-06) ==========

/**
 * Formatea el saldo del wallet con la moneda del wallet.
 * Usa 2 decimales obligatorios (critico para importes monetarios de retiro).
 * @param importe - Importe a formatear
 * @param monedaNombre - Codigo ISO de la moneda (ej: 'EUR')
 * @returns String formateado (ej: '150,50 €')
 */
export function formatWalletImporte(
    importe: number,
    monedaNombre: string = 'EUR'
): string {
    return new Intl.NumberFormat('es-ES', {
        style: 'currency',
        currency: monedaNombre,
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
    }).format(importe);
}

/**
 * Formatea el importe de una transaccion con signo segun si es credito o debito.
 * @param importe - Importe absoluto (siempre positivo)
 * @param esCredito - true=ingreso (positivo), false=retiro (negativo)
 * @param monedaNombre - Codigo ISO de la moneda (ej: 'EUR')
 * @returns String formateado con signo (ej: '+10,00 €' o '-50,00 €')
 */
export function formatTransaccionImporte(
    importe: number,
    esCredito: boolean,
    monedaNombre: string = 'EUR'
): string {
    const formatted = formatWalletImporte(importe, monedaNombre);
    return esCredito ? `+${formatted}` : `-${formatted}`;
}
