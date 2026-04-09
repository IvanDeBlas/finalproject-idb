import { test, expect } from "@playwright/test"

/**
 * E2E tests for the Templates List page at /crowdsourcing/templates.
 *
 * Tests verify page structure, empty/populated states, search filtering,
 * and navigation to create/detail pages. Uses API route mocking to
 * control data without requiring a running backend.
 */

const LIST_URL = "/crowdsourcing/templates"

const mockTemplates = [
    {
        id: "t-1",
        nombre: "Produccion de EP",
        descripcion: "Plantilla para EP",
        icono: "music",
        orden: 1,
        precioMinTotal: 3700,
        precioMaxTotal: 11400,
        moneda: 1,
        cantidadNecesidades: 8,
        fases: ["Preproduccion", "Grabacion"],
    },
    {
        id: "t-2",
        nombre: "Album Completo",
        descripcion: "Plantilla para album completo",
        icono: "disc",
        orden: 2,
        precioMinTotal: 8000,
        precioMaxTotal: 25000,
        moneda: 1,
        cantidadNecesidades: 12,
        fases: ["Preproduccion", "Grabacion", "Mezcla y Master"],
    },
]

test.describe("Templates List Page", () => {
    test.describe("Page structure", () => {
        test("renders the page heading", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByRole("heading", { name: /Templates de Proyecto/i })
            ).toBeVisible()
        })

        test("shows subtitle", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByText(/Gestiona las plantillas para artistas noveles/i)
            ).toBeVisible()
        })

        test("shows 'Nuevo Template' button", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByRole("button", { name: /Nuevo Template/i })
            ).toBeVisible()
        })

        test("shows search input", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByPlaceholderText("Buscar templates...")
            ).toBeVisible()
        })
    })

    test.describe("Empty state", () => {
        test("shows empty state when no templates", async ({ page }) => {
            await page.route("**/api/crowdsourcing/templates", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({ data: [], messages: [] }),
                })
            )

            await page.goto(LIST_URL)

            await expect(
                page.getByText("No hay templates disponibles")
            ).toBeVisible({ timeout: 10000 })
        })
    })

    test.describe("Populated state", () => {
        test.beforeEach(async ({ page }) => {
            await page.route("**/api/crowdsourcing/templates", (route) => {
                if (route.request().method() === "GET") {
                    return route.fulfill({
                        status: 200,
                        contentType: "application/json",
                        body: JSON.stringify({
                            data: mockTemplates,
                            messages: [],
                        }),
                    })
                }
                return route.continue()
            })
        })

        test("renders templates in table", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByText("Produccion de EP")
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByText("Album Completo")
            ).toBeVisible()
        })

        test("shows column headers", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(page.getByText("Nombre")).toBeVisible({ timeout: 10000 })
            await expect(page.getByText("Neces.")).toBeVisible()
            await expect(page.getByText("Estado")).toBeVisible()
        })

        test("shows necesidades count", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByText("Produccion de EP")
            ).toBeVisible({ timeout: 10000 })

            await expect(page.getByText("8")).toBeVisible()
            await expect(page.getByText("12")).toBeVisible()
        })

        test("shows active badges", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByText("Produccion de EP")
            ).toBeVisible({ timeout: 10000 })

            const badges = page.getByText("Activo")
            await expect(badges.first()).toBeVisible()
        })
    })

    test.describe("Search filtering", () => {
        test.beforeEach(async ({ page }) => {
            await page.route("**/api/crowdsourcing/templates", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: mockTemplates,
                        messages: [],
                    }),
                })
            )
        })

        test("filters templates by search query", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByText("Produccion de EP")
            ).toBeVisible({ timeout: 10000 })

            await page.getByPlaceholderText("Buscar templates...").fill("Album")

            await expect(page.getByText("Album Completo")).toBeVisible()
            await expect(page.getByText("Produccion de EP")).not.toBeVisible()
        })

        test("shows all templates when search is cleared", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByText("Produccion de EP")
            ).toBeVisible({ timeout: 10000 })

            const searchInput = page.getByPlaceholderText("Buscar templates...")
            await searchInput.fill("Album")
            await expect(page.getByText("Produccion de EP")).not.toBeVisible()

            await searchInput.clear()
            await expect(page.getByText("Produccion de EP")).toBeVisible()
            await expect(page.getByText("Album Completo")).toBeVisible()
        })
    })

    test.describe("Navigation", () => {
        test("'Nuevo Template' button navigates to create page", async ({ page }) => {
            await page.goto(LIST_URL)

            await page.getByRole("button", { name: /Nuevo Template/i }).click()

            await expect(page).toHaveURL(/\/crowdsourcing\/templates\/nuevo/)
        })

        test.beforeEach(async ({ page }) => {
            await page.route("**/api/crowdsourcing/templates", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: mockTemplates,
                        messages: [],
                    }),
                })
            )
        })

        test("'Ver' button navigates to detail page", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByText("Produccion de EP")
            ).toBeVisible({ timeout: 10000 })

            const viewBtn = page.getByRole("button", {
                name: /Ver template Produccion de EP/i,
            })
            await viewBtn.click()

            await expect(page).toHaveURL(/\/crowdsourcing\/templates\/t-1/)
        })

        test("'Editar' button navigates to edit page", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByText("Produccion de EP")
            ).toBeVisible({ timeout: 10000 })

            const editBtn = page.getByRole("button", {
                name: /Editar template Produccion de EP/i,
            })
            await editBtn.click()

            await expect(page).toHaveURL(/\/crowdsourcing\/templates\/t-1\/editar/)
        })
    })
})
