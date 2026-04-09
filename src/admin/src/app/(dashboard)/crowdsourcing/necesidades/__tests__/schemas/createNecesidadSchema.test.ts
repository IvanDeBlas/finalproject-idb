import { describe, it, expect } from "vitest"
import { createNecesidadSchema } from "@shared/schemas/crowdsourcing.schema"

describe("createNecesidadSchema", () => {
    const validData = {
        titulo: "Mezcla de pistas para EP",
        descripcion: "Necesito un ingeniero de mezcla",
        tipoNecesidadId: 3,
        modalidadTrabajoId: 2,
        proyectoArtisticoId: "proj-123",
    }

    it("validates valid data", () => {
        const result = createNecesidadSchema.safeParse(validData)
        expect(result.success).toBe(true)
    })

    it("validates valid data with full fields", () => {
        const fullData = {
            ...validData,
            presupuestoMin: 150,
            presupuestoMax: 800,
            monedaId: 1,
            fechaLimitePropuestas: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(),
            fechaInicioPrevista: new Date(Date.now() + 14 * 24 * 60 * 60 * 1000).toISOString(),
        }

        const result = createNecesidadSchema.safeParse(fullData)
        expect(result.success).toBe(true)
    })

    it("requires titulo min 5 chars", () => {
        const result = createNecesidadSchema.safeParse({
            ...validData,
            titulo: "Abc",
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            const messages = result.error.errors.map((e) => e.message)
            expect(messages).toContain("El titulo debe tener al menos 5 caracteres")
        }
    })

    it("requires titulo max 200 chars", () => {
        const result = createNecesidadSchema.safeParse({
            ...validData,
            titulo: "A".repeat(201),
        })

        expect(result.success).toBe(false)
    })

    it("requires descripcion max 4000 chars", () => {
        const result = createNecesidadSchema.safeParse({
            ...validData,
            descripcion: "A".repeat(4001),
        })

        expect(result.success).toBe(false)
    })

    it("requires tipoNecesidadId", () => {
        const { tipoNecesidadId: _tipoNecesidadId, ...withoutTipo } = validData
        const result = createNecesidadSchema.safeParse(withoutTipo)

        expect(result.success).toBe(false)
    })

    it("requires modalidadTrabajoId", () => {
        const { modalidadTrabajoId: _modalidadTrabajoId, ...withoutModalidad } = validData
        const result = createNecesidadSchema.safeParse(withoutModalidad)

        expect(result.success).toBe(false)
    })

    it("requires proyectoArtisticoId", () => {
        const { proyectoArtisticoId: _proyectoArtisticoId, ...withoutProyecto } = validData
        const result = createNecesidadSchema.safeParse(withoutProyecto)

        expect(result.success).toBe(false)
    })

    it("validates presupuesto max >= min", () => {
        const result = createNecesidadSchema.safeParse({
            ...validData,
            presupuestoMin: 800,
            presupuestoMax: 150,
            monedaId: 1,
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            const messages = result.error.errors.map((e) => e.message)
            expect(messages).toContain(
                "El presupuesto maximo debe ser mayor o igual al minimo"
            )
        }
    })

    it("requires monedaId when presupuesto present", () => {
        const result = createNecesidadSchema.safeParse({
            ...validData,
            presupuestoMin: 150,
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            const messages = result.error.errors.map((e) => e.message)
            expect(messages).toContain(
                "La moneda es obligatoria cuando se especifica presupuesto"
            )
        }
    })

    it("requires ubicacion when modalidad Presencial", () => {
        const result = createNecesidadSchema.safeParse({
            ...validData,
            modalidadTrabajoId: 1,
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            const messages = result.error.errors.map((e) => e.message)
            expect(messages).toContain(
                "La ubicacion es obligatoria para modalidad Presencial o Hibrida"
            )
        }
    })

    it("requires ubicacion when modalidad Hibrido", () => {
        const result = createNecesidadSchema.safeParse({
            ...validData,
            modalidadTrabajoId: 3,
        })

        expect(result.success).toBe(false)
    })

    it("allows null ubicacion when modalidad Remoto", () => {
        const result = createNecesidadSchema.safeParse({
            ...validData,
            modalidadTrabajoId: 2,
        })

        expect(result.success).toBe(true)
    })

    it("validates fechaLimitePropuestas must be future", () => {
        const yesterday = new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString()

        const result = createNecesidadSchema.safeParse({
            ...validData,
            fechaLimitePropuestas: yesterday,
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            const messages = result.error.errors.map((e) => e.message)
            expect(messages).toContain("La fecha limite debe ser posterior a hoy")
        }
    })

    it("validates fechaInicioPrevista must be today or future", () => {
        const yesterday = new Date(Date.now() - 48 * 60 * 60 * 1000).toISOString()

        const result = createNecesidadSchema.safeParse({
            ...validData,
            fechaInicioPrevista: yesterday,
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            const messages = result.error.errors.map((e) => e.message)
            expect(messages).toContain(
                "La fecha de inicio debe ser igual o posterior a hoy"
            )
        }
    })
})
