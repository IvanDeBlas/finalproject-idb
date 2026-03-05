import { describe, it, expect, vi, beforeEach } from "vitest"
import { necesidadService } from "../necesidad.service"
import { apiFetch } from "@/lib/api-client"
import type { ServiceResponse } from "@shared/types"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
}))

describe("NecesidadService", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    describe("getMisNecesidades", () => {
        it("returns paginated list", async () => {
            const mockData = {
                items: [{ id: "n-1", titulo: "Necesidad 1" }],
                totalCount: 1,
                page: 1,
                pageSize: 12,
                totalPages: 1,
            }
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockData,
                messages: [],
            } as ServiceResponse<unknown>)

            const result = await necesidadService.getMisNecesidades()

            expect(result).toEqual(mockData)
            expect(apiFetch).toHaveBeenCalledWith(
                expect.stringContaining("/api/crowdsourcing/necesidades/mis-necesidades")
            )
        })

        it("appends query params when provided", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: { items: [], totalCount: 0, page: 2, pageSize: 10, totalPages: 0 },
                messages: [],
            } as ServiceResponse<unknown>)

            await necesidadService.getMisNecesidades({
                page: 2,
                pageSize: 10,
                estado: 1,
                search: "mezcla",
            })

            const calledUrl = vi.mocked(apiFetch).mock.calls[0][0] as string
            expect(calledUrl).toContain("page=2")
            expect(calledUrl).toContain("pageSize=10")
            expect(calledUrl).toContain("estado=1")
            expect(calledUrl).toContain("search=mezcla")
        })
    })

    describe("getById", () => {
        it("returns necesidad when found", async () => {
            const mockNecesidad = { id: "n-1", titulo: "Test" }
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockNecesidad,
                messages: [],
            } as ServiceResponse<unknown>)

            const result = await necesidadService.getById("n-1")

            expect(result).toEqual(mockNecesidad)
        })

        it("returns null when not found", async () => {
            vi.mocked(apiFetch).mockRejectedValue(new Error("Not found"))

            const result = await necesidadService.getById("nonexistent")

            expect(result).toBeNull()
        })
    })

    describe("create", () => {
        it("returns new id on success", async () => {
            const mockResult = { id: "new-id", titulo: "New" }
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockResult,
                messages: [],
            } as ServiceResponse<unknown>)

            const result = await necesidadService.create({
                titulo: "New",
                tipoNecesidadId: 3,
                modalidadTrabajoId: 2,
                proyectoArtisticoId: "proj-1",
            })

            expect(result).toEqual(mockResult)
            expect(apiFetch).toHaveBeenCalledWith(
                expect.stringContaining("/api/crowdsourcing/necesidades"),
                expect.objectContaining({ method: "POST" })
            )
        })
    })

    describe("update", () => {
        it("calls PUT with correct url", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: { id: "n-1" },
                messages: [],
            } as ServiceResponse<unknown>)

            await necesidadService.update("n-1", {
                titulo: "Updated",
                modalidadTrabajoId: 2,
            })

            expect(apiFetch).toHaveBeenCalledWith(
                expect.stringContaining("/api/crowdsourcing/necesidades/n-1"),
                expect.objectContaining({ method: "PUT" })
            )
        })
    })

    describe("cerrar", () => {
        it("calls PATCH with correct url", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: { propuestasRechazadas: 2 },
                messages: [],
            } as ServiceResponse<unknown>)

            await necesidadService.cerrar("n-1", { motivo: "Ya no necesito" })

            expect(apiFetch).toHaveBeenCalledWith(
                expect.stringContaining("n-1/cerrar"),
                expect.objectContaining({ method: "PATCH" })
            )
        })
    })
})
