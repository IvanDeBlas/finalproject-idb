import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { CompletarTareaDialog } from "../../tareas/presentation/components/CompletarTareaDialog"
import { mockTarea_Pendiente } from "../../__mocks__/tareas.mock"

const defaultProps = {
    open: true,
    onOpenChange: vi.fn(),
    tarea: mockTarea_Pendiente,
    programaId: "prog-001",
    esReenvio: false,
    onSubmit: vi.fn().mockResolvedValue(undefined),
    isSubmitting: false,
}

describe("CompletarTareaDialog", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("does not render when open is false", () => {
        render(<CompletarTareaDialog {...defaultProps} open={false} />)

        expect(screen.queryByRole("dialog")).not.toBeInTheDocument()
    })

    it("renders dialog when open is true", () => {
        render(<CompletarTareaDialog {...defaultProps} />)

        expect(screen.getByRole("dialog")).toBeInTheDocument()
    })

    it("shows the task name in the title", () => {
        render(<CompletarTareaDialog {...defaultProps} />)

        expect(
            screen.getByText(/Completar:.*Comparte en Instagram Stories/)
        ).toBeInTheDocument()
    })

    it("shows instructions URL link when available", () => {
        render(<CompletarTareaDialog {...defaultProps} />)

        expect(
            screen.getByText("Ver instrucciones completas")
        ).toBeInTheDocument()
    })

    it("shows URL required error on empty submit", async () => {
        render(<CompletarTareaDialog {...defaultProps} />)

        const user = userEvent.setup()
        await user.click(
            screen.getByRole("button", { name: /Enviar prueba/i })
        )

        await waitFor(() => {
            expect(screen.getByRole("alert")).toBeInTheDocument()
        })
    })

    it("does not submit with invalid URL format", async () => {
        const handleSubmit = vi.fn()
        render(
            <CompletarTareaDialog
                {...defaultProps}
                onSubmit={handleSubmit}
            />
        )

        const user = userEvent.setup()
        const urlInput = screen.getByPlaceholderText(/https:\/\//)
        await user.type(urlInput, "no-es-una-url")
        await user.click(
            screen.getByRole("button", { name: /Enviar prueba/i })
        )

        // Wait a tick to allow any async validation
        await waitFor(() => {
            expect(handleSubmit).not.toHaveBeenCalled()
        })
    })

    it("accepts valid URL without error", async () => {
        render(<CompletarTareaDialog {...defaultProps} />)

        const user = userEvent.setup()
        const urlInput = screen.getByPlaceholderText(/https:\/\//)
        await user.type(urlInput, "https://instagram.com/stories/test")
        await user.click(
            screen.getByRole("button", { name: /Enviar prueba/i })
        )

        await waitFor(() => {
            expect(defaultProps.onSubmit).toHaveBeenCalled()
        })
    })

    it("comment field is optional", async () => {
        render(<CompletarTareaDialog {...defaultProps} />)

        const user = userEvent.setup()
        const urlInput = screen.getByPlaceholderText(/https:\/\//)
        await user.type(urlInput, "https://instagram.com/stories/test")
        await user.click(
            screen.getByRole("button", { name: /Enviar prueba/i })
        )

        await waitFor(() => {
            expect(defaultProps.onSubmit).toHaveBeenCalledWith(
                expect.objectContaining({
                    urlPruebaCompletado: "https://instagram.com/stories/test",
                })
            )
        })
    })

    it("shows character counter starting at 0/500", () => {
        render(<CompletarTareaDialog {...defaultProps} />)

        expect(screen.getByText(/0 \/ 500 caracteres/)).toBeInTheDocument()
    })

    it("updates character counter when typing in comment", async () => {
        render(<CompletarTareaDialog {...defaultProps} />)

        const user = userEvent.setup()
        const textarea = screen.getByPlaceholderText(
            /Describe brevemente/
        )
        await user.type(textarea, "Mi comentario")

        expect(screen.getByText(/13 \/ 500 caracteres/)).toBeInTheDocument()
    })

    it("calls onSubmit with form data on valid submit", async () => {
        render(<CompletarTareaDialog {...defaultProps} />)

        const user = userEvent.setup()
        const urlInput = screen.getByPlaceholderText(/https:\/\//)
        const textarea = screen.getByPlaceholderText(
            /Describe brevemente/
        )

        await user.type(urlInput, "https://instagram.com/p/test")
        await user.type(textarea, "Test comment")
        await user.click(
            screen.getByRole("button", { name: /Enviar prueba/i })
        )

        await waitFor(() => {
            expect(defaultProps.onSubmit).toHaveBeenCalledWith(
                expect.objectContaining({
                    urlPruebaCompletado: "https://instagram.com/p/test",
                    comentarioPromotor: "Test comment",
                })
            )
        })
    })

    it("shows loading state when isSubmitting", () => {
        render(
            <CompletarTareaDialog {...defaultProps} isSubmitting={true} />
        )

        expect(screen.getByText("Enviando...")).toBeInTheDocument()
        expect(
            screen.getByRole("button", { name: /Cancelar/i })
        ).toBeDisabled()
    })

    it("calls onOpenChange(false) on Cancel click", async () => {
        const handleOpenChange = vi.fn()
        render(
            <CompletarTareaDialog
                {...defaultProps}
                onOpenChange={handleOpenChange}
            />
        )

        const user = userEvent.setup()
        await user.click(
            screen.getByRole("button", { name: /Cancelar/i })
        )

        expect(handleOpenChange).toHaveBeenCalledWith(false)
    })

    it("shows re-send note when esReenvio is true", () => {
        render(
            <CompletarTareaDialog {...defaultProps} esReenvio={true} />
        )

        expect(
            screen.getByText(/Re-envio por rechazo previo/)
        ).toBeInTheDocument()
    })
})
