"use client"

import { Card } from "@/components/ui/card"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { Input } from "@/components/ui/input"
import { Search } from "lucide-react"
import { ESTADO_NECESIDAD } from "@shared/constants"

interface NecesidadFiltersProps {
    estadoFilter: string
    onEstadoChange: (estado: string) => void
    searchQuery: string
    onSearchChange: (query: string) => void
}

export function NecesidadFilters({
    estadoFilter,
    onEstadoChange,
    searchQuery,
    onSearchChange,
}: NecesidadFiltersProps) {
    return (
        <Card className="p-4">
            <div className="flex flex-col sm:flex-row gap-4">
                <Select value={estadoFilter} onValueChange={onEstadoChange}>
                    <SelectTrigger className="w-full sm:w-48">
                        <SelectValue placeholder="Estado" />
                    </SelectTrigger>
                    <SelectContent>
                        <SelectItem value="all">Todos</SelectItem>
                        <SelectItem value={ESTADO_NECESIDAD.ABIERTA.toString()}>
                            Abierta
                        </SelectItem>
                        <SelectItem value={ESTADO_NECESIDAD.EN_PROGRESO.toString()}>
                            En Progreso
                        </SelectItem>
                        <SelectItem value={ESTADO_NECESIDAD.CERRADA.toString()}>
                            Cerrada
                        </SelectItem>
                        <SelectItem value={ESTADO_NECESIDAD.CANCELADA.toString()}>
                            Cancelada
                        </SelectItem>
                    </SelectContent>
                </Select>

                <div className="relative flex-1">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input
                        placeholder="Buscar por titulo o descripcion..."
                        value={searchQuery}
                        onChange={(e) => onSearchChange(e.target.value)}
                        className="pl-10"
                    />
                </div>
            </div>
        </Card>
    )
}
