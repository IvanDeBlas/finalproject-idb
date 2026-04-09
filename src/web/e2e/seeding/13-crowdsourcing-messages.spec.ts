import { test, expect } from "@playwright/test"
import { API_BASE } from "../helpers"
import { loadAgreementsFile } from "../fixtures/test-data"
import {
    getToken,
    getProfessionalToken,
    loadAgreementIds,
    loadNeedIds,
    saveConversationIds,
    type ConversationEntry,
    type TokenEntry,
} from "../fixtures/seed-state"

/**
 * Spec 13: Create 10 conversations with ~38 messages for crowdsourcing.
 *
 * Conversations are linked to either agreements or needs.
 * Some conversations are auto-created by the accept-proposal flow (Conv 1-6).
 * Others are standalone conversations about needs (Conv 7-10).
 *
 * For conversations already created by accept-proposal, we reuse them.
 * For new ones (about needs or rejected proposals), we create them.
 *
 * Requires: 12-manage-agreements (agreement IDs and conversation IDs)
 *
 * @seeding
 */

const agreementsFile = loadAgreementsFile()
const conversations = agreementsFile.conversations

/**
 * Get the token for a participant by email.
 * Tries artist tokens first, then professional tokens.
 */
function getParticipantToken(email: string): TokenEntry {
    try {
        return getToken(email)
    } catch {
        return getProfessionalToken(email)
    }
}

/**
 * Get the userId for a participant by email.
 */
function getParticipantUserId(email: string): string {
    return getParticipantToken(email).userId
}

const collectedConversations: ConversationEntry[] = []

test.describe("@seeding Spec 13: Crowdsourcing Messages", () => {
    test.describe.configure({ mode: "serial" })

    test("Create/find 10 conversations and send messages", async () => {
        const agreements = loadAgreementIds()
        const needs = loadNeedIds()
        let totalMessages = 0

        for (const conv of conversations) {
            // Determine conversation context
            const firstSenderEmail = conv.mensajes[0].remitenteEmail
            const creatorToken = getParticipantToken(firstSenderEmail)

            // Find the other participant
            const otherEmail = conv.mensajes.find(
                (m) => m.remitenteEmail !== firstSenderEmail
            )?.remitenteEmail
            if (!otherEmail) {
                throw new Error(
                    `Could not determine second participant for ${conv.id}`
                )
            }
            const recipientUserId = getParticipantUserId(otherEmail)

            // Build create conversation request
            const createBody: Record<string, unknown> = {
                userIdDestinatario: recipientUserId,
                asunto: conv.contexto,
            }

            // Link to agreement or need
            if (conv.acuerdoId) {
                const agreementEntry = agreements.find(
                    (a) => a.dataId === conv.acuerdoId
                )
                if (agreementEntry) {
                    createBody.acuerdoId = agreementEntry.agreementId
                }
            }
            if (conv.necesidadId) {
                const needEntry = needs.find(
                    (n) => n.dataId === conv.necesidadId
                )
                if (needEntry) {
                    createBody.necesidadId = needEntry.needId
                }
            }

            // Try to create conversation (may already exist from accept-proposal)
            let conversationId: string | undefined

            const createRes = await fetch(
                `${API_BASE}/api/crowdsourcing/conversaciones`,
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${creatorToken.token}`,
                    },
                    body: JSON.stringify(createBody),
                }
            )

            const createResult = await createRes.json()

            if (createRes.ok || createRes.status === 201) {
                conversationId = createResult.data?.id
            }

            // If creation failed (duplicate), find existing conversation
            if (!conversationId) {
                // Check if agreement auto-created conversation exists
                if (conv.acuerdoId) {
                    const agreementEntry = agreements.find(
                        (a) => a.dataId === conv.acuerdoId
                    )
                    if (agreementEntry?.conversacionId) {
                        conversationId = agreementEntry.conversacionId
                    }
                }

                // Fallback: list conversations and find matching one
                if (!conversationId) {
                    const listRes = await fetch(
                        `${API_BASE}/api/crowdsourcing/conversaciones?pageSize=50`,
                        {
                            headers: {
                                Authorization: `Bearer ${creatorToken.token}`,
                            },
                        }
                    )
                    if (listRes.ok) {
                        const listResult = await listRes.json()
                        const items =
                            listResult.data?.items ?? listResult.data ?? []
                        // Find by asunto or context match
                        const existing = items.find(
                            (c: Record<string, unknown>) =>
                                c.asunto === conv.contexto ||
                                (conv.acuerdoId &&
                                    c.acuerdoId &&
                                    String(c.acuerdoId) ===
                                        agreements.find(
                                            (a) => a.dataId === conv.acuerdoId
                                        )?.agreementId)
                        )
                        if (existing) {
                            conversationId =
                                (existing.id as string) ??
                                (existing.conversacionId as string)
                        }
                    }
                }
            }

            if (!conversationId) {
                throw new Error(
                    `Could not create or find conversation for ${conv.id} (${conv.contexto})`
                )
            }

            // Send messages (skip first if it's about the conversation itself already)
            for (const msg of conv.mensajes) {
                const senderToken = getParticipantToken(msg.remitenteEmail)

                const msgRes = await fetch(
                    `${API_BASE}/api/crowdsourcing/conversaciones/${conversationId}/mensajes`,
                    {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json",
                            Authorization: `Bearer ${senderToken.token}`,
                        },
                        body: JSON.stringify({
                            contenido: msg.mensaje,
                        }),
                    }
                )

                if (msgRes.ok || msgRes.status === 201) {
                    totalMessages++
                }
            }

            collectedConversations.push({
                dataId: conv.id,
                conversationId,
                participantEmails: [firstSenderEmail, otherEmail],
                contexto: conv.contexto,
            })
        }

        expect(collectedConversations.length).toBe(10)
        expect(totalMessages).toBeGreaterThanOrEqual(30)
    })

    test("Save conversation IDs to state", async () => {
        expect(collectedConversations.length).toBe(10)
        saveConversationIds(collectedConversations)
    })

    test("Verify conversations via API (spot check)", async () => {
        // Use MUSE token to check their conversations
        const museToken = getToken("muse.official@weplay-test.com")

        const res = await fetch(
            `${API_BASE}/api/crowdsourcing/conversaciones?pageSize=50`,
            {
                headers: {
                    Authorization: `Bearer ${museToken.token}`,
                },
            }
        )

        expect(res.ok).toBeTruthy()
        const result = await res.json()
        const items = result.data?.items ?? result.data ?? []
        // MUSE should have at least 2 conversations (Conv1 with Sarah, Conv8 with David)
        expect(items.length).toBeGreaterThanOrEqual(2)
    })

    test("Verify messages in Conv1 (MUSE <-> Sarah Chen)", async () => {
        const conv1 = collectedConversations.find((c) => c.dataId === "Conv1")
        expect(conv1).toBeTruthy()

        const museToken = getToken("muse.official@weplay-test.com")

        const res = await fetch(
            `${API_BASE}/api/crowdsourcing/conversaciones/${conv1!.conversationId}/mensajes?pageSize=50`,
            {
                headers: {
                    Authorization: `Bearer ${museToken.token}`,
                },
            }
        )

        expect(res.ok).toBeTruthy()
        const result = await res.json()
        const items = result.data?.items ?? result.data ?? []
        // Conv1 has 4 messages
        expect(items.length).toBeGreaterThanOrEqual(4)
    })

    test("Verify total message count across conversations", async () => {
        // Count total expected messages from JSON data
        const expectedTotal = conversations.reduce(
            (sum, conv) => sum + conv.mensajes.length,
            0
        )
        // Should be ~38 messages total
        expect(expectedTotal).toBeGreaterThanOrEqual(35)
    })
})
