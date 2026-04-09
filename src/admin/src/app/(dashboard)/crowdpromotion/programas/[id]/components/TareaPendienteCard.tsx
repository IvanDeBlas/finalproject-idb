"use client"

import { Card } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Check, X, ExternalLink, Clock } from "lucide-react"
import type { TareaPendienteItem } from "@shared/types/crowdpromotion"

interface TareaPendienteCardProps {
    item: TareaPendienteItem
    onValidar: (item: TareaPendienteItem) => void
    onRechazar: (item: TareaPendienteItem) => void
    isProcessing?: boolean
}

function formatOrdinal(n: number): string {
    if (n === 1) return "1ra vez"
    if (n === 2) return "2da vez"
    if (n === 3) return "3ra vez"
    return `${n}ta vez`
}

function formatRelativeDate(dateStr: string): string {
    const date = new Date(dateStr)
    const now = new Date()
    const diffMs = now.getTime() - date.getTime()
    const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))

    if (diffDays === 0) return "hoy"
    if (diffDays === 1) return "hace 1 dia"
    if (diffDays < 30) return `hace ${diffDays} dias`
    const diffMonths = Math.floor(diffDays / 30)
    if (diffMonths === 1) return "hace 1 mes"
    return `hace ${diffMonths} meses`
}

export function TareaPendienteCard({ item, onValidar, onRechazar, isProcessing }: TareaPendienteCardProps) {
    return (
        <Card className="bg-[#151525] border-[#334155] p-5 mb-4">
            <div className="flex items-start justify-between gap-4">
                <div className="flex-1 min-w-0">
                    <p className="text-base font-semibold text-white">
                        {item.promotorNombre}
                        {item.promotorTipoNombre && (
                            <span className="text-xs text-[#64748b] ml-2 font-normal">
                                {item.promotorTipoNombre}
                            </span>
                        )}
                    </p>
                    <p className="text-sm text-[#94a3b8] mt-0.5">
                        {item.tareaNombre} ({formatOrdinal(item.vecesCompletada)})
                    </p>

                    {item.urlPruebaCompletado && (
                        <div className="flex items-center gap-1.5 mt-2">
                            <span className="text-xs text-[#64748b] shrink-0">Prueba:</span>
                            <a
                                href={item.urlPruebaCompletado}
                                target="_blank"
                                rel="noopener noreferrer"
                                className="text-sm text-[#a855f7] hover:underline truncate flex items-center gap-1"
                            >
                                {item.urlPruebaCompletado}
                                <ExternalLink className="w-3 h-3 shrink-0" aria-hidden="true" />
                            </a>
                        </div>
                    )}

                    {item.comentarioPromotor ? (
                        <p className="text-sm text-[#94a3b8] italic mt-2 border-l-2 border-[#334155] pl-3">
                            {item.comentarioPromotor}
                        </p>
                    ) : (
                        <p className="text-xs text-[#64748b] italic mt-2">(sin comentario)</p>
                    )}

                    {item.fechaUltimaCompletada && (
                        <p className="text-xs text-[#64748b] mt-2 flex items-center gap-1">
                            <Clock className="w-3 h-3" aria-hidden="true" />
                            {formatRelativeDate(item.fechaUltimaCompletada)}
                        </p>
                    )}
                </div>

                <div className="flex items-center gap-2 shrink-0">
                    <Button
                        size="sm"
                        className="bg-green-950/50 text-green-400 border border-green-800/50 hover:bg-green-900/50 h-8 px-3 text-xs"
                        disabled={isProcessing}
                        onClick={() => onValidar(item)}
                    >
                        <Check className="w-3.5 h-3.5 mr-1" aria-hidden="true" />
                        Validar
                    </Button>
                    <Button
                        size="sm"
                        variant="outline"
                        className="border-red-800/50 text-red-400 hover:bg-red-950/30 hover:text-red-300 h-8 px-3 text-xs"
                        disabled={isProcessing}
                        onClick={() => onRechazar(item)}
                    >
                        <X className="w-3.5 h-3.5 mr-1" aria-hidden="true" />
                        Rechazar
                    </Button>
                </div>
            </div>
        </Card>
    )
}
