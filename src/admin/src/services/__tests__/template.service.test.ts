import { describe, it, expect, vi, beforeEach } from "vitest"
import { templateService } from "../template.service"
import { apiFetch } from "@/lib/api-client"
import type { ServiceResponse } from "@shared/types"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
}))

describe("TemplateService", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    describe("getAll", () => {
        it("returns list of templates", async () => {
            const mockData = [
                { id: "t-1", nombre: "EP Template" },
                { id: "t-2", nombre: "Album Template" },
            ]
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockData,
                messages: [],
            } as ServiceResponse<unknown>)

            const result = await templateService.getAll()

            expect(result).toEqual(mockData)
            expect(apiFetch).toHaveBeenCalledWith("/crowdsourcing/templates")
        })
    })

    describe("getById", () => {
        it("returns template when found", async () => {
            const mockTemplate = { id: "t-1", nombre: "EP Template" }
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockTemplate,
                messages: [],
            } as ServiceResponse<unknown>)

            const result = await templateService.getById("t-1")

            expect(result).toEqual(mockTemplate)
        })

        it("returns null when not found", async () => {
            vi.mocked(apiFetch).mockRejectedValue(new Error("Not found"))

            const result = await templateService.getById("nonexistent")

            expect(result).toBeNull()
        })
    })

    describe("create", () => {
        it("returns new id on success", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: "new-id-123",
                messages: [],
            } as ServiceResponse<unknown>)

            const result = await templateService.create({
                nombre: "New Template",
                icono: "music",
                orden: 1,
                activo: true,
                necesidades: [],
            })

            expect(result).toBe("new-id-123")
        })
    })

    describe("update", () => {
        it("calls PUT with correct url", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: "t-1",
                messages: [],
            } as ServiceResponse<unknown>)

            await templateService.update("t-1", {
                nombre: "Updated",
                icono: "disc",
                orden: 2,
                activo: true,
                necesidades: [],
            })

            expect(apiFetch).toHaveBeenCalledWith(
                "/crowdsourcing/templates/t-1",
                expect.objectContaining({ method: "PUT" })
            )
        })
    })

    describe("delete", () => {
        it("calls DELETE with correct url", async () => {
            vi.mocked(apiFetch).mockResolvedValue(undefined)

            await templateService.delete("t-1")

            expect(apiFetch).toHaveBeenCalledWith(
                "/crowdsourcing/templates/t-1",
                expect.objectContaining({ method: "DELETE" })
            )
        })
    })

    describe("toggleStatus", () => {
        it("calls PATCH with correct url", async () => {
            vi.mocked(apiFetch).mockResolvedValue(undefined)

            await templateService.toggleStatus("t-1")

            expect(apiFetch).toHaveBeenCalledWith(
                "/crowdsourcing/templates/t-1/toggle-status",
                expect.objectContaining({ method: "PATCH" })
            )
        })
    })
})
