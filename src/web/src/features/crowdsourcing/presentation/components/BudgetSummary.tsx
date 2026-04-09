import { FC } from "react"
import { Card, CardHeader, CardContent, CardTitle } from "@/components/ui/card"
import { Progress } from "@/components/ui/progress"
import { formatCurrency } from "@/features/campanias/application/utils"

interface BudgetSummaryProps {
    minTotal: number
    maxTotal: number
    selectedCount: number
    totalCount: number
}

export const BudgetSummary: FC<BudgetSummaryProps> = ({
    minTotal,
    maxTotal,
    selectedCount,
    totalCount,
}) => {
    const percentage = totalCount > 0 ? (selectedCount / totalCount) * 100 : 0

    return (
        <Card className="bg-gradient-to-r from-purple-900/20 to-pink-900/20 border border-[#a855f7] p-6 lg:sticky lg:top-4">
            <CardHeader className="p-0 mb-4">
                <CardTitle className="text-lg font-semibold text-white">
                    Resumen Presupuestario
                </CardTitle>
            </CardHeader>
            <CardContent className="p-0 space-y-4">
                <div className="space-y-2">
                    <div className="flex justify-between items-baseline">
                        <span className="text-sm text-[#94a3b8]">Min total:</span>
                        <span className="text-2xl font-bold text-white">
                            {formatCurrency(minTotal)}
                        </span>
                    </div>
                    <div className="flex justify-between items-baseline">
                        <span className="text-sm text-[#94a3b8]">Max total:</span>
                        <span className="text-2xl font-bold text-white">
                            {formatCurrency(maxTotal)}
                        </span>
                    </div>
                    <div className="flex justify-between items-baseline border-t border-[#334155] pt-2">
                        <span className="text-sm text-[#94a3b8]">Promedio:</span>
                        <span className="text-base font-semibold text-[#a855f7]">
                            ~{formatCurrency(Math.round((minTotal + maxTotal) / 2))}
                        </span>
                    </div>
                </div>

                <div>
                    <p className="text-sm text-[#94a3b8] mb-2">
                        Necesidades seleccionadas:{" "}
                        <span className="font-bold text-white">{selectedCount}</span> de{" "}
                        {totalCount}
                    </p>
                    <Progress value={percentage} className="h-2" />
                </div>

                {selectedCount === 0 && (
                    <p className="text-xs text-yellow-400">
                        Selecciona al menos una necesidad para continuar
                    </p>
                )}
            </CardContent>
        </Card>
    )
}
