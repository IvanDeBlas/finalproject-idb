import { describe, it, expect } from "vitest"
import {
    createBackingSchema,
    validateBackingAmount,
    validateRewardAvailability,
    validateCampaniaActive,
} from "@shared/schemas/backing.schema"
import type { RewardPublic } from "@shared/types/campania"

describe("createBackingSchema", () => {
    it("validates a valid backing request", () => {
        const result = createBackingSchema.safeParse({
            monto: 25,
            mensaje: "Mucha suerte!",
            esAnonimo: false,
        })

        expect(result.success).toBe(true)
    })

    it("validates monto >= 1", () => {
        const result = createBackingSchema.safeParse({
            monto: 0.5,
            esAnonimo: false,
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].message).toContain("minimo es 1 EUR")
        }
    })

    it("validates monto <= 100000", () => {
        const result = createBackingSchema.safeParse({
            monto: 150000,
            esAnonimo: false,
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].message).toContain("maximo es 100,000 EUR")
        }
    })

    it("validates monto is a number", () => {
        const result = createBackingSchema.safeParse({
            monto: "not a number",
            esAnonimo: false,
        })

        expect(result.success).toBe(false)
    })

    it("validates mensaje max 500 chars", () => {
        const result = createBackingSchema.safeParse({
            monto: 10,
            mensaje: "a".repeat(501),
            esAnonimo: false,
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].message).toContain("500 caracteres")
        }
    })

    it("allows empty string for mensaje", () => {
        const result = createBackingSchema.safeParse({
            monto: 10,
            mensaje: "",
            esAnonimo: false,
        })

        expect(result.success).toBe(true)
    })

    it("allows rewardId optional", () => {
        const result = createBackingSchema.safeParse({
            monto: 10,
            esAnonimo: false,
        })

        expect(result.success).toBe(true)
        if (result.success) {
            expect(result.data.rewardId).toBeUndefined()
        }
    })

    it("validates rewardId is UUID when provided", () => {
        const result = createBackingSchema.safeParse({
            rewardId: "not-a-uuid",
            monto: 10,
            esAnonimo: false,
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].message).toContain("invalido")
        }
    })

    it("accepts valid UUID for rewardId", () => {
        const result = createBackingSchema.safeParse({
            rewardId: "550e8400-e29b-41d4-a716-446655440000",
            monto: 10,
            esAnonimo: false,
        })

        expect(result.success).toBe(true)
    })

    it("defaults esAnonimo to false", () => {
        const result = createBackingSchema.safeParse({
            monto: 10,
        })

        expect(result.success).toBe(true)
        if (result.success) {
            expect(result.data.esAnonimo).toBe(false)
        }
    })
})

describe("validateBackingAmount", () => {
    const mockReward: RewardPublic = {
        id: "123e4567-e89b-12d3-a456-426614174000",
        nombre: "CD Fisico",
        importeMinimo: 25,
        cantidadVendida: 10,
        disponible: true,
        incluyeEnvioFisico: true,
        orden: 1,
        esActivo: true,
    }

    it("returns error if monto < reward.importeMinimo", () => {
        const error = validateBackingAmount(10, mockReward)

        expect(error).not.toBeNull()
        expect(error).toContain("al menos 25")
    })

    it("returns null if monto equals reward.importeMinimo", () => {
        const error = validateBackingAmount(25, mockReward)

        expect(error).toBeNull()
    })

    it("returns null if monto is above reward.importeMinimo", () => {
        const error = validateBackingAmount(50, mockReward)

        expect(error).toBeNull()
    })

    it("returns null if no reward provided", () => {
        const error = validateBackingAmount(5, undefined)

        expect(error).toBeNull()
    })
})

describe("validateRewardAvailability", () => {
    it("returns null for available reward", () => {
        const reward: RewardPublic = {
            id: "123",
            nombre: "Test",
            importeMinimo: 10,
            cantidadVendida: 5,
            disponible: true,
            incluyeEnvioFisico: false,
            orden: 1,
            esActivo: true,
        }

        expect(validateRewardAvailability(reward)).toBeNull()
    })

    it("detects agotado (disponible=false)", () => {
        const reward: RewardPublic = {
            id: "123",
            nombre: "Test",
            importeMinimo: 10,
            cantidadVendida: 50,
            disponible: false,
            incluyeEnvioFisico: false,
            orden: 1,
            esActivo: true,
        }

        const error = validateRewardAvailability(reward)
        expect(error).not.toBeNull()
        expect(error).toContain("agotada")
    })

    it("detects inactivo (esActivo=false)", () => {
        const reward: RewardPublic = {
            id: "123",
            nombre: "Test",
            importeMinimo: 10,
            cantidadVendida: 5,
            disponible: true,
            incluyeEnvioFisico: false,
            orden: 1,
            esActivo: false,
        }

        const error = validateRewardAvailability(reward)
        expect(error).not.toBeNull()
        expect(error).toContain("no esta disponible")
    })

    it("returns null for no reward", () => {
        expect(validateRewardAvailability(undefined)).toBeNull()
    })
})

describe("validateCampaniaActive", () => {
    it("returns null for active campania (estado=2)", () => {
        const error = validateCampaniaActive(2)
        expect(error).toBeNull()
    })

    it("detects finalizada (estado=3)", () => {
        const error = validateCampaniaActive(3)
        expect(error).not.toBeNull()
        expect(error).toContain("no esta activa")
    })

    it("detects borrador (estado=1)", () => {
        const error = validateCampaniaActive(1)
        expect(error).not.toBeNull()
        expect(error).toContain("no esta activa")
    })

    it("detects fecha fin pasada", () => {
        const pastDate = new Date(Date.now() - 86400000).toISOString()
        const error = validateCampaniaActive(2, pastDate)
        expect(error).not.toBeNull()
        expect(error).toContain("ha finalizado")
    })

    it("allows future fecha fin", () => {
        const futureDate = new Date(Date.now() + 86400000 * 30).toISOString()
        const error = validateCampaniaActive(2, futureDate)
        expect(error).toBeNull()
    })
})
