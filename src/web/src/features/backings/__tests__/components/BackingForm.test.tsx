import { describe, it, expect, vi } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { BackingForm } from "../../presentation/components/BackingForm"

const mockReward = {
    id: "123e4567-e89b-12d3-a456-426614174000",
    nombre: "CD Fisico Firmado",
    descripcion: "CD fisico con firma del artista",
    importeMinimo: 25,
}

describe("BackingForm", () => {
    const defaultProps = {
        campaniaId: "550e8400-e29b-41d4-a716-446655440000",
        reward: mockReward,
        onSubmit: vi.fn(),
    }

    it("renders form with reward info", () => {
        render(<BackingForm {...defaultProps} />)

        // Reward name appears in both the selected reward card and the summary
        expect(screen.getAllByText("CD Fisico Firmado").length).toBeGreaterThan(0)
    })

    it("pre-fills amount with reward minimum", () => {
        render(<BackingForm {...defaultProps} />)

        const input = screen.getByRole("spinbutton")
        expect(input).toHaveValue(25)
    })

    it("defaults to minAmount=5 when no reward", () => {
        render(<BackingForm {...defaultProps} reward={null} />)

        const input = screen.getByRole("spinbutton")
        expect(input).toHaveValue(5)
    })

    it("shows 'Aporte libre' when no reward selected", () => {
        render(<BackingForm {...defaultProps} reward={null} />)

        expect(screen.getByText("Aporte libre")).toBeInTheDocument()
    })

    it("shows quick amount buttons (+5, +10, +25)", () => {
        render(<BackingForm {...defaultProps} />)

        expect(screen.getByText("+€5")).toBeInTheDocument()
        expect(screen.getByText("+€10")).toBeInTheDocument()
        expect(screen.getByText("+€25")).toBeInTheDocument()
    })

    it("increments amount when quick button is clicked", async () => {
        const user = userEvent.setup()
        render(<BackingForm {...defaultProps} />)

        const addButton = screen.getByText("+€10")
        await user.click(addButton)

        const input = screen.getByRole("spinbutton")
        expect(input).toHaveValue(35) // 25 + 10
    })

    it("shows message textarea with character counter", () => {
        render(<BackingForm {...defaultProps} />)

        expect(screen.getByPlaceholderText("Escribe un mensaje de apoyo...")).toBeInTheDocument()
        expect(screen.getByText("0/500")).toBeInTheDocument()
    })

    it("updates character counter as user types", async () => {
        const user = userEvent.setup()
        render(<BackingForm {...defaultProps} />)

        const textarea = screen.getByPlaceholderText("Escribe un mensaje de apoyo...")
        await user.type(textarea, "Hola")

        expect(screen.getByText("4/500")).toBeInTheDocument()
    })

    it("shows anonymous checkbox", () => {
        render(<BackingForm {...defaultProps} />)

        expect(screen.getByText(/anonimo mi apoyo/i)).toBeInTheDocument()
    })

    it("shows summary card with total", () => {
        render(<BackingForm {...defaultProps} />)

        expect(screen.getByText("Resumen")).toBeInTheDocument()
        expect(screen.getByText("Total:")).toBeInTheDocument()
    })

    it("shows 'Cambiar' button when onChangeReward is provided", () => {
        render(<BackingForm {...defaultProps} onChangeReward={vi.fn()} />)

        expect(screen.getByText("Cambiar")).toBeInTheDocument()
    })

    it("hides 'Cambiar' button when onChangeReward is not provided", () => {
        render(<BackingForm {...defaultProps} />)

        expect(screen.queryByText("Cambiar")).not.toBeInTheDocument()
    })

    it("shows cancel button when onCancel is provided", () => {
        render(<BackingForm {...defaultProps} onCancel={vi.fn()} />)

        expect(screen.getByText("Cancelar")).toBeInTheDocument()
    })

    it("calls onCancel when cancel button is clicked", async () => {
        const user = userEvent.setup()
        const handleCancel = vi.fn()
        render(<BackingForm {...defaultProps} onCancel={handleCancel} />)

        await user.click(screen.getByText("Cancelar"))

        expect(handleCancel).toHaveBeenCalled()
    })

    it("shows submit button with 'Confirmar Apoyo'", () => {
        render(<BackingForm {...defaultProps} />)

        expect(screen.getByRole("button", { name: "Confirmar Apoyo" })).toBeInTheDocument()
    })

    it("shows loading state when isSubmitting", () => {
        render(<BackingForm {...defaultProps} isSubmitting={true} />)

        expect(screen.getByText("Procesando...")).toBeInTheDocument()
    })

    it("disables submit button when isSubmitting", () => {
        render(<BackingForm {...defaultProps} isSubmitting={true} />)

        const submitBtn = screen.getByRole("button", { name: /Procesando/i })
        expect(submitBtn).toBeDisabled()
    })

    it("shows reward amount error when monto < importeMinimo", async () => {
        const user = userEvent.setup()
        render(<BackingForm {...defaultProps} />)

        const input = screen.getByRole("spinbutton")
        await user.clear(input)
        await user.type(input, "10")

        await waitFor(() => {
            expect(screen.getByText(/El monto debe ser al menos 25/)).toBeInTheDocument()
        })
    })

    it("calls onSubmit with form data on valid submit", async () => {
        const user = userEvent.setup()
        const handleSubmit = vi.fn()
        render(<BackingForm {...defaultProps} onSubmit={handleSubmit} />)

        const submitBtn = screen.getByRole("button", { name: "Confirmar Apoyo" })
        await user.click(submitBtn)

        await waitFor(() => {
            expect(handleSubmit).toHaveBeenCalledWith(
                expect.objectContaining({
                    campaniaId: "550e8400-e29b-41d4-a716-446655440000",
                    monto: 25,
                    esAnonimo: false,
                })
            )
        })
    })
})
