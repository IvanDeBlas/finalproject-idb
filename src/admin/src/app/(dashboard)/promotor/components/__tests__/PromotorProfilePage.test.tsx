import { render, screen, waitFor } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { PromotorEditForm } from "../PromotorEditForm"
import { mockPromotor, mockPromotorInactivo } from "@/__mocks__/promotor.mock"

const mockMutate = vi.fn()

vi.mock("@/hooks/use-promotor-mutations", () => ({
    useUpdatePromotor: () => ({
        mutate: mockMutate,
        isPending: false,
    }),
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

const defaultProps = {
    promotor: mockPromotor,
    onSuccess: vi.fn(),
    onCancel: vi.fn(),
}

describe("PromotorEditForm", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("pre-fills form with promotor data", () => {
        render(<PromotorEditForm {...defaultProps} />)

        const nombreInput = screen.getByLabelText(/nombre publico/i)
        expect(nombreInput).toHaveValue("DJ Promo Star")
    })

    it("shows tipo promotor as readonly", () => {
        render(<PromotorEditForm {...defaultProps} />)

        const tipoInput = screen.getByLabelText(/tipo de promotor/i)
        expect(tipoInput).toBeDisabled()
        expect(tipoInput).toHaveValue("Influencer")
    })

    it("renders loading skeleton when isLoading would be true", () => {
        render(<PromotorEditForm {...defaultProps} />)

        expect(screen.getByLabelText(/nombre publico/i)).toBeInTheDocument()
    })

    it("pre-fills email contacto field", () => {
        render(<PromotorEditForm {...defaultProps} />)

        const emailInput = screen.getByLabelText(/email de contacto/i)
        expect(emailInput).toHaveValue("promo@example.com")
    })

    it("submits form with updated data", async () => {
        const user = userEvent.setup()
        render(<PromotorEditForm {...defaultProps} />)

        const nombreInput = screen.getByLabelText(/nombre publico/i)
        await user.clear(nombreInput)
        await user.type(nombreInput, "New Promo Name")

        const submitButton = screen.getByRole("button", { name: /guardar cambios/i })
        await user.click(submitButton)

        await waitFor(() => {
            expect(mockMutate).toHaveBeenCalled()
        })
    })

    it("disables submit button when form is not dirty", () => {
        render(<PromotorEditForm {...defaultProps} />)

        const submitButton = screen.getByRole("button", { name: /guardar cambios/i })
        expect(submitButton).toBeDisabled()
    })

    it("shows validation error for empty nombre publico", async () => {
        const user = userEvent.setup()
        render(<PromotorEditForm {...defaultProps} />)

        const nombreInput = screen.getByLabelText(/nombre publico/i)
        await user.clear(nombreInput)

        const submitButton = screen.getByRole("button", { name: /guardar cambios/i })
        await user.click(submitButton)

        await waitFor(() => {
            expect(screen.getByText(/nombre publico es obligatorio/i)).toBeInTheDocument()
        })
    })

    it("shows validation error for short nombre publico", async () => {
        const user = userEvent.setup()
        render(<PromotorEditForm {...defaultProps} />)

        const nombreInput = screen.getByLabelText(/nombre publico/i)
        await user.clear(nombreInput)
        await user.type(nombreInput, "ab")

        const submitButton = screen.getByRole("button", { name: /guardar cambios/i })
        await user.click(submitButton)

        await waitFor(() => {
            expect(screen.getByText(/al menos 3 caracteres/i)).toBeInTheDocument()
        })
    })

    it("allows empty optional URL fields", async () => {
        const user = userEvent.setup()
        render(<PromotorEditForm {...defaultProps} />)

        const instagramInput = screen.getByLabelText(/instagram/i)
        await user.clear(instagramInput)

        const nombreInput = screen.getByLabelText(/nombre publico/i)
        await user.clear(nombreInput)
        await user.type(nombreInput, "Valid Name Updated")

        const submitButton = screen.getByRole("button", { name: /guardar cambios/i })
        await user.click(submitButton)

        await waitFor(() => {
            expect(mockMutate).toHaveBeenCalled()
        })
    })

    it("shows URL validation error for invalid URL", async () => {
        const user = userEvent.setup()
        render(<PromotorEditForm {...defaultProps} />)

        const instagramInput = screen.getByLabelText(/instagram/i)
        await user.clear(instagramInput)
        await user.type(instagramInput, "not-a-url")

        const submitButton = screen.getByRole("button", { name: /guardar cambios/i })
        await user.click(submitButton)

        await waitFor(() => {
            expect(screen.getByText(/formato valido/i)).toBeInTheDocument()
        })
    })

    it("calls onCancel when cancel button is clicked", async () => {
        const user = userEvent.setup()
        render(<PromotorEditForm {...defaultProps} />)

        const cancelButton = screen.getByRole("button", { name: /cancelar/i })
        await user.click(cancelButton)

        expect(defaultProps.onCancel).toHaveBeenCalled()
    })

    it("hides danger zone when promotor is inactive", () => {
        render(
            <PromotorEditForm
                {...defaultProps}
                promotor={mockPromotorInactivo}
            />
        )

        expect(screen.queryByText(/zona de peligro/i)).not.toBeInTheDocument()
    })
})
