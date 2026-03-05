import { test, expect } from "@playwright/test"
import {
    loginToAdminUI,
    publishCampaign,
    publishViaApi,
    loginViaApi,
    API_BASE,
} from "../helpers"
import { loadSeedDataWithFallback } from "../fixtures/test-data"
import { getToken, loadCampaignIds, type CampaignEntry } from "../fixtures/seed-state"

/**
 * Spec 05: Publish all campaigns.
 * - First 2-3 via Admin UI (validates publish flow: button + modal + toast)
 * - Remaining via API (efficiency)
 * - Verifies all campaigns are in PUBLICADA state.
 *
 * @seeding
 */

const seedData = loadSeedDataWithFallback()
const artists = seedData.artists
const UI_COUNT = 3

let campaignEntries: CampaignEntry[] = []

test.describe("@seeding Spec 05: Publish Campaigns", () => {
    test.describe.configure({ mode: "serial" })

    test("Load campaign state", async () => {
        campaignEntries = loadCampaignIds()
        expect(campaignEntries.length).toBeGreaterThan(0)
    })

    // --- UI Publish (first 2-3 campaigns) ---

    for (let i = 0; i < UI_COUNT; i++) {
        test(`UI publish campaign ${i + 1}`, async ({ page }) => {
            if (i >= campaignEntries.length) return

            const entry = campaignEntries[i]
            const artist = artists.find((a) => a.email === entry.email)
            if (!artist) return

            await loginToAdminUI(page, artist.email, artist.password)
            await publishCampaign(page, entry.campaignId)

            // Verify via API that campaign is published
            const tokenEntry = getToken(artist.email)
            const res = await fetch(`${API_BASE}/api/campanias/${entry.campaignId}`, {
                headers: { Authorization: `Bearer ${tokenEntry.token}` },
            })
            const data = await res.json()
            expect(data.data?.estado?.toLowerCase()).toMatch(/publicada|activa|published|active/)
        })
    }

    // --- API Publish (remaining campaigns) ---

    test("API publish remaining campaigns", async () => {
        if (campaignEntries.length <= UI_COUNT) return

        for (let i = UI_COUNT; i < campaignEntries.length; i++) {
            const entry = campaignEntries[i]
            const artist = artists.find((a) => a.email === entry.email)
            if (!artist) continue

            let tokenEntry: { token: string; userId: string }

            try {
                tokenEntry = getToken(entry.email)
            } catch {
                tokenEntry = await loginViaApi(artist.email, artist.password)
            }

            try {
                await publishViaApi(tokenEntry.token, entry.campaignId)
            } catch (error) {
                const errorMsg = String(error)
                // Skip if already published (re-run scenario)
                if (
                    errorMsg.includes("already") ||
                    errorMsg.includes("ya esta publicada") ||
                    errorMsg.includes("publicada")
                ) {
                    continue
                }
                throw error
            }
        }
    })

    // --- Final verification ---

    test("Verify all campaigns are published", async () => {
        for (const entry of campaignEntries) {
            const res = await fetch(`${API_BASE}/api/campanias/${entry.campaignId}`)
            expect(res.ok).toBeTruthy()

            const data = await res.json()
            const estado = String(data.data?.estado ?? "").toLowerCase()
            expect(estado).toMatch(/publicada|activa|published|active/)
        }
    })
})
