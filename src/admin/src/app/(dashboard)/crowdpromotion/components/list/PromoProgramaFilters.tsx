"use client"

import { useState, useEffect } from "react"
import { Input } from "@/components/ui/input"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { Search } from "lucide-react"
import type { PromoProgramaEstado } from "@shared/types"

type FiltroEstado = PromoProgramaEstado | "todos"

interface PromoProgramaFiltersProps {
    filtroEstado: FiltroEstado
    busqueda: string
    onFiltroEstadoChange: (valor: FiltroEstado) => void
    onBusquedaChange: (valor: string) => void
}

export function PromoProgramaFilters({
    filtroEstado,
    busqueda,
    onFiltroEstadoChange,
    onBusquedaChange,
}: PromoProgramaFiltersProps) {
    const [inputValue, setInputValue] = useState(busqueda)

    useEffect(() => {
        const timer = setTimeout(() => {
            onBusquedaChange(inputValue)
        }, 300)
        return () => clearTimeout(timer)
    }, [inputValue, onBusquedaChange])

    return (
        <div className="flex flex-col sm:flex-row gap-3">
            <div className="relative flex-1">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-zinc-400" />
                <Input
                    placeholder="Buscar programas..."
                    value={inputValue}
                    onChange={(e) => setInputValue(e.target.value)}
                    className="pl-9 bg-[#1e1e38] border-zinc-700"
                />
            </div>
            <Select
                value={filtroEstado}
                onValueChange={(val) => onFiltroEstadoChange(val as FiltroEstado)}
            >
                <SelectTrigger className="w-full sm:w-[180px] bg-[#1e1e38] border-zinc-700">
                    <SelectValue placeholder="Estado" />
                </SelectTrigger>
                <SelectContent>
                    <SelectItem value="todos">Todos</SelectItem>
                    <SelectItem value="activo">Activos</SelectItem>
                    <SelectItem value="inactivo">Inactivos</SelectItem>
                </SelectContent>
            </Select>
        </div>
    )
}
