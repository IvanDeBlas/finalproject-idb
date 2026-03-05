import { render, screen, waitFor } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { RewardFormModal } from "../RewardFormModal"
import type { Reward } from "@shared/types"

// Mock hooks
const mockCreate = vi.fn()
const mockUpdate = vi.fn()

vi.mock("@/hooks/use-rewards", () => ({
    useCreateReward: () => ({
        mutate: mockCreate,
        isPending: false,
    }),
    useUpdateReward: () => ({
        mutate: mockUpdate,
        isPending: false,
    }),
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

const CAMPANIA_UUID = "a1b2c3d4-e5f6-7890-abcd-ef1234567890"

const baseReward: Reward = {
    id: "b2c3d4e5-f6a7-8901-bcde-f12345678901",
    campaniaId: CAMPANIA_UUID,
    tipoRewardId: 1,
    nombre: "Descarga Digital",
    descripcion: "Acceso anticipado al album",
    importeMinimo: 10,
    monedaId: 1,
    esAddOn: false,
    incluyeEnvioFisico: false,
    orden: 1,
    esActivo: true,
    fechaCreacion: "2026-01-01T00:00:00Z",
}

describe("RewardFormModal", () => {
    const defaultProps = {
        isOpen: true,
        onClose: vi.fn(),
        campaniaId: CAMPANIA_UUID,
        reward: null as Reward | null,
    }

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders form in create mode", () => {
        render(<RewardFormModal {...defaultProps} />)

        expect(screen.getByText("Nueva Recompensa")).toBeInTheDocument()
        expect(
            screen.getByPlaceholderText("Ej: Descarga Digital")
        ).toBeInTheDocument()
    })

    it("renders form in edit mode with pre-filled data", () => {
        render(<RewardFormModal {...defaultProps} reward={baseReward} />)

        expect(screen.getByText("Editar Recompensa")).toBeInTheDocument()
        expect(screen.getByDisplayValue("Descarga Digital")).toBeInTheDocument()
    })

    it("shows character counter for nombre", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        const nombreInput = screen.getByPlaceholderText("Ej: Descarga Digital")
        await user.type(nombreInput, "Test")

        expect(screen.getByText("4/200 caracteres")).toBeInTheDocument()
    })

    it("shows character counter for descripcion", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        const descripcionInput = screen.getByPlaceholderText(
            /Describe que incluye/
        )
        await user.type(descripcionInput, "Hola mundo")

        expect(screen.getByText("10/2000 caracteres")).toBeInTheDocument()
    })

    it("validates nombre is required on submit", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        await user.click(screen.getByText("Guardar"))

        await waitFor(() => {
            expect(
                screen.getByText("El nombre es obligatorio")
            ).toBeInTheDocument()
        })
    })

    it("validates importe minimo must be positive", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        const nombreInput = screen.getByPlaceholderText("Ej: Descarga Digital")
        await user.type(nombreInput, "Test Reward")

        const importeInput = screen.getByPlaceholderText("10.00")
        await user.type(importeInput, "0")

        await user.click(screen.getByText("Guardar"))

        await waitFor(() => {
            expect(
                screen.getByText(/El importe debe ser mayor a 0|El importe minimo es 1 EUR/)
            ).toBeInTheDocument()
        })
    })

    it("shows Cancelar and Guardar buttons", () => {
        render(<RewardFormModal {...defaultProps} />)

        expect(screen.getByText("Cancelar")).toBeInTheDocument()
        expect(screen.getByText("Guardar")).toBeInTheDocument()
    })

    it("calls onClose when Cancelar clicked", async () => {
        const onClose = vi.fn()
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} onClose={onClose} />)

        await user.click(screen.getByText("Cancelar"))

        expect(onClose).toHaveBeenCalled()
    })

    it("renders stock limitado section when checkbox checked", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        expect(
            screen.queryByPlaceholderText("200")
        ).not.toBeInTheDocument()

        // Click the checkbox button directly (Radix checkbox renders as button)
        const stockLabel = screen.getByText("Stock limitado")
        const stockCheckbox = stockLabel.parentElement?.querySelector("button")
        if (stockCheckbox) {
            await user.click(stockCheckbox)
        }

        await waitFor(() => {
            expect(screen.getByPlaceholderText("200")).toBeInTheDocument()
        })
    })

    it("renders envio fisico section when checkbox checked", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        expect(
            screen.queryByPlaceholderText("Marzo 2025")
        ).not.toBeInTheDocument()

        const envioLabel = screen.getByText("Incluye envio fisico")
        const envioCheckbox = envioLabel.parentElement?.querySelector("button")
        if (envioCheckbox) {
            await user.click(envioCheckbox)
        }

        await waitFor(() => {
            expect(
                screen.getByPlaceholderText("Marzo 2025")
            ).toBeInTheDocument()
        })
    })

    it("does not call create mutation when required fields missing", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} />)

        // Only fill nombre, skip tipoRewardId and importe
        const nombreInput = screen.getByPlaceholderText("Ej: Descarga Digital")
        await user.type(nombreInput, "Nueva Reward")

        await user.click(screen.getByText("Guardar"))

        // Form should NOT submit due to missing required fields
        expect(mockCreate).not.toHaveBeenCalled()
    })

    it("calls update mutation in edit mode", async () => {
        const user = userEvent.setup()

        render(<RewardFormModal {...defaultProps} reward={baseReward} />)

        const nombreInput = screen.getByDisplayValue("Descarga Digital")
        await user.clear(nombreInput)
        await user.type(nombreInput, "Descarga Actualizada")

        await user.click(screen.getByText("Guardar"))

        await waitFor(() => {
            expect(mockUpdate).toHaveBeenCalled()
        })

        const callArgs = mockUpdate.mock.calls[0]
        expect(callArgs[0]).toMatchObject({ id: baseReward.id })
        expect(callArgs[0].data.nombre).toBe("Descarga Actualizada")
    })

    it("shows edit mode with pre-filled descripcion", () => {
        render(<RewardFormModal {...defaultProps} reward={baseReward} />)

        expect(
            screen.getByDisplayValue("Acceso anticipado al album")
        ).toBeInTheDocument()
    })
})
