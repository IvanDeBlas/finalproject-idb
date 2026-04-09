import { describe, it, expect, vi } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import React from "react"
import { useRewardsByCampania } from "../../application/hooks/useRewardsByCampania"
import { mockRewardsList } from "../../__mocks__/reward.mock"

// Mock the reward service
vi.mock("../../infrastructure/services/reward.service", () => ({
    rewardService: {
        getByCampaniaId: vi.fn(),
        getAll: vi.fn(),
        getById: vi.fn(),
    },
}))

import { rewardService } from "../../infrastructure/services/reward.service"

const createWrapper = () => {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
        },
    })

    return function Wrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(
            QueryClientProvider,
            { client: queryClient },
            children
        )
    }
}

describe("useRewardsByCampania", () => {
    const campaniaId = "550e8400-e29b-41d4-a716-446655440000"

    it("fetches rewards by campaniaId", async () => {
        vi.mocked(rewardService.getByCampaniaId).mockResolvedValue(mockRewardsList)

        const { result } = renderHook(
            () => useRewardsByCampania(campaniaId),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toHaveLength(4)
        expect(rewardService.getByCampaniaId).toHaveBeenCalledWith(
            campaniaId,
            { esActivo: true }
        )
    })

    it("returns empty array when no rewards", async () => {
        vi.mocked(rewardService.getByCampaniaId).mockResolvedValue([])

        const { result } = renderHook(
            () => useRewardsByCampania(campaniaId),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toEqual([])
    })

    it("handles loading state", async () => {
        vi.mocked(rewardService.getByCampaniaId).mockResolvedValue(mockRewardsList)

        const { result } = renderHook(
            () => useRewardsByCampania(campaniaId),
            { wrapper: createWrapper() }
        )

        expect(result.current.isLoading).toBe(true)

        await waitFor(() => expect(result.current.isLoading).toBe(false))
        expect(result.current.isSuccess).toBe(true)
    })

    it("handles error state", async () => {
        vi.mocked(rewardService.getByCampaniaId).mockRejectedValue(
            new Error("API Error")
        )

        const { result } = renderHook(
            () => useRewardsByCampania(campaniaId),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).not.toBeNull()
        expect(result.current.data).toBeUndefined()
    })

    it("sorts rewards by price ascending", async () => {
        const unsortedRewards = [
            { ...mockRewardsList[3], importeMinimo: 100 },
            { ...mockRewardsList[0], importeMinimo: 10 },
            { ...mockRewardsList[2], importeMinimo: 50 },
            { ...mockRewardsList[1], importeMinimo: 25 },
        ]
        vi.mocked(rewardService.getByCampaniaId).mockResolvedValue(unsortedRewards)

        const { result } = renderHook(
            () => useRewardsByCampania(campaniaId),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        const prices = result.current.data!.map((r) => r.importeMinimo)
        expect(prices).toEqual([10, 25, 50, 100])
    })

    it("does not fetch when campaniaId is empty", () => {
        vi.mocked(rewardService.getByCampaniaId).mockResolvedValue([])

        const { result } = renderHook(
            () => useRewardsByCampania(""),
            { wrapper: createWrapper() }
        )

        expect(result.current.isFetching).toBe(false)
    })

    it("does not fetch when enabled is false", () => {
        vi.mocked(rewardService.getByCampaniaId).mockResolvedValue([])

        const { result } = renderHook(
            () => useRewardsByCampania(campaniaId, { enabled: false }),
            { wrapper: createWrapper() }
        )

        expect(result.current.isFetching).toBe(false)
    })
})
