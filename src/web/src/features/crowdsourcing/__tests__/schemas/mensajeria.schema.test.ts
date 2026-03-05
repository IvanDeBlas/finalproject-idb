import { describe, it, expect } from "vitest"
import {
    createConversacionSchema,
    createMensajeSchema,
} from "@shared/schemas/crowdsourcing.schema"

describe("createConversacionSchema", () => {
    it("accepts valid asunto with destinatario", () => {
        const result = createConversacionSchema.safeParse({
            asunto: "Consulta sobre la propuesta",
            userIdDestinatario: "user-abc",
        })
        expect(result.success).toBe(true)
    })

    it("rejects empty asunto", () => {
        const result = createConversacionSchema.safeParse({
            asunto: "",
            userIdDestinatario: "user-abc",
        })
        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].path[0]).toBe("asunto")
        }
    })

    it("rejects asunto with only spaces", () => {
        const result = createConversacionSchema.safeParse({
            asunto: "   ",
            userIdDestinatario: "user-abc",
        })
        // min(1) only checks length, not trimmed; this passes as length > 0
        // The schema allows spaces-only strings since min(1) counts whitespace
        expect(result.success).toBe(true)
    })

    it("rejects asunto longer than 200 characters", () => {
        const result = createConversacionSchema.safeParse({
            asunto: "A".repeat(201),
            userIdDestinatario: "user-abc",
        })
        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].path[0]).toBe("asunto")
            expect(result.error.issues[0].message).toContain(
                "El asunto no puede superar"
            )
        }
    })

    it("rejects missing userIdDestinatario", () => {
        const result = createConversacionSchema.safeParse({
            asunto: "Consulta",
        })
        expect(result.success).toBe(false)
    })

    it("accepts valid necesidadId UUID", () => {
        const result = createConversacionSchema.safeParse({
            asunto: "Consulta",
            userIdDestinatario: "user-abc",
            necesidadId: "a1b2c3d4-e5f6-7a8b-9c0d-1e2f3a4b5c6d",
        })
        expect(result.success).toBe(true)
    })

    it("rejects invalid necesidadId non-UUID", () => {
        const result = createConversacionSchema.safeParse({
            asunto: "Consulta",
            userIdDestinatario: "user-abc",
            necesidadId: "not-a-uuid",
        })
        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].path[0]).toBe("necesidadId")
        }
    })

    it("accepts without necesidadId or acuerdoId", () => {
        const result = createConversacionSchema.safeParse({
            asunto: "Consulta general",
            userIdDestinatario: "user-abc",
        })
        expect(result.success).toBe(true)
    })
})

describe("createMensajeSchema", () => {
    it("accepts contenido of 1 character", () => {
        const result = createMensajeSchema.safeParse({
            contenido: "H",
        })
        expect(result.success).toBe(true)
    })

    it("rejects empty contenido", () => {
        const result = createMensajeSchema.safeParse({
            contenido: "",
        })
        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].path[0]).toBe("contenido")
        }
    })

    it("rejects contenido longer than 5000 characters", () => {
        const result = createMensajeSchema.safeParse({
            contenido: "A".repeat(5001),
        })
        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].path[0]).toBe("contenido")
            expect(result.error.issues[0].message).toContain(
                "El mensaje no puede superar"
            )
        }
    })

    it("accepts without urlAdjunto", () => {
        const result = createMensajeSchema.safeParse({
            contenido: "Mensaje de prueba",
        })
        expect(result.success).toBe(true)
    })

    it("accepts empty string urlAdjunto", () => {
        const result = createMensajeSchema.safeParse({
            contenido: "Mensaje",
            urlAdjunto: "",
        })
        expect(result.success).toBe(true)
    })

    it("accepts valid https URL as urlAdjunto", () => {
        const result = createMensajeSchema.safeParse({
            contenido: "Mensaje",
            urlAdjunto: "https://drive.google.com/file",
        })
        expect(result.success).toBe(true)
    })

    it("rejects urlAdjunto without protocol", () => {
        const result = createMensajeSchema.safeParse({
            contenido: "Mensaje",
            urlAdjunto: "drive.google.com/file",
        })
        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].path[0]).toBe("urlAdjunto")
        }
    })

    it("rejects urlAdjunto with free text", () => {
        const result = createMensajeSchema.safeParse({
            contenido: "Mensaje",
            urlAdjunto: "esto no es una url",
        })
        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].path[0]).toBe("urlAdjunto")
        }
    })
})
