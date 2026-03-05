import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import type { ReactNode } from "react"
import { createElement } from "react"
import {
    useCreatePromoPrograma,
    useUpdatePromoPrograma,
    useDesactivarPromoPrograma,
} from "../use-promo-programas-mutations"
import { promoProgramaService } from "@/services/promo-programa.service"
import {
    mockCreatedResult,
    mockUpdatedResult,
    mockDesactivadoResult,
} from "@/__mocks__/promo-programa.mock"

vi.mock("@/services/promo-programa.service", () => ({
    promoProgramaService: {
        getMisProgramas: vi.fn(),
        getById: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        desactivar: vi.fn(),
    },
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

describe("useCreatePromoPrograma", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls create service with data", async () => {
        vi.mocked(promoProgramaService.create).mockResolvedValue(mockCreatedResult)

        const { result } = renderHook(() => useCreatePromoPrograma(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                titulo: "Nuevo",
                tipoPromoId: 1,
                monedaId: 1,
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(promoProgramaService.create).toHaveBeenCalledWith({
            titulo: "Nuevo",
            tipoPromoId: 1,
            monedaId: 1,
        })
    })

    it("returns created result on success", async () => {
        vi.mocked(promoProgramaService.create).mockResolvedValue(mockCreatedResult)

        const { result } = renderHook(() => useCreatePromoPrograma(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({ titulo: "Test", tipoPromoId: 1, monedaId: 1 })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.id).toBe("programa-new")
    })

    it("handles error from service", async () => {
        vi.mocked(promoProgramaService.create).mockRejectedValue(new Error("Create failed"))

        const { result } = renderHook(() => useCreatePromoPrograma(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({ titulo: "Test", tipoPromoId: 1, monedaId: 1 })
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})

describe("useUpdatePromoPrograma", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls update service with id and data", async () => {
        vi.mocked(promoProgramaService.update).mockResolvedValue(mockUpdatedResult)

        const { result } = renderHook(() => useUpdatePromoPrograma(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                id: "programa-1",
                data: { titulo: "Updated", tipoPromoId: 1, monedaId: 1 },
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(promoProgramaService.update).toHaveBeenCalledWith("programa-1", {
            titulo: "Updated",
            tipoPromoId: 1,
            monedaId: 1,
        })
    })

    it("handles error from service", async () => {
        vi.mocked(promoProgramaService.update).mockRejectedValue(new Error("Update failed"))

        const { result } = renderHook(() => useUpdatePromoPrograma(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                id: "programa-1",
                data: { titulo: "Test", tipoPromoId: 1, monedaId: 1 },
            })
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})

describe("useDesactivarPromoPrograma", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls desactivar service with id", async () => {
        vi.mocked(promoProgramaService.desactivar).mockResolvedValue(mockDesactivadoResult)

        const { result } = renderHook(() => useDesactivarPromoPrograma(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate("programa-1")
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(promoProgramaService.desactivar).toHaveBeenCalledWith("programa-1")
    })

    it("returns desactivado result", async () => {
        vi.mocked(promoProgramaService.desactivar).mockResolvedValue(mockDesactivadoResult)

        const { result } = renderHook(() => useDesactivarPromoPrograma(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate("programa-1")
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.tareasDesactivadas).toBe(2)
    })

    it("handles error from service", async () => {
        vi.mocked(promoProgramaService.desactivar).mockRejectedValue(
            new Error("Already inactive")
        )

        const { result } = renderHook(() => useDesactivarPromoPrograma(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate("programa-1")
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})
