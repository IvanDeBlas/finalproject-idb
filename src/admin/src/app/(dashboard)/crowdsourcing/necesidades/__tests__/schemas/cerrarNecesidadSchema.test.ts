import { describe, it, expect } from "vitest"
import { cerrarNecesidadSchema } from "@shared/schemas/crowdsourcing.schema"

describe("cerrarNecesidadSchema", () => {
    it("validates valid data with motivo", () => {
        const result = cerrarNecesidadSchema.safeParse({
            motivo: "Ya no necesito este servicio",
        })

        expect(result.success).toBe(true)
    })

    it("validates valid data without motivo", () => {
        const result = cerrarNecesidadSchema.safeParse({})
        expect(result.success).toBe(true)
    })

    it("validates empty motivo string", () => {
        const result = cerrarNecesidadSchema.safeParse({ motivo: "" })
        expect(result.success).toBe(true)
    })

    it("rejects motivo > 500 chars", () => {
        const result = cerrarNecesidadSchema.safeParse({
            motivo: "A".repeat(501),
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.errors[0].message).toContain(
                "no puede superar los 500 caracteres"
            )
        }
    })

    it("accepts motivo exactly 500 chars", () => {
        const result = cerrarNecesidadSchema.safeParse({
            motivo: "A".repeat(500),
        })

        expect(result.success).toBe(true)
    })
})
