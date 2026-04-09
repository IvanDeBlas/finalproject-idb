import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { RewardDeleteDialog } from "../RewardDeleteDialog"
import { toast } from "sonner"
import type { Reward } from "@shared/types"

const mockDeleteReward = vi.fn()

vi.mock("@/hooks/use-rewards", () => ({
    useDeleteReward: () => ({
        mutate: mockDeleteReward,
        isPending: false,
    }),
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
        info: vi.fn(),
    },
}))

const baseReward: Reward = {
    id: "reward-1",
    campaniaId: "campania-1",
    tipoRewardId: 1,
    nombre: "Descarga Digital",
    importeMinimo: 10,
    monedaId: 1,
    esAddOn: false,
    incluyeEnvioFisico: false,
    orden: 1,
    esActivo: true,
    fechaCreacion: "2026-01-01T00:00:00Z",
}

describe("RewardDeleteDialog", () => {
    const defaultProps = {
        isOpen: true,
        onClose: vi.fn(),
        reward: baseReward,
    }

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders dialog with reward name", () => {
        render(<RewardDeleteDialog {...defaultProps} />)

        expect(screen.getByText("Eliminar Recompensa")).toBeInTheDocument()
        expect(screen.getByText(/Descarga Digital/)).toBeInTheDocument()
    })

    it("shows reward price", () => {
        render(<RewardDeleteDialog {...defaultProps} />)

        expect(screen.getByText(/10\.00/)).toBeInTheDocument()
    })

    it("shows warning text", () => {
        render(<RewardDeleteDialog {...defaultProps} />)

        expect(
            screen.getByText(/Esta accion no se puede deshacer/)
        ).toBeInTheDocument()
    })

    it("shows confirmation question", () => {
        render(<RewardDeleteDialog {...defaultProps} />)

        expect(
            screen.getByText(/Estas seguro que deseas eliminar/)
        ).toBeInTheDocument()
    })

    it("renders Cancelar and Eliminar buttons", () => {
        render(<RewardDeleteDialog {...defaultProps} />)

        expect(screen.getByText("Cancelar")).toBeInTheDocument()
        expect(screen.getByText("Eliminar")).toBeInTheDocument()
    })

    it("calls delete mutation when Eliminar clicked", async () => {
        const user = userEvent.setup()

        render(<RewardDeleteDialog {...defaultProps} />)

        await user.click(screen.getByText("Eliminar"))

        expect(mockDeleteReward).toHaveBeenCalledWith(
            "reward-1",
            expect.objectContaining({
                onSuccess: expect.any(Function),
                onError: expect.any(Function),
            })
        )
    })

    it("calls onClose on success callback", async () => {
        const onClose = vi.fn()
        mockDeleteReward.mockImplementation((_id: string, options: { onSuccess: () => void }) => {
            options.onSuccess()
        })

        const user = userEvent.setup()

        render(<RewardDeleteDialog {...defaultProps} onClose={onClose} />)

        await user.click(screen.getByText("Eliminar"))

        expect(toast.success).toHaveBeenCalledWith(
            "Recompensa eliminada exitosamente"
        )
        expect(onClose).toHaveBeenCalled()
    })

    it("shows generic error toast on error callback", async () => {
        mockDeleteReward.mockImplementation((_id: string, options: { onError: (err: unknown) => void }) => {
            options.onError(new Error("Server error"))
        })

        const user = userEvent.setup()

        render(<RewardDeleteDialog {...defaultProps} />)

        await user.click(screen.getByText("Eliminar"))

        expect(toast.error).toHaveBeenCalledWith(
            "Error al eliminar recompensa"
        )
    })

    it("shows specific error toast for ErrorCode 4010 (has backings)", async () => {
        mockDeleteReward.mockImplementation((_id: string, options: { onError: (err: unknown) => void }) => {
            options.onError({ errorCode: "4010" })
        })

        const user = userEvent.setup()

        render(<RewardDeleteDialog {...defaultProps} />)

        await user.click(screen.getByText("Eliminar"))

        expect(toast.error).toHaveBeenCalledWith(
            "Esta recompensa tiene aportes y no puede ser eliminada. Desactivala en su lugar."
        )
    })

    it("returns null when reward is null", () => {
        const { container } = render(
            <RewardDeleteDialog {...defaultProps} reward={null} />
        )

        expect(container.innerHTML).toBe("")
    })
})
