import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import { PromoProgramaBloqueadosTab } from "../PromoProgramaBloqueadosTab"
import { inscripcionService } from "@/services/inscripcion.service"
import {
    mockInscripcionesBloqueadasResponse,
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

describe("PromoProgramaBloqueadosTab", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders bloqueado promotor name", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesBloqueadasResponse
        )

        render(<PromoProgramaBloqueadosTab programaId="programa-1" />)

        expect(await screen.findByText("Spammer123")).toBeInTheDocument()
    })

    it("renders BLOQUEADO badge", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesBloqueadasResponse
        )

        render(<PromoProgramaBloqueadosTab programaId="programa-1" />)

        expect(await screen.findByText("BLOQUEADO")).toBeInTheDocument()
    })

    it("renders informative blue warning", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesBloqueadasResponse
        )

        render(<PromoProgramaBloqueadosTab programaId="programa-1" />)

        expect(
            await screen.findByText(/Los promotores bloqueados no pueden re-solicitar/)
        ).toBeInTheDocument()
    })

    it("renders empty state when no blocked promotores", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesVaciaResponse
        )

        render(<PromoProgramaBloqueadosTab programaId="programa-1" />)

        expect(
            await screen.findByText("No hay promotores bloqueados")
        ).toBeInTheDocument()
    })

    it("renders error state on fetch failure", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockRejectedValue(new Error("Fail"))

        render(<PromoProgramaBloqueadosTab programaId="programa-1" />)

        expect(
            await screen.findByText("Error al cargar los promotores bloqueados")
        ).toBeInTheDocument()
    })

    it("has no action buttons (read-only tab)", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesBloqueadasResponse
        )

        render(<PromoProgramaBloqueadosTab programaId="programa-1" />)

        await screen.findByText("Spammer123")

        expect(screen.queryByText("Dar de baja")).not.toBeInTheDocument()
        expect(screen.queryByText("Aprobar")).not.toBeInTheDocument()
        expect(screen.queryByText("Bloquear")).not.toBeInTheDocument()
    })
})
