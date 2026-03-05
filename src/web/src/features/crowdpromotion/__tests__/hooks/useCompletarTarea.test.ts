import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import React from "react"
import { mockCompletarTareaResponse } from "../../__mocks__/tareas.mock"

vi.mock("sonner", () => ({
    toast: { success: vi.fn(), error: vi.fn() },
}))

vi.mock("../../tareas/infrastructure/tareas.service", () => ({
    tareasService: {
        misTareas: vi.fn(),
        completarTarea: vi.fn(),
    },
}))

import { toast } from "sonner"
import { tareasService } from "../../tareas/infrastructure/tareas.service"
import { useCompletarTarea } from "../../tareas/application/hooks/useCompletarTarea"

const createWrapper = () => {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
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

const mutationVars = {
    programaId: "prog-001",
    tareaId: "tarea-001",
    data: { urlPruebaCompletado: "https://instagram.com/p/test" },
    esReenvio: false,
}

describe("useCompletarTarea", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("mutation executes successfully", async () => {
        vi.mocked(tareasService.completarTarea).mockResolvedValue(
            mockCompletarTareaResponse
        )

        const { result } = renderHook(() => useCompletarTarea(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate(mutationVars)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
    })

    it("calls service with correct parameters", async () => {
        vi.mocked(tareasService.completarTarea).mockResolvedValue(
            mockCompletarTareaResponse
        )

        const { result } = renderHook(() => useCompletarTarea(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate(mutationVars)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(tareasService.completarTarea).toHaveBeenCalledWith(
            "prog-001",
            "tarea-001",
            { urlPruebaCompletado: "https://instagram.com/p/test" }
        )
    })

    it("shows success toast on completion", async () => {
        vi.mocked(tareasService.completarTarea).mockResolvedValue(
            mockCompletarTareaResponse
        )

        const { result } = renderHook(() => useCompletarTarea(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate(mutationVars)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(toast.success).toHaveBeenCalledWith(
            expect.stringContaining("Tarea enviada")
        )
    })

    it("shows re-send toast when esReenvio is true", async () => {
        vi.mocked(tareasService.completarTarea).mockResolvedValue(
            mockCompletarTareaResponse
        )

        const { result } = renderHook(() => useCompletarTarea(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({ ...mutationVars, esReenvio: true })
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(toast.success).toHaveBeenCalledWith(
            expect.stringContaining("re-enviada")
        )
    })

    it("invalidates mis-tareas query on success", async () => {
        vi.mocked(tareasService.completarTarea).mockResolvedValue(
            mockCompletarTareaResponse
        )

        const queryClient = new QueryClient({
            defaultOptions: {
                queries: { retry: false, gcTime: 0 },
                mutations: { retry: false },
            },
        })
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const wrapper = function Wrapper({
            children,
        }: {
            children: React.ReactNode
        }) {
            return React.createElement(
                QueryClientProvider,
                { client: queryClient },
                children
            )
        }

        const { result } = renderHook(() => useCompletarTarea(), { wrapper })

        await act(async () => {
            result.current.mutate(mutationVars)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(invalidateSpy).toHaveBeenCalledWith(
            expect.objectContaining({
                queryKey: ["crowdpromotion", "tareas", "prog-001", "mis"],
            })
        )
    })

    it("isError true when service returns error 4027", async () => {
        const error = new Error("Tarea no repetible ya completada")
        ;(error as Error & { errorCode: string }).errorCode = "4027"
        vi.mocked(tareasService.completarTarea).mockRejectedValue(error)

        const { result } = renderHook(() => useCompletarTarea(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate(mutationVars)
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(toast.error).toHaveBeenCalledWith(
            expect.stringContaining("no es repetible")
        )
    })

    it("shows specific toast for error 4028", async () => {
        const error = new Error("Limite alcanzado")
        ;(error as Error & { errorCode: string }).errorCode = "4028"
        vi.mocked(tareasService.completarTarea).mockRejectedValue(error)

        const { result } = renderHook(() => useCompletarTarea(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate(mutationVars)
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(toast.error).toHaveBeenCalledWith(
            expect.stringContaining("maximo de repeticiones")
        )
    })

    it("isPending is true during mutation", async () => {
        let resolvePromise: (value: typeof mockCompletarTareaResponse) => void
        vi.mocked(tareasService.completarTarea).mockReturnValue(
            new Promise((resolve) => {
                resolvePromise = resolve
            })
        )

        const { result } = renderHook(() => useCompletarTarea(), {
            wrapper: createWrapper(),
        })

        act(() => {
            result.current.mutate(mutationVars)
        })

        await waitFor(() => expect(result.current.isPending).toBe(true))

        await act(async () => {
            resolvePromise!(mockCompletarTareaResponse)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
    })
})
