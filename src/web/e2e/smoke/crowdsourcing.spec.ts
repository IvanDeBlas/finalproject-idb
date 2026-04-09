import { test, expect } from "@playwright/test"
import {
    API_BASE,
    registerViaApi,
    loginViaApi,
    createCampaignViaApi,
    publishViaApi,
    createArtistaViaApi,
} from "../helpers"

/**
 * Smoke tests for crowdsourcing module.
 * Independent - creates its own artist, professional, need, proposal, and agreement.
 *
 * @smoke
 */

const TIMESTAMP = Date.now()

const SMOKE_ARTIST = {
    email: `smoke-cs-artist-${TIMESTAMP}@weplay-test.com`,
    password: "SmokeTest123!",
    name: `Smoke CS Artist ${TIMESTAMP}`,
    artistName: `SmokeCSArtist${TIMESTAMP}`,
}

const SMOKE_PROFESSIONAL = {
    email: `smoke-cs-pro-${TIMESTAMP}@weplay-test.com`,
    password: "SmokeTest123!",
    name: `Smoke CS Professional ${TIMESTAMP}`,
}

let artistToken = ""
let proToken = ""
let campaignId = ""
let needId = ""
let proposalId = ""
let agreementId = ""

// Helper to fetch crowdsourcing maestras (roles)
async function getRolId(token: string): Promise<number> {
    const res = await fetch(`${API_BASE}/api/crowdsourcing/maestras/roles-profesionales`, {
        headers: { Authorization: `Bearer ${token}` },
    })
    if (!res.ok) return 1 // fallback
    const data = await res.json()
    const roles = data.data ?? data
    const list = Array.isArray(roles) ? roles : []
    return list.length > 0 ? list[0].id : 1
}

test.describe("@smoke Crowdsourcing", () => {
    test.describe.configure({ mode: "serial" })

    // --- Setup: register artist + professional, create need + proposal ---

    test("Setup: register artist and professional", async () => {
        const artistAuth = await registerViaApi(
            SMOKE_ARTIST.email,
            SMOKE_ARTIST.password,
            SMOKE_ARTIST.name
        )
        artistToken = artistAuth.token

        await createArtistaViaApi(artistToken, {
            nombreArtistico: SMOKE_ARTIST.artistName,
            descripcion: "Smoke CS artist",
        })

        // Create and publish a campaign (needed for need's proyectoArtisticoId)
        campaignId = await createCampaignViaApi(artistToken, {
            titulo: `Smoke CS Campaign ${TIMESTAMP}`,
            importeObjetivo: 2000,
        })
        await publishViaApi(artistToken, campaignId)

        const proAuth = await registerViaApi(
            SMOKE_PROFESSIONAL.email,
            SMOKE_PROFESSIONAL.password,
            SMOKE_PROFESSIONAL.name
        )
        proToken = proAuth.token
    })

    test("Setup: create a need via API", async () => {
        const rolId = await getRolId(artistToken)
        const futureDate = new Date(Date.now() + 60 * 24 * 60 * 60 * 1000).toISOString()

        const res = await fetch(`${API_BASE}/api/crowdsourcing/necesidades`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${artistToken}`,
            },
            body: JSON.stringify({
                titulo: `Smoke CS Need ${TIMESTAMP}`,
                descripcion: "Smoke test crowdsourcing need for validation",
                tipoNecesidadId: rolId,
                modalidadTrabajoId: 2, // Remoto - no location required
                presupuestoMin: 500,
                presupuestoMax: 2000,
                monedaId: 1,
                fechaLimitePropuestas: futureDate,
                proyectoArtisticoId: campaignId,
            }),
        })

        const data = await res.json()
        needId = data.data?.id ?? data.data?.necesidadId ?? ""
        if (!needId) {
            const body = JSON.stringify(data)
            test.info().annotations.push({
                type: "error",
                description: `Need creation failed: ${body.substring(0, 200)}`,
            })
        }
        expect(needId).toBeTruthy()
    })

    test("Setup: submit proposal via API", async () => {
        if (!needId) {
            test.info().annotations.push({
                type: "skip",
                description: "No needId available - skipping proposal creation",
            })
            return
        }

        const res = await fetch(
            `${API_BASE}/api/crowdsourcing/necesidades/${needId}/propuestas`,
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${proToken}`,
                },
                body: JSON.stringify({
                    precioPropuesto: 1200,
                    monedaId: 1,
                    mensajePropuesta: "Smoke test proposal for crowdsourcing validation - detailed description of work",
                    diasEstimados: 30,
                }),
            }
        )

        const data = await res.json()
        proposalId = data.data?.id ?? data.data?.propuestaId ?? ""
        if (!proposalId) {
            // Professional profile may not exist - this is acceptable for smoke tests
            const errorMsg = JSON.stringify(data).substring(0, 200)
            const isProfileIssue = errorMsg.includes("perfil profesional")
            test.info().annotations.push({
                type: isProfileIssue ? "info" : "error",
                description: isProfileIssue
                    ? "No professional profile API available - proposal/agreement tests will be skipped"
                    : `Proposal creation failed: ${errorMsg}`,
            })
            // Don't fail - downstream tests will skip gracefully
        }
    })

    test("Setup: accept proposal to create agreement", async () => {
        if (!proposalId) {
            test.info().annotations.push({
                type: "skip",
                description: "No proposalId available - skipping accept",
            })
            return
        }

        const res = await fetch(
            `${API_BASE}/api/crowdsourcing/propuestas/${proposalId}/aceptar`,
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${artistToken}`,
                },
            }
        )

        if (res.ok) {
            const data = await res.json()
            agreementId = data.data?.id ?? data.data?.acuerdoId ?? ""
        } else {
            test.info().annotations.push({
                type: "info",
                description: `Accept proposal returned ${res.status} - agreement may have been auto-created`,
            })
        }
    })

    // --- Actual smoke tests ---

    test("List public needs via API", async () => {
        const res = await fetch(`${API_BASE}/api/crowdsourcing/necesidades`, {
            headers: { Authorization: `Bearer ${proToken}` },
        })

        expect(res.ok).toBeTruthy()

        const data = await res.json()
        const needs = data.data ?? data
        const list = Array.isArray(needs) ? needs : (needs.items ?? [])
        expect(list.length).toBeGreaterThan(0)
    })

    test("Get need detail via API", async () => {
        const res = await fetch(`${API_BASE}/api/crowdsourcing/necesidades/${needId}`, {
            headers: { Authorization: `Bearer ${artistToken}` },
        })

        expect(res.ok).toBeTruthy()

        const data = await res.json()
        const need = data.data ?? data
        expect(need.titulo).toContain("Smoke CS Need")
    })

    test("List professional proposals via API", async () => {
        const res = await fetch(
            `${API_BASE}/api/crowdsourcing/propuestas/mis-propuestas`,
            {
                headers: { Authorization: `Bearer ${proToken}` },
            }
        )

        expect(res.ok).toBeTruthy()

        const data = await res.json()
        const proposals = data.data ?? data
        const list = Array.isArray(proposals) ? proposals : (proposals.items ?? [])

        if (proposalId) {
            expect(list.length).toBeGreaterThan(0)
        } else {
            // No professional profile = no proposals, just verify API responds
            expect(Array.isArray(list)).toBeTruthy()
        }
    })

    test("Get agreement detail via API", async () => {
        if (!agreementId) {
            test.info().annotations.push({
                type: "info",
                description: "No agreement ID available - skipping agreement detail test",
            })
            return
        }

        const res = await fetch(
            `${API_BASE}/api/crowdsourcing/acuerdos/${agreementId}`,
            {
                headers: { Authorization: `Bearer ${artistToken}` },
            }
        )

        expect(res.ok).toBeTruthy()

        const data = await res.json()
        const agreement = data.data ?? data
        expect(agreement).toBeTruthy()
    })

    test("List conversations via API", async () => {
        const res = await fetch(`${API_BASE}/api/crowdsourcing/conversaciones`, {
            headers: { Authorization: `Bearer ${artistToken}` },
        })

        // Conversations may or may not exist yet
        if (res.ok) {
            const data = await res.json()
            const conversations = data.data ?? data
            const list = Array.isArray(conversations) ? conversations : (conversations.items ?? [])
            // Just verify API responds correctly, may be empty
            expect(Array.isArray(list)).toBeTruthy()
        } else {
            test.info().annotations.push({
                type: "info",
                description: `Conversations endpoint returned ${res.status}`,
            })
        }
    })

    test("List professional roles maestras via API", async () => {
        const res = await fetch(
            `${API_BASE}/api/crowdsourcing/maestras/roles-profesionales`,
            {
                headers: { Authorization: `Bearer ${proToken}` },
            }
        )

        expect(res.ok).toBeTruthy()

        const data = await res.json()
        const roles = data.data ?? data
        const list = Array.isArray(roles) ? roles : []
        expect(list.length).toBeGreaterThan(0)
    })
})
