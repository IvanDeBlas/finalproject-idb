"use client"

import { Card } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { Search } from "lucide-react"

interface TemplateFiltersProps {
    searchQuery: string
    onSearchChange: (value: string) => void
    statusFilter: string
    onStatusChange: (value: string) => void
}

export function TemplateFilters({
    searchQuery,
    onSearchChange,
    statusFilter,
    onStatusChange,
}: TemplateFiltersProps) {
    return (
        <Card className="p-4 mb-4">
            <div className="flex flex-col md:flex-row gap-4">
                <div className="relative w-full md:w-64">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input
                        placeholder="Buscar templates..."
                        value={searchQuery}
                        onChange={(e) => onSearchChange(e.target.value)}
                        className="pl-9"
                        aria-label="Buscar templates por nombre"
                    />
                </div>
                <Select value={statusFilter} onValueChange={onStatusChange}>
                    <SelectTrigger className="w-full md:w-48">
                        <SelectValue placeholder="Estado" />
                    </SelectTrigger>
                    <SelectContent>
                        <SelectItem value="all">Todos</SelectItem>
                        <SelectItem value="active">Activos</SelectItem>
                        <SelectItem value="inactive">Inactivos</SelectItem>
                    </SelectContent>
                </Select>
            </div>
        </Card>
    )
}
