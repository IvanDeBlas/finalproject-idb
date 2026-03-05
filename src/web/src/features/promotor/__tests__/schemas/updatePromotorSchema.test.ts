import { describe, it, expect } from "vitest"
import { updatePromotorSchema } from "@shared/schemas/crowdpromotion.schema"

describe("updatePromotorSchema", () => {
    const validData = {
        nombrePublico: "DJ Marketing Pro Updated",
        emailContacto: "nuevo@djmarketing.com",
        urlSitioWeb: "https://djmarketing.com",
        urlInstagram: "https://instagram.com/djmarketing",
        urlTikTok: "",
        urlYouTube: "",
        urlTwitter: "",
    }

    it("validates valid update request", () => {
        const result = updatePromotorSchema.safeParse(validData)
        expect(result.success).toBe(true)
    })

    it("requires nombrePublico", () => {
        const result = updatePromotorSchema.safeParse({
            ...validData,
            nombrePublico: "",
        })
        expect(result.success).toBe(false)
        if (!result.success) {
            const messages = result.error.errors.map((e) => e.message)
            expect(messages).toContain("El nombre publico es obligatorio")
        }
    })

    it("rejects nombrePublico shorter than 3 chars", () => {
        const result = updatePromotorSchema.safeParse({
            ...validData,
            nombrePublico: "AB",
        })
        expect(result.success).toBe(false)
        if (!result.success) {
            const messages = result.error.errors.map((e) => e.message)
            expect(messages).toContain("El nombre debe tener al menos 3 caracteres")
        }
    })

    it("rejects nombrePublico longer than 200 chars", () => {
        const result = updatePromotorSchema.safeParse({
            ...validData,
            nombrePublico: "A".repeat(201),
        })
        expect(result.success).toBe(false)
    })

    it("strips tipoPromotorId if included (not in schema)", () => {
        const result = updatePromotorSchema.safeParse({
            ...validData,
            tipoPromotorId: 2,
        })
        expect(result.success).toBe(true)
        if (result.success) {
            expect("tipoPromotorId" in result.data).toBe(false)
        }
    })

    it("accepts valid emailContacto", () => {
        const result = updatePromotorSchema.safeParse({
            nombrePublico: "Test Name",
            emailContacto: "valid@email.com",
        })
        expect(result.success).toBe(true)
    })

    it("rejects invalid emailContacto", () => {
        const result = updatePromotorSchema.safeParse({
            nombrePublico: "Test Name",
            emailContacto: "email-invalido",
        })
        expect(result.success).toBe(false)
    })

    it("rejects invalid URL field", () => {
        const result = updatePromotorSchema.safeParse({
            nombrePublico: "Test Name",
            urlSitioWeb: "not-a-url",
        })
        expect(result.success).toBe(false)
    })

    it("allows all URL fields empty", () => {
        const result = updatePromotorSchema.safeParse({
            nombrePublico: "Test Name",
            urlSitioWeb: "",
            urlInstagram: "",
            urlTikTok: "",
            urlYouTube: "",
            urlTwitter: "",
        })
        expect(result.success).toBe(true)
    })

    it("rejects emailContacto over 200 chars", () => {
        const result = updatePromotorSchema.safeParse({
            nombrePublico: "Test Name",
            emailContacto: "a".repeat(191) + "@email.com",
        })
        expect(result.success).toBe(false)
    })
})
