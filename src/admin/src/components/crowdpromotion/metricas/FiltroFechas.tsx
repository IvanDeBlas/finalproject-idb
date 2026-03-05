"use client"

import { useMemo } from "react"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Calendar, X, Loader2 } from "lucide-react"

interface FiltroFechasProps {
    fechaDesde: string
    fechaHasta: string
    onFechaDesdeChange: (value: string) => void
    onFechaHastaChange: (value: string) => void
    onAplicar: () => void
    onLimpiar: () => void
    tieneFiltroPeriodo: boolean
    isLoading: boolean
}

export function FiltroFechas({
    fechaDesde,
    fechaHasta,
    onFechaDesdeChange,
    onFechaHastaChange,
    onAplicar,
    onLimpiar,
    tieneFiltroPeriodo,
    isLoading,
}: FiltroFechasProps) {
    const hoy = useMemo(() => new Date().toISOString().split("T")[0], [])

    const errorFechas = useMemo(() => {
        if (fechaDesde && fechaHasta && fechaDesde > fechaHasta) {
            return "La fecha inicio no puede ser posterior a la fecha fin"
        }
        return null
    }, [fechaDesde, fechaHasta])

    const isAplicarDisabled = isLoading || !!errorFechas

    return (
        <div className="flex flex-col gap-3 sm:flex-row sm:items-end sm:gap-4">
            <div className="flex items-center gap-2 flex-1">
                <Calendar className="w-4 h-4 text-[#64748b] shrink-0" />
                <div className="flex-1">
                    <Input
                        type="date"
                        value={fechaDesde}
                        onChange={(e) => onFechaDesdeChange(e.target.value)}
                        max={hoy}
                        aria-label="Fecha de inicio"
                        className="bg-[#0f0f1f] border-zinc-700 text-white text-sm h-9 focus-visible:ring-2 focus-visible:ring-[#a855f7]"
                    />
                    {errorFechas && (
                        <p className="text-xs text-destructive mt-1">{errorFechas}</p>
                    )}
                </div>
                <span className="text-[#64748b] text-sm">—</span>
                <div className="flex-1">
                    <Input
                        type="date"
                        value={fechaHasta}
                        onChange={(e) => onFechaHastaChange(e.target.value)}
                        max={hoy}
                        aria-label="Fecha de fin"
                        className="bg-[#0f0f1f] border-zinc-700 text-white text-sm h-9 focus-visible:ring-2 focus-visible:ring-[#a855f7]"
                    />
                </div>
            </div>
            <div className="flex items-center gap-2">
                <Button
                    onClick={onAplicar}
                    disabled={isAplicarDisabled}
                    size="sm"
                    className="bg-gradient-to-r from-[#ec4899] to-[#a855f7] hover:opacity-90 text-white focus-visible:ring-2 focus-visible:ring-[#a855f7]"
                >
                    {isLoading ? (
                        <Loader2 className="w-4 h-4 animate-spin mr-1" />
                    ) : null}
                    Aplicar
                </Button>
                {tieneFiltroPeriodo && (
                    <Button
                        onClick={onLimpiar}
                        variant="ghost"
                        size="sm"
                        className="text-[#94a3b8] hover:text-white focus-visible:ring-2 focus-visible:ring-[#a855f7]"
                    >
                        Limpiar
                    </Button>
                )}
            </div>
            {tieneFiltroPeriodo && (
                <Badge
                    variant="secondary"
                    className="bg-[#1e1e38] text-[#94a3b8] border border-zinc-700 text-xs flex items-center gap-1.5 w-fit"
                >
                    Filtrando: {fechaDesde || "..."} — {fechaHasta || "..."}
                    <button
                        onClick={onLimpiar}
                        aria-label="Quitar filtro de fechas"
                        className="hover:text-white focus-visible:ring-2 focus-visible:ring-[#a855f7] rounded"
                    >
                        <X className="w-3 h-3" />
                    </button>
                </Badge>
            )}
        </div>
    )
}
