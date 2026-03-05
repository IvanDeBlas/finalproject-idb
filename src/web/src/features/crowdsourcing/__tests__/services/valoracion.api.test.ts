import { describe, it, expect, vi, beforeEach } from "vitest"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
}))

import { apiFetch } from "@/lib/api-client"
import { valoracionApi } from "../../infrastructure/api/valoracion.api"
import {
    MOCK_ACUERDO_ID,
    MOCK_USER_ID,
    mockValoracionCreada,
    mockValoracionesUsuarioConDatos,
    mockValoracionesUsuarioVacio,
} from "../../__mocks__/valoracion.mock"

describe("valoracionApi.create", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls correct endpoint with POST method", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockValoracionCreada,
            messages: [
                {
                    message: "Valoracion enviada. Gracias por tu feedback.",
                    errorCode: "0001",
                },
            ],
        })

        await valoracionApi.create(MOCK_ACUERDO_ID, {
            puntuacion: 5,
            comentario: "Excelente",
        })

        expect(apiFetch).toHaveBeenCalledWith(
            expect.stringContaining(
                `/api/crowdsourcing/acuerdos/${MOCK_ACUERDO_ID}/valoraciones`
            ),
            expect.objectContaining({ method: "POST" })
        )
    })

    it("sends puntuacion and comentario in request body", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockValoracionCreada,
            messages: [],
        })

        await valoracionApi.create(MOCK_ACUERDO_ID, {
            puntuacion: 5,
            comentario: "Buen trabajo",
        })

        expect(apiFetch).toHaveBeenCalledWith(
            expect.anything(),
            expect.objectContaining({
                data: { puntuacion: 5, comentario: "Buen trabajo" },
            })
        )
    })

    it("returns created valoracion data on success", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockValoracionCreada,
            messages: [],
        })

        const result = await valoracionApi.create(MOCK_ACUERDO_ID, {
            puntuacion: 5,
        })

        expect(result).toEqual(mockValoracionCreada)
    })

    it("throws when API returns error (no data)", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: null,
            messages: [
                { message: "Ya valorado", errorCode: "4014" },
            ],
        })

        await expect(
            valoracionApi.create(MOCK_ACUERDO_ID, { puntuacion: 5 })
        ).rejects.toThrow("4014")
    })
})

describe("valoracionApi.getByUser", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls correct endpoint with userId in path", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockValoracionesUsuarioConDatos,
            messages: [],
        })

        await valoracionApi.getByUser(MOCK_USER_ID)

        expect(apiFetch).toHaveBeenCalledWith(
            expect.stringContaining(
                `/api/crowdsourcing/usuarios/${MOCK_USER_ID}/valoraciones`
            )
        )
    })

    it("sends page and pageSize as query params", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockValoracionesUsuarioConDatos,
            messages: [],
        })

        await valoracionApi.getByUser(MOCK_USER_ID, {
            page: 2,
            pageSize: 5,
        })

        const calledUrl = vi.mocked(apiFetch).mock.calls[0][0] as string
        expect(calledUrl).toContain("page=2")
        expect(calledUrl).toContain("pageSize=5")
    })

    it("returns resumen and paginated valoraciones on success", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockValoracionesUsuarioConDatos,
            messages: [],
        })

        const result = await valoracionApi.getByUser(MOCK_USER_ID)

        expect(result.resumen).toBeDefined()
        expect(result.valoraciones).toBeDefined()
        expect(result.valoraciones.items).toHaveLength(2)
    })

    it("throws when API returns error (no data)", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: null,
            messages: [
                { message: "User not found", errorCode: "2000" },
            ],
        })

        await expect(
            valoracionApi.getByUser(MOCK_USER_ID)
        ).rejects.toThrow("2000")
    })

    it("uses default page 1 and pageSize 10 when params not provided", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockValoracionesUsuarioConDatos,
            messages: [],
        })

        await valoracionApi.getByUser(MOCK_USER_ID)

        const calledUrl = vi.mocked(apiFetch).mock.calls[0][0] as string
        // When page=1 and pageSize=10, the implementation doesn't append them
        expect(calledUrl).not.toContain("page=")
        expect(calledUrl).not.toContain("pageSize=")
    })

    it("returns empty valoraciones list when user has no ratings", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockValoracionesUsuarioVacio,
            messages: [],
        })

        const result = await valoracionApi.getByUser(MOCK_USER_ID)

        expect(result.resumen.totalValoraciones).toBe(0)
        expect(result.valoraciones.items).toHaveLength(0)
    })
})
