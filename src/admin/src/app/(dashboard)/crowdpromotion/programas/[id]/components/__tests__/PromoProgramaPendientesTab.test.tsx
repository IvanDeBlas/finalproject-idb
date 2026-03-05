import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import { PromoProgramaPendientesTab } from "../PromoProgramaPendientesTab"
import { tareasService } from "@/services/tareas.service"
import {
    PROGRAMA_ID,
    mockTareasPendientesResponse,
    mockTareasPendientesResponseVacia,
    mockTareasPendientesResponsePaginada,
} from "@/__mocks__/cp-tareas-promocion.mock"

vi.mock("@/services/tareas.service", () => ({
    tareasService: {
        getTareasPendientes: vi.fn(),
        validarTarea: vi.fn(),
        rechazarTarea: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: Object.assign(vi.fn(), {
        success: vi.fn(),
        error: vi.fn(),
    }),
}))

describe("PromoProgramaPendientesTab", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders loading skeleton", () => {
        vi.mocked(tareasService.getTareasPendientes).mockReturnValue(
            new Promise(() => {})
        )

        render(<PromoProgramaPendientesTab programaId={PROGRAMA_ID} />)

        expect(screen.getByLabelText("Cargando tareas pendientes")).toBeInTheDocument()
    })

    it("renders empty state when no pending tasks", async () => {
        vi.mocked(tareasService.getTareasPendientes).mockResolvedValue(
            mockTareasPendientesResponseVacia
        )

        render(<PromoProgramaPendientesTab programaId={PROGRAMA_ID} />)

        expect(
            await screen.findByText("No hay tareas pendientes de validacion")
        ).toBeInTheDocument()
    })

    it("renders list of pending tasks", async () => {
        vi.mocked(tareasService.getTareasPendientes).mockResolvedValue(
            mockTareasPendientesResponse
        )

        render(<PromoProgramaPendientesTab programaId={PROGRAMA_ID} />)

        expect(await screen.findByText("Maria Lopez")).toBeInTheDocument()
        expect(screen.getByText("Carlos Ruiz")).toBeInTheDocument()
    })

    it("shows pagination controls when totalPages > 1", async () => {
        vi.mocked(tareasService.getTareasPendientes).mockResolvedValue(
            mockTareasPendientesResponsePaginada
        )

        render(<PromoProgramaPendientesTab programaId={PROGRAMA_ID} />)

        expect(await screen.findByText(/Pagina 1 de 3/)).toBeInTheDocument()
        expect(screen.getByRole("button", { name: /Siguiente/i })).toBeInTheDocument()
    })

    it("handles error state", async () => {
        vi.mocked(tareasService.getTareasPendientes).mockRejectedValue(new Error("Fail"))

        render(<PromoProgramaPendientesTab programaId={PROGRAMA_ID} />)

        expect(
            await screen.findByText("Error al cargar las tareas pendientes")
        ).toBeInTheDocument()
        expect(screen.getByRole("button", { name: /Reintentar/i })).toBeInTheDocument()
    })

    it("shows total count in header", async () => {
        vi.mocked(tareasService.getTareasPendientes).mockResolvedValue(
            mockTareasPendientesResponse
        )

        render(<PromoProgramaPendientesTab programaId={PROGRAMA_ID} />)

        expect(
            await screen.findByText(/Tareas pendientes de validacion \(2\)/)
        ).toBeInTheDocument()
    })
})
