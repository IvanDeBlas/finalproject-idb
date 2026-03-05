import { describe, it, expect, vi } from "vitest"
import { render, screen, fireEvent } from "@/test-utils"
import { CerrarNecesidadDialog } from "../CerrarNecesidadDialog"

describe("CerrarNecesidadDialog", () => {
    const defaultProps = {
        open: true,
        onOpenChange: vi.fn(),
        onConfirm: vi.fn(),
        isPending: false,
    }

    it("renders when open", () => {
        render(<CerrarNecesidadDialog {...defaultProps} />)

        expect(screen.getByRole("heading", { name: "Cerrar Necesidad" })).toBeInTheDocument()
        expect(
            screen.getByText(/propuestas pendientes seran rechazadas automaticamente/i)
        ).toBeInTheDocument()
    })

    it("does not render content when closed", () => {
        render(<CerrarNecesidadDialog {...defaultProps} open={false} />)

        expect(screen.queryByText("Cerrar Necesidad")).not.toBeInTheDocument()
    })

    it("allows typing motivo", () => {
        render(<CerrarNecesidadDialog {...defaultProps} />)

        const textarea = screen.getByLabelText(/motivo/i)
        fireEvent.change(textarea, { target: { value: "Ya no necesito" } })

        expect(textarea).toHaveValue("Ya no necesito")
    })

    it("displays character counter", () => {
        render(<CerrarNecesidadDialog {...defaultProps} />)

        expect(screen.getByText("0 / 500 caracteres")).toBeInTheDocument()

        const textarea = screen.getByLabelText(/motivo/i)
        fireEvent.change(textarea, { target: { value: "Test" } })

        expect(screen.getByText("4 / 500 caracteres")).toBeInTheDocument()
    })

    it("calls onConfirm with motivo when submitted", () => {
        const handleConfirm = vi.fn()
        render(
            <CerrarNecesidadDialog {...defaultProps} onConfirm={handleConfirm} />
        )

        const textarea = screen.getByLabelText(/motivo/i)
        fireEvent.change(textarea, { target: { value: "Motivo de prueba" } })

        const confirmButton = screen.getByRole("button", { name: /Cerrar Necesidad/i })
        fireEvent.click(confirmButton)

        expect(handleConfirm).toHaveBeenCalledWith("Motivo de prueba")
    })

    it("calls onConfirm without motivo when empty", () => {
        const handleConfirm = vi.fn()
        render(
            <CerrarNecesidadDialog {...defaultProps} onConfirm={handleConfirm} />
        )

        const confirmButton = screen.getByRole("button", { name: /Cerrar Necesidad/i })
        fireEvent.click(confirmButton)

        expect(handleConfirm).toHaveBeenCalledWith(undefined)
    })

    it("calls onOpenChange(false) when cancel button clicked", () => {
        const handleOpenChange = vi.fn()
        render(
            <CerrarNecesidadDialog {...defaultProps} onOpenChange={handleOpenChange} />
        )

        const cancelButton = screen.getByRole("button", { name: /Cancelar/i })
        fireEvent.click(cancelButton)

        expect(handleOpenChange).toHaveBeenCalledWith(false)
    })

    it("disables buttons during pending state", () => {
        render(<CerrarNecesidadDialog {...defaultProps} isPending={true} />)

        expect(screen.getByRole("button", { name: /Cancelar/i })).toBeDisabled()
        expect(screen.getByLabelText(/motivo/i)).toBeDisabled()
    })
})
