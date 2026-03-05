import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import React from "react"
import { mockMisTareasResponse_ConTareas } from "../../__mocks__/tareas.mock"

vi.mock("../../tareas/infrastructure/tareas.service", () => ({
    tareasService: {
        misTareas: vi.fn(),
        completarTarea: vi.fn(),
    },
}))

import { tareasService } from "../../tareas/infrastructure/tareas.service"
import { useMisTareas } from "../../tareas/application/hooks/useMisTareas"

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

describe("useMisTareas", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns data on successful fetch", async () => {
        vi.mocked(tareasService.misTareas).mockResolvedValue(
            mockMisTareasResponse_ConTareas
        )

        const { result } = renderHook(() => useMisTareas("prog-001"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))
        expect(result.current.data).toEqual(mockMisTareasResponse_ConTareas)
    })

    it("isLoading is true initially", () => {
        vi.mocked(tareasService.misTareas).mockReturnValue(
            new Promise(() => {})
        )

        const { result } = renderHook(() => useMisTareas("prog-001"), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("isError is true when the service fails", async () => {
        vi.mocked(tareasService.misTareas).mockRejectedValue(
            new Error("API Error")
        )

        const { result } = renderHook(() => useMisTareas("prog-001"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true), {
            timeout: 5000,
        })
    })

    it("calls the service with the correct programaId", async () => {
        vi.mocked(tareasService.misTareas).mockResolvedValue(
            mockMisTareasResponse_ConTareas
        )

        renderHook(() => useMisTareas("prog-especifico"), {
            wrapper: createWrapper(),
        })

        await waitFor(() =>
            expect(tareasService.misTareas).toHaveBeenCalledWith(
                "prog-especifico"
            )
        )
    })

    it("does not fetch when programaId is empty", () => {
        const { result } = renderHook(() => useMisTareas(""), {
            wrapper: createWrapper(),
        })

        expect(result.current.fetchStatus).toBe("idle")
        expect(tareasService.misTareas).not.toHaveBeenCalled()
    })
})
