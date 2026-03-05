import { test, expect } from "@playwright/test"

/**
 * E2E tests for the Campaign List page at /campanias.
 *
 * The list page (MisCampaniasPage) uses TanStack Query to fetch campaigns from
 * the API. Without a running backend, the page will show either the loading
 * skeleton or the empty state (when the query resolves with no data / error).
 *
 * These tests focus on:
 *   - Page structure and heading rendering
 *   - Empty state UI when no campaigns exist
 *   - Navigation button to create a new campaign
 *   - Header "Nueva campania" button (when campaigns exist, the button is in the header)
 *
 * NOTE: For tests that validate campaign cards rendering, mock the API or seed
 * the database with test data before running.
 */

const LIST_URL = "/campanias"

test.describe("Campaign List Page", () => {
    test.describe("Page structure", () => {
        test("renders the page heading", async ({ page }) => {
            await page.goto(LIST_URL)

            // The page always shows this heading (both in empty and populated state)
            await expect(
                page.getByRole("heading", { name: /Mis Campanias/i })
            ).toBeVisible()
        })

        test("shows subtitle about managing campaigns", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByText(
                    /Gestiona tus campanias de crowdfunding/i
                )
            ).toBeVisible()
        })
    })

    test.describe("Empty state", () => {
        test("shows empty state when no campaigns are returned", async ({
            page,
        }) => {
            // Mock the API to return empty list
            await page.route("**/api/campanias/mis-campanias", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({ data: [], messages: [] }),
                })
            )

            await page.goto(LIST_URL)

            // Wait for loading to finish and empty state to appear
            await expect(
                page.getByText(/No tienes campanias aun/i)
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByText(
                    /Crea tu primera campania para empezar a recaudar/i
                )
            ).toBeVisible()
        })

        test("shows 'Crear tu primera campania' action button in empty state", async ({
            page,
        }) => {
            await page.route("**/api/campanias/mis-campanias", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({ data: [], messages: [] }),
                })
            )

            await page.goto(LIST_URL)

            const createBtn = page.getByRole("button", {
                name: /Crear tu primera campania/i,
            })
            await expect(createBtn).toBeVisible({ timeout: 10000 })
        })

        test("'Crear tu primera campania' button navigates to wizard", async ({
            page,
        }) => {
            await page.route("**/api/campanias/mis-campanias", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({ data: [], messages: [] }),
                })
            )

            await page.goto(LIST_URL)

            const createBtn = page.getByRole("button", {
                name: /Crear tu primera campania/i,
            })
            await expect(createBtn).toBeVisible({ timeout: 10000 })
            await createBtn.click()

            await expect(page).toHaveURL(/\/campanias\/nueva/)
        })
    })

    test.describe("Populated state", () => {
        const mockCampania = {
            id: "test-campania-id-1",
            titulo: "Mi Album Increible",
            subtitulo: "Un disco que cambiara todo",
            descripcionCorta: "Descripcion de prueba",
            importeObjetivo: 5000,
            importePledgedActual: 1200,
            estadoCampaniaId: 1, // Borrador
            tipoFinanciacionId: 1,
            monedaId: 1,
            fechaCreacion: "2026-01-15T00:00:00Z",
            fechaFin: "2026-04-15T00:00:00Z",
            imagenPrincipalUrl: "",
            videoPrincipalUrl: "",
            permiteAportacionesAnonimas: false,
            permitePropinas: false,
        }

        test("shows 'Nueva campania' button in header when campaigns exist", async ({
            page,
        }) => {
            await page.route("**/api/campanias/mis-campanias", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: [mockCampania],
                        messages: [],
                    }),
                })
            )

            await page.goto(LIST_URL)

            const nuevaBtn = page.getByRole("button", {
                name: /Nueva campania/i,
            })
            await expect(nuevaBtn).toBeVisible({ timeout: 10000 })
        })

        test("'Nueva campania' button navigates to wizard", async ({
            page,
        }) => {
            await page.route("**/api/campanias/mis-campanias", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: [mockCampania],
                        messages: [],
                    }),
                })
            )

            await page.goto(LIST_URL)

            const nuevaBtn = page.getByRole("button", {
                name: /Nueva campania/i,
            })
            await expect(nuevaBtn).toBeVisible({ timeout: 10000 })
            await nuevaBtn.click()

            await expect(page).toHaveURL(/\/campanias\/nueva/)
        })

        test("renders campaign card with title", async ({ page }) => {
            await page.route("**/api/campanias/mis-campanias", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: [mockCampania],
                        messages: [],
                    }),
                })
            )

            await page.goto(LIST_URL)

            await expect(
                page.getByText("Mi Album Increible")
            ).toBeVisible({ timeout: 10000 })
        })
    })
})
