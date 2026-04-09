import { test, expect } from "@playwright/test"

/**
 * E2E tests for the Artist Dashboard page at /dashboard.
 *
 * The dashboard shows stats cards, campaign list, recent backings,
 * and a chart placeholder. It fetches data from multiple API endpoints.
 *
 * Tests use route mocking to provide deterministic data without
 * requiring a running backend.
 */

const DASHBOARD_URL = "/dashboard"

const mockDashboardResumen = {
    artistaId: "artista-123",
    nombreArtistico: "Test Artist",
    totalRecaudado: 15340.5,
    totalBackers: 487,
    campaniasActivas: 2,
    campaniasCompletadas: 3,
    totalCampanias: 5,
    monedaSimbolo: "EUR",
    fechaUltimoAporte: "2026-02-14T10:30:00Z",
}

const mockArtista = {
    id: "artista-123",
    userId: "user-123",
    nombreArtistico: "Test Artist",
    descripcion: "Artista de prueba",
    generoMusical: "Rock",
}

const mockCampanias = [
    {
        id: "campania-1",
        artistaId: "artista-123",
        titulo: "Mi Album Debut",
        subtitulo: "Primer album",
        descripcionCorta: "Un album increible",
        imagenPrincipalUrl: "",
        importeObjetivo: 5000,
        importePledgedActual: 2340,
        estadoCampaniaId: 2,
        fechaCreacion: "2026-01-15T12:00:00Z",
        fechaFin: "2026-03-31T23:59:59Z",
    },
    {
        id: "campania-2",
        artistaId: "artista-123",
        titulo: "Gira Nacional 2026",
        subtitulo: "Gira por toda Espana",
        descripcionCorta: "Apoya nuestra gira",
        imagenPrincipalUrl: "",
        importeObjetivo: 10000,
        importePledgedActual: 7890,
        estadoCampaniaId: 2,
        fechaCreacion: "2026-01-10T09:00:00Z",
        fechaFin: "2026-03-01T23:59:59Z",
    },
]

function setupApiMocks(page: import("@playwright/test").Page) {
    return Promise.all([
        page.route("**/api/dashboard/resumen", (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: mockDashboardResumen,
                    messages: [{ message: "OK", errorCode: "0000" }],
                }),
            })
        ),
        page.route("**/api/artistas/me", (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: mockArtista,
                    messages: [{ message: "OK", errorCode: "0000" }],
                }),
            })
        ),
        page.route("**/api/campanias/mis-campanias", (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: mockCampanias,
                    messages: [{ message: "OK", errorCode: "0000" }],
                }),
            })
        ),
    ])
}

test.describe("Dashboard Page", () => {
    test.describe("Page structure", () => {
        test("renders the dashboard heading", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(DASHBOARD_URL)

            await expect(
                page.getByRole("heading", { name: /Dashboard/i })
            ).toBeVisible()
        })

        test("shows welcome message with artist name", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(DASHBOARD_URL)

            await expect(
                page.getByText(/Bienvenido, Test Artist/i)
            ).toBeVisible({ timeout: 10000 })
        })

        test("shows 'Nueva campania' button", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(DASHBOARD_URL)

            await expect(
                page.getByRole("link", { name: /Nueva campania/i })
            ).toBeVisible()
        })
    })

    test.describe("Stats cards", () => {
        test("displays all 4 stats cards with data", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(DASHBOARD_URL)

            await expect(
                page.getByText("Total Recaudado")
            ).toBeVisible({ timeout: 10000 })
            await expect(page.getByText("Backers Totales")).toBeVisible()
            await expect(page.getByText("Campanias Activas")).toBeVisible()
            await expect(page.getByText("Completadas")).toBeVisible()
        })

        test("shows loading skeletons before data loads", async ({ page }) => {
            // Delay the API response to catch loading state
            await page.route("**/api/dashboard/resumen", async (route) => {
                await new Promise((r) => setTimeout(r, 2000))
                await route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: mockDashboardResumen,
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            })
            await page.route("**/api/artistas/me", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: mockArtista,
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            )
            await page.route("**/api/campanias/mis-campanias", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: mockCampanias,
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            )

            await page.goto(DASHBOARD_URL)

            // Skeletons should be visible while loading
            const skeletons = page.locator('[class*="animate-pulse"]')
            await expect(skeletons.first()).toBeVisible({ timeout: 3000 })
        })
    })

    test.describe("Mis Campanias section", () => {
        test("displays campaign list", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(DASHBOARD_URL)

            await expect(
                page.getByText("Mis Campanias")
            ).toBeVisible({ timeout: 10000 })
            await expect(page.getByText("Mi Album Debut")).toBeVisible()
            await expect(page.getByText("Gira Nacional 2026")).toBeVisible()
        })

        test("shows 'Ver todas' link", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(DASHBOARD_URL)

            const verTodasLink = page.getByRole("link", { name: /Ver todas/i })
            await expect(verTodasLink).toBeVisible({ timeout: 10000 })
            await expect(verTodasLink).toHaveAttribute("href", "/campanias")
        })

        test("campaign items link to detail page", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(DASHBOARD_URL)

            await expect(
                page.getByText("Mi Album Debut")
            ).toBeVisible({ timeout: 10000 })

            const campaniaLink = page.getByRole("link", {
                name: /Mi Album Debut/i,
            })
            await expect(campaniaLink).toHaveAttribute(
                "href",
                "/campanias/campania-1"
            )
        })

        test("shows empty state when no campaigns", async ({ page }) => {
            await page.route("**/api/dashboard/resumen", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: { ...mockDashboardResumen, totalCampanias: 0 },
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            )
            await page.route("**/api/artistas/me", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: mockArtista,
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            )
            await page.route("**/api/campanias/mis-campanias", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: [],
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            )

            await page.goto(DASHBOARD_URL)

            await expect(
                page.getByText(/No tienes campanias aun/i)
            ).toBeVisible({ timeout: 10000 })
        })
    })

    test.describe("Chart placeholder", () => {
        test("shows chart placeholder section", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(DASHBOARD_URL)

            await expect(
                page.getByText("Recaudacion ultimos 30 dias")
            ).toBeVisible({ timeout: 10000 })
            await expect(
                page.getByText("Grafica proximamente")
            ).toBeVisible()
        })
    })

    test.describe("Navigation", () => {
        test("'Nueva campania' button navigates to create page", async ({
            page,
        }) => {
            await setupApiMocks(page)
            await page.goto(DASHBOARD_URL)

            const newCampaignLink = page.getByRole("link", {
                name: /Nueva campania/i,
            })
            await expect(newCampaignLink).toBeVisible({ timeout: 10000 })
            await newCampaignLink.click()

            await expect(page).toHaveURL(/\/campanias\/nueva/)
        })
    })

    test.describe("Error handling", () => {
        test("shows error alert when stats API fails", async ({ page }) => {
            await page.route("**/api/dashboard/resumen", (route) =>
                route.fulfill({
                    status: 500,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: null,
                        messages: [
                            {
                                message: "Error interno",
                                errorCode: "5000",
                            },
                        ],
                    }),
                })
            )
            await page.route("**/api/artistas/me", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: mockArtista,
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            )
            await page.route("**/api/campanias/mis-campanias", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: mockCampanias,
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            )

            await page.goto(DASHBOARD_URL)

            await expect(
                page.getByText(/Error al cargar estadisticas/i)
            ).toBeVisible({ timeout: 10000 })
        })
    })
})
