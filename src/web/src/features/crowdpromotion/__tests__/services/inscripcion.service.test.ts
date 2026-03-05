import { describe, it, expect, vi, beforeEach } from "vitest"
import { mockPaginatedProgramas, mockInscripcionCreada, mockPaginatedMisProgramas } from "../../__mocks__/inscripcion.mock"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
}))

import { apiFetch } from "@/lib/api-client"
import { inscripcionService } from "../../infrastructure/inscripcion.service"

describe("inscripcionService", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    describe("explorarProgramas", () => {
        it("returns paginated list", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockPaginatedProgramas,
                messages: [],
            })

            const result = await inscripcionService.explorarProgramas()

            expect(result).toEqual(mockPaginatedProgramas)
            expect(apiFetch).toHaveBeenCalledWith(
                expect.stringContaining("/crowdpromotion/programas/explorar")
            )
        })

        it("sends artistaNombre filter as query param", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockPaginatedProgramas,
                messages: [],
            })

            await inscripcionService.explorarProgramas({ artistaNombre: "Luna Nova" })

            const calledUrl = vi.mocked(apiFetch).mock.calls[0][0] as string
            expect(calledUrl).toContain("artistaNombre=Luna+Nova")
        })

        it("sends tipoPromoId filter as query param", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockPaginatedProgramas,
                messages: [],
            })

            await inscripcionService.explorarProgramas({ tipoPromoId: 2 })

            const calledUrl = vi.mocked(apiFetch).mock.calls[0][0] as string
            expect(calledUrl).toContain("tipoPromoId=2")
        })

        it("sends pagination params", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockPaginatedProgramas,
                messages: [],
            })

            await inscripcionService.explorarProgramas({ page: 3, pageSize: 20 })

            const calledUrl = vi.mocked(apiFetch).mock.calls[0][0] as string
            expect(calledUrl).toContain("page=3")
            expect(calledUrl).toContain("pageSize=20")
        })

        it("omits undefined filters", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockPaginatedProgramas,
                messages: [],
            })

            await inscripcionService.explorarProgramas({})

            const calledUrl = vi.mocked(apiFetch).mock.calls[0][0] as string
            expect(calledUrl).not.toContain("artistaNombre")
            expect(calledUrl).not.toContain("tipoPromoId")
        })
    })

    describe("solicitarInscripcion", () => {
        it("returns inscripcion creada on 201", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockInscripcionCreada,
                messages: [{ message: "Solicitud enviada al artista", errorCode: "0001" }],
            })

            const result = await inscripcionService.solicitarInscripcion("3fa85f64-5717-4562-b3fc-2c963f66afa6")

            expect(result).toEqual(mockInscripcionCreada)
            expect(apiFetch).toHaveBeenCalledWith(
                expect.stringContaining("/crowdpromotion/programas/3fa85f64-5717-4562-b3fc-2c963f66afa6/inscripcion"),
                expect.objectContaining({ method: "POST" })
            )
        })

        it("throws on 400 (ya inscrito)", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [{ message: "Ya estas inscrito en este programa", errorCode: "4021" }],
            })

            await expect(
                inscripcionService.solicitarInscripcion("3fa85f64-5717-4562-b3fc-2c963f66afa6")
            ).rejects.toThrow("Ya estas inscrito en este programa")
        })

        it("throws on 403 (bloqueado)", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [{ message: "No puedes inscribirte en este programa", errorCode: "4022" }],
            })

            await expect(
                inscripcionService.solicitarInscripcion("3fa85f64-5717-4562-b3fc-2c963f66afa6")
            ).rejects.toThrow("No puedes inscribirte en este programa")
        })
    })

    describe("misProgramas", () => {
        it("returns paginated inscripciones", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockPaginatedMisProgramas,
                messages: [],
            })

            const result = await inscripcionService.misProgramas(1, 50)

            expect(result).toEqual(mockPaginatedMisProgramas)
            expect(apiFetch).toHaveBeenCalledWith(
                expect.stringContaining("mis-programas")
            )
        })

        it("codigoReferido and url only on aprobado", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockPaginatedMisProgramas,
                messages: [],
            })

            const result = await inscripcionService.misProgramas()

            const aprobado = result.items.find(i => i.estado === "Aprobado")
            expect(aprobado?.codigoReferido).toBe("album-2026-x7k9m")
            expect(aprobado?.urlTrackingPersonalizada).toContain("utm_source=weplay")

            const pendiente = result.items.find(i => i.estado === "Pendiente")
            expect(pendiente?.codigoReferido).toBeNull()
            expect(pendiente?.urlTrackingPersonalizada).toBeNull()
        })
    })
})
