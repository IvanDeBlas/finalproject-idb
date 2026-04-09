import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import React from "react"
import { mockInscripcionCreada } from "../../__mocks__/inscripcion.mock"

vi.mock("sonner", () => ({
    toast: { success: vi.fn(), error: vi.fn(), info: vi.fn() },
}))

vi.mock("../../infrastructure/inscripcion.service", () => ({
    inscripcionService: {
        explorarProgramas: vi.fn(),
        solicitarInscripcion: vi.fn(),
        misProgramas: vi.fn(),
    },
}))

import { inscripcionService } from "../../infrastructure/inscripcion.service"
import { useSolicitarInscripcion } from "../../application/hooks/useSolicitarInscripcion"

const createWrapper = () => {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
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

describe("useSolicitarInscripcion", () => {
    const programaId = "3fa85f64-5717-4562-b3fc-2c963f66afa6"

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("mutation executes successfully", async () => {
        vi.mocked(inscripcionService.solicitarInscripcion).mockResolvedValue(mockInscripcionCreada)

        const { result } = renderHook(
            () => useSolicitarInscripcion(),
            { wrapper: createWrapper() }
        )

        await act(async () => {
            result.current.mutate(programaId)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
    })

    it("calls service with programaId", async () => {
        vi.mocked(inscripcionService.solicitarInscripcion).mockResolvedValue(mockInscripcionCreada)

        const { result } = renderHook(
            () => useSolicitarInscripcion(),
            { wrapper: createWrapper() }
        )

        await act(async () => {
            result.current.mutate(programaId)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(inscripcionService.solicitarInscripcion).toHaveBeenCalledWith(programaId)
    })

    it("invalidates explorar-programas query on success", async () => {
        vi.mocked(inscripcionService.solicitarInscripcion).mockResolvedValue(mockInscripcionCreada)

        const queryClient = new QueryClient({
            defaultOptions: { queries: { retry: false, gcTime: 0 }, mutations: { retry: false } },
        })
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const wrapper = function Wrapper({ children }: { children: React.ReactNode }) {
            return React.createElement(
                QueryClientProvider,
                { client: queryClient },
                children
            )
        }

        const { result } = renderHook(() => useSolicitarInscripcion(), { wrapper })

        await act(async () => {
            result.current.mutate(programaId)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(invalidateSpy).toHaveBeenCalledWith(
            expect.objectContaining({
                queryKey: ["crowdpromotion", "programas", "explorar"],
            })
        )
    })

    it("invalidates mis-programas query on success", async () => {
        vi.mocked(inscripcionService.solicitarInscripcion).mockResolvedValue(mockInscripcionCreada)

        const queryClient = new QueryClient({
            defaultOptions: { queries: { retry: false, gcTime: 0 }, mutations: { retry: false } },
        })
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const wrapper = function Wrapper({ children }: { children: React.ReactNode }) {
            return React.createElement(
                QueryClientProvider,
                { client: queryClient },
                children
            )
        }

        const { result } = renderHook(() => useSolicitarInscripcion(), { wrapper })

        await act(async () => {
            result.current.mutate(programaId)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(invalidateSpy).toHaveBeenCalledWith(
            expect.objectContaining({
                queryKey: ["crowdpromotion", "inscripciones", "mis-programas"],
            })
        )
    })

    it("isError true when service returns 400", async () => {
        const error = new Error("Ya estas inscrito en este programa")
        ;(error as Error & { errorCode: string }).errorCode = "4021"
        vi.mocked(inscripcionService.solicitarInscripcion).mockRejectedValue(error)

        const { result } = renderHook(
            () => useSolicitarInscripcion(),
            { wrapper: createWrapper() }
        )

        await act(async () => {
            result.current.mutate(programaId)
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.data).toBeUndefined()
    })

    it("isError true when service returns 403 (bloqueado)", async () => {
        const error = new Error("No puedes inscribirte en este programa")
        ;(error as Error & { errorCode: string }).errorCode = "4022"
        vi.mocked(inscripcionService.solicitarInscripcion).mockRejectedValue(error)

        const { result } = renderHook(
            () => useSolicitarInscripcion(),
            { wrapper: createWrapper() }
        )

        await act(async () => {
            result.current.mutate(programaId)
        })

        await waitFor(() => expect(result.current.isError).toBe(true))
    })

    it("isPending true during mutation", async () => {
        let resolvePromise: (value: typeof mockInscripcionCreada) => void
        vi.mocked(inscripcionService.solicitarInscripcion).mockReturnValue(
            new Promise((resolve) => { resolvePromise = resolve })
        )

        const { result } = renderHook(
            () => useSolicitarInscripcion(),
            { wrapper: createWrapper() }
        )

        act(() => {
            result.current.mutate(programaId)
        })

        await waitFor(() => expect(result.current.isPending).toBe(true))

        await act(async () => {
            resolvePromise!(mockInscripcionCreada)
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
    })
})
