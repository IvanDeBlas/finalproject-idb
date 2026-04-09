import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { WalletSaldoCard } from "../../wallet/presentation/components/WalletSaldoCard"
import { mockWallet, mockWallet_SaldoBajo } from "../../__mocks__/wallet.mock"

describe("WalletSaldoCard", () => {
    const defaultProps = {
        wallet: mockWallet,
        isLoading: false,
        onSolicitarCobro: vi.fn(),
    }

    it("renders wallet data correctly", () => {
        render(<WalletSaldoCard {...defaultProps} />)

        expect(screen.getByText(/150,50/)).toBeDefined()
        expect(screen.getByText(/500,00/)).toBeDefined()
        expect(screen.getByText(/324,50/)).toBeDefined()
        expect(screen.getByText("EUR")).toBeDefined()
    })

    it("renders skeleton when loading", () => {
        render(<WalletSaldoCard wallet={undefined} isLoading={true} onSolicitarCobro={vi.fn()} />)

        expect(screen.queryByText(/150,50/)).toBeNull()
        expect(screen.queryByText("Solicitar cobro")).toBeNull()
    })

    it("enables button when saldo >= minimoRetiro", () => {
        render(<WalletSaldoCard {...defaultProps} />)

        const button = screen.getByRole("button", { name: /solicitar cobro/i })
        expect(button).not.toBeDisabled()
    })

    it("disables button when saldo < minimoRetiro", () => {
        render(<WalletSaldoCard {...defaultProps} wallet={mockWallet_SaldoBajo} />)

        const button = screen.getByRole("button", { name: /solicitar cobro/i })
        expect(button).toBeDisabled()
    })

    it("shows minimum retiro warning when saldo is below minimum", () => {
        render(<WalletSaldoCard {...defaultProps} wallet={mockWallet_SaldoBajo} />)

        expect(screen.getByText(/necesitas al menos/i)).toBeDefined()
    })

    it("calls onSolicitarCobro when button is clicked", async () => {
        const user = userEvent.setup()
        const handleClick = vi.fn()

        render(<WalletSaldoCard {...defaultProps} onSolicitarCobro={handleClick} />)

        await user.click(screen.getByRole("button", { name: /solicitar cobro/i }))

        expect(handleClick).toHaveBeenCalledOnce()
    })
})
