import { test, expect } from "@playwright/test"
import {
    loginToAdminUI,
    createArtistaViaApi,
    loginViaApi,
    ADMIN_BASE,
    API_BASE,
} from "../helpers"
import { loadSeedDataWithFallback } from "../fixtures/test-data"
import {
    loadTokens,
    getToken,
    saveArtistaIds,
    type ArtistaEntry,
} from "../fixtures/seed-state"

/**
 * Spec 02: Create artist profiles for all registered artists.
 * - First 2 via Admin UI (validates profile form)
 * - Remaining via API (efficiency)
 * - Saves artistaIds to state file for subsequent specs.
 *
 * @seeding
 */

const seedData = loadSeedDataWithFallback()
const artists = seedData.artists
const UI_COUNT = 2

const collectedArtistas: ArtistaEntry[] = []

/**
 * Map generoMusical from seed data to the closest GENEROS_MUSICALES option.
 * The Admin form uses a select with fixed options.
 */
function mapGeneroToOption(generoMusical: string | undefined): string {
    if (!generoMusical) return ""

    const generoMap: Record<string, string> = {
        "Alternative Rock": "Rock",
        "Stoner Rock": "Rock",
        "Indie Rock": "Indie",
        "Art Rock": "Rock",
        "Reggaeton": "Reggaeton",
        "Flamenco Urbano": "Flamenco",
        "Pop Experimental": "Pop",
        "Rock Alternativo": "Rock",
        "Electropop": "Pop",
        "Indie Folk": "Folk",
        "Indie Pop": "Indie",
        "Electronica": "Electronica",
        "Electronic": "Electronica",
        "Ambient": "Electronica",
        "Neo-Flamenco": "Flamenco",
        "Jazz": "Jazz",
        "Pop": "Pop",
        "Rock": "Rock",
        "Hip Hop": "Hip Hop",
        "Folk": "Folk",
        "Clasica": "Clasica",
        "Metal": "Metal",
        "Indie": "Indie",
        "Flamenco": "Flamenco",
    }

    return generoMap[generoMusical] ?? "Otro"
}

test.describe("@seeding Spec 02: Create Artist Profiles", () => {
    test.describe.configure({ mode: "serial" })

    // --- UI Profile Creation (first 2 artists) ---

    for (let i = 0; i < Math.min(UI_COUNT, artists.length); i++) {
        const artist = artists[i]

        test(`UI create profile ${i + 1}: ${artist.profile.nombreArtistico}`, async ({ page }) => {
            // Login to Admin as this artist
            await loginToAdminUI(page, artist.email, artist.password)

            // Navigate to profile page
            await page.goto(`${ADMIN_BASE}/perfil`)
            await page.waitForTimeout(2000)

            // Check if profile form is visible (create mode)
            const nombreInput = page.getByLabel("Nombre artistico")
            await nombreInput.waitFor({ state: "visible", timeout: 10000 })

            // Fill the form
            await nombreInput.fill(artist.profile.nombreArtistico)

            // Select genero musical from dropdown
            const generoOption = mapGeneroToOption(artist.profile.generoMusical)
            if (generoOption) {
                await page.getByLabel("Genero musical").selectOption(generoOption)
            }

            // Fill biografia (textarea)
            await page.getByLabel("Biografia").fill(artist.profile.descripcion)

            // Fill imagen URL
            if (artist.profile.imagenUrl) {
                await page
                    .getByLabel("URL de imagen de perfil")
                    .fill(artist.profile.imagenUrl)
            }

            // Submit form
            await page
                .getByRole("button", { name: /Crear perfil/i })
                .click()

            // Wait for success toast or page update
            await expect(
                page.getByText(/Perfil creado|creado exitosamente/i).first()
            ).toBeVisible({ timeout: 10000 })

            // Get artistaId via API
            const tokenEntry = getToken(artist.email)
            const meRes = await fetch(`${API_BASE}/api/artistas/me`, {
                headers: { Authorization: `Bearer ${tokenEntry.token}` },
            })
            const meData = await meRes.json()
            const artistaId = meData.data?.id

            expect(artistaId).toBeTruthy()

            collectedArtistas.push({
                email: artist.email,
                artistaId,
            })
        })
    }

    // --- API Profile Creation (remaining artists) ---

    if (artists.length > UI_COUNT) {
        test(`API create profiles for remaining ${artists.length - UI_COUNT} artists`, async () => {
            for (let i = UI_COUNT; i < artists.length; i++) {
                const artist = artists[i]
                const tokenEntry = getToken(artist.email)

                try {
                    const artistaId = await createArtistaViaApi(
                        tokenEntry.token,
                        {
                            nombreArtistico: artist.profile.nombreArtistico,
                            descripcion: artist.profile.descripcion,
                            pais: artist.profile.pais ?? "Spain",
                            ciudad: artist.profile.ciudad ?? "Madrid",
                            imagenUrl: artist.profile.imagenUrl,
                        }
                    )

                    expect(artistaId).toBeTruthy()
                    collectedArtistas.push({
                        email: artist.email,
                        artistaId,
                    })
                } catch (error) {
                    // If profile already exists (re-run), fetch existing
                    const errorMsg = String(error)
                    if (
                        errorMsg.includes("already") ||
                        errorMsg.includes("ya existe") ||
                        errorMsg.includes("409")
                    ) {
                        const meRes = await fetch(
                            `${API_BASE}/api/artistas/me`,
                            {
                                headers: {
                                    Authorization: `Bearer ${tokenEntry.token}`,
                                },
                            }
                        )
                        const meData = await meRes.json()
                        const existingId = meData.data?.id
                        if (existingId) {
                            collectedArtistas.push({
                                email: artist.email,
                                artistaId: existingId,
                            })
                        } else {
                            throw error
                        }
                    } else {
                        throw error
                    }
                }
            }

            expect(collectedArtistas.length).toBe(artists.length)
        })
    }

    // --- Save state for subsequent specs ---

    test("Save artista IDs to state file", async () => {
        expect(collectedArtistas.length).toBeGreaterThanOrEqual(artists.length)
        saveArtistaIds(collectedArtistas)
    })

    // --- Final verification ---

    test("Verify all artist profiles exist via API", async () => {
        for (const entry of collectedArtistas) {
            const res = await fetch(
                `${API_BASE}/api/artistas/${entry.artistaId}`
            )
            const data = await res.json()

            expect(res.ok).toBeTruthy()

            const artist = artists.find((a) => a.email === entry.email)
            expect(data.data?.nombreArtistico).toBe(
                artist?.profile.nombreArtistico
            )
        }
    })
})
