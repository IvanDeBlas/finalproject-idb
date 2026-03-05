import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import React from "react"
import { useCampanias, useCampania, useMisCampanias } from "../../application/useCampanias"
import {
    mockCampaniasList,
    mockCampaniaFull,
} from "../../__mocks__/campania.mock"

// Mock the API service
vi.mock("../../infrastructure/campania.api", () => ({
    campaniaApi: {
        getAll: vi.fn(),
        getById: vi.fn(),
        getByArtistaId: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        publicar: vi.fn(),
    },
    campaniaService: {
        getAll: vi.fn(),
        getById: vi.fn(),
        getByArtistaId: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        publicar: vi.fn(),
    },
}))

import { campaniaApi } from "../../infrastructure/campania.api"

const createWrapper = () => {
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

describe("useCampanias", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("fetches campanias successfully", async () => {
        vi.mocked(campaniaApi.getAll).mockResolvedValue(mockCampaniasList)

        const { result } = renderHook(
            () => useCampanias(),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toHaveLength(3)
        expect(result.current.data![0].titulo).toBe("Mi Primer Album - Rock Alternativo")
    })

    it("handles loading state", () => {
        vi.mocked(campaniaApi.getAll).mockReturnValue(new Promise(() => {}))

        const { result } = renderHook(
            () => useCampanias(),
            { wrapper: createWrapper() }
        )

        expect(result.current.isLoading).toBe(true)
        expect(result.current.data).toBeUndefined()
    })

    it("handles error state", async () => {
        vi.mocked(campaniaApi.getAll).mockRejectedValue(new Error("Network error"))

        const { result } = renderHook(
            () => useCampanias(),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeTruthy()
        expect(result.current.data).toBeUndefined()
    })

    it("returns empty array when backend returns empty", async () => {
        vi.mocked(campaniaApi.getAll).mockResolvedValue([])

        const { result } = renderHook(
            () => useCampanias(),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toEqual([])
    })

    it("passes filters to API service", async () => {
        vi.mocked(campaniaApi.getAll).mockResolvedValue(mockCampaniasList)
        const filters = { searchTerm: "rock", estadoCampaniaId: 2 }

        renderHook(
            () => useCampanias(filters),
            { wrapper: createWrapper() }
        )

        await waitFor(() => {
            expect(campaniaApi.getAll).toHaveBeenCalledWith(filters, undefined)
        })
    })

    it("passes pagination to API service", async () => {
        vi.mocked(campaniaApi.getAll).mockResolvedValue(mockCampaniasList)
        const filters = { searchTerm: "test" }
        const pagination = { pageNumber: 2, pageSize: 10 }

        renderHook(
            () => useCampanias(filters, pagination),
            { wrapper: createWrapper() }
        )

        await waitFor(() => {
            expect(campaniaApi.getAll).toHaveBeenCalledWith(filters, pagination)
        })
    })
})

describe("useCampania", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("fetches single campania by id", async () => {
        vi.mocked(campaniaApi.getById).mockResolvedValue(mockCampaniaFull)

        const { result } = renderHook(
            () => useCampania("550e8400-e29b-41d4-a716-446655440000"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toBeTruthy()
        expect(result.current.data!.titulo).toBe("Mi Primer Album - Rock Alternativo")
        expect(campaniaApi.getById).toHaveBeenCalledWith("550e8400-e29b-41d4-a716-446655440000")
    })

    it("returns null when campania not found", async () => {
        vi.mocked(campaniaApi.getById).mockResolvedValue(null)

        const { result } = renderHook(
            () => useCampania("non-existent-id"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toBeNull()
    })

    it("is disabled when id is empty", () => {
        vi.mocked(campaniaApi.getById).mockResolvedValue(mockCampaniaFull)

        const { result } = renderHook(
            () => useCampania(""),
            { wrapper: createWrapper() }
        )

        expect(result.current.isFetching).toBe(false)
        expect(campaniaApi.getById).not.toHaveBeenCalled()
    })

    it("handles error state", async () => {
        vi.mocked(campaniaApi.getById).mockRejectedValue(new Error("Server error"))

        const { result } = renderHook(
            () => useCampania("some-id"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeTruthy()
    })
})

describe("useMisCampanias", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("fetches campanias by artista id", async () => {
        vi.mocked(campaniaApi.getByArtistaId).mockResolvedValue(mockCampaniasList)

        const { result } = renderHook(
            () => useMisCampanias("artista-001"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toHaveLength(3)
        expect(campaniaApi.getByArtistaId).toHaveBeenCalledWith("artista-001")
    })

    it("is disabled when artistaId is empty", () => {
        vi.mocked(campaniaApi.getByArtistaId).mockResolvedValue([])

        const { result } = renderHook(
            () => useMisCampanias(""),
            { wrapper: createWrapper() }
        )

        expect(result.current.isFetching).toBe(false)
        expect(campaniaApi.getByArtistaId).not.toHaveBeenCalled()
    })

    it("handles error state", async () => {
        vi.mocked(campaniaApi.getByArtistaId).mockRejectedValue(new Error("Unauthorized"))

        const { result } = renderHook(
            () => useMisCampanias("artista-001"),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeTruthy()
    })
})
