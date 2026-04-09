import { MONEDA_SYMBOLS } from "@/lib/constants"

/**
 * Calcula dias restantes desde hoy hasta fecha fin
 */
export function calcularDiasRestantes(fechaFin: string | Date): number {
    const fin = typeof fechaFin === "string" ? new Date(fechaFin) : fechaFin
    const hoy = new Date()
    const diffMs = fin.getTime() - hoy.getTime()
    const dias = Math.ceil(diffMs / (1000 * 60 * 60 * 24))
    return Math.max(0, dias)
}

/**
 * Calcula porcentaje financiado
 */
export function calcularPorcentaje(pledged: number, objetivo: number): number {
    if (objetivo === 0) return 0
    return Math.min(Math.round((pledged / objetivo) * 100), 100)
}

/**
 * Formatea un monto con simbolo de moneda
 */
export function formatCurrency(amount: number, monedaId: number = 1): string {
    const symbol = MONEDA_SYMBOLS[monedaId] || "\u20AC"
    return `${symbol} ${amount.toLocaleString("es-ES", { minimumFractionDigits: 0, maximumFractionDigits: 0 })}`
}

/**
 * Obtiene el simbolo de moneda
 */
export function getCurrencySymbol(monedaId: number): string {
    return MONEDA_SYMBOLS[monedaId] || "\u20AC"
}

/**
 * Extrae video ID de URL de YouTube o Vimeo
 */
export function extractVideoId(url: string): { type: "youtube" | "vimeo" | null; id: string } {
    // YouTube
    const youtubeRegex = /(?:youtube\.com\/watch\?v=|youtu\.be\/)([^&#]+)/
    const youtubeMatch = url.match(youtubeRegex)
    if (youtubeMatch) {
        return { type: "youtube", id: youtubeMatch[1] }
    }

    // Vimeo
    const vimeoRegex = /vimeo\.com\/(\d+)/
    const vimeoMatch = url.match(vimeoRegex)
    if (vimeoMatch) {
        return { type: "vimeo", id: vimeoMatch[1] }
    }

    return { type: null, id: "" }
}

/**
 * Genera la URL de embed para video
 */
export function getVideoEmbedUrl(url: string): string | null {
    const video = extractVideoId(url)
    if (video.type === "youtube") {
        return `https://www.youtube.com/embed/${video.id}`
    }
    if (video.type === "vimeo") {
        return `https://player.vimeo.com/video/${video.id}`
    }
    return null
}

/**
 * Formatea una fecha como tiempo relativo (ej: "hace 2 horas")
 */
export function formatRelativeTime(dateString: string): string {
    try {
        const date = new Date(dateString)
        const now = new Date()
        const diffMs = now.getTime() - date.getTime()
        const diffSec = Math.floor(diffMs / 1000)
        const diffMin = Math.floor(diffSec / 60)
        const diffHrs = Math.floor(diffMin / 60)
        const diffDays = Math.floor(diffHrs / 24)

        if (diffSec < 60) return "hace un momento"
        if (diffMin < 60) return `hace ${diffMin} min`
        if (diffHrs < 24) return `hace ${diffHrs}h`
        if (diffDays === 1) return "hace 1 dia"
        if (diffDays < 30) return `hace ${diffDays} dias`
        if (diffDays < 365) return `hace ${Math.floor(diffDays / 30)} meses`
        return `hace ${Math.floor(diffDays / 365)} anos`
    } catch {
        return ""
    }
}

/**
 * Formatea fecha completa (ej: "15 Feb 2026, 14:32")
 */
export function formatDate(dateString: string): string {
    try {
        const date = new Date(dateString)
        return date.toLocaleDateString("es-ES", {
            day: "numeric",
            month: "short",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit",
        })
    } catch {
        return ""
    }
}
