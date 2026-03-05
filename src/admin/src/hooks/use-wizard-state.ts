"use client"

import { useState, useEffect, useCallback } from "react"
import type { CreateCampaniaFormData } from "@shared/schemas"

const WIZARD_STORAGE_KEY = "wizard-draft"

export interface TemplateData {
    templateId: string
    necesidadIds: string[]
}

interface WizardStorageData {
    formData: Partial<CreateCampaniaFormData>
    templateData: TemplateData | null
    currentStep: number
    completedSteps: number[]
}

export function useWizardState(initialData?: Partial<CreateCampaniaFormData>) {
    const [currentStep, setCurrentStep] = useState(1)
    const [formData, setFormData] = useState<Partial<CreateCampaniaFormData>>(
        initialData || {}
    )
    const [templateData, setTemplateData] = useState<TemplateData | null>(null)
    const [completedSteps, setCompletedSteps] = useState<number[]>([])
    const [isHydrated, setIsHydrated] = useState(false)

    // Load from localStorage on mount (only in create mode, not edit)
    useEffect(() => {
        if (initialData) {
            setIsHydrated(true)
            return
        }

        try {
            const saved = localStorage.getItem(WIZARD_STORAGE_KEY)
            if (saved) {
                const parsed: WizardStorageData = JSON.parse(saved)
                if (parsed.formData) {
                    setFormData(parsed.formData)
                }
                if (parsed.templateData !== undefined) {
                    setTemplateData(parsed.templateData)
                }
                if (parsed.currentStep) {
                    setCurrentStep(parsed.currentStep)
                }
                if (parsed.completedSteps) {
                    setCompletedSteps(parsed.completedSteps)
                }
            }
        } catch {
            // Silently ignore parse errors
        }

        setIsHydrated(true)
    }, [initialData])

    // Persist to localStorage on changes (only after hydration)
    useEffect(() => {
        if (!isHydrated || initialData) return

        try {
            const data: WizardStorageData = {
                formData,
                templateData,
                currentStep,
                completedSteps,
            }
            localStorage.setItem(WIZARD_STORAGE_KEY, JSON.stringify(data))
        } catch {
            // Silently ignore storage errors
        }
    }, [formData, templateData, currentStep, completedSteps, isHydrated, initialData])

    const goToStep = useCallback(
        (step: number) => {
            if (
                step >= 1 &&
                step <= 5 &&
                (step <= Math.max(...completedSteps, 0) + 1 ||
                    completedSteps.includes(step) ||
                    step === currentStep)
            ) {
                setCurrentStep(step)
            }
        },
        [completedSteps, currentStep]
    )

    const updateFormData = useCallback(
        (data: Partial<CreateCampaniaFormData>) => {
            setFormData((prev) => ({ ...prev, ...data }))
            if (!completedSteps.includes(currentStep)) {
                setCompletedSteps((prev) => [...prev, currentStep])
            }
        },
        [completedSteps, currentStep]
    )

    const markStepCompleted = useCallback(
        (step: number) => {
            if (!completedSteps.includes(step)) {
                setCompletedSteps((prev) => [...prev, step])
            }
        },
        [completedSteps]
    )

    const updateTemplateData = useCallback(
        (data: TemplateData | null) => {
            setTemplateData(data)
        },
        []
    )

    const resetWizard = useCallback(() => {
        setCurrentStep(1)
        setFormData({})
        setTemplateData(null)
        setCompletedSteps([])
        try {
            localStorage.removeItem(WIZARD_STORAGE_KEY)
        } catch {
            // Silently ignore storage errors
        }
    }, [])

    return {
        currentStep,
        formData,
        templateData,
        completedSteps,
        isHydrated,
        goToStep,
        updateFormData,
        updateTemplateData,
        markStepCompleted,
        resetWizard,
    }
}
