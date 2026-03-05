import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import { PromoProgramaAprobadosTab } from "../PromoProgramaAprobadosTab"
import { inscripcionService } from "@/services/inscripcion.service"
import {
    mockInscripcionesAprobadasResponse,
    mockInscripcionesVaciaResponse,
} from "@/__mocks__/inscripcion.mock"

vi.mock("@/services/inscripcion.service", () => ({
    inscripcionService: {
        getInscripciones: vi.fn(),
        aprobar: vi.fn(),
        rechazar: vi.fn(),
        bloquear: vi.fn(),
        darDeBaja: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: Object.assign(vi.fn(), {
        success: vi.fn(),
        error: vi.fn(),
    }),
}))

describe("PromoProgramaAprobadosTab", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders promotor name", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesAprobadasResponse
        )

        render(<PromoProgramaAprobadosTab programaId="programa-1" />)

        expect(await screen.findByText("MusicBlog.es")).toBeInTheDocument()
    })

    it("renders codigoReferido when present", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesAprobadasResponse
        )

        render(<PromoProgramaAprobadosTab programaId="programa-1" />)

        expect(await screen.findByText("album-2026-m3k2n")).toBeInTheDocument()
    })

    it("renders Dar de baja button per row", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesAprobadasResponse
        )

        render(<PromoProgramaAprobadosTab programaId="programa-1" />)

        expect(await screen.findByText("Dar de baja")).toBeInTheDocument()
    })

    it("renders empty state when no approved promotores", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesVaciaResponse
        )

        render(<PromoProgramaAprobadosTab programaId="programa-1" />)

        expect(
            await screen.findByText("No hay promotores aprobados en este programa")
        ).toBeInTheDocument()
    })

    it("renders error state on fetch failure", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockRejectedValue(new Error("Fail"))

        render(<PromoProgramaAprobadosTab programaId="programa-1" />)

        expect(
            await screen.findByText("Error al cargar los promotores aprobados")
        ).toBeInTheDocument()
    })
})
