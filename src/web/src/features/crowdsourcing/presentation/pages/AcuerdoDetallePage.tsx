import { useState } from "react"
import { useParams, Link } from "react-router-dom"
import { Button } from "@/components/ui/button"
import { Separator } from "@/components/ui/separator"
import {
    ArrowLeft,
    CheckCircle2,
    XCircle,
    Upload,
    ShieldAlert,
    AlertCircle,
} from "lucide-react"
import { ESTADO_ACUERDO, APP_ROUTES } from "@shared/constants"
import { useAcuerdo } from "../../application/hooks/useAcuerdo"
import { AcuerdoCabecera } from "../components/AcuerdoCabecera"
import { MilestonesSection } from "../components/MilestonesSection"
import { EntregablesSinMilestone } from "../components/EntregablesSinMilestone"
import { AcuerdoTimeline } from "../components/AcuerdoTimeline"
import { ConversacionLink } from "../components/ConversacionLink"
import { MilestoneFormDialog } from "../components/MilestoneFormDialog"
import { SubirEntregableDialog } from "../components/SubirEntregableDialog"
import { AprobarEntregableDialog } from "../components/AprobarEntregableDialog"
import { RechazarEntregableDialog } from "../components/RechazarEntregableDialog"
import { CompletarAcuerdoDialog } from "../components/CompletarAcuerdoDialog"
import { CancelarAcuerdoDialog } from "../components/CancelarAcuerdoDialog"
import { ValoracionForm } from "../components/ValoracionForm"
import { ValoracionReadOnly } from "../components/ValoracionReadOnly"
import type { Milestone, Entregable, ValoracionCreatedResult } from "../../domain"

export default function AcuerdoDetallePage() {
    const { id } = useParams<{ id: string }>()
    const { data: acuerdo, isLoading, error } = useAcuerdo(id ?? "")

    // Dialog state
    const [milestoneDialogOpen, setMilestoneDialogOpen] = useState(false)
    const [milestoneToEdit, setMilestoneToEdit] = useState<Milestone | null>(null)
    const [entregableDialogOpen, setEntregableDialogOpen] = useState(false)
    const [entregableDialogMilestoneId, setEntregableDialogMilestoneId] =
        useState<string | undefined>(undefined)
    const [entregableToAprobar, setEntregableToAprobar] =
        useState<Entregable | null>(null)
    const [entregableToRechazar, setEntregableToRechazar] =
        useState<Entregable | null>(null)
    const [completarDialogOpen, setCompletarDialogOpen] = useState(false)
    const [cancelarDialogOpen, setCancelarDialogOpen] = useState(false)
    const [valoracionEnviada, setValoracionEnviada] =
        useState<ValoracionCreatedResult | null>(null)

    // Handlers
    const handleAgregarMilestone = () => {
        setMilestoneToEdit(null)
        setMilestoneDialogOpen(true)
    }

    const handleEditarMilestone = (milestone: Milestone) => {
        setMilestoneToEdit(milestone)
        setMilestoneDialogOpen(true)
    }

    const handleCloseMilestoneDialog = () => {
        setMilestoneDialogOpen(false)
        setMilestoneToEdit(null)
    }

    const handleSubirEntregable = (milestoneId?: string) => {
        setEntregableDialogMilestoneId(milestoneId)
        setEntregableDialogOpen(true)
    }

    // Loading state
    if (isLoading) {
        return (
            <div className="max-w-5xl mx-auto px-4 py-8 space-y-4">
                <div className="h-6 w-40 bg-slate-800 rounded animate-pulse" />
                <div className="h-48 bg-slate-800 rounded-lg animate-pulse" />
                <div className="h-64 bg-slate-800 rounded-lg animate-pulse" />
            </div>
        )
    }

    // Error states
    if (error) {
        const is403 = error.message === "3002" || error.message?.includes("403")
        return (
            <div className="max-w-5xl mx-auto px-4 py-16 text-center">
                {is403 ? (
                    <>
                        <ShieldAlert className="h-12 w-12 mx-auto text-amber-400 mb-4" />
                        <h2 className="text-xl font-semibold text-white mb-2">
                            Sin acceso
                        </h2>
                        <p className="text-slate-400 mb-6">
                            No tienes acceso a este acuerdo
                        </p>
                    </>
                ) : (
                    <>
                        <AlertCircle className="h-12 w-12 mx-auto text-red-400 mb-4" />
                        <h2 className="text-xl font-semibold text-white mb-2">
                            Acuerdo no encontrado
                        </h2>
                        <p className="text-slate-400 mb-6">
                            El acuerdo que buscas no existe o fue eliminado
                        </p>
                    </>
                )}
                <Link to={APP_ROUTES.landing.crowdsourcing.misPropuestas}>
                    <Button variant="outline" className="border-slate-600 text-white">
                        <ArrowLeft className="h-4 w-4 mr-2" />
                        Volver a mis propuestas
                    </Button>
                </Link>
            </div>
        )
    }

    if (!acuerdo) return null

    const isActivo = acuerdo.estadoAcuerdoId === ESTADO_ACUERDO.ACTIVO
    const isCompletado = acuerdo.estadoAcuerdoId === ESTADO_ACUERDO.COMPLETADO
    const isArtista = acuerdo.miRol === "Artista"
    const isProfesional = acuerdo.miRol === "Profesional"

    // Entregables sin milestone: collect from milestones that have empty milestone IDs
    // The API returns all entregables within milestones, and those without milestone
    // are in a separate array or within a "virtual" milestone. For now, we handle via
    // the response structure where entregables without milestone appear at top level.
    // Since the Acuerdo type has milestones[] and entregables are nested inside milestones,
    // entregables without a milestone would be those not inside any milestone.
    // The backend handles this by including entregables in milestones where they belong.
    // For the "sin milestone" section, we'd need a flat list, but since the current API
    // nests all entregables inside their milestones, this section may be empty initially.
    const entregablesSinMilestone: Entregable[] = []

    return (
        <div className="max-w-5xl mx-auto px-4 py-8">
            <Link
                to={APP_ROUTES.landing.crowdsourcing.necesidades}
                className="inline-flex items-center text-sm text-slate-400 hover:text-white mb-4"
            >
                <ArrowLeft className="h-4 w-4 mr-1" />
                Volver
            </Link>

            <div className="space-y-6">
                <AcuerdoCabecera acuerdo={acuerdo} />

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                    {/* Main content */}
                    <div className="lg:col-span-2 space-y-6">
                        <MilestonesSection
                            milestones={acuerdo.milestones}
                            importeTotal={acuerdo.importeTotalPactado}
                            importeAsignado={acuerdo.importeAsignado}
                            porcentajeAsignado={acuerdo.porcentajeAsignado}
                            monedaNombre={acuerdo.monedaNombre}
                            miRol={acuerdo.miRol}
                            estadoAcuerdoId={acuerdo.estadoAcuerdoId}
                            onAgregarMilestone={handleAgregarMilestone}
                            onEditarMilestone={handleEditarMilestone}
                            onEliminarMilestone={(milestoneId) => {
                                // Handled via useDeleteMilestone inside MilestoneCard
                                // This callback is for the section to forward the event
                            }}
                            onAprobarEntregable={(e) => setEntregableToAprobar(e)}
                            onRechazarEntregable={(e) => setEntregableToRechazar(e)}
                            onSubirEntregable={(milestoneId) =>
                                handleSubirEntregable(milestoneId)
                            }
                        />

                        <EntregablesSinMilestone
                            entregables={entregablesSinMilestone}
                            miRol={acuerdo.miRol}
                            estadoAcuerdoId={acuerdo.estadoAcuerdoId}
                            onAprobarEntregable={(e) => setEntregableToAprobar(e)}
                            onRechazarEntregable={(e) => setEntregableToRechazar(e)}
                        />

                        <AcuerdoTimeline timeline={acuerdo.timeline} />

                        {isCompletado && (
                            valoracionEnviada !== null ? (
                                <ValoracionReadOnly valoracion={valoracionEnviada} />
                            ) : (
                                <ValoracionForm
                                    acuerdoId={acuerdo.id}
                                    userIdValorado={
                                        acuerdo.miRol === "Artista"
                                            ? acuerdo.profesional.userId
                                            : acuerdo.artista.id
                                    }
                                    onSuccess={(valoracion) =>
                                        setValoracionEnviada(valoracion)
                                    }
                                />
                            )
                        )}
                    </div>

                    {/* Sidebar */}
                    <div className="space-y-4 lg:sticky lg:top-24 lg:self-start">
                        <ConversacionLink conversacionId={acuerdo.conversacionId} />

                        {isProfesional && isActivo && (
                            <Button
                                className="w-full gap-2 bg-blue-600 hover:bg-blue-700"
                                onClick={() => handleSubirEntregable()}
                            >
                                <Upload className="h-4 w-4" />
                                Subir entregable
                            </Button>
                        )}

                        {isActivo && (
                            <>
                                <Separator className="bg-slate-700" />

                                {isArtista && (
                                    <Button
                                        className="w-full gap-2 bg-green-600 hover:bg-green-700"
                                        onClick={() => setCompletarDialogOpen(true)}
                                    >
                                        <CheckCircle2 className="h-4 w-4" />
                                        Completar acuerdo
                                    </Button>
                                )}

                                <Button
                                    variant="outline"
                                    className="w-full gap-2 border-red-700 text-red-400 hover:bg-red-900/30"
                                    onClick={() => setCancelarDialogOpen(true)}
                                >
                                    <XCircle className="h-4 w-4" />
                                    Cancelar acuerdo
                                </Button>
                            </>
                        )}
                    </div>
                </div>
            </div>

            {/* Dialogs */}
            <MilestoneFormDialog
                acuerdoId={acuerdo.id}
                importeTotal={acuerdo.importeTotalPactado}
                importeYaAsignado={acuerdo.importeAsignado}
                monedaNombre={acuerdo.monedaNombre}
                fechaInicioAcuerdo={acuerdo.fechaInicio}
                milestoneToEdit={milestoneToEdit}
                isOpen={milestoneDialogOpen}
                onClose={handleCloseMilestoneDialog}
            />

            <SubirEntregableDialog
                acuerdoId={acuerdo.id}
                milestones={acuerdo.milestones}
                preselectedMilestoneId={entregableDialogMilestoneId}
                isOpen={entregableDialogOpen}
                onClose={() => {
                    setEntregableDialogOpen(false)
                    setEntregableDialogMilestoneId(undefined)
                }}
            />

            <AprobarEntregableDialog
                entregable={entregableToAprobar}
                acuerdoId={acuerdo.id}
                isOpen={!!entregableToAprobar}
                onClose={() => setEntregableToAprobar(null)}
            />

            <RechazarEntregableDialog
                entregable={entregableToRechazar}
                acuerdoId={acuerdo.id}
                isOpen={!!entregableToRechazar}
                onClose={() => setEntregableToRechazar(null)}
            />

            {acuerdo && (
                <CompletarAcuerdoDialog
                    acuerdo={acuerdo}
                    isOpen={completarDialogOpen}
                    onClose={() => setCompletarDialogOpen(false)}
                />
            )}

            <CancelarAcuerdoDialog
                acuerdoId={acuerdo.id}
                isOpen={cancelarDialogOpen}
                onClose={() => setCancelarDialogOpen(false)}
            />
        </div>
    )
}
