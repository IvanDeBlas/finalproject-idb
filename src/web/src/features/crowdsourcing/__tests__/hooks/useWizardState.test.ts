import { describe, it, expect } from "vitest"
import { renderHook, act } from "@testing-library/react"
import { useWizardState } from "../../application/hooks/useWizardState"
import { mockTemplateDetail } from "../../__mocks__/crowdsourcing.mock"

describe("useWizardState", () => {
    it("initializes with empty state when no template", () => {
        const { result } = renderHook(() => useWizardState(undefined))

        expect(result.current.selectedNecesidades.size).toBe(0)
        expect(result.current.presupuestos.size).toBe(0)
        expect(result.current.proyectoId).toBeUndefined()
    })

    it("auto-selects Alta priority needs when template loads", () => {
        const { result } = renderHook(() => useWizardState(mockTemplateDetail))

        // mockTemplateDetail has 1 Alta priority need (nec-001)
        expect(result.current.selectedNecesidades.has("nec-001")).toBe(true)
        expect(result.current.selectedNecesidades.size).toBe(1)
    })

    it("initializes budgets for auto-selected needs", () => {
        const { result } = renderHook(() => useWizardState(mockTemplateDetail))

        const budget = result.current.presupuestos.get("nec-001")
        expect(budget).toBeDefined()
        expect(budget?.min).toBe(500)
        expect(budget?.max).toBe(1500)
    })

    it("toggles necesidad on", () => {
        const { result } = renderHook(() => useWizardState(mockTemplateDetail))

        act(() => {
            result.current.toggleNecesidad("nec-002")
        })

        expect(result.current.selectedNecesidades.has("nec-002")).toBe(true)
    })

    it("toggles necesidad off", () => {
        const { result } = renderHook(() => useWizardState(mockTemplateDetail))

        // nec-001 is auto-selected, toggle it off
        act(() => {
            result.current.toggleNecesidad("nec-001")
        })

        expect(result.current.selectedNecesidades.has("nec-001")).toBe(false)
    })

    it("removes budget when necesidad is toggled off", () => {
        const { result } = renderHook(() => useWizardState(mockTemplateDetail))

        // nec-001 is auto-selected with budget
        expect(result.current.presupuestos.has("nec-001")).toBe(true)

        act(() => {
            result.current.toggleNecesidad("nec-001")
        })

        expect(result.current.presupuestos.has("nec-001")).toBe(false)
    })

    it("initializes budget from orientative prices when toggling on", () => {
        const { result } = renderHook(() => useWizardState(mockTemplateDetail))

        act(() => {
            result.current.toggleNecesidad("nec-002")
        })

        const budget = result.current.presupuestos.get("nec-002")
        expect(budget?.min).toBe(800)
        expect(budget?.max).toBe(2000)
    })

    it("updates presupuesto correctly", () => {
        const { result } = renderHook(() => useWizardState(mockTemplateDetail))

        act(() => {
            result.current.updatePresupuesto("nec-001", 600, 1600)
        })

        const budget = result.current.presupuestos.get("nec-001")
        expect(budget?.min).toBe(600)
        expect(budget?.max).toBe(1600)
    })

    it("calculates totals correctly with single selection", () => {
        const { result } = renderHook(() => useWizardState(mockTemplateDetail))

        // Only nec-001 auto-selected: min=500, max=1500
        const totals = result.current.getTotals()
        expect(totals.minTotal).toBe(500)
        expect(totals.maxTotal).toBe(1500)
        expect(totals.selectedCount).toBe(1)
        expect(totals.totalCount).toBe(3)
    })

    it("calculates totals correctly with multiple selections", () => {
        const { result } = renderHook(() => useWizardState(mockTemplateDetail))

        act(() => {
            result.current.toggleNecesidad("nec-002")
        })

        // nec-001 (500, 1500) + nec-002 (800, 2000)
        const totals = result.current.getTotals()
        expect(totals.minTotal).toBe(1300)
        expect(totals.maxTotal).toBe(3500)
        expect(totals.selectedCount).toBe(2)
    })

    it("sets proyecto id", () => {
        const { result } = renderHook(() => useWizardState(mockTemplateDetail))

        act(() => {
            result.current.setProyectoId("proj-123")
        })

        expect(result.current.proyectoId).toBe("proj-123")
    })

    it("resets all state", () => {
        const { result } = renderHook(() => useWizardState(mockTemplateDetail))

        act(() => {
            result.current.toggleNecesidad("nec-002")
            result.current.setProyectoId("proj-123")
        })

        act(() => {
            result.current.reset()
        })

        expect(result.current.selectedNecesidades.size).toBe(0)
        expect(result.current.presupuestos.size).toBe(0)
        expect(result.current.proyectoId).toBeUndefined()
    })

    it("returns zero totals when nothing selected after reset", () => {
        const { result } = renderHook(() => useWizardState(mockTemplateDetail))

        act(() => {
            result.current.reset()
        })

        const totals = result.current.getTotals()
        expect(totals.minTotal).toBe(0)
        expect(totals.maxTotal).toBe(0)
        expect(totals.selectedCount).toBe(0)
    })
})
