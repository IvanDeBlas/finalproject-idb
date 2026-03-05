import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import type { ReactNode } from "react"
import { createElement } from "react"
import {
    useCreateNecesidad,
    useUpdateNecesidad,
    useCerrarNecesidad,
} from "../use-necesidades-mutations"
import { necesidadService } from "@/services/necesidad.service"

vi.mock("@/services/necesidad.service", () => ({
    necesidadService: {
        getMisNecesidades: vi.fn(),
        getById: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        cerrar: vi.fn(),
    },
}))

vi.mock("next/navigation", () => ({
    useRouter: () => ({
        push: vi.fn(),
        back: vi.fn(),
        replace: vi.fn(),
        refresh: vi.fn(),
    }),
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
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
        return createElement(
            QueryClientProvider,
            { client: queryClient },
            children
        )
    }
}

describe("useCreateNecesidad", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("creates necesidad successfully", async () => {
        vi.mocked(necesidadService.create).mockResolvedValue({
            id: "new-id",
            titulo: "New",
            estadoNecesidadId: 1,
            estadoNecesidadNombre: "Abierta",
            fechaCreacion: "2026-02-16T10:00:00Z",
        })

        const { result } = renderHook(() => useCreateNecesidad(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                titulo: "New necesidad",
                tipoNecesidadId: 3,
                modalidadTrabajoId: 2,
                proyectoArtisticoId: "proj-1",
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })
    })

    it("handles error on create", async () => {
        vi.mocked(necesidadService.create).mockRejectedValue(
            new Error("Validation error")
        )

        const { result } = renderHook(() => useCreateNecesidad(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                titulo: "Bad",
                tipoNecesidadId: 3,
                modalidadTrabajoId: 2,
                proyectoArtisticoId: "proj-1",
            })
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})

describe("useUpdateNecesidad", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("updates necesidad successfully", async () => {
        vi.mocked(necesidadService.update).mockResolvedValue({
            id: "n-1",
            titulo: "Updated",
            estadoNecesidadId: 1,
            estadoNecesidadNombre: "Abierta",
            fechaActualizacion: "2026-02-16T12:00:00Z",
        })

        const { result } = renderHook(() => useUpdateNecesidad("n-1"), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                titulo: "Updated",
                modalidadTrabajoId: 2,
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })
    })

    it("handles error on update", async () => {
        vi.mocked(necesidadService.update).mockRejectedValue(
            new Error("Solo Abierta")
        )

        const { result } = renderHook(() => useUpdateNecesidad("n-closed"), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                titulo: "Updated",
                modalidadTrabajoId: 2,
            })
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})

describe("useCerrarNecesidad", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("closes necesidad successfully", async () => {
        vi.mocked(necesidadService.cerrar).mockResolvedValue({
            propuestasRechazadas: 2,
        })

        const { result } = renderHook(() => useCerrarNecesidad("n-1"), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({ motivo: "Ya no necesito" })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.propuestasRechazadas).toBe(2)
    })

    it("handles error on cerrar", async () => {
        vi.mocked(necesidadService.cerrar).mockRejectedValue(
            new Error("Solo Abierta o En Progreso")
        )

        const { result } = renderHook(() => useCerrarNecesidad("n-closed"), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({})
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})
