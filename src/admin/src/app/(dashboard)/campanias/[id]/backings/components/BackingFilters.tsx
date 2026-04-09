"use client"

import { useState, useCallback, useEffect } from "react"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Search } from "lucide-react"

export type BackingFilterType = "all" | "with-reward" | "anonymous"

interface BackingFiltersProps {
    onSearchChange: (query: string) => void
    onFilterChange: (filter: BackingFilterType) => void
    currentFilter: BackingFilterType
}

export function BackingFilters({
    onSearchChange,
    onFilterChange,
    currentFilter,
}: BackingFiltersProps) {
    const [searchQuery, setSearchQuery] = useState("")

    const debouncedSearch = useCallback(
        (value: string) => {
            const timer = setTimeout(() => {
                onSearchChange(value)
            }, 300)
            return () => clearTimeout(timer)
        },
        [onSearchChange]
    )

    useEffect(() => {
        const cleanup = debouncedSearch(searchQuery)
        return cleanup
    }, [searchQuery, debouncedSearch])

    return (
        <Card className="mb-6">
            <CardContent className="pt-6">
                <div className="flex flex-col md:flex-row gap-4">
                    <div className="flex-1">
                        <div className="relative">
                            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                            <Input
                                placeholder="Buscar por nombre..."
                                value={searchQuery}
                                onChange={(e) => setSearchQuery(e.target.value)}
                                className="pl-10 max-w-sm"
                                aria-label="Buscar backings por nombre de backer"
                            />
                        </div>
                    </div>

                    <div className="flex gap-2 flex-wrap">
                        <Button
                            variant={currentFilter === "all" ? "default" : "outline"}
                            onClick={() => onFilterChange("all")}
                            size="sm"
                        >
                            Todos
                        </Button>
                        <Button
                            variant={currentFilter === "with-reward" ? "default" : "outline"}
                            onClick={() => onFilterChange("with-reward")}
                            size="sm"
                        >
                            Con Reward
                        </Button>
                        <Button
                            variant={currentFilter === "anonymous" ? "default" : "outline"}
                            onClick={() => onFilterChange("anonymous")}
                            size="sm"
                        >
                            Anonimos
                        </Button>
                    </div>
                </div>
            </CardContent>
        </Card>
    )
}
