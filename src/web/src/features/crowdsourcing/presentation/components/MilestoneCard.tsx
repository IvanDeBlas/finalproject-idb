import { FC } from "react"
import { Card, CardContent } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import { CheckCircle2, Clock, Pencil, Trash2, Upload } from "lucide-react"
import { ESTADO_ACUERDO } from "@shared/constants"
import { EntregableItem } from "./EntregableItem"
import type { Milestone, Entregable, RolAcuerdo } from "../../domain"

interface MilestoneCardProps {
    milestone: Milestone
    importeTotal: number
    monedaNombre: string
    miRol: RolAcuerdo
    estadoAcuerdoId: number
    onEditar?: (milestone: Milestone) => void
    onEliminar?: (milestoneId: string) => void
    onAprobarEntregable: (entregable: Entregable) => void
    onRechazarEntregable: (entregable: Entregable) => void
    onSubirEntregable: (milestoneId: string) => void
}

function formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString("es-ES", {
        day: "2-digit",
        month: "short",
        year: "numeric",
    })
}

export const MilestoneCard: FC<MilestoneCardProps> = ({
    milestone,
    importeTotal,
    monedaNombre,
    miRol,
    estadoAcuerdoId,
    onEditar,
    onEliminar,
    onAprobarEntregable,
    onRechazarEntregable,
    onSubirEntregable,
}) => {
    const isCompleted = !!milestone.fechaCompletado
    const isAcuerdoActivo = estadoAcuerdoId === ESTADO_ACUERDO.ACTIVO
    const isArtista = miRol === "Artista"
    const isProfesional = miRol === "Profesional"

    const canEdit = isArtista && isAcuerdoActivo && !isCompleted
    const canDelete =
        isArtista &&
        isAcuerdoActivo &&
        !isCompleted &&
        milestone.entregables.length === 0
    const canUpload = isProfesional && isAcuerdoActivo

    const porcentaje = importeTotal > 0
        ? Math.round((milestone.importeParcial / importeTotal) * 100)
        : 0

    return (
        <Card className="bg-[#0f1729] border-[#334155]">
            <CardContent className="pt-4 space-y-3">
                <div className="flex items-start justify-between gap-2">
                    <div className="flex-1 min-w-0">
                        <div className="flex items-center gap-2 flex-wrap">
                            <h4 className="text-sm font-medium text-white">
                                {milestone.titulo}
                            </h4>
                            {isCompleted ? (
                                <Badge className="bg-green-900/30 border border-green-600 text-green-300 text-xs px-2 py-0 rounded-full">
                                    <CheckCircle2 className="h-3 w-3 mr-1" />
                                    Completado
                                </Badge>
                            ) : (
                                <Badge className="bg-slate-800 border border-slate-600 text-slate-300 text-xs px-2 py-0 rounded-full">
                                    <Clock className="h-3 w-3 mr-1" />
                                    Pendiente
                                </Badge>
                            )}
                        </div>
                        {milestone.descripcion && (
                            <p className="text-xs text-slate-400 mt-1 line-clamp-2">
                                {milestone.descripcion}
                            </p>
                        )}
                    </div>
                    <div className="flex items-center gap-1 shrink-0">
                        {canEdit && (
                            <Button
                                size="sm"
                                variant="ghost"
                                className="h-7 w-7 p-0 text-slate-400 hover:text-white"
                                onClick={() => onEditar?.(milestone)}
                                aria-label="Editar milestone"
                            >
                                <Pencil className="h-3.5 w-3.5" />
                            </Button>
                        )}
                        {canDelete && (
                            <Button
                                size="sm"
                                variant="ghost"
                                className="h-7 w-7 p-0 text-slate-400 hover:text-red-400"
                                onClick={() => {
                                    if (window.confirm("Eliminar este milestone?")) {
                                        onEliminar?.(milestone.id)
                                    }
                                }}
                                aria-label="Eliminar milestone"
                            >
                                <Trash2 className="h-3.5 w-3.5" />
                            </Button>
                        )}
                    </div>
                </div>

                <div className="flex items-center gap-4 text-xs text-slate-400">
                    <span>
                        {milestone.importeParcial.toLocaleString("es-ES", { minimumFractionDigits: 2 })}{" "}
                        {monedaNombre} ({porcentaje}%)
                    </span>
                    {milestone.fechaLimite && (
                        <span>Limite: {formatDate(milestone.fechaLimite)}</span>
                    )}
                    {milestone.fechaCompletado && (
                        <span>Completado: {formatDate(milestone.fechaCompletado)}</span>
                    )}
                </div>

                {milestone.entregables.length > 0 && (
                    <>
                        <Separator className="bg-slate-700" />
                        <div>
                            <p className="text-xs font-medium text-slate-400 mb-1">
                                Entregables ({milestone.entregables.length})
                            </p>
                            {milestone.entregables.map((entregable, index) => (
                                <div key={entregable.id}>
                                    {index > 0 && (
                                        <Separator className="bg-slate-800" />
                                    )}
                                    <EntregableItem
                                        entregable={entregable}
                                        miRol={miRol}
                                        estadoAcuerdoId={estadoAcuerdoId}
                                        onAprobar={onAprobarEntregable}
                                        onRechazar={onRechazarEntregable}
                                    />
                                </div>
                            ))}
                        </div>
                    </>
                )}

                {milestone.entregables.length === 0 && (
                    <>
                        <Separator className="bg-slate-700" />
                        <p className="text-xs text-slate-500 italic">
                            Sin entregables
                        </p>
                    </>
                )}

                {canUpload && (
                    <Button
                        size="sm"
                        variant="outline"
                        className="h-7 text-xs gap-1 border-slate-600 text-slate-300 hover:bg-slate-800"
                        onClick={() => onSubirEntregable(milestone.id)}
                    >
                        <Upload className="h-3 w-3" />
                        Subir entregable
                    </Button>
                )}
            </CardContent>
        </Card>
    )
}
