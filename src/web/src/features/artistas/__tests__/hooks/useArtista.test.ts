import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import React from "react"
import { useArtista } from "../../application/hooks/useArtista"
import { artistaService } from "../../infrastructure/artista.service"
import type { Artista } from "../../domain/types"

// Mock the service
vi.mock("../../infrastructure/artista.service", () => ({
    artistaService: {
        getById: vi.fn(),
        getMyProfile: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
    },
}))

// Mock the shared constants
vi.mock("@shared/constants", () => ({
    QUERY_KEYS: {
        artistas: {
            byId: (id: string) => ["artistas", id],
        },
    },
}))

const mockArtista: Artista = {
    id: "artista-001",
    userId: "user-123",
    nombreArtistico: "Test Artist",
    descripcion: "Rock band from Madrid",
    pais: "Espana",
    ciudad: "Madrid",
    imagenUrl: "https://example.com/photo.jpg",
    generoMusical: "Rock",
    createdAt: new Date("2026-01-15T10:00:00Z"),
    updatedAt: new Date("2026-02-20T15:30:00Z"),
}

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
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

describe("useArtista", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("fetches artista by id successfully", async () => {
        vi.mocked(artistaService.getById).mockResolvedValue(mockArtista)

        const { result } = renderHook(
            () => useArtista("artista-001"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toEqual(mockArtista)
        expect(result.current.data?.nombreArtistico).toBe("Test Artist")
        expect(artistaService.getById).toHaveBeenCalledWith("artista-001")
    })

    it("handles loading state", () => {
        vi.mocked(artistaService.getById).mockReturnValue(
            new Promise(() => {})
        )

        const { result } = renderHook(
            () => useArtista("artista-001"),
            { wrapper: createWrapper() }
        )

        expect(result.current.isLoading).toBe(true)
        expect(result.current.data).toBeUndefined()
    })

    it("is disabled when id is empty", () => {
        vi.mocked(artistaService.getById).mockResolvedValue(mockArtista)

        const { result } = renderHook(
            () => useArtista(""),
            { wrapper: createWrapper() }
        )

        expect(result.current.isFetching).toBe(false)
        expect(artistaService.getById).not.toHaveBeenCalled()
    })

    it("handles error state", async () => {
        vi.mocked(artistaService.getById).mockRejectedValue(
            new Error("Not found")
        )

        const { result } = renderHook(
            () => useArtista("non-existent"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeTruthy()
        expect(result.current.data).toBeUndefined()
    })
})
