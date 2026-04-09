import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { createElement } from "react"
import { useAcuerdo } from "../../application/hooks/useAcuerdo"
import { mockAcuerdoActivo } from "../../__mocks__/acuerdo.mock"

vi.mock("../../infrastructure", () => ({
    acuerdoApi: {
        getById: vi.fn(),
    },
}))

import { acuerdoApi } from "../../infrastructure"

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

describe("useAcuerdo", () => {
    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("fetches acuerdo data successfully", async () => {
        vi.mocked(acuerdoApi.getById).mockResolvedValue(mockAcuerdoActivo)

        const { result } = renderHook(
            () => useAcuerdo(mockAcuerdoActivo.id),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(result.current.data).toEqual(mockAcuerdoActivo)
    })

    it("isLoading is true initially", () => {
        vi.mocked(acuerdoApi.getById).mockReturnValue(new Promise(() => {}))

        const { result } = renderHook(
            () => useAcuerdo(mockAcuerdoActivo.id),
            { wrapper: createWrapper() }
        )

        expect(result.current.isLoading).toBe(true)
    })

    it("handles error state", async () => {
        vi.mocked(acuerdoApi.getById).mockRejectedValue(
            new Error("Network error")
        )

        const { result } = renderHook(
            () => useAcuerdo(mockAcuerdoActivo.id),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isError).toBe(true))

        expect(result.current.error).toBeTruthy()
    })

    it("calls acuerdoApi.getById with correct id", async () => {
        vi.mocked(acuerdoApi.getById).mockResolvedValue(mockAcuerdoActivo)

        const testId = "test-acuerdo-id-123"
        const { result } = renderHook(
            () => useAcuerdo(testId),
            { wrapper: createWrapper() }
        )

        await waitFor(() => expect(result.current.isSuccess).toBe(true))

        expect(vi.mocked(acuerdoApi.getById)).toHaveBeenCalledWith(testId)
    })
})
