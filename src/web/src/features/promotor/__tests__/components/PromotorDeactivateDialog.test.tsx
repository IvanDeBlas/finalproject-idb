import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { describe, it, expect, vi } from "vitest"
import { PromotorDeactivateDialog } from "../../presentation/components/PromotorDeactivateDialog"

const defaultProps = {
    open: true,
    onOpenChange: vi.fn(),
    onConfirm: vi.fn().mockResolvedValue(undefined),
    isConfirming: false,
    totalProgramasActivos: 3,
}

describe("PromotorDeactivateDialog", () => {
    it("renders nothing when open is false", () => {
        render(
            <PromotorDeactivateDialog {...defaultProps} open={false} />
        )
        expect(
            screen.queryByText("Desactivar cuenta de promotor")
        ).not.toBeInTheDocument()
    })

    it("renders dialog when open is true", () => {
        render(<PromotorDeactivateDialog {...defaultProps} />)
        expect(
            screen.getByText("Desactivar cuenta de promotor")
        ).toBeInTheDocument()
    })

    it("shows program count when totalProgramasActivos > 0", () => {
        render(<PromotorDeactivateDialog {...defaultProps} />)
        expect(screen.getByText("3")).toBeInTheDocument()
        expect(
            screen.getByText(/programas activos/)
        ).toBeInTheDocument()
    })

    it("shows simple confirmation when totalProgramasActivos is 0", () => {
        render(
            <PromotorDeactivateDialog
                {...defaultProps}
                totalProgramasActivos={0}
            />
        )
        expect(
            screen.getByText(/tu perfil dejara de estar visible/)
        ).toBeInTheDocument()
    })

    it("calls onConfirm when confirm button is clicked", async () => {
        const user = userEvent.setup()
        const onConfirm = vi.fn().mockResolvedValue(undefined)
        render(
            <PromotorDeactivateDialog
                {...defaultProps}
                onConfirm={onConfirm}
            />
        )

        await user.click(screen.getByRole("button", { name: /desactivar cuenta/i }))
        expect(onConfirm).toHaveBeenCalledTimes(1)
    })

    it("calls onOpenChange(false) when cancel button is clicked", async () => {
        const user = userEvent.setup()
        const onOpenChange = vi.fn()
        render(
            <PromotorDeactivateDialog
                {...defaultProps}
                onOpenChange={onOpenChange}
            />
        )

        await user.click(screen.getByRole("button", { name: /cancelar/i }))
        expect(onOpenChange).toHaveBeenCalledWith(false)
    })

    it("disables cancel and confirm buttons when isConfirming is true", () => {
        render(
            <PromotorDeactivateDialog
                {...defaultProps}
                isConfirming={true}
            />
        )

        expect(
            screen.getByRole("button", { name: /cancelar/i })
        ).toBeDisabled()
        expect(
            screen.getByRole("button", { name: /desactivando/i })
        ).toBeDisabled()
    })

    it("shows loading text in confirm button when isConfirming", () => {
        render(
            <PromotorDeactivateDialog
                {...defaultProps}
                isConfirming={true}
            />
        )
        expect(screen.getByText("Desactivando...")).toBeInTheDocument()
    })

    it("uses singular when totalProgramasActivos is 1", () => {
        render(
            <PromotorDeactivateDialog
                {...defaultProps}
                totalProgramasActivos={1}
            />
        )
        expect(
            screen.getByText(/programa activo/)
        ).toBeInTheDocument()
    })
})
