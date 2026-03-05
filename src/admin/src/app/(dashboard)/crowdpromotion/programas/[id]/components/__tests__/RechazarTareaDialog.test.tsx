import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { toast } from "sonner"
import { RechazarTareaDialog } from "../RechazarTareaDialog"
import { tareasService } from "@/services/tareas.service"
import {
    PROGRAMA_ID,
    mockTareaPendienteItem,
    mockRechazarTareaResponse,
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

describe("RechazarTareaDialog", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders dialog with item info", () => {
        render(
            <RechazarTareaDialog
                isOpen={true}
                onClose={vi.fn()}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        expect(screen.getByText("Maria Lopez")).toBeInTheDocument()
        expect(screen.getByText("Comparte en Instagram Stories")).toBeInTheDocument()
        expect(screen.getByRole("heading", { name: "Rechazar tarea" })).toBeInTheDocument()
    })

    it("shows rejection reason textarea", () => {
        render(
            <RechazarTareaDialog
                isOpen={true}
                onClose={vi.fn()}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        expect(screen.getByRole("textbox")).toBeInTheDocument()
        expect(screen.getByText(/Motivo del rechazo/)).toBeInTheDocument()
    })

    it("validates reason is required on submit", async () => {
        const user = userEvent.setup()

        render(
            <RechazarTareaDialog
                isOpen={true}
                onClose={vi.fn()}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        await user.click(screen.getByRole("button", { name: /Rechazar tarea/i }))

        expect(
            await screen.findByText(/El motivo de rechazo es obligatorio/)
        ).toBeInTheDocument()
        expect(tareasService.rechazarTarea).not.toHaveBeenCalled()
    })

    it("shows character counter", async () => {
        const user = userEvent.setup()

        render(
            <RechazarTareaDialog
                isOpen={true}
                onClose={vi.fn()}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        await user.type(screen.getByRole("textbox"), "URL invalida")

        expect(screen.getByText(/12 \/ 500 caracteres/)).toBeInTheDocument()
    })

    it("shows warning message about rejection", () => {
        render(
            <RechazarTareaDialog
                isOpen={true}
                onClose={vi.fn()}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        expect(
            screen.getByText(/El promotor vera este motivo y podra re-enviar/)
        ).toBeInTheDocument()
    })

    it("submits with rejection reason", async () => {
        vi.mocked(tareasService.rechazarTarea).mockResolvedValue(mockRechazarTareaResponse)
        const user = userEvent.setup()

        render(
            <RechazarTareaDialog
                isOpen={true}
                onClose={vi.fn()}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        await user.type(
            screen.getByRole("textbox"),
            "La URL no corresponde a la tarea solicitada"
        )
        await user.click(screen.getByRole("button", { name: /Rechazar tarea/i }))

        await vi.waitFor(() => {
            expect(tareasService.rechazarTarea).toHaveBeenCalledWith(
                PROGRAMA_ID,
                mockTareaPendienteItem.tareaPromotorId,
                { comentarioValidacion: "La URL no corresponde a la tarea solicitada" }
            )
        })
    })

    it("calls onClose after successful rejection", async () => {
        vi.mocked(tareasService.rechazarTarea).mockResolvedValue(mockRechazarTareaResponse)
        const onClose = vi.fn()
        const user = userEvent.setup()

        render(
            <RechazarTareaDialog
                isOpen={true}
                onClose={onClose}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        await user.type(screen.getByRole("textbox"), "No corresponde a la tarea")
        await user.click(screen.getByRole("button", { name: /Rechazar tarea/i }))

        await vi.waitFor(() => {
            expect(toast.success).toHaveBeenCalledWith(
                expect.stringContaining("rechazada")
            )
            expect(onClose).toHaveBeenCalled()
        })
    })

    it("shows error toast on mutation error", async () => {
        vi.mocked(tareasService.rechazarTarea).mockRejectedValue(new Error("Network error"))
        const onClose = vi.fn()
        const user = userEvent.setup()

        render(
            <RechazarTareaDialog
                isOpen={true}
                onClose={onClose}
                item={mockTareaPendienteItem}
                programaId={PROGRAMA_ID}
            />
        )

        await user.type(screen.getByRole("textbox"), "No corresponde a la tarea")
        await user.click(screen.getByRole("button", { name: /Rechazar tarea/i }))

        await vi.waitFor(() => {
            expect(toast.error).toHaveBeenCalledWith(
                "No se pudo rechazar la tarea. Intentalo de nuevo."
            )
        })
        expect(onClose).not.toHaveBeenCalled()
    })
})
