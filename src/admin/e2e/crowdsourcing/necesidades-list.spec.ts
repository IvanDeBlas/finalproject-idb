import { test, expect } from "@playwright/test"

/**
 * E2E tests for the Necesidades List page at /crowdsourcing/necesidades.
 *
 * Tests verify page structure, empty/populated states, filtering,
 * and navigation. Uses API route mocking to control data without
 * requiring a running backend.
 */

const LIST_URL = "/crowdsourcing/necesidades"

const mockNecesidades = {
    items: [
        {
            id: "nec-1",
            titulo: "Mezcla de pistas para EP",
            estadoNecesidadId: 1,
            estadoNecesidadNombre: "Abierta",
            tipoNecesidadId: 3,
            tipoNecesidadNombre: "Ingenieria de Audio",
            presupuestoMin: 150,
            presupuestoMax: 800,
            monedaId: 1,
            monedaNombre: "EUR",
            modalidadTrabajoId: 2,
            modalidadTrabajoNombre: "Remoto",
            numeroPropuestas: 3,
            fechaCreacion: "2026-02-16T10:00:00Z",
            fechaLimitePropuestas: "2026-03-15T00:00:00Z",
            fechaActualizacion: null,
        },
        {
            id: "nec-2",
            titulo: "Diseno de portada del album",
            estadoNecesidadId: 2,
            estadoNecesidadNombre: "En Progreso",
            tipoNecesidadId: 5,
            tipoNecesidadNombre: "Diseno Grafico",
            presupuestoMin: 200,
            presupuestoMax: 500,
            monedaId: 1,
            monedaNombre: "EUR",
            modalidadTrabajoId: 2,
            modalidadTrabajoNombre: "Remoto",
            numeroPropuestas: 1,
            fechaCreacion: "2026-02-10T10:00:00Z",
            fechaLimitePropuestas: null,
            fechaActualizacion: "2026-02-14T16:20:00Z",
        },
    ],
    totalCount: 2,
    page: 1,
    pageSize: 12,
    totalPages: 1,
}

test.describe("Necesidades List Page", () => {
    test.describe("Page structure", () => {
        test("renders the page heading", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByRole("heading", { name: /Mis Necesidades/i })
            ).toBeVisible()
        })

        test("shows Nueva Necesidad button", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByRole("link", { name: /Nueva Necesidad/i })
            ).toBeVisible()
        })

        test("shows search input", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByPlaceholderText(/buscar por titulo/i)
            ).toBeVisible()
        })
    })

    test.describe("Empty state", () => {
        test("shows empty state when no necesidades", async ({ page }) => {
            await page.route("**/api/crowdsourcing/necesidades/mis-necesidades*", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: {
                            items: [],
                            totalCount: 0,
                            page: 1,
                            pageSize: 12,
                            totalPages: 0,
                        },
                        messages: [],
                    }),
                })
            )

            await page.goto(LIST_URL)

            await expect(
                page.getByText("No tienes necesidades publicadas")
            ).toBeVisible({ timeout: 10000 })
        })
    })

    test.describe("Populated state", () => {
        test.beforeEach(async ({ page }) => {
            await page.route("**/api/crowdsourcing/necesidades/mis-necesidades*", (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: mockNecesidades,
                        messages: [],
                    }),
                })
            )
        })

        test("renders necesidad cards", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByText("Mezcla de pistas para EP")
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByText("Diseno de portada del album")
            ).toBeVisible()
        })

        test("shows estado badges", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByText("Mezcla de pistas para EP")
            ).toBeVisible({ timeout: 10000 })

            await expect(page.getByText("Abierta")).toBeVisible()
            await expect(page.getByText("En Progreso")).toBeVisible()
        })

        test("shows propuestas count", async ({ page }) => {
            await page.goto(LIST_URL)

            await expect(
                page.getByText("Mezcla de pistas para EP")
            ).toBeVisible({ timeout: 10000 })

            await expect(page.getByText("3 propuestas")).toBeVisible()
        })
    })

    test.describe("Navigation", () => {
        test("Nueva Necesidad navigates to create page", async ({ page }) => {
            await page.goto(LIST_URL)

            await page.getByRole("link", { name: /Nueva Necesidad/i }).click()

            await expect(page).toHaveURL(/\/crowdsourcing\/necesidades\/nueva/)
        })
    })
})
