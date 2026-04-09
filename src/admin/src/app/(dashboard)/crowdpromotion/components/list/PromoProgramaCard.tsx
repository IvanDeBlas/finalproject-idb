"use client"

import { useRouter } from "next/navigation"
import { Card } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { MoreVertical, Users, ClipboardList, Music, Calendar, Eye, Pencil, PowerOff } from "lucide-react"
import { PromoProgramaStatusBadge } from "./PromoProgramaStatusBadge"
import { TIPO_PROMO_LABELS } from "@shared/constants"
import { APP_ROUTES } from "@shared/constants"
import type { PromoProgramaListItem } from "@shared/types"

interface PromoProgramaCardProps {
    programa: PromoProgramaListItem
    onDesactivar: (programa: PromoProgramaListItem) => void
}

export function PromoProgramaCard({ programa, onDesactivar }: PromoProgramaCardProps) {
    const router = useRouter()

    const comisionTexto = [
        programa.importeComisionPorcentaje != null ? `${programa.importeComisionPorcentaje}%` : null,
        programa.importeComisionFija != null ? `${programa.importeComisionFija} ${programa.monedaNombre}` : null,
    ]
        .filter(Boolean)
        .join(" + ")

    const fechaTexto = [
        programa.fechaInicio ? new Date(programa.fechaInicio).toLocaleDateString("es-ES") : null,
        programa.fechaFin ? new Date(programa.fechaFin).toLocaleDateString("es-ES") : null,
    ]
        .filter(Boolean)
        .join(" - ")

    return (
        <Card
            className="bg-[#1a1a2e] border-zinc-800 hover:border-zinc-600 transition-colors cursor-pointer p-4"
            onClick={() => router.push(APP_ROUTES.dashboard.crowdpromotion.programas.detalle(programa.id))}
        >
            <div className="flex items-start justify-between gap-3">
                <div className="flex-1 min-w-0">
                    <div className="flex items-center gap-2 mb-2 flex-wrap">
                        <h3 className="text-base font-semibold text-white truncate">
                            {programa.titulo}
                        </h3>
                        <Badge variant="outline" className="text-xs border-purple-500/30 text-purple-400">
                            {TIPO_PROMO_LABELS[programa.tipoPromoId] || programa.tipoPromoNombre}
                        </Badge>
                        <PromoProgramaStatusBadge esActivo={programa.esActivo} />
                    </div>

                    <div className="flex flex-wrap gap-x-4 gap-y-1 text-sm text-zinc-400">
                        <span className="flex items-center gap-1">
                            <Users className="h-3.5 w-3.5" />
                            {programa.numeroPromotores} promotores
                        </span>
                        <span className="flex items-center gap-1">
                            <ClipboardList className="h-3.5 w-3.5" />
                            {programa.numeroTareas} tareas
                        </span>
                        {comisionTexto && (
                            <span className="text-emerald-400">
                                Comision: {comisionTexto}
                            </span>
                        )}
                    </div>

                    <div className="flex flex-wrap gap-x-4 gap-y-1 mt-1 text-xs text-zinc-500">
                        {programa.campaniaTitulo && (
                            <span className="flex items-center gap-1">
                                <Music className="h-3 w-3" />
                                {programa.campaniaTitulo}
                            </span>
                        )}
                        {fechaTexto && (
                            <span className="flex items-center gap-1">
                                <Calendar className="h-3 w-3" />
                                {fechaTexto}
                            </span>
                        )}
                    </div>
                </div>

                <DropdownMenu>
                    <DropdownMenuTrigger asChild onClick={(e) => e.stopPropagation()}>
                        <Button variant="ghost" size="icon" className="h-8 w-8 text-zinc-400">
                            <MoreVertical className="h-4 w-4" />
                        </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end" onClick={(e) => e.stopPropagation()}>
                        <DropdownMenuItem
                            onClick={() => router.push(APP_ROUTES.dashboard.crowdpromotion.programas.detalle(programa.id))}
                        >
                            <Eye className="h-4 w-4 mr-2" />
                            Ver detalle
                        </DropdownMenuItem>
                        <DropdownMenuItem
                            onClick={() => router.push(APP_ROUTES.dashboard.crowdpromotion.programas.editar(programa.id))}
                        >
                            <Pencil className="h-4 w-4 mr-2" />
                            Editar
                        </DropdownMenuItem>
                        {programa.esActivo && (
                            <DropdownMenuItem
                                className="text-red-400 focus:text-red-400"
                                onClick={() => onDesactivar(programa)}
                            >
                                <PowerOff className="h-4 w-4 mr-2" />
                                Desactivar
                            </DropdownMenuItem>
                        )}
                    </DropdownMenuContent>
                </DropdownMenu>
            </div>
        </Card>
    )
}
