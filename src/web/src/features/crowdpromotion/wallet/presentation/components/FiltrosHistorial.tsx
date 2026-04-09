import { type FC, useState, useEffect, useRef } from "react"
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { X } from "lucide-react"
import { ESTADO_WALLET_TRANSACCION, ESTADO_WALLET_TRANSACCION_LABELS } from "@shared/constants"
import type { WalletTransaccionesFilters } from "../../domain"

interface FiltrosHistorialProps {
    filters: WalletTransaccionesFilters
    onChange: (filters: WalletTransaccionesFilters) => void
    isLoading?: boolean
}

export const FiltrosHistorial: FC<FiltrosHistorialProps> = ({ filters, onChange, isLoading }) => {
    const [fechaDesde, setFechaDesde] = useState(filters.fechaDesde ?? "")
    const [fechaHasta, setFechaHasta] = useState(filters.fechaHasta ?? "")
    const [fechaError, setFechaError] = useState<string | null>(null)
    const debounceRef = useRef<ReturnType<typeof setTimeout>>()

    useEffect(() => {
        setFechaDesde(filters.fechaDesde ?? "")
        setFechaHasta(filters.fechaHasta ?? "")
    }, [filters.fechaDesde, filters.fechaHasta])

    const hasActiveFilters =
        filters.esCredito !== undefined ||
        filters.estadoTransaccionId !== undefined ||
        !!filters.fechaDesde ||
        !!filters.fechaHasta

    const handleTipoChange = (value: string) => {
        const esCredito = value === "all" ? undefined : value === "credito"
        onChange({ ...filters, esCredito, page: 1 })
    }

    const handleEstadoChange = (value: string) => {
        const estadoTransaccionId = value === "all" ? undefined : Number(value)
        onChange({ ...filters, estadoTransaccionId, page: 1 })
    }

    const applyDateFilter = (desde: string, hasta: string) => {
        if (desde && hasta && desde > hasta) {
            setFechaError("La fecha desde no puede ser posterior a la fecha hasta")
            return
        }
        setFechaError(null)
        onChange({
            ...filters,
            fechaDesde: desde || undefined,
            fechaHasta: hasta || undefined,
            page: 1,
        })
    }

    const handleFechaDesdeChange = (value: string) => {
        setFechaDesde(value)
        if (debounceRef.current) clearTimeout(debounceRef.current)
        debounceRef.current = setTimeout(() => applyDateFilter(value, fechaHasta), 500)
    }

    const handleFechaHastaChange = (value: string) => {
        setFechaHasta(value)
        if (debounceRef.current) clearTimeout(debounceRef.current)
        debounceRef.current = setTimeout(() => applyDateFilter(fechaDesde, value), 500)
    }

    const handleClearFilters = () => {
        setFechaDesde("")
        setFechaHasta("")
        setFechaError(null)
        onChange({ page: 1, pageSize: filters.pageSize })
    }

    const tipoValue = filters.esCredito === undefined ? "all" : filters.esCredito ? "credito" : "debito"
    const estadoValue = filters.estadoTransaccionId === undefined ? "all" : String(filters.estadoTransaccionId)

    return (
        <div className="flex flex-col gap-3 sm:flex-row sm:items-end sm:flex-wrap">
            <div className="w-full sm:w-40">
                <Select value={tipoValue} onValueChange={handleTipoChange} disabled={isLoading}>
                    <SelectTrigger className="bg-[#0f0f1f] border-[#334155] focus:border-[#a855f7]">
                        <SelectValue placeholder="Tipo" />
                    </SelectTrigger>
                    <SelectContent>
                        <SelectItem value="all">Todos</SelectItem>
                        <SelectItem value="credito">Creditos</SelectItem>
                        <SelectItem value="debito">Debitos</SelectItem>
                    </SelectContent>
                </Select>
            </div>

            <div className="w-full sm:w-44">
                <Select value={estadoValue} onValueChange={handleEstadoChange} disabled={isLoading}>
                    <SelectTrigger className="bg-[#0f0f1f] border-[#334155] focus:border-[#a855f7]">
                        <SelectValue placeholder="Estado" />
                    </SelectTrigger>
                    <SelectContent>
                        <SelectItem value="all">Todos los estados</SelectItem>
                        {Object.entries(ESTADO_WALLET_TRANSACCION).map(([, id]) => (
                            <SelectItem key={id} value={String(id)}>
                                {ESTADO_WALLET_TRANSACCION_LABELS[id]}
                            </SelectItem>
                        ))}
                    </SelectContent>
                </Select>
            </div>

            <div className="w-full sm:w-40">
                <Input
                    type="date"
                    value={fechaDesde}
                    onChange={(e) => handleFechaDesdeChange(e.target.value)}
                    disabled={isLoading}
                    placeholder="Desde"
                    className={`bg-[#0f0f1f] border-[#334155] focus:border-[#a855f7] ${
                        fechaError ? "border-red-500" : ""
                    }`}
                />
            </div>

            <div className="w-full sm:w-40">
                <Input
                    type="date"
                    value={fechaHasta}
                    onChange={(e) => handleFechaHastaChange(e.target.value)}
                    disabled={isLoading}
                    placeholder="Hasta"
                    className={`bg-[#0f0f1f] border-[#334155] focus:border-[#a855f7] ${
                        fechaError ? "border-red-500" : ""
                    }`}
                />
            </div>

            {fechaError && (
                <p className="text-xs text-red-500 w-full sm:w-auto">{fechaError}</p>
            )}

            {hasActiveFilters && (
                <Button
                    variant="ghost"
                    size="sm"
                    onClick={handleClearFilters}
                    className="text-[#94a3b8] hover:text-white"
                >
                    <X className="h-4 w-4 mr-1" />
                    Limpiar filtros
                </Button>
            )}
        </div>
    )
}
