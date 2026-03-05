import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { createElement } from "react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { useTareasPendientes } from "../use-tareas-pendientes"
import { tareasService } from "@/services/tareas.service"
import {
    PROGRAMA_ID,
    mockTareasPendientesResponse,
} from "@/__mocks__/cp-tareas-promocion.mock"
import type { ReactNode } from "react"

vi.mock("@/services/tareas.service", () => ({
    tareasService: {
        getTareasPendientes: vi.fn(),
        validarTarea: vi.fn(),
        rechazarTarea: vi.fn(),
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
        return createElement(QueryClientProvider, { client: queryClient }, children)
    }
}

describe("useTareasPendientes", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns paginated data on success", async () => {
        vi.mocked(tareasService.getTareasPendientes).mockResolvedValue(
            mockTareasPendientesResponse
        )

        const { result } = renderHook(
            () => useTareasPendientes(PROGRAMA_ID),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.items).toHaveLength(2)
        expect(result.current.data?.totalCount).toBe(2)
        expect(result.current.data?.page).toBe(1)
    })

    it("handles loading state initially", () => {
        vi.mocked(tareasService.getTareasPendientes).mockReturnValue(
            new Promise(() => {})
        )

        const { result } = renderHook(
            () => useTareasPendientes(PROGRAMA_ID),
            { wrapper: createWrapper() }
        )

        expect(result.current.isLoading).toBe(true)
    })

    it("handles error state when service fails", async () => {
        vi.mocked(tareasService.getTareasPendientes).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(
            () => useTareasPendientes(PROGRAMA_ID),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(result.current.error).toBeDefined()
    })

    it("does not fetch when programaId is empty", () => {
        const { result } = renderHook(
            () => useTareasPendientes(""),
            { wrapper: createWrapper() }
        )

        expect(result.current.fetchStatus).toBe("idle")
        expect(tareasService.getTareasPendientes).not.toHaveBeenCalled()
    })

    it("calls service with correct params including pagination", async () => {
        vi.mocked(tareasService.getTareasPendientes).mockResolvedValue(
            mockTareasPendientesResponse
        )

        const { result } = renderHook(
            () => useTareasPendientes(PROGRAMA_ID, { page: 2, pageSize: 5 }),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(tareasService.getTareasPendientes).toHaveBeenCalledWith(
            PROGRAMA_ID,
            { page: 2, pageSize: 5 }
        )
    })
})
