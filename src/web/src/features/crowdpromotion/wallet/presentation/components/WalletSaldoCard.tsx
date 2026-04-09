import type { FC } from "react"
import { Card, CardContent } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { Info } from "lucide-react"
import { formatWalletImporte } from "@shared/utils/format"
import { MIN_RETIRO_WALLET } from "@shared/constants"
import type { PromotorWallet } from "../../domain"

interface WalletSaldoCardProps {
    wallet: PromotorWallet | undefined
    isLoading: boolean
    onSolicitarCobro: () => void
}

export const WalletSaldoCard: FC<WalletSaldoCardProps> = ({ wallet, isLoading, onSolicitarCobro }) => {
    if (isLoading || !wallet) {
        return (
            <Card className="bg-[#151525] border-[#334155]">
                <CardContent className="p-6">
                    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
                        {Array.from({ length: 4 }).map((_, i) => (
                            <div key={i} className="space-y-2">
                                <Skeleton className="h-4 w-24 bg-[#1e1e38]" />
                                <Skeleton className="h-8 w-32 bg-[#1e1e38]" />
                            </div>
                        ))}
                    </div>
                    <div className="mt-6">
                        <Skeleton className="h-10 w-40 bg-[#1e1e38]" />
                    </div>
                </CardContent>
            </Card>
        )
    }

    const canSolicitar = wallet.saldoDisponible >= wallet.minimoRetiro
    const moneda = wallet.monedaNombre || "EUR"

    return (
        <Card className="bg-[#151525] border-[#334155]">
            <CardContent className="p-6">
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
                    <div>
                        <p className="text-sm text-[#94a3b8]">Saldo disponible</p>
                        <p className="text-4xl font-bold text-[#f59e0b]">
                            {formatWalletImporte(wallet.saldoDisponible, moneda)}
                        </p>
                    </div>
                    <div>
                        <p className="text-sm text-[#94a3b8]">Total ganado</p>
                        <p className="text-2xl font-semibold text-[#10b981]">
                            {formatWalletImporte(wallet.totalGanado, moneda)}
                        </p>
                    </div>
                    <div>
                        <p className="text-sm text-[#94a3b8]">Total retirado</p>
                        <p className="text-2xl font-semibold text-[#94a3b8]">
                            {formatWalletImporte(wallet.totalRetirado, moneda)}
                        </p>
                    </div>
                    <div>
                        <p className="text-sm text-[#94a3b8]">Moneda</p>
                        <p className="text-2xl font-semibold text-white">{moneda}</p>
                    </div>
                </div>

                <div className="mt-6 flex items-center gap-4">
                    <Button
                        onClick={onSolicitarCobro}
                        disabled={!canSolicitar}
                        aria-disabled={!canSolicitar}
                        className="bg-[#a855f7] hover:bg-[#9333ea] text-white"
                    >
                        Solicitar cobro
                    </Button>

                    {!canSolicitar && (
                        <div className="flex items-center gap-2 text-sm text-[#f59e0b]">
                            <Info className="h-4 w-4 shrink-0" />
                            <span>
                                Necesitas al menos {formatWalletImporte(MIN_RETIRO_WALLET, moneda)} para solicitar un cobro
                            </span>
                        </div>
                    )}
                </div>
            </CardContent>
        </Card>
    )
}
