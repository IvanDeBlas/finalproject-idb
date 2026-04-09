import { describe, it, expect, vi, beforeEach } from "vitest"
import {
    mockMisTareasResponse_ConTareas,
    mockCompletarTareaResponse,
} from "../../__mocks__/tareas.mock"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
}))

import { apiFetch } from "@/lib/api-client"
import { tareasService } from "../../tareas/infrastructure/tareas.service"

describe("tareasService", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    describe("misTareas", () => {
        it("returns MisTareasResponse on successful fetch", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockMisTareasResponse_ConTareas,
                messages: [],
            })

            const result = await tareasService.misTareas("prog-001")

            expect(result).toEqual(mockMisTareasResponse_ConTareas)
        })

        it("calls the correct URL with programaId", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockMisTareasResponse_ConTareas,
                messages: [],
            })

            await tareasService.misTareas("prog-xyz")

            expect(apiFetch).toHaveBeenCalledWith(
                expect.stringContaining("/crowdpromotion/programas/prog-xyz/mis-tareas")
            )
        })

        it("throws when messages contain non-success errorCode", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [{ message: "Programa no encontrado", errorCode: "2019" }],
            })

            await expect(tareasService.misTareas("prog-001")).rejects.toThrow(
                "Programa no encontrado"
            )
        })

        it("throws with errorCode 4026 when promotor not approved", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [{ message: "No tienes permiso", errorCode: "4026" }],
            })

            try {
                await tareasService.misTareas("prog-001")
                expect.fail("Should have thrown")
            } catch (e: unknown) {
                expect((e as { errorCode?: string }).errorCode).toBe("4026")
            }
        })
    })

    describe("completarTarea", () => {
        const body = { urlPruebaCompletado: "https://instagram.com/p/test" }

        it("returns CompletarTareaResponse on successful POST", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockCompletarTareaResponse,
                messages: [{ message: "Tarea enviada", errorCode: "0001" }],
            })

            const result = await tareasService.completarTarea(
                "prog-001",
                "tarea-001",
                body
            )

            expect(result).toEqual(mockCompletarTareaResponse)
        })

        it("calls the correct endpoint with POST method", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockCompletarTareaResponse,
                messages: [{ message: "OK", errorCode: "0001" }],
            })

            await tareasService.completarTarea("prog-001", "tarea-001", body)

            expect(apiFetch).toHaveBeenCalledWith(
                expect.stringContaining(
                    "/programas/prog-001/tareas/tarea-001/completar"
                ),
                expect.objectContaining({ method: "POST" })
            )
        })

        it("sends comentarioPromotor in the body when provided", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockCompletarTareaResponse,
                messages: [{ message: "OK", errorCode: "0001" }],
            })

            const bodyWithComment = {
                urlPruebaCompletado: "https://test.com",
                comentarioPromotor: "Mi comentario",
            }

            await tareasService.completarTarea(
                "prog-001",
                "tarea-001",
                bodyWithComment
            )

            expect(apiFetch).toHaveBeenCalledWith(
                expect.any(String),
                expect.objectContaining({
                    data: bodyWithComment,
                })
            )
        })

        it("throws error 4027 when tarea is not repeatable", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [
                    {
                        message: "Tarea no repetible ya completada",
                        errorCode: "4027",
                    },
                ],
            })

            try {
                await tareasService.completarTarea("prog-001", "tarea-001", body)
                expect.fail("Should have thrown")
            } catch (e: unknown) {
                expect((e as { errorCode?: string }).errorCode).toBe("4027")
            }
        })

        it("throws error 4028 when max repetitions reached", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [
                    {
                        message: "Limite de repeticiones alcanzado",
                        errorCode: "4028",
                    },
                ],
            })

            try {
                await tareasService.completarTarea("prog-001", "tarea-001", body)
                expect.fail("Should have thrown")
            } catch (e: unknown) {
                expect((e as { errorCode?: string }).errorCode).toBe("4028")
            }
        })

        it("throws on network error", async () => {
            vi.mocked(apiFetch).mockRejectedValue(new Error("Network error"))

            await expect(
                tareasService.completarTarea("prog-001", "tarea-001", body)
            ).rejects.toThrow("Network error")
        })
    })
})
