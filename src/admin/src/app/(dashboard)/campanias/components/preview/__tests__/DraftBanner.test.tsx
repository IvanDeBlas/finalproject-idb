import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { DraftBanner } from "../DraftBanner"

describe("DraftBanner", () => {
    it("renders banner message", () => {
        render(<DraftBanner />)

        expect(
            screen.getByText(
                "VISTA PREVIA - Campania en borrador (No visible publicamente)"
            )
        ).toBeInTheDocument()
    })

    it("hides when close button clicked", async () => {
        const user = userEvent.setup()

        render(<DraftBanner />)

        expect(screen.getByRole("alert")).toBeInTheDocument()

        const closeButton = screen.getByRole("button")
        await user.click(closeButton)

        expect(screen.queryByRole("alert")).not.toBeInTheDocument()
    })

    it("calls onClose callback when closed", async () => {
        const onClose = vi.fn()
        const user = userEvent.setup()

        render(<DraftBanner onClose={onClose} />)

        const closeButton = screen.getByRole("button")
        await user.click(closeButton)

        expect(onClose).toHaveBeenCalledOnce()
    })

    it("renders without onClose prop", async () => {
        const user = userEvent.setup()

        render(<DraftBanner />)

        expect(screen.getByRole("alert")).toBeInTheDocument()

        // Should still hide without error when close is clicked
        const closeButton = screen.getByRole("button")
        await user.click(closeButton)

        expect(screen.queryByRole("alert")).not.toBeInTheDocument()
    })
})
