"use client"

import { useState } from "react"
import { Card } from "@/components/ui/card"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Button } from "@/components/ui/button"
import { UserMinus, Calendar } from "lucide-react"
import { useDarDeBajaInscripcion } from "@/hooks/use-inscripciones-mutations"
import { InscripcionConfirmDialog } from "./InscripcionConfirmDialog"
import type { InscripcionListItem } from "@shared/types"

interface AprobadoCardProps {
    inscripcion: InscripcionListItem
    programaId: string
}

function getInitials(name: string): string {
    return name.slice(0, 2).toUpperCase()
}

function formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString("es-ES", {
        day: "2-digit",
        month: "short",
        year: "numeric",
    })
}

export function AprobadoCard({ inscripcion, programaId }: AprobadoCardProps) {
    const [showDarDeBajaDialog, setShowDarDeBajaDialog] = useState(false)
    const [isHiding, setIsHiding] = useState(false)

    const darDeBajaMutation = useDarDeBajaInscripcion()

    const handleDarDeBaja = () => {
        darDeBajaMutation.mutate(
            { programaId, inscripcionId: inscripcion.id, promotorNombre: inscripcion.promotorNombre },
            {
                onSuccess: () => {
                    setShowDarDeBajaDialog(false)
                    setIsHiding(true)
                },
            }
        )
    }

    return (
        <>
            <Card
                className={`bg-[#0f0f1f] border border-[#334155] p-4 transition-all duration-200 ${
                    isHiding ? "opacity-0 max-h-0 overflow-hidden" : "opacity-100"
                }`}
            >
                <div className="flex items-start gap-3">
                    <Avatar className="w-9 h-9">
                        <AvatarFallback className="bg-[#1e1e38] text-[#94a3b8] text-xs font-semibold">
                            {getInitials(inscripcion.promotorNombre)}
                        </AvatarFallback>
                    </Avatar>
                    <div className="flex-1 min-w-0">
                        <p className="text-sm font-semibold text-white truncate">
                            {inscripcion.promotorNombre}
                        </p>
                        <p className="text-xs text-[#94a3b8]">{inscripcion.tipoPromotorNombre}</p>
                        <p className="text-xs text-[#64748b] mt-0.5 flex items-center gap-1">
                            <Calendar className="w-3 h-3" aria-hidden="true" />
                            Aprobado: {formatDate(inscripcion.fechaAlta)}
                        </p>
                    </div>
                    <div className="text-right shrink-0">
                        {inscripcion.codigoReferido && (
                            <p className="text-xs font-mono text-[#a855f7] font-medium">
                                {inscripcion.codigoReferido}
                            </p>
                        )}
                        <p className="text-xs text-[#64748b] mt-0.5">Clics: 0 | Conv.: 0</p>
                        <Button
                            size="sm"
                            variant="outline"
                            className="border-red-800/50 text-red-400 hover:bg-red-950/30 hover:text-red-300 h-7 px-3 text-xs mt-2"
                            onClick={() => setShowDarDeBajaDialog(true)}
                        >
                            <UserMinus className="w-3.5 h-3.5 mr-1" aria-hidden="true" />
                            Dar de baja
                        </Button>
                    </div>
                </div>
            </Card>

            <InscripcionConfirmDialog
                open={showDarDeBajaDialog}
                onOpenChange={setShowDarDeBajaDialog}
                onConfirm={handleDarDeBaja}
                isPending={darDeBajaMutation.isPending}
                variante="dar-de-baja"
                promotorNombre={inscripcion.promotorNombre}
            />
        </>
    )
}
