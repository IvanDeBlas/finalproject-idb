/* eslint-disable @next/next/no-img-element */
"use client"

import { Pencil, Send, Trash2, Music, Calendar, Target, Users, Heart } from "lucide-react"
import {
    Card,
    CardContent,
    Button,
    Badge,
    Progress,
} from "@/components/ui"
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs"
import { Separator } from "@/components/ui/separator"
import { DraftBanner } from "./DraftBanner"
import type { Campania } from "@shared/types"
import {
    TIPO_FINANCIACION_LABELS,
    CAMPANIA_ESTADOS,
} from "@shared/constants"
import {
    calculatePercentage,
    formatCurrencyWithSymbol,
    formatDate,
    getDaysRemaining,
} from "@shared/utils/format"
import { CampaniaStatusBadge } from "../list/CampaniaStatusBadge"

interface PreviewLayoutProps {
    campania: Campania
    onEdit: () => void
    onPublish: () => void
    onDelete: () => void
    onViewBackings?: () => void
    showDraftBanner: boolean
}

export function PreviewLayout({
    campania,
    onEdit,
    onPublish,
    onDelete,
    onViewBackings,
    showDraftBanner,
}: PreviewLayoutProps) {
    const porcentaje = calculatePercentage(
        campania.importePledgedActual,
        campania.importeObjetivo
    )
    const diasRestantes = campania.fechaFin
        ? getDaysRemaining(campania.fechaFin)
        : 0
    const isBorrador = campania.estadoCampaniaId === CAMPANIA_ESTADOS.BORRADOR

    return (
        <div className="space-y-4">
            {/* Draft Banner */}
            {showDraftBanner && <DraftBanner />}

            {/* Action bar */}
            {isBorrador && (
                <div className="flex flex-wrap gap-3 mb-6">
                    <Button
                        variant="outline"
                        onClick={onEdit}
                        className="border-border text-foreground"
                    >
                        <Pencil className="mr-2 h-4 w-4" />
                        Editar
                    </Button>
                    <Button
                        onClick={onPublish}
                        className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white"
                    >
                        <Send className="mr-2 h-4 w-4" />
                        Publicar Ahora
                    </Button>
                    <Button
                        variant="outline"
                        onClick={onDelete}
                        className="border-destructive/50 text-destructive hover:bg-destructive/10"
                    >
                        <Trash2 className="mr-2 h-4 w-4" />
                        Eliminar
                    </Button>
                </div>
            )}

            {/* View Backings */}
            {!isBorrador && onViewBackings && (
                <div className="flex flex-wrap gap-3 mb-6">
                    <Button
                        variant="outline"
                        onClick={onViewBackings}
                        className="border-primary text-primary hover:bg-primary/10"
                    >
                        <Heart className="mr-2 h-4 w-4" />
                        Ver Apoyos Recibidos
                    </Button>
                </div>
            )}

            {/* Hero Image */}
            {campania.imagenPrincipalUrl ? (
                <img
                    src={campania.imagenPrincipalUrl}
                    alt={campania.titulo}
                    className="w-full h-64 md:h-96 object-cover rounded-lg"
                />
            ) : (
                <div className="w-full h-64 md:h-96 rounded-lg bg-muted/20 flex items-center justify-center">
                    <Music className="w-16 h-16 text-muted-foreground" />
                </div>
            )}

            {/* Content Grid */}
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                {/* Main Content */}
                <div className="lg:col-span-2 space-y-6">
                    {/* Title and Status */}
                    <div>
                        <div className="flex items-start gap-3 mb-2">
                            <h1 className="text-3xl md:text-4xl font-bold text-foreground">
                                {campania.titulo}
                            </h1>
                        </div>
                        {campania.subtitulo && (
                            <p className="text-lg text-muted-foreground mb-3">
                                {campania.subtitulo}
                            </p>
                        )}
                        <div className="flex items-center gap-3">
                            <CampaniaStatusBadge
                                estadoId={campania.estadoCampaniaId}
                            />
                            {campania.tipoFinanciacionId && (
                                <Badge variant="outline">
                                    {
                                        TIPO_FINANCIACION_LABELS[
                                            campania.tipoFinanciacionId
                                        ]
                                    }
                                </Badge>
                            )}
                        </div>
                    </div>

                    {/* Funding Progress */}
                    <Card className="bg-card border-border">
                        <CardContent className="p-6">
                            <div className="text-3xl font-bold text-foreground mb-2">
                                {formatCurrencyWithSymbol(
                                    campania.importePledgedActual,
                                    campania.monedaId
                                )}
                                <span className="text-lg font-normal text-muted-foreground ml-2">
                                    de{" "}
                                    {formatCurrencyWithSymbol(
                                        campania.importeObjetivo,
                                        campania.monedaId
                                    )}
                                </span>
                            </div>

                            <Progress
                                value={porcentaje}
                                className="h-3 mb-4"
                            />

                            <div className="flex items-center gap-6 text-sm text-muted-foreground">
                                <div className="flex items-center gap-1">
                                    <Target className="w-4 h-4" />
                                    <span>{porcentaje}% financiado</span>
                                </div>
                                {campania.fechaFin && (
                                    <div className="flex items-center gap-1">
                                        <Calendar className="w-4 h-4" />
                                        <span>
                                            {diasRestantes} dias restantes
                                        </span>
                                    </div>
                                )}
                                <div className="flex items-center gap-1">
                                    <Users className="w-4 h-4" />
                                    <span>0 backers</span>
                                </div>
                            </div>
                        </CardContent>
                    </Card>

                    {/* Tabs */}
                    <Tabs defaultValue="historia">
                        <TabsList className="w-full justify-start bg-transparent border-b border-border rounded-none h-auto p-0">
                            <TabsTrigger
                                value="historia"
                                className="rounded-none border-b-2 border-transparent data-[state=active]:border-primary data-[state=active]:bg-transparent data-[state=active]:shadow-none px-6 py-3"
                            >
                                Historia
                            </TabsTrigger>
                            <TabsTrigger
                                value="actualizaciones"
                                className="rounded-none border-b-2 border-transparent data-[state=active]:border-primary data-[state=active]:bg-transparent data-[state=active]:shadow-none px-6 py-3"
                            >
                                Actualizaciones
                            </TabsTrigger>
                            <TabsTrigger
                                value="comentarios"
                                className="rounded-none border-b-2 border-transparent data-[state=active]:border-primary data-[state=active]:bg-transparent data-[state=active]:shadow-none px-6 py-3"
                            >
                                Comentarios
                            </TabsTrigger>
                        </TabsList>

                        <TabsContent value="historia" className="pt-6">
                            {campania.descripcionCorta ? (
                                <div className="prose prose-invert max-w-none text-muted-foreground whitespace-pre-wrap">
                                    {campania.descripcionCorta}
                                </div>
                            ) : (
                                <p className="text-muted-foreground italic">
                                    Sin descripcion disponible
                                </p>
                            )}
                        </TabsContent>

                        <TabsContent
                            value="actualizaciones"
                            className="pt-6"
                        >
                            <p className="text-muted-foreground text-sm">
                                No hay actualizaciones por el momento.
                            </p>
                        </TabsContent>

                        <TabsContent value="comentarios" className="pt-6">
                            <p className="text-muted-foreground text-sm">
                                No hay comentarios por el momento.
                            </p>
                        </TabsContent>
                    </Tabs>
                </div>

                {/* Sidebar */}
                <div className="space-y-4">
                    <Card className="bg-card border-border sticky top-24">
                        <CardContent className="p-6 space-y-4">
                            <Button
                                className="w-full bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold py-3"
                                disabled={isBorrador}
                            >
                                Apoyar esta campania
                            </Button>

                            {isBorrador && (
                                <p className="text-xs text-center text-muted-foreground">
                                    Publica tu campania para recibir apoyos
                                </p>
                            )}

                            <Separator />

                            <div>
                                <h3 className="font-semibold text-foreground mb-3">
                                    Recompensas
                                </h3>
                                <p className="text-sm text-muted-foreground">
                                    No hay recompensas disponibles aun.
                                </p>
                            </div>

                            <Separator />

                            <div className="space-y-2 text-sm text-muted-foreground">
                                {campania.fechaFin && (
                                    <p>
                                        Finaliza:{" "}
                                        {formatDate(campania.fechaFin)}
                                    </p>
                                )}
                                <p>
                                    Creada: {formatDate(campania.fechaCreacion)}
                                </p>
                            </div>
                        </CardContent>
                    </Card>
                </div>
            </div>
        </div>
    )
}
