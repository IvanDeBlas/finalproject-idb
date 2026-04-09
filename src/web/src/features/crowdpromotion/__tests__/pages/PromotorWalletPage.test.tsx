import { describe, it, expect, vi, beforeEach } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import PromotorWalletPage from "../../wallet/presentation/pages/PromotorWalletPage"
import {
    mockWallet,
    mockTransaccionesPagedResponse,
    mockTransaccionesPagedResponse_Empty,
} from "../../__mocks__/wallet.mock"

vi.mock("../../wallet/infrastructure/wallet.service", () => ({
    walletService: {
        getWalletResumen: vi.fn(),
        getTransacciones: vi.fn(),
        solicitarCobro: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

import { walletService } from "../../wallet/infrastructure/wallet.service"
const mockGetWalletResumen = vi.mocked(walletService.getWalletResumen)
const mockGetTransacciones = vi.mocked(walletService.getTransacciones)

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: { queries: { retry: false, gcTime: 0 } },
    })
    return ({ children }: { children: React.ReactNode }) => (
        <QueryClientProvider client={queryClient}>
            <MemoryRouter>{children}</MemoryRouter>
        </QueryClientProvider>
    )
}

describe("PromotorWalletPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders page title", () => {
        mockGetWalletResumen.mockReturnValue(new Promise(() => {}))
        mockGetTransacciones.mockReturnValue(new Promise(() => {}))

        const Wrapper = createWrapper()
        render(<Wrapper><PromotorWalletPage /></Wrapper>)

        expect(screen.getByText("Mi Wallet")).toBeDefined()
    })

    it("shows wallet data and transactions on success", async () => {
        mockGetWalletResumen.mockResolvedValueOnce(mockWallet)
        mockGetTransacciones.mockResolvedValueOnce(mockTransaccionesPagedResponse)

        const Wrapper = createWrapper()
        render(<Wrapper><PromotorWalletPage /></Wrapper>)

        await waitFor(() => {
            expect(screen.getByText(/150,50/)).toBeDefined()
        })

        expect(screen.getByText(/comision backing/i)).toBeDefined()
    })

    it("shows error state when wallet fails to load", async () => {
        mockGetWalletResumen.mockRejectedValue(new Error("Server Error"))
        mockGetTransacciones.mockResolvedValue(mockTransaccionesPagedResponse_Empty)

        const Wrapper = createWrapper()
        render(<Wrapper><PromotorWalletPage /></Wrapper>)

        await waitFor(() => {
            expect(screen.getByText(/no se pudo cargar/i)).toBeDefined()
        }, { timeout: 3000 })

        expect(screen.getByRole("button", { name: /reintentar/i })).toBeDefined()
    })

    it("shows empty state when no transactions exist", async () => {
        mockGetWalletResumen.mockResolvedValueOnce(mockWallet)
        mockGetTransacciones.mockResolvedValueOnce(mockTransaccionesPagedResponse_Empty)

        const Wrapper = createWrapper()
        render(<Wrapper><PromotorWalletPage /></Wrapper>)

        await waitFor(() => {
            expect(screen.getByText(/aun no tienes transacciones/i)).toBeDefined()
        })

        expect(screen.getByText(/explorar programas/i)).toBeDefined()
    })

    it("shows solicitar cobro button in wallet card", async () => {
        mockGetWalletResumen.mockResolvedValueOnce(mockWallet)
        mockGetTransacciones.mockResolvedValueOnce(mockTransaccionesPagedResponse)

        const Wrapper = createWrapper()
        render(<Wrapper><PromotorWalletPage /></Wrapper>)

        await waitFor(() => {
            expect(screen.getByRole("button", { name: /solicitar cobro/i })).toBeDefined()
        })
    })
})
