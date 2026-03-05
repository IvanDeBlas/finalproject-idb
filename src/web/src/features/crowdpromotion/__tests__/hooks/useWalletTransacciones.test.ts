import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { useWalletTransacciones } from "../../wallet/application/hooks/useWalletTransacciones"
import {
    mockTransaccionesPagedResponse,
    mockTransaccionesPagedResponse_Empty,
} from "../../__mocks__/wallet.mock"

vi.mock("../../wallet/infrastructure/wallet.service", () => ({
    walletService: {
        getTransacciones: vi.fn(),
    },
}))

import { walletService } from "../../wallet/infrastructure/wallet.service"
const mockGetTransacciones = vi.mocked(walletService.getTransacciones)

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
        },
    })
    return ({ children }: { children: React.ReactNode }) =>
        createElement(QueryClientProvider, { client: queryClient }, children)
}

describe("useWalletTransacciones", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns transacciones on success", async () => {
        mockGetTransacciones.mockResolvedValueOnce(mockTransaccionesPagedResponse)

        const { result } = renderHook(
            () => useWalletTransacciones({ page: 1, pageSize: 10 }),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toEqual(mockTransaccionesPagedResponse)
        expect(result.current.data?.items).toHaveLength(4)
    })

    it("passes filters to service", async () => {
        mockGetTransacciones.mockResolvedValueOnce(mockTransaccionesPagedResponse_Empty)

        const filters = { esCredito: true, estadoTransaccionId: 2, page: 1, pageSize: 10 }
        const { result } = renderHook(
            () => useWalletTransacciones(filters),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(mockGetTransacciones).toHaveBeenCalledWith(filters)
    })

    it("sets isError on failure", async () => {
        mockGetTransacciones.mockRejectedValue(new Error("Server Error"))

        const { result } = renderHook(
            () => useWalletTransacciones({ page: 1, pageSize: 10 }),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true), { timeout: 3000 })
    })

    it("returns empty items for no results", async () => {
        mockGetTransacciones.mockResolvedValueOnce(mockTransaccionesPagedResponse_Empty)

        const { result } = renderHook(
            () => useWalletTransacciones({ page: 1, pageSize: 10 }),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.items).toHaveLength(0)
        expect(result.current.data?.totalCount).toBe(0)
    })
})
