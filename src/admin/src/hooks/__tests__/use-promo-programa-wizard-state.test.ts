import { promoProgramaWizardReducer, type PromoProgramaWizardState } from "../use-promo-programa-wizard-state"

const initialState: PromoProgramaWizardState = {
    pasoActual: 1,
    formData: {},
    pasosCompletados: [],
    isHydrated: false,
}

describe("promoProgramaWizardReducer", () => {
    describe("SET_PASO", () => {
        it("navigates to next accessible step", () => {
            const state = { ...initialState, pasosCompletados: [1], isHydrated: true }
            const result = promoProgramaWizardReducer(state, { type: "SET_PASO", paso: 2 })
            expect(result.pasoActual).toBe(2)
        })

        it("navigates back to completed step", () => {
            const state = { ...initialState, pasoActual: 3, pasosCompletados: [1, 2], isHydrated: true }
            const result = promoProgramaWizardReducer(state, { type: "SET_PASO", paso: 1 })
            expect(result.pasoActual).toBe(1)
        })

        it("does not navigate to inaccessible step", () => {
            const state = { ...initialState, pasosCompletados: [], isHydrated: true }
            const result = promoProgramaWizardReducer(state, { type: "SET_PASO", paso: 3 })
            expect(result.pasoActual).toBe(1) // unchanged
        })

        it("does not navigate below step 1", () => {
            const result = promoProgramaWizardReducer(initialState, { type: "SET_PASO", paso: 0 })
            expect(result.pasoActual).toBe(1)
        })

        it("does not navigate above step 4", () => {
            const state = { ...initialState, pasosCompletados: [1, 2, 3, 4] }
            const result = promoProgramaWizardReducer(state, { type: "SET_PASO", paso: 5 })
            expect(result.pasoActual).toBe(1)
        })
    })

    describe("UPDATE_FORM_DATA", () => {
        it("merges form data", () => {
            const result = promoProgramaWizardReducer(initialState, {
                type: "UPDATE_FORM_DATA",
                data: { titulo: "Test" },
            })
            expect(result.formData.titulo).toBe("Test")
        })

        it("marks current step as completed", () => {
            const result = promoProgramaWizardReducer(initialState, {
                type: "UPDATE_FORM_DATA",
                data: { titulo: "Test" },
            })
            expect(result.pasosCompletados).toContain(1)
        })

        it("does not duplicate completed steps", () => {
            const state = { ...initialState, pasosCompletados: [1] }
            const result = promoProgramaWizardReducer(state, {
                type: "UPDATE_FORM_DATA",
                data: { titulo: "Updated" },
            })
            expect(result.pasosCompletados.filter((p) => p === 1)).toHaveLength(1)
        })

        it("preserves existing form data", () => {
            const state = { ...initialState, formData: { titulo: "Existing" } }
            const result = promoProgramaWizardReducer(state, {
                type: "UPDATE_FORM_DATA",
                data: { tipoPromoId: 1 },
            })
            expect(result.formData.titulo).toBe("Existing")
            expect(result.formData.tipoPromoId).toBe(1)
        })
    })

    describe("UPDATE_TAREAS", () => {
        it("updates tareas in formData", () => {
            const tareas = [{ titulo: "Tarea 1", tipoEventoPromoId: 1, tipoRewardId: 1, esRepetible: false }]
            const result = promoProgramaWizardReducer(initialState, {
                type: "UPDATE_TAREAS",
                tareas: tareas as Parameters<typeof promoProgramaWizardReducer>[1] extends { type: "UPDATE_TAREAS" } ? Parameters<typeof promoProgramaWizardReducer>[1]["tareas"] : never,
            })
            expect(result.formData.tareas).toHaveLength(1)
        })

        it("marks step 3 as completed", () => {
            const result = promoProgramaWizardReducer(initialState, {
                type: "UPDATE_TAREAS",
                tareas: [],
            })
            expect(result.pasosCompletados).toContain(3)
        })
    })

    describe("MARCAR_PASO_COMPLETADO", () => {
        it("adds step to completed list", () => {
            const result = promoProgramaWizardReducer(initialState, {
                type: "MARCAR_PASO_COMPLETADO",
                paso: 2,
            })
            expect(result.pasosCompletados).toContain(2)
        })

        it("does not duplicate", () => {
            const state = { ...initialState, pasosCompletados: [2] }
            const result = promoProgramaWizardReducer(state, {
                type: "MARCAR_PASO_COMPLETADO",
                paso: 2,
            })
            expect(result.pasosCompletados).toEqual([2])
        })
    })

    describe("RESET", () => {
        it("resets to initial state with isHydrated true", () => {
            const state = {
                pasoActual: 3,
                formData: { titulo: "Test" },
                pasosCompletados: [1, 2],
                isHydrated: true,
            }
            const result = promoProgramaWizardReducer(state, { type: "RESET" })
            expect(result.pasoActual).toBe(1)
            expect(result.formData).toEqual({})
            expect(result.pasosCompletados).toEqual([])
            expect(result.isHydrated).toBe(true)
        })
    })

    describe("HYDRATE", () => {
        it("sets isHydrated to true", () => {
            const result = promoProgramaWizardReducer(initialState, {
                type: "HYDRATE",
                state: {},
            })
            expect(result.isHydrated).toBe(true)
        })

        it("merges hydrated state", () => {
            const result = promoProgramaWizardReducer(initialState, {
                type: "HYDRATE",
                state: {
                    pasoActual: 2,
                    formData: { titulo: "Saved" },
                    pasosCompletados: [1],
                },
            })
            expect(result.pasoActual).toBe(2)
            expect(result.formData.titulo).toBe("Saved")
            expect(result.pasosCompletados).toEqual([1])
        })
    })
})
