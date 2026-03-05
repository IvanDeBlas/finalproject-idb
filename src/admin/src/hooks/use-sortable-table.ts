import { useState, useMemo } from "react"

type SortDirection = "asc" | "desc"
type SortIconState = "asc" | "desc" | "none"

interface UseSortableTableReturn<T> {
    sortedData: T[]
    sortKey: keyof T
    sortDirection: SortDirection
    handleSort: (key: keyof T) => void
    getSortIcon: (key: keyof T) => SortIconState
}

export function useSortableTable<T>(
    data: T[],
    defaultSortKey: keyof T,
    defaultDirection: SortDirection = "desc"
): UseSortableTableReturn<T> {
    const [sortKey, setSortKey] = useState<keyof T>(defaultSortKey)
    const [sortDirection, setSortDirection] = useState<SortDirection>(defaultDirection)

    const sortedData = useMemo(() => {
        if (!data.length) return []

        return [...data].sort((a, b) => {
            const aVal = a[sortKey]
            const bVal = b[sortKey]

            // null/undefined always sort to end
            if (aVal == null && bVal == null) return 0
            if (aVal == null) return 1
            if (bVal == null) return -1

            let comparison = 0
            if (typeof aVal === "number" && typeof bVal === "number") {
                comparison = aVal - bVal
            } else if (typeof aVal === "string" && typeof bVal === "string") {
                comparison = aVal.localeCompare(bVal)
            }

            return sortDirection === "asc" ? comparison : -comparison
        })
    }, [data, sortKey, sortDirection])

    function handleSort(key: keyof T) {
        if (key === sortKey) {
            setSortDirection((prev) => (prev === "asc" ? "desc" : "asc"))
        } else {
            setSortKey(key)
            setSortDirection("desc")
        }
    }

    function getSortIcon(key: keyof T): SortIconState {
        if (key !== sortKey) return "none"
        return sortDirection
    }

    return { sortedData, sortKey, sortDirection, handleSort, getSortIcon }
}
