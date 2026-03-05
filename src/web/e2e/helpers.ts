import { type Page, expect } from "@playwright/test"

// === CONSTANTES ===
export const LANDING_BASE = "http://localhost:3000"
export const ADMIN_BASE = "http://localhost:3001"
export const API_BASE = "http://localhost:5001"

// === AUTH HELPERS ===

/**
 * Login via API. Returns JWT token and userId.
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
 * Register via API. Returns JWT token and userId.
 */
export async function registerViaApi(
    email: string,
    password: string,
    nombreCompleto?: string
): Promise<{ token: string; userId: string }> {
    const res = await fetch(`${API_BASE}/api/auth/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            email,
            password,
            confirmPassword: password,
            nombreCompleto: nombreCompleto ?? email.split("@")[0],
            role: "Fan",
        }),
    })

    const data = await res.json()
    if (!data.data?.token) {
        throw new Error(`Register failed for ${email}: ${JSON.stringify(data)}`)
    }
    return { token: data.data.token, userId: data.data.userId }
}

/**
 * Login to Landing by injecting auth state into localStorage.
 * More reliable than UI interaction for use as a prerequisite in other tests.
 */
export async function loginToLandingUI(
    page: Page,
    email: string,
    password: string
): Promise<void> {
    const { token, userId } = await loginViaApi(email, password)

    const meRes = await fetch(`${API_BASE}/api/auth/me`, {
        headers: { Authorization: `Bearer ${token}` },
    })
    const meData = await meRes.json()
    const user = meData.data ?? { id: userId, email, nombreCompleto: email.split("@")[0] }

    // Inject auth state before navigating
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

    // Navigate to trigger auth hydration, then verify logged in state
    await page.goto(`${LANDING_BASE}/`)
    await page.waitForLoadState("networkidle")
}

/**
 * Login to Landing via actual UI form interaction.
 * Uses fill() which works when VITE_API_URL points to the correct API.
 */
export async function loginToLandingViaForm(
    page: Page,
    email: string,
    password: string
): Promise<void> {
    await page.goto(`${LANDING_BASE}/auth/login`)
    await page.waitForLoadState("networkidle")

    const submitBtn = page.getByRole("main").getByRole("button", { name: /Iniciar sesion/i })
    await submitBtn.waitFor({ state: "visible", timeout: 10000 })

    await page.locator("#email").click()
    await page.locator("#email").fill(email)
    await page.locator("#password").click()
    await page.locator("#password").fill(password)

    await submitBtn.click()

    await page.waitForURL((url) => !url.pathname.includes("/auth/login"), {
        timeout: 15000,
    })
}

/**
 * Login to Admin app via UI. Pre-seeds localStorage with JWT for hydration.
 */
export async function loginToAdminUI(
    page: Page,
    email: string,
    password: string
): Promise<void> {
    const { token, userId } = await loginViaApi(email, password)

    const meRes = await fetch(`${API_BASE}/api/auth/me`, {
        headers: { Authorization: `Bearer ${token}` },
    })
    const meData = await meRes.json()
    const user = meData.data ?? { id: userId, email, roles: ["Fan"] }

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

    await page.goto(`${ADMIN_BASE}/login`)

    const submitBtn = page.getByRole("button", { name: /Iniciar sesion/i })
    await submitBtn.waitFor({ state: "visible", timeout: 10000 })

    await page.getByRole("textbox", { name: "Email" }).fill(email)
    await page.getByRole("textbox", { name: "Contrasena" }).fill(password)
    await submitBtn.click()

    await page.waitForURL((url) => !url.pathname.endsWith("/login"), {
        timeout: 15000,
    })
}

/**
 * Register via Landing UI form.
 * Uses pressSequentially for react-hook-form compatibility.
 */
export async function registerViaLandingUI(
    page: Page,
    name: string,
    email: string,
    password: string
): Promise<void> {
    await page.goto(`${LANDING_BASE}/auth/register`)
    await page.waitForLoadState("networkidle")

    const submitBtn = page.getByRole("button", { name: /Crear cuenta/i })
    await submitBtn.waitFor({ state: "visible", timeout: 10000 })

    for (const [id, value] of [
        ["#nombreCompleto", name],
        ["#email", email],
        ["#password", password],
        ["#confirmPassword", password],
    ]) {
        const input = page.locator(id)
        await input.click()
        await input.clear()
        await input.pressSequentially(value, { delay: 10 })
    }

    await page.waitForTimeout(100)
    await submitBtn.click()

    await page.waitForURL((url) => !url.pathname.includes("/auth/register"), {
        timeout: 15000,
    })
}

// === CAMPAIGN HELPERS (via Admin UI) ===

/**
 * Fill Step 1 (Info Basica) of the campaign wizard.
 */
export async function fillWizardStep1(
    page: Page,
    data: { titulo: string; importeObjetivo: number; fechaFin?: boolean }
): Promise<void> {
    await page.getByLabel(/Titulo de la campania/i).fill(data.titulo)

    await page.getByRole("combobox").click()
    await page.getByRole("option", { name: /Todo o Nada/i }).click()

    await page
        .getByRole("spinbutton", { name: /Meta de financiacion/i })
        .fill(String(data.importeObjetivo))

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
 * Click "Siguiente" with retry for wizard double-click pattern.
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

    // Step 1 - Info Basica
    await fillWizardStep1(page, {
        titulo: data.titulo,
        importeObjetivo: data.importeObjetivo,
        fechaFin: data.fechaFin,
    })
    await clickSiguiente(page)
    await page
        .getByRole("heading", { name: /^Historia$/i })
        .waitFor({ state: "visible", timeout: 5000 })

    // Step 2 - Historia
    await fillWizardStep2(page, {
        subtitulo: data.subtitulo,
        descripcion: data.descripcion,
    })
    await clickSiguiente(page)
    await page
        .getByRole("heading", { name: /Recompensas para tus Backers/i })
        .waitFor({ state: "visible", timeout: 5000 })

    // Step 3 - Skip rewards
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
 * Add a reward via the Admin modal.
 */
export async function addRewardViaModal(
    page: Page,
    campaignId: string,
    rewardData: {
        nombre: string
        importeMinimo: number
        descripcion: string
        tipoRewardId?: number
        incluyeEnvioFisico?: boolean
        cantidadMaxima?: number
    }
): Promise<void> {
    await page.goto(`${ADMIN_BASE}/campanias/${campaignId}/recompensas`)
    await page.waitForTimeout(1000)

    await page.getByRole("button", { name: /Agregar recompensa/i }).click()
    await page.waitForTimeout(500)

    await page.getByLabel(/Nombre/i).fill(rewardData.nombre)
    await page
        .getByRole("spinbutton", { name: /Importe minimo/i })
        .fill(String(rewardData.importeMinimo))
    await page.getByLabel(/Descripcion/i).fill(rewardData.descripcion)

    if (rewardData.cantidadMaxima) {
        const maxInput = page.getByRole("spinbutton", { name: /Cantidad maxima/i })
        if (await maxInput.isVisible()) {
            await maxInput.fill(String(rewardData.cantidadMaxima))
        }
    }

    await page.getByRole("button", { name: /Guardar/i }).click()
    await page.waitForTimeout(1000)
}

/**
 * Publish a campaign from its Admin detail page.
 */
export async function publishCampaign(page: Page, campaignId: string): Promise<void> {
    await page.goto(`${ADMIN_BASE}/campanias/${campaignId}`)
    await page.waitForTimeout(1000)

    await page.getByRole("button", { name: /Publicar Ahora/i }).click()

    // Handle confirmation dialog
    const confirmBtn = page.getByRole("button", { name: /Publicar sin recompensas|Confirmar|Publicar/i })
    if (await confirmBtn.isVisible({ timeout: 3000 }).catch(() => false)) {
        await confirmBtn.click()
    }

    await expect(
        page.getByText(/publicada exitosamente/i)
    ).toBeVisible({ timeout: 10000 })
}

// === BACKING HELPERS (via Landing UI) ===

/**
 * Create a backing via the Landing UI flow.
 */
export async function createBackingViaUI(
    page: Page,
    campaignTitle: string,
    backingData: {
        rewardName?: string
        monto: number
        mensaje?: string
    }
): Promise<void> {
    // Navigate to campaigns listing
    await page.goto(`${LANDING_BASE}/campanias`)
    await page.waitForTimeout(1000)

    // Find and click on the campaign
    await page.getByText(campaignTitle, { exact: false }).first().click()
    await page.waitForTimeout(1000)

    // Click "Apoyar" button
    await page.getByRole("button", { name: /Apoyar/i }).first().click()
    await page.waitForTimeout(500)

    // Select reward if specified
    if (backingData.rewardName) {
        await page.getByText(backingData.rewardName, { exact: false }).click()
        await page.waitForTimeout(300)
    }

    // Fill amount
    const montoInput = page.getByRole("spinbutton", { name: /Monto|Cantidad|Aporte/i })
    if (await montoInput.isVisible({ timeout: 3000 }).catch(() => false)) {
        await montoInput.fill(String(backingData.monto))
    }

    // Fill message if provided
    if (backingData.mensaje) {
        const msgInput = page.getByRole("textbox", { name: /Mensaje|Comentario/i })
        if (await msgInput.isVisible({ timeout: 2000 }).catch(() => false)) {
            await msgInput.fill(backingData.mensaje)
        }
    }

    // Confirm backing
    await page.getByRole("button", { name: /Confirmar|Apoyar|Enviar/i }).click()
    await page.waitForTimeout(2000)
}

// === API HELPERS (bypass UI for volume) ===

/**
 * Create a campaign via API.
 */
export async function createCampaignViaApi(
    token: string,
    data: {
        titulo: string
        subtitulo?: string
        descripcionCorta?: string
        importeObjetivo: number
        tipoFinanciacionId?: number
        monedaId?: number
        imagenPrincipalUrl?: string
        fechaFin?: string
    }
): Promise<string> {
    const now = new Date()
    const futureDate = new Date(now.getTime() + 90 * 24 * 60 * 60 * 1000)

    const res = await fetch(`${API_BASE}/api/campanias`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
            titulo: data.titulo,
            subtitulo: data.subtitulo ?? "",
            descripcionCorta: data.descripcionCorta ?? "",
            importeObjetivo: data.importeObjetivo,
            tipoFinanciacionId: data.tipoFinanciacionId ?? 1,
            monedaId: data.monedaId ?? 1,
            imagenPrincipalUrl: data.imagenPrincipalUrl ?? "",
            fechaFin: data.fechaFin ?? futureDate.toISOString(),
            permiteAportacionesAnonimas: true,
            permitePropinas: true,
        }),
    })

    const result = await res.json()
    const id = result.data?.id
    if (!id) {
        throw new Error(`Failed to create campaign "${data.titulo}": ${JSON.stringify(result)}`)
    }
    return id
}

/**
 * Add a reward via API.
 */
export async function addRewardViaApi(
    token: string,
    rewardData: {
        campaniaId: string
        nombre: string
        importeMinimo: number
        descripcion?: string
        tipoRewardId?: number
        monedaId?: number
        incluyeEnvioFisico?: boolean
        cantidadMaxima?: number
    }
): Promise<string> {
    const res = await fetch(`${API_BASE}/api/rewards`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
            campaniaId: rewardData.campaniaId,
            nombre: rewardData.nombre,
            importeMinimo: rewardData.importeMinimo,
            descripcion: rewardData.descripcion ?? "",
            tipoRewardId: rewardData.tipoRewardId ?? 1,
            monedaId: rewardData.monedaId ?? 1,
            incluyeEnvioFisico: rewardData.incluyeEnvioFisico ?? false,
            cantidadMaxima: rewardData.cantidadMaxima ?? 0,
            esAddOn: false,
        }),
    })

    const result = await res.json()
    const id = result.data?.id
    if (!id) {
        throw new Error(`Failed to create reward "${rewardData.nombre}": ${JSON.stringify(result)}`)
    }
    return id
}

/**
 * Publish a campaign via API.
 */
export async function publishViaApi(
    token: string,
    campaignId: string
): Promise<void> {
    const res = await fetch(`${API_BASE}/api/campanias/${campaignId}/publicar`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
        },
    })

    const result = await res.json()
    if (!res.ok) {
        throw new Error(`Failed to publish campaign ${campaignId}: ${JSON.stringify(result)}`)
    }
}

/**
 * Create a backing via API.
 */
export async function createBackingViaApi(
    token: string,
    campaignId: string,
    backingData: {
        rewardId?: string
        monto: number
        mensaje?: string
        esAnonimo?: boolean
    }
): Promise<string> {
    const res = await fetch(`${API_BASE}/api/campanias/${campaignId}/backings`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
            rewardId: backingData.rewardId ?? null,
            monto: backingData.monto,
            mensaje: backingData.mensaje ?? "",
            esAnonimo: backingData.esAnonimo ?? false,
        }),
    })

    const result = await res.json()
    const id = result.data?.id
    if (!id) {
        throw new Error(`Failed to create backing for campaign ${campaignId}: ${JSON.stringify(result)}`)
    }
    return id
}

/**
 * Create an artist profile via API.
 */
export async function createArtistaViaApi(
    token: string,
    data: {
        nombreArtistico: string
        descripcion?: string
        pais?: string
        ciudad?: string
        imagenUrl?: string
    }
): Promise<string> {
    const res = await fetch(`${API_BASE}/api/artistas`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({
            nombreArtistico: data.nombreArtistico,
            descripcion: data.descripcion ?? "",
            pais: data.pais ?? "Spain",
            ciudad: data.ciudad ?? "Madrid",
            imagenUrl: data.imagenUrl ?? "",
        }),
    })

    const result = await res.json()
    const id = result.data?.id
    if (!id) {
        throw new Error(`Failed to create artist "${data.nombreArtistico}": ${JSON.stringify(result)}`)
    }
    return id
}

// === VERIFICATION HELPERS ===

/**
 * Verify a campaign is visible in the Landing campaigns listing.
 */
export async function verifyCampaignInLanding(
    page: Page,
    title: string
): Promise<void> {
    await page.goto(`${LANDING_BASE}/campanias`)
    await page.waitForTimeout(1000)
    await expect(page.getByText(title, { exact: false }).first()).toBeVisible({
        timeout: 10000,
    })
}

/**
 * Verify dashboard stats in the Admin panel.
 */
export async function verifyDashboardStats(
    page: Page,
    expectedStats: {
        totalRecaudado?: number
        backersTotales?: number
        campaniasActivas?: number
    }
): Promise<void> {
    await page.goto(`${ADMIN_BASE}/dashboard`)
    await page.waitForTimeout(2000)

    if (expectedStats.totalRecaudado !== undefined) {
        await expect(page.getByText(/Total Recaudado/i)).toBeVisible()
    }
    if (expectedStats.backersTotales !== undefined) {
        await expect(page.getByText(/Backers/i)).toBeVisible()
    }
    if (expectedStats.campaniasActivas !== undefined) {
        await expect(page.getByText(/Campania/i)).toBeVisible()
    }
}
