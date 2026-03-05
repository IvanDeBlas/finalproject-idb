import { test, expect } from "@playwright/test"
import {
    ADMIN_BASE,
    LANDING_BASE,
    API_BASE,
    TEST_USER,
    loginToAdminUI,
    loginViaApi,
    createCampaignViaWizard,
    publishCampaign,
} from "./helpers"

/**
 * WPR-015: Full E2E Integration Tests
 *
 * Prerequisites: docker compose up -d (all 4 services running)
 *   - API:     http://localhost:5001
 *   - Admin:   http://localhost:3001
 *   - Landing: http://localhost:3000
 *   - SQL:     localhost:1433
 *
 * Uses the seeded test user: usuario1@mail.com / 123456
 *
 * Run: npm run test:e2e -- e2e/integration
 */

test.describe.configure({ mode: "serial" })

const campaignTitle = `E2E Campaign ${Date.now()}`
let campaignId: string
let apiToken: string

test.describe("WPR-015: Full E2E Flow", () => {
    // -----------------------------------------------------------------
    // 1. Services health
    // -----------------------------------------------------------------

    test("1.1 API Swagger loads", async ({ page }) => {
        const res = await page.goto(`${API_BASE}/swagger`)
        expect(res?.status()).toBe(200)
        await expect(page).toHaveTitle(/Swagger UI/)
    })

    test("1.2 Landing loads", async ({ page }) => {
        await page.goto(`${LANDING_BASE}`)
        await expect(page).toHaveTitle(/WePlay Rises/)
    })

    test("1.3 Admin loads", async ({ page }) => {
        await page.goto(`${ADMIN_BASE}`)
        await expect(page).toHaveTitle(/WePlay Rises/)
    })

    // -----------------------------------------------------------------
    // 2. Login
    // -----------------------------------------------------------------

    test("2.1 Login to Admin via UI", async ({ page }) => {
        await loginToAdminUI(page)

        // Should be on dashboard
        await expect(
            page.getByRole("heading", { name: /Dashboard/i })
        ).toBeVisible({ timeout: 10000 })

        // User email visible in header
        await expect(page.getByText(TEST_USER.email)).toBeVisible()
    })

    test("2.2 Get API token for programmatic calls", async () => {
        const result = await loginViaApi(
            TEST_USER.email,
            TEST_USER.password
        )
        apiToken = result.token
        expect(apiToken).toBeTruthy()
    })

    // -----------------------------------------------------------------
    // 3. Artist Profile
    // -----------------------------------------------------------------

    test("3.1 Artist profile page loads in Admin", async ({ page }) => {
        await loginToAdminUI(page)
        await page.goto(`${ADMIN_BASE}/perfil`)

        await expect(
            page.getByRole("heading", { name: /Editar Perfil/i })
        ).toBeVisible({ timeout: 10000 })

        // Name field should have a value (artist was created previously)
        const nameInput = page.getByLabel(/Nombre artistico/i)
        await expect(nameInput).not.toHaveValue("")
    })

    test("3.2 Update artist profile", async ({ page }) => {
        await loginToAdminUI(page)
        await page.goto(`${ADMIN_BASE}/perfil`)

        await expect(
            page.getByLabel(/Nombre artistico/i)
        ).toBeVisible({ timeout: 10000 })

        // Update bio
        await page
            .getByLabel(/Biografia/i)
            .fill("Bio actualizada por Playwright E2E test")

        await page
            .getByRole("button", { name: /Actualizar perfil/i })
            .click()

        await expect(
            page.getByText(/Perfil actualizado/i)
        ).toBeVisible({ timeout: 5000 })
    })

    // -----------------------------------------------------------------
    // 4. Campaign Creation
    // -----------------------------------------------------------------

    test("4.1 Wizard loads with 4 steps", async ({ page }) => {
        await loginToAdminUI(page)
        await page.goto(`${ADMIN_BASE}/campanias/nueva`)

        await expect(
            page.getByRole("heading", { name: /Crear Nueva Campania/i })
        ).toBeVisible({ timeout: 10000 })

        // Verify 4 steps in the progress bar
        const stepper = page.getByRole("progressbar")
        await expect(stepper.getByText("Informacion Basica")).toBeVisible()
        await expect(stepper.getByText("Historia")).toBeVisible()
        await expect(stepper.getByText("Recompensas")).toBeVisible()
        await expect(stepper.getByText("Revision")).toBeVisible()
    })

    test("4.2 Create campaign as borrador", async ({ page }) => {
        await loginToAdminUI(page)

        campaignId = await createCampaignViaWizard(page, {
            titulo: campaignTitle,
            importeObjetivo: 5000,
            subtitulo: "Subtitulo E2E test",
            descripcion:
                "Campania creada por Playwright E2E para validar el flujo completo.",
            fechaFin: true,
        })

        expect(campaignId).toMatch(
            /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/
        )
    })

    test("4.3 Campaign detail shows borrador state", async ({ page }) => {
        await loginToAdminUI(page)
        await page.goto(`${ADMIN_BASE}/campanias/${campaignId}`)

        await expect(
            page.getByRole("heading", { name: campaignTitle })
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByText(/Borrador/)).toBeVisible()
        await expect(page.getByText(/VISTA PREVIA/i)).toBeVisible()
        await expect(
            page.getByRole("button", { name: /Publicar Ahora/i })
        ).toBeVisible()
    })

    test("4.4 Campaign in Mis Campanias list", async ({ page }) => {
        await loginToAdminUI(page)
        await page.goto(`${ADMIN_BASE}/campanias`)

        await expect(page.getByText(campaignTitle)).toBeVisible({
            timeout: 10000,
        })
    })

    // -----------------------------------------------------------------
    // 5. Publish
    // -----------------------------------------------------------------

    test("5.1 Publish campaign", async ({ page }) => {
        await loginToAdminUI(page)
        await page.goto(`${ADMIN_BASE}/campanias/${campaignId}`)

        await expect(
            page.getByRole("heading", { name: campaignTitle })
        ).toBeVisible({ timeout: 10000 })

        await publishCampaign(page)
    })

    test("5.2 Published campaign shows Publicada in list", async ({
        page,
    }) => {
        await loginToAdminUI(page)
        await page.goto(`${ADMIN_BASE}/campanias`)

        await expect(page.getByText(campaignTitle)).toBeVisible({
            timeout: 10000,
        })
    })

    // -----------------------------------------------------------------
    // 6. Landing visibility
    // -----------------------------------------------------------------

    test("6.1 Campaign visible on Landing /campanias", async ({ page }) => {
        await page.goto(`${LANDING_BASE}/campanias`)

        await expect(page.getByText(campaignTitle)).toBeVisible({
            timeout: 10000,
        })
    })

    test("6.2 Campaign detail on Landing", async ({ page }) => {
        await page.goto(`${LANDING_BASE}/campanias/${campaignId}`)

        await expect(
            page.getByRole("heading", { name: campaignTitle })
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByText("Subtitulo E2E test")).toBeVisible()
        await expect(page.getByText(/Activa/)).toBeVisible()
        await expect(page.getByText(/5.?000/)).toBeVisible()

        // Historia tab is visible and selectable
        await expect(
            page.getByRole("tab", { name: /Historia/i })
        ).toBeVisible()

        await expect(
            page.getByRole("button", { name: /Apoyar/i })
        ).toBeEnabled()
    })

    test("6.3 Artist section on campaign detail", async ({ page }) => {
        await page.goto(`${LANDING_BASE}/campanias/${campaignId}`)

        await expect(
            page.getByRole("heading", { name: campaignTitle })
        ).toBeVisible({ timeout: 10000 })

        await expect(
            page.getByRole("heading", { name: /Sobre el Artista/i })
        ).toBeVisible()
        await expect(
            page.getByRole("link", { name: /Ver perfil/i })
        ).toBeVisible()
    })

    // -----------------------------------------------------------------
    // 7. Dashboard
    // -----------------------------------------------------------------

    test("7.1 Dashboard shows campaign", async ({ page }) => {
        await loginToAdminUI(page)
        await page.goto(`${ADMIN_BASE}/dashboard`)

        await expect(
            page.getByRole("heading", { name: /Dashboard/i })
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByText(campaignTitle)).toBeVisible({
            timeout: 10000,
        })
    })

    // -----------------------------------------------------------------
    // Cleanup
    // -----------------------------------------------------------------

    test("99. Cleanup: delete test campaign", async () => {
        if (!campaignId || !apiToken) return

        const res = await fetch(
            `${API_BASE}/api/campanias/${campaignId}`,
            {
                method: "DELETE",
                headers: { Authorization: `Bearer ${apiToken}` },
            }
        )
        expect([200, 204, 404]).toContain(res.status)
    })
})
