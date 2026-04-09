import { TrendingUp } from "lucide-react"
import { formatTasaConversion } from "@shared/utils/format"

interface TasaConversionInlineProps {
    tasa: number
}

export function TasaConversionInline({ tasa }: TasaConversionInlineProps) {
    return (
        <div className="flex items-center gap-2 mb-6 text-sm">
            <TrendingUp className="w-4 h-4 text-[#94a3b8] shrink-0" />
            <span className="text-[#94a3b8]">Tasa de conversion:</span>
            <span className="font-semibold text-white">{formatTasaConversion(tasa)}</span>
            <span className="text-xs text-[#64748b]">(conversiones / clicks)</span>
        </div>
    )
}
