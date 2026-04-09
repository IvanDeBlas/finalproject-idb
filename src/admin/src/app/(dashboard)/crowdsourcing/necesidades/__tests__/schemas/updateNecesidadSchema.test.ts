import { describe, it, expect } from "vitest"
import { updateNecesidadSchema } from "@shared/schemas/crowdsourcing.schema"

describe("updateNecesidadSchema", () => {
    const validData = {
        titulo: "Titulo actualizado",
        modalidadTrabajoId: 2,
    }

    it("validates valid data", () => {
        const result = updateNecesidadSchema.safeParse(validData)
        expect(result.success).toBe(true)
    })

    it("does not require tipoNecesidadId", () => {
        const result = updateNecesidadSchema.safeParse(validData)
        expect(result.success).toBe(true)
    })

    it("does not require proyectoArtisticoId", () => {
        const result = updateNecesidadSchema.safeParse(validData)
        expect(result.success).toBe(true)
    })

    it("requires titulo min 5 chars", () => {
        const result = updateNecesidadSchema.safeParse({
            ...validData,
            titulo: "Abc",
        })

        expect(result.success).toBe(false)
    })

    it("validates presupuesto max >= min", () => {
        const result = updateNecesidadSchema.safeParse({
            ...validData,
            presupuestoMin: 800,
            presupuestoMax: 150,
            monedaId: 1,
        })

        expect(result.success).toBe(false)
    })

    it("requires ubicacion when modalidad Presencial", () => {
        const result = updateNecesidadSchema.safeParse({
            ...validData,
            modalidadTrabajoId: 1,
        })

        expect(result.success).toBe(false)
    })

    it("allows presupuesto with moneda", () => {
        const result = updateNecesidadSchema.safeParse({
            ...validData,
            presupuestoMin: 200,
            presupuestoMax: 900,
            monedaId: 1,
        })

        expect(result.success).toBe(true)
    })
})
