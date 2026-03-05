"use client"

import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Pencil, PowerOff, Music } from "lucide-react"
import { PromoProgramaStatusBadge } from "../../../components/list/PromoProgramaStatusBadge"
import { TIPO_PROMO_LABELS, APP_ROUTES } from "@shared/constants"
import type { PromoProgramaDetail } from "@shared/types"

interface PromoProgramaHeaderProps {
    programa: PromoProgramaDetail
    onDesactivar: () => void
}

export function PromoProgramaHeader({ programa, onDesactivar }: PromoProgramaHeaderProps) {
    const router = useRouter()

    return (
        <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
            <div>
                <div className="flex items-center gap-2 flex-wrap mb-1">
                    <h1 className="text-2xl font-bold text-white">{programa.titulo}</h1>
                    <Badge variant="outline" className="text-xs border-purple-500/30 text-purple-400">
                        {TIPO_PROMO_LABELS[programa.tipoPromoId] || programa.tipoPromoNombre}
                    </Badge>
                    <PromoProgramaStatusBadge esActivo={programa.esActivo} />
                </div>
                {programa.campaniaTitulo && (
                    <p className="text-sm text-zinc-400 flex items-center gap-1">
                        <Music className="h-3.5 w-3.5" />
                        {programa.campaniaTitulo}
                    </p>
                )}
            </div>

            <div className="flex items-center gap-2">
                <Button
                    variant="outline"
                    size="sm"
                    onClick={() => router.push(APP_ROUTES.dashboard.crowdpromotion.programas.editar(programa.id))}
                >
                    <Pencil className="h-4 w-4 mr-2" />
                    Editar
                </Button>
                {programa.esActivo && (
                    <Button
                        variant="outline"
                        size="sm"
                        className="border-red-500/30 text-red-400 hover:bg-red-500/10"
                        onClick={onDesactivar}
                    >
                        <PowerOff className="h-4 w-4 mr-2" />
                        Desactivar
                    </Button>
                )}
            </div>
        </div>
    )
}
