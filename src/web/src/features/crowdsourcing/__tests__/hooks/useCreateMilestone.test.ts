import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { useCreateMilestone } from "../../application/hooks/useCreateMilestone"
import { mockMilestoneCreatedResult } from "../../__mocks__/acuerdo.mock"
import type { CreateMilestoneRequest } from "../../domain"

vi.mock("../../infrastructure", () => ({
    milestoneApi: {
        create: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

import { milestoneApi } from "../../infrastructure"

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

const validData: CreateMilestoneRequest = {
    titulo: "Mezcla de pistas 1-3",
    descripcion: "Mezcla de las primeras 3 canciones",
    importeParcial: 270,
    fechaLimite: "2026-03-08T00:00:00Z",
}

const acuerdoId = "e1f2a3b4-c5d6-7e8f-9a0b-1c2d3e4f5a6b"

describe("useCreateMilestone", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls milestoneApi.create with acuerdoId and data", async () => {
        vi.mocked(milestoneApi.create).mockResolvedValue(
            mockMilestoneCreatedResult
        )

        const { result } = renderHook(
            () => useCreateMilestone(acuerdoId),
            { wrapper: createWrapper() }
        )

        act(() => {
            result.current.mutate(validData)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(vi.mocked(milestoneApi.create)).toHaveBeenCalledWith(
            acuerdoId,
            validData
        )
    })

    it("calls onSuccess callback on success", async () => {
        vi.mocked(milestoneApi.create).mockResolvedValue(
            mockMilestoneCreatedResult
        )

        const onSuccess = vi.fn()
        const { result } = renderHook(
            () => useCreateMilestone(acuerdoId, onSuccess),
            { wrapper: createWrapper() }
        )

        act(() => {
            result.current.mutate(validData)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(onSuccess).toHaveBeenCalled()
    })

    it("handles error", async () => {
        vi.mocked(milestoneApi.create).mockRejectedValue(
            new Error("Server error")
        )

        const { result } = renderHook(
            () => useCreateMilestone(acuerdoId),
            { wrapper: createWrapper() }
        )

        act(() => {
            result.current.mutate(validData)
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeTruthy()
    })
})
