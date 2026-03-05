import { test, expect } from "@playwright/test"
import { API_BASE } from "../helpers"
import { loadAgreementsFile } from "../fixtures/test-data"
import {
    getToken,
    getProfessionalToken,
    getAgreementId,
    type TokenEntry,
} from "../fixtures/seed-state"

/**
 * Spec 14: Create 6 bidirectional ratings for completed agreements.
 *
 * Only completed agreements (A1, A2, A3) receive ratings.
 * Each agreement gets 2 ratings: artist rates professional, professional rates artist.
 *
 * Requires: 12-manage-agreements (agreement IDs with completed status)
 *
 * @seeding
 */

const agreementsFile = loadAgreementsFile()
const ratings = agreementsFile.ratings

/**
 * Get the token for a rater by email.
 * Tries artist tokens first, then professional tokens.
 */
function getRaterToken(email: string): TokenEntry {
    try {
        return getToken(email)
    } catch {
        return getProfessionalToken(email)
    }
}

test.describe("@seeding Spec 14: Crowdsourcing Ratings", () => {
    test.describe.configure({ mode: "serial" })

    test(`Create ${ratings.length} ratings for completed agreements`, async () => {
        let createdCount = 0

        for (const rating of ratings) {
            const agreementUuid = getAgreementId(rating.acuerdoId)
            const raterToken = getRaterToken(rating.valoradorEmail)

            const res = await fetch(
                `${API_BASE}/api/crowdsourcing/acuerdos/${agreementUuid}/valoraciones`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${raterToken.token}`,
                    },
                    body: JSON.stringify({
                        puntuacion: rating.puntuacion,
                        comentario: rating.comentario,
                    }),
                }
            )

            const result = await res.json()
            if (res.ok || res.status === 201) {
                createdCount++
            } else {
                throw new Error(
                    `Failed to create rating "${rating.id}" (${rating.valoradorNombre} -> ${rating.valoradoNombre}): ${JSON.stringify(result)}`
                )
            }
        }

        expect(createdCount).toBe(6)
    })

    test("Verify ratings for A1 (MUSE <-> Sarah Chen)", async () => {
        const a1Id = getAgreementId("A1")

        // Check from MUSE side
        const museToken = getToken("muse.official@weplay-test.com")
        const museRes = await fetch(
            `${API_BASE}/api/crowdsourcing/usuarios/${museToken.userId}/valoraciones?pageSize=50`,
            {
                headers: {
                    Authorization: `Bearer ${museToken.token}`,
                },
            }
        )

        expect(museRes.ok).toBeTruthy()
        const museResult = await museRes.json()
        const museRatings = museResult.data?.items ?? museResult.data ?? []
        // MUSE should have at least 1 rating (given or received)
        expect(museRatings.length).toBeGreaterThanOrEqual(1)

        // Check from Sarah side
        const sarahToken = getProfessionalToken(
            "sarah.chen@weplay-test.com"
        )
        const sarahRes = await fetch(
            `${API_BASE}/api/crowdsourcing/usuarios/${sarahToken.userId}/valoraciones?pageSize=50`,
            {
                headers: {
                    Authorization: `Bearer ${sarahToken.token}`,
                },
            }
        )

        expect(sarahRes.ok).toBeTruthy()
        const sarahResult = await sarahRes.json()
        const sarahRatings =
            sarahResult.data?.items ?? sarahResult.data ?? []
        expect(sarahRatings.length).toBeGreaterThanOrEqual(1)
    })

    test("Verify rating summary counts", async () => {
        // Count ratings by agreement
        const ratingsByAgreement = new Map<string, number>()
        for (const rating of ratings) {
            const count = ratingsByAgreement.get(rating.acuerdoId) ?? 0
            ratingsByAgreement.set(rating.acuerdoId, count + 1)
        }

        // Each completed agreement should have 2 ratings (bidirectional)
        expect(ratingsByAgreement.get("A1")).toBe(2)
        expect(ratingsByAgreement.get("A2")).toBe(2)
        expect(ratingsByAgreement.get("A3")).toBe(2)
        expect(ratings.length).toBe(6)
    })

    test("Verify ratings are bidirectional", async () => {
        // For each completed agreement, verify both parties rated
        const completedAgreements = ["A1", "A2", "A3"]

        for (const agreementDataId of completedAgreements) {
            const agreementRatings = ratings.filter(
                (r) => r.acuerdoId === agreementDataId
            )
            expect(agreementRatings.length).toBe(2)

            // Verify different raters
            const raters = agreementRatings.map((r) => r.valoradorEmail)
            expect(new Set(raters).size).toBe(2)

            // Verify all puntuaciones are 4 or 5
            for (const r of agreementRatings) {
                expect(r.puntuacion).toBeGreaterThanOrEqual(4)
                expect(r.puntuacion).toBeLessThanOrEqual(5)
            }
        }
    })
})
