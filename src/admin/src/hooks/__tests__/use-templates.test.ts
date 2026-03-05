import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import type { ReactNode } from "react"
import { createElement } from "react"
import { useTemplates, useTemplate } from "../use-templates"
import { templateService } from "@/services/template.service"
import type { PlantillaProyectoList, PlantillaProyecto } from "@shared/types"

vi.mock("@/services/template.service", () => ({
    templateService: {
        getAll: vi.fn(),
        getById: vi.fn(),
        create: vi.fn(),
        update: vi.fn(),
        delete: vi.fn(),
        toggleStatus: vi.fn(),
    },
}))

const mockTemplateList: PlantillaProyectoList[] = [
    {
        id: "t-1",
        nombre: "Produccion de EP",
        descripcion: "Plantilla para EP",
        icono: "music",
        orden: 1,
        precioMinTotal: 3700,
        precioMaxTotal: 11400,
        moneda: 1,
        cantidadNecesidades: 8,
        fases: ["Preproduccion", "Grabacion"],
    },
    {
        id: "t-2",
        nombre: "Album Completo",
        descripcion: "Plantilla para album",
        icono: "disc",
        orden: 2,
        precioMinTotal: 8000,
        precioMaxTotal: 25000,
        moneda: 1,
        cantidadNecesidades: 12,
        fases: ["Preproduccion", "Grabacion", "Mezcla y Master"],
    },
]

const mockTemplateDetail: PlantillaProyecto = {
    id: "t-1",
    nombre: "Produccion de EP",
    descripcion: "Plantilla para EP",
    icono: "music",
    orden: 1,
    necesidades: [],
    resumen: {
        precioMinTotal: 3700,
        precioMaxTotal: 11400,
        moneda: 1,
        cantidadNecesidadesAlta: 4,
        cantidadNecesidadesMedia: 2,
        cantidadNecesidadesBaja: 2,
    },
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

describe("useTemplates", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns data on success", async () => {
        vi.mocked(templateService.getAll).mockResolvedValue(mockTemplateList)

        const { result } = renderHook(() => useTemplates(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data).toHaveLength(2)
        expect(result.current.data?.[0].nombre).toBe("Produccion de EP")
    })

    it("handles loading state", () => {
        vi.mocked(templateService.getAll).mockReturnValue(
            new Promise(() => {})
        )

        const { result } = renderHook(() => useTemplates(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("handles error state", async () => {
        vi.mocked(templateService.getAll).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(() => useTemplates(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })

        expect(result.current.error).toBeDefined()
    })
})

describe("useTemplate", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns template detail on success", async () => {
        vi.mocked(templateService.getById).mockResolvedValue(mockTemplateDetail)

        const { result } = renderHook(() => useTemplate("t-1"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.nombre).toBe("Produccion de EP")
        expect(result.current.data?.resumen.precioMinTotal).toBe(3700)
    })

    it("does not fetch when id is empty", () => {
        renderHook(() => useTemplate(""), {
            wrapper: createWrapper(),
        })

        expect(templateService.getById).not.toHaveBeenCalled()
    })

    it("handles not found (null)", async () => {
        vi.mocked(templateService.getById).mockResolvedValue(null)

        const { result } = renderHook(() => useTemplate("nonexistent"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data).toBeNull()
    })
})
