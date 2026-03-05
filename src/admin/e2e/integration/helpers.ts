import { type Page, expect } from "@playwright/test"

// ---------------------------------------------------------------------------
// URLs
// ---------------------------------------------------------------------------

export const ADMIN_BASE = "http://localhost:3001"
export const LANDING_BASE = "http://localhost:3000"
export const API_BASE = "http://localhost:5001"

// ---------------------------------------------------------------------------
// Default test user (seeded in Docker DB)
// ---------------------------------------------------------------------------

export const TEST_USER = {
    email: "usuario1@mail.com",
    password: "123456",
}

// ---------------------------------------------------------------------------
// Auth helpers
// ---------------------------------------------------------------------------

/**
 * Login via the API directly. Returns JWT token for programmatic API calls.
 */
export async function loginViaApi(
    email: string,
    password: string
): Promise<{ token: string; userId: string }> {
    const res = await fetch(`${API_BASE}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
    })

    const data = await res.json()
    if (!data.data?.token) {
        throw new Error(`Login failed for ${email}: ${JSON.stringify(data)}`)
    }
    return { token: data.data.token, userId: data.data.userId }
}

/**
 * Login to the Admin app through the UI form.
 *
 * Strategy:
 *  1. Pre-seed localStorage with a valid JWT so authService.login()'s
 *     getCurrentUser() call succeeds (it fires before the store saves the token).
 *  2. Submit the login form normally.
 *  3. Register addInitScript so subsequent page.goto calls also have the auth
 *     data in localStorage (needed because the dashboard layout's hydration guard
 *     reads from the zustand persist store which rehydrates from localStorage).
 */
export async function loginToAdminUI(
    page: Page,
    email: string = TEST_USER.email,
    password: string = TEST_USER.password
): Promise<void> {
    // 1. Get a valid token via API (Node.js context)
    const { token, userId } = await loginViaApi(email, password)

    // 2. Fetch user profile for the zustand store shape
    const meRes = await fetch(`${API_BASE}/api/auth/me`, {
        headers: { Authorization: `Bearer ${token}` },
    })
    const meData = await meRes.json()
    const user = meData.data ?? { id: userId, email, roles: ["Fan"] }

    // 3. Register initScript so EVERY future navigation in this page has auth
    await page.addInitScript(
        ({ token, user }: { token: string; user: Record<string, unknown> }) => {
            localStorage.setItem("token", token)
            localStorage.setItem(
                "auth-storage",
                JSON.stringify({
                    state: { user, token, isAuthenticated: true },
                    version: 0,
                })
            )
        },
        { token, user }
    )

    // 4. Navigate to login page (initScript pre-seeds localStorage)
    await page.goto(`${ADMIN_BASE}/login`)

    // 5. Fill and submit the login form
    const submitBtn = page.getByRole("button", { name: /Iniciar sesion/i })
    await submitBtn.waitFor({ state: "visible", timeout: 10000 })

    await page.getByRole("textbox", { name: "Email" }).fill(email)
    await page.getByRole("textbox", { name: "Contrasena" }).fill(password)
    await submitBtn.click()

    // 6. Wait for navigation away from login page
    await page.waitForURL((url) => !url.pathname.endsWith("/login"), {
        timeout: 15000,
    })
}

// ---------------------------------------------------------------------------
// Campaign wizard helpers
// ---------------------------------------------------------------------------

/**
 * Fill Step 1 (Informacion Basica) of the campaign wizard.
 */
export async function fillWizardStep1(
    page: Page,
    data: { titulo: string; importeObjetivo: number; fechaFin?: boolean }
): Promise<void> {
    await page.getByLabel(/Titulo de la campania/i).fill(data.titulo)

    // Select "Todo o Nada"
    await page.getByRole("combobox").click()
    await page.getByRole("option", { name: /Todo o Nada/i }).click()

    // Fill funding goal
    await page
        .getByRole("spinbutton", { name: /Meta de financiacion/i })
        .fill(String(data.importeObjetivo))

    // Optionally set end date (required for publishing)
    if (data.fechaFin) {
        await page.getByRole("button", { name: /Selecciona fecha/i }).click()
        await page
            .getByRole("button", { name: /Go to next month/i })
            .click()
        await page.getByRole("gridcell", { name: "15" }).click()
        await page.keyboard.press("Escape")
    }
}

/**
 * Click "Siguiente" and wait for step transition.
 * The wizard needs a double-click: first validates, second advances.
 */
export async function clickSiguiente(page: Page): Promise<void> {
    const btn = page.getByRole("button", { name: /Siguiente/i })
    await btn.click()
    await page.waitForTimeout(300)
    if (await btn.isVisible()) {
        await btn.click()
        await page.waitForTimeout(300)
    }
}

/**
 * Fill Step 2 (Historia) of the campaign wizard.
 */
export async function fillWizardStep2(
    page: Page,
    data: { subtitulo: string; descripcion: string }
): Promise<void> {
    await page.getByLabel(/Subtitulo/i).fill(data.subtitulo)
    await page.getByLabel(/Descripcion corta/i).fill(data.descripcion)
}

/**
 * Run through the entire wizard and create a campaign as borrador.
 * Returns the campaign ID from the redirect URL.
 */
export async function createCampaignViaWizard(
    page: Page,
    data: {
        titulo: string
        importeObjetivo: number
        subtitulo: string
        descripcion: string
        fechaFin?: boolean
    }
): Promise<string> {
    await page.goto(`${ADMIN_BASE}/campanias/nueva`)
    await page
        .getByRole("heading", { name: /Crear Nueva Campania/i })
        .waitFor({ state: "visible", timeout: 10000 })

    // Step 1
    await fillWizardStep1(page, {
        titulo: data.titulo,
        importeObjetivo: data.importeObjetivo,
        fechaFin: data.fechaFin,
    })
    await clickSiguiente(page)
    await page
        .getByRole("heading", { name: /^Historia$/i })
        .waitFor({ state: "visible", timeout: 5000 })

    // Step 2
    await fillWizardStep2(page, {
        subtitulo: data.subtitulo,
        descripcion: data.descripcion,
    })
    await clickSiguiente(page)
    await page
        .getByRole("heading", { name: /Recompensas para tus Backers/i })
        .waitFor({ state: "visible", timeout: 5000 })

    // Step 3 - Skip (MVP)
    await clickSiguiente(page)
    await page
        .getByRole("heading", { name: /Revision Final/i })
        .waitFor({ state: "visible", timeout: 5000 })

    // Step 4 - Submit
    await page.getByRole("button", { name: /Crear campania/i }).click()
    await page.waitForURL(/\/campanias\/[0-9a-f-]+$/, { timeout: 15000 })

    const url = page.url()
    const match = url.match(/\/campanias\/([0-9a-f-]+)$/)
    if (!match)
        throw new Error(`Could not extract campaign ID from URL: ${url}`)
    return match[1]
}

/**
 * Publish a campaign from its detail page. Handles the confirmation dialog.
 */
export async function publishCampaign(page: Page): Promise<void> {
    await page
        .getByRole("button", { name: /Publicar Ahora/i })
        .click()

    // Confirm in dialog
    await page
        .getByRole("button", { name: /Publicar sin recompensas/i })
        .click()

    // Wait for success toast
    await expect(
        page.getByText(/Campania publicada exitosamente/i)
    ).toBeVisible({ timeout: 10000 })
}
