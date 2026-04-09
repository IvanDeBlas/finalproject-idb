import { describe, it, expect } from "vitest"
import { renderHook, act } from "@testing-library/react"
import { useSortableTable } from "../use-sortable-table"
import type { RankingPromotorItem } from "@shared/types/crowdpromotion"
import {
    mockRankingItems,
    buildRankingItem,
} from "@/__mocks__/cp-tracking-metricas.mock"

describe("useSortableTable", () => {
    it("returns data sorted by defaultSortKey DESC", () => {
        const { result } = renderHook(() =>
            useSortableTable<RankingPromotorItem>(mockRankingItems, "conversiones", "desc")
        )

        expect(result.current.sortedData[0].conversiones).toBeGreaterThanOrEqual(
            result.current.sortedData[1].conversiones
        )
    })

    it("returns data sorted ASC when defaultDirection is asc", () => {
        const { result } = renderHook(() =>
            useSortableTable<RankingPromotorItem>(mockRankingItems, "conversiones", "asc")
        )

        expect(result.current.sortedData[0].conversiones).toBeLessThanOrEqual(
            result.current.sortedData[1].conversiones
        )
    })

    it("handleSort toggles direction on same key", () => {
        const { result } = renderHook(() =>
            useSortableTable<RankingPromotorItem>(mockRankingItems, "conversiones", "desc")
        )

        act(() => result.current.handleSort("conversiones"))

        expect(result.current.sortDirection).toBe("asc")
    })

    it("handleSort resets to desc on new key", () => {
        const { result } = renderHook(() =>
            useSortableTable<RankingPromotorItem>(mockRankingItems, "conversiones", "desc")
        )

        act(() => result.current.handleSort("clicks"))

        expect(result.current.sortKey).toBe("clicks")
        expect(result.current.sortDirection).toBe("desc")
    })

    it("getSortIcon returns asc for active column ascending", () => {
        const { result } = renderHook(() =>
            useSortableTable<RankingPromotorItem>(mockRankingItems, "clicks", "asc")
        )

        expect(result.current.getSortIcon("clicks")).toBe("asc")
    })

    it("getSortIcon returns desc for active column descending", () => {
        const { result } = renderHook(() =>
            useSortableTable<RankingPromotorItem>(mockRankingItems, "conversiones", "desc")
        )

        expect(result.current.getSortIcon("conversiones")).toBe("desc")
    })

    it("getSortIcon returns none for inactive column", () => {
        const { result } = renderHook(() =>
            useSortableTable<RankingPromotorItem>(mockRankingItems, "conversiones", "desc")
        )

        expect(result.current.getSortIcon("promotorNombre")).toBe("none")
    })

    it("null/undefined values sort to end regardless of direction", () => {
        const itemWithNull = buildRankingItem({
            promotorId: "null-item",
            valorGenerado: null as unknown as number,
        })
        const dataWithNull = [...mockRankingItems, itemWithNull]

        const { result: descResult } = renderHook(() =>
            useSortableTable<RankingPromotorItem>(dataWithNull, "valorGenerado", "desc")
        )
        const lastDesc = descResult.current.sortedData[descResult.current.sortedData.length - 1]
        expect(lastDesc.promotorId).toBe("null-item")

        const { result: ascResult } = renderHook(() =>
            useSortableTable<RankingPromotorItem>(dataWithNull, "valorGenerado", "asc")
        )
        const lastAsc = ascResult.current.sortedData[ascResult.current.sortedData.length - 1]
        expect(lastAsc.promotorId).toBe("null-item")
    })

    it("returns empty array for empty input", () => {
        const { result } = renderHook(() =>
            useSortableTable<RankingPromotorItem>([], "conversiones", "desc")
        )

        expect(result.current.sortedData).toEqual([])
    })

    it("re-sorts when data changes", () => {
        const { result, rerender } = renderHook(
            ({ data }) =>
                useSortableTable<RankingPromotorItem>(data, "conversiones", "desc"),
            { initialProps: { data: mockRankingItems } }
        )

        const newItem = buildRankingItem({
            promotorId: "new-item",
            conversiones: 100,
        })

        rerender({ data: [...mockRankingItems, newItem] })

        expect(result.current.sortedData[0].promotorId).toBe("new-item")
    })
})
