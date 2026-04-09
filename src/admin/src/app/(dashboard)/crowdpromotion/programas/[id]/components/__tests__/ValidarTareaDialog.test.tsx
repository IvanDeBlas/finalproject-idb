import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { toast } from "sonner"
import { ValidarTareaDialog } from "../ValidarTareaDialog"
import { tareasService } from "@/services/tareas.service"
import {
    PROGRAMA_ID,
    mockTareaPendienteItem,
    mockValidarTareaResponse,
    mockValidarTareaResponseSinRecompensa,
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

describe("ValidarTareaDialog", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders dialog with item data read-only", () => {
        render(
            <ValidarTareaDialog
                isOpen={true}
                onClose={vi.fn()}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        expect(screen.getByText("Maria Lopez")).toBeInTheDocument()
        expect(screen.getByText("Comparte en Instagram Stories")).toBeInTheDocument()
        expect(screen.getByText("Validar tarea completada")).toBeInTheDocument()
    })

    it("renders proof URL as clickable link", () => {
        render(
            <ValidarTareaDialog
                isOpen={true}
                onClose={vi.fn()}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        const link = screen.getByRole("link")
        expect(link).toHaveAttribute("href", "https://instagram.com/stories/maria_promo_abc123")
        expect(link).toHaveAttribute("target", "_blank")
    })

    it("submits without comment (optional)", async () => {
        vi.mocked(tareasService.validarTarea).mockResolvedValue(mockValidarTareaResponse)
        const user = userEvent.setup()

        render(
            <ValidarTareaDialog
                isOpen={true}
                onClose={vi.fn()}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        await user.click(screen.getByRole("button", { name: /Validar tarea/i }))

        expect(tareasService.validarTarea).toHaveBeenCalledWith(
            PROGRAMA_ID,
            mockTareaPendienteItem.tareaPromotorId,
            { comentarioValidacion: undefined }
        )
    })

    it("submits with optional comment", async () => {
        vi.mocked(tareasService.validarTarea).mockResolvedValue(mockValidarTareaResponse)
        const user = userEvent.setup()

        render(
            <ValidarTareaDialog
                isOpen={true}
                onClose={vi.fn()}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        await user.type(screen.getByRole("textbox"), "Buen trabajo")
        await user.click(screen.getByRole("button", { name: /Validar tarea/i }))

        expect(tareasService.validarTarea).toHaveBeenCalledWith(
            PROGRAMA_ID,
            mockTareaPendienteItem.tareaPromotorId,
            { comentarioValidacion: "Buen trabajo" }
        )
    })

    it("calls onClose after successful mutation", async () => {
        vi.mocked(tareasService.validarTarea).mockResolvedValue(mockValidarTareaResponse)
        const onClose = vi.fn()
        const user = userEvent.setup()

        render(
            <ValidarTareaDialog
                isOpen={true}
                onClose={onClose}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        await user.click(screen.getByRole("button", { name: /Validar tarea/i }))

        await vi.waitFor(() => {
            expect(onClose).toHaveBeenCalled()
        })
    })

    it("shows success toast with reward info on success", async () => {
        vi.mocked(tareasService.validarTarea).mockResolvedValue(mockValidarTareaResponse)
        const user = userEvent.setup()

        render(
            <ValidarTareaDialog
                isOpen={true}
                onClose={vi.fn()}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        await user.click(screen.getByRole("button", { name: /Validar tarea/i }))

        await vi.waitFor(() => {
            expect(toast.success).toHaveBeenCalledWith(
                expect.stringContaining("5")
            )
        })
    })

    it("shows simple success toast when no reward", async () => {
        vi.mocked(tareasService.validarTarea).mockResolvedValue(mockValidarTareaResponseSinRecompensa)
        const user = userEvent.setup()

        render(
            <ValidarTareaDialog
                isOpen={true}
                onClose={vi.fn()}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        await user.click(screen.getByRole("button", { name: /Validar tarea/i }))

        await vi.waitFor(() => {
            expect(toast.success).toHaveBeenCalledWith(
                "Tarea validada exitosamente."
            )
        })
    })

    it("shows error toast on mutation error", async () => {
        vi.mocked(tareasService.validarTarea).mockRejectedValue(new Error("Server error"))
        const onClose = vi.fn()
        const user = userEvent.setup()

        render(
            <ValidarTareaDialog
                isOpen={true}
                onClose={onClose}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        await user.click(screen.getByRole("button", { name: /Validar tarea/i }))

        await vi.waitFor(() => {
            expect(toast.error).toHaveBeenCalledWith(
                "No se pudo validar la tarea. Intentalo de nuevo."
            )
        })
        expect(onClose).not.toHaveBeenCalled()
    })
})
