import type { FC } from "react"
import { Card, CardContent } from "@/components/ui/card"
import { Skeleton } from "@/components/ui/skeleton"
import { Button } from "@/components/ui/button"
import { SearchX } from "lucide-react"
import { TransaccionItem } from "./TransaccionItem"
import type { WalletTransaccionItem } from "../../domain"

interface TransaccionesListProps {
    transacciones: WalletTransaccionItem[]
    isLoading?: boolean
    hasActiveFilters?: boolean
    monedaNombre?: string
    onClearFilters?: () => void
}

function TransaccionesListSkeleton() {
    return (
        <div className="space-y-1">
            {Array.from({ length: 5 }).map((_, i) => (
                <div key={i} className="flex items-center gap-4 p-4">
                    <Skeleton className="h-10 w-10 rounded-full bg-[#1e1e38]" />
                    <div className="flex-1 space-y-2">
                        <Skeleton className="h-4 w-48 bg-[#1e1e38]" />
                        <Skeleton className="h-3 w-24 bg-[#1e1e38]" />
                    </div>
                    <div className="flex items-center gap-3">
                        <Skeleton className="h-4 w-20 bg-[#1e1e38]" />
                        <Skeleton className="h-5 w-16 rounded-full bg-[#1e1e38]" />
                    </div>
                </div>
            ))}
        </div>
    )
}

export const TransaccionesList: FC<TransaccionesListProps> = ({
    transacciones,
    isLoading = false,
    hasActiveFilters = false,
    monedaNombre = "EUR",
    onClearFilters,
}) => {
    if (isLoading) {
        return (
            <Card className="bg-[#151525] border-[#334155]">
                <CardContent className="p-0">
                    <TransaccionesListSkeleton />
                </CardContent>
            </Card>
        )
    }

    if (transacciones.length === 0 && hasActiveFilters) {
        return (
            <Card className="bg-[#151525] border-[#334155]">
                <CardContent className="flex flex-col items-center justify-center py-16 gap-4">
                    <div className="flex items-center justify-center h-16 w-16 rounded-full bg-[#151525] border border-[#334155]">
                        <SearchX className="h-8 w-8 text-[#94a3b8]" />
                    </div>
                    <p className="text-[#94a3b8] text-center">No hay transacciones que coincidan con los filtros</p>
                    {onClearFilters && (
                        <Button variant="outline" size="sm" onClick={onClearFilters} className="border-[#334155]">
                            Limpiar filtros
                        </Button>
                    )}
                </CardContent>
            </Card>
        )
    }

    return (
        <Card className="bg-[#151525] border-[#334155]">
            <CardContent className="p-0">
                <div role="list">
                    {transacciones.map((tx) => (
                        <TransaccionItem key={tx.id} transaccion={tx} monedaNombre={monedaNombre} />
                    ))}
                </div>
            </CardContent>
        </Card>
    )
}
