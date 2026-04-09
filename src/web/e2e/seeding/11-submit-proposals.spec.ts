import { test, expect } from "@playwright/test"
import { API_BASE } from "../helpers"
import { loadProposals } from "../fixtures/test-data"
import {
    loadProfessionalTokens,
    getProfessionalToken,
    loadNeedIds,
    getNeedId,
    saveProposalIds,
    type ProposalEntry,
} from "../fixtures/seed-state"

/**
 * Spec 11: Submit 18 proposals from professionals to needs.
 * All via API. Each proposal links a professional to a need.
 * Requires: 09-register-professionals (tokens), 10-create-needs (need IDs).
 *
 * @seeding
 */

const proposals = loadProposals()

/** Parse duration string to estimated days */
function parseDurationToDays(duracion: string): number | undefined {
    const semanas = duracion.match(/(\d+)\s*semana/i)
    if (semanas) return parseInt(semanas[1], 10) * 7

    const dias = duracion.match(/(\d+)\s*dia/i)
    if (dias) return parseInt(dias[1], 10)

    const meses = duracion.match(/(\d+)\s*mes/i)
    if (meses) return parseInt(meses[1], 10) * 30

    const sesiones = duracion.match(/(\d+)\s*sesion/i)
    if (sesiones) return parseInt(sesiones[1], 10) * 2

    const conciertos = duracion.match(/(\d+)\s*concierto/i)
    if (conciertos) return parseInt(conciertos[1], 10) * 3

    return undefined
}

/** Map moneda string to monedaId */
function getMonedaId(moneda: string): number {
    return moneda === "EUR" ? 1 : 1
}

const collectedProposals: ProposalEntry[] = []

test.describe("@seeding Spec 11: Submit Proposals", () => {
    test.describe.configure({ mode: "serial" })

    test(`Submit ${proposals.length} proposals via API`, async () => {
        for (const proposal of proposals) {
            // Get the professional's token
            const profToken = getProfessionalToken(proposal.profesionalEmail)

            // Get the actual need UUID from state
            const needUuid = getNeedId(proposal.necesidadId)

            const body: Record<string, unknown> = {
                precioPropuesto: proposal.precioPropuesto,
                monedaId: getMonedaId(proposal.moneda),
                mensajePropuesta: proposal.mensaje,
            }

            const estimatedDays = parseDurationToDays(proposal.duracion)
            if (estimatedDays) {
                body.diasEstimados = estimatedDays
            }

            const res = await fetch(
                `${API_BASE}/api/crowdsourcing/necesidades/${needUuid}/propuestas`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${profToken.token}`,
                    },
                    body: JSON.stringify(body),
                }
            )

            const result = await res.json()
            const proposalId = result.data?.id ?? result.data?.propuestaId
            if (!proposalId) {
                throw new Error(
                    `Failed to create proposal "${proposal.id}" (${proposal.profesionalNombre} -> ${proposal.necesidadTitulo}): ${JSON.stringify(result)}`
                )
            }

            collectedProposals.push({
                dataId: proposal.id,
                necesidadDataId: proposal.necesidadId,
                profesionalEmail: proposal.profesionalEmail,
                proposalId: proposalId,
                estadoFinalId: proposal.estadoFinalId,
            })
        }

        expect(collectedProposals.length).toBe(proposals.length)
    })

    test("Save proposal IDs to state file", async () => {
        expect(collectedProposals.length).toBe(proposals.length)
        saveProposalIds(collectedProposals)
    })

    test("Verify proposals summary", async () => {
        // Count proposals by final status
        const accepted = collectedProposals.filter(
            (p) => p.estadoFinalId === 2
        )
        const rejected = collectedProposals.filter(
            (p) => p.estadoFinalId === 3
        )
        const pending = collectedProposals.filter(
            (p) => p.estadoFinalId === 1
        )

        expect(accepted.length).toBe(8)
        expect(rejected.length).toBe(6)
        expect(pending.length).toBe(4)
        expect(collectedProposals.length).toBe(18)
    })

    test("Verify proposals via API (spot check)", async () => {
        // Use a professional token to check their proposals
        const profTokens = loadProfessionalTokens()
        const sarahToken = profTokens.find(
            (t) => t.email === "sarah.chen@weplay-test.com"
        )
        if (!sarahToken) {
            throw new Error("Sarah Chen token not found")
        }

        const res = await fetch(
            `${API_BASE}/api/crowdsourcing/propuestas/mis-propuestas?pageSize=50`,
            {
                headers: {
                    Authorization: `Bearer ${sarahToken.token}`,
                },
            }
        )

        const result = await res.json()
        expect(res.ok).toBeTruthy()

        const items = result.data?.items ?? result.data ?? []
        // Sarah Chen submitted 3 proposals: PR1 (N1), PR7 (N4), PR13 (N7)
        expect(items.length).toBeGreaterThanOrEqual(3)
    })
})
