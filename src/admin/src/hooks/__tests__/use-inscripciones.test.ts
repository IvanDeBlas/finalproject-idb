import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import type { ReactNode } from "react"
import { createElement } from "react"
import {
    useInscripcionesPendientes,
    useInscripcionesAprobadas,
    useInscripcionesBloqueadas,
} from "../use-inscripciones"
import { inscripcionService } from "@/services/inscripcion.service"
import {
    mockInscripcionesPendientesResponse,
    mockInscripcionesAprobadasResponse,
    mockInscripcionesBloqueadasResponse,
} from "@/__mocks__/inscripcion.mock"

vi.mock("@/services/inscripcion.service", () => ({
    inscripcionService: {
        getInscripciones: vi.fn(),
        aprobar: vi.fn(),
        rechazar: vi.fn(),
        bloquear: vi.fn(),
        darDeBaja: vi.fn(),
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

describe("useInscripcionesPendientes", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns paginated data on success", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesPendientesResponse
        )

        const { result } = renderHook(() => useInscripcionesPendientes("programa-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.items).toHaveLength(1)
        expect(result.current.data?.items[0].estado).toBe("Pendiente")
    })

    it("passes programaId and estado filter to service", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesPendientesResponse
        )

        renderHook(() => useInscripcionesPendientes("programa-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(inscripcionService.getInscripciones).toHaveBeenCalledWith(
                "programa-1",
                { estado: "Pendiente" }
            )
        })
    })

    it("handles loading state", () => {
        vi.mocked(inscripcionService.getInscripciones).mockReturnValue(new Promise(() => {}))

        const { result } = renderHook(() => useInscripcionesPendientes("programa-1"), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("handles error state", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockRejectedValue(new Error("Fail"))

        const { result } = renderHook(() => useInscripcionesPendientes("programa-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })

    it("does not fetch when programaId is empty", () => {
        const { result } = renderHook(() => useInscripcionesPendientes(""), {
            wrapper: createWrapper(),
        })

        expect(result.current.fetchStatus).toBe("idle")
        expect(inscripcionService.getInscripciones).not.toHaveBeenCalled()
    })

    it("does not retry on error", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockRejectedValue(new Error("Fail"))

        const { result } = renderHook(() => useInscripcionesPendientes("programa-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })

        expect(inscripcionService.getInscripciones).toHaveBeenCalledTimes(1)
    })
})

describe("useInscripcionesAprobadas", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns aprobadas on success", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesAprobadasResponse
        )

        const { result } = renderHook(() => useInscripcionesAprobadas("programa-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.items[0].estado).toBe("Aprobado")
    })

    it("passes estado Aprobado filter", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesAprobadasResponse
        )

        renderHook(() => useInscripcionesAprobadas("programa-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(inscripcionService.getInscripciones).toHaveBeenCalledWith(
                "programa-1",
                { estado: "Aprobado" }
            )
        })
    })
})

describe("useInscripcionesBloqueadas", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns bloqueadas on success", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesBloqueadasResponse
        )

        const { result } = renderHook(() => useInscripcionesBloqueadas("programa-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.items[0].estado).toBe("Bloqueado")
    })

    it("passes estado Bloqueado filter", async () => {
        vi.mocked(inscripcionService.getInscripciones).mockResolvedValue(
            mockInscripcionesBloqueadasResponse
        )

        renderHook(() => useInscripcionesBloqueadas("programa-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(inscripcionService.getInscripciones).toHaveBeenCalledWith(
                "programa-1",
                { estado: "Bloqueado" }
            )
        })
    })
})
