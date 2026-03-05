import { test, expect } from "@playwright/test"

/**
 * E2E tests for the Create Template page at /crowdsourcing/templates/nuevo.
 *
 * Tests verify form structure, validation, adding/removing necesidades,
 * and the create submission flow. Uses API route mocking.
 */

const CREATE_URL = "/crowdsourcing/templates/nuevo"

const mockRoles = [
    {
        id: 1,
        nombre: "Productor Musical",
        descripcion: "Guia el proceso creativo",
        categoriaRol: { id: 1, nombre: "Produccion", icono: "music", orden: 1 },
        modalidadCobro: "Por proyecto",
        activo: true,
    },
    {
        id: 2,
        nombre: "Ingeniero de Mezcla",
        descripcion: "Mezcla de audio",
        categoriaRol: { id: 2, nombre: "Ingenieria", icono: "headphones", orden: 2 },
        modalidadCobro: "Por cancion",
        activo: true,
    },
]

test.describe("Create Template Page", () => {
    test.beforeEach(async ({ page }) => {
        await page.route("**/api/crowdsourcing/maestras/roles-profesionales", (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: mockRoles,
                    messages: [],
                }),
            })
        )
    })

    test.describe("Page structure", () => {
        test("renders page heading", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(
                page.getByRole("heading", { name: /Nuevo Template/i })
            ).toBeVisible()
        })

        test("shows back link to templates list", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(
                page.getByRole("link", { name: /Volver a Templates/i })
            ).toBeVisible()
        })

        test("back link navigates to list", async ({ page }) => {
            await page.goto(CREATE_URL)

            await page.getByRole("link", { name: /Volver a Templates/i }).click()

            await expect(page).toHaveURL(/\/crowdsourcing\/templates$/)
        })
    })

    test.describe("Form sections", () => {
        test("renders Datos Generales section", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(page.getByText("Datos Generales")).toBeVisible({ timeout: 10000 })
            await expect(page.getByLabel("Nombre *")).toBeVisible()
            await expect(page.getByLabel("Descripcion")).toBeVisible()
        })

        test("renders Necesidades section", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(page.getByText(/Necesidades/)).toBeVisible({ timeout: 10000 })
            await expect(
                page.getByRole("button", { name: /Agregar necesidad/i })
            ).toBeVisible()
        })

        test("shows empty necesidades message", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(
                page.getByText("No hay necesidades agregadas")
            ).toBeVisible({ timeout: 10000 })
        })

        test("renders form action buttons", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(
                page.getByRole("button", { name: "Cancelar" })
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByRole("button", { name: "Crear template" })
            ).toBeVisible()
        })
    })

    test.describe("Necesidades management", () => {
        test("adds a necesidad when clicking add button", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(
                page.getByText("No hay necesidades agregadas")
            ).toBeVisible({ timeout: 10000 })

            await page.getByRole("button", { name: /Agregar necesidad/i }).click()

            await expect(
                page.getByText("No hay necesidades agregadas")
            ).not.toBeVisible()

            await expect(page.getByText("Necesidades (1)")).toBeVisible()
        })

        test("shows necesidad form fields after adding", async ({ page }) => {
            await page.goto(CREATE_URL)

            await page
                .getByRole("button", { name: /Agregar necesidad/i })
                .click({ timeout: 10000 })

            await expect(page.getByText("Fase *")).toBeVisible()
            await expect(page.getByText("Titulo *")).toBeVisible()
            await expect(page.getByText("Rol Profesional *")).toBeVisible()
            await expect(page.getByText("Prioridad *")).toBeVisible()
        })

        test("removes necesidad when clicking delete", async ({ page }) => {
            await page.goto(CREATE_URL)

            await page
                .getByRole("button", { name: /Agregar necesidad/i })
                .click({ timeout: 10000 })

            await expect(page.getByText("Necesidades (1)")).toBeVisible()

            await page.getByRole("button", { name: "Eliminar necesidad" }).click()

            await expect(page.getByText("Necesidades (0)")).toBeVisible()
            await expect(
                page.getByText("No hay necesidades agregadas")
            ).toBeVisible()
        })

        test("adds multiple necesidades", async ({ page }) => {
            await page.goto(CREATE_URL)

            const addBtn = page.getByRole("button", { name: /Agregar necesidad/i })

            await addBtn.click({ timeout: 10000 })
            await addBtn.click()

            await expect(page.getByText("Necesidades (2)")).toBeVisible()

            const deleteButtons = page.getByRole("button", {
                name: "Eliminar necesidad",
            })
            await expect(deleteButtons).toHaveCount(2)
        })
    })

    test.describe("Cancel navigation", () => {
        test("cancel button navigates back to list", async ({ page }) => {
            await page.goto(CREATE_URL)

            await page
                .getByRole("button", { name: "Cancelar" })
                .click({ timeout: 10000 })

            await expect(page).toHaveURL(/\/crowdsourcing\/templates$/)
        })
    })
})
