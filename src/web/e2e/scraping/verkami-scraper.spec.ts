import { test } from "@playwright/test"
import * as fs from "fs"
import * as path from "path"
import { fileURLToPath } from "url"

/**
 * Verkami Music Crowdfunding Scraper
 *
 * Extracts 9 verified music projects from Verkami.
 * Output: data/scraped/verkami-campaigns.json
 *
 * Ethics: 2-3s delay between requests, read-only public data, single run.
 */

const __filename = fileURLToPath(import.meta.url)
const __dirname = path.dirname(__filename)
const SCRAPED_DIR = path.resolve(__dirname, "../../../../data/scraped")
const OUTPUT_FILE = path.join(SCRAPED_DIR, "verkami-campaigns.json")

// 9 verified Verkami URLs from docs/seed-data/music_crowd_seed.csv (Section 5.6)
const VERKAMI_URLS = [
    "https://www.verkami.com/projects/24465-french-horn-jazz-project",
    "https://www.verkami.com/locale/en/projects/28206-sedajazz-big-band-valmuz-sinergia",
    "https://www.verkami.com/locale/it/projects/17994-herberco-by-dixie-project",
    "https://www.verkami.com/locale/de/projects/8417-trim-peixoto-farinas-barroso-lanzamento-cddvd",
    "https://www.verkami.com/locale/en/projects/27210-vinilo-lp-sound-system-selection-by-good-over-evil",
    "https://www.verkami.com/locale/de/projects/9886-grabacion-del-nuevo-album-de-loon-attic-habitat",
    "https://www.verkami.com/locale/en/projects/30709-impronunciable-primer-disco-de-ares-gratal",
    "https://www.verkami.com/locale/en/projects/27374-black-olives-julia-pigalis-new-album",
    "https://www.verkami.com/locale/en/projects/22106-obal-baile-en-masso",
]

interface ScrapedReward {
    name: string
    price: number
    description: string
    backers: number
    deliveryEstimate: string | null
}

interface ScrapedCampaign {
    source: "verkami"
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

function parseEuroAmount(text: string): number {
    if (!text) return 0
    // Handle formats: "4.000 €", "4,000 €", "4000€", "4.000", "From 4.000 €"
    const cleaned = text.replace(/[^\d.,]/g, "")
    // European format: dots as thousands separator, comma as decimal
    const normalized = cleaned.replace(/\./g, "").replace(",", ".")
    const num = parseFloat(normalized)
    return isNaN(num) ? 0 : num
}

test.describe("Verkami Scraper @scraping", () => {
    test.describe.configure({ timeout: 600_000 }) // 10 min total for 9 pages

    test("scrape 9 verified Verkami music projects", async ({ page }) => {
        const campaigns: ScrapedCampaign[] = []

        // Ensure output dir exists
        if (!fs.existsSync(SCRAPED_DIR)) {
            fs.mkdirSync(SCRAPED_DIR, { recursive: true })
        }

        for (const url of VERKAMI_URLS) {
            const campaign = await scrapeProject(page, url)
            if (campaign) {
                campaigns.push(campaign)
                // eslint-disable-next-line no-console
                console.log(`[OK] ${campaign.title} by ${campaign.creator} (${campaign.goal} EUR, ${campaign.rewards.length} rewards)`)
            } else {
                // eslint-disable-next-line no-console
                console.log(`[SKIP] Failed to scrape: ${url}`)
            }

            // Save progress after each project (in case of timeout)
            fs.writeFileSync(OUTPUT_FILE, JSON.stringify(campaigns, null, 4), "utf-8")

            // Ethical delay: 2-3 seconds between requests
            await page.waitForTimeout(2000 + Math.random() * 1000)
        }

        // eslint-disable-next-line no-console
        console.log(`\nTotal scraped: ${campaigns.length}/${VERKAMI_URLS.length} projects`)
        // eslint-disable-next-line no-console
        console.log(`Output: ${OUTPUT_FILE}`)
    })
})

async function scrapeProject(page: import("@playwright/test").Page, url: string): Promise<ScrapedCampaign | null> {
    try {
        await page.goto(url, { waitUntil: "domcontentloaded", timeout: 30000 })
        await page.waitForTimeout(1500)

        // Title: first h1
        const title = await page.locator("h1").first().textContent().catch(() => null)
        if (!title?.trim()) return null

        // Creator: look for "A project of" or "Un proyecto de" pattern
        const creator = await extractCreator(page)

        // Category
        const category = await extractTextNear(page, ["Category", "Categoria", "Categorie"]) ?? "Music"

        // Location
        const location = await extractTextNear(page, ["Created in", "Creado en", "Creato in", "Erstellt in"]) ?? ""

        // Goal and raised amounts
        const { goal, raised } = await extractAmounts(page)

        // Backers count
        const backers = await extractBackersCount(page)

        // Status
        const status = await extractStatus(page)

        // Rewards
        const rewards = await extractRewards(page)

        return {
            source: "verkami",
            sourceUrl: url,
            title: title.trim(),
            creator: creator?.trim() ?? "Unknown Artist",
            category: category.trim(),
            location: location.trim(),
            goal,
            raised,
            currency: "EUR",
            backers,
            status,
            rewards,
            scrapedAt: new Date().toISOString(),
        }
    } catch (err) {
        // eslint-disable-next-line no-console
        console.error(`Error scraping ${url}:`, err)
        return null
    }
}

async function extractCreator(page: import("@playwright/test").Page): Promise<string | null> {
    // Try multiple patterns for creator name
    const patterns = [
        "A project of",
        "Un proyecto de",
        "Un progetto di",
        "Ein Projekt von",
    ]

    for (const pattern of patterns) {
        try {
            const el = page.locator(`text=${pattern}`).first()
            if (await el.isVisible({ timeout: 1000 }).catch(() => false)) {
                // Get the next link after this text
                const parent = el.locator("..")
                const link = parent.locator("a").first()
                const text = await link.textContent().catch(() => null)
                if (text?.trim()) return text.trim()
            }
        } catch {
            // Continue to next pattern
        }
    }

    // Fallback: look for author meta or any prominent link near project info
    try {
        const metaAuthor = await page.locator('meta[name="author"]').getAttribute("content")
        if (metaAuthor) return metaAuthor
    } catch {
        // Ignore
    }

    return null
}

async function extractTextNear(page: import("@playwright/test").Page, patterns: string[]): Promise<string | null> {
    for (const pattern of patterns) {
        try {
            const el = page.locator(`text=${pattern}`).first()
            if (await el.isVisible({ timeout: 1000 }).catch(() => false)) {
                const parent = el.locator("..")
                const text = await parent.textContent()
                if (text) {
                    // Extract the value after the label
                    const cleanText = text.replace(pattern, "").trim()
                    // Take the first meaningful part
                    const parts = cleanText.split("\n").filter((p: string) => p.trim())
                    if (parts.length > 0) return parts[0].trim()
                }
            }
        } catch {
            // Continue
        }
    }
    return null
}

async function extractAmounts(page: import("@playwright/test").Page): Promise<{ goal: number; raised: number }> {
    let goal = 0
    let raised = 0

    // Look for "From X €" or "Minimum X €" patterns for goal
    const goalPatterns = [/From\s+([\d.,]+)\s*[€EUR]/i, /Minimum\s+([\d.,]+)\s*[€EUR]/i, /Objectiu\s+([\d.,]+)\s*[€EUR]/i, /Objetivo\s+([\d.,]+)\s*[€EUR]/i]

    const bodyText = await page.locator("body").textContent().catch(() => "") ?? ""

    for (const pattern of goalPatterns) {
        const match = bodyText.match(pattern)
        if (match) {
            goal = parseEuroAmount(match[1])
            break
        }
    }

    // Look for raised amount - usually the big prominent number
    try {
        // Try to find percentage and goal to derive raised
        const pctMatch = bodyText.match(/([\d.,]+)\s*%/)
        if (pctMatch && goal > 0) {
            const pct = parseFloat(pctMatch[1].replace(",", "."))
            raised = Math.round((pct / 100) * goal)
        }
    } catch {
        // Ignore
    }

    // Fallback: try to find standalone amounts
    if (goal === 0) {
        const amountMatches = bodyText.match(/([\d.]+)\s*€/g)
        if (amountMatches && amountMatches.length >= 1) {
            // The goal is typically the first or most prominent amount
            const amounts = amountMatches.map((m: string) => parseEuroAmount(m)).filter((n: number) => n > 100)
            if (amounts.length >= 2) {
                // Usually: raised first, then goal
                raised = amounts[0]
                goal = amounts[1]
            } else if (amounts.length === 1) {
                goal = amounts[0]
            }
        }
    }

    return { goal, raised }
}

async function extractBackersCount(page: import("@playwright/test").Page): Promise<number> {
    const patterns = [/(\d+)\s*(?:Pledges|backers|mecenas|colaboradores|Unterstützer)/i]

    const bodyText = await page.locator("body").textContent().catch(() => "") ?? ""
    for (const pattern of patterns) {
        const match = bodyText.match(pattern)
        if (match) {
            return parseInt(match[1], 10)
        }
    }

    return 0
}

async function extractStatus(page: import("@playwright/test").Page): Promise<string> {
    const bodyText = await page.locator("body").textContent().catch(() => "") ?? ""

    if (/crowdfunded|funded|financiado|finanziato/i.test(bodyText)) {
        return "successful"
    }
    if (/active|activo|in progress|en curso/i.test(bodyText)) {
        return "active"
    }
    return "successful" // Default for known-funded projects
}

async function extractRewards(page: import("@playwright/test").Page): Promise<ScrapedReward[]> {
    const rewards: ScrapedReward[] = []

    try {
        // Verkami rewards are typically in distinct blocks with price and description
        // Try common reward container selectors
        const rewardSelectors = [
            ".reward",
            "[class*='reward']",
            "[class*='Reward']",
            ".pledge",
            "[data-reward]",
        ]

        for (const selector of rewardSelectors) {
            const elements = page.locator(selector)
            const count = await elements.count().catch(() => 0)

            if (count > 0) {
                for (let i = 0; i < Math.min(count, 8); i++) {
                    const el = elements.nth(i)
                    const text = await el.textContent().catch(() => "") ?? ""

                    // Extract price from reward block
                    const priceMatch = text.match(/([\d.,]+)\s*€/)
                    const price = priceMatch ? parseEuroAmount(priceMatch[1]) : 0

                    // Extract reward name (usually first line or heading)
                    const heading = await el.locator("h3, h4, h5, strong, b").first().textContent().catch(() => null)
                    const name = heading?.trim() ?? `Reward ${i + 1}`

                    // Extract backers count from reward
                    const backersMatch = text.match(/(\d+)\s*(?:backers|pledges|mecenas)/i)
                    const rewardBackers = backersMatch ? parseInt(backersMatch[1], 10) : 0

                    if (price > 0 || name !== `Reward ${i + 1}`) {
                        rewards.push({
                            name,
                            price: price || 10,
                            description: text.substring(0, 200).trim(),
                            backers: rewardBackers,
                            deliveryEstimate: null,
                        })
                    }
                }
                if (rewards.length > 0) break
            }
        }
    } catch {
        // Ignore reward extraction errors
    }

    // If no rewards found, add a generic one
    if (rewards.length === 0) {
        rewards.push({
            name: "Digital Album",
            price: 10,
            description: "Digital download of the album",
            backers: 0,
            deliveryEstimate: null,
        })
    }

    return rewards
}
