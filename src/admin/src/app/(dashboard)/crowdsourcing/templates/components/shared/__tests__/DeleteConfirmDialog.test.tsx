import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { DeleteConfirmDialog } from "../DeleteConfirmDialog"

describe("DeleteConfirmDialog", () => {
    const defaultProps = {
        open: true,
        onOpenChange: vi.fn(),
        onConfirm: vi.fn(),
        title: "Confirmar accion",
        description: "Esta accion no se puede deshacer.",
    }

    it("renders dialog with title and description", () => {
        render(<DeleteConfirmDialog {...defaultProps} />)

        expect(screen.getByText("Confirmar accion")).toBeInTheDocument()
        expect(screen.getByText("Esta accion no se puede deshacer.")).toBeInTheDocument()
    })

    it("calls onConfirm when confirm button is clicked", async () => {
        const user = userEvent.setup()
        render(<DeleteConfirmDialog {...defaultProps} />)

        await user.click(screen.getByText("Confirmar"))

        expect(defaultProps.onConfirm).toHaveBeenCalledOnce()
    })

    it("shows loading state when isPending", () => {
        render(<DeleteConfirmDialog {...defaultProps} isPending={true} />)

        const confirmButton = screen.getByText("Confirmar")
        expect(confirmButton.closest("button")).toBeDisabled()
    })

    it("does not render when open is false", () => {
        render(<DeleteConfirmDialog {...defaultProps} open={false} />)

        expect(screen.queryByText("Confirmar accion")).not.toBeInTheDocument()
    })
})
