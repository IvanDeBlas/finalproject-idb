import { describe, it, expect, vi, beforeEach, afterEach } from "vitest"
import { renderHook } from "@testing-library/react"

vi.mock("../../metricas/infrastructure/tracking.service", () => ({
    trackingService: {
        registrarEvento: vi.fn(),
        getMetricasPromotor: vi.fn(),
    },
}))

import { trackingService } from "../../metricas/infrastructure/tracking.service"
import { useTrackingInterceptor } from "../../metricas/application/hooks/useTrackingInterceptor"

const originalLocation = window.location

function setLocationSearch(search: string, href?: string) {
    Object.defineProperty(window, "location", {
        value: {
            ...originalLocation,
            search,
            href: href ?? `http://localhost${search}`,
        },
        writable: true,
        configurable: true,
    })
}

function restoreLocation() {
    Object.defineProperty(window, "location", {
        value: originalLocation,
        writable: true,
        configurable: true,
    })
}

describe("useTrackingInterceptor", () => {
    beforeEach(() => {
        vi.clearAllMocks()
        sessionStorage.clear()
        document.cookie = "wp_ref=; max-age=0; path=/"
        vi.mocked(trackingService.registrarEvento).mockResolvedValue({
            eventoId: "test-id",
            registrado: true,
        })
    })

    afterEach(() => {
        restoreLocation()
    })

    it("does not call service when no ref param and no cookie", () => {
        setLocationSearch("")
        renderHook(() => useTrackingInterceptor())
        expect(trackingService.registrarEvento).not.toHaveBeenCalled()
    })

    it("detects ref param and calls service with tipo Click", () => {
        setLocationSearch("?ref=album-2026-x7k9m&utm_source=weplay")
        renderHook(() => useTrackingInterceptor())

        expect(trackingService.registrarEvento).toHaveBeenCalledWith(
            expect.objectContaining({
                tipoEventoPromoId: 1,
                codigoReferido: "album-2026-x7k9m",
                utmSource: "weplay",
            })
        )
    })

    it("persists ref in sessionStorage when detected in URL", () => {
        setLocationSearch("?ref=album-2026-x7k9m")
        renderHook(() => useTrackingInterceptor())

        expect(sessionStorage.getItem("wp_ref")).toBe("album-2026-x7k9m")
    })

    it("persists utm_source in sessionStorage", () => {
        setLocationSearch("?ref=test&utm_source=weplay")
        renderHook(() => useTrackingInterceptor())

        expect(sessionStorage.getItem("wp_utm_source")).toBe("weplay")
    })

    it("persists utm_medium in sessionStorage", () => {
        setLocationSearch("?ref=test&utm_medium=referral")
        renderHook(() => useTrackingInterceptor())

        expect(sessionStorage.getItem("wp_utm_medium")).toBe("referral")
    })

    it("persists utm_campaign in sessionStorage", () => {
        setLocationSearch("?ref=test&utm_campaign=album-2026")
        renderHook(() => useTrackingInterceptor())

        expect(sessionStorage.getItem("wp_utm_campaign")).toBe("album-2026")
    })

    it("writes wp_ref cookie when ref detected", () => {
        setLocationSearch("?ref=album-2026-x7k9m")
        renderHook(() => useTrackingInterceptor())

        expect(document.cookie).toContain("wp_ref=album-2026-x7k9m")
    })

    it("sends urlOrigen in the tracking payload", () => {
        const href = "https://weplay.com/campanias/xxx?ref=test"
        setLocationSearch("?ref=test", href)
        renderHook(() => useTrackingInterceptor())

        expect(trackingService.registrarEvento).toHaveBeenCalledWith(
            expect.objectContaining({ urlOrigen: href })
        )
    })

    it("handles 429 silently without retrying", async () => {
        const error = new Error("Rate limit") as Error & { errorCode: string }
        error.errorCode = "4032"
        vi.mocked(trackingService.registrarEvento).mockRejectedValue(error)

        setLocationSearch("?ref=test")
        renderHook(() => useTrackingInterceptor())

        // Wait for async operations
        await vi.waitFor(() => {
            expect(trackingService.registrarEvento).toHaveBeenCalledTimes(1)
        })
    })

    it("handles 400 error and clears sessionStorage", async () => {
        const error = new Error("Bad request") as Error & { errorCode: string }
        error.errorCode = "1033"
        vi.mocked(trackingService.registrarEvento).mockRejectedValue(error)

        setLocationSearch("?ref=test")
        renderHook(() => useTrackingInterceptor())

        await vi.waitFor(() => {
            expect(sessionStorage.getItem("wp_ref")).toBeNull()
        })
    })

    it("handles generic error silently", () => {
        vi.mocked(trackingService.registrarEvento).mockRejectedValue(
            new Error("Network error")
        )

        setLocationSearch("?ref=test")

        // Should not throw
        expect(() => renderHook(() => useTrackingInterceptor())).not.toThrow()
    })

    it("does not call service if ref is not in URL even with sessionStorage data", () => {
        sessionStorage.setItem("wp_ref", "existing-ref")
        setLocationSearch("")
        renderHook(() => useTrackingInterceptor())

        expect(trackingService.registrarEvento).not.toHaveBeenCalled()
    })

    it("calls service only once per mount", () => {
        setLocationSearch("?ref=test")
        renderHook(() => useTrackingInterceptor())

        expect(trackingService.registrarEvento).toHaveBeenCalledTimes(1)
    })
})
