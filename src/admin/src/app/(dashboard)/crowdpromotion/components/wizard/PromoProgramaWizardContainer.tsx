"use client"

import { useState, useMemo } from "react"
import { useRouter } from "next/navigation"
import { toast } from "sonner"
import { usePromoProgramaWizardState } from "@/hooks/use-promo-programa-wizard-state"
import { usePromoPrograma } from "@/hooks/use-promo-programas"
import { useCreatePromoPrograma, useUpdatePromoPrograma } from "@/hooks/use-promo-programas-mutations"
import { PromoProgramaWizardHeader } from "./PromoProgramaWizardHeader"
import { PromoProgramaWizardStepper } from "./PromoProgramaWizardStepper"
import { PromoProgramaWizardFooter } from "./PromoProgramaWizardFooter"
import { PromoProgramaAbandonDialog } from "./PromoProgramaAbandonDialog"
import { DatosBasicosStep } from "./steps/DatosBasicosStep"
import { ComisionesStep } from "./steps/ComisionesStep"
import { TareasStep } from "./steps/TareasStep"
import { RevisarPublicarStep } from "./steps/RevisarPublicarStep"
import { APP_ROUTES } from "@shared/constants"
import { getPromoProgramaErrorMessage } from "@shared/utils/error-messages"
import type { CreatePromoProgramaFormData } from "@shared/schemas/crowdpromotion.schema"
import type { CreatePromoProgramaRequest, CreatePromoTareaItem } from "@shared/types"
import type { AxiosError } from "axios"

interface PromoProgramaWizardContainerProps {
    mode: "create" | "edit"
    programaId?: string
}

interface ErrorResponseData {
    messages?: Array<{ errorCode?: string; message?: string }>
}

export function PromoProgramaWizardContainer({ mode, programaId }: PromoProgramaWizardContainerProps) {
    const router = useRouter()
    const [showAbandonDialog, setShowAbandonDialog] = useState(false)

    const { data: existingPrograma, isLoading: isLoadingPrograma } = usePromoPrograma(
        mode === "edit" ? (programaId ?? "") : ""
    )

    const initialData = useMemo(() => {
        if (mode !== "edit" || !existingPrograma) return undefined
        const detail = existingPrograma
        return {
            titulo: detail.titulo,
            descripcion: detail.descripcion ?? "",
            tipoPromoId: detail.tipoPromoId,
            campaniaCrowdfundingId: detail.campaniaCrowdfundingId ?? "",
            proyectoArtisticoId: detail.proyectoArtisticoId ?? "",
            urlLanding: detail.urlLanding ?? "",
            codigoTrackingBase: detail.codigoTrackingBase ?? "",
            monedaId: detail.monedaId,
            importeComisionPorcentaje: detail.importeComisionPorcentaje ?? undefined,
            importeComisionFija: detail.importeComisionFija ?? undefined,
            fechaInicio: detail.fechaInicio ?? "",
            fechaFin: detail.fechaFin ?? "",
            tareas: detail.tareas.map((t) => ({
                titulo: t.titulo,
                descripcion: t.descripcion ?? "",
                tipoEventoPromoId: t.tipoEventoPromoId,
                tipoRewardId: t.tipoRewardId,
                importeRecompensa: t.importeRecompensa ?? undefined,
                monedaId: t.monedaId ?? undefined,
                puntosRecompensa: t.puntosRecompensa ?? undefined,
                urlInstrucciones: t.urlInstrucciones ?? "",
                esRepetible: t.esRepetible,
                maxRepeticiones: t.maxRepeticiones ?? undefined,
                fechaInicio: t.fechaInicio ?? "",
                fechaFin: t.fechaFin ?? "",
            })),
        } as Partial<CreatePromoProgramaFormData>
    }, [mode, existingPrograma])

    const {
        pasoActual,
        formData,
        pasosCompletados,
        isHydrated,
        irAPaso,
        actualizarFormData,
        actualizarTareas,
        marcarPasoCompletado,
        resetWizard,
    } = usePromoProgramaWizardState(initialData)

    const createMutation = useCreatePromoPrograma()
    const updateMutation = useUpdatePromoPrograma()
    const isSubmitting = createMutation.isPending || updateMutation.isPending

    const tienePromotores = mode === "edit" && (existingPrograma?.promotores?.length ?? 0) > 0

    const handleStepNext = (data: Partial<CreatePromoProgramaFormData>) => {
        actualizarFormData(data)
        irAPaso(pasoActual + 1)
    }

    const handleTareasChange = (tareas: CreatePromoTareaItem[]) => {
        actualizarTareas(tareas)
    }

    const handleSiguiente = () => {
        if (pasoActual === 3) {
            marcarPasoCompletado(3)
            irAPaso(4)
        } else if (pasoActual === 4) {
            handleSubmit()
        }
    }

    const handleAnterior = () => {
        if (pasoActual > 1) {
            irAPaso(pasoActual - 1)
        }
    }

    const handleSubmit = async () => {
        const tareas = (formData.tareas ?? []) as CreatePromoTareaItem[]
        const payload: CreatePromoProgramaRequest = {
            titulo: formData.titulo!,
            tipoPromoId: formData.tipoPromoId!,
            monedaId: formData.monedaId!,
            descripcion: formData.descripcion || undefined,
            campaniaCrowdfundingId: formData.campaniaCrowdfundingId || undefined,
            proyectoArtisticoId: formData.proyectoArtisticoId || undefined,
            urlLanding: formData.urlLanding || undefined,
            codigoTrackingBase: formData.codigoTrackingBase || undefined,
            importeComisionPorcentaje: formData.importeComisionPorcentaje,
            importeComisionFija: formData.importeComisionFija,
            fechaInicio: formData.fechaInicio || undefined,
            fechaFin: formData.fechaFin || undefined,
            tareas: tareas.map((t) => ({
                titulo: t.titulo,
                tipoEventoPromoId: t.tipoEventoPromoId,
                tipoRewardId: t.tipoRewardId,
                esRepetible: t.esRepetible,
                descripcion: t.descripcion || undefined,
                importeRecompensa: t.importeRecompensa,
                monedaId: t.monedaId,
                puntosRecompensa: t.puntosRecompensa,
                urlInstrucciones: t.urlInstrucciones || undefined,
                maxRepeticiones: t.maxRepeticiones,
                fechaInicio: t.fechaInicio || undefined,
                fechaFin: t.fechaFin || undefined,
            })),
        }

        try {
            if (mode === "create") {
                const result = await createMutation.mutateAsync(payload)
                resetWizard()
                toast.success("Programa de promocion creado")
                router.push(APP_ROUTES.dashboard.crowdpromotion.programas.detalle(result.id))
            } else {
                await updateMutation.mutateAsync({ id: programaId!, data: payload })
                toast.success("Programa actualizado correctamente")
                router.push(APP_ROUTES.dashboard.crowdpromotion.programas.detalle(programaId!))
            }
        } catch (error) {
            const axiosError = error as AxiosError<ErrorResponseData>
            const errorCode = axiosError?.response?.data?.messages?.[0]?.errorCode
            if (errorCode) {
                toast.error(getPromoProgramaErrorMessage(errorCode))
            }
        }
    }

    const handleAbandon = () => {
        resetWizard()
        router.push(APP_ROUTES.dashboard.crowdpromotion.programas.list)
    }

    if (mode === "edit" && isLoadingPrograma) {
        return (
            <div className="flex flex-col h-screen bg-[#0d0d1a]">
                <div className="h-14 border-b border-zinc-800 animate-pulse bg-[#1e1e38]" />
                <div className="flex-1 flex items-center justify-center">
                    <div className="h-[400px] w-full max-w-2xl animate-pulse rounded-lg bg-[#1e1e38]" />
                </div>
            </div>
        )
    }

    if (!isHydrated) {
        return (
            <div className="flex flex-col h-screen bg-[#0d0d1a]">
                <div className="h-14 border-b border-zinc-800" />
                <div className="flex-1" />
            </div>
        )
    }

    const tareas = (formData.tareas ?? []) as CreatePromoTareaItem[]

    return (
        <div className="flex flex-col h-screen bg-[#0d0d1a]">
            <PromoProgramaWizardHeader
                mode={mode}
                onCancelar={() => setShowAbandonDialog(true)}
            />

            <PromoProgramaWizardStepper
                pasoActual={pasoActual}
                pasosCompletados={pasosCompletados}
                onPasoClick={irAPaso}
            />

            <div className="flex-1 overflow-y-auto px-4 py-6">
                {pasoActual === 1 && (
                    <DatosBasicosStep
                        defaultValues={formData}
                        onNext={handleStepNext}
                        tienePromotores={tienePromotores}
                    />
                )}
                {pasoActual === 2 && (
                    <ComisionesStep
                        defaultValues={formData}
                        onNext={handleStepNext}
                        tienePromotores={tienePromotores}
                    />
                )}
                {pasoActual === 3 && (
                    <TareasStep
                        tareas={tareas}
                        onTareasChange={handleTareasChange}
                        onNext={() => { marcarPasoCompletado(3); irAPaso(4) }}
                        tienePromotores={tienePromotores}
                    />
                )}
                {pasoActual === 4 && (
                    <RevisarPublicarStep
                        formData={formData}
                        onEditarPaso={irAPaso}
                        isSubmitting={isSubmitting}
                        mode={mode}
                    />
                )}
            </div>

            <PromoProgramaWizardFooter
                pasoActual={pasoActual}
                onAnterior={handleAnterior}
                onSiguiente={handleSiguiente}
                isSubmitting={isSubmitting}
                mode={mode}
            />

            <PromoProgramaAbandonDialog
                open={showAbandonDialog}
                onOpenChange={setShowAbandonDialog}
                onConfirmar={handleAbandon}
            />
        </div>
    )
}
