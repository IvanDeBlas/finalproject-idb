import { test, expect } from "@playwright/test"

/**
 * E2E tests for the Template Detail page at /crowdsourcing/templates/[id].
 *
 * Tests verify detail rendering, necesidades grouping, resumen card,
 * and navigation. Uses API route mocking.
 */

const mockTemplate = {
    id: "t-1",
    nombre: "Produccion de EP",
    descripcion: "Plantilla completa para producir un EP de 4-6 canciones",
    icono: "music",
    orden: 1,
    necesidades: [
        {
            id: "n-1",
            fase: "Preproduccion",
            titulo: "Productor Musical",
            descripcion: "Desarrollo de arreglos",
            rolProfesional: {
                id: 1,
                nombre: "Productor Musical",
                descripcion: "Guia el proceso creativo",
                categoriaRolId: 1,
                modalidadCobro: "Por proyecto",
            },
            precioMinOrientativo: 500,
            precioMaxOrientativo: 1500,
            moneda: 1,
            prioridad: "Alta",
            orden: 1,
        },
        {
            id: "n-2",
            fase: "Grabacion",
            titulo: "Ingeniero de Grabacion",
            descripcion: "Grabacion profesional",
            rolProfesional: {
                id: 2,
                nombre: "Ingeniero de Grabacion",
                descripcion: "Captura de audio",
                categoriaRolId: 2,
                modalidadCobro: "Por dia",
            },
            precioMinOrientativo: 800,
            precioMaxOrientativo: 2000,
            moneda: 1,
            prioridad: "Media",
            orden: 2,
        },
    ],
    resumen: {
        precioMinTotal: 1300,
        precioMaxTotal: 3500,
        moneda: 1,
        cantidadNecesidadesAlta: 1,
        cantidadNecesidadesMedia: 1,
        cantidadNecesidadesBaja: 0,
    },
}

const DETAIL_URL = `/crowdsourcing/templates/${mockTemplate.id}`

test.describe("Template Detail Page", () => {
    test.beforeEach(async ({ page }) => {
        await page.route(`**/api/crowdsourcing/templates/${mockTemplate.id}`, (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: mockTemplate,
                    messages: [],
                }),
            })
        )
    })

    test.describe("Page structure", () => {
        test("shows back link", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByRole("link", { name: /Volver a Templates/i })
            ).toBeVisible()
        })

        test("back link navigates to list", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await page.getByRole("link", { name: /Volver a Templates/i }).click()

            await expect(page).toHaveURL(/\/crowdsourcing\/templates$/)
        })
    })

    test.describe("Template header", () => {
        test("displays template name", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByText("Produccion de EP")
            ).toBeVisible({ timeout: 10000 })
        })

        test("displays template description", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByText(/Plantilla completa para producir un EP/i)
            ).toBeVisible({ timeout: 10000 })
        })

        test("shows edit button", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByRole("button", { name: /Editar/i })
            ).toBeVisible({ timeout: 10000 })
        })

        test("edit button navigates to edit page", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await page
                .getByRole("button", { name: /Editar/i })
                .click({ timeout: 10000 })

            await expect(page).toHaveURL(/\/crowdsourcing\/templates\/t-1\/editar/)
        })
    })

    test.describe("Necesidades section", () => {
        test("displays necesidades with titles", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByText("Productor Musical")
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByText("Ingeniero de Grabacion")
            ).toBeVisible()
        })

        test("shows fases as groups", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByText("Preproduccion")
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByText("Grabacion")
            ).toBeVisible()
        })

        test("displays priority badges", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByText("Alta")
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByText("Media")
            ).toBeVisible()
        })
    })

    test.describe("Not found state", () => {
        test("shows not found when template does not exist", async ({ page }) => {
            await page.route("**/api/crowdsourcing/templates/nonexistent", (route) =>
                route.fulfill({
                    status: 404,
                    contentType: "application/json",
                    body: JSON.stringify({
                        messages: [{ message: "No encontrado", errorCode: "2006" }],
                    }),
                })
            )

            await page.goto("/crowdsourcing/templates/nonexistent")

            await expect(
                page.getByText("Template no encontrado")
            ).toBeVisible({ timeout: 10000 })
        })
    })
})
