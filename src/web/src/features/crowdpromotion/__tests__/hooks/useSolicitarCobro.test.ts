import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { useSolicitarCobro } from "../../wallet/application/hooks/useSolicitarCobro"
import { mockSolicitarCobroResponse } from "../../__mocks__/wallet.mock"

vi.mock("../../wallet/infrastructure/wallet.service", () => ({
    walletService: {
        solicitarCobro: vi.fn(),
    },
}))

import { walletService } from "../../wallet/infrastructure/wallet.service"
const mockSolicitarCobro = vi.mocked(walletService.solicitarCobro)

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
        },
    })
    return ({ children }: { children: React.ReactNode }) =>
        createElement(QueryClientProvider, { client: queryClient }, children)
}

describe("useSolicitarCobro", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls service with correct data on mutateAsync", async () => {
        mockSolicitarCobro.mockResolvedValueOnce(mockSolicitarCobroResponse)

        const { result } = renderHook(() => useSolicitarCobro(), {
            wrapper: createWrapper(),
        })

        let response: typeof mockSolicitarCobroResponse | undefined
        await act(async () => {
            response = await result.current.mutateAsync({
                importe: 50,
                descripcion: "Test cobro",
            })
        })

        expect(response).toEqual(mockSolicitarCobroResponse)
        expect(mockSolicitarCobro).toHaveBeenCalledWith({
            importe: 50,
            descripcion: "Test cobro",
        })
    })

    it("sets isError on mutation failure", async () => {
        const error = new Error("Saldo insuficiente")
        ;(error as Error & { errorCode: string }).errorCode = "4040"
        mockSolicitarCobro.mockRejectedValueOnce(error)

        const { result } = renderHook(() => useSolicitarCobro(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            try {
                await result.current.mutateAsync({ importe: 1000 })
            } catch {
                // Expected - mutation rejects
            }
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
    })

    it("is idle before mutation is called", () => {
        const { result } = renderHook(() => useSolicitarCobro(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isPending).toBe(false)
        expect(result.current.isError).toBe(false)
    })
})
