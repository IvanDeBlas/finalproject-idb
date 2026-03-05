"use client"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Pencil, Info } from "lucide-react"
import { TIPO_PROMO_LABELS, TIPO_EVENTO_PROMO_LABELS, TIPO_REWARD_PROMO_LABELS } from "@shared/constants"
import type { CreatePromoProgramaFormData } from "@shared/schemas/crowdpromotion.schema"

interface RevisarPublicarStepProps {
    formData: Partial<CreatePromoProgramaFormData>
    onEditarPaso: (paso: number) => void
    isSubmitting: boolean
    mode: "create" | "edit"
}

function Value({ children }: { children: React.ReactNode }) {
    return children ? (
        <span className="text-sm text-white">{children}</span>
    ) : (
        <span className="text-sm text-zinc-500 italic">--</span>
    )
}

export function RevisarPublicarStep({
    formData,
    onEditarPaso,
    mode,
}: RevisarPublicarStepProps) {
    const tareas = (formData.tareas ?? []) as Array<{
        titulo: string
        tipoEventoPromoId: number
        tipoRewardId: number
        importeRecompensa?: number
        puntosRecompensa?: number
        esRepetible: boolean
    }>

    return (
        <div className="space-y-6 max-w-2xl mx-auto">
            <div className="flex items-center gap-2 text-xs text-zinc-500 bg-[#1e1e38] rounded-lg p-3">
                <Info className="h-4 w-4 flex-shrink-0" />
                <span>
                    {mode === "create"
                        ? "Al publicar, el programa se activara y sera visible para los promotores."
                        : "Los cambios se guardaran inmediatamente."}
                </span>
            </div>

            {/* Datos del programa */}
            <Card className="bg-[#1a1a2e] border-zinc-800">
                <CardHeader className="flex flex-row items-center justify-between pb-3">
                    <CardTitle className="text-sm text-white">Datos del programa</CardTitle>
                    <Button variant="ghost" size="sm" onClick={() => onEditarPaso(1)}>
                        <Pencil className="h-3.5 w-3.5 mr-1" />
                        Editar
                    </Button>
                </CardHeader>
                <CardContent className="space-y-3">
                    <div className="grid grid-cols-2 gap-3 text-sm">
                        <div>
                            <span className="text-xs text-zinc-500">Titulo</span>
                            <p className="text-white">{formData.titulo || <span className="text-zinc-500 italic">--</span>}</p>
                        </div>
                        <div>
                            <span className="text-xs text-zinc-500">Tipo</span>
                            <p className="text-white">
                                {formData.tipoPromoId ? TIPO_PROMO_LABELS[formData.tipoPromoId] : <span className="text-zinc-500 italic">--</span>}
                            </p>
                        </div>
                    </div>
                    {formData.descripcion && (
                        <div>
                            <span className="text-xs text-zinc-500">Descripcion</span>
                            <p className="text-sm text-zinc-300">{formData.descripcion}</p>
                        </div>
                    )}
                    <div className="grid grid-cols-2 gap-3 text-sm">
                        <div>
                            <span className="text-xs text-zinc-500">Periodo</span>
                            <Value>
                                {formData.fechaInicio || formData.fechaFin
                                    ? `${formData.fechaInicio || "?"} - ${formData.fechaFin || "?"}`
                                    : null}
                            </Value>
                        </div>
                        <div>
                            <span className="text-xs text-zinc-500">Codigo tracking</span>
                            <Value>{formData.codigoTrackingBase || null}</Value>
                        </div>
                    </div>
                </CardContent>
            </Card>

            {/* Comisiones */}
            <Card className="bg-[#1a1a2e] border-zinc-800">
                <CardHeader className="flex flex-row items-center justify-between pb-3">
                    <CardTitle className="text-sm text-white">Comisiones</CardTitle>
                    <Button variant="ghost" size="sm" onClick={() => onEditarPaso(2)}>
                        <Pencil className="h-3.5 w-3.5 mr-1" />
                        Editar
                    </Button>
                </CardHeader>
                <CardContent>
                    <div className="grid grid-cols-3 gap-3 text-sm">
                        <div>
                            <span className="text-xs text-zinc-500">Moneda</span>
                            <Value>{formData.monedaId === 1 ? "EUR" : formData.monedaId === 2 ? "USD" : null}</Value>
                        </div>
                        <div>
                            <span className="text-xs text-zinc-500">Comision %</span>
                            <Value>{formData.importeComisionPorcentaje != null ? `${formData.importeComisionPorcentaje}%` : null}</Value>
                        </div>
                        <div>
                            <span className="text-xs text-zinc-500">Comision fija</span>
                            <Value>{formData.importeComisionFija != null ? `${formData.importeComisionFija}` : null}</Value>
                        </div>
                    </div>
                </CardContent>
            </Card>

            {/* Tareas */}
            <Card className="bg-[#1a1a2e] border-zinc-800">
                <CardHeader className="flex flex-row items-center justify-between pb-3">
                    <CardTitle className="text-sm text-white">Tareas ({tareas.length})</CardTitle>
                    <Button variant="ghost" size="sm" onClick={() => onEditarPaso(3)}>
                        <Pencil className="h-3.5 w-3.5 mr-1" />
                        Editar
                    </Button>
                </CardHeader>
                <CardContent>
                    {tareas.length === 0 ? (
                        <p className="text-sm text-zinc-500 italic">Sin tareas definidas</p>
                    ) : (
                        <div className="space-y-2">
                            {tareas.map((tarea, index) => (
                                <div key={index} className="flex items-center gap-2 text-sm p-2 rounded bg-[#0d0d1a]">
                                    <span className="text-white">{tarea.titulo}</span>
                                    <Badge variant="outline" className="text-xs border-blue-500/30 text-blue-400">
                                        {TIPO_EVENTO_PROMO_LABELS[tarea.tipoEventoPromoId]}
                                    </Badge>
                                    <Badge variant="outline" className="text-xs border-amber-500/30 text-amber-400">
                                        {TIPO_REWARD_PROMO_LABELS[tarea.tipoRewardId]}
                                    </Badge>
                                </div>
                            ))}
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    )
}
