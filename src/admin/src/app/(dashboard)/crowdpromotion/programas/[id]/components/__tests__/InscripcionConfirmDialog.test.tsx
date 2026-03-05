import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { InscripcionConfirmDialog } from "../InscripcionConfirmDialog"

describe("InscripcionConfirmDialog", () => {
    const defaultProps = {
        open: true,
        onOpenChange: vi.fn(),
        onConfirm: vi.fn(),
        isPending: false,
        variante: "rechazar" as const,
        promotorNombre: "DJ Marketing Pro",
    }

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders rechazar dialog title", () => {
        render(<InscripcionConfirmDialog {...defaultProps} />)
        expect(screen.getByText("Rechazar solicitud")).toBeInTheDocument()
    })

    it("shows promotor name in rechazar description", () => {
        render(<InscripcionConfirmDialog {...defaultProps} />)
        expect(screen.getByText(/DJ Marketing Pro/)).toBeInTheDocument()
    })

    it("renders Cancelar and Rechazar buttons", () => {
        render(<InscripcionConfirmDialog {...defaultProps} />)
        expect(screen.getByText("Cancelar")).toBeInTheDocument()
        expect(screen.getByText("Rechazar")).toBeInTheDocument()
    })

    it("does not render when open is false", () => {
        render(<InscripcionConfirmDialog {...defaultProps} open={false} />)
        expect(screen.queryByText("Rechazar solicitud")).not.toBeInTheDocument()
    })

    it("calls onConfirm when confirm button clicked", async () => {
        const user = userEvent.setup()
        render(<InscripcionConfirmDialog {...defaultProps} />)

        await user.click(screen.getByText("Rechazar"))

        expect(defaultProps.onConfirm).toHaveBeenCalled()
    })

    it("disables buttons when isPending", () => {
        render(<InscripcionConfirmDialog {...defaultProps} isPending={true} />)

        expect(screen.getByText("Cancelar")).toBeDisabled()
        expect(screen.getByText("Rechazando...")).toBeDisabled()
    })

    it("renders bloquear dialog with destructive style", () => {
        render(
            <InscripcionConfirmDialog
                {...defaultProps}
                variante="bloquear"
            />
        )
        expect(screen.getByText("Bloquear promotor")).toBeInTheDocument()
        expect(screen.getByText(/No podra volver a solicitar/)).toBeInTheDocument()
    })

    it("renders dar-de-baja dialog", () => {
        render(
            <InscripcionConfirmDialog
                {...defaultProps}
                variante="dar-de-baja"
            />
        )
        expect(screen.getByText("Dar de baja al promotor")).toBeInTheDocument()
        expect(screen.getByText(/codigo referido quedara desactivado/)).toBeInTheDocument()
    })

    it("shows loading text for bloquear variant", () => {
        render(
            <InscripcionConfirmDialog
                {...defaultProps}
                variante="bloquear"
                isPending={true}
            />
        )
        expect(screen.getByText("Bloqueando...")).toBeInTheDocument()
    })

    it("shows loading text for dar-de-baja variant", () => {
        render(
            <InscripcionConfirmDialog
                {...defaultProps}
                variante="dar-de-baja"
                isPending={true}
            />
        )
        expect(screen.getByText("Dando de baja...")).toBeInTheDocument()
    })
})
