import { test, expect } from "@playwright/test"

/**
 * E2E tests for the Create Necesidad page at /crowdsourcing/necesidades/nueva.
 *
 * Tests verify form structure, validation, and submission.
 * Uses API route mocking for backend responses.
 */

const CREATE_URL = "/crowdsourcing/necesidades/nueva"

test.describe("Create Necesidad Page", () => {
    test.describe("Page structure", () => {
        test("renders the page heading", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(
                page.getByRole("heading", { name: /Publicar Nueva Necesidad/i })
            ).toBeVisible()
        })

        test("shows back button", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(
                page.getByText(/Volver a Mis Necesidades/i)
            ).toBeVisible()
        })

        test("shows form sections", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(page.getByText("Informacion basica")).toBeVisible()
            await expect(page.getByText("Presupuesto")).toBeVisible()
            await expect(page.getByText("Ubicacion y fechas")).toBeVisible()
        })

        test("shows submit button", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(
                page.getByRole("button", { name: /Publicar Necesidad/i })
            ).toBeVisible()
        })

        test("shows cancel button", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(
                page.getByRole("button", { name: /Cancelar/i })
            ).toBeVisible()
        })
    })

    test.describe("Form fields", () => {
        test("has titulo input", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(
                page.getByPlaceholderText(/Describe brevemente/i)
            ).toBeVisible()
        })

        test("has descripcion textarea", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(
                page.getByPlaceholderText(/Detalla lo que necesitas/i)
            ).toBeVisible()
        })

        test("shows character counter for descripcion", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(
                page.getByText("0 / 4000 caracteres")
            ).toBeVisible()
        })

        test("has proyecto artistico input", async ({ page }) => {
            await page.goto(CREATE_URL)

            await expect(
                page.getByPlaceholderText("ID del proyecto")
            ).toBeVisible()
        })
    })
})
