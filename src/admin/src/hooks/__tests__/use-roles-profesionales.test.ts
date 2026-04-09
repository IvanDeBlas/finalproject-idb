import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import type { ReactNode } from "react"
import { createElement } from "react"
import { useRolesProfesionales } from "../use-roles-profesionales"
import { maestrasService } from "@/services/maestras.service"
import type { RolProfesionalConCategoria } from "@shared/types"

vi.mock("@/services/maestras.service", () => ({
    maestrasService: {
        getRolesProfesionales: vi.fn(),
        getCategoriasRol: vi.fn(),
    },
}))

const mockRoles: RolProfesionalConCategoria[] = [
    {
        id: 1,
        nombre: "Productor Musical",
        descripcion: "Profesional de produccion",
        categoriaRol: { id: 1, nombre: "Produccion", orden: 1 },
        modalidadCobro: "Por proyecto",
        activo: true,
    },
    {
        id: 2,
        nombre: "Ingeniero de Grabacion",
        categoriaRol: { id: 2, nombre: "Ingenieria", orden: 2 },
        activo: true,
    },
]

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

describe("useRolesProfesionales", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns roles on success", async () => {
        vi.mocked(maestrasService.getRolesProfesionales).mockResolvedValue(mockRoles)

        const { result } = renderHook(() => useRolesProfesionales(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data).toHaveLength(2)
        expect(result.current.data?.[0].nombre).toBe("Productor Musical")
    })

    it("handles loading state", () => {
        vi.mocked(maestrasService.getRolesProfesionales).mockReturnValue(
            new Promise(() => {})
        )

        const { result } = renderHook(() => useRolesProfesionales(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("handles error state", async () => {
        vi.mocked(maestrasService.getRolesProfesionales).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(() => useRolesProfesionales(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})
