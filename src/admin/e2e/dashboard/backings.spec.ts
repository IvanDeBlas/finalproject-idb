import { test, expect } from "@playwright/test"

/**
 * E2E tests for the Campaign Backings page at /campanias/[id]/backings.
 *
 * This page shows backing stats, a searchable/filterable table of backings,
 * and an export CSV button. Uses route mocking for deterministic data.
 */

const BACKINGS_URL = "/campanias/campania-1/backings"

const mockCampania = {
    id: "campania-1",
    artistaId: "artista-123",
    titulo: "Mi Album Debut",
    subtitulo: "Primer album",
    descripcionCorta: "Un album increible",
    importeObjetivo: 5000,
    importePledgedActual: 2340,
    estadoCampaniaId: 2,
    fechaCreacion: "2026-01-15T12:00:00Z",
    fechaFin: "2026-03-31T23:59:59Z",
}

const mockStats = {
    campaniaId: "campania-1",
    totalBackers: 42,
    totalRecaudado: 5000,
    promedioAporte: 119,
    aporteMinimo: 10,
    aporteMaximo: 500,
    diasRestantes: 30,
}

const mockBackings = [
    {
        id: "backing-1",
        nombreBacker: "Maria Lopez",
        monto: 50,
        rewardNombre: "CD Fisico Firmado",
        mensaje: "Mucha suerte con el proyecto!",
        fechaCreacion: "2026-02-14T10:30:00Z",
    },
    {
        id: "backing-2",
        nombreBacker: "Anonimo",
        monto: 25,
        rewardNombre: null,
        mensaje: null,
        fechaCreacion: "2026-02-13T09:15:00Z",
    },
    {
        id: "backing-3",
        nombreBacker: "Carlos Sanchez",
        monto: 100,
        rewardNombre: "Vinilo Limitado",
        mensaje: null,
        fechaCreacion: "2026-02-12T15:45:00Z",
    },
]

function setupApiMocks(page: import("@playwright/test").Page) {
    return Promise.all([
        page.route("**/api/campanias/campania-1", (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: mockCampania,
                    messages: [{ message: "OK", errorCode: "0000" }],
                }),
            })
        ),
        page.route("**/api/campanias/campania-1/stats", (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: mockStats,
                    messages: [{ message: "OK", errorCode: "0000" }],
                }),
            })
        ),
        page.route("**/api/campanias/campania-1/backings*", (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: mockBackings,
                    messages: [{ message: "OK", errorCode: "0000" }],
                }),
            })
        ),
    ])
}

test.describe("Campaign Backings Page", () => {
    test.describe("Page structure", () => {
        test("renders the page heading", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(BACKINGS_URL)

            await expect(
                page.getByRole("heading", { name: /Apoyos Recibidos/i })
            ).toBeVisible({ timeout: 10000 })
        })

        test("shows campaign title as subtitle", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(BACKINGS_URL)

            await expect(
                page.getByText("Mi Album Debut")
            ).toBeVisible({ timeout: 10000 })
        })

        test("shows 'Volver a Campania' button", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(BACKINGS_URL)

            await expect(
                page.getByRole("button", { name: /Volver a Campania/i })
            ).toBeVisible({ timeout: 10000 })
        })
    })

    test.describe("Stats grid", () => {
        test("displays stats cards", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(BACKINGS_URL)

            await expect(
                page.getByText("Total Recaudado")
            ).toBeVisible({ timeout: 10000 })
            await expect(page.getByText("Total Apoyos")).toBeVisible()
        })
    })

    test.describe("Backings table", () => {
        test("renders backer names in table", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(BACKINGS_URL)

            await expect(
                page.getByText("Maria Lopez")
            ).toBeVisible({ timeout: 10000 })
            await expect(page.getByText("Carlos Sanchez")).toBeVisible()
        })

        test("shows reward names", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(BACKINGS_URL)

            await expect(
                page.getByText("CD Fisico Firmado")
            ).toBeVisible({ timeout: 10000 })
            await expect(page.getByText("Vinilo Limitado")).toBeVisible()
            await expect(page.getByText("Sin recompensa")).toBeVisible()
        })

        test("search input filters by backer name", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(BACKINGS_URL)

            await expect(
                page.getByText("Maria Lopez")
            ).toBeVisible({ timeout: 10000 })

            const searchInput = page.getByPlaceholder("Buscar por nombre...")
            await searchInput.fill("Carlos")

            await expect(page.getByText("Carlos Sanchez")).toBeVisible()
            await expect(page.getByText("Maria Lopez")).not.toBeVisible()
        })

        test("search shows no results message", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(BACKINGS_URL)

            await expect(
                page.getByText("Maria Lopez")
            ).toBeVisible({ timeout: 10000 })

            const searchInput = page.getByPlaceholder("Buscar por nombre...")
            await searchInput.fill("zzzzz")

            await expect(
                page.getByText(/No se encontraron apoyos/i)
            ).toBeVisible()
        })
    })

    test.describe("Export CSV", () => {
        test("shows export CSV button when backings exist", async ({
            page,
        }) => {
            await setupApiMocks(page)
            await page.goto(BACKINGS_URL)

            await expect(
                page.getByRole("button", { name: /Exportar CSV/i })
            ).toBeVisible({ timeout: 10000 })
        })

        test("hides export button when no backings", async ({ page }) => {
            await page.route("**/api/campanias/campania-1", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: mockCampania,
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            )
            await page.route("**/api/campanias/campania-1/stats", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: mockStats,
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            )
            await page.route("**/api/campanias/campania-1/backings*", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: [],
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            )

            await page.goto(BACKINGS_URL)

            await expect(
                page.getByText(/No hay apoyos aun/i)
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByRole("button", { name: /Exportar CSV/i })
            ).not.toBeVisible()
        })
    })

    test.describe("Empty state", () => {
        test("shows empty state when no backings exist", async ({ page }) => {
            await page.route("**/api/campanias/campania-1", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: mockCampania,
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            )
            await page.route("**/api/campanias/campania-1/stats", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: { ...mockStats, totalBackers: 0, totalRecaudado: 0 },
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            )
            await page.route("**/api/campanias/campania-1/backings*", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: [],
                        messages: [{ message: "OK", errorCode: "0000" }],
                    }),
                })
            )

            await page.goto(BACKINGS_URL)

            await expect(
                page.getByText(/No hay apoyos aun/i)
            ).toBeVisible({ timeout: 10000 })
            await expect(
                page.getByText(/Cuando recibas tus primeros aportes/i)
            ).toBeVisible()
        })
    })

    test.describe("Not found", () => {
        test("shows not found when campaign does not exist", async ({
            page,
        }) => {
            await page.route("**/api/campanias/campania-999", (route) =>
                route.fulfill({
                    status: 404,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: null,
                        messages: [
                            {
                                message: "Campania no encontrada",
                                errorCode: "2003",
                            },
                        ],
                    }),
                })
            )
            await page.route("**/api/campanias/campania-999/stats", (route) =>
                route.fulfill({
                    status: 404,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: null,
                        messages: [],
                    }),
                })
            )
            await page.route(
                "**/api/campanias/campania-999/backings*",
                (route) =>
                    route.fulfill({
                        status: 404,
                        contentType: "application/json",
                        body: JSON.stringify({
                            data: [],
                            messages: [],
                        }),
                    })
            )

            await page.goto("/campanias/campania-999/backings")

            await expect(
                page.getByText(/Campania no encontrada/i)
            ).toBeVisible({ timeout: 10000 })
        })
    })

    test.describe("Navigation", () => {
        test("'Volver a Campania' navigates back", async ({ page }) => {
            await setupApiMocks(page)
            await page.goto(BACKINGS_URL)

            const backButton = page.getByRole("button", {
                name: /Volver a Campania/i,
            })
            await expect(backButton).toBeVisible({ timeout: 10000 })
            await backButton.click()

            await expect(page).toHaveURL(
                /\/dashboard\/campanias\/campania-1/
            )
        })
    })
})
