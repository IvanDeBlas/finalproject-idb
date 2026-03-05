import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor, act } from "@testing-library/react"
import { createElement } from "react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { useValidarTarea } from "../use-validar-tarea"
import { tareasService } from "@/services/tareas.service"
import {
    PROGRAMA_ID,
    mockValidarTareaResponse,
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

describe("useValidarTarea", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls validarTarea service on mutate", async () => {
        vi.mocked(tareasService.validarTarea).mockResolvedValue(mockValidarTareaResponse)

        const { result } = renderHook(() => useValidarTarea(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: PROGRAMA_ID,
                tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
                data: { comentarioValidacion: undefined },
            })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(tareasService.validarTarea).toHaveBeenCalledWith(
            PROGRAMA_ID,
            "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
            { comentarioValidacion: undefined }
        )
    })

    it("returns success response data", async () => {
        vi.mocked(tareasService.validarTarea).mockResolvedValue(mockValidarTareaResponse)

        const { result } = renderHook(() => useValidarTarea(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: PROGRAMA_ID,
                tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
                data: {},
            })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.estadoTareaId).toBe(3)
        expect(result.current.data?.recompensaAcreditada).toBe(5.00)
    })

    it("handles service error", async () => {
        vi.mocked(tareasService.validarTarea).mockRejectedValue(new Error("Server error"))

        const { result } = renderHook(() => useValidarTarea(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: PROGRAMA_ID,
                tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
                data: {},
            })
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(result.current.error).toBeDefined()
    })
})
