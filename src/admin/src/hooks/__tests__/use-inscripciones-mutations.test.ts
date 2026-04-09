import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import type { ReactNode } from "react"
import { createElement } from "react"
import {
    useAprobarInscripcion,
    useRechazarInscripcion,
    useBloquearInscripcion,
    useDarDeBajaInscripcion,
} from "../use-inscripciones-mutations"
import { inscripcionService } from "@/services/inscripcion.service"
import {
    mockAprobadaResult,
    mockRechazadaResult,
    mockBloqueadaResult,
    mockDadaDeBajaResult,
} from "@/__mocks__/inscripcion.mock"
import { toast } from "sonner"

vi.mock("@/services/inscripcion.service", () => ({
    inscripcionService: {
        getInscripciones: vi.fn(),
        aprobar: vi.fn(),
        rechazar: vi.fn(),
        bloquear: vi.fn(),
        darDeBaja: vi.fn(),
    },
}))

vi.mock("sonner", () => ({
    toast: Object.assign(vi.fn(), {
        success: vi.fn(),
        error: vi.fn(),
    }),
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

describe("useAprobarInscripcion", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls aprobar with programaId and inscripcionId", async () => {
        vi.mocked(inscripcionService.aprobar).mockResolvedValue(mockAprobadaResult)

        const { result } = renderHook(() => useAprobarInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-1",
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(inscripcionService.aprobar).toHaveBeenCalledWith({
            programaId: "programa-1",
            inscripcionId: "inscripcion-1",
        })
    })

    it("returns InscripcionAprobada on success", async () => {
        vi.mocked(inscripcionService.aprobar).mockResolvedValue(mockAprobadaResult)

        const { result } = renderHook(() => useAprobarInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-1",
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.codigoReferido).toBe("album-2026-x7k9m")
    })

    it("shows success toast on success", async () => {
        vi.mocked(inscripcionService.aprobar).mockResolvedValue(mockAprobadaResult)

        const { result } = renderHook(() => useAprobarInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-1",
                promotorNombre: "DJ Marketing Pro",
            })
        })

        await waitFor(() => {
            expect(toast.success).toHaveBeenCalled()
        })
    })

    it("shows error toast on failure", async () => {
        vi.mocked(inscripcionService.aprobar).mockRejectedValue(new Error("Failed"))

        const { result } = renderHook(() => useAprobarInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-1",
            })
        })

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalled()
        })
    })

    it("handles error from service", async () => {
        vi.mocked(inscripcionService.aprobar).mockRejectedValue(new Error("Fail"))

        const { result } = renderHook(() => useAprobarInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-1",
            })
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})

describe("useRechazarInscripcion", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls rechazar with programaId and inscripcionId", async () => {
        vi.mocked(inscripcionService.rechazar).mockResolvedValue(mockRechazadaResult)

        const { result } = renderHook(() => useRechazarInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-1",
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(inscripcionService.rechazar).toHaveBeenCalledWith({
            programaId: "programa-1",
            inscripcionId: "inscripcion-1",
        })
    })

    it("shows toast on success", async () => {
        vi.mocked(inscripcionService.rechazar).mockResolvedValue(mockRechazadaResult)

        const { result } = renderHook(() => useRechazarInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-1",
                promotorNombre: "Test",
            })
        })

        await waitFor(() => {
            expect(toast).toHaveBeenCalled()
        })
    })

    it("shows error toast on failure", async () => {
        vi.mocked(inscripcionService.rechazar).mockRejectedValue(new Error("Failed"))

        const { result } = renderHook(() => useRechazarInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-1",
            })
        })

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalled()
        })
    })
})

describe("useBloquearInscripcion", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls bloquear with programaId and inscripcionId", async () => {
        vi.mocked(inscripcionService.bloquear).mockResolvedValue(mockBloqueadaResult)

        const { result } = renderHook(() => useBloquearInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-1",
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(inscripcionService.bloquear).toHaveBeenCalledWith({
            programaId: "programa-1",
            inscripcionId: "inscripcion-1",
        })
    })

    it("returns InscripcionBloqueada with esBloqueado true", async () => {
        vi.mocked(inscripcionService.bloquear).mockResolvedValue(mockBloqueadaResult)

        const { result } = renderHook(() => useBloquearInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-1",
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.esBloqueado).toBe(true)
    })

    it("shows toast on success", async () => {
        vi.mocked(inscripcionService.bloquear).mockResolvedValue(mockBloqueadaResult)

        const { result } = renderHook(() => useBloquearInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-1",
                promotorNombre: "Spammer",
            })
        })

        await waitFor(() => {
            expect(toast).toHaveBeenCalled()
        })
    })

    it("shows error toast on failure", async () => {
        vi.mocked(inscripcionService.bloquear).mockRejectedValue(new Error("Failed"))

        const { result } = renderHook(() => useBloquearInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-1",
            })
        })

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalled()
        })
    })
})

describe("useDarDeBajaInscripcion", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls darDeBaja with programaId and inscripcionId", async () => {
        vi.mocked(inscripcionService.darDeBaja).mockResolvedValue(mockDadaDeBajaResult)

        const { result } = renderHook(() => useDarDeBajaInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-2",
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(inscripcionService.darDeBaja).toHaveBeenCalledWith({
            programaId: "programa-1",
            inscripcionId: "inscripcion-2",
        })
    })

    it("returns InscripcionDadaDeBaja with fechaBaja", async () => {
        vi.mocked(inscripcionService.darDeBaja).mockResolvedValue(mockDadaDeBajaResult)

        const { result } = renderHook(() => useDarDeBajaInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-2",
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.fechaBaja).toBeTruthy()
    })

    it("shows toast on success", async () => {
        vi.mocked(inscripcionService.darDeBaja).mockResolvedValue(mockDadaDeBajaResult)

        const { result } = renderHook(() => useDarDeBajaInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-2",
                promotorNombre: "Test",
            })
        })

        await waitFor(() => {
            expect(toast).toHaveBeenCalled()
        })
    })

    it("shows error toast on failure", async () => {
        vi.mocked(inscripcionService.darDeBaja).mockRejectedValue(new Error("Failed"))

        const { result } = renderHook(() => useDarDeBajaInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-2",
            })
        })

        await waitFor(() => {
            expect(toast.error).toHaveBeenCalled()
        })
    })

    it("handles error from service", async () => {
        vi.mocked(inscripcionService.darDeBaja).mockRejectedValue(new Error("Fail"))

        const { result } = renderHook(() => useDarDeBajaInscripcion(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                programaId: "programa-1",
                inscripcionId: "inscripcion-2",
            })
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})
