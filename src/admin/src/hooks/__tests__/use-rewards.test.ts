import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import type { ReactNode } from "react"
import { createElement } from "react"
import {
    useRewards,
    useCreateReward,
    useUpdateReward,
    useDeleteReward,
    useReorderRewards,
} from "../use-rewards"
import { rewardService } from "@/services/reward.service"
import type { Reward, CreateRewardRequest } from "@shared/types"

// Mock the service
vi.mock("@/services/reward.service", () => ({
    rewardService: {
        getByCampania: vi.fn(),
        getById: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        delete: vi.fn(),
        reorder: vi.fn(),
    },
}))

const mockReward: Reward = {
    id: "reward-1",
    campaniaId: "campania-1",
    tipoRewardId: 1,
    nombre: "Descarga Digital",
    importeMinimo: 10,
    monedaId: 1,
    esAddOn: false,
    incluyeEnvioFisico: false,
    orden: 1,
    esActivo: true,
    fechaCreacion: "2026-01-01T00:00:00Z",
}

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: ReactNode }) {
        return createElement(
            QueryClientProvider,
            { client: queryClient },
            children
        )
    }
}

describe("useRewards", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns data on success", async () => {
        vi.mocked(rewardService.getByCampania).mockResolvedValue([mockReward])

        const { result } = renderHook(() => useRewards("campania-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data).toHaveLength(1)
        expect(result.current.data?.[0].nombre).toBe("Descarga Digital")
    })

    it("handles loading state", () => {
        vi.mocked(rewardService.getByCampania).mockReturnValue(
            new Promise(() => {})
        )

        const { result } = renderHook(() => useRewards("campania-1"), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("handles error state", async () => {
        vi.mocked(rewardService.getByCampania).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(() => useRewards("campania-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })

        expect(result.current.error).toBeDefined()
    })

    it("does not fetch when campaniaId is empty", () => {
        renderHook(() => useRewards(""), {
            wrapper: createWrapper(),
        })

        expect(rewardService.getByCampania).not.toHaveBeenCalled()
    })
})

describe("useCreateReward", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls create service on mutate", async () => {
        vi.mocked(rewardService.create).mockResolvedValue(mockReward)

        const { result } = renderHook(() => useCreateReward(), {
            wrapper: createWrapper(),
        })

        const createData: CreateRewardRequest = {
            campaniaId: "campania-1",
            tipoRewardId: 1,
            nombre: "Test",
            importeMinimo: 10,
            monedaId: 1,
            esAddOn: false,
            incluyeEnvioFisico: false,
            orden: 1,
        }

        await act(async () => {
            result.current.mutate(createData)
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(rewardService.create).toHaveBeenCalledWith(createData)
    })

    it("handles create error", async () => {
        vi.mocked(rewardService.create).mockRejectedValue(
            new Error("Create failed")
        )

        const { result } = renderHook(() => useCreateReward(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                campaniaId: "campania-1",
                tipoRewardId: 1,
                nombre: "Test",
                importeMinimo: 10,
                monedaId: 1,
                esAddOn: false,
                incluyeEnvioFisico: false,
                orden: 1,
            })
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})

describe("useUpdateReward", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls update service on mutate", async () => {
        vi.mocked(rewardService.update).mockResolvedValue(mockReward)

        const { result } = renderHook(() => useUpdateReward(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                id: "reward-1",
                data: { id: "reward-1", nombre: "Updated" },
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(rewardService.update).toHaveBeenCalledWith("reward-1", {
            id: "reward-1",
            nombre: "Updated",
        })
    })
})

describe("useDeleteReward", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls delete service on mutate", async () => {
        vi.mocked(rewardService.delete).mockResolvedValue()

        const { result } = renderHook(() => useDeleteReward(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate("reward-1")
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(rewardService.delete).toHaveBeenCalledWith("reward-1")
    })
})

describe("useReorderRewards", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls reorder service on mutate", async () => {
        vi.mocked(rewardService.reorder).mockResolvedValue()

        const { result } = renderHook(() => useReorderRewards(), {
            wrapper: createWrapper(),
        })

        const reorderData = {
            campaniaId: "campania-1",
            rewardOrders: [
                { rewardId: "reward-1", orden: 2 },
                { rewardId: "reward-2", orden: 1 },
            ],
        }

        await act(async () => {
            result.current.mutate(reorderData)
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(rewardService.reorder).toHaveBeenCalledWith(reorderData)
    })

    it("handles reorder error", async () => {
        vi.mocked(rewardService.reorder).mockRejectedValue(
            new Error("Reorder failed")
        )

        const { result } = renderHook(() => useReorderRewards(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                campaniaId: "campania-1",
                rewardOrders: [{ rewardId: "reward-1", orden: 1 }],
            })
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})
