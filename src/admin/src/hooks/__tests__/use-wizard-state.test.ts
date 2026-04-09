import { describe, it, expect, vi, beforeEach } from "vitest"
import { renderHook, act } from "@testing-library/react"
import { useWizardState } from "../use-wizard-state"
import type { CreateCampaniaFormData } from "@shared/schemas"

const WIZARD_STORAGE_KEY = "wizard-draft"

describe("useWizardState", () => {
    beforeEach(() => {
        vi.restoreAllMocks()
        localStorage.clear()
    })

    it("initializes with default values", () => {
        const { result } = renderHook(() => useWizardState())

        expect(result.current.currentStep).toBe(1)
        expect(result.current.formData).toEqual({})
        expect(result.current.completedSteps).toEqual([])
    })

    it("initializes with initialData when provided (edit mode)", () => {
        const initialData: Partial<CreateCampaniaFormData> = {
            titulo: "Mi Campania",
            importeObjetivo: 5000,
        }

        const { result } = renderHook(() => useWizardState(initialData))

        expect(result.current.formData).toEqual(initialData)
        expect(result.current.currentStep).toBe(1)
    })

    it("goToStep navigates to valid step after completing step 1", () => {
        const { result } = renderHook(() => useWizardState())

        // Complete step 1 first by updating form data
        act(() => {
            result.current.updateFormData({ titulo: "Test" })
        })

        // Now step 1 is completed, so we can go to step 2
        act(() => {
            result.current.goToStep(2)
        })

        expect(result.current.currentStep).toBe(2)
    })

    it("goToStep rejects unreachable step", () => {
        const { result } = renderHook(() => useWizardState())

        // Trying to jump to step 4 from step 1 with no completed steps
        act(() => {
            result.current.goToStep(4)
        })

        expect(result.current.currentStep).toBe(1)
    })

    it("updateFormData merges data and marks step completed", () => {
        const { result } = renderHook(() => useWizardState())

        act(() => {
            result.current.updateFormData({ titulo: "Mi Campania" })
        })

        expect(result.current.formData).toEqual({ titulo: "Mi Campania" })
        expect(result.current.completedSteps).toContain(1)

        // Merge additional data
        act(() => {
            result.current.updateFormData({ importeObjetivo: 5000 })
        })

        expect(result.current.formData).toEqual({
            titulo: "Mi Campania",
            importeObjetivo: 5000,
        })
    })

    it("resetWizard clears everything including localStorage", () => {
        const removeItemSpy = vi.spyOn(Storage.prototype, "removeItem")

        const { result } = renderHook(() => useWizardState())

        // Set up some state first
        act(() => {
            result.current.updateFormData({ titulo: "Test" })
        })

        act(() => {
            result.current.goToStep(2)
        })

        // Now reset
        act(() => {
            result.current.resetWizard()
        })

        expect(result.current.currentStep).toBe(1)
        expect(result.current.formData).toEqual({})
        expect(result.current.completedSteps).toEqual([])
        expect(removeItemSpy).toHaveBeenCalledWith(WIZARD_STORAGE_KEY)
    })

    it("persists to localStorage in create mode", async () => {
        const setItemSpy = vi.spyOn(Storage.prototype, "setItem")

        const { result } = renderHook(() => useWizardState())

        // Wait for hydration effect to complete
        await act(async () => {
            // Allow useEffect to fire
        })

        await act(async () => {
            result.current.updateFormData({ titulo: "Persisted" })
        })

        // The persist effect fires asynchronously after state update
        await act(async () => {
            // Allow the persist useEffect to fire
        })

        expect(setItemSpy).toHaveBeenCalledWith(
            WIZARD_STORAGE_KEY,
            expect.any(String)
        )

        // Find the LAST call to localStorage with our key
        const wizardCalls = setItemSpy.mock.calls.filter(
            (call) => call[0] === WIZARD_STORAGE_KEY
        )
        expect(wizardCalls.length).toBeGreaterThan(0)
        const lastCall = wizardCalls[wizardCalls.length - 1]
        const storedData = JSON.parse(lastCall[1] as string)
        expect(storedData.formData.titulo).toBe("Persisted")
    })

    it("does not persist to localStorage in edit mode", async () => {
        const setItemSpy = vi.spyOn(Storage.prototype, "setItem")

        const initialData: Partial<CreateCampaniaFormData> = {
            titulo: "Existing",
        }

        const { result } = renderHook(() => useWizardState(initialData))

        // Wait for hydration
        await act(async () => {
            // Allow useEffect to fire
        })

        act(() => {
            result.current.updateFormData({ importeObjetivo: 1000 })
        })

        await act(async () => {
            // Allow effects to fire
        })

        // Should NOT have persisted to localStorage
        const wizardCalls = setItemSpy.mock.calls.filter(
            (call) => call[0] === WIZARD_STORAGE_KEY
        )
        expect(wizardCalls).toHaveLength(0)
    })
})
