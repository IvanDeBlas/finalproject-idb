import { test, expect } from "@playwright/test"
import { API_BASE } from "../helpers"
import { loadAgreementsFile } from "../fixtures/test-data"
import {
    loadProposalIds,
    getProposalId,
    getToken,
    getProfessionalToken,
    getNeedId,
    saveAgreementIds,
    saveMilestoneIds,
    saveDeliverableIds,
    type AgreementEntry,
    type MilestoneEntry,
    type DeliverableEntry,
} from "../fixtures/seed-state"

/**
 * Spec 12: Manage Agreements (Acuerdos), Milestones, and Deliverables.
 *
 * Flow:
 * 1. Artists accept 8 proposals (creating 6 agreements, some proposals share needs)
 * 2. Artists reject 6 proposals
 * 3. Create 8 milestones distributed across agreements
 * 4. Create 12 deliverables linked to milestones
 * 5. Approve deliverables for completed agreements (A1, A2, A3)
 * 6. Complete agreements A1, A2, A3
 * 7. Cancel agreement A6
 *
 * Requires: 11-submit-proposals (proposal IDs), 01-register-artists (tokens)
 *
 * @seeding
 */

const agreementsFile = loadAgreementsFile()

/** Calculate a date relative to today */
function relativeDate(daysFromToday: number): Date {
    const d = new Date()
    d.setDate(d.getDate() + daysFromToday)
    return d
}

/**
 * Map agreement data ID (e.g. "N1") to the proposal data ID that was accepted.
 * Based on the plan: each agreement links to a specific proposal.
 * Agreement necesidadId -> find accepted proposal for that need.
 */
function findProposalDataIdForAgreement(
    agreementNecesidadId: string,
    agreementProfesionalEmail: string
): string {
    const proposals = loadProposalIds()
    const entry = proposals.find(
        (p) =>
            p.necesidadDataId === agreementNecesidadId &&
            p.profesionalEmail === agreementProfesionalEmail &&
            p.estadoFinalId === 2 // accepted
    )
    if (!entry) {
        throw new Error(
            `No accepted proposal found for need ${agreementNecesidadId} by ${agreementProfesionalEmail}`
        )
    }
    return entry.dataId
}

const collectedAgreements: AgreementEntry[] = []
const collectedMilestones: MilestoneEntry[] = []
const collectedDeliverables: DeliverableEntry[] = []

test.describe("@seeding Spec 12: Manage Agreements", () => {
    test.describe.configure({ mode: "serial" })

    // --- Step 1: Accept proposals (creates agreements) ---

    test("Accept 8 proposals and create 6 agreements", async () => {
        for (const agreement of agreementsFile.agreements) {
            // Find which proposal to accept
            const proposalDataId = findProposalDataIdForAgreement(
                agreement.necesidadId,
                agreement.profesionalEmail
            )
            const proposalUuid = getProposalId(proposalDataId)

            // Artist accepts the proposal
            const artistToken = getToken(agreement.artistaEmail)

            const fechaInicio = relativeDate(agreement.fechaInicioDiasRelativo)
            const fechaFin = relativeDate(agreement.fechaFinDiasRelativo)

            const res = await fetch(
                `${API_BASE}/api/crowdsourcing/propuestas/${proposalUuid}/aceptar`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${artistToken.token}`,
                    },
                    body: JSON.stringify({
                        tituloInterno: agreement.necesidadTitulo,
                        fechaInicio: fechaInicio.toISOString(),
                        fechaFinPrevista: fechaFin.toISOString(),
                    }),
                }
            )

            const result = await res.json()
            const acuerdoId =
                result.data?.acuerdoId ?? result.data?.id
            if (!acuerdoId) {
                throw new Error(
                    `Failed to accept proposal "${proposalDataId}" for agreement "${agreement.id}": ${JSON.stringify(result)}`
                )
            }

            collectedAgreements.push({
                dataId: agreement.id,
                proposalDataId,
                necesidadDataId: agreement.necesidadId,
                artistaEmail: agreement.artistaEmail,
                profesionalEmail: agreement.profesionalEmail,
                agreementId: acuerdoId,
                estadoId: 1, // Activo initially
                conversacionId: result.data?.conversacionId,
            })
        }

        expect(collectedAgreements.length).toBe(6)
    })

    // --- Step 2: Reject remaining proposals ---

    test("Reject 6 proposals", async () => {
        const proposals = loadProposalIds()
        const toReject = proposals.filter((p) => p.estadoFinalId === 3) // rejected

        let rejectedCount = 0
        for (const proposal of toReject) {
            // Find the artist who owns the need
            const needEntries = await import("../fixtures/seed-state").then(
                (m) => m.loadNeedIds()
            )
            const needEntry = needEntries.find(
                (n) => n.dataId === proposal.necesidadDataId
            )
            if (!needEntry) continue

            const artistToken = getToken(needEntry.artistaEmail)

            const res = await fetch(
                `${API_BASE}/api/crowdsourcing/propuestas/${proposal.proposalId}/rechazar`,
                {
                    method: "PATCH",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${artistToken.token}`,
                    },
                    body: JSON.stringify({
                        motivo: "No encaja con lo que buscamos para este proyecto",
                    }),
                }
            )

            if (res.ok) {
                rejectedCount++
            }
        }

        expect(rejectedCount).toBe(6)
    })

    // --- Step 3: Create milestones ---

    test("Create 8 milestones across agreements", async () => {
        for (const milestone of agreementsFile.milestones) {
            const agreementEntry = collectedAgreements.find(
                (a) => a.dataId === milestone.acuerdoId
            )
            if (!agreementEntry) {
                throw new Error(
                    `Agreement not found for milestone ${milestone.id} (acuerdoId: ${milestone.acuerdoId})`
                )
            }

            // The professional creates milestones (they do the work planning)
            const profToken = getProfessionalToken(
                agreementEntry.profesionalEmail
            )

            const deadline = relativeDate(milestone.deadlineDiasRelativo)

            const res = await fetch(
                `${API_BASE}/api/crowdsourcing/acuerdos/${agreementEntry.agreementId}/milestones`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${profToken.token}`,
                    },
                    body: JSON.stringify({
                        titulo: milestone.titulo,
                        importeParcial: milestone.monto,
                        fechaLimite: deadline.toISOString(),
                    }),
                }
            )

            const result = await res.json()
            const milestoneId =
                result.data?.milestoneId ?? result.data?.id
            if (!milestoneId) {
                throw new Error(
                    `Failed to create milestone "${milestone.id}": ${JSON.stringify(result)}`
                )
            }

            collectedMilestones.push({
                dataId: milestone.id,
                acuerdoDataId: milestone.acuerdoId,
                agreementId: agreementEntry.agreementId,
                milestoneId,
                titulo: milestone.titulo,
            })
        }

        expect(collectedMilestones.length).toBe(8)
    })

    // --- Step 4: Create deliverables ---

    test("Create 12 deliverables linked to milestones", async () => {
        for (const deliverable of agreementsFile.deliverables) {
            const milestoneEntry = collectedMilestones.find(
                (m) => m.dataId === deliverable.milestoneId
            )
            if (!milestoneEntry) {
                throw new Error(
                    `Milestone not found for deliverable ${deliverable.id} (milestoneId: ${deliverable.milestoneId})`
                )
            }

            const agreementEntry = collectedAgreements.find(
                (a) => a.dataId === deliverable.acuerdoId
            )
            if (!agreementEntry) {
                throw new Error(
                    `Agreement not found for deliverable ${deliverable.id}`
                )
            }

            // The professional submits deliverables
            const profToken = getProfessionalToken(
                agreementEntry.profesionalEmail
            )

            const res = await fetch(
                `${API_BASE}/api/crowdsourcing/acuerdos/${agreementEntry.agreementId}/entregables`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${profToken.token}`,
                    },
                    body: JSON.stringify({
                        titulo: deliverable.titulo,
                        milestoneId: milestoneEntry.milestoneId,
                    }),
                }
            )

            const result = await res.json()
            const deliverableId =
                result.data?.entregableId ?? result.data?.id
            if (!deliverableId) {
                throw new Error(
                    `Failed to create deliverable "${deliverable.id}": ${JSON.stringify(result)}`
                )
            }

            collectedDeliverables.push({
                dataId: deliverable.id,
                milestoneDataId: deliverable.milestoneId,
                milestoneId: milestoneEntry.milestoneId,
                acuerdoDataId: deliverable.acuerdoId,
                agreementId: agreementEntry.agreementId,
                deliverableId,
                titulo: deliverable.titulo,
                estadoId: deliverable.estadoId,
            })
        }

        expect(collectedDeliverables.length).toBe(12)
    })

    // --- Step 5: Approve deliverables for completed agreements (A1, A2, A3) ---

    test("Approve 9 deliverables for completed agreements", async () => {
        const toApprove = collectedDeliverables.filter(
            (d) => d.estadoId === 2 // Aprobado
        )

        let approvedCount = 0
        for (const deliverable of toApprove) {
            const agreementEntry = collectedAgreements.find(
                (a) => a.dataId === deliverable.acuerdoDataId
            )
            if (!agreementEntry) continue

            // Artist approves deliverables
            const artistToken = getToken(agreementEntry.artistaEmail)

            const res = await fetch(
                `${API_BASE}/api/crowdsourcing/entregables/${deliverable.deliverableId}/aprobar`,
                {
                    method: "PATCH",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${artistToken.token}`,
                    },
                    body: JSON.stringify({
                        comentario: "Aprobado - excelente trabajo",
                    }),
                }
            )

            if (res.ok) {
                approvedCount++
            }
        }

        expect(approvedCount).toBe(9)
    })

    // --- Step 6: Complete agreements A1, A2, A3 ---

    test("Complete 3 agreements (A1, A2, A3)", async () => {
        const toComplete = ["A1", "A2", "A3"]
        let completedCount = 0

        for (const dataId of toComplete) {
            const agreementEntry = collectedAgreements.find(
                (a) => a.dataId === dataId
            )
            if (!agreementEntry) continue

            const artistToken = getToken(agreementEntry.artistaEmail)

            const res = await fetch(
                `${API_BASE}/api/crowdsourcing/acuerdos/${agreementEntry.agreementId}/completar`,
                {
                    method: "PATCH",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${artistToken.token}`,
                    },
                }
            )

            if (res.ok) {
                agreementEntry.estadoId = 2 // Completado
                completedCount++
            }
        }

        expect(completedCount).toBe(3)
    })

    // --- Step 7: Cancel agreement A6 ---

    test("Cancel agreement A6 (C. Tangana)", async () => {
        const a6 = collectedAgreements.find((a) => a.dataId === "A6")
        expect(a6).toBeTruthy()

        const a6Data = agreementsFile.agreements.find((a) => a.id === "A6")
        const artistToken = getToken(a6!.artistaEmail)

        const res = await fetch(
            `${API_BASE}/api/crowdsourcing/acuerdos/${a6!.agreementId}/cancelar`,
            {
                method: "PATCH",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${artistToken.token}`,
                },
                body: JSON.stringify({
                    motivo:
                        a6Data?.motivoCancelacion ??
                        "Concierto pospuesto a nueva fecha por determinar.",
                }),
            }
        )

        expect(res.ok).toBeTruthy()
        a6!.estadoId = 3 // Cancelado
    })

    // --- Step 8: Save state ---

    test("Save agreement, milestone, and deliverable IDs to state", async () => {
        expect(collectedAgreements.length).toBe(6)
        expect(collectedMilestones.length).toBe(8)
        expect(collectedDeliverables.length).toBe(12)

        saveAgreementIds(collectedAgreements)
        saveMilestoneIds(collectedMilestones)
        saveDeliverableIds(collectedDeliverables)
    })

    // --- Step 9: Verify summary ---

    test("Verify agreements summary", async () => {
        const completed = collectedAgreements.filter(
            (a) => a.estadoId === 2
        )
        const active = collectedAgreements.filter((a) => a.estadoId === 1)
        const cancelled = collectedAgreements.filter(
            (a) => a.estadoId === 3
        )

        expect(completed.length).toBe(3)
        expect(active.length).toBe(2)
        expect(cancelled.length).toBe(1)
        expect(collectedAgreements.length).toBe(6)
    })

    test("Verify via API (spot check A1)", async () => {
        const a1 = collectedAgreements.find((a) => a.dataId === "A1")
        expect(a1).toBeTruthy()

        const artistToken = getToken(a1!.artistaEmail)

        const res = await fetch(
            `${API_BASE}/api/crowdsourcing/acuerdos/${a1!.agreementId}`,
            {
                headers: {
                    Authorization: `Bearer ${artistToken.token}`,
                },
            }
        )

        expect(res.ok).toBeTruthy()
        const result = await res.json()
        const data = result.data
        expect(data).toBeTruthy()
    })
})
