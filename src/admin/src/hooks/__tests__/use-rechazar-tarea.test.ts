import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor, act } from "@testing-library/react"
import { createElement } from "react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { useRechazarTarea } from "../use-rechazar-tarea"
import { tareasService } from "@/services/tareas.service"
import {
    PROGRAMA_ID,
    mockRechazarTareaResponse,
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

describe("useRechazarTarea", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls rechazarTarea service on mutate", async () => {
        vi.mocked(tareasService.rechazarTarea).mockResolvedValue(mockRechazarTareaResponse)

        const { result } = renderHook(() => useRechazarTarea(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: PROGRAMA_ID,
                tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
                data: { comentarioValidacion: "La URL no corresponde a la tarea" },
            })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(tareasService.rechazarTarea).toHaveBeenCalledWith(
            PROGRAMA_ID,
            "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
            { comentarioValidacion: "La URL no corresponde a la tarea" }
        )
    })

    it("returns rechazada response", async () => {
        vi.mocked(tareasService.rechazarTarea).mockResolvedValue(mockRechazarTareaResponse)

        const { result } = renderHook(() => useRechazarTarea(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: PROGRAMA_ID,
                tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
                data: { comentarioValidacion: "Motivo de rechazo" },
            })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.estadoTareaId).toBe(4)
        expect(result.current.data?.estadoTareaNombre).toBe("Rechazada")
    })

    it("handles service error", async () => {
        vi.mocked(tareasService.rechazarTarea).mockRejectedValue(new Error("Forbidden"))

        const { result } = renderHook(() => useRechazarTarea(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: PROGRAMA_ID,
                tareaPromotorId: "b2c3d4e5-f6a7-8901-bc23-de45fa678901",
                data: { comentarioValidacion: "Motivo" },
            })
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
        expect(result.current.error).toBeDefined()
    })
})
