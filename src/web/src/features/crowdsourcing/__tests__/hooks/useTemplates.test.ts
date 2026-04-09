import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { useTemplates } from "../../application/hooks/useTemplates"
import { mockTemplatesList } from "../../__mocks__/crowdsourcing.mock"

vi.mock("../../infrastructure", () => ({
    crowdsourcingApi: {
        getTemplates: vi.fn(),
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

describe("useTemplates", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("fetches templates successfully", async () => {
        vi.mocked(crowdsourcingApi.getTemplates).mockResolvedValue(mockTemplatesList)

        const { result } = renderHook(() => useTemplates(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toHaveLength(3)
        expect(result.current.data?.[0].nombre).toBe("Produccion de EP")
    })

    it("sets isLoading to true initially", () => {
        vi.mocked(crowdsourcingApi.getTemplates).mockReturnValue(new Promise(() => {}))

        const { result } = renderHook(() => useTemplates(), {
            wrapper: createWrapper(),
        })

        expect(result.current.isLoading).toBe(true)
    })

    it("handles error state", async () => {
        vi.mocked(crowdsourcingApi.getTemplates).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(() => useTemplates(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeTruthy()
    })

    it("returns empty array when API returns empty", async () => {
        vi.mocked(crowdsourcingApi.getTemplates).mockResolvedValue([])

        const { result } = renderHook(() => useTemplates(), {
            wrapper: createWrapper(),
        })

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toHaveLength(0)
    })
})
