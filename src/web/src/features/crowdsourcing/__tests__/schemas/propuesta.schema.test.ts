import { describe, it, expect } from "vitest"
import { createPropuestaSchema } from "@shared/schemas/crowdsourcing.schema"

describe("createPropuestaSchema", () => {
    const validData = {
        precioPropuesto: 450,
        monedaId: 1,
        diasEstimados: 14,
        mensajePropuesta: "Soy ingeniero de mezcla con experiencia profesional amplia",
    }

    it("validates valid data", () => {
        const result = createPropuestaSchema.safeParse(validData)
        expect(result.success).toBe(true)
    })

    it("validates data without diasEstimados (optional)", () => {
        const result = createPropuestaSchema.safeParse({
            ...validData,
            diasEstimados: undefined,
        })
        expect(result.success).toBe(true)
    })

    it("rejects precioPropuesto = 0", () => {
        const result = createPropuestaSchema.safeParse({
            ...validData,
            precioPropuesto: 0,
        })
        expect(result.success).toBe(false)
    })

    it("rejects negative precioPropuesto", () => {
        const result = createPropuestaSchema.safeParse({
            ...validData,
            precioPropuesto: -100,
        })
        expect(result.success).toBe(false)
    })

    it("rejects missing precioPropuesto", () => {
        const result = createPropuestaSchema.safeParse({
            monedaId: 1,
            mensajePropuesta: "Mensaje de al menos 20 caracteres aqui",
        })
        expect(result.success).toBe(false)
    })

    it("rejects monedaId = 0", () => {
        const result = createPropuestaSchema.safeParse({
            ...validData,
            monedaId: 0,
        })
        expect(result.success).toBe(false)
    })

    it("rejects mensajePropuesta shorter than 20 chars", () => {
        const result = createPropuestaSchema.safeParse({
            ...validData,
            mensajePropuesta: "Muy corto",
        })
        expect(result.success).toBe(false)
    })

    it("rejects empty mensajePropuesta", () => {
        const result = createPropuestaSchema.safeParse({
            ...validData,
            mensajePropuesta: "",
        })
        expect(result.success).toBe(false)
    })

    it("rejects mensajePropuesta longer than 2000 chars", () => {
        const result = createPropuestaSchema.safeParse({
            ...validData,
            mensajePropuesta: "a".repeat(2001),
        })
        expect(result.success).toBe(false)
    })

    it("rejects diasEstimados > 365", () => {
        const result = createPropuestaSchema.safeParse({
            ...validData,
            diasEstimados: 400,
        })
        expect(result.success).toBe(false)
    })

    it("rejects diasEstimados = 0", () => {
        const result = createPropuestaSchema.safeParse({
            ...validData,
            diasEstimados: 0,
        })
        expect(result.success).toBe(false)
    })

    it("rejects non-integer diasEstimados", () => {
        const result = createPropuestaSchema.safeParse({
            ...validData,
            diasEstimados: 14.5,
        })
        expect(result.success).toBe(false)
    })

    it("accepts mensajePropuesta exactly at 20 chars", () => {
        const result = createPropuestaSchema.safeParse({
            ...validData,
            mensajePropuesta: "12345678901234567890",
        })
        expect(result.success).toBe(true)
    })

    it("accepts mensajePropuesta exactly at 2000 chars", () => {
        const result = createPropuestaSchema.safeParse({
            ...validData,
            mensajePropuesta: "a".repeat(2000),
        })
        expect(result.success).toBe(true)
    })
})
