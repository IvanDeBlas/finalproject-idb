import { test, expect } from "@playwright/test"
import {
    registerViaApi,
    registerViaLandingUI,
    loginViaApi,
    LANDING_BASE,
} from "../helpers"
import { loadSeedDataWithFallback } from "../fixtures/test-data"
import { saveTokens, type TokenEntry } from "../fixtures/seed-state"

/**
 * Spec 01: Register all artists from seed data.
 * - First 3 via Landing UI (validates registration form)
 * - Remaining via API (efficiency)
 * - Saves tokens to state file for subsequent specs.
 *
 * @seeding
 */

const seedData = loadSeedDataWithFallback()
const artists = seedData.artists
const UI_COUNT = 3

const collectedTokens: TokenEntry[] = []

test.describe("@seeding Spec 01: Register Artists", () => {
    test.describe.configure({ mode: "serial" })

    // --- UI Registration (first 3 artists) ---

    for (let i = 0; i < Math.min(UI_COUNT, artists.length); i++) {
        const artist = artists[i]

        test(`UI register artist ${i + 1}: ${artist.profile.nombreArtistico}`, async ({ page }) => {
            await page.goto(`${LANDING_BASE}/auth/register`)

            const submitBtn = page.getByRole("button", { name: /Crear cuenta/i })
            await submitBtn.waitFor({ state: "visible", timeout: 10000 })

            // Fill registration form
            await page.getByLabel("Nombre completo").fill(
                artist.profile.nombreArtistico
            )
            await page.getByLabel("Email").fill(artist.email)
            await page
                .getByLabel("Contrasena", { exact: true })
                .fill(artist.password)
            await page.getByLabel("Confirmar contrasena").fill(artist.password)

            await submitBtn.click()

            // Wait for redirect away from register page
            await page.waitForURL(
                (url) => !url.pathname.includes("/auth/register"),
                { timeout: 15000 }
            )

            // Verify registration succeeded by getting token via API login
            const { token, userId } = await loginViaApi(
                artist.email,
                artist.password
            )
            expect(token).toBeTruthy()
            expect(userId).toBeTruthy()

            collectedTokens.push({
                email: artist.email,
                token,
                userId,
            })
        })
    }

    // --- API Registration (remaining artists) ---

    if (artists.length > UI_COUNT) {
        test(`API register remaining ${artists.length - UI_COUNT} artists`, async () => {
            for (let i = UI_COUNT; i < artists.length; i++) {
                const artist = artists[i]

                try {
                    const { token, userId } = await registerViaApi(
                        artist.email,
                        artist.password,
                        artist.profile.nombreArtistico
                    )
                    expect(token).toBeTruthy()
                    expect(userId).toBeTruthy()

                    collectedTokens.push({
                        email: artist.email,
                        token,
                        userId,
                    })
                } catch (error) {
                    // If already registered (e.g. re-running), try login instead
                    const errorMsg = String(error)
                    if (errorMsg.includes("ya esta registrado") || errorMsg.includes("already")) {
                        const { token, userId } = await loginViaApi(
                            artist.email,
                            artist.password
                        )
                        collectedTokens.push({
                            email: artist.email,
                            token,
                            userId,
                        })
                    } else {
                        throw error
                    }
                }
            }

            expect(collectedTokens.length).toBe(artists.length)
        })
    }

    // --- Save state for subsequent specs ---

    test("Save tokens to state file", async () => {
        expect(collectedTokens.length).toBeGreaterThanOrEqual(artists.length)
        saveTokens(collectedTokens)
    })

    // --- Final verification ---

    test("Verify all artists can login", async () => {
        for (const entry of collectedTokens) {
            const { token } = await loginViaApi(
                entry.email,
                artists.find((a) => a.email === entry.email)!.password
            )
            expect(token).toBeTruthy()
        }
    })
})
