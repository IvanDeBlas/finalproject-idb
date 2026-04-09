import type { FC } from "react"
import { Badge } from "@/components/ui/badge"
import { ArrowUp, ArrowDown } from "lucide-react"
import { formatTransaccionImporte } from "@shared/utils/format"
import { ESTADO_WALLET_TRANSACCION_LABELS } from "@shared/constants"
import type { WalletTransaccionItem } from "../../domain"

interface TransaccionItemProps {
    transaccion: WalletTransaccionItem
    monedaNombre?: string
}

const BADGE_CLASSES: Record<number, string> = {
    1: "bg-amber-950/50 text-[#f59e0b] border border-amber-800/50 text-xs font-medium",
    2: "bg-green-950/50 text-[#10b981] border border-green-800/50 text-xs font-medium",
    3: "bg-blue-950/50 text-[#3b82f6] border border-blue-800/50 text-xs font-medium",
    4: "bg-slate-900/50 text-[#64748b] border border-slate-700/50 text-xs font-medium",
}

export const TransaccionItem: FC<TransaccionItemProps> = ({ transaccion, monedaNombre = "EUR" }) => {
    const isCredit = transaccion.esCredito
    const Icon = isCredit ? ArrowUp : ArrowDown
    const iconColorClass = isCredit ? "text-green-500 bg-green-950/50" : "text-red-500 bg-red-950/50"
    const importeColorClass = isCredit ? "text-green-500" : "text-red-500"
    const estadoLabel = ESTADO_WALLET_TRANSACCION_LABELS[transaccion.estadoTransaccionId] ?? "Desconocido"
    const badgeClass = BADGE_CLASSES[transaccion.estadoTransaccionId] ?? BADGE_CLASSES[1]

    const fechaFormatted = new Date(transaccion.fechaCreacion).toLocaleDateString("es-ES", {
        day: "numeric",
        month: "short",
        year: "numeric",
    })

    return (
        <div
            role="listitem"
            className="flex items-center gap-4 p-4 hover:bg-[#1e1e38] transition-colors duration-150 rounded-lg"
        >
            <div className={`flex items-center justify-center h-10 w-10 rounded-full shrink-0 ${iconColorClass}`}>
                <Icon className="h-5 w-5" />
            </div>

            <div className="flex-1 min-w-0">
                <p className="text-sm font-medium text-white truncate">
                    {transaccion.concepto || transaccion.descripcion || "Transaccion"}
                </p>
                <div className="flex items-center gap-2 text-xs text-[#94a3b8]">
                    {transaccion.tipoRewardNombre && (
                        <>
                            <span>{transaccion.tipoRewardNombre}</span>
                            <span>&middot;</span>
                        </>
                    )}
                    <time dateTime={transaccion.fechaCreacion}>{fechaFormatted}</time>
                </div>
            </div>

            <div className="text-right shrink-0 flex items-center gap-3">
                <span className={`text-sm font-semibold ${importeColorClass}`}>
                    {formatTransaccionImporte(transaccion.importe, isCredit, monedaNombre)}
                </span>
                <Badge variant="outline" className={badgeClass}>
                    {estadoLabel}
                </Badge>
            </div>
        </div>
    )
}
