import { test, expect } from "@playwright/test"

test.describe("Programas de Promocion - Wizard Crear", () => {
    test("shows wizard header and stepper", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/nuevo")

        await expect(
            page.getByText("Crear programa de promocion")
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByText("Datos basicos")).toBeVisible()
        await expect(page.getByText("Comisiones")).toBeVisible()
        await expect(page.getByText("Definir tareas")).toBeVisible()
        await expect(page.getByText("Revisar")).toBeVisible()
    })

    test("shows step 1 form fields", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/nuevo")

        await expect(
            page.getByLabel(/Titulo/i)
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByText("Paso 1 de 4")).toBeVisible()
    })

    test("validates required fields on step 1", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/nuevo")

        await page.waitForTimeout(1000)

        // Click Siguiente without filling required fields
        await page.getByRole("button", { name: /Siguiente/i }).click()

        await expect(
            page.getByText(/El titulo es obligatorio/i)
        ).toBeVisible({ timeout: 5000 })
    })

    test("shows abandon dialog on X click", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/nuevo")

        await page.waitForTimeout(1000)

        // Click the X button in header
        const closeButton = page.locator("button").filter({ has: page.locator("svg") }).first()
        await closeButton.click()

        await expect(
            page.getByText("Salir del wizard")
        ).toBeVisible({ timeout: 5000 })

        await expect(
            page.getByText("Seguir editando")
        ).toBeVisible()
    })
})
