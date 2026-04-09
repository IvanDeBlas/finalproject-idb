import { test, expect } from "@playwright/test"
import {
    loginToAdminUI,
    addRewardViaModal,
    addRewardViaApi,
    loginViaApi,
    API_BASE,
} from "../helpers"
import { loadSeedDataWithFallback, getArtistCampaigns } from "../fixtures/test-data"
import type { ArtistReward } from "../fixtures/test-data"
import {
    getToken,
    loadCampaignIds,
    saveRewardIds,
    type RewardEntry,
} from "../fixtures/seed-state"

/**
 * Spec 04: Add rewards to all campaigns.
 * - First 8 rewards via Admin UI modal (validates reward form)
 * - Remaining via API (efficiency)
 * - Saves rewardIds to state file for subsequent specs.
 *
 * @seeding
 */

const seedData = loadSeedDataWithFallback()
const artists = seedData.artists
const UI_REWARD_COUNT = 8

interface RewardWithContext {
    email: string
    campaignId: string
    campaignTitle: string
    reward: ArtistReward
}

/**
 * Build the flattened rewards list at runtime (inside tests),
 * so campaign-ids.json only needs to exist when tests actually run.
 */
function buildRewardsList(): RewardWithContext[] {
    const campaignEntries = loadCampaignIds()
    const rewards: RewardWithContext[] = []

    for (const artist of artists) {
        const campaigns = getArtistCampaigns(artist)
        if (campaigns.length === 0) continue

        const campaign = campaigns[0]
        const entry = campaignEntries.find(
            (e) => e.email === artist.email && e.campaignTitle === campaign.titulo
        ) ?? campaignEntries.find((e) => e.email === artist.email)

        if (!entry) continue

        for (const reward of campaign.rewards ?? []) {
            rewards.push({
                email: artist.email,
                campaignId: entry.campaignId,
                campaignTitle: campaign.titulo,
                reward,
            })
        }
    }

    return rewards
}

let allRewards: RewardWithContext[] = []
const collectedRewards: RewardEntry[] = []

test.describe("@seeding Spec 04: Add Rewards", () => {
    test.describe.configure({ mode: "serial" })

    test("Load campaign state and build rewards list", async () => {
        allRewards = buildRewardsList()
        expect(allRewards.length).toBeGreaterThan(0)
    })

    // We use a single test for UI rewards since the list is built at runtime
    test(`UI add first ${UI_REWARD_COUNT} rewards via Admin modal`, async ({ page }) => {
        const uiCount = Math.min(UI_REWARD_COUNT, allRewards.length)

        for (let i = 0; i < uiCount; i++) {
            const ctx = allRewards[i]

            await loginToAdminUI(page, ctx.email, artists.find((a) => a.email === ctx.email)!.password)

            await addRewardViaModal(page, ctx.campaignId, {
                nombre: ctx.reward.nombre,
                importeMinimo: ctx.reward.importeMinimo,
                descripcion: ctx.reward.descripcion,
                tipoRewardId: ctx.reward.tipoRewardId,
                incluyeEnvioFisico: ctx.reward.incluyeEnvioFisico,
                cantidadMaxima: ctx.reward.cantidadMaxima ?? undefined,
            })

            // Verify reward appears on the page
            await expect(
                page.getByText(ctx.reward.nombre, { exact: false }).first()
            ).toBeVisible({ timeout: 10000 })

            // Fetch reward ID from API
            const tokenEntry = getToken(ctx.email)
            const res = await fetch(
                `${API_BASE}/api/campanias/${ctx.campaignId}/rewards`,
                {
                    headers: { Authorization: `Bearer ${tokenEntry.token}` },
                }
            )
            const data = await res.json()
            const rewards = data.data?.items ?? data.data ?? []
            const found = rewards.find(
                (r: Record<string, unknown>) => r.nombre === ctx.reward.nombre
            )

            if (found?.id) {
                collectedRewards.push({
                    campaignId: ctx.campaignId,
                    rewardName: ctx.reward.nombre,
                    rewardId: found.id,
                })
            }
        }
    })

    // --- API Reward Creation (remaining rewards) ---

    test("API add remaining rewards", async () => {
        if (allRewards.length <= UI_REWARD_COUNT) return

        for (let i = UI_REWARD_COUNT; i < allRewards.length; i++) {
            const ctx = allRewards[i]
            const artist = artists.find((a) => a.email === ctx.email)!
            let tokenEntry: { token: string; userId: string }

            try {
                tokenEntry = getToken(ctx.email)
            } catch {
                tokenEntry = await loginViaApi(ctx.email, artist.password)
            }

            try {
                const rewardId = await addRewardViaApi(tokenEntry.token, {
                    campaniaId: ctx.campaignId,
                    nombre: ctx.reward.nombre,
                    importeMinimo: ctx.reward.importeMinimo,
                    descripcion: ctx.reward.descripcion,
                    tipoRewardId: ctx.reward.tipoRewardId ?? 1,
                    incluyeEnvioFisico: ctx.reward.incluyeEnvioFisico ?? false,
                    cantidadMaxima: ctx.reward.cantidadMaxima ?? 0,
                })

                expect(rewardId).toBeTruthy()

                collectedRewards.push({
                    campaignId: ctx.campaignId,
                    rewardName: ctx.reward.nombre,
                    rewardId,
                })
            } catch (error) {
                const errorMsg = String(error)
                if (
                    errorMsg.includes("already") ||
                    errorMsg.includes("ya existe") ||
                    errorMsg.includes("duplicate")
                ) {
                    // Reward already exists, fetch from API
                    const res = await fetch(
                        `${API_BASE}/api/campanias/${ctx.campaignId}/rewards`,
                        {
                            headers: { Authorization: `Bearer ${tokenEntry.token}` },
                        }
                    )
                    const data = await res.json()
                    const rewards = data.data?.items ?? data.data ?? []
                    const found = rewards.find(
                        (r: Record<string, unknown>) => r.nombre === ctx.reward.nombre
                    )

                    if (found?.id) {
                        collectedRewards.push({
                            campaignId: ctx.campaignId,
                            rewardName: ctx.reward.nombre,
                            rewardId: found.id,
                        })
                    } else {
                        throw error
                    }
                } else {
                    throw error
                }
            }
        }

        expect(collectedRewards.length).toBeGreaterThanOrEqual(allRewards.length)
    })

    // --- Save state for subsequent specs ---

    test("Save reward IDs to state file", async () => {
        expect(collectedRewards.length).toBeGreaterThanOrEqual(allRewards.length)
        saveRewardIds(collectedRewards)
    })

    // --- Final verification ---

    test("Verify total rewards count", async () => {
        expect(collectedRewards.length).toBeGreaterThanOrEqual(allRewards.length)

        // Verify each campaign has rewards
        const campaignsWithRewards = new Set(collectedRewards.map((r) => r.campaignId))
        const expectedCampaigns = new Set(allRewards.map((r) => r.campaignId))
        for (const cId of expectedCampaigns) {
            expect(campaignsWithRewards.has(cId)).toBeTruthy()
        }
    })
})
