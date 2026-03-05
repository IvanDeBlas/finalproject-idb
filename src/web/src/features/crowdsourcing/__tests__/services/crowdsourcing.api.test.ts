import { describe, it, expect, vi, beforeEach } from "vitest"
import { mockTemplatesList, mockTemplateDetail, mockGenerarResult } from "../../__mocks__/crowdsourcing.mock"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
}))

import { apiFetch } from "@/lib/api-client"
import { crowdsourcingApi } from "../../infrastructure/api/crowdsourcing.api"

describe("CrowdsourcingApiService", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    describe("getTemplates", () => {
        it("returns list of templates", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockTemplatesList,
                messages: [{ message: "OK", errorCode: "0000" }],
            })

            const result = await crowdsourcingApi.getTemplates()

            expect(result).toHaveLength(3)
            expect(result[0].nombre).toBe("Produccion de EP")
        })

        it("calls correct endpoint", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: [],
                messages: [],
            })

            await crowdsourcingApi.getTemplates()

            expect(apiFetch).toHaveBeenCalledWith("/crowdsourcing/templates")
        })

        it("returns empty array when data is null", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [],
            })

            const result = await crowdsourcingApi.getTemplates()

            expect(result).toEqual([])
        })
    })

    describe("getTemplateById", () => {
        it("returns template with necesidades", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockTemplateDetail,
                messages: [],
            })

            const result = await crowdsourcingApi.getTemplateById("tpl-001")

            expect(result.nombre).toBe("Produccion de EP")
            expect(result.necesidades).toHaveLength(3)
        })

        it("calls correct endpoint with id", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockTemplateDetail,
                messages: [],
            })

            await crowdsourcingApi.getTemplateById("tpl-001")

            expect(apiFetch).toHaveBeenCalledWith(
                "/crowdsourcing/templates/tpl-001"
            )
        })

        it("throws when data is null", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [{ message: "Not found", errorCode: "2006" }],
            })

            await expect(
                crowdsourcingApi.getTemplateById("invalid")
            ).rejects.toThrow("Template not found")
        })
    })

    describe("generarNecesidades", () => {
        const payload = {
            proyectoArtisticoId: "proj-001",
            necesidadesSeleccionadas: [
                {
                    plantillaNecesidadId: "nec-001",
                    presupuestoMin: 500,
                    presupuestoMax: 1500,
                    monedaId: 1,
                },
            ],
        }

        it("sends POST request with correct data", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockGenerarResult,
                messages: [],
            })

            await crowdsourcingApi.generarNecesidades("tpl-001", payload)

            expect(apiFetch).toHaveBeenCalledWith(
                "/crowdsourcing/templates/tpl-001/generar",
                {
                    method: "POST",
                    data: payload,
                }
            )
        })

        it("returns generation result", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: mockGenerarResult,
                messages: [],
            })

            const result = await crowdsourcingApi.generarNecesidades(
                "tpl-001",
                payload
            )

            expect(result.necesidadesCreadas).toBe(2)
            expect(result.necesidadIds).toHaveLength(2)
        })

        it("throws when data is null", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [{ message: "Error", errorCode: "5000" }],
            })

            await expect(
                crowdsourcingApi.generarNecesidades("tpl-001", payload)
            ).rejects.toThrow("Failed to generate necesidades")
        })
    })

    describe("getRolesProfesionales", () => {
        it("calls correct endpoint", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: [],
                messages: [],
            })

            await crowdsourcingApi.getRolesProfesionales()

            expect(apiFetch).toHaveBeenCalledWith(
                "/crowdsourcing/maestras/roles-profesionales"
            )
        })

        it("returns empty array when data is null", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: null,
                messages: [],
            })

            const result = await crowdsourcingApi.getRolesProfesionales()

            expect(result).toEqual([])
        })
    })

    describe("getCategoriasRol", () => {
        it("calls correct endpoint", async () => {
            vi.mocked(apiFetch).mockResolvedValue({
                data: [],
                messages: [],
            })

            await crowdsourcingApi.getCategoriasRol()

            expect(apiFetch).toHaveBeenCalledWith(
                "/crowdsourcing/maestras/categorias-rol"
            )
        })
    })
})
