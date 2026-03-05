import { describe, it, expect, vi, beforeEach } from "vitest"
import { mockRegistrarEventoResponse, mockPromotorMetricasResponse } from "../../__mocks__/tracking.mock"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
}))

import { apiFetch } from "@/lib/api-client"
import { trackingService } from "../../metricas/infrastructure/tracking.service"

describe("trackingService", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    describe("registrarEvento", () => {
        it("returns RegistrarEventoResponse on success", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockRegistrarEventoResponse,
                messages: [{ message: "OK", errorCode: "0001" }],
            })

            const result = await trackingService.registrarEvento({
                tipoEventoPromoId: 1,
                codigoReferido: "test-ref",
            })

            expect(result.eventoId).toBe("b2c3d4e5-f6a7-8901-bc23-de45fa678901")
            expect(result.registrado).toBe(true)
        })

        it("calls the correct endpoint with POST", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockRegistrarEventoResponse,
                messages: [],
            })

            await trackingService.registrarEvento({ tipoEventoPromoId: 1 })

            expect(apiFetch).toHaveBeenCalledWith(
                expect.stringContaining("/crowdpromotion/tracking/evento"),
                expect.objectContaining({ method: "POST" })
            )
        })

        it("sends the full payload in the body", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockRegistrarEventoResponse,
                messages: [],
            })

            const payload = {
                tipoEventoPromoId: 1 as const,
                codigoReferido: "my-ref",
                utmSource: "weplay",
                utmMedium: "referral",
                utmCampaign: "test-campaign",
            }

            await trackingService.registrarEvento(payload)

            expect(apiFetch).toHaveBeenCalledWith(
                expect.any(String),
                expect.objectContaining({ data: payload })
            )
        })

        it("sends minimal payload with only tipoEventoPromoId", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockRegistrarEventoResponse,
                messages: [],
            })

            await trackingService.registrarEvento({ tipoEventoPromoId: 2 })

            expect(apiFetch).toHaveBeenCalledWith(
                expect.any(String),
                expect.objectContaining({
                    data: { tipoEventoPromoId: 2 },
                })
            )
        })

        it("throws error with errorCode 1033 for invalid event type", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [{ message: "Tipo no reconocido", errorCode: "1033" }],
            })

            try {
                await trackingService.registrarEvento({ tipoEventoPromoId: 99 as never })
                expect.fail("Should have thrown")
            } catch (e: unknown) {
                expect((e as { errorCode?: string }).errorCode).toBe("1033")
            }
        })

        it("throws error with errorCode 4032 for rate limit", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [{ message: "Rate limit", errorCode: "4032" }],
            })

            try {
                await trackingService.registrarEvento({ tipoEventoPromoId: 1 })
                expect.fail("Should have thrown")
            } catch (e: unknown) {
                expect((e as { errorCode?: string }).errorCode).toBe("4032")
            }
        })

        it("throws on network error", async () => {
            vi.mocked(apiFetch).mockRejectedValue(new Error("Network error"))

            await expect(
                trackingService.registrarEvento({ tipoEventoPromoId: 1 })
            ).rejects.toThrow("Network error")
        })
    })

    describe("getMetricasPromotor", () => {
        it("returns PromotorMetricasResponse on success", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockPromotorMetricasResponse,
                messages: [],
            })

            const result = await trackingService.getMetricasPromotor("prog-001")

            expect(result).toEqual(mockPromotorMetricasResponse)
        })

        it("calls the correct endpoint with programaId in query", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockPromotorMetricasResponse,
                messages: [],
            })

            await trackingService.getMetricasPromotor("prog-001")

            expect(apiFetch).toHaveBeenCalledWith(
                expect.stringContaining("programaId=prog-001")
            )
        })

        it("calls endpoint without programaId when undefined", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockPromotorMetricasResponse,
                messages: [],
            })

            await trackingService.getMetricasPromotor(undefined)

            const calledUrl = vi.mocked(apiFetch).mock.calls[0][0] as string
            expect(calledUrl).not.toContain("programaId")
        })

        it("throws error 3001 when not authenticated", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [{ message: "No autorizado", errorCode: "3001" }],
            })

            try {
                await trackingService.getMetricasPromotor("prog-001")
                expect.fail("Should have thrown")
            } catch (e: unknown) {
                expect((e as { errorCode?: string }).errorCode).toBe("3001")
            }
        })

        it("throws error 5000 on server error", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [{ message: "Error interno", errorCode: "5000" }],
            })

            try {
                await trackingService.getMetricasPromotor("prog-001")
                expect.fail("Should have thrown")
            } catch (e: unknown) {
                expect((e as { errorCode?: string }).errorCode).toBe("5000")
            }
        })

        it("throws on network error", async () => {
            vi.mocked(apiFetch).mockRejectedValue(new Error("Network error"))

            await expect(
                trackingService.getMetricasPromotor("prog-001")
            ).rejects.toThrow("Network error")
        })
    })
})
