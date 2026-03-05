import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { SolicitarCobroDialog } from "../../wallet/presentation/components/SolicitarCobroDialog"
import { mockSolicitarCobroResponse } from "../../__mocks__/wallet.mock"

vi.mock("../../wallet/application/hooks/useSolicitarCobro", () => ({
    useSolicitarCobro: vi.fn(),
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

import { useSolicitarCobro } from "../../wallet/application/hooks/useSolicitarCobro"
const mockUseSolicitarCobro = vi.mocked(useSolicitarCobro)

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: { queries: { retry: false, gcTime: 0 } },
    })
    return ({ children }: { children: React.ReactNode }) => (
        <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    )
}

describe("SolicitarCobroDialog", () => {
    const defaultProps = {
        open: true,
        onOpenChange: vi.fn(),
        saldoDisponible: 150.50,
        minimoRetiro: 10.00,
        monedaNombre: "EUR",
        onSuccess: vi.fn(),
    }

    beforeEach(() => {
        vi.clearAllMocks()
        mockUseSolicitarCobro.mockReturnValue({
            mutateAsync: vi.fn().mockResolvedValue(mockSolicitarCobroResponse),
            isPending: false,
            isError: false,
            error: null,
        } as ReturnType<typeof useSolicitarCobro>)
    })

    it("renders dialog with title and form fields", () => {
        const Wrapper = createWrapper()
        render(<Wrapper><SolicitarCobroDialog {...defaultProps} /></Wrapper>)

        expect(screen.getByText("Solicitar cobro")).toBeDefined()
        expect(screen.getByLabelText(/importe/i)).toBeDefined()
        expect(screen.getByLabelText(/descripcion/i)).toBeDefined()
    })

    it("shows saldo disponible and minimo retiro", () => {
        const Wrapper = createWrapper()
        render(<Wrapper><SolicitarCobroDialog {...defaultProps} /></Wrapper>)

        expect(screen.getByText(/150,50/)).toBeDefined()
        expect(screen.getByText(/10,00/)).toBeDefined()
    })

    it("shows cancel and confirm buttons", () => {
        const Wrapper = createWrapper()
        render(<Wrapper><SolicitarCobroDialog {...defaultProps} /></Wrapper>)

        expect(screen.getByRole("button", { name: /cancelar/i })).toBeDefined()
        expect(screen.getByRole("button", { name: /confirmar cobro/i })).toBeDefined()
    })

    it("shows validation error for empty importe", async () => {
        const user = userEvent.setup()
        const Wrapper = createWrapper()
        render(<Wrapper><SolicitarCobroDialog {...defaultProps} /></Wrapper>)

        await user.click(screen.getByRole("button", { name: /confirmar cobro/i }))

        await waitFor(() => {
            const errorMessages = screen.getAllByRole("alert")
            expect(errorMessages.length).toBeGreaterThan(0)
        })
    })

    it("does not render when open is false", () => {
        const Wrapper = createWrapper()
        render(<Wrapper><SolicitarCobroDialog {...defaultProps} open={false} /></Wrapper>)

        expect(screen.queryByText("Solicitar cobro")).toBeNull()
    })

    it("shows character counter for descripcion", () => {
        const Wrapper = createWrapper()
        render(<Wrapper><SolicitarCobroDialog {...defaultProps} /></Wrapper>)

        expect(screen.getByText("0/500")).toBeDefined()
    })
})
