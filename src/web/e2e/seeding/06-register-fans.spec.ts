import { test, expect } from "@playwright/test"
import {
    registerViaLandingUI,
    loginViaApi,
    LANDING_BASE,
} from "../helpers"
import { loadSeedDataWithFallback } from "../fixtures/test-data"
import { saveFanTokens, type TokenEntry } from "../fixtures/seed-state"

/**
 * Spec 06: Register all fans via Landing UI.
 * - All 5 fans registered via Landing UI (validates registration form)
 * - Saves fan tokens to state file for subsequent specs.
 *
 * @seeding
 */

const seedData = loadSeedDataWithFallback()
const fans = seedData.fans

const collectedFanTokens: TokenEntry[] = []

test.describe("@seeding Spec 06: Register Fans", () => {
    test.describe.configure({ mode: "serial" })

    for (let i = 0; i < fans.length; i++) {
        const fan = fans[i]

        test(`UI register fan ${i + 1}: ${fan.name}`, async ({ page }) => {
            try {
                await registerViaLandingUI(page, fan.name, fan.email, fan.password)
            } catch {
                // If already registered (re-run), skip UI registration
                await page.goto(`${LANDING_BASE}/auth/login`)
                await page.waitForTimeout(500)
            }

            // Verify registration succeeded by getting token via API login
            const { token, userId } = await loginViaApi(fan.email, fan.password)
            expect(token).toBeTruthy()
            expect(userId).toBeTruthy()

            collectedFanTokens.push({
                email: fan.email,
                token,
                userId,
            })
        })
    }

    // --- Save state for subsequent specs ---

    test("Save fan tokens to state file", async () => {
        expect(collectedFanTokens.length).toBe(fans.length)
        saveFanTokens(collectedFanTokens)
    })

    // --- Final verification ---

    test("Verify all fans can login", async () => {
        for (const fan of fans) {
            const { token } = await loginViaApi(fan.email, fan.password)
            expect(token).toBeTruthy()
        }
    })
})
