import { describe, it, expect } from "vitest"
import { loginSchema, registerSchema } from "../application/schemas"

describe("loginSchema", () => {
    it("validates a valid login form", () => {
        const result = loginSchema.safeParse({
            email: "user@example.com",
            password: "123456",
        })

        expect(result.success).toBe(true)
    })

    it("rejects empty email", () => {
        const result = loginSchema.safeParse({
            email: "",
            password: "123456",
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].message).toContain("Email invalido")
        }
    })

    it("rejects invalid email format", () => {
        const result = loginSchema.safeParse({
            email: "not-an-email",
            password: "123456",
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].message).toContain("Email invalido")
        }
    })

    it("rejects password shorter than 6 characters", () => {
        const result = loginSchema.safeParse({
            email: "user@example.com",
            password: "12345",
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].message).toContain("al menos 6 caracteres")
        }
    })

    it("accepts password with exactly 6 characters", () => {
        const result = loginSchema.safeParse({
            email: "user@example.com",
            password: "123456",
        })

        expect(result.success).toBe(true)
    })

    it("rejects missing fields", () => {
        const result = loginSchema.safeParse({})

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues.length).toBeGreaterThanOrEqual(2)
        }
    })
})

describe("registerSchema", () => {
    const validData = {
        email: "user@example.com",
        password: "123456",
        confirmPassword: "123456",
        nombreCompleto: "Juan Perez",
    }

    it("validates a valid registration form", () => {
        const result = registerSchema.safeParse(validData)

        expect(result.success).toBe(true)
    })

    it("rejects invalid email", () => {
        const result = registerSchema.safeParse({
            ...validData,
            email: "bad-email",
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].message).toContain("Email invalido")
        }
    })

    it("rejects short password", () => {
        const result = registerSchema.safeParse({
            ...validData,
            password: "12345",
            confirmPassword: "12345",
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            const passwordIssue = result.error.issues.find(
                (i) => i.path.includes("password") && !i.path.includes("confirmPassword")
            )
            expect(passwordIssue?.message).toContain("al menos 6 caracteres")
        }
    })

    it("rejects mismatched passwords", () => {
        const result = registerSchema.safeParse({
            ...validData,
            password: "123456",
            confirmPassword: "654321",
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            const mismatchIssue = result.error.issues.find(
                (i) => i.path.includes("confirmPassword")
            )
            expect(mismatchIssue?.message).toContain("no coinciden")
        }
    })

    it("rejects short nombreCompleto", () => {
        const result = registerSchema.safeParse({
            ...validData,
            nombreCompleto: "A",
        })

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues[0].message).toContain("obligatorio")
        }
    })

    it("rejects empty confirmPassword", () => {
        const result = registerSchema.safeParse({
            ...validData,
            confirmPassword: "",
        })

        expect(result.success).toBe(false)
    })

    it("rejects missing fields", () => {
        const result = registerSchema.safeParse({})

        expect(result.success).toBe(false)
        if (!result.success) {
            expect(result.error.issues.length).toBeGreaterThanOrEqual(3)
        }
    })
})
