import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { useTemplateDetail } from "../../application/hooks/useTemplateDetail"
import { mockTemplateDetail } from "../../__mocks__/crowdsourcing.mock"

vi.mock("../../infrastructure", () => ({
    crowdsourcingApi: {
        getTemplateById: vi.fn(),
    },
}))

import { crowdsourcingApi } from "../../infrastructure"

function createWrapper() {
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: { retry: false, gcTime: 0 },
        },
    })
    return function Wrapper({ children }: { children: React.ReactNode }) {
        return createElement(QueryClientProvider, { client: queryClient }, children)
    }
}

describe("useTemplateDetail", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("fetches template by id successfully", async () => {
        vi.mocked(crowdsourcingApi.getTemplateById).mockResolvedValue(
            mockTemplateDetail
        )

        const { result } = renderHook(() => useTemplateDetail("tpl-001"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.nombre).toBe("Produccion de EP")
        expect(result.current.data?.necesidades).toHaveLength(3)
    })

    it("includes necesidades in response", async () => {
        vi.mocked(crowdsourcingApi.getTemplateById).mockResolvedValue(
            mockTemplateDetail
        )

        const { result } = renderHook(() => useTemplateDetail("tpl-001"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data?.necesidades[0].titulo).toBe(
            "Productor Musical"
        )
        expect(result.current.data?.resumen).toBeDefined()
    })

    it("does not fetch when id is empty", () => {
        const { result } = renderHook(() => useTemplateDetail(""), {
            wrapper: createWrapper(),
        })

        expect(result.current.fetchStatus).toBe("idle")
        expect(crowdsourcingApi.getTemplateById).not.toHaveBeenCalled()
    })

    it("handles error state", async () => {
        vi.mocked(crowdsourcingApi.getTemplateById).mockRejectedValue(
            new Error("Template not found")
        )

        const { result } = renderHook(() => useTemplateDetail("invalid-id"), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeTruthy()
    })

    it("sets isLoading to true initially", () => {
        vi.mocked(crowdsourcingApi.getTemplateById).mockReturnValue(
            new Promise(() => {})
        )

        const { result } = renderHook(() => useTemplateDetail("tpl-001"), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })
})
