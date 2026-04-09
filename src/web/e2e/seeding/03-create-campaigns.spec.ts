import { test, expect } from "@playwright/test"
import {
    loginToAdminUI,
    createCampaignViaWizard,
    createCampaignViaApi,
    loginViaApi,
    API_BASE,
} from "../helpers"
import { loadSeedDataWithFallback, getArtistCampaigns } from "../fixtures/test-data"
import {
    getToken,
    saveCampaignIds,
    type CampaignEntry,
} from "../fixtures/seed-state"

/**
 * Spec 03: Create campaigns for all artists.
 * - First 3 via Admin wizard UI (validates 5-step wizard)
 * - Remaining via API (efficiency)
 * - Saves campaignIds to state file for subsequent specs.
 *
 * @seeding
 */

const seedData = loadSeedDataWithFallback()
const artists = seedData.artists
const UI_COUNT = 3

const collectedCampaigns: CampaignEntry[] = []

test.describe("@seeding Spec 03: Create Campaigns", () => {
    test.describe.configure({ mode: "serial" })

    // --- UI Campaign Creation (first 3 artists via wizard) ---

    for (let i = 0; i < Math.min(UI_COUNT, artists.length); i++) {
        const artist = artists[i]
        const campaigns = getArtistCampaigns(artist)
        if (campaigns.length === 0) continue

        const campaign = campaigns[0]

        test(`UI create campaign ${i + 1}: ${campaign.titulo}`, async ({ page }) => {
            await loginToAdminUI(page, artist.email, artist.password)

            const campaignId = await createCampaignViaWizard(page, {
                titulo: campaign.titulo,
                importeObjetivo: campaign.importeObjetivo,
                subtitulo: campaign.subtitulo,
                descripcion: campaign.descripcionCorta,
                fechaFin: true,
            })

            expect(campaignId).toBeTruthy()
            expect(campaignId).toMatch(/^[0-9a-f-]+$/)

            // Verify campaign created in BORRADOR via API
            const tokenEntry = getToken(artist.email)
            const res = await fetch(`${API_BASE}/api/campanias/${campaignId}`, {
                headers: { Authorization: `Bearer ${tokenEntry.token}` },
            })
            const data = await res.json()
            expect(data.data?.titulo).toBe(campaign.titulo)

            collectedCampaigns.push({
                email: artist.email,
                campaignTitle: campaign.titulo,
                campaignId,
            })
        })
    }

    // --- API Campaign Creation (remaining artists) ---

    if (artists.length > UI_COUNT) {
        test(`API create campaigns for remaining ${artists.length - UI_COUNT} artists`, async () => {
            for (let i = UI_COUNT; i < artists.length; i++) {
                const artist = artists[i]
                const campaigns = getArtistCampaigns(artist)
                if (campaigns.length === 0) continue

                const campaign = campaigns[0]
                let tokenEntry: { token: string; userId: string }

                try {
                    tokenEntry = getToken(artist.email)
                } catch {
                    // If token expired or missing, re-login
                    tokenEntry = await loginViaApi(artist.email, artist.password)
                }

                const now = new Date()
                const futureDate = new Date(
                    now.getTime() + (campaign.fechaFinDias ?? 60) * 24 * 60 * 60 * 1000
                )

                try {
                    const campaignId = await createCampaignViaApi(
                        tokenEntry.token,
                        {
                            titulo: campaign.titulo,
                            subtitulo: campaign.subtitulo,
                            descripcionCorta: campaign.descripcionCorta,
                            importeObjetivo: campaign.importeObjetivo,
                            tipoFinanciacionId: campaign.tipoFinanciacionId ?? 1,
                            imagenPrincipalUrl: campaign.imagenPrincipalUrl ?? "",
                            fechaFin: futureDate.toISOString(),
                        }
                    )

                    expect(campaignId).toBeTruthy()

                    collectedCampaigns.push({
                        email: artist.email,
                        campaignTitle: campaign.titulo,
                        campaignId,
                    })
                } catch (error) {
                    // If campaign already exists (re-run), try to find it
                    const errorMsg = String(error)
                    if (
                        errorMsg.includes("already") ||
                        errorMsg.includes("ya existe") ||
                        errorMsg.includes("duplicate")
                    ) {
                        const listRes = await fetch(
                            `${API_BASE}/api/campanias?titulo=${encodeURIComponent(campaign.titulo)}`,
                            {
                                headers: { Authorization: `Bearer ${tokenEntry.token}` },
                            }
                        )
                        const listData = await listRes.json()
                        const existing = listData.data?.items?.find(
                            (c: Record<string, unknown>) => c.titulo === campaign.titulo
                        ) ?? listData.data?.[0]

                        if (existing?.id) {
                            collectedCampaigns.push({
                                email: artist.email,
                                campaignTitle: campaign.titulo,
                                campaignId: existing.id,
                            })
                        } else {
                            throw error
                        }
                    } else {
                        throw error
                    }
                }
            }

            expect(collectedCampaigns.length).toBeGreaterThanOrEqual(artists.length)
        })
    }

    // --- Save state for subsequent specs ---

    test("Save campaign IDs to state file", async () => {
        expect(collectedCampaigns.length).toBeGreaterThanOrEqual(artists.length)
        saveCampaignIds(collectedCampaigns)
    })

    // --- Final verification ---

    test("Verify all campaigns exist via API", async () => {
        for (const entry of collectedCampaigns) {
            const res = await fetch(`${API_BASE}/api/campanias/${entry.campaignId}`)
            expect(res.ok).toBeTruthy()

            const data = await res.json()
            expect(data.data?.titulo).toBe(entry.campaignTitle)
        }
    })
})
