import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import type { ReactNode } from "react"
import { createElement } from "react"
import { useMisProgramas, usePromoPrograma } from "../use-promo-programas"
import { promoProgramaService } from "@/services/promo-programa.service"
import {
    mockProgramaListResult,
    mockProgramaDetail,
} from "@/__mocks__/promo-programa.mock"

vi.mock("@/services/promo-programa.service", () => ({
    promoProgramaService: {
        getMisProgramas: vi.fn(),
        getById: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        desactivar: vi.fn(),
    },
}))

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

describe("useMisProgramas", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns paginated data on success", async () => {
        vi.mocked(promoProgramaService.getMisProgramas).mockResolvedValue(mockProgramaListResult)

        const { result } = renderHook(() => useMisProgramas(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.items).toHaveLength(2)
        expect(result.current.data?.totalCount).toBe(2)
    })

    it("passes filter params to service", async () => {
        vi.mocked(promoProgramaService.getMisProgramas).mockResolvedValue(mockProgramaListResult)

        renderHook(() => useMisProgramas({ esActivo: true, page: 2, pageSize: 5 }), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(promoProgramaService.getMisProgramas).toHaveBeenCalledWith({
                esActivo: true,
                page: 2,
                pageSize: 5,
            })
        })
    })

    it("handles loading state", () => {
        vi.mocked(promoProgramaService.getMisProgramas).mockReturnValue(new Promise(() => {}))

        const { result } = renderHook(() => useMisProgramas(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("handles error state", async () => {
        vi.mocked(promoProgramaService.getMisProgramas).mockRejectedValue(new Error("Fail"))

        const { result } = renderHook(() => useMisProgramas(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})

describe("usePromoPrograma", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns program detail on success", async () => {
        vi.mocked(promoProgramaService.getById).mockResolvedValue(mockProgramaDetail)

        const { result } = renderHook(() => usePromoPrograma("programa-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.titulo).toBe("Campana de Referidos Q1")
    })

    it("returns null when not found", async () => {
        vi.mocked(promoProgramaService.getById).mockResolvedValue(null)

        const { result } = renderHook(() => usePromoPrograma("nonexistent"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data).toBeNull()
    })

    it("does not fetch when id is empty", () => {
        const { result } = renderHook(() => usePromoPrograma(""), {
            wrapper: createWrapper(),
        })

        expect(result.current.fetchStatus).toBe("idle")
        expect(promoProgramaService.getById).not.toHaveBeenCalled()
    })

    it("does not retry on error", async () => {
        vi.mocked(promoProgramaService.getById).mockRejectedValue(new Error("Fail"))

        const { result } = renderHook(() => usePromoPrograma("programa-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })

        expect(promoProgramaService.getById).toHaveBeenCalledTimes(1)
    })
})
