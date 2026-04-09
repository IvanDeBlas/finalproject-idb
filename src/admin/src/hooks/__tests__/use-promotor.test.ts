import { renderHook, waitFor, act } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import type { ReactNode } from "react"
import { createElement } from "react"
import { usePromotor } from "../use-promotor"
import { useUpdatePromotor, useDesactivarPromotor } from "../use-promotor-mutations"
import { promotorService } from "@/services/promotor.service"
import {
    mockPromotor,
    mockPromotorUpdatedResult,
    mockDesactivadoConProgramas,
} from "@/__mocks__/promotor.mock"

vi.mock("@/services/promotor.service", () => ({
    promotorService: {
        getMe: vi.fn(),
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

describe("usePromotor", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("returns promotor data on success", async () => {
        vi.mocked(promotorService.getMe).mockResolvedValue(mockPromotor)

        const { result } = renderHook(() => usePromotor(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.nombrePublico).toBe("DJ Promo Star")
    })

    it("returns null when no promotor profile (404)", async () => {
        vi.mocked(promotorService.getMe).mockResolvedValue(null)

        const { result } = renderHook(() => usePromotor(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data).toBeNull()
    })

    it("handles loading state", () => {
        vi.mocked(promotorService.getMe).mockReturnValue(new Promise(() => {}))

        const { result } = renderHook(() => usePromotor(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("handles error state", async () => {
        vi.mocked(promotorService.getMe).mockRejectedValue(new Error("Fail"))

        const { result } = renderHook(() => usePromotor(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })

    it("does not retry on error", async () => {
        vi.mocked(promotorService.getMe).mockRejectedValue(new Error("Fail"))

        const { result } = renderHook(() => usePromotor(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })

        expect(promotorService.getMe).toHaveBeenCalledTimes(1)
    })
})

describe("useUpdatePromotor", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls service with mapped request data", async () => {
        vi.mocked(promotorService.update).mockResolvedValue(mockPromotorUpdatedResult)

        const { result } = renderHook(() => useUpdatePromotor(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                nombrePublico: "New Name",
                emailContacto: "",
                urlSitioWeb: "",
                urlInstagram: "",
                urlTikTok: "",
                urlYouTube: "",
                urlTwitter: "",
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(promotorService.update).toHaveBeenCalledWith({
            nombrePublico: "New Name",
            emailContacto: undefined,
            urlSitioWeb: undefined,
            urlInstagram: undefined,
            urlTikTok: undefined,
            urlYouTube: undefined,
            urlTwitter: undefined,
        })
    })

    it("invalidates promotor query on success", async () => {
        vi.mocked(promotorService.update).mockResolvedValue(mockPromotorUpdatedResult)

        const { result } = renderHook(() => useUpdatePromotor(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                nombrePublico: "Test",
                emailContacto: "",
                urlSitioWeb: "",
                urlInstagram: "",
                urlTikTok: "",
                urlYouTube: "",
                urlTwitter: "",
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })
    })

    it("handles error from service", async () => {
        vi.mocked(promotorService.update).mockRejectedValue(new Error("Update failed"))

        const { result } = renderHook(() => useUpdatePromotor(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                nombrePublico: "Test",
                emailContacto: "",
                urlSitioWeb: "",
                urlInstagram: "",
                urlTikTok: "",
                urlYouTube: "",
                urlTwitter: "",
            })
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })

    it("does not send empty strings as URL values", async () => {
        vi.mocked(promotorService.update).mockResolvedValue(mockPromotorUpdatedResult)

        const { result } = renderHook(() => useUpdatePromotor(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate({
                nombrePublico: "Test",
                emailContacto: "",
                urlSitioWeb: "",
                urlInstagram: "https://instagram.com/test",
                urlTikTok: "",
                urlYouTube: "",
                urlTwitter: "",
            })
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        const calledWith = vi.mocked(promotorService.update).mock.calls[0][0]
        expect(calledWith.urlSitioWeb).toBeUndefined()
        expect(calledWith.urlInstagram).toBe("https://instagram.com/test")
    })
})

describe("useDesactivarPromotor", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("calls desactivar service", async () => {
        vi.mocked(promotorService.desactivar).mockResolvedValue(mockDesactivadoConProgramas)

        const { result } = renderHook(() => useDesactivarPromotor(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate()
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(promotorService.desactivar).toHaveBeenCalledTimes(1)
    })

    it("invalidates query on success", async () => {
        vi.mocked(promotorService.desactivar).mockResolvedValue(mockDesactivadoConProgramas)

        const { result } = renderHook(() => useDesactivarPromotor(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate()
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })
    })

    it("returns programasDadosDeBaja on success", async () => {
        vi.mocked(promotorService.desactivar).mockResolvedValue(mockDesactivadoConProgramas)

        const { result } = renderHook(() => useDesactivarPromotor(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate()
        })

        await waitFor(() => {
            expect(result.current.isSuccess).toBe(true)
        })

        expect(result.current.data?.programasDadosDeBaja).toBe(3)
    })

    it("handles error from service", async () => {
        vi.mocked(promotorService.desactivar).mockRejectedValue(
            new Error("Already inactive")
        )

        const { result } = renderHook(() => useDesactivarPromotor(), {
            wrapper: createWrapper(),
        })

        await act(async () => {
            result.current.mutate()
        })

        await waitFor(() => {
            expect(result.current.isError).toBe(true)
        })
    })
})
