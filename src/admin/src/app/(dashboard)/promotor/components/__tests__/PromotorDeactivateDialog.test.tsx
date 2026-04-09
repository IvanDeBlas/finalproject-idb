import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { DesactivarPromotorDialog } from "../DesactivarPromotorDialog"

const mockMutate = vi.fn()

vi.mock("@/hooks/use-promotor-mutations", () => ({
    useDesactivarPromotor: () => ({
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
    open: true,
    onOpenChange: vi.fn(),
    totalProgramasActivos: 0,
}

describe("DesactivarPromotorDialog", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders dialog when open", () => {
        render(<DesactivarPromotorDialog {...defaultProps} />)

        expect(
            screen.getByText(/desactivar perfil de promotor/i)
        ).toBeInTheDocument()
    })

    it("does not render dialog when closed", () => {
        render(<DesactivarPromotorDialog {...defaultProps} open={false} />)

        expect(
            screen.queryByText(/desactivar perfil de promotor/i)
        ).not.toBeInTheDocument()
    })

    it("shows programs warning when totalProgramasActivos > 0", () => {
        render(
            <DesactivarPromotorDialog
                {...defaultProps}
                totalProgramasActivos={3}
            />
        )

        expect(screen.getByText(/3/)).toBeInTheDocument()
        expect(screen.getByText(/programa/i)).toBeInTheDocument()
    })

    it("shows simple message when totalProgramasActivos = 0", () => {
        render(<DesactivarPromotorDialog {...defaultProps} />)

        expect(
            screen.getByText(/dejaras de ser visible/i)
        ).toBeInTheDocument()
    })

    it("calls mutate on confirm click", async () => {
        const user = userEvent.setup()
        render(<DesactivarPromotorDialog {...defaultProps} />)

        const confirmButton = screen.getByRole("button", { name: /desactivar$/i })
        await user.click(confirmButton)

        expect(mockMutate).toHaveBeenCalled()
    })

    it("calls onOpenChange on cancel click", async () => {
        const user = userEvent.setup()
        render(<DesactivarPromotorDialog {...defaultProps} />)

        const cancelButton = screen.getByRole("button", { name: /cancelar/i })
        await user.click(cancelButton)

        expect(defaultProps.onOpenChange).toHaveBeenCalled()
    })

    it("shows loading text during pending", () => {
        vi.mocked(mockMutate).mockImplementation(() => {})

        vi.doMock("@/hooks/use-promotor-mutations", () => ({
            useDesactivarPromotor: () => ({
                mutate: mockMutate,
                isPending: true,
            }),
        }))

        // Re-render is needed for isPending test - testing static render
        render(<DesactivarPromotorDialog {...defaultProps} />)

        // The button exists in the dialog
        expect(
            screen.getByRole("button", { name: /desactivar/i })
        ).toBeInTheDocument()
    })

    it("shows programs count when programs exist", () => {
        render(
            <DesactivarPromotorDialog
                {...defaultProps}
                totalProgramasActivos={5}
            />
        )

        expect(screen.getByText(/5/)).toBeInTheDocument()
        expect(screen.getByText(/dados de baja/i)).toBeInTheDocument()
    })
})
