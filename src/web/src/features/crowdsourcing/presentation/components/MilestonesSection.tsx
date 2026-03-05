import { FC } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Plus, Target } from "lucide-react"
import { ESTADO_ACUERDO } from "@shared/constants"
import { ImporteAsignadoBar } from "./ImporteAsignadoBar"
import { MilestoneCard } from "./MilestoneCard"
import type { Milestone, Entregable, RolAcuerdo } from "../../domain"

interface MilestonesSectionProps {
    milestones: Milestone[]
    importeTotal: number
    importeAsignado: number
    porcentajeAsignado: number
    monedaNombre: string
    miRol: RolAcuerdo
    estadoAcuerdoId: number
    onAgregarMilestone: () => void
    onEditarMilestone: (milestone: Milestone) => void
    onEliminarMilestone: (milestoneId: string) => void
    onAprobarEntregable: (entregable: Entregable) => void
    onRechazarEntregable: (entregable: Entregable) => void
    onSubirEntregable: (milestoneId: string) => void
}

export const MilestonesSection: FC<MilestonesSectionProps> = ({
    milestones,
    importeTotal,
    importeAsignado,
    porcentajeAsignado,
    monedaNombre,
    miRol,
    estadoAcuerdoId,
    onAgregarMilestone,
    onEditarMilestone,
    onEliminarMilestone,
    onAprobarEntregable,
    onRechazarEntregable,
    onSubirEntregable,
}) => {
    const canAddMilestone =
        miRol === "Artista" && estadoAcuerdoId === ESTADO_ACUERDO.ACTIVO

    const sortedMilestones = [...milestones].sort((a, b) => a.orden - b.orden)

    return (
        <div className="space-y-4">
            <div className="flex items-center justify-between">
                <h3 className="text-white text-lg font-semibold flex items-center gap-2">
                    <Target className="h-5 w-5 text-blue-400" />
                    Milestones
                </h3>
                {canAddMilestone && (
                    <Button
                        size="sm"
                        variant="outline"
                        className="gap-1 border-blue-700 text-blue-400 hover:bg-blue-900/30"
                        onClick={onAgregarMilestone}
                    >
                        <Plus className="h-4 w-4" />
                        Agregar milestone
                    </Button>
                )}
            </div>

            {milestones.length > 0 && (
                <ImporteAsignadoBar
                    importeAsignado={importeAsignado}
                    importeTotal={importeTotal}
                    porcentaje={porcentajeAsignado}
                    monedaNombre={monedaNombre}
                />
            )}

            {sortedMilestones.length === 0 ? (
                <Card className="bg-[#0f1729] border-[#334155]">
                    <CardContent className="py-8 text-center">
                        <Target className="h-8 w-8 mx-auto text-slate-600 mb-2" />
                        <p className="text-sm text-slate-400">
                            No hay milestones definidos
                        </p>
                        <p className="text-xs text-slate-500 mt-1">
                            Los milestones son opcionales para organizar el trabajo
                        </p>
                    </CardContent>
                </Card>
            ) : (
                <div className="space-y-3">
                    {sortedMilestones.map((milestone) => (
                        <MilestoneCard
                            key={milestone.id}
                            milestone={milestone}
                            importeTotal={importeTotal}
                            monedaNombre={monedaNombre}
                            miRol={miRol}
                            estadoAcuerdoId={estadoAcuerdoId}
                            onEditar={onEditarMilestone}
                            onEliminar={onEliminarMilestone}
                            onAprobarEntregable={onAprobarEntregable}
                            onRechazarEntregable={onRechazarEntregable}
                            onSubirEntregable={onSubirEntregable}
                        />
                    ))}
                </div>
            )}
        </div>
    )
}
