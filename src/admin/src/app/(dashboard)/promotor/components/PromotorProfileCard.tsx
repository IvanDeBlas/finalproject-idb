"use client"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { StatsCard } from "@/components/dashboard/stats-card"
import { PromotorStatusBadge } from "./PromotorStatusBadge"
import { TIPO_PROMOTOR_LABELS } from "@shared/constants"
import { formatComisionesGanadas } from "@shared/utils/format"
import {
    Megaphone,
    Mail,
    Globe,
    Instagram,
    Youtube,
    Pencil,
    AlertTriangle,
    Wallet,
} from "lucide-react"
import type { Promotor } from "@shared/types"

interface PromotorProfileCardProps {
    promotor: Promotor
    onEditClick: () => void
    onDesactivarClick: () => void
}

function SocialLink({ url, icon: Icon, label }: { url: string | null; icon: React.ComponentType<{ className?: string }>; label: string }) {
    if (!url) return null
    return (
        <a
            href={url}
            target="_blank"
            rel="noopener noreferrer"
            className="flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors"
            aria-label={label}
        >
            <Icon className="h-4 w-4" aria-hidden="true" />
            <span className="truncate max-w-[250px]">{url}</span>
        </a>
    )
}

export function PromotorProfileCard({
    promotor,
    onEditClick,
    onDesactivarClick,
}: PromotorProfileCardProps) {
    const initials = promotor.nombrePublico
        .split(" ")
        .map((w) => w[0])
        .join("")
        .toUpperCase()
        .slice(0, 2)

    const tipoLabel = TIPO_PROMOTOR_LABELS[promotor.tipoPromotorId] ?? promotor.tipoPromotorNombre

    return (
        <div className="space-y-6">
            {/* KPI Cards */}
            <div className="grid gap-4 grid-cols-1 sm:grid-cols-2">
                <StatsCard
                    title="Programas Activos"
                    value={promotor.totalProgramasActivos.toString()}
                    icon={Megaphone}
                />
                <StatsCard
                    title="Comisiones Ganadas"
                    value={formatComisionesGanadas(
                        promotor.totalComisionesGanadas,
                        promotor.monedaComisiones
                    )}
                    icon={Wallet}
                />
            </div>

            {/* Profile Card */}
            <Card>
                <CardHeader className="flex flex-row items-center justify-between">
                    <CardTitle>Perfil de Promotor</CardTitle>
                    <div className="flex items-center gap-2">
                        <PromotorStatusBadge esActivo={promotor.esActivo} />
                        <Button variant="outline" size="sm" onClick={onEditClick}>
                            <Pencil className="mr-2 h-4 w-4" aria-hidden="true" />
                            Editar
                        </Button>
                    </div>
                </CardHeader>
                <CardContent className="space-y-6">
                    {/* Avatar + Name */}
                    <div className="flex flex-col sm:flex-row items-start gap-4">
                        <Avatar className="h-16 w-16">
                            <AvatarFallback className="bg-gradient-to-br from-pink-500 to-purple-600 text-white text-lg">
                                {initials}
                            </AvatarFallback>
                        </Avatar>
                        <div className="space-y-1">
                            <h3 className="text-xl font-semibold">{promotor.nombrePublico}</h3>
                            <p className="text-sm text-muted-foreground">{tipoLabel}</p>
                        </div>
                    </div>

                    <Separator />

                    {/* Contact Info */}
                    <div className="space-y-3">
                        <h4 className="text-sm font-medium">Contacto</h4>
                        {promotor.emailContacto && (
                            <div className="flex items-center gap-2 text-sm text-muted-foreground">
                                <Mail className="h-4 w-4" aria-hidden="true" />
                                <span>{promotor.emailContacto}</span>
                            </div>
                        )}
                        {promotor.urlSitioWeb && (
                            <a
                                href={promotor.urlSitioWeb}
                                target="_blank"
                                rel="noopener noreferrer"
                                className="flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground transition-colors"
                            >
                                <Globe className="h-4 w-4" aria-hidden="true" />
                                <span className="truncate max-w-[300px]">{promotor.urlSitioWeb}</span>
                            </a>
                        )}
                        {!promotor.emailContacto && !promotor.urlSitioWeb && (
                            <p className="text-sm text-muted-foreground">Sin informacion de contacto</p>
                        )}
                    </div>

                    {/* Social Links */}
                    {(promotor.urlInstagram || promotor.urlTikTok || promotor.urlYouTube || promotor.urlTwitter) && (
                        <>
                            <Separator />
                            <div className="space-y-3">
                                <h4 className="text-sm font-medium">Redes Sociales</h4>
                                <SocialLink url={promotor.urlInstagram} icon={Instagram} label="Instagram" />
                                <SocialLink url={promotor.urlYouTube} icon={Youtube} label="YouTube" />
                                <SocialLink url={promotor.urlTikTok} icon={Megaphone} label="TikTok" />
                                <SocialLink url={promotor.urlTwitter} icon={Megaphone} label="Twitter/X" />
                            </div>
                        </>
                    )}

                    {/* Danger Zone */}
                    {promotor.esActivo && (
                        <>
                            <Separator />
                            <div className="rounded-lg border border-red-900/50 p-4">
                                <div className="flex items-center justify-between">
                                    <div className="flex items-center gap-2">
                                        <AlertTriangle className="h-4 w-4 text-red-400" aria-hidden="true" />
                                        <span className="text-sm font-medium text-red-400">
                                            Desactivar perfil
                                        </span>
                                    </div>
                                    <Button
                                        variant="outline"
                                        size="sm"
                                        className="border-red-900/50 text-red-400 hover:bg-red-950/50"
                                        onClick={onDesactivarClick}
                                    >
                                        Desactivar
                                    </Button>
                                </div>
                            </div>
                        </>
                    )}
                </CardContent>
            </Card>
        </div>
    )
}
