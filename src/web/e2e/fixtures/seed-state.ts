import * as fs from "fs"
import * as path from "path"
import { fileURLToPath } from "url"

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)
const STATE_DIR = path.resolve(__dirname, "../.state")

function ensureStateDir(): void {
    if (!fs.existsSync(STATE_DIR)) {
        fs.mkdirSync(STATE_DIR, { recursive: true })
    }
}

function statePath(filename: string): string {
    return path.join(STATE_DIR, filename)
}

// === Token State ===

export interface TokenEntry {
    email: string
    token: string
    userId: string
}

export function saveTokens(tokens: TokenEntry[]): void {
    ensureStateDir()
    fs.writeFileSync(statePath("tokens.json"), JSON.stringify(tokens, null, 2))
}

export function loadTokens(): TokenEntry[] {
    const filePath = statePath("tokens.json")
    if (!fs.existsSync(filePath)) {
        throw new Error("tokens.json not found. Run 01-register-artists.spec.ts first.")
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as TokenEntry[]
}

export function getToken(email: string): TokenEntry {
    const tokens = loadTokens()
    const entry = tokens.find((t) => t.email === email)
    if (!entry) {
        throw new Error(`Token not found for ${email}`)
    }
    return entry
}

// === Artista ID State ===

export interface ArtistaEntry {
    email: string
    artistaId: string
}

export function saveArtistaIds(entries: ArtistaEntry[]): void {
    ensureStateDir()
    fs.writeFileSync(statePath("artista-ids.json"), JSON.stringify(entries, null, 2))
}

export function loadArtistaIds(): ArtistaEntry[] {
    const filePath = statePath("artista-ids.json")
    if (!fs.existsSync(filePath)) {
        throw new Error("artista-ids.json not found. Run 02-create-profiles.spec.ts first.")
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as ArtistaEntry[]
}

export function getArtistaId(email: string): string {
    const entries = loadArtistaIds()
    const entry = entries.find((e) => e.email === email)
    if (!entry) {
        throw new Error(`Artista ID not found for ${email}`)
    }
    return entry.artistaId
}

// === Campaign ID State ===

export interface CampaignEntry {
    email: string
    campaignTitle: string
    campaignId: string
}

export function saveCampaignIds(entries: CampaignEntry[]): void {
    ensureStateDir()
    fs.writeFileSync(statePath("campaign-ids.json"), JSON.stringify(entries, null, 2))
}

export function loadCampaignIds(): CampaignEntry[] {
    const filePath = statePath("campaign-ids.json")
    if (!fs.existsSync(filePath)) {
        throw new Error("campaign-ids.json not found. Run 03-create-campaigns.spec.ts first.")
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as CampaignEntry[]
}

// === Reward ID State ===

export interface RewardEntry {
    campaignId: string
    rewardName: string
    rewardId: string
}

export function saveRewardIds(entries: RewardEntry[]): void {
    ensureStateDir()
    fs.writeFileSync(statePath("reward-ids.json"), JSON.stringify(entries, null, 2))
}

export function loadRewardIds(): RewardEntry[] {
    const filePath = statePath("reward-ids.json")
    if (!fs.existsSync(filePath)) {
        throw new Error("reward-ids.json not found. Run 04-add-rewards.spec.ts first.")
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as RewardEntry[]
}

export function getRewardId(campaignId: string, rewardName: string): string {
    const entries = loadRewardIds()
    const entry = entries.find(
        (e) => e.campaignId === campaignId && e.rewardName === rewardName
    )
    if (!entry) {
        throw new Error(`Reward ID not found for "${rewardName}" in campaign ${campaignId}`)
    }
    return entry.rewardId
}

// === Fan Token State ===

export function saveFanTokens(tokens: TokenEntry[]): void {
    ensureStateDir()
    fs.writeFileSync(statePath("fan-tokens.json"), JSON.stringify(tokens, null, 2))
}

export function loadFanTokens(): TokenEntry[] {
    const filePath = statePath("fan-tokens.json")
    if (!fs.existsSync(filePath)) {
        throw new Error("fan-tokens.json not found. Run 06-register-fans.spec.ts first.")
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as TokenEntry[]
}

export function getFanToken(email: string): TokenEntry {
    const tokens = loadFanTokens()
    const entry = tokens.find((t) => t.email === email)
    if (!entry) {
        throw new Error(`Fan token not found for ${email}`)
    }
    return entry
}

// === Backing ID State ===

export interface BackingEntry {
    fanEmail: string
    campaignId: string
    campaignTitle: string
    rewardId?: string
    rewardName?: string
    backingId: string
    monto: number
}

export function saveBackingIds(entries: BackingEntry[]): void {
    ensureStateDir()
    fs.writeFileSync(statePath("backing-ids.json"), JSON.stringify(entries, null, 2))
}

export function loadBackingIds(): BackingEntry[] {
    const filePath = statePath("backing-ids.json")
    if (!fs.existsSync(filePath)) {
        throw new Error("backing-ids.json not found. Run 07-create-backings.spec.ts first.")
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as BackingEntry[]
}

// === Professional Token State ===

export function saveProfessionalTokens(tokens: TokenEntry[]): void {
    ensureStateDir()
    fs.writeFileSync(statePath("professional-tokens.json"), JSON.stringify(tokens, null, 2))
}

export function loadProfessionalTokens(): TokenEntry[] {
    const filePath = statePath("professional-tokens.json")
    if (!fs.existsSync(filePath)) {
        throw new Error("professional-tokens.json not found. Run 09-register-professionals.spec.ts first.")
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as TokenEntry[]
}

export function getProfessionalToken(email: string): TokenEntry {
    const tokens = loadProfessionalTokens()
    const entry = tokens.find((t) => t.email === email)
    if (!entry) {
        throw new Error(`Professional token not found for ${email}`)
    }
    return entry
}

// === Need ID State ===

export interface NeedEntry {
    dataId: string
    artistaEmail: string
    titulo: string
    needId: string
}

export function saveNeedIds(entries: NeedEntry[]): void {
    ensureStateDir()
    fs.writeFileSync(statePath("need-ids.json"), JSON.stringify(entries, null, 2))
}

export function loadNeedIds(): NeedEntry[] {
    const filePath = statePath("need-ids.json")
    if (!fs.existsSync(filePath)) {
        throw new Error("need-ids.json not found. Run 10-create-needs.spec.ts first.")
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as NeedEntry[]
}

export function getNeedId(dataId: string): string {
    const entries = loadNeedIds()
    const entry = entries.find((e) => e.dataId === dataId)
    if (!entry) {
        throw new Error(`Need ID not found for data ID ${dataId}`)
    }
    return entry.needId
}

// === Proposal ID State ===

export interface ProposalEntry {
    dataId: string
    necesidadDataId: string
    profesionalEmail: string
    proposalId: string
    estadoFinalId: number
}

export function saveProposalIds(entries: ProposalEntry[]): void {
    ensureStateDir()
    fs.writeFileSync(statePath("proposal-ids.json"), JSON.stringify(entries, null, 2))
}

export function loadProposalIds(): ProposalEntry[] {
    const filePath = statePath("proposal-ids.json")
    if (!fs.existsSync(filePath)) {
        throw new Error("proposal-ids.json not found. Run 11-submit-proposals.spec.ts first.")
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as ProposalEntry[]
}

export function getProposalId(dataId: string): string {
    const entries = loadProposalIds()
    const entry = entries.find((e) => e.dataId === dataId)
    if (!entry) {
        throw new Error(`Proposal ID not found for data ID ${dataId}`)
    }
    return entry.proposalId
}

// === Agreement ID State ===

export interface AgreementEntry {
    dataId: string
    proposalDataId: string
    necesidadDataId: string
    artistaEmail: string
    profesionalEmail: string
    agreementId: string
    estadoId: number
    conversacionId?: string
}

export function saveAgreementIds(entries: AgreementEntry[]): void {
    ensureStateDir()
    fs.writeFileSync(statePath("agreement-ids.json"), JSON.stringify(entries, null, 2))
}

export function loadAgreementIds(): AgreementEntry[] {
    const filePath = statePath("agreement-ids.json")
    if (!fs.existsSync(filePath)) {
        throw new Error("agreement-ids.json not found. Run 12-manage-agreements.spec.ts first.")
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as AgreementEntry[]
}

export function getAgreementId(dataId: string): string {
    const entries = loadAgreementIds()
    const entry = entries.find((e) => e.dataId === dataId)
    if (!entry) {
        throw new Error(`Agreement ID not found for data ID ${dataId}`)
    }
    return entry.agreementId
}

export function getAgreementEntry(dataId: string): AgreementEntry {
    const entries = loadAgreementIds()
    const entry = entries.find((e) => e.dataId === dataId)
    if (!entry) {
        throw new Error(`Agreement entry not found for data ID ${dataId}`)
    }
    return entry
}

// === Milestone ID State ===

export interface MilestoneEntry {
    dataId: string
    acuerdoDataId: string
    agreementId: string
    milestoneId: string
    titulo: string
}

export function saveMilestoneIds(entries: MilestoneEntry[]): void {
    ensureStateDir()
    fs.writeFileSync(statePath("milestone-ids.json"), JSON.stringify(entries, null, 2))
}

export function loadMilestoneIds(): MilestoneEntry[] {
    const filePath = statePath("milestone-ids.json")
    if (!fs.existsSync(filePath)) {
        throw new Error("milestone-ids.json not found. Run 12-manage-agreements.spec.ts first.")
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as MilestoneEntry[]
}

export function getMilestoneId(dataId: string): string {
    const entries = loadMilestoneIds()
    const entry = entries.find((e) => e.dataId === dataId)
    if (!entry) {
        throw new Error(`Milestone ID not found for data ID ${dataId}`)
    }
    return entry.milestoneId
}

// === Deliverable ID State ===

export interface DeliverableEntry {
    dataId: string
    milestoneDataId: string
    milestoneId: string
    acuerdoDataId: string
    agreementId: string
    deliverableId: string
    titulo: string
    estadoId: number
}

export function saveDeliverableIds(entries: DeliverableEntry[]): void {
    ensureStateDir()
    fs.writeFileSync(statePath("deliverable-ids.json"), JSON.stringify(entries, null, 2))
}

export function loadDeliverableIds(): DeliverableEntry[] {
    const filePath = statePath("deliverable-ids.json")
    if (!fs.existsSync(filePath)) {
        throw new Error("deliverable-ids.json not found. Run 12-manage-agreements.spec.ts first.")
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as DeliverableEntry[]
}

// === Conversation ID State ===

export interface ConversationEntry {
    dataId: string
    conversationId: string
    participantEmails: string[]
    contexto: string
}

export function saveConversationIds(entries: ConversationEntry[]): void {
    ensureStateDir()
    fs.writeFileSync(statePath("conversation-ids.json"), JSON.stringify(entries, null, 2))
}

export function loadConversationIds(): ConversationEntry[] {
    const filePath = statePath("conversation-ids.json")
    if (!fs.existsSync(filePath)) {
        throw new Error("conversation-ids.json not found. Run 13-crowdsourcing-messages.spec.ts first.")
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as ConversationEntry[]
}
