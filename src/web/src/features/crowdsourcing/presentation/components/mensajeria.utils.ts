/**
 * Format a timestamp as relative time for conversation list.
 * - Undefined/null: empty string
 * - Today: HH:MM
 * - Yesterday: "ayer"
 * - Older: dd/MM
 */
export function formatTimestamp(fechaUltimoMensaje?: string): string {
    if (!fechaUltimoMensaje) return ""

    const date = new Date(fechaUltimoMensaje)
    const now = new Date()
    const today = new Date(now.getFullYear(), now.getMonth(), now.getDate())
    const yesterday = new Date(today)
    yesterday.setDate(yesterday.getDate() - 1)

    if (date >= today) {
        return date.toLocaleTimeString("es-ES", {
            hour: "2-digit",
            minute: "2-digit",
        })
    }

    if (date >= yesterday) {
        return "ayer"
    }

    return date.toLocaleDateString("es-ES", {
        day: "2-digit",
        month: "2-digit",
    })
}

/**
 * Format a message creation time as HH:MM.
 */
export function formatMessageTime(fechaCreacion: string): string {
    const date = new Date(fechaCreacion)
    return date.toLocaleTimeString("es-ES", {
        hour: "2-digit",
        minute: "2-digit",
    })
}

/**
 * Get the initials from a name (first 2 characters, uppercase).
 */
export function getInitials(name: string): string {
    return name.slice(0, 2).toUpperCase()
}

/**
 * Check if two dates are on different calendar days.
 */
export function isDifferentDay(date1: string, date2: string): boolean {
    const d1 = new Date(date1)
    const d2 = new Date(date2)
    return (
        d1.getFullYear() !== d2.getFullYear() ||
        d1.getMonth() !== d2.getMonth() ||
        d1.getDate() !== d2.getDate()
    )
}
