import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import type { ReactNode } from "react"
import { createElement } from "react"
import { useMisNecesidades, useNecesidad } from "../use-necesidades"
import { necesidadService } from "@/services/necesidad.service"
import type { NecesidadCrowdsourcingList, NecesidadCrowdsourcing, PaginatedResponse } from "@shared/types"

vi.mock("@/services/necesidad.service", () => ({
    necesidadService: {
        getMisNecesidades: vi.fn(),
        getById: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        cerrar: vi.fn(),
    },
}))

const mockPaginatedData: PaginatedResponse<NecesidadCrowdsourcingList> = {
    items: [
        {
            id: "n-1",
            titulo: "Necesidad 1",
            estadoNecesidadId: 1,
            estadoNecesidadNombre: "Abierta",
            tipoNecesidadId: 3,
            tipoNecesidadNombre: "Ingenieria de Audio",
            presupuestoMin: 150,
            presupuestoMax: 800,
            monedaId: 1,
            monedaNombre: "EUR",
            modalidadTrabajoId: 2,
            modalidadTrabajoNombre: "Remoto",
            numeroPropuestas: 3,
            fechaCreacion: "2026-02-16T10:00:00Z",
            fechaLimitePropuestas: null,
            fechaActualizacion: null,
        },
    ],
    totalCount: 1,
    page: 1,
    pageSize: 12,
    totalPages: 1,
}

const mockNecesidadDetail: NecesidadCrowdsourcing = {
    id: "n-1",
    titulo: "Necesidad 1",
    estadoNecesidadId: 1,
    estadoNecesidadNombre: "Abierta",
    tipoNecesidadId: 3,
    tipoNecesidadNombre: "Ingenieria de Audio",
    presupuestoMin: 150,
    presupuestoMax: 800,
    monedaId: 1,
    monedaNombre: "EUR",
    modalidadTrabajoId: 2,
    modalidadTrabajoNombre: "Remoto",
    numeroPropuestas: 3,
    fechaCreacion: "2026-02-16T10:00:00Z",
    fechaLimitePropuestas: null,
    fechaActualizacion: null,
    descripcion: "Descripcion test",
    ubicacionCiudad: null,
    ubicacionPais: null,
    fechaInicioPrevista: null,
    proyectoArtisticoId: "proj-1",
    proyectoArtisticoNombre: "Mi Proyecto",
    propuestas: [],
}

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false },
            mutations: { retry: false },
        },
    })
    return function Wrapper({ children }: { children: ReactNode }) {
        return createElement(
            QueryClientProvider,
            { client: queryClient },
            children
        )
    }
}

describe("useMisNecesidades", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns paginated data on success", async () => {
        vi.mocked(necesidadService.getMisNecesidades).mockResolvedValue(mockPaginatedData)

        const { result } = renderHook(() => useMisNecesidades(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.items).toHaveLength(1)
        expect(result.current.data?.totalCount).toBe(1)
    })

    it("handles loading state", () => {
        vi.mocked(necesidadService.getMisNecesidades).mockReturnValue(
            new Promise(() => {})
        )

        const { result } = renderHook(() => useMisNecesidades(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("handles error state", async () => {
        vi.mocked(necesidadService.getMisNecesidades).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(() => useMisNecesidades(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })

        expect(result.current.error).toBeDefined()
    })

    it("passes filter params to service", async () => {
        vi.mocked(necesidadService.getMisNecesidades).mockResolvedValue(mockPaginatedData)

        renderHook(
            () => useMisNecesidades({ page: 2, pageSize: 10, estado: 1, search: "mezcla" }),
            { wrapper: createWrapper() }
        )

        await waitFor(() => {
            expect(necesidadService.getMisNecesidades).toHaveBeenCalledWith({
                page: 2,
                pageSize: 10,
                estado: 1,
                search: "mezcla",
            })
        })
    })
})

describe("useNecesidad", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns necesidad detail on success", async () => {
        vi.mocked(necesidadService.getById).mockResolvedValue(mockNecesidadDetail)

        const { result } = renderHook(() => useNecesidad("n-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.id).toBe("n-1")
        expect(result.current.data?.titulo).toBe("Necesidad 1")
    })

    it("does not fetch when id is empty", () => {
        renderHook(() => useNecesidad(""), {
            wrapper: createWrapper(),
        })

        expect(necesidadService.getById).not.toHaveBeenCalled()
    })

    it("handles null response (not found)", async () => {
        vi.mocked(necesidadService.getById).mockResolvedValue(null)

        const { result } = renderHook(() => useNecesidad("nonexistent"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data).toBeNull()
    })
})
