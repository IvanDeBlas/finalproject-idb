import { describe, it, expect, vi, afterEach } from "vitest"
import {
    calcularDiasRestantes,
    calcularPorcentaje,
    formatCurrency,
    extractVideoId,
    getVideoEmbedUrl,
} from "../application/utils"

describe("calcularDiasRestantes", () => {
    afterEach(() => {
        vi.useRealTimers()
    })

    it("returns positive days for a future date", () => {
        // Arrange
        vi.useFakeTimers()
        vi.setSystemTime(new Date("2026-01-01T12:00:00Z"))
        const fechaFin = "2026-01-11T12:00:00Z"

        // Act
        const result = calcularDiasRestantes(fechaFin)

        // Assert
        expect(result).toBe(10)
    })

    it("returns 0 for a past date", () => {
        // Arrange
        vi.useFakeTimers()
        vi.setSystemTime(new Date("2026-03-15T12:00:00Z"))
        const fechaFin = "2026-01-01T12:00:00Z"

        // Act
        const result = calcularDiasRestantes(fechaFin)

        // Assert
        expect(result).toBe(0)
    })

    it("returns 0 for today", () => {
        // Arrange
        vi.useFakeTimers()
        const now = new Date("2026-06-15T12:00:00Z")
        vi.setSystemTime(now)
        const fechaFin = new Date("2026-06-15T12:00:00Z")

        // Act
        const result = calcularDiasRestantes(fechaFin)

        // Assert
        expect(result).toBe(0)
    })

    it("accepts a Date object as input", () => {
        // Arrange
        vi.useFakeTimers()
        vi.setSystemTime(new Date("2026-01-01T00:00:00Z"))
        const fechaFin = new Date("2026-01-06T00:00:00Z")

        // Act
        const result = calcularDiasRestantes(fechaFin)

        // Assert
        expect(result).toBe(5)
    })
})

describe("calcularPorcentaje", () => {
    it("calculates correct percentage", () => {
        // Arrange
        const pledged = 1250
        const objetivo = 5000

        // Act
        const result = calcularPorcentaje(pledged, objetivo)

        // Assert
        expect(result).toBe(25)
    })

    it("returns 0 when objetivo is 0", () => {
        // Arrange
        const pledged = 500
        const objetivo = 0

        // Act
        const result = calcularPorcentaje(pledged, objetivo)

        // Assert
        expect(result).toBe(0)
    })

    it("caps at 100% when over-funded", () => {
        // Arrange
        const pledged = 7500
        const objetivo = 5000

        // Act
        const result = calcularPorcentaje(pledged, objetivo)

        // Assert
        expect(result).toBe(100)
    })

    it("returns 0 when pledged is 0", () => {
        // Arrange
        const pledged = 0
        const objetivo = 5000

        // Act
        const result = calcularPorcentaje(pledged, objetivo)

        // Assert
        expect(result).toBe(0)
    })

    it("rounds to the nearest integer", () => {
        // Arrange
        const pledged = 333
        const objetivo = 1000

        // Act
        const result = calcularPorcentaje(pledged, objetivo)

        // Assert
        expect(result).toBe(33)
    })
})

describe("formatCurrency", () => {
    it("formats with EUR symbol by default (monedaId=1)", () => {
        // Arrange
        const amount = 5000

        // Act
        const result = formatCurrency(amount, 1)

        // Assert
        expect(result).toContain("\u20AC")
        // toLocaleString formatting varies by environment; just check the number is present
        expect(result).toContain("5000")
    })

    it("formats with locale es-ES using dot separator for thousands", () => {
        // Arrange
        const amount = 12500

        // Act
        const result = formatCurrency(amount)

        // Assert
        expect(result).toContain("12.500")
    })

    it("uses fallback EUR symbol for unknown monedaId", () => {
        // Arrange
        const amount = 100
        const unknownMonedaId = 99

        // Act
        const result = formatCurrency(amount, unknownMonedaId)

        // Assert
        expect(result).toContain("\u20AC")
    })

    it("formats with USD symbol when monedaId is 2", () => {
        // Arrange
        const amount = 3000

        // Act
        const result = formatCurrency(amount, 2)

        // Assert
        expect(result).toContain("$")
        expect(result).toContain("3000")
    })

    it("defaults monedaId to 1 (EUR) when not provided", () => {
        // Arrange
        const amount = 750

        // Act
        const result = formatCurrency(amount)

        // Assert
        expect(result).toBe("\u20AC 750")
    })
})

describe("extractVideoId", () => {
    it("extracts YouTube video ID from watch URL", () => {
        // Arrange
        const url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"

        // Act
        const result = extractVideoId(url)

        // Assert
        expect(result).toEqual({ type: "youtube", id: "dQw4w9WgXcQ" })
    })

    it("extracts YouTube video ID from short URL", () => {
        // Arrange
        const url = "https://youtu.be/dQw4w9WgXcQ"

        // Act
        const result = extractVideoId(url)

        // Assert
        expect(result).toEqual({ type: "youtube", id: "dQw4w9WgXcQ" })
    })

    it("extracts Vimeo video ID", () => {
        // Arrange
        const url = "https://vimeo.com/123456789"

        // Act
        const result = extractVideoId(url)

        // Assert
        expect(result).toEqual({ type: "vimeo", id: "123456789" })
    })

    it("returns null type for non-video URL", () => {
        // Arrange
        const url = "https://example.com/some-page"

        // Act
        const result = extractVideoId(url)

        // Assert
        expect(result).toEqual({ type: null, id: "" })
    })

    it("extracts YouTube ID from URL with extra parameters", () => {
        // Arrange
        const url = "https://www.youtube.com/watch?v=abc123&t=120"

        // Act
        const result = extractVideoId(url)

        // Assert
        expect(result).toEqual({ type: "youtube", id: "abc123" })
    })
})

describe("getVideoEmbedUrl", () => {
    it("returns YouTube embed URL", () => {
        // Arrange
        const url = "https://www.youtube.com/watch?v=dQw4w9WgXcQ"

        // Act
        const result = getVideoEmbedUrl(url)

        // Assert
        expect(result).toBe("https://www.youtube.com/embed/dQw4w9WgXcQ")
    })

    it("returns Vimeo embed URL", () => {
        // Arrange
        const url = "https://vimeo.com/123456789"

        // Act
        const result = getVideoEmbedUrl(url)

        // Assert
        expect(result).toBe("https://player.vimeo.com/video/123456789")
    })

    it("returns null for invalid URL", () => {
        // Arrange
        const url = "https://example.com/not-a-video"

        // Act
        const result = getVideoEmbedUrl(url)

        // Assert
        expect(result).toBeNull()
    })

    it("returns YouTube embed URL from short URL", () => {
        // Arrange
        const url = "https://youtu.be/abc123"

        // Act
        const result = getVideoEmbedUrl(url)

        // Assert
        expect(result).toBe("https://www.youtube.com/embed/abc123")
    })
})
