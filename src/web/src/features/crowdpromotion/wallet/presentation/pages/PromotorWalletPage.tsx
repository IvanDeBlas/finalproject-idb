import { useState, useCallback } from "react"
import { Card, CardContent } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { AlertCircle } from "lucide-react"
import { WALLET_DEFAULT_PAGE_SIZE } from "@shared/constants"
import { usePromotorWallet } from "../../application/hooks/usePromotorWallet"
import { useWalletTransacciones } from "../../application/hooks/useWalletTransacciones"
import { WalletSaldoCard } from "../components/WalletSaldoCard"
import { FiltrosHistorial } from "../components/FiltrosHistorial"
import { TransaccionesList } from "../components/TransaccionesList"
import { PaginacionWallet } from "../components/PaginacionWallet"
import { WalletEmptyState } from "../components/WalletEmptyState"
import { SolicitarCobroDialog } from "../components/SolicitarCobroDialog"
import type { WalletTransaccionesFilters } from "../../domain"

export default function PromotorWalletPage() {
    const [isCobroDialogOpen, setIsCobroDialogOpen] = useState(false)
    const [filters, setFilters] = useState<WalletTransaccionesFilters>({
        page: 1,
        pageSize: WALLET_DEFAULT_PAGE_SIZE,
    })

    const {
        data: wallet,
        isLoading: isWalletLoading,
        isError: isWalletError,
        refetch: refetchWallet,
    } = usePromotorWallet()

    const {
        data: transaccionesData,
        isLoading: isTransaccionesLoading,
        isFetching: isTransaccionesFetching,
    } = useWalletTransacciones(filters)

    const hasActiveFilters =
        filters.esCredito !== undefined ||
        filters.estadoTransaccionId !== undefined ||
        !!filters.fechaDesde ||
        !!filters.fechaHasta

    const handleFiltersChange = useCallback((newFilters: WalletTransaccionesFilters) => {
        setFilters(newFilters)
    }, [])

    const handleClearFilters = useCallback(() => {
        setFilters({ page: 1, pageSize: WALLET_DEFAULT_PAGE_SIZE })
    }, [])

    const handlePageChange = useCallback((page: number) => {
        setFilters((prev) => ({ ...prev, page }))
    }, [])

    const handleCobroSuccess = useCallback(() => {
        // Queries are invalidated inside the dialog; refetch happens automatically
    }, [])

    const isLoading = isWalletLoading || isTransaccionesLoading
    const totalCount = transaccionesData?.totalCount ?? 0
    const hasNoTransactions = !isLoading && totalCount === 0 && !hasActiveFilters

    return (
        <div className="min-h-screen bg-[#1a1a2e] p-4 md:p-6 lg:p-8" aria-busy={isLoading}>
            <div className="max-w-5xl mx-auto space-y-6">
                <h1 className="text-2xl font-bold text-white">Mi Wallet</h1>

                {isWalletError ? (
                    <Card className="bg-[#151525] border-[#334155]">
                        <CardContent className="flex flex-col items-center justify-center py-16 gap-4">
                            <AlertCircle className="h-12 w-12 text-red-500" />
                            <p className="text-[#94a3b8] text-center">No se pudo cargar tu wallet</p>
                            <Button variant="outline" onClick={() => refetchWallet()} className="border-[#334155]">
                                Reintentar
                            </Button>
                        </CardContent>
                    </Card>
                ) : (
                    <>
                        <WalletSaldoCard
                            wallet={wallet}
                            isLoading={isWalletLoading}
                            onSolicitarCobro={() => setIsCobroDialogOpen(true)}
                        />

                        {hasNoTransactions ? (
                            <WalletEmptyState />
                        ) : (
                            <>
                                {!isWalletLoading && (
                                    <FiltrosHistorial
                                        filters={filters}
                                        onChange={handleFiltersChange}
                                        isLoading={isTransaccionesFetching}
                                    />
                                )}

                                <TransaccionesList
                                    transacciones={transaccionesData?.items ?? []}
                                    isLoading={isTransaccionesLoading}
                                    hasActiveFilters={hasActiveFilters}
                                    monedaNombre={wallet?.monedaNombre}
                                    onClearFilters={handleClearFilters}
                                />

                                {transaccionesData && transaccionesData.items.length > 0 && (
                                    <PaginacionWallet
                                        page={transaccionesData.page}
                                        totalPages={transaccionesData.totalPages}
                                        totalCount={transaccionesData.totalCount}
                                        pageSize={filters.pageSize ?? WALLET_DEFAULT_PAGE_SIZE}
                                        onPageChange={handlePageChange}
                                    />
                                )}
                            </>
                        )}

                        {wallet && (
                            <SolicitarCobroDialog
                                open={isCobroDialogOpen}
                                onOpenChange={setIsCobroDialogOpen}
                                saldoDisponible={wallet.saldoDisponible}
                                minimoRetiro={wallet.minimoRetiro}
                                monedaNombre={wallet.monedaNombre}
                                onSuccess={handleCobroSuccess}
                            />
                        )}
                    </>
                )}
            </div>
        </div>
    )
}
