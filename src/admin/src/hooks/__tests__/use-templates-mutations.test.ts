import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import type { ReactNode } from "react"
import { createElement } from "react"
import {
    useCreateTemplate,
    useUpdateTemplate,
    useDeleteTemplate,
    useToggleTemplateStatus,
} from "../use-templates-mutations"
import { templateService } from "@/services/template.service"
import type { CreateTemplateRequest } from "@/services/template.service"

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

const mockCreateData: CreateTemplateRequest = {
    nombre: "Test Template",
    descripcion: "Test desc",
    icono: "music",
    orden: 1,
    activo: true,
    necesidades: [],
}

describe("useCreateTemplate", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls create service on mutate", async () => {
        vi.mocked(templateService.create).mockResolvedValue("new-id")

        const { result } = renderHook(() => useCreateTemplate(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate(mockCreateData)
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(templateService.create).toHaveBeenCalledWith(mockCreateData)
    })

    it("handles create error", async () => {
        vi.mocked(templateService.create).mockRejectedValue(
            new Error("Create failed")
        )

        const { result } = renderHook(() => useCreateTemplate(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate(mockCreateData)
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})

describe("useUpdateTemplate", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls update service on mutate", async () => {
        vi.mocked(templateService.update).mockResolvedValue()

        const { result } = renderHook(() => useUpdateTemplate(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({ id: "t-1", data: mockCreateData })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(templateService.update).toHaveBeenCalledWith("t-1", mockCreateData)
    })
})

describe("useDeleteTemplate", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls delete service on mutate", async () => {
        vi.mocked(templateService.delete).mockResolvedValue()

        const { result } = renderHook(() => useDeleteTemplate(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate("t-1")
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(templateService.delete).toHaveBeenCalledWith("t-1")
    })
})

describe("useToggleTemplateStatus", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls toggleStatus service on mutate", async () => {
        vi.mocked(templateService.toggleStatus).mockResolvedValue()

        const { result } = renderHook(() => useToggleTemplateStatus(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate("t-1")
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(templateService.toggleStatus).toHaveBeenCalledWith("t-1")
    })

    it("handles toggle error", async () => {
        vi.mocked(templateService.toggleStatus).mockRejectedValue(
            new Error("Toggle failed")
        )

        const { result } = renderHook(() => useToggleTemplateStatus(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate("t-1")
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})
