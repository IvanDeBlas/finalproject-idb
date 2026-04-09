import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { useGenerarNecesidades } from "../../application/hooks/useGenerarNecesidades"
import { mockGenerarResult } from "../../__mocks__/crowdsourcing.mock"
import type { GenerarNecesidadesRequest } from "../../domain"

vi.mock("../../infrastructure", () => ({
    crowdsourcingApi: {
        generarNecesidades: vi.fn(),
    },
}))

import { crowdsourcingApi } from "../../infrastructure"

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return createElement(QueryClientProvider, { client: queryClient }, children)
    }
}

const validPayload: { templateId: string; data: GenerarNecesidadesRequest } = {
    templateId: "tpl-001",
    data: {
        proyectoArtisticoId: "proj-001",
        necesidadesSeleccionadas: [
            {
                plantillaNecesidadId: "nec-001",
                presupuestoMin: 500,
                presupuestoMax: 1500,
                monedaId: 1,
            },
        ],
    },
}

describe("useGenerarNecesidades", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("generates necesidades successfully", async () => {
        vi.mocked(crowdsourcingApi.generarNecesidades).mockResolvedValue(
            mockGenerarResult
        )

        const { result } = renderHook(() => useGenerarNecesidades(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(validPayload)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.necesidadesCreadas).toBe(2)
        expect(result.current.data?.necesidadIds).toHaveLength(2)
    })

    it("calls API with correct parameters", async () => {
        vi.mocked(crowdsourcingApi.generarNecesidades).mockResolvedValue(
            mockGenerarResult
        )

        const { result } = renderHook(() => useGenerarNecesidades(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(validPayload)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(crowdsourcingApi.generarNecesidades).toHaveBeenCalledWith(
            "tpl-001",
            validPayload.data
        )
    })

    it("sets isPending during mutation lifecycle", async () => {
        vi.mocked(crowdsourcingApi.generarNecesidades).mockResolvedValue(
            mockGenerarResult
        )

        const { result } = renderHook(() => useGenerarNecesidades(), {
            wrapper: createWrapper(),
        })

        // Before mutation, isPending should be false
        expect(result.current.isPending).toBe(false)

        act(() => {
            result.current.mutate(validPayload)
        })

        // After mutation completes, isPending is false
        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.isPending).toBe(false)
    })

    it("handles error state", async () => {
        vi.mocked(crowdsourcingApi.generarNecesidades).mockRejectedValue(
            new Error("Server error")
        )

        const { result } = renderHook(() => useGenerarNecesidades(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(validPayload)
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeTruthy()
    })

    it("calls onSuccess callback with result data", async () => {
        vi.mocked(crowdsourcingApi.generarNecesidades).mockResolvedValue(
            mockGenerarResult
        )

        const onSuccess = vi.fn()
        const { result } = renderHook(() => useGenerarNecesidades(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(validPayload, { onSuccess })
        })

        await waitFor(() => expect(onSuccess).toHaveBeenCalled())

        // First argument is the result data
        expect(onSuccess.mock.calls[0][0]).toEqual(mockGenerarResult)
        // Second argument is the variables
        expect(onSuccess.mock.calls[0][1]).toEqual(validPayload)
    })
})
