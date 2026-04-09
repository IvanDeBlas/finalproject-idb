"use client"

import { useState } from "react"
import { Card } from "@/components/ui/card"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { UserCheck, UserX, Ban, Globe, Mail, Link as LinkIcon, Loader2 } from "lucide-react"
import { useAprobarInscripcion, useRechazarInscripcion, useBloquearInscripcion } from "@/hooks/use-inscripciones-mutations"
import { InscripcionConfirmDialog } from "./InscripcionConfirmDialog"
import type { InscripcionListItem } from "@shared/types"

interface SolicitudCardProps {
    inscripcion: InscripcionListItem
    programaId: string
}

function getInitials(name: string): string {
    return name.slice(0, 2).toUpperCase()
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

export function SolicitudCard({ inscripcion, programaId }: SolicitudCardProps) {
    const [showRechazarDialog, setShowRechazarDialog] = useState(false)
    const [showBloquearDialog, setShowBloquearDialog] = useState(false)
    const [isHiding, setIsHiding] = useState(false)

    const aprobarMutation = useAprobarInscripcion()
    const rechazarMutation = useRechazarInscripcion()
    const bloquearMutation = useBloquearInscripcion()

    const isAnyPending = aprobarMutation.isPending || rechazarMutation.isPending || bloquearMutation.isPending

    const handleAprobar = () => {
        aprobarMutation.mutate(
            { programaId, inscripcionId: inscripcion.id, promotorNombre: inscripcion.promotorNombre },
            {
                onSuccess: () => {
                    setIsHiding(true)
                },
            }
        )
    }

    const handleRechazar = () => {
        rechazarMutation.mutate(
            { programaId, inscripcionId: inscripcion.id, promotorNombre: inscripcion.promotorNombre },
            {
                onSuccess: () => {
                    setShowRechazarDialog(false)
                    setIsHiding(true)
                },
            }
        )
    }

    const handleBloquear = () => {
        bloquearMutation.mutate(
            { programaId, inscripcionId: inscripcion.id, promotorNombre: inscripcion.promotorNombre },
            {
                onSuccess: () => {
                    setShowBloquearDialog(false)
                    setIsHiding(true)
                },
            }
        )
    }

    const hasAnySocial =
        inscripcion.promotorUrlInstagram ||
        inscripcion.promotorUrlTikTok ||
        inscripcion.promotorUrlSitioWeb ||
        inscripcion.promotorEmailContacto

    return (
        <>
            <Card
                className={`bg-[#0f0f1f] border border-[#334155] p-4 mb-3 transition-all duration-200 ${
                    isHiding ? "opacity-0 max-h-0 overflow-hidden" : "opacity-100"
                }`}
            >
                <div className="flex items-start justify-between">
                    <div className="flex items-center gap-3">
                        <Avatar className="w-10 h-10">
                            <AvatarFallback className="bg-[#1e1e38] text-[#94a3b8] text-sm font-semibold">
                                {getInitials(inscripcion.promotorNombre)}
                            </AvatarFallback>
                        </Avatar>
                        <div>
                            <p className="text-sm font-semibold text-white">
                                {inscripcion.promotorNombre}
                            </p>
                            <p className="text-xs text-[#94a3b8]">
                                {inscripcion.tipoPromotorNombre}
                            </p>
                        </div>
                    </div>
                    <p className="text-xs text-[#64748b] shrink-0">
                        {formatRelativeDate(inscripcion.fechaAlta)}
                    </p>
                </div>

                <Separator className="bg-[#334155] my-3" />

                {hasAnySocial ? (
                    <div className="space-y-1.5">
                        {inscripcion.promotorUrlInstagram && (
                            <div className="flex items-center gap-2">
                                <LinkIcon className="w-3.5 h-3.5 text-[#64748b] shrink-0" aria-hidden="true" />
                                <span className="text-xs text-[#64748b] w-20 shrink-0">Instagram</span>
                                <a
                                    href={inscripcion.promotorUrlInstagram}
                                    target="_blank"
                                    rel="noopener noreferrer"
                                    className="text-xs text-[#a855f7] hover:underline truncate"
                                >
                                    {inscripcion.promotorUrlInstagram}
                                </a>
                            </div>
                        )}
                        {inscripcion.promotorUrlTikTok && (
                            <div className="flex items-center gap-2">
                                <LinkIcon className="w-3.5 h-3.5 text-[#64748b] shrink-0" aria-hidden="true" />
                                <span className="text-xs text-[#64748b] w-20 shrink-0">TikTok</span>
                                <a
                                    href={inscripcion.promotorUrlTikTok}
                                    target="_blank"
                                    rel="noopener noreferrer"
                                    className="text-xs text-[#a855f7] hover:underline truncate"
                                >
                                    {inscripcion.promotorUrlTikTok}
                                </a>
                            </div>
                        )}
                        {inscripcion.promotorUrlSitioWeb && (
                            <div className="flex items-center gap-2">
                                <Globe className="w-3.5 h-3.5 text-[#64748b] shrink-0" aria-hidden="true" />
                                <span className="text-xs text-[#64748b] w-20 shrink-0">Web</span>
                                <a
                                    href={inscripcion.promotorUrlSitioWeb}
                                    target="_blank"
                                    rel="noopener noreferrer"
                                    className="text-xs text-[#a855f7] hover:underline truncate"
                                >
                                    {inscripcion.promotorUrlSitioWeb}
                                </a>
                            </div>
                        )}
                        {inscripcion.promotorEmailContacto && (
                            <div className="flex items-center gap-2">
                                <Mail className="w-3.5 h-3.5 text-[#64748b] shrink-0" aria-hidden="true" />
                                <span className="text-xs text-[#64748b] w-20 shrink-0">Email</span>
                                <span className="text-xs text-[#94a3b8] truncate">
                                    {inscripcion.promotorEmailContacto}
                                </span>
                            </div>
                        )}
                    </div>
                ) : (
                    <p className="text-xs text-[#64748b] italic">Sin redes sociales registradas</p>
                )}

                <Separator className="bg-[#334155] mt-3 mb-2" />

                <div className="flex justify-end gap-2 mt-2 flex-col sm:flex-row">
                    <Button
                        size="sm"
                        variant="outline"
                        className="border-red-800/50 text-red-400 hover:bg-red-950/30 hover:text-red-300 h-8 px-3 text-xs"
                        disabled={isAnyPending}
                        onClick={() => setShowBloquearDialog(true)}
                    >
                        <Ban className="w-3.5 h-3.5 mr-1.5" aria-hidden="true" />
                        Bloquear
                    </Button>
                    <Button
                        size="sm"
                        variant="outline"
                        className="border-[#334155] text-[#94a3b8] hover:text-white hover:bg-[#1e1e38] h-8 px-3 text-xs"
                        disabled={isAnyPending}
                        onClick={() => setShowRechazarDialog(true)}
                    >
                        <UserX className="w-3.5 h-3.5 mr-1.5" aria-hidden="true" />
                        Rechazar
                    </Button>
                    <Button
                        size="sm"
                        className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold h-8 px-4 text-xs"
                        disabled={isAnyPending}
                        onClick={handleAprobar}
                    >
                        {aprobarMutation.isPending ? (
                            <>
                                <Loader2 className="w-3.5 h-3.5 animate-spin mr-1.5" aria-hidden="true" />
                                Aprobando...
                            </>
                        ) : (
                            <>
                                <UserCheck className="w-3.5 h-3.5 mr-1.5" aria-hidden="true" />
                                Aprobar
                            </>
                        )}
                    </Button>
                </div>
            </Card>

            <InscripcionConfirmDialog
                open={showRechazarDialog}
                onOpenChange={setShowRechazarDialog}
                onConfirm={handleRechazar}
                isPending={rechazarMutation.isPending}
                variante="rechazar"
                promotorNombre={inscripcion.promotorNombre}
            />

            <InscripcionConfirmDialog
                open={showBloquearDialog}
                onOpenChange={setShowBloquearDialog}
                onConfirm={handleBloquear}
                isPending={bloquearMutation.isPending}
                variante="bloquear"
                promotorNombre={inscripcion.promotorNombre}
            />
        </>
    )
}
