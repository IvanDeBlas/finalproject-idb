import { useState } from "react"
import { useParams, Link } from "react-router-dom"
import { Card } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Skeleton } from "@/components/ui/skeleton"
import { Separator } from "@/components/ui/separator"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import {
    ArrowLeft,
    Wifi,
    MapPin,
    GitBranch,
    Clock,
    Users,
    Calendar,
    CheckCircle,
    Info,
} from "lucide-react"
import { useNecesidadPublica } from "../../application"
import { UrgenciaBadge } from "../components/UrgenciaBadge"
import { EnviarPropuestaForm } from "../components/EnviarPropuestaForm"
import { PerfilProfesionalCTA } from "../components/PerfilProfesionalCTA"
import { APP_ROUTES, MODALIDAD_TRABAJO } from "@shared/constants"

const MODALIDAD_ICONS: Record<number, typeof Wifi> = {
    [MODALIDAD_TRABAJO.REMOTO]: Wifi,
    [MODALIDAD_TRABAJO.PRESENCIAL]: MapPin,
    [MODALIDAD_TRABAJO.HIBRIDO]: GitBranch,
}

function formatFecha(fecha: string): string {
    return new Date(fecha).toLocaleDateString("es-ES", {
        day: "numeric",
        month: "long",
        year: "numeric",
    })
}

function getFechaLimiteColor(fechaLimite: string): string {
    const diasRestantes = Math.ceil(
        (new Date(fechaLimite).getTime() - Date.now()) / 86400000
    )
    if (diasRestantes < 3) return "text-red-400"
    if (diasRestantes < 7) return "text-amber-400"
    return "text-white"
}

export default function NecesidadDetallePage() {
    const { id } = useParams<{ id: string }>()
    const { data, isLoading, error } = useNecesidadPublica(id ?? "")
    const [isFormOpen, setIsFormOpen] = useState(false)

    if (isLoading) {
        return (
            <div className="min-h-screen bg-[#1a1a2e]">
                <div className="max-w-6xl mx-auto px-4 py-8">
                    <Skeleton className="h-5 w-48 mb-6" />
                    <div className="flex gap-8">
                        <div className="flex-1 space-y-6">
                            <Card className="bg-[#0f1729] border-[#334155] p-6">
                                <Skeleton className="h-6 w-32 mb-3" />
                                <Skeleton className="h-8 w-3/4 mb-4" />
                                <Skeleton className="h-5 w-48" />
                            </Card>
                            <Card className="bg-[#0f1729] border-[#334155] p-6">
                                <Skeleton className="h-5 w-28 mb-3" />
                                <Skeleton className="h-4 w-full mb-2" />
                                <Skeleton className="h-4 w-full mb-2" />
                                <Skeleton className="h-4 w-3/4" />
                            </Card>
                        </div>
                        <aside className="w-80 hidden md:block">
                            <Card className="bg-[#0f1729] border-[#334155] p-6">
                                <Skeleton className="h-4 w-24 mb-2" />
                                <Skeleton className="h-8 w-40 mb-4" />
                                <Skeleton className="h-12 w-full mb-4" />
                                <Skeleton className="h-4 w-full mb-2" />
                                <Skeleton className="h-4 w-full mb-2" />
                                <Skeleton className="h-4 w-full" />
                            </Card>
                        </aside>
                    </div>
                </div>
            </div>
        )
    }

    if (error || !data) {
        return (
            <div className="min-h-screen bg-[#1a1a2e]">
                <div className="max-w-6xl mx-auto px-4 py-8 text-center">
                    <h1 className="text-2xl font-bold text-white mb-4">
                        Necesidad no encontrada
                    </h1>
                    <p className="text-[#94a3b8] mb-6">
                        La necesidad que buscas no existe o ha sido eliminada.
                    </p>
                    <Link to={APP_ROUTES.landing.crowdsourcing.necesidades}>
                        <Button className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700">
                            Volver a explorar
                        </Button>
                    </Link>
                </div>
            </div>
        )
    }

    const ModalidadIcon = MODALIDAD_ICONS[data.modalidadTrabajoId] ?? Wifi
    const presupuestoText =
        data.presupuestoMin !== undefined && data.presupuestoMax !== undefined
            ? `${data.presupuestoMin} - ${data.presupuestoMax} ${data.monedaNombre ?? "EUR"}`
            : "Sin presupuesto definido"

    const renderCTA = () => {
        if (data.esPropietario) {
            return (
                <div className="bg-[#1e2a42] border border-[#334155] rounded-lg p-4 mb-4 flex items-center gap-2">
                    <Info className="w-5 h-5 text-[#94a3b8] flex-shrink-0" aria-hidden="true" />
                    <p className="text-sm text-[#94a3b8]">
                        Esta es tu necesidad
                    </p>
                </div>
            )
        }
        if (data.yaPropuso) {
            return (
                <div className="bg-green-900/20 border border-green-700 rounded-lg p-4 mb-4 flex items-center gap-2">
                    <CheckCircle className="w-5 h-5 text-green-400 flex-shrink-0" aria-hidden="true" />
                    <p className="text-sm text-green-300">
                        Ya enviaste una propuesta para esta necesidad
                    </p>
                </div>
            )
        }
        if (!data.tienePerfilProfesional) {
            return <PerfilProfesionalCTA className="mb-4" />
        }
        return (
            <Button
                className="w-full h-12 text-base font-semibold bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 mb-4"
                onClick={() => setIsFormOpen(true)}
                aria-label="Enviar propuesta para esta necesidad"
            >
                Enviar propuesta
            </Button>
        )
    }

    return (
        <div className="min-h-screen bg-[#1a1a2e]">
            <div className="max-w-6xl mx-auto px-4 py-8">
                <Link
                    to={APP_ROUTES.landing.crowdsourcing.necesidades}
                    className="flex items-center gap-2 text-[#94a3b8] hover:text-white transition-colors mb-6"
                >
                    <ArrowLeft className="w-4 h-4" aria-hidden="true" />
                    Explorar necesidades
                </Link>

                <div className="flex flex-col md:flex-row gap-8">
                    {/* Main content */}
                    <main className="flex-1 min-w-0">
                        {/* Header Card */}
                        <Card className="bg-[#0f1729] border-[#334155] p-6 mb-6">
                            <div className="flex items-center gap-2 mb-3 flex-wrap">
                                <Badge className="bg-purple-900/30 text-purple-300 border border-purple-700">
                                    {data.tipoNecesidadNombre}
                                </Badge>
                                <Badge
                                    variant="outline"
                                    className="border-[#334155] text-[#94a3b8] flex items-center gap-1"
                                >
                                    <ModalidadIcon className="w-3 h-3" aria-hidden="true" />
                                    {data.modalidadTrabajoNombre}
                                </Badge>
                                {data.fechaLimitePropuestas && (
                                    <UrgenciaBadge fechaLimite={data.fechaLimitePropuestas} />
                                )}
                            </div>
                            <h1 className="text-2xl md:text-3xl font-bold text-white mb-4 leading-tight">
                                {data.titulo}
                            </h1>
                            <div className="flex items-center gap-3">
                                <Avatar className="w-8 h-8">
                                    <AvatarImage
                                        src={data.artista.imagenUrl}
                                        alt={`${data.artista.nombreArtistico} avatar`}
                                    />
                                    <AvatarFallback className="bg-[#334155] text-[#94a3b8] text-xs">
                                        {data.artista.nombreArtistico.slice(0, 2).toUpperCase()}
                                    </AvatarFallback>
                                </Avatar>
                                <span className="text-sm font-medium text-[#94a3b8]">
                                    {data.artista.nombreArtistico}
                                </span>
                            </div>
                        </Card>

                        {/* Description Card */}
                        {data.descripcion && (
                            <Card className="bg-[#0f1729] border-[#334155] p-6 mb-6">
                                <h2 className="text-base font-semibold text-white mb-3">
                                    Descripcion
                                </h2>
                                <p className="text-base text-[#94a3b8] leading-relaxed whitespace-pre-line">
                                    {data.descripcion}
                                </p>
                            </Card>
                        )}

                        {/* Details Card */}
                        <Card className="bg-[#0f1729] border-[#334155] p-6 mb-6">
                            <h2 className="text-base font-semibold text-white mb-4">
                                Detalles
                            </h2>
                            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                                <div className="flex flex-col gap-1">
                                    <span className="text-xs text-[#64748b] uppercase tracking-wide">
                                        Tipo
                                    </span>
                                    <span className="text-sm font-medium text-white">
                                        {data.tipoNecesidadNombre}
                                    </span>
                                </div>
                                <div className="flex flex-col gap-1">
                                    <span className="text-xs text-[#64748b] uppercase tracking-wide">
                                        Modalidad
                                    </span>
                                    <span className="text-sm font-medium text-white">
                                        {data.modalidadTrabajoNombre}
                                    </span>
                                </div>
                                {(data.ubicacionCiudad || data.ubicacionPais) && (
                                    <div className="flex flex-col gap-1">
                                        <span className="text-xs text-[#64748b] uppercase tracking-wide">
                                            Ubicacion
                                        </span>
                                        <span className="text-sm font-medium text-white">
                                            {[data.ubicacionCiudad, data.ubicacionPais]
                                                .filter(Boolean)
                                                .join(", ")}
                                        </span>
                                    </div>
                                )}
                                {data.fechaInicioPrevista && (
                                    <div className="flex flex-col gap-1">
                                        <span className="text-xs text-[#64748b] uppercase tracking-wide">
                                            Inicio previsto
                                        </span>
                                        <span className="text-sm font-medium text-white">
                                            {formatFecha(data.fechaInicioPrevista)}
                                        </span>
                                    </div>
                                )}
                            </div>
                        </Card>
                    </main>

                    {/* Action Sidebar (Desktop) */}
                    <aside className="w-80 flex-shrink-0 hidden md:block">
                        <Card className="bg-[#0f1729] border-[#334155] p-6 sticky top-24">
                            <p className="text-xs text-[#64748b] uppercase tracking-wide mb-1">
                                Presupuesto
                            </p>
                            <p className="text-2xl font-bold text-white mb-4">
                                {presupuestoText}
                            </p>

                            {renderCTA()}

                            <Separator className="my-4 bg-[#334155]" />

                            <div className="space-y-3">
                                <div className="flex items-center justify-between">
                                    <span className="text-xs text-[#64748b] flex items-center gap-1">
                                        <Users className="w-3 h-3" aria-hidden="true" />
                                        Propuestas
                                    </span>
                                    <span className="text-sm font-medium text-white">
                                        {data.numeroPropuestas}
                                    </span>
                                </div>
                                <div className="flex items-center justify-between">
                                    <span className="text-xs text-[#64748b] flex items-center gap-1">
                                        <Clock className="w-3 h-3" aria-hidden="true" />
                                        Publicado
                                    </span>
                                    <span className="text-sm font-medium text-white">
                                        {formatFecha(data.fechaCreacion)}
                                    </span>
                                </div>
                                {data.fechaLimitePropuestas && (
                                    <div className="flex items-center justify-between">
                                        <span className="text-xs text-[#64748b] flex items-center gap-1">
                                            <Calendar className="w-3 h-3" aria-hidden="true" />
                                            Fecha limite
                                        </span>
                                        <span
                                            className={`text-sm font-medium ${getFechaLimiteColor(data.fechaLimitePropuestas)}`}
                                        >
                                            {formatFecha(data.fechaLimitePropuestas)}
                                        </span>
                                    </div>
                                )}
                                {data.fechaInicioPrevista && (
                                    <div className="flex items-center justify-between">
                                        <span className="text-xs text-[#64748b] flex items-center gap-1">
                                            <Calendar className="w-3 h-3" aria-hidden="true" />
                                            Inicio previsto
                                        </span>
                                        <span className="text-sm font-medium text-white">
                                            {formatFecha(data.fechaInicioPrevista)}
                                        </span>
                                    </div>
                                )}
                            </div>
                        </Card>
                    </aside>
                </div>

                {/* Mobile CTA Sticky Bottom */}
                <div className="fixed bottom-0 left-0 right-0 p-4 bg-[#0f1729] border-t border-[#334155] md:hidden z-50">
                    {renderCTA()}
                </div>
            </div>

            {/* Enviar Propuesta Dialog */}
            <EnviarPropuestaForm
                necesidadId={data.id}
                necesidadTitulo={data.titulo}
                presupuestoMin={data.presupuestoMin}
                presupuestoMax={data.presupuestoMax}
                monedaNombre={data.monedaNombre}
                isOpen={isFormOpen}
                onClose={() => setIsFormOpen(false)}
            />
        </div>
    )
}
