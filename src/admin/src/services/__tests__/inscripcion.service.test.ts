import { inscripcionService } from "../inscripcion.service"
import { apiFetch } from "@/lib/api-client"
import { API_ROUTES } from "@shared/constants"
import type { ServiceResponse } from "@shared/types"
import type {
    InscripcionesListResponse,
    InscripcionAprobada,
} from "@shared/types"
import {
    mockInscripcionesPendientesResponse,
    mockInscripcionesVaciaResponse,
    mockAprobadaResult,
    mockRechazadaResult,
    mockBloqueadaResult,
    mockDadaDeBajaResult,
} from "@/__mocks__/inscripcion.mock"

vi.mock("@/lib/api-client", () => ({
    apiFetch: vi.fn(),
    apiClient: {
        interceptors: {
            request: { use: vi.fn() },
            response: { use: vi.fn() },
        },
    },
}))

describe("inscripcionService.getInscripciones", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns list on success", async () => {
        const response: ServiceResponse<InscripcionesListResponse> = {
            data: mockInscripcionesPendientesResponse,
            messages: [],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        const result = await inscripcionService.getInscripciones("programa-1")

        expect(result.items).toHaveLength(1)
        expect(result.totalCount).toBe(1)
    })

    it("calls correct URL with programaId", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockInscripcionesPendientesResponse,
            messages: [],
        })

        await inscripcionService.getInscripciones("programa-1")

        expect(apiFetch).toHaveBeenCalledWith(
            API_ROUTES.crowdpromotion.programas.inscripciones("programa-1")
        )
    })

    it("appends estado filter when provided", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockInscripcionesPendientesResponse,
            messages: [],
        })

        await inscripcionService.getInscripciones("programa-1", { estado: "Pendiente" })

        const calledUrl = vi.mocked(apiFetch).mock.calls[0][0] as string
        expect(calledUrl).toContain("estado=Pendiente")
    })

    it("appends page and pageSize when provided", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockInscripcionesPendientesResponse,
            messages: [],
        })

        await inscripcionService.getInscripciones("programa-1", { page: 2, pageSize: 10 })

        const calledUrl = vi.mocked(apiFetch).mock.calls[0][0] as string
        expect(calledUrl).toContain("page=2")
        expect(calledUrl).toContain("pageSize=10")
    })

    it("returns empty list when no results", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockInscripcionesVaciaResponse,
            messages: [],
        })

        const result = await inscripcionService.getInscripciones("programa-1")

        expect(result.items).toHaveLength(0)
        expect(result.totalCount).toBe(0)
    })

    it("throws on error response", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: null,
            messages: [{ message: "Not authorized", errorCode: "4026" }],
        })

        await expect(
            inscripcionService.getInscripciones("programa-1")
        ).rejects.toThrow()
    })
})

describe("inscripcionService.aprobar", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("sends PATCH to correct URL", async () => {
        const response: ServiceResponse<InscripcionAprobada> = {
            data: mockAprobadaResult,
            messages: [{ message: "OK", errorCode: "0001" }],
        }
        vi.mocked(apiFetch).mockResolvedValue(response)

        await inscripcionService.aprobar({
            programaId: "programa-1",
            inscripcionId: "inscripcion-1",
        })

        expect(apiFetch).toHaveBeenCalledWith(
            API_ROUTES.crowdpromotion.programas.aprobar("programa-1", "inscripcion-1"),
            { method: "PATCH" }
        )
    })

    it("returns InscripcionAprobada on success", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockAprobadaResult,
            messages: [{ message: "OK", errorCode: "0001" }],
        })

        const result = await inscripcionService.aprobar({
            programaId: "programa-1",
            inscripcionId: "inscripcion-1",
        })

        expect(result.esAprobado).toBe(true)
        expect(result.codigoReferido).toBe("album-2026-x7k9m")
    })

    it("throws on error response (4025)", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: null,
            messages: [{ message: "Estado invalido", errorCode: "4025" }],
        })

        await expect(
            inscripcionService.aprobar({ programaId: "p", inscripcionId: "i" })
        ).rejects.toThrow()
    })

    it("propagates network error", async () => {
        vi.mocked(apiFetch).mockRejectedValue(new Error("Network error"))

        await expect(
            inscripcionService.aprobar({ programaId: "p", inscripcionId: "i" })
        ).rejects.toThrow("Network error")
    })
})

describe("inscripcionService.rechazar", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("sends PATCH to correct URL", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockRechazadaResult,
            messages: [{ message: "OK", errorCode: "0001" }],
        })

        await inscripcionService.rechazar({
            programaId: "programa-1",
            inscripcionId: "inscripcion-1",
        })

        expect(apiFetch).toHaveBeenCalledWith(
            API_ROUTES.crowdpromotion.programas.rechazar("programa-1", "inscripcion-1"),
            { method: "PATCH" }
        )
    })

    it("returns InscripcionRechazada on success", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockRechazadaResult,
            messages: [{ message: "OK", errorCode: "0001" }],
        })

        const result = await inscripcionService.rechazar({
            programaId: "programa-1",
            inscripcionId: "inscripcion-1",
        })

        expect(result.inscripcionId).toBe("inscripcion-1")
        expect(result.promotorNombre).toBe("DJ Marketing Pro")
    })

    it("throws on error response", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: null,
            messages: [{ message: "Error", errorCode: "4025" }],
        })

        await expect(
            inscripcionService.rechazar({ programaId: "p", inscripcionId: "i" })
        ).rejects.toThrow()
    })
})

describe("inscripcionService.bloquear", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("sends PATCH to correct URL", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockBloqueadaResult,
            messages: [{ message: "OK", errorCode: "0001" }],
        })

        await inscripcionService.bloquear({
            programaId: "programa-1",
            inscripcionId: "inscripcion-1",
        })

        expect(apiFetch).toHaveBeenCalledWith(
            API_ROUTES.crowdpromotion.programas.bloquear("programa-1", "inscripcion-1"),
            { method: "PATCH" }
        )
    })

    it("returns InscripcionBloqueada on success", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockBloqueadaResult,
            messages: [{ message: "OK", errorCode: "0001" }],
        })

        const result = await inscripcionService.bloquear({
            programaId: "programa-1",
            inscripcionId: "inscripcion-1",
        })

        expect(result.esBloqueado).toBe(true)
    })

    it("throws if already blocked (4025)", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: null,
            messages: [{ message: "Already blocked", errorCode: "4025" }],
        })

        await expect(
            inscripcionService.bloquear({ programaId: "p", inscripcionId: "i" })
        ).rejects.toThrow()
    })
})

describe("inscripcionService.darDeBaja", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("sends PATCH to correct URL", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockDadaDeBajaResult,
            messages: [{ message: "OK", errorCode: "0001" }],
        })

        await inscripcionService.darDeBaja({
            programaId: "programa-1",
            inscripcionId: "inscripcion-2",
        })

        expect(apiFetch).toHaveBeenCalledWith(
            API_ROUTES.crowdpromotion.programas.darDeBaja("programa-1", "inscripcion-2"),
            { method: "PATCH" }
        )
    })

    it("returns InscripcionDadaDeBaja on success", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: mockDadaDeBajaResult,
            messages: [{ message: "OK", errorCode: "0001" }],
        })

        const result = await inscripcionService.darDeBaja({
            programaId: "programa-1",
            inscripcionId: "inscripcion-2",
        })

        expect(result.esAprobado).toBe(false)
        expect(result.fechaBaja).toBeTruthy()
    })

    it("throws if not in aprobado state (4025)", async () => {
        vi.mocked(apiFetch).mockResolvedValue({
            data: null,
            messages: [{ message: "Not aprobado", errorCode: "4025" }],
        })

        await expect(
            inscripcionService.darDeBaja({ programaId: "p", inscripcionId: "i" })
        ).rejects.toThrow()
    })
})
