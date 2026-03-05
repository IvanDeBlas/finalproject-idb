import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import { PromoProgramaSolicitudesTab } from "../PromoProgramaSolicitudesTab"
import { inscripcionService } from "@/services/inscripcion.service"
import {
    mockInscripcionesPendientesResponse,
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

describe("PromoProgramaSolicitudesTab", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders promotor name", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesPendientesResponse
        )

        render(<PromoProgramaSolicitudesTab programaId="programa-1" />)

        expect(await screen.findByText("DJ Marketing Pro")).toBeInTheDocument()
    })

    it("renders tipo promotor", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesPendientesResponse
        )

        render(<PromoProgramaSolicitudesTab programaId="programa-1" />)

        expect(await screen.findByText("Influencer")).toBeInTheDocument()
    })

    it("renders Aprobar button", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesPendientesResponse
        )

        render(<PromoProgramaSolicitudesTab programaId="programa-1" />)

        expect(await screen.findByText("Aprobar")).toBeInTheDocument()
    })

    it("renders Rechazar button", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesPendientesResponse
        )

        render(<PromoProgramaSolicitudesTab programaId="programa-1" />)

        expect(await screen.findByText("Rechazar")).toBeInTheDocument()
    })

    it("renders Bloquear button", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesPendientesResponse
        )

        render(<PromoProgramaSolicitudesTab programaId="programa-1" />)

        expect(await screen.findByText("Bloquear")).toBeInTheDocument()
    })

    it("renders empty state when no pending inscripciones", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesVaciaResponse
        )

        render(<PromoProgramaSolicitudesTab programaId="programa-1" />)

        expect(
            await screen.findByText("No hay solicitudes pendientes")
        ).toBeInTheDocument()
    })

    it("renders error state on fetch failure", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockRejectedValue(new Error("Fail"))

        render(<PromoProgramaSolicitudesTab programaId="programa-1" />)

        expect(
            await screen.findByText("Error al cargar las solicitudes")
        ).toBeInTheDocument()
    })

    it("renders Instagram link when present", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesPendientesResponse
        )

        render(<PromoProgramaSolicitudesTab programaId="programa-1" />)

        const link = await screen.findByText("https://instagram.com/djmarketing")
        expect(link).toBeInTheDocument()
        expect(link.closest("a")).toHaveAttribute("href", "https://instagram.com/djmarketing")
    })
})
