import { describe, it, expect } from "vitest"
import { artistaSchema } from "../application/schemas"

describe("artistaSchema", () => {
    const validData = {
        nombreArtistico: "Rock Band",
        descripcion: "Una banda de rock alternativo",
        imagenUrl: "https://example.com/foto.jpg",
        generoMusical: "Rock",
    }

    it("validates a complete valid form", () => {
        const result = artistaSchema.safeParse(validData)

        expect(result.success).toBe(true)
    })

    it("validates with only required fields", () => {
        const result = artistaSchema.safeParse({
            nombreArtistico: "Rock Band",
        })

        expect(result.success).toBe(true)
    })

    it("rejects short nombreArtistico (less than 2 chars)", () => {
        const result = artistaSchema.safeParse({
            ...validData,
            nombreArtistico: "A",
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].message).toContain("obligatorio")
        }
    })

    it("rejects empty nombreArtistico", () => {
        const result = artistaSchema.safeParse({
            ...validData,
            nombreArtistico: "",
        })

        expect(result.success).toBe(false)
    })

    it("rejects nombreArtistico longer than 100 characters", () => {
        const result = artistaSchema.safeParse({
            ...validData,
            nombreArtistico: "A".repeat(101),
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].message).toContain("100 caracteres")
        }
    })

    it("accepts nombreArtistico with exactly 100 characters", () => {
        const result = artistaSchema.safeParse({
            ...validData,
            nombreArtistico: "A".repeat(100),
        })

        expect(result.success).toBe(true)
    })

    it("allows optional descripcion", () => {
        const result = artistaSchema.safeParse({
            nombreArtistico: "Test Artist",
        })

        expect(result.success).toBe(true)
    })

    it("rejects descripcion longer than 1000 characters", () => {
        const result = artistaSchema.safeParse({
            ...validData,
            descripcion: "A".repeat(1001),
        })

        expect(result.success).toBe(false)
    })

    it("rejects invalid imagenUrl", () => {
        const result = artistaSchema.safeParse({
            ...validData,
            imagenUrl: "not-a-url",
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].message).toContain("URL invalida")
        }
    })

    it("allows empty string for imagenUrl", () => {
        const result = artistaSchema.safeParse({
            ...validData,
            imagenUrl: "",
        })

        expect(result.success).toBe(true)
    })

    it("allows valid URL for imagenUrl", () => {
        const result = artistaSchema.safeParse({
            ...validData,
            imagenUrl: "https://cdn.example.com/photo.png",
        })

        expect(result.success).toBe(true)
    })

    it("rejects generoMusical longer than 50 characters", () => {
        const result = artistaSchema.safeParse({
            ...validData,
            generoMusical: "A".repeat(51),
        })

        expect(result.success).toBe(false)
    })

    it("allows generoMusical with exactly 50 characters", () => {
        const result = artistaSchema.safeParse({
            ...validData,
            generoMusical: "A".repeat(50),
        })

        expect(result.success).toBe(true)
    })
})
