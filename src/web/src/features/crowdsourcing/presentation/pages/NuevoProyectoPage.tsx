import { useSearchParams, useNavigate } from "react-router-dom"
import { Loader2 } from "lucide-react"
import { toast } from "sonner"
import { TooltipProvider } from "@/components/ui/tooltip"
import { Button } from "@/components/ui/button"
import { useTemplates } from "../../application/hooks/useTemplates"
import { useTemplateDetail } from "../../application/hooks/useTemplateDetail"
import { useGenerarNecesidades } from "../../application/hooks/useGenerarNecesidades"
import { useWizardState } from "../../application/hooks/useWizardState"
import { WizardStepper } from "../components/WizardStepper"
import { TemplateGallery } from "../components/TemplateGallery"
import { NecesidadList } from "../components/NecesidadList"
import { NecesidadListSkeleton } from "../components/NecesidadListSkeleton"
import { BudgetSummary } from "../components/BudgetSummary"
import { ConfirmationSummary } from "../components/ConfirmationSummary"
import { ProyectoSelector } from "../components/ProyectoSelector"
import type { GenerarNecesidadesRequest } from "../../domain"

function NuevoProyectoPage() {
    const [searchParams, setSearchParams] = useSearchParams()
    const navigate = useNavigate()

    const currentStep = Math.min(3, Math.max(1, parseInt(searchParams.get("step") || "1"))) as 1 | 2 | 3
    const templateId = searchParams.get("template") || ""

    const { data: templates, isLoading: loadingTemplates } = useTemplates()
    const { data: template, isLoading: loadingTemplate } = useTemplateDetail(templateId)
    const { mutate: generarNecesidades, isPending } = useGenerarNecesidades()

    const {
        selectedNecesidades,
        presupuestos,
        proyectoId,
        toggleNecesidad,
        updatePresupuesto,
        setProyectoId,
        getTotals,
    } = useWizardState(template)

    const totals = getTotals()

    const handleSelectTemplate = (id: string) => {
        setSearchParams({ step: "2", template: id })
    }

    const handleNext = () => {
        if (currentStep < 3) {
            setSearchParams({ step: String(currentStep + 1), template: templateId })
        }
    }

    const handleBack = () => {
        if (currentStep > 1) {
            setSearchParams({ step: String(currentStep - 1), template: templateId })
        }
    }

    const handleSubmit = () => {
        if (!templateId) return

        const payload: GenerarNecesidadesRequest = {
            proyectoArtisticoId: proyectoId || "",
            necesidadesSeleccionadas: Array.from(selectedNecesidades).map((id) => ({
                plantillaNecesidadId: id,
                presupuestoMin: presupuestos.get(id)?.min,
                presupuestoMax: presupuestos.get(id)?.max,
                monedaId: 1,
            })),
        }

        generarNecesidades(
            { templateId, data: payload },
            {
                onSuccess: (result) => {
                    toast.success(`${result.necesidadesCreadas} necesidades publicadas correctamente`)
                    navigate("/crowdsourcing/mis-necesidades")
                },
                onError: () => {
                    toast.error("Error al publicar necesidades. Intenta de nuevo.")
                },
            }
        )
    }

    return (
        <TooltipProvider>
            <div className="container mx-auto px-4 py-8">
                <WizardStepper currentStep={currentStep} />

                {currentStep === 1 && (
                    <>
                        <h1 className="text-3xl md:text-4xl font-bold text-white mb-2 text-center">
                            Selecciona tu tipo de proyecto
                        </h1>
                        <p className="text-lg text-[#94a3b8] mb-8 text-center">
                            Elige la plantilla que mejor se adapte a tus objetivos
                        </p>
                        <TemplateGallery
                            templates={templates ?? []}
                            isLoading={loadingTemplates}
                            onSelectTemplate={handleSelectTemplate}
                        />
                    </>
                )}

                {currentStep === 2 && (
                    <>
                        {loadingTemplate ? (
                            <NecesidadListSkeleton />
                        ) : template ? (
                            <>
                                <h1 className="text-2xl md:text-3xl font-bold text-white mb-2">
                                    {template.nombre}
                                </h1>
                                <p className="text-lg text-[#94a3b8] mb-8">
                                    Personaliza las necesidades de tu proyecto
                                </p>
                                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 lg:gap-8">
                                    <div className="lg:col-span-2">
                                        <NecesidadList
                                            necesidades={template.necesidades}
                                            selectedNecesidades={presupuestos}
                                            onToggleNecesidad={toggleNecesidad}
                                            onBudgetChange={updatePresupuesto}
                                        />
                                    </div>
                                    <div className="lg:col-span-1">
                                        <BudgetSummary {...totals} />
                                    </div>
                                </div>
                                <div className="flex justify-between mt-8">
                                    <Button
                                        variant="outline"
                                        onClick={handleBack}
                                        className="border-[#334155] text-white hover:bg-[#1e2a42]"
                                    >
                                        &larr; Atras
                                    </Button>
                                    <Button
                                        onClick={handleNext}
                                        disabled={selectedNecesidades.size === 0}
                                        className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold"
                                    >
                                        Siguiente &rarr;
                                    </Button>
                                </div>
                            </>
                        ) : (
                            <p className="text-center text-[#94a3b8]">
                                No se encontro la plantilla seleccionada
                            </p>
                        )}
                    </>
                )}

                {currentStep === 3 && template && (
                    <>
                        <h1 className="text-2xl md:text-3xl font-bold text-white mb-2 text-center">
                            Resumen de tu proyecto
                        </h1>
                        <p className="text-lg text-[#94a3b8] mb-8 text-center">
                            Revisa los detalles antes de publicar
                        </p>
                        <ConfirmationSummary
                            template={template}
                            selectedNecesidades={Array.from(selectedNecesidades)
                                .map((id) => template.necesidades.find((n) => n.id === id))
                                .filter(Boolean) as typeof template.necesidades}
                            presupuestos={presupuestos}
                            minTotal={totals.minTotal}
                            maxTotal={totals.maxTotal}
                        />
                        <ProyectoSelector
                            value={proyectoId}
                            onChange={setProyectoId}
                            proyectos={[]}
                        />
                        <div className="flex justify-between mt-8 max-w-4xl mx-auto">
                            <Button
                                variant="outline"
                                onClick={handleBack}
                                className="border-[#334155] text-white hover:bg-[#1e2a42]"
                            >
                                &larr; Atras
                            </Button>
                            <Button
                                onClick={handleSubmit}
                                disabled={isPending}
                                aria-busy={isPending}
                                className="bg-gradient-to-r from-pink-500 to-purple-600 hover:from-pink-600 hover:to-purple-700 text-white font-semibold px-8 py-3"
                            >
                                {isPending ? (
                                    <>
                                        <Loader2 className="animate-spin mr-2 h-4 w-4" />
                                        Publicando...
                                    </>
                                ) : (
                                    "Confirmar y publicar"
                                )}
                            </Button>
                        </div>
                    </>
                )}
            </div>
        </TooltipProvider>
    )
}

export default NuevoProyectoPage
