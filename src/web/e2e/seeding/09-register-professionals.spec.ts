import { test, expect } from "@playwright/test"
import { registerViaApi, loginViaApi } from "../helpers"
import { loadProfessionals } from "../fixtures/test-data"
import {
    saveProfessionalTokens,
    type TokenEntry,
} from "../fixtures/seed-state"

/**
 * Spec 09: Register 8 crowdsourcing professionals.
 * All via API (efficiency - professionals don't need UI validation).
 * Saves tokens to state file for subsequent specs.
 *
 * @seeding
 */

const professionals = loadProfessionals()
const collectedTokens: TokenEntry[] = []

test.describe("@seeding Spec 09: Register Professionals", () => {
    test.describe.configure({ mode: "serial" })

    test(`Register ${professionals.length} professionals via API`, async () => {
        for (const prof of professionals) {
            try {
                const { token, userId } = await registerViaApi(
                    prof.email,
                    prof.password,
                    prof.nombre
                )
                expect(token).toBeTruthy()
                expect(userId).toBeTruthy()

                collectedTokens.push({
                    email: prof.email,
                    token,
                    userId,
                })
            } catch (error) {
                // If already registered (re-running), try login instead
                const errorMsg = String(error)
                if (
                    errorMsg.includes("ya esta registrado") ||
                    errorMsg.includes("already")
                ) {
                    const { token, userId } = await loginViaApi(
                        prof.email,
                        prof.password
                    )
                    collectedTokens.push({
                        email: prof.email,
                        token,
                        userId,
                    })
                } else {
                    throw error
                }
            }
        }

        expect(collectedTokens.length).toBe(professionals.length)
    })

    test("Save professional tokens to state file", async () => {
        expect(collectedTokens.length).toBe(professionals.length)
        saveProfessionalTokens(collectedTokens)
    })

    test("Verify all professionals can login", async () => {
        for (const prof of professionals) {
            const { token } = await loginViaApi(prof.email, prof.password)
            expect(token).toBeTruthy()
        }
    })
})
