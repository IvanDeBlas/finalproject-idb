import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { usePromotorWallet } from "../../wallet/application/hooks/usePromotorWallet"
import { mockWallet } from "../../__mocks__/wallet.mock"

vi.mock("../../wallet/infrastructure/wallet.service", () => ({
    walletService: {
        getWalletResumen: vi.fn(),
    },
}))

import { walletService } from "../../wallet/infrastructure/wallet.service"
const mockGetWalletResumen = vi.mocked(walletService.getWalletResumen)

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
        },
    })
    return ({ children }: { children: React.ReactNode }) =>
        createElement(QueryClientProvider, { client: queryClient }, children)
}

describe("usePromotorWallet", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns wallet data on success", async () => {
        mockGetWalletResumen.mockResolvedValueOnce(mockWallet)

        const { result } = renderHook(() => usePromotorWallet(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toEqual(mockWallet)
        expect(mockGetWalletResumen).toHaveBeenCalledOnce()
    })

    it("sets isError on failure", async () => {
        mockGetWalletResumen.mockRejectedValue(new Error("Network Error"))

        const { result } = renderHook(() => usePromotorWallet(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true), { timeout: 3000 })

        expect(result.current.data).toBeUndefined()
    })

    it("starts in loading state", () => {
        mockGetWalletResumen.mockReturnValue(new Promise(() => {}))

        const { result } = renderHook(() => usePromotorWallet(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })
})
