"use client"

import { useReducer, useEffect, useCallback } from "react"
import type { CreatePromoProgramaFormData } from "@shared/schemas/crowdpromotion.schema"
import type { CreatePromoTareaItem } from "@shared/types"

const WIZARD_STORAGE_KEY = "promo-programa-wizard-draft"

export interface PromoProgramaWizardState {
    pasoActual: number
    formData: Partial<CreatePromoProgramaFormData>
    pasosCompletados: number[]
    isHydrated: boolean
}

export type PromoProgramaWizardAction =
    | { type: "SET_PASO"; paso: number }
    | { type: "UPDATE_FORM_DATA"; data: Partial<CreatePromoProgramaFormData> }
    | { type: "UPDATE_TAREAS"; tareas: CreatePromoTareaItem[] }
    | { type: "MARCAR_PASO_COMPLETADO"; paso: number }
    | { type: "RESET" }
    | { type: "HYDRATE"; state: Partial<PromoProgramaWizardState> }

const initialState: PromoProgramaWizardState = {
    pasoActual: 1,
    formData: {},
    pasosCompletados: [],
    isHydrated: false,
}

export function promoProgramaWizardReducer(
    state: PromoProgramaWizardState,
    action: PromoProgramaWizardAction
): PromoProgramaWizardState {
    switch (action.type) {
        case "SET_PASO": {
            const maxAccessible = Math.max(...state.pasosCompletados, 0) + 1
            if (
                action.paso >= 1 &&
                action.paso <= 4 &&
                (action.paso <= maxAccessible ||
                    state.pasosCompletados.includes(action.paso) ||
                    action.paso === state.pasoActual)
            ) {
                return { ...state, pasoActual: action.paso }
            }
            return state
        }
        case "UPDATE_FORM_DATA": {
            const newPasosCompletados = state.pasosCompletados.includes(state.pasoActual)
                ? state.pasosCompletados
                : [...state.pasosCompletados, state.pasoActual]
            return {
                ...state,
                formData: { ...state.formData, ...action.data },
                pasosCompletados: newPasosCompletados,
            }
        }
        case "UPDATE_TAREAS": {
            const newPasosCompletados = state.pasosCompletados.includes(3)
                ? state.pasosCompletados
                : [...state.pasosCompletados, 3]
            return {
                ...state,
                formData: { ...state.formData, tareas: action.tareas as CreatePromoProgramaFormData["tareas"] },
                pasosCompletados: newPasosCompletados,
            }
        }
        case "MARCAR_PASO_COMPLETADO": {
            if (state.pasosCompletados.includes(action.paso)) {
                return state
            }
            return {
                ...state,
                pasosCompletados: [...state.pasosCompletados, action.paso],
            }
        }
        case "RESET":
            return { ...initialState, isHydrated: true }
        case "HYDRATE":
            return {
                ...state,
                ...action.state,
                isHydrated: true,
            }
        default:
            return state
    }
}

export function usePromoProgramaWizardState(
    initialData?: Partial<CreatePromoProgramaFormData>
) {
    const [state, dispatch] = useReducer(
        promoProgramaWizardReducer,
        initialData
            ? {
                pasoActual: 1,
                formData: initialData,
                pasosCompletados: [1, 2, 3],
                isHydrated: true,
            }
            : initialState
    )

    // Load from localStorage on mount (only in create mode)
    useEffect(() => {
        if (initialData) return

        try {
            const saved = localStorage.getItem(WIZARD_STORAGE_KEY)
            if (saved) {
                const parsed = JSON.parse(saved) as Partial<PromoProgramaWizardState>
                dispatch({
                    type: "HYDRATE",
                    state: {
                        formData: parsed.formData || {},
                        pasoActual: parsed.pasoActual || 1,
                        pasosCompletados: parsed.pasosCompletados || [],
                    },
                })
            } else {
                dispatch({ type: "HYDRATE", state: {} })
            }
        } catch {
            dispatch({ type: "HYDRATE", state: {} })
        }
    }, [initialData])

    // Persist to localStorage on changes (only after hydration, only in create mode)
    useEffect(() => {
        if (!state.isHydrated || initialData) return

        try {
            const data = {
                formData: state.formData,
                pasoActual: state.pasoActual,
                pasosCompletados: state.pasosCompletados,
            }
            localStorage.setItem(WIZARD_STORAGE_KEY, JSON.stringify(data))
        } catch {
            // Silently ignore storage errors
        }
    }, [state.formData, state.pasoActual, state.pasosCompletados, state.isHydrated, initialData])

    const irAPaso = useCallback(
        (paso: number) => dispatch({ type: "SET_PASO", paso }),
        []
    )

    const actualizarFormData = useCallback(
        (data: Partial<CreatePromoProgramaFormData>) =>
            dispatch({ type: "UPDATE_FORM_DATA", data }),
        []
    )

    const actualizarTareas = useCallback(
        (tareas: CreatePromoTareaItem[]) =>
            dispatch({ type: "UPDATE_TAREAS", tareas }),
        []
    )

    const marcarPasoCompletado = useCallback(
        (paso: number) => dispatch({ type: "MARCAR_PASO_COMPLETADO", paso }),
        []
    )

    const resetWizard = useCallback(() => {
        dispatch({ type: "RESET" })
        try {
            localStorage.removeItem(WIZARD_STORAGE_KEY)
        } catch {
            // Silently ignore storage errors
        }
    }, [])

    return {
        pasoActual: state.pasoActual,
        formData: state.formData,
        pasosCompletados: state.pasosCompletados,
        isHydrated: state.isHydrated,
        irAPaso,
        actualizarFormData,
        actualizarTareas,
        marcarPasoCompletado,
        resetWizard,
    }
}
