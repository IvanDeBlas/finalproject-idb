import { test, expect } from "@playwright/test"
import * as fs from "fs"
import * as path from "path"
import { fileURLToPath } from "url"

/**
 * Merge Seed Data
 *
 * Combines scraped Verkami data + fictional data into a unified
 * data/seed/all-campaigns.json file for consumption by seeding specs.
 *
 * Fallback: If Verkami data is missing, generates from fictional data only (15+ campaigns).
 */

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)
const DATA_DIR = path.resolve(__dirname, "../../../../data")
const SCRAPED_FILE = path.join(DATA_DIR, "scraped/verkami-campaigns.json")
const FAMOUS_FILE = path.join(DATA_DIR, "fictional/famous-artists.json")
const INDIE_FILE = path.join(DATA_DIR, "fictional/indie-artists.json")
const FANS_FILE = path.join(DATA_DIR, "fictional/fans.json")
const SEED_DIR = path.join(DATA_DIR, "seed")
const OUTPUT_FILE = path.join(SEED_DIR, "all-campaigns.json")

interface ScrapedReward {
    name: string
    price: number
    description: string
    backers: number
    deliveryEstimate: string | null
}

interface ScrapedCampaign {
    source: string
    sourceUrl: string
    title: string
    creator: string
    category: string
    location: string
    goal: number
    raised: number
    currency: string
    backers: number
    status: string
    rewards: ScrapedReward[]
    scrapedAt: string
}

interface FictionalArtist {
    email: string
    password: string
    profile: {
        nombreArtistico: string
        generoMusical: string
        descripcion: string
        imagenUrl: string
    }
    campaign: {
        titulo: string
        subtitulo: string
        descripcionCorta: string
        importeObjetivo: number
        tipoFinanciacionId: number
        fechaFinDias: number
        imagenPrincipalUrl: string
    }
    rewards: Array<{
        nombre: string
        importeMinimo: number
        descripcion: string
        envioFisico: boolean
        entregaEstimada: string | null
        cantidadMaxima: number | null
    }>
}

interface FansFile {
    fans: Array<{
        email: string
        password: string
        name: string
        backings: Array<{
            campaignTitle: string
            rewardName?: string
            monto: number
            mensaje?: string
        }>
    }>
}

// Unified format for all-campaigns.json
interface UnifiedArtist {
    email: string
    password: string
    source: "fictional" | "verkami"
    profile: {
        nombreArtistico: string
        generoMusical: string
        descripcion: string
        imagenUrl: string
        pais?: string
        ciudad?: string
    }
    campaigns: Array<{
        titulo: string
        subtitulo: string
        descripcionCorta: string
        importeObjetivo: number
        tipoFinanciacionId: number
        fechaFinDias: number
        imagenPrincipalUrl: string
        rewards: Array<{
            nombre: string
            importeMinimo: number
            descripcion: string
            tipoRewardId?: number
            monedaId?: number
            incluyeEnvioFisico: boolean
            cantidadMaxima: number | null
            esAddOn: boolean
        }>
    }>
}

interface UnifiedFan {
    email: string
    password: string
    name: string
}

interface UnifiedBacking {
    fanEmail: string
    campaignTitle: string
    rewardName: string | null
    monto: number
    mensaje: string
    esAnonimo: boolean
}

interface AllCampaignsData {
    artists: UnifiedArtist[]
    fans: UnifiedFan[]
    backings: UnifiedBacking[]
    metadata: {
        generatedAt: string
        sources: string[]
        totalArtists: number
        totalCampaigns: number
        totalRewards: number
        totalFans: number
        totalBackings: number
    }
}

function transformFictionalArtist(artist: FictionalArtist, source: "fictional"): UnifiedArtist {
    return {
        email: artist.email,
        password: artist.password,
        source,
        profile: {
            nombreArtistico: artist.profile.nombreArtistico,
            generoMusical: artist.profile.generoMusical,
            descripcion: artist.profile.descripcion,
            imagenUrl: artist.profile.imagenUrl,
        },
        campaigns: [
            {
                titulo: artist.campaign.titulo,
                subtitulo: artist.campaign.subtitulo,
                descripcionCorta: artist.campaign.descripcionCorta,
                importeObjetivo: artist.campaign.importeObjetivo,
                tipoFinanciacionId: artist.campaign.tipoFinanciacionId,
                fechaFinDias: artist.campaign.fechaFinDias,
                imagenPrincipalUrl: artist.campaign.imagenPrincipalUrl,
                rewards: artist.rewards.map((r) => ({
                    nombre: r.nombre,
                    importeMinimo: r.importeMinimo,
                    descripcion: r.descripcion,
                    tipoRewardId: 1,
                    monedaId: 1,
                    incluyeEnvioFisico: r.envioFisico,
                    cantidadMaxima: r.cantidadMaxima,
                    esAddOn: false,
                })),
            },
        ],
    }
}

function transformVerkamiCampaign(campaign: ScrapedCampaign, index: number): UnifiedArtist {
    const emailSlug = campaign.creator
        .toLowerCase()
        .replace(/[^a-z0-9]/g, "")
        .substring(0, 20)
    const email = `verkami.${emailSlug || `artist${index}`}@weplay-test.com`

    // Map category to generoMusical
    const categoryMap: Record<string, string> = {
        Music: "Music",
        Jazz: "Jazz",
        "Reggae/Ska": "Reggae/Ska",
        Pop: "Pop",
        Songwriter: "Singer-Songwriter",
        Folk: "Folk",
        Clasica: "Classical",
    }
    const genero = categoryMap[campaign.category] ?? "Music"

    // Determine location
    const locationParts = campaign.location?.split(",").map((s: string) => s.trim()) ?? []
    const ciudad = locationParts[0] ?? ""
    const pais = locationParts[1] ?? "Spain"

    return {
        email,
        password: "WePlay2026!",
        source: "verkami",
        profile: {
            nombreArtistico: campaign.creator,
            generoMusical: genero,
            descripcion: `Artista de ${genero} con un proyecto exitoso en Verkami: "${campaign.title}". Proyecto financiado con ${campaign.backers} backers y ${campaign.raised} EUR recaudados.`,
            imagenUrl: `https://placehold.co/400x400/2d2d44/4fc3f7?text=${encodeURIComponent(campaign.creator.substring(0, 15))}`,
            pais,
            ciudad,
        },
        campaigns: [
            {
                titulo: campaign.title,
                subtitulo: `Proyecto musical de ${campaign.creator}`,
                descripcionCorta: `Proyecto de ${genero} originalmente financiado en Verkami con un objetivo de ${campaign.goal} EUR. ${campaign.backers} personas ya apoyaron este proyecto.`,
                importeObjetivo: campaign.goal || 5000,
                tipoFinanciacionId: 1,
                fechaFinDias: 45,
                imagenPrincipalUrl: `https://placehold.co/800x450/2d2d44/4fc3f7?text=${encodeURIComponent(campaign.title.substring(0, 25))}`,
                rewards: campaign.rewards.map((r) => ({
                    nombre: r.name,
                    importeMinimo: r.price || 10,
                    descripcion: r.description.substring(0, 300),
                    tipoRewardId: 1,
                    monedaId: 1,
                    incluyeEnvioFisico: false,
                    cantidadMaxima: null,
                    esAddOn: false,
                })),
            },
        ],
    }
}

test.describe("Merge Seed Data @scraping", () => {
    test("generate all-campaigns.json from fictional + scraped data", async () => {
        const sources: string[] = []
        const allArtists: UnifiedArtist[] = []

        // 1. Load fictional famous artists (10)
        const famous: FictionalArtist[] = JSON.parse(fs.readFileSync(FAMOUS_FILE, "utf-8"))
        for (const artist of famous) {
            allArtists.push(transformFictionalArtist(artist, "fictional"))
        }
        sources.push(`fictional/famous-artists.json (${famous.length} artists)`)
        // eslint-disable-next-line no-console
        console.log(`Loaded ${famous.length} famous artists`)

        // 2. Load fictional indie artists (5)
        const indie: FictionalArtist[] = JSON.parse(fs.readFileSync(INDIE_FILE, "utf-8"))
        for (const artist of indie) {
            allArtists.push(transformFictionalArtist(artist, "fictional"))
        }
        sources.push(`fictional/indie-artists.json (${indie.length} artists)`)
        // eslint-disable-next-line no-console
        console.log(`Loaded ${indie.length} indie artists`)

        // 3. Load scraped Verkami data (if available)
        let verkamiCount = 0
        if (fs.existsSync(SCRAPED_FILE)) {
            try {
                const scraped: ScrapedCampaign[] = JSON.parse(fs.readFileSync(SCRAPED_FILE, "utf-8"))
                if (scraped.length > 0) {
                    for (let i = 0; i < scraped.length; i++) {
                        allArtists.push(transformVerkamiCampaign(scraped[i], i))
                    }
                    verkamiCount = scraped.length
                    sources.push(`scraped/verkami-campaigns.json (${scraped.length} campaigns)`)
                    // eslint-disable-next-line no-console
                    console.log(`Loaded ${scraped.length} Verkami campaigns`)
                }
            } catch (err) {
                // eslint-disable-next-line no-console
                console.log(`Warning: Could not parse Verkami data, using fallback. Error: ${err}`)
            }
        } else {
            // eslint-disable-next-line no-console
            console.log("No Verkami scraped data found - using fictional data only (fallback)")
            sources.push("verkami: FALLBACK (no scraped data)")
        }

        // 4. Load fans and backings
        const fansFile: FansFile = JSON.parse(fs.readFileSync(FANS_FILE, "utf-8"))
        const fans: UnifiedFan[] = fansFile.fans.map((f) => ({
            email: f.email,
            password: f.password,
            name: f.name,
        }))
        const backings: UnifiedBacking[] = fansFile.fans.flatMap((f) =>
            f.backings.map((b) => ({
                fanEmail: f.email,
                campaignTitle: b.campaignTitle,
                rewardName: b.rewardName ?? null,
                monto: b.monto,
                mensaje: b.mensaje ?? "",
                esAnonimo: false,
            }))
        )
        sources.push(`fictional/fans.json (${fans.length} fans, ${backings.length} backings)`)

        // 5. Build output
        const totalRewards = allArtists.reduce(
            (sum, a) => sum + a.campaigns.reduce((s, c) => s + c.rewards.length, 0),
            0
        )

        const output: AllCampaignsData = {
            artists: allArtists,
            fans,
            backings,
            metadata: {
                generatedAt: new Date().toISOString(),
                sources,
                totalArtists: allArtists.length,
                totalCampaigns: allArtists.reduce((sum, a) => sum + a.campaigns.length, 0),
                totalRewards,
                totalFans: fans.length,
                totalBackings: backings.length,
            },
        }

        // 6. Write output
        if (!fs.existsSync(SEED_DIR)) {
            fs.mkdirSync(SEED_DIR, { recursive: true })
        }
        fs.writeFileSync(OUTPUT_FILE, JSON.stringify(output, null, 4), "utf-8")

        // 7. Summary
        // eslint-disable-next-line no-console
        console.log(`\n=== all-campaigns.json generated ===`)
        // eslint-disable-next-line no-console
        console.log(`Artists: ${output.metadata.totalArtists} (${famous.length} famous + ${indie.length} indie + ${verkamiCount} verkami)`)
        // eslint-disable-next-line no-console
        console.log(`Campaigns: ${output.metadata.totalCampaigns}`)
        // eslint-disable-next-line no-console
        console.log(`Rewards: ${output.metadata.totalRewards}`)
        // eslint-disable-next-line no-console
        console.log(`Fans: ${output.metadata.totalFans}`)
        // eslint-disable-next-line no-console
        console.log(`Backings: ${output.metadata.totalBackings}`)
        // eslint-disable-next-line no-console
        console.log(`Output: ${OUTPUT_FILE}`)

        // 8. Assertions
        expect(output.artists.length).toBeGreaterThanOrEqual(15) // At minimum: 10 famous + 5 indie
        expect(output.fans.length).toBe(5)
        expect(output.backings.length).toBe(15)
        expect(output.metadata.totalRewards).toBeGreaterThanOrEqual(55) // 40 famous + 15 indie

        // Verify structure of each artist
        for (const artist of output.artists) {
            expect(artist.email).toBeTruthy()
            expect(artist.password).toBeTruthy()
            expect(artist.profile.nombreArtistico).toBeTruthy()
            expect(artist.campaigns.length).toBeGreaterThanOrEqual(1)
            for (const campaign of artist.campaigns) {
                expect(campaign.titulo).toBeTruthy()
                expect(campaign.importeObjetivo).toBeGreaterThan(0)
                expect(campaign.rewards.length).toBeGreaterThanOrEqual(1)
            }
        }
    })
})
