import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { PublishConfirmModal } from "../PublishConfirmModal"

describe("PublishConfirmModal", () => {
    const defaultProps = {
        isOpen: true,
        onClose: vi.fn(),
        onConfirm: vi.fn(),
        hasRewards: true,
        isPublishing: false,
    }

    it("renders with rewards warning when hasRewards is false", () => {
        render(
            <PublishConfirmModal {...defaultProps} hasRewards={false} />
        )

        // Title and button both say "Publicar sin recompensas"
        expect(
            screen.getAllByText("Publicar sin recompensas").length
        ).toBeGreaterThanOrEqual(1)
        expect(
            screen.getByText(/Podras agregar recompensas despues/)
        ).toBeInTheDocument()
    })

    it("renders standard message when hasRewards is true", () => {
        render(
            <PublishConfirmModal {...defaultProps} hasRewards={true} />
        )

        expect(
            screen.getByText("Publicar campania")
        ).toBeInTheDocument()
        expect(
            screen.getByText(
                /Una vez publicada, la campania sera visible publicamente/
            )
        ).toBeInTheDocument()
    })

    it("calls onConfirm when publish button clicked", async () => {
        const onConfirm = vi.fn()
        const user = userEvent.setup()

        render(
            <PublishConfirmModal
                {...defaultProps}
                onConfirm={onConfirm}
                hasRewards={true}
            />
        )

        const publishButton = screen.getByRole("button", {
            name: "Publicar",
        })
        await user.click(publishButton)

        expect(onConfirm).toHaveBeenCalledOnce()
    })

    it("calls onClose when cancel button clicked", async () => {
        const onClose = vi.fn()
        const user = userEvent.setup()

        render(
            <PublishConfirmModal {...defaultProps} onClose={onClose} />
        )

        const cancelButton = screen.getByRole("button", {
            name: "Cancelar",
        })
        await user.click(cancelButton)

        expect(onClose).toHaveBeenCalled()
    })

    it("shows loading state when isPublishing", () => {
        render(
            <PublishConfirmModal {...defaultProps} isPublishing={true} />
        )

        expect(screen.getByText("Publicando...")).toBeInTheDocument()
    })

    it("disables buttons when isPublishing", () => {
        render(
            <PublishConfirmModal {...defaultProps} isPublishing={true} />
        )

        const cancelButton = screen.getByRole("button", {
            name: "Cancelar",
        })
        const publishButton = screen.getByRole("button", {
            name: /Publicando/,
        })

        expect(cancelButton).toBeDisabled()
        expect(publishButton).toBeDisabled()
    })
})
