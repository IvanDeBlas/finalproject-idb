import { test, expect } from "@playwright/test"
import { API_BASE } from "../helpers"
import { loadNeeds } from "../fixtures/test-data"
import {
    loadTokens,
    getToken,
    loadCampaignIds,
    saveNeedIds,
    type NeedEntry,
} from "../fixtures/seed-state"

/**
 * Spec 10: Create 10 crowdsourcing needs via API.
 * Each need is published by an artist for their campaign.
 * Requires: 01-register-artists (tokens), 03-create-campaigns (campaign IDs).
 *
 * @seeding
 */

const needs = loadNeeds()

/** Map modalidad string to API's modalidadTrabajoId */
function getModalidadId(modalidad: string): number {
    const map: Record<string, number> = {
        Presencial: 1,
        Remoto: 2,
        Hibrido: 3,
    }
    return map[modalidad] ?? 2
}

/** Map moneda string to monedaId */
function getMonedaId(moneda: string): number {
    return moneda === "EUR" ? 1 : 1
}

/** Get location info for Presencial/Hibrido needs based on artist and description */
function getLocation(
    need: (typeof needs)[0]
): { ciudad: string; pais: string } | null {
    const modalidadId = getModalidadId(need.modalidad)
    // Remoto (2) doesn't need location
    if (modalidadId === 2) return null

    // Extract location from need descriptions
    const locationMap: Record<string, { ciudad: string; pais: string }> = {
        N2: { ciudad: "Madrid", pais: "Spain" },
        N3: { ciudad: "Madrid", pais: "Spain" },
        N4: { ciudad: "Paris", pais: "France" },
        N5: { ciudad: "Madrid", pais: "Spain" },
        N9: { ciudad: "Sevilla", pais: "Spain" },
        N10: { ciudad: "Madrid", pais: "Spain" },
    }
    return locationMap[need.id] ?? { ciudad: "Madrid", pais: "Spain" }
}

const collectedNeeds: NeedEntry[] = []

test.describe("@seeding Spec 10: Create Crowdsourcing Needs", () => {
    test.describe.configure({ mode: "serial" })

    test(`Create ${needs.length} needs via API`, async () => {
        const campaignIds = loadCampaignIds()

        for (const need of needs) {
            // Get the artist's token
            const artistToken = getToken(need.artistaEmail)

            // Find the artist's campaign ID (proyectoArtisticoId)
            const campaignEntry = campaignIds.find(
                (c) => c.email === need.artistaEmail
            )
            if (!campaignEntry) {
                throw new Error(
                    `Campaign not found for artist ${need.artistaEmail} (${need.artistaNombre})`
                )
            }

            const modalidadId = getModalidadId(need.modalidad)
            const monedaId = getMonedaId(need.moneda)
            const location = getLocation(need)

            const now = new Date()
            const fechaLimite = new Date(
                now.getTime() + need.fechaLimiteDias * 24 * 60 * 60 * 1000
            )

            const body: Record<string, unknown> = {
                titulo: need.titulo,
                descripcion: need.descripcion,
                tipoNecesidadId: need.rolRequeridoId,
                modalidadTrabajoId: modalidadId,
                presupuestoMin: need.presupuestoMin,
                presupuestoMax: need.presupuestoMax,
                monedaId: monedaId,
                fechaLimitePropuestas: fechaLimite.toISOString(),
                proyectoArtisticoId: campaignEntry.campaignId,
            }

            if (location) {
                body.ubicacionCiudad = location.ciudad
                body.ubicacionPais = location.pais
            }

            const res = await fetch(
                `${API_BASE}/api/crowdsourcing/necesidades`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${artistToken.token}`,
                    },
                    body: JSON.stringify(body),
                }
            )

            const result = await res.json()
            const needId = result.data?.id ?? result.data?.necesidadId
            if (!needId) {
                throw new Error(
                    `Failed to create need "${need.titulo}" for ${need.artistaNombre}: ${JSON.stringify(result)}`
                )
            }

            collectedNeeds.push({
                dataId: need.id,
                artistaEmail: need.artistaEmail,
                titulo: need.titulo,
                needId: needId,
            })
        }

        expect(collectedNeeds.length).toBe(needs.length)
    })

    test("Save need IDs to state file", async () => {
        expect(collectedNeeds.length).toBe(needs.length)
        saveNeedIds(collectedNeeds)
    })

    test("Verify needs via GET endpoint", async () => {
        // Use any artist token to query public needs
        const tokens = loadTokens()
        const anyToken = tokens[0].token

        const res = await fetch(
            `${API_BASE}/api/crowdsourcing/necesidades?pageSize=50`,
            {
                headers: {
                    Authorization: `Bearer ${anyToken}`,
                },
            }
        )

        const result = await res.json()
        expect(res.ok).toBeTruthy()

        const items = result.data?.items ?? result.data ?? []
        // Verify at least our 10 needs exist
        expect(items.length).toBeGreaterThanOrEqual(needs.length)

        // Spot-check: verify MUSE need exists
        const museNeed = collectedNeeds.find((n) => n.dataId === "N1")
        if (museNeed) {
            const found = items.find(
                (item: Record<string, unknown>) => item.id === museNeed.needId
            )
            expect(found).toBeTruthy()
        }
    })
})
