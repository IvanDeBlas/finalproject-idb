import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import React from "react"
import {
    useMyArtistProfile,
    useCreateArtista,
    useUpdateArtista,
} from "../application/useArtista"
import { artistaService } from "../infrastructure/artista.service"
import type { Artista } from "../domain/types"

// Mock the service
vi.mock("../infrastructure/artista.service", () => ({
    artistaService: {
        getById: vi.fn(),
        getMyProfile: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
    },
}))

const mockArtista: Artista = {
    id: "artista-001",
    userId: "user-123",
    nombreArtistico: "Test Artist",
    descripcion: "Una banda de rock",
    pais: "Espana",
    ciudad: "Madrid",
    imagenUrl: "https://example.com/photo.jpg",
    generoMusical: "Rock",
    createdAt: new Date("2026-01-15T10:00:00Z"),
    updatedAt: new Date("2026-02-20T15:30:00Z"),
}

function createWrapper(queryClient?: QueryClient) {
    const client = queryClient || new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0, staleTime: 0 },
            mutations: { retry: false },
        },
    })

    return function Wrapper({ children }: { children: React.ReactNode }) {
        return React.createElement(
            QueryClientProvider,
            { client },
            children
        )
    }
}

describe("useMyArtistProfile", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("fetches artist profile successfully", async () => {
        vi.mocked(artistaService.getMyProfile).mockResolvedValue(mockArtista)

        const { result } = renderHook(() => useMyArtistProfile(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toEqual(mockArtista)
        expect(result.current.data?.nombreArtistico).toBe("Test Artist")
    })

    it("handles loading state", () => {
        vi.mocked(artistaService.getMyProfile).mockReturnValue(
            new Promise(() => {})
        )

        const { result } = renderHook(() => useMyArtistProfile(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
        expect(result.current.data).toBeUndefined()
    })

    it("returns null when no artist profile exists", async () => {
        vi.mocked(artistaService.getMyProfile).mockResolvedValue(null)

        const { result } = renderHook(() => useMyArtistProfile(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toBeNull()
    })

    it("handles error state", async () => {
        vi.mocked(artistaService.getMyProfile).mockRejectedValue(
            new Error("Unauthorized")
        )

        const { result } = renderHook(() => useMyArtistProfile(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeTruthy()
    })
})

describe("useCreateArtista", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("creates artista successfully", async () => {
        vi.mocked(artistaService.create).mockResolvedValue(mockArtista)

        const { result } = renderHook(() => useCreateArtista(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            nombreArtistico: "Test Artist",
            descripcion: "Una banda de rock",
            generoMusical: "Rock",
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toEqual(mockArtista)
    })

    it("passes correct data to service", async () => {
        vi.mocked(artistaService.create).mockResolvedValue(mockArtista)

        const { result } = renderHook(() => useCreateArtista(), {
            wrapper: createWrapper(),
        })

        const createData = {
            nombreArtistico: "New Artist",
            descripcion: "Descripcion",
            generoMusical: "Pop",
        }

        result.current.mutate(createData)

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(artistaService.create).toHaveBeenCalledWith(createData)
    })

    it("invalidates artist profile query on success", async () => {
        vi.mocked(artistaService.create).mockResolvedValue(mockArtista)

        const queryClient = new QueryClient({
            defaultOptions: {
                queries: { retry: false },
                mutations: { retry: false },
            },
        })
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const { result } = renderHook(() => useCreateArtista(), {
            wrapper: createWrapper(queryClient),
        })

        result.current.mutate({
            nombreArtistico: "Test Artist",
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(invalidateSpy).toHaveBeenCalledWith({
            queryKey: ["artista", "me"],
        })
    })

    it("handles creation error", async () => {
        vi.mocked(artistaService.create).mockRejectedValue(
            new Error("Artista already exists")
        )

        const { result } = renderHook(() => useCreateArtista(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            nombreArtistico: "Duplicate Artist",
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeDefined()
    })
})

describe("useUpdateArtista", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("updates artista successfully", async () => {
        const updatedArtista = {
            ...mockArtista,
            nombreArtistico: "Updated Name",
        }
        vi.mocked(artistaService.update).mockResolvedValue(updatedArtista)

        const { result } = renderHook(() => useUpdateArtista(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            id: "artista-001",
            data: { nombreArtistico: "Updated Name" },
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.nombreArtistico).toBe("Updated Name")
    })

    it("passes id and data to service", async () => {
        vi.mocked(artistaService.update).mockResolvedValue(mockArtista)

        const { result } = renderHook(() => useUpdateArtista(), {
            wrapper: createWrapper(),
        })

        const updateData = { nombreArtistico: "New Name", generoMusical: "Jazz" }

        result.current.mutate({ id: "artista-001", data: updateData })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(artistaService.update).toHaveBeenCalledWith(
            "artista-001",
            updateData
        )
    })

    it("invalidates artist profile query on success", async () => {
        vi.mocked(artistaService.update).mockResolvedValue(mockArtista)

        const queryClient = new QueryClient({
            defaultOptions: {
                queries: { retry: false },
                mutations: { retry: false },
            },
        })
        const invalidateSpy = vi.spyOn(queryClient, "invalidateQueries")

        const { result } = renderHook(() => useUpdateArtista(), {
            wrapper: createWrapper(queryClient),
        })

        result.current.mutate({
            id: "artista-001",
            data: { nombreArtistico: "Updated" },
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(invalidateSpy).toHaveBeenCalledWith({
            queryKey: ["artista", "me"],
        })
    })

    it("handles update error", async () => {
        vi.mocked(artistaService.update).mockRejectedValue(
            new Error("Not found")
        )

        const { result } = renderHook(() => useUpdateArtista(), {
            wrapper: createWrapper(),
        })

        result.current.mutate({
            id: "non-existent",
            data: { nombreArtistico: "Test" },
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeDefined()
    })
})
