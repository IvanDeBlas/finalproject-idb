import * as fs from "fs"
import * as path from "path"
import { fileURLToPath } from "url"

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)
const DATA_DIR = path.resolve(__dirname, "../../../../data")

function loadJson<T>(relativePath: string): T {
    const filePath = path.join(DATA_DIR, relativePath)
    if (!fs.existsSync(filePath)) {
        throw new Error(`Data file not found: ${filePath}`)
    }
    return JSON.parse(fs.readFileSync(filePath, "utf-8")) as T
}

// === Type definitions for seed data ===

export interface ArtistReward {
    nombre: string
    importeMinimo: number
    descripcion: string
    tipoRewardId?: number
    incluyeEnvioFisico?: boolean
    cantidadMaxima?: number
}

export interface ArtistCampaign {
    titulo: string
    subtitulo: string
    descripcionCorta: string
    importeObjetivo: number
    tipoFinanciacionId?: number
    fechaFinDias?: number
    imagenPrincipalUrl?: string
    rewards: ArtistReward[]
}

export interface ArtistProfile {
    nombreArtistico: string
    generoMusical?: string
    descripcion: string
    imagenUrl?: string
    pais?: string
    ciudad?: string
}

export interface ArtistData {
    email: string
    password: string
    nombreCompleto?: string
    source?: string
    profile: ArtistProfile
    // all-campaigns.json uses campaigns[] with embedded rewards
    campaigns?: ArtistCampaign[]
    // fictional JSONs use campaign (singular) + separate rewards[]
    campaign?: Omit<ArtistCampaign, "rewards">
    rewards?: ArtistReward[]
}

/**
 * Normalize artist data to get campaigns with embedded rewards.
 * Handles both all-campaigns.json (campaigns[]) and fictional (campaign + rewards[]).
 */
export function getArtistCampaigns(artist: ArtistData): ArtistCampaign[] {
    if (artist.campaigns && artist.campaigns.length > 0) {
        return artist.campaigns
    }
    if (artist.campaign) {
        return [{
            ...artist.campaign,
            rewards: artist.rewards ?? [],
        }]
    }
    return []
}

export interface FanBacking {
    campaignTitle: string
    rewardName?: string
    monto: number
    mensaje?: string
}

export interface FanData {
    email: string
    password: string
    name: string
    backings: FanBacking[]
}

export interface ProfessionalData {
    email: string
    password: string
    nombre: string
    rolPrincipalId?: number
    especializacion?: string
    ubicacion?: string
}

export interface CrowdsourcingNeedJsonData {
    id: string
    artistaEmail: string
    artistaNombre: string
    titulo: string
    descripcion: string
    rolRequeridoId: number
    rolRequeridoNombre: string
    modalidad: string
    presupuestoMin: number
    presupuestoMax: number
    moneda: string
    fechaLimiteDias: number
    estadoFinalId: number
    estadoFinalNombre: string
}

export interface CrowdsourcingProposalJsonData {
    id: string
    necesidadId: string
    necesidadTitulo: string
    profesionalId: string
    profesionalNombre: string
    profesionalEmail: string
    precioPropuesto: number
    moneda: string
    duracion: string
    estadoFinalId: number
    estadoFinalNombre: string
    mensaje: string
}

/** @deprecated Use CrowdsourcingNeedJsonData for actual JSON loading */
export interface NeedData {
    artista: string
    titulo: string
    rolRequeridoId?: number
    modalidad?: string
    presupuestoMin?: number
    presupuestoMax?: number
    fechaLimite?: string
    descripcion: string
    estadoFinal?: string
}

/** @deprecated Use CrowdsourcingProposalJsonData for actual JSON loading */
export interface ProposalData {
    necesidadIndex: number
    profesionalIndex: number
    precioPropuesto: number
    duracion?: string
    estadoFinal?: string
    mensajeExtracto?: string
}

export interface BackingPlanData {
    fanEmail: string
    campaignTitle: string
    rewardName?: string
    monto: number
    mensaje?: string
    esAnonimo?: boolean
}

/** @deprecated Use CrowdsourcingAgreementJsonData for actual JSON loading */
export interface AgreementData {
    propuestaIndex: number
    estado: string
    milestones?: MilestoneData[]
}

/** @deprecated Use CrowdsourcingMilestoneJsonData for actual JSON loading */
export interface MilestoneData {
    titulo: string
    estado: string
    entregables?: DeliverableData[]
}

/** @deprecated Use CrowdsourcingDeliverableJsonData for actual JSON loading */
export interface DeliverableData {
    titulo: string
    estado: string
}

// === Crowdsourcing Agreement JSON data shapes ===

export interface CrowdsourcingAgreementJsonData {
    id: string
    necesidadId: string
    necesidadTitulo: string
    artistaEmail: string
    artistaNombre: string
    profesionalId: string
    profesionalNombre: string
    profesionalEmail: string
    montoAcordado: number
    moneda: string
    fechaInicioDiasRelativo: number
    fechaFinDiasRelativo: number
    estadoId: number
    estadoNombre: string
    motivoCancelacion?: string
}

export interface CrowdsourcingMilestoneJsonData {
    id: string
    acuerdoId: string
    titulo: string
    monto: number
    moneda: string
    deadlineDiasRelativo: number
    estado: string
}

export interface CrowdsourcingDeliverableJsonData {
    id: string
    milestoneId: string
    acuerdoId: string
    titulo: string
    estadoId: number
    estadoNombre: string
}

export interface CrowdsourcingConversationJsonData {
    id: string
    acuerdoId?: string
    necesidadId?: string
    participantes: string[]
    contexto: string
    mensajes: CrowdsourcingMessageJsonData[]
}

export interface CrowdsourcingMessageJsonData {
    remitente: string
    remitenteEmail: string
    mensaje: string
    timestampDiasRelativo: number
}

export interface CrowdsourcingRatingJsonData {
    id: string
    acuerdoId: string
    valoradorNombre: string
    valoradorEmail: string
    valoradoNombre: string
    valoradoEmail: string
    puntuacion: number
    comentario: string
}

export interface CrowdsourcingAgreementsFile {
    agreements: CrowdsourcingAgreementJsonData[]
    milestones: CrowdsourcingMilestoneJsonData[]
    deliverables: CrowdsourcingDeliverableJsonData[]
    conversations: CrowdsourcingConversationJsonData[]
    ratings: CrowdsourcingRatingJsonData[]
    summary: Record<string, number>
}

// === Unified seed data ===

export interface AllSeedData {
    artists: ArtistData[]
    fans: FanData[]
    backingPlans?: BackingPlanData[]
    professionals?: ProfessionalData[]
    needs?: NeedData[]
    proposals?: ProposalData[]
    agreements?: AgreementData[]
}

// === Data loaders ===

export function loadFamousArtists(): ArtistData[] {
    return loadJson<ArtistData[]>("fictional/famous-artists.json")
}

export function loadIndieArtists(): ArtistData[] {
    return loadJson<ArtistData[]>("fictional/indie-artists.json")
}

export function loadFans(): FanData[] {
    return loadJson<FanData[]>("fictional/fans.json")
}

export function loadProfessionals(): ProfessionalData[] {
    return loadJson<ProfessionalData[]>("fictional/crowdsourcing-professionals.json")
}

export function loadNeeds(): CrowdsourcingNeedJsonData[] {
    return loadJson<CrowdsourcingNeedJsonData[]>("fictional/crowdsourcing-needs.json")
}

export function loadProposals(): CrowdsourcingProposalJsonData[] {
    return loadJson<CrowdsourcingProposalJsonData[]>("fictional/crowdsourcing-proposals.json")
}

/** @deprecated Use loadAgreementsFile for typed JSON loading */
export function loadAgreements(): AgreementData[] {
    return loadJson<AgreementData[]>("fictional/crowdsourcing-agreements.json")
}

export function loadAgreementsFile(): CrowdsourcingAgreementsFile {
    return loadJson<CrowdsourcingAgreementsFile>("fictional/crowdsourcing-agreements.json")
}

/**
 * Load unified seed data from all-campaigns.json
 * (generated after scraping + merge step)
 */
export function loadAllSeedData(): AllSeedData {
    const raw = loadJson<Record<string, unknown>>("seed/all-campaigns.json")
    const result: AllSeedData = {
        artists: (raw.artists ?? []) as ArtistData[],
        fans: (raw.fans ?? []) as FanData[],
        backingPlans: (raw.backings ?? []) as BackingPlanData[],
    }
    return result
}

/**
 * Load backing plans from all-campaigns.json or fans.json fallback.
 */
export function loadBackingPlans(): BackingPlanData[] {
    try {
        const raw = loadJson<Record<string, unknown>>("seed/all-campaigns.json")
        return (raw.backings ?? []) as BackingPlanData[]
    } catch {
        // Fallback: extract from fans.json
        const fansFile = loadJson<Record<string, unknown>>("fictional/fans.json")
        const fans = (fansFile.fans ?? fansFile) as FanData[]
        const plans: BackingPlanData[] = []
        for (const fan of fans) {
            for (const b of fan.backings ?? []) {
                plans.push({
                    fanEmail: fan.email,
                    campaignTitle: b.campaignTitle,
                    rewardName: b.rewardName,
                    monto: b.monto,
                    mensaje: b.mensaje,
                    esAnonimo: false,
                })
            }
        }
        return plans
    }
}

/**
 * Try to load all-campaigns.json, fallback to fictional data only.
 */
export function loadSeedDataWithFallback(): AllSeedData {
    try {
        return loadAllSeedData()
    } catch {
        const artists = [
            ...loadFamousArtists(),
            ...loadIndieArtists(),
        ]
        const fansFile = loadJson<Record<string, unknown>>("fictional/fans.json")
        const fans = ((fansFile.fans ?? fansFile) as FanData[])
        const backingPlans: BackingPlanData[] = []
        for (const fan of fans) {
            for (const b of fan.backings ?? []) {
                backingPlans.push({
                    fanEmail: fan.email,
                    campaignTitle: b.campaignTitle,
                    rewardName: b.rewardName,
                    monto: b.monto,
                    mensaje: b.mensaje,
                    esAnonimo: false,
                })
            }
        }
        return { artists, fans, backingPlans }
    }
}
