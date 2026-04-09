import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { MemoryRouter } from "react-router-dom"
import { toast } from "sonner"
import { useCreateBacking } from "../../application/useBackings"
import { backingService } from "../../infrastructure/backing.service"
import type { BackingDto } from "@shared/types/backing"
import React from "react"

// Mock the service
vi.mock("../../infrastructure/backing.service", () => ({
    backingService: {
        create: vi.fn(),
        getByCampaniaId: vi.fn(),
        getMyBackings: vi.fn(),
    },
}))

// Mock sonner
vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

// Mock useNavigate
const mockNavigate = vi.fn()
vi.mock("react-router-dom", async () => {
    const actual = await vi.importActual("react-router-dom")
    return {
        ...actual,
        useNavigate: () => mockNavigate,
    }
})

const mockBackingResponse: BackingDto = {
    id: "900e8400-e29b-41d4-a716-446655440003",
    campaniaId: "550e8400-e29b-41d4-a716-446655440000",
    userId: "user-123",
    rewardId: "223e4567-e89b-12d3-a456-426614174001",
    monto: 25,
    mensaje: "Mucha suerte!",
    esAnonimo: false,
    fechaCreacion: new Date().toISOString(),
    campaniaTitulo: "Mi Album Debut",
    userName: "Juan Perez",
    rewardNombre: "CD Fisico Firmado",
    monedaSimbolo: "EUR",
    estadoPedido: "Completado",
}

function createWrapper(queryClient?: QueryClient) {
    const client = queryClient || new QueryClient({
        defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
        },
    })

    return function Wrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(
            QueryClientProvider,
            { client },
            React.createElement(MemoryRouter, null, children)
        )
    }
}

describe("useCreateBacking", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("creates backing successfully", async () => {
        vi.mocked(backingService.create).mockResolvedValue(mockBackingResponse)

        const { result } = renderHook(() => useCreateBacking(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            campaniaId: "550e8400-e29b-41d4-a716-446655440000",
            rewardId: "223e4567-e89b-12d3-a456-426614174001",
            monto: 25,
            mensaje: "Mucha suerte!",
            esAnonimo: false,
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toEqual(mockBackingResponse)
    })

    it("passes correct data to service", async () => {
        vi.mocked(backingService.create).mockResolvedValue(mockBackingResponse)

        const { result } = renderHook(() => useCreateBacking(), {
            wrapper: createWrapper(),
        })

        const requestData = {
            campaniaId: "550e8400-e29b-41d4-a716-446655440000",
            rewardId: "223e4567-e89b-12d3-a456-426614174001",
            monto: 25,
            mensaje: "Mucha suerte!",
            esAnonimo: false,
        }

        result.current.mutate(requestData)

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(backingService.create).toHaveBeenCalledWith(requestData)
    })

    it("navigates to confirmation page on success", async () => {
        vi.mocked(backingService.create).mockResolvedValue(mockBackingResponse)

        const { result } = renderHook(() => useCreateBacking(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            campaniaId: "550e8400-e29b-41d4-a716-446655440000",
            monto: 25,
            esAnonimo: false,
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(mockNavigate).toHaveBeenCalledWith(
            `/campanias/550e8400-e29b-41d4-a716-446655440000/confirmacion?backingId=${mockBackingResponse.id}`
        )
        expect(toast.success).toHaveBeenCalledWith("Apoyo confirmado!", {
            description: "Gracias por tu contribucion",
        })
    })

    it("invalidates queries on success", async () => {
        vi.mocked(backingService.create).mockResolvedValue(mockBackingResponse)

        const queryClient = new QueryClient({
            defaultOptions: {
                queries: { retry: false },
                mutations: { retry: false },
            },
        })
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const { result } = renderHook(() => useCreateBacking(), {
            wrapper: createWrapper(queryClient),
        })

        result.current.mutate({
            campaniaId: "550e8400-e29b-41d4-a716-446655440000",
            monto: 25,
            esAnonimo: false,
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(invalidateSpy).toHaveBeenCalledWith({
            queryKey: ["backings"],
        })
        expect(invalidateSpy).toHaveBeenCalledWith({
            queryKey: ["campania", "550e8400-e29b-41d4-a716-446655440000"],
        })
        expect(invalidateSpy).toHaveBeenCalledWith({
            queryKey: ["campanias"],
        })
    })

    it("handles error from service", async () => {
        vi.mocked(backingService.create).mockRejectedValue(
            new Error("Error al crear backing")
        )

        const { result } = renderHook(() => useCreateBacking(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            campaniaId: "550e8400-e29b-41d4-a716-446655440000",
            monto: 25,
            esAnonimo: false,
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeDefined()
        expect(mockNavigate).not.toHaveBeenCalled()
        expect(toast.error).toHaveBeenCalledWith("Error al procesar apoyo", {
            description: "Error al crear backing",
        })
    })

    it("allows backing without reward (rewardId undefined)", async () => {
        vi.mocked(backingService.create).mockResolvedValue({
            ...mockBackingResponse,
            rewardId: undefined,
            rewardNombre: undefined,
        })

        const { result } = renderHook(() => useCreateBacking(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            campaniaId: "550e8400-e29b-41d4-a716-446655440000",
            monto: 15,
            esAnonimo: false,
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(backingService.create).toHaveBeenCalledWith(
            expect.objectContaining({
                monto: 15,
            })
        )
        expect(backingService.create).toHaveBeenCalledWith(
            expect.not.objectContaining({
                rewardId: expect.anything(),
            })
        )
    })
})
