import { describe, it, expect, vi, beforeEach } from "vitest"
import { apiFetch } from "@/lib/api-client"
import { walletService } from "../../wallet/infrastructure/wallet.service"
import {
    mockWallet,
    mockTransaccionesPagedResponse,
    mockSolicitarCobroResponse,
} from "../../__mocks__/wallet.mock"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
}))

const mockApiFetch = vi.mocked(apiFetch)

describe("WalletService", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    // ========== getWalletResumen ==========

    describe("getWalletResumen", () => {
        it("returns wallet data on success", async () => {
            mockApiFetch.mockResolvedValueOnce({
                data: mockWallet,
                messages: [{ message: "OK", errorCode: "0000" }],
            })

            const result = await walletService.getWalletResumen()

            expect(result).toEqual(mockWallet)
            expect(mockApiFetch).toHaveBeenCalledWith(
                "/crowdpromotion/promotor/wallet"
            )
        })

        it("throws error with errorCode on backend error", async () => {
            mockApiFetch.mockResolvedValueOnce({
                data: null,
                messages: [{ message: "Wallet no encontrado", errorCode: "2030" }],
            })

            await expect(walletService.getWalletResumen()).rejects.toThrow("Wallet no encontrado")

            try {
                mockApiFetch.mockResolvedValueOnce({
                    data: null,
                    messages: [{ message: "Wallet no encontrado", errorCode: "2030" }],
                })
                await walletService.getWalletResumen()
            } catch (error) {
                expect((error as Error & { errorCode: string }).errorCode).toBe("2030")
            }
        })

        it("throws on network error", async () => {
            mockApiFetch.mockRejectedValueOnce(new Error("Network Error"))

            await expect(walletService.getWalletResumen()).rejects.toThrow("Network Error")
        })
    })

    // ========== getTransacciones ==========

    describe("getTransacciones", () => {
        it("returns paged response on success", async () => {
            mockApiFetch.mockResolvedValueOnce({
                data: mockTransaccionesPagedResponse,
                messages: [{ message: "OK", errorCode: "0000" }],
            })

            const result = await walletService.getTransacciones({ page: 1, pageSize: 10 })

            expect(result).toEqual(mockTransaccionesPagedResponse)
        })

        it("passes filters as query parameters", async () => {
            mockApiFetch.mockResolvedValueOnce({
                data: mockTransaccionesPagedResponse,
                messages: [{ message: "OK", errorCode: "0000" }],
            })

            await walletService.getTransacciones({
                esCredito: true,
                estadoTransaccionId: 2,
                fechaDesde: "2026-01-01",
                fechaHasta: "2026-02-28",
                page: 2,
                pageSize: 20,
            })

            const calledUrl = mockApiFetch.mock.calls[0][0] as string
            expect(calledUrl).toContain("esCredito=true")
            expect(calledUrl).toContain("estadoTransaccionId=2")
            expect(calledUrl).toContain("fechaDesde=2026-01-01")
            expect(calledUrl).toContain("fechaHasta=2026-02-28")
            expect(calledUrl).toContain("page=2")
            expect(calledUrl).toContain("pageSize=20")
        })

        it("uses default page and pageSize when not provided", async () => {
            mockApiFetch.mockResolvedValueOnce({
                data: mockTransaccionesPagedResponse,
                messages: [{ message: "OK", errorCode: "0000" }],
            })

            await walletService.getTransacciones({})

            const calledUrl = mockApiFetch.mock.calls[0][0] as string
            expect(calledUrl).toContain("page=1")
            expect(calledUrl).toContain("pageSize=10")
        })

        it("throws error with errorCode on backend error", async () => {
            mockApiFetch.mockResolvedValueOnce({
                data: null,
                messages: [{ message: "Error inesperado", errorCode: "5000" }],
            })

            await expect(
                walletService.getTransacciones({ page: 1, pageSize: 10 })
            ).rejects.toThrow("Error inesperado")
        })
    })

    // ========== solicitarCobro ==========

    describe("solicitarCobro", () => {
        it("returns response on success", async () => {
            mockApiFetch.mockResolvedValueOnce({
                data: mockSolicitarCobroResponse,
                messages: [{ message: "Creado", errorCode: "0001" }],
            })

            const result = await walletService.solicitarCobro({
                importe: 50,
                descripcion: "Cobro mensual",
            })

            expect(result).toEqual(mockSolicitarCobroResponse)
            expect(mockApiFetch).toHaveBeenCalledWith(
                "/crowdpromotion/promotor/wallet/cobro",
                { method: "POST", data: { importe: 50, descripcion: "Cobro mensual" } }
            )
        })

        it("throws error 4040 on insufficient balance", async () => {
            mockApiFetch.mockResolvedValueOnce({
                data: null,
                messages: [{ message: "Saldo insuficiente", errorCode: "4040" }],
            })

            try {
                await walletService.solicitarCobro({ importe: 1000 })
            } catch (error) {
                expect((error as Error & { errorCode: string }).errorCode).toBe("4040")
                expect((error as Error).message).toBe("Saldo insuficiente")
            }
        })

        it("throws error 4042 on concurrent withdrawal", async () => {
            mockApiFetch.mockResolvedValueOnce({
                data: null,
                messages: [{ message: "Cobro pendiente", errorCode: "4042" }],
            })

            try {
                await walletService.solicitarCobro({ importe: 50 })
            } catch (error) {
                expect((error as Error & { errorCode: string }).errorCode).toBe("4042")
            }
        })

        it("throws on network error", async () => {
            mockApiFetch.mockRejectedValueOnce(new Error("Network Error"))

            await expect(
                walletService.solicitarCobro({ importe: 50 })
            ).rejects.toThrow("Network Error")
        })
    })
})
