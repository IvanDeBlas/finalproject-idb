import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement, type ReactNode } from "react"
import { useProgramaMetricas } from "../use-programa-metricas"
import { metricasService } from "@/services/metricas.service"
import {
    PROGRAMA_ID,
    mockProgramaMetricasResponse,
    mockFiltroConFechas,
} from "@/__mocks__/cp-tracking-metricas.mock"

vi.mock("@/services/metricas.service", () => ({
    metricasService: {
        getProgramaMetricas: vi.fn(),
    },
}))

const mockedGetProgramaMetricas = vi.mocked(metricasService.getProgramaMetricas)

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: ReactNode }) {
        return createElement(QueryClientProvider, { client: queryClient }, children)
    }
}

describe("useProgramaMetricas", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns data on success without filter", async () => {
        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponse)

        const { result } = renderHook(() => useProgramaMetricas(PROGRAMA_ID), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data?.programaId).toBe(PROGRAMA_ID)
    })

    it("returns data on success with date filter", async () => {
        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponse)

        const { result } = renderHook(
            () => useProgramaMetricas(PROGRAMA_ID, mockFiltroConFechas),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data).toEqual(mockProgramaMetricasResponse)
    })

    it("passes programaId and filtro to service", async () => {
        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponse)

        const { result } = renderHook(
            () => useProgramaMetricas(PROGRAMA_ID, mockFiltroConFechas),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(mockedGetProgramaMetricas).toHaveBeenCalledWith(PROGRAMA_ID, mockFiltroConFechas)
    })

    it("handles isLoading state", () => {
        mockedGetProgramaMetricas.mockReturnValue(new Promise(() => {}))

        const { result } = renderHook(() => useProgramaMetricas(PROGRAMA_ID), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("handles isError state", async () => {
        mockedGetProgramaMetricas.mockRejectedValue(new Error("Fail"))

        const { result } = renderHook(() => useProgramaMetricas(PROGRAMA_ID), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
    })

    it("does not fetch when programaId is empty", () => {
        const { result } = renderHook(() => useProgramaMetricas(""), {
            wrapper: createWrapper(),
        })

        expect(result.current.fetchStatus).toBe("idle")
        expect(mockedGetProgramaMetricas).not.toHaveBeenCalled()
    })

    it("does not retry on error", async () => {
        mockedGetProgramaMetricas.mockRejectedValue(new Error("Fail"))

        const { result } = renderHook(() => useProgramaMetricas(PROGRAMA_ID), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(mockedGetProgramaMetricas).toHaveBeenCalledTimes(1)
    })

    it("exposes refetch function", async () => {
        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponse)

        const { result } = renderHook(() => useProgramaMetricas(PROGRAMA_ID), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(typeof result.current.refetch).toBe("function")
    })

    it("uses correct query key with dates", async () => {
        mockedGetProgramaMetricas.mockResolvedValue(mockProgramaMetricasResponse)

        const { result, rerender } = renderHook(
            ({ filtro }) => useProgramaMetricas(PROGRAMA_ID, filtro),
            {
                wrapper: createWrapper(),
                initialProps: { filtro: {} },
            }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        rerender({ filtro: mockFiltroConFechas })

        await waitFor(() =>
            expect(mockedGetProgramaMetricas).toHaveBeenCalledWith(PROGRAMA_ID, mockFiltroConFechas)
        )
    })
})
