import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { describe, it, expect, vi } from "vitest"
import { PromotorForm } from "../../presentation/components/PromotorForm"

const defaultCreateProps = {
    mode: "create" as const,
    onSubmit: vi.fn().mockResolvedValue(undefined),
    isSubmitting: false,
    onCancel: vi.fn(),
}

const defaultEditProps = {
    mode: "edit" as const,
    defaultValues: {
        nombrePublico: "DJ Marketing Pro",
        emailContacto: "contacto@djmarketing.com",
        urlSitioWeb: "https://djmarketing.com",
        urlInstagram: "https://instagram.com/djmarketing",
        urlTikTok: "",
        urlYouTube: "",
        urlTwitter: "",
    },
    tipoPromotorNombre: "Influencer",
    onSubmit: vi.fn().mockResolvedValue(undefined),
    isSubmitting: false,
    onCancel: vi.fn(),
}

describe("PromotorForm", () => {
    describe("Create mode - Render", () => {
        it("renders nombre publico field", () => {
            render(<PromotorForm {...defaultCreateProps} />)
            expect(screen.getByLabelText(/nombre publico/i)).toBeInTheDocument()
        })

        it("renders tipo promotor select in create mode", () => {
            render(<PromotorForm {...defaultCreateProps} />)
            expect(screen.getByText(/tipo de promotor/i)).toBeInTheDocument()
            expect(screen.getByText("Selecciona un tipo")).toBeInTheDocument()
        })

        it("renders email contacto field", () => {
            render(<PromotorForm {...defaultCreateProps} />)
            expect(screen.getByLabelText(/email de contacto/i)).toBeInTheDocument()
        })

        it("renders social media fields", () => {
            render(<PromotorForm {...defaultCreateProps} />)
            expect(screen.getByText("Redes sociales")).toBeInTheDocument()
            expect(screen.getByPlaceholderText(/instagram.com/i)).toBeInTheDocument()
            expect(screen.getByPlaceholderText(/tiktok.com/i)).toBeInTheDocument()
            expect(screen.getByPlaceholderText(/youtube.com/i)).toBeInTheDocument()
            expect(screen.getByPlaceholderText(/x.com/i)).toBeInTheDocument()
        })

        it("shows 'Crear perfil' submit text in create mode", () => {
            render(<PromotorForm {...defaultCreateProps} />)
            expect(
                screen.getByRole("button", { name: /crear perfil/i })
            ).toBeInTheDocument()
        })
    })

    describe("Edit mode - Render", () => {
        it("renders tipo promotor as read-only text in edit mode", () => {
            render(<PromotorForm {...defaultEditProps} />)
            expect(screen.getByDisplayValue("Influencer")).toBeInTheDocument()
            expect(
                screen.queryByText("Selecciona un tipo")
            ).not.toBeInTheDocument()
        })

        it("pre-fills form with defaultValues in edit mode", () => {
            render(<PromotorForm {...defaultEditProps} />)
            expect(
                screen.getByDisplayValue("DJ Marketing Pro")
            ).toBeInTheDocument()
            expect(
                screen.getByDisplayValue("contacto@djmarketing.com")
            ).toBeInTheDocument()
        })

        it("shows 'Guardar cambios' submit text in edit mode", () => {
            render(<PromotorForm {...defaultEditProps} />)
            expect(
                screen.getByRole("button", { name: /guardar cambios/i })
            ).toBeInTheDocument()
        })

        it("shows note that tipo promotor cannot be changed", () => {
            render(<PromotorForm {...defaultEditProps} />)
            expect(
                screen.getByText(/no se puede cambiar/i)
            ).toBeInTheDocument()
        })
    })

    describe("Validation", () => {
        it("shows required error when nombrePublico is empty on submit", async () => {
            const user = userEvent.setup()
            render(<PromotorForm {...defaultCreateProps} />)

            await user.click(
                screen.getByRole("button", { name: /crear perfil/i })
            )

            expect(
                await screen.findByText(/nombre publico es obligatorio/i)
            ).toBeInTheDocument()
        })

        it("shows min-length error when nombrePublico has 2 chars", async () => {
            const user = userEvent.setup()
            render(<PromotorForm {...defaultCreateProps} />)

            await user.type(screen.getByLabelText(/nombre publico/i), "AB")
            await user.click(
                screen.getByRole("button", { name: /crear perfil/i })
            )

            expect(
                await screen.findByText(/al menos 3 caracteres/i)
            ).toBeInTheDocument()
        })

        it("renders email field with email type attribute", () => {
            render(<PromotorForm {...defaultEditProps} />)
            const emailInput = screen.getByLabelText(/email de contacto/i)
            expect(emailInput).toHaveAttribute("type", "email")
        })
    })

    describe("Submit button states", () => {
        it("shows loading state when isSubmitting is true", () => {
            render(
                <PromotorForm {...defaultCreateProps} isSubmitting={true} />
            )
            expect(screen.getByText("Guardando...")).toBeInTheDocument()
        })

        it("disables submit button when isSubmitting is true", () => {
            render(
                <PromotorForm {...defaultCreateProps} isSubmitting={true} />
            )
            expect(
                screen.getByRole("button", { name: /guardando/i })
            ).toBeDisabled()
        })

        it("disables input fields when isSubmitting is true", () => {
            render(
                <PromotorForm {...defaultCreateProps} isSubmitting={true} />
            )
            expect(screen.getByLabelText(/nombre publico/i)).toBeDisabled()
        })
    })

    describe("Cancel button", () => {
        it("calls onCancel when cancel button is clicked", async () => {
            const user = userEvent.setup()
            const onCancel = vi.fn()
            render(
                <PromotorForm {...defaultCreateProps} onCancel={onCancel} />
            )

            await user.click(
                screen.getByRole("button", { name: /cancelar/i })
            )
            expect(onCancel).toHaveBeenCalledTimes(1)
        })
    })
})
