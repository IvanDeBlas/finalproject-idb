"use client"

import { useRouter } from "next/navigation"
import { toast } from "sonner"
import { useWizardState } from "@/hooks/use-wizard-state"
import type { TemplateData } from "@/hooks/use-wizard-state"
import { useCreateCampania, useUpdateCampania } from "@/hooks/use-campanias"
import { useGenerateNecesidades } from "@/hooks/use-templates-mutations"
import { useTemplate } from "@/hooks/use-templates"
import { getCampaniaErrorMessage } from "@shared/utils/error-messages"
import type { CreateCampaniaFormData } from "@shared/schemas"
import type { CreateCampaniaRequest, UpdateCampaniaRequest } from "@shared/types"
import { WizardStepper } from "./WizardStepper"
import { TemplateStep } from "./TemplateStep"
import { BasicInfoStep } from "./BasicInfoStep"
import { StoryStep } from "./StoryStep"
import { RewardsStep } from "./RewardsStep"
import { ReviewStep } from "./ReviewStep"

interface WizardContainerProps {
    mode: "create" | "edit"
    campaniaId?: string
    initialData?: Partial<CreateCampaniaFormData>
}

export function WizardContainer({
    mode,
    campaniaId,
    initialData,
}: WizardContainerProps) {
    const router = useRouter()
    const {
        currentStep,
        formData,
        templateData,
        completedSteps,
        goToStep,
        updateFormData,
        updateTemplateData,
        markStepCompleted,
        resetWizard,
    } = useWizardState(initialData)

    const createMutation = useCreateCampania()
    const updateMutation = useUpdateCampania()
    const generateMutation = useGenerateNecesidades()

    const { data: selectedTemplate } = useTemplate(templateData?.templateId || "")

    const handleTemplateNext = (data: TemplateData | null) => {
        updateTemplateData(data)
        markStepCompleted(1)
        goToStep(2)
    }

    const handleStepComplete = (
        step: number,
        data: Partial<CreateCampaniaFormData>
    ) => {
        updateFormData(data)
        goToStep(step + 1)
    }

    const handleRewardsNext = () => {
        markStepCompleted(4)
        goToStep(5)
    }

    const handleSubmit = async () => {
        try {
            if (mode === "create") {
                const requestData: CreateCampaniaRequest = {
                    titulo: formData.titulo || "",
                    importeObjetivo: formData.importeObjetivo || 0,
                    monedaId: formData.monedaId || 1,
                    tipoFinanciacionId: formData.tipoFinanciacionId || 1,
                    permiteAportacionesAnonimas:
                        formData.permiteAportacionesAnonimas || false,
                    permitePropinas: formData.permitePropinas || false,
                    subtitulo: formData.subtitulo || undefined,
                    descripcionCorta: formData.descripcionCorta || undefined,
                    imagenPrincipalUrl:
                        formData.imagenPrincipalUrl || undefined,
                    videoPrincipalUrl:
                        formData.videoPrincipalUrl || undefined,
                    fechaFin: formData.fechaFin || undefined,
                    fechaInicio: formData.fechaInicio || undefined,
                    proyectoArtisticoId:
                        formData.proyectoArtisticoId || undefined,
                }

                const result = await createMutation.mutateAsync(requestData)

                if (templateData?.templateId && selectedTemplate?.necesidades) {
                    try {
                        await generateMutation.mutateAsync({
                            templateId: templateData.templateId,
                            data: {
                                proyectoArtisticoId: result.id,
                                necesidadesSeleccionadas:
                                    selectedTemplate.necesidades.map((n) => ({
                                        plantillaNecesidadId: n.id,
                                        presupuestoMin: n.precioMinOrientativo,
                                        presupuestoMax: n.precioMaxOrientativo,
                                        monedaId: n.moneda,
                                    })),
                            },
                        })
                    } catch {
                        // Error toast is handled by the mutation hook
                    }
                }

                resetWizard()
                toast.success("Campania creada exitosamente", {
                    description: "Redirigiendo a vista previa...",
                })
                router.push(`/campanias/${result.id}`)
            } else if (campaniaId) {
                const updateData: UpdateCampaniaRequest = {
                    id: campaniaId,
                    titulo: formData.titulo || undefined,
                    subtitulo: formData.subtitulo || undefined,
                    descripcionCorta: formData.descripcionCorta || undefined,
                    imagenPrincipalUrl:
                        formData.imagenPrincipalUrl || undefined,
                    videoPrincipalUrl:
                        formData.videoPrincipalUrl || undefined,
                    importeObjetivo: formData.importeObjetivo || undefined,
                    tipoFinanciacionId:
                        formData.tipoFinanciacionId || undefined,
                    fechaFin: formData.fechaFin || undefined,
                }

                await updateMutation.mutateAsync({
                    id: campaniaId,
                    data: updateData,
                })
                resetWizard()
                toast.success("Campania actualizada exitosamente")
                router.push(`/campanias/${campaniaId}`)
            }
        } catch (error: unknown) {
            const axiosError = error as {
                response?: {
                    data?: {
                        messages?: Array<{ errorCode: string }>
                    }
                }
            }
            if (axiosError?.response?.data?.messages?.[0]?.errorCode) {
                toast.error(
                    getCampaniaErrorMessage(
                        axiosError.response.data.messages[0].errorCode
                    )
                )
            } else {
                toast.error("Error inesperado. Por favor, intenta nuevamente")
            }
        }
    }

    const isPending =
        createMutation.isPending ||
        updateMutation.isPending ||
        generateMutation.isPending

    return (
        <div className="max-w-4xl mx-auto py-8 px-4 sm:px-6">
            <div className="text-center mb-8">
                <h1 className="text-3xl font-bold text-foreground mb-2">
                    {mode === "create"
                        ? "Crear Nueva Campania"
                        : "Editar Campania"}
                </h1>
                <p className="text-muted-foreground">
                    Completa los pasos para publicar tu proyecto musical
                </p>
            </div>

            <WizardStepper
                currentStep={currentStep}
                totalSteps={5}
                completedSteps={completedSteps}
                onStepClick={goToStep}
            />

            <div className="transition-all duration-300">
                {currentStep === 1 && (
                    <TemplateStep
                        selectedTemplateId={templateData?.templateId}
                        onNext={handleTemplateNext}
                    />
                )}

                {currentStep === 2 && (
                    <BasicInfoStep
                        defaultValues={formData}
                        onNext={(data) => handleStepComplete(2, data)}
                        onBack={() => goToStep(1)}
                        isSubmitting={false}
                    />
                )}

                {currentStep === 3 && (
                    <StoryStep
                        defaultValues={formData}
                        onNext={(data) => handleStepComplete(3, data)}
                        onBack={() => goToStep(2)}
                        isSubmitting={false}
                    />
                )}

                {currentStep === 4 && (
                    <RewardsStep
                        campaniaId={campaniaId}
                        onNext={handleRewardsNext}
                        onBack={() => goToStep(3)}
                    />
                )}

                {currentStep === 5 && (
                    <ReviewStep
                        formData={formData}
                        onSubmit={handleSubmit}
                        onBack={() => goToStep(4)}
                        onEdit={goToStep}
                        isSubmitting={isPending}
                    />
                )}
            </div>
        </div>
    )
}
