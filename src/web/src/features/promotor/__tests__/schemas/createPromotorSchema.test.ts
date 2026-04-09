import { describe, it, expect } from "vitest"
import { createPromotorSchema } from "@shared/schemas/crowdpromotion.schema"

describe("createPromotorSchema", () => {
    const validData = {
        nombrePublico: "DJ Marketing Pro",
        tipoPromotorId: 2,
        emailContacto: "contacto@djmarketing.com",
        urlSitioWeb: "https://djmarketing.com",
        urlInstagram: "https://instagram.com/djmarketing",
        urlTikTok: "https://tiktok.com/@djmarketing",
        urlYouTube: "",
        urlTwitter: "",
    }

    it("validates valid full request", () => {
        const result = createPromotorSchema.safeParse(validData)
        expect(result.success).toBe(true)
    })

    it("validates minimal request (only required fields)", () => {
        const result = createPromotorSchema.safeParse({
            nombrePublico: "Fan Test",
            tipoPromotorId: 1,
        })
        expect(result.success).toBe(true)
    })

    it("requires nombrePublico", () => {
        const result = createPromotorSchema.safeParse({
            nombrePublico: "",
            tipoPromotorId: 1,
        })
        expect(result.success).toBe(false)
        if (!result.success) {
            const messages = result.error.errors.map((e) => e.message)
            expect(messages).toContain("El nombre publico es obligatorio")
        }
    })

    it("rejects nombrePublico shorter than 3 chars", () => {
        const result = createPromotorSchema.safeParse({
            nombrePublico: "AB",
            tipoPromotorId: 1,
        })
        expect(result.success).toBe(false)
        if (!result.success) {
            const messages = result.error.errors.map((e) => e.message)
            expect(messages).toContain("El nombre debe tener al menos 3 caracteres")
        }
    })

    it("rejects nombrePublico longer than 200 chars", () => {
        const result = createPromotorSchema.safeParse({
            nombrePublico: "A".repeat(201),
            tipoPromotorId: 1,
        })
        expect(result.success).toBe(false)
        if (!result.success) {
            const messages = result.error.errors.map((e) => e.message)
            expect(messages).toContain("Maximo 200 caracteres")
        }
    })

    it("requires tipoPromotorId to be at least 1", () => {
        const result = createPromotorSchema.safeParse({
            nombrePublico: "Fan Test",
            tipoPromotorId: 0,
        })
        expect(result.success).toBe(false)
        if (!result.success) {
            const messages = result.error.errors.map((e) => e.message)
            expect(messages).toContain("El tipo de promotor es obligatorio")
        }
    })

    it("rejects non-integer tipoPromotorId", () => {
        const result = createPromotorSchema.safeParse({
            nombrePublico: "Fan Test",
            tipoPromotorId: "not-a-number",
        })
        expect(result.success).toBe(false)
    })

    it("accepts valid emailContacto", () => {
        const result = createPromotorSchema.safeParse({
            nombrePublico: "Fan Test",
            tipoPromotorId: 1,
            emailContacto: "valid@email.com",
        })
        expect(result.success).toBe(true)
    })

    it("rejects invalid emailContacto format", () => {
        const result = createPromotorSchema.safeParse({
            nombrePublico: "Fan Test",
            tipoPromotorId: 1,
            emailContacto: "no-es-email",
        })
        expect(result.success).toBe(false)
    })

    it("allows empty string for emailContacto", () => {
        const result = createPromotorSchema.safeParse({
            nombrePublico: "Fan Test",
            tipoPromotorId: 1,
            emailContacto: "",
        })
        expect(result.success).toBe(true)
    })

    it("allows undefined emailContacto", () => {
        const result = createPromotorSchema.safeParse({
            nombrePublico: "Fan Test",
            tipoPromotorId: 1,
        })
        expect(result.success).toBe(true)
    })

    it("rejects invalid URL in urlInstagram", () => {
        const result = createPromotorSchema.safeParse({
            nombrePublico: "Fan Test",
            tipoPromotorId: 1,
            urlInstagram: "not-a-url",
        })
        expect(result.success).toBe(false)
    })

    it("allows empty string for URL fields", () => {
        const result = createPromotorSchema.safeParse({
            nombrePublico: "Fan Test",
            tipoPromotorId: 1,
            urlInstagram: "",
            urlTikTok: "",
            urlYouTube: "",
            urlTwitter: "",
        })
        expect(result.success).toBe(true)
    })

    it("rejects URL longer than 300 chars", () => {
        const result = createPromotorSchema.safeParse({
            nombrePublico: "Fan Test",
            tipoPromotorId: 1,
            urlInstagram: "https://instagram.com/" + "a".repeat(290),
        })
        expect(result.success).toBe(false)
    })
})
