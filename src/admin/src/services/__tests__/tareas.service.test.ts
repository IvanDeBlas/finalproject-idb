import { describe, it, expect, vi, beforeEach } from "vitest"
import { tareasService } from "../tareas.service"
import { apiFetch } from "@/lib/api-client"
import {
    PROGRAMA_ID,
    mockTareasPendientesResponse,
    mockValidarTareaResponse,
    mockRechazarTareaResponse,
} from "@/__mocks__/cp-tareas-promocion.mock"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
}))

describe("TareasService", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    describe("getTareasPendientes", () => {
        it("calls correct URL without params", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockTareasPendientesResponse,
                messages: [],
            })

            await tareasService.getTareasPendientes(PROGRAMA_ID)

            expect(apiFetch).toHaveBeenCalledWith(
                `/api/crowdpromotion/programas/${PROGRAMA_ID}/tareas-pendientes`
            )
        })

        it("calls correct URL with pagination params", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockTareasPendientesResponse,
                messages: [],
            })

            await tareasService.getTareasPendientes(PROGRAMA_ID, { page: 2, pageSize: 5 })

            expect(apiFetch).toHaveBeenCalledWith(
                `/api/crowdpromotion/programas/${PROGRAMA_ID}/tareas-pendientes?page=2&pageSize=5`
            )
        })

        it("returns items from response", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockTareasPendientesResponse,
                messages: [],
            })

            const result = await tareasService.getTareasPendientes(PROGRAMA_ID)

            expect(result.items).toHaveLength(2)
            expect(result.totalCount).toBe(2)
            expect(result.items[0].promotorNombre).toBe("Maria Lopez")
        })

        it("throws on error response", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [{ message: "Error", errorCode: "5000" }],
            })

            await expect(
                tareasService.getTareasPendientes(PROGRAMA_ID)
            ).rejects.toThrow()
        })
    })

    describe("validarTarea", () => {
        it("calls PATCH with correct URL and body", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockValidarTareaResponse,
                messages: [],
            })

            const tareaPromotorId = "b2c3d4e5-f6a7-8901-bc23-de45fa678901"
            await tareasService.validarTarea(PROGRAMA_ID, tareaPromotorId, {
                comentarioValidacion: "Bien hecho",
            })

            expect(apiFetch).toHaveBeenCalledWith(
                `/api/crowdpromotion/programas/${PROGRAMA_ID}/tareas-promotor/${tareaPromotorId}/validar`,
                expect.objectContaining({
                    method: "PATCH",
                    data: { comentarioValidacion: "Bien hecho" },
                })
            )
        })
    })

    describe("rechazarTarea", () => {
        it("calls PATCH with correct URL and body", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockRechazarTareaResponse,
                messages: [],
            })

            const tareaPromotorId = "b2c3d4e5-f6a7-8901-bc23-de45fa678901"
            await tareasService.rechazarTarea(PROGRAMA_ID, tareaPromotorId, {
                comentarioValidacion: "La URL no corresponde",
            })

            expect(apiFetch).toHaveBeenCalledWith(
                `/api/crowdpromotion/programas/${PROGRAMA_ID}/tareas-promotor/${tareaPromotorId}/rechazar`,
                expect.objectContaining({
                    method: "PATCH",
                    data: { comentarioValidacion: "La URL no corresponde" },
                })
            )
        })
    })

    it("handles apiFetch error propagation", async () => {
        vi.mocked(apiFetch).mockRejectedValue(new Error("Network error"))

        await expect(
            tareasService.getTareasPendientes(PROGRAMA_ID)
        ).rejects.toThrow("Network error")
    })
})
