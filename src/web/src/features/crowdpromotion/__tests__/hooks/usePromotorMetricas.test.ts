import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import React from "react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import {
    mockPromotorMetricasResponse,
    mockPromotorMetricasResponse_SinEventos,
} from "../../__mocks__/tracking.mock"

vi.mock("../../metricas/infrastructure/tracking.service", () => ({
    trackingService: {
        registrarEvento: vi.fn(),
        getMetricasPromotor: vi.fn(),
    },
}))

import { trackingService } from "../../metricas/infrastructure/tracking.service"
import { usePromotorMetricas } from "../../metricas/application/hooks/usePromotorMetricas"

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(QueryClientProvider, { client: queryClient }, children)
    }
}

describe("usePromotorMetricas", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns data correctly on success", async () => {
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        const { result } = renderHook(() => usePromotorMetricas("prog-001"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data).toEqual(mockPromotorMetricasResponse)
    })

    it("isLoading is true initially", () => {
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        const { result } = renderHook(() => usePromotorMetricas("prog-001"), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("isError is true when the service fails", async () => {
        vi.mocked(trackingService.getMetricasPromotor).mockRejectedValue(
            new Error("Server error")
        )

        const { result } = renderHook(() => usePromotorMetricas("prog-001"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true), { timeout: 5000 })
    })

    it("calls service with the correct programaId", async () => {
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        renderHook(() => usePromotorMetricas("prog-xyz"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(trackingService.getMetricasPromotor).toHaveBeenCalledWith("prog-xyz")
        })
    })

    it("does not fetch when programaId is undefined", () => {
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        const { result } = renderHook(() => usePromotorMetricas(undefined), {
            wrapper: createWrapper(),
        })

        expect(result.current.fetchStatus).toBe("idle")
        expect(trackingService.getMetricasPromotor).not.toHaveBeenCalled()
    })

    it("does not fetch when programaId is empty string", () => {
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse
        )

        const { result } = renderHook(() => usePromotorMetricas(""), {
            wrapper: createWrapper(),
        })

        expect(result.current.fetchStatus).toBe("idle")
    })

    it("returns kpis with zero values when no activity", async () => {
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse_SinEventos
        )

        const { result } = renderHook(() => usePromotorMetricas("prog-001"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data?.kpis.misClicks).toBe(0)
    })

    it("returns empty eventosRecientes when no events", async () => {
        vi.mocked(trackingService.getMetricasPromotor).mockResolvedValue(
            mockPromotorMetricasResponse_SinEventos
        )

        const { result } = renderHook(() => usePromotorMetricas("prog-001"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data?.eventosRecientes).toHaveLength(0)
    })
})
