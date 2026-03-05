import { test, expect, type Page } from "@playwright/test"

/**
 * E2E tests for the 4-step campaign creation wizard at /campanias/nueva.
 *
 * These tests verify UI structure and navigation flow against the rendered
 * Next.js pages. They do NOT require a running backend API -- they validate
 * that components render, stepper state updates, and step navigation works
 * correctly through form submissions and button clicks.
 *
 * Wizard steps:
 *   1. Informacion Basica  (BasicInfoStep)
 *   2. Historia             (StoryStep)
 *   3. Recompensas          (RewardsStep - MVP placeholder)
 *   4. Revision             (ReviewStep)
 */

const WIZARD_URL = "/campanias/nueva"

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

/** Clear wizard localStorage draft so each test starts fresh. */
async function clearWizardDraft(page: Page) {
    await page.evaluate(() => {
        localStorage.removeItem("wizard-draft")
    })
}

/** Fill the minimum required fields on Step 1 and submit. */
async function fillStep1AndProceed(page: Page) {
    await page.getByLabel(/Titulo de la campania/i).fill("Mi Album de Prueba")

    // Open the Select and choose "Todo o Nada"
    await page.getByRole("combobox").click()
    await page.getByRole("option", { name: /Todo o Nada/i }).click()

    // Fill importe objetivo
    await page.getByLabel(/Meta de financiacion/i).fill("5000")

    // Submit step 1
    await page.getByRole("button", { name: /Siguiente/i }).click()
}

/** Fill optional fields on Step 2 and submit. */
async function fillStep2AndProceed(page: Page) {
    await page.getByLabel(/Subtitulo/i).fill("Subtitulo de prueba")
    await page.getByLabel(/Descripcion corta/i).fill("Una descripcion corta para el proyecto musical de prueba.")

    await page.getByRole("button", { name: /Siguiente/i }).click()
}

/** Skip Step 3 (rewards MVP placeholder) by clicking Siguiente. */
async function skipStep3(page: Page) {
    await page.getByRole("button", { name: /Siguiente/i }).click()
}

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

test.describe("Campaign Creation Wizard", () => {
    test.beforeEach(async ({ page }) => {
        await page.goto(WIZARD_URL)
        await clearWizardDraft(page)
        // Reload to ensure clean state after clearing localStorage
        await page.goto(WIZARD_URL)
        // Wait for wizard container to hydrate
        await expect(
            page.getByRole("heading", { name: /Crear Nueva Campania/i })
        ).toBeVisible()
    })

    // -----------------------------------------------------------------------
    // Stepper rendering
    // -----------------------------------------------------------------------

    test("displays wizard stepper with 4 steps", async ({ page }) => {
        // Verify stepper labels (desktop stepper)
        await expect(page.getByText("Informacion Basica")).toBeVisible()
        await expect(page.getByText("Historia")).toBeVisible()
        await expect(page.getByText("Recompensas")).toBeVisible()
        await expect(page.getByText("Revision")).toBeVisible()

        // Step 1 should be active (aria-current="step")
        const step1Button = page.locator('button[aria-current="step"]')
        await expect(step1Button).toBeVisible()
        await expect(step1Button).toHaveText("1")
    })

    // -----------------------------------------------------------------------
    // Step 1 -> Step 2
    // -----------------------------------------------------------------------

    test("Step 1: fills basic info and navigates to step 2", async ({
        page,
    }) => {
        // Verify we see the Step 1 card heading
        await expect(
            page.getByRole("heading", { name: /Informacion Basica/i })
        ).toBeVisible()

        // Verify the "Paso 1 de 4" indicator
        await expect(page.getByText("Paso 1 de 4")).toBeVisible()

        // Anterior button should be disabled on step 1
        const anteriorBtn = page.getByRole("button", { name: /Anterior/i })
        await expect(anteriorBtn).toBeDisabled()

        // Fill required fields and proceed
        await fillStep1AndProceed(page)

        // Now step 2 should be visible
        await expect(
            page.getByRole("heading", { name: /^Historia$/i })
        ).toBeVisible()
        await expect(page.getByText("Paso 2 de 4")).toBeVisible()
    })

    // -----------------------------------------------------------------------
    // Step 2 -> Step 3
    // -----------------------------------------------------------------------

    test("Step 2: fills story and navigates to step 3", async ({ page }) => {
        // Navigate to step 2
        await fillStep1AndProceed(page)

        await expect(
            page.getByRole("heading", { name: /^Historia$/i })
        ).toBeVisible()

        // Fill step 2 fields
        await fillStep2AndProceed(page)

        // Step 3 should now appear
        await expect(
            page.getByRole("heading", {
                name: /Recompensas para tus Backers/i,
            })
        ).toBeVisible()
        await expect(page.getByText("Paso 3 de 4")).toBeVisible()
    })

    // -----------------------------------------------------------------------
    // Step 3 -> Step 4
    // -----------------------------------------------------------------------

    test("Step 3: skips rewards (MVP) and goes to step 4", async ({
        page,
    }) => {
        await fillStep1AndProceed(page)
        await fillStep2AndProceed(page)

        // Verify MVP placeholder texts on rewards step
        await expect(
            page.getByText(/Aun no has creado recompensas/i)
        ).toBeVisible()
        await expect(
            page.getByText(
                /La gestion de recompensas estara disponible tras crear/i
            )
        ).toBeVisible()

        // Proceed to step 4
        await skipStep3(page)

        // Step 4: Revision Final
        await expect(
            page.getByRole("heading", { name: /Revision Final/i })
        ).toBeVisible()
    })

    // -----------------------------------------------------------------------
    // Step 4: Review content
    // -----------------------------------------------------------------------

    test("Step 4: shows review of all entered data", async ({ page }) => {
        await fillStep1AndProceed(page)
        await fillStep2AndProceed(page)
        await skipStep3(page)

        // Verify the review card heading
        await expect(
            page.getByRole("heading", { name: /Revision Final/i })
        ).toBeVisible()

        // Titulo should appear in the preview
        await expect(page.getByText("Mi Album de Prueba")).toBeVisible()

        // Importe objetivo -- rendered as "5.000" (es-ES locale) with currency symbol
        await expect(page.getByText(/5.000/)).toBeVisible()

        // Descripcion corta text
        await expect(
            page.getByText(
                /Una descripcion corta para el proyecto musical de prueba/i
            )
        ).toBeVisible()

        // Tipo financiacion label should appear
        await expect(page.getByText("Todo o Nada")).toBeVisible()

        // "Crear campania" button should be present
        await expect(
            page.getByRole("button", { name: /Crear campania/i })
        ).toBeVisible()

        // "Guardar borrador" button should also be present
        await expect(
            page.getByRole("button", { name: /Guardar borrador/i })
        ).toBeVisible()

        // BORRADOR warning note
        await expect(
            page.getByText(/Tu campania se creara como BORRADOR/i)
        ).toBeVisible()
    })

    // -----------------------------------------------------------------------
    // Back navigation
    // -----------------------------------------------------------------------

    test("can navigate back through steps", async ({ page }) => {
        // Go to step 3
        await fillStep1AndProceed(page)
        await fillStep2AndProceed(page)

        // On step 3 -- click Anterior to go back to step 2
        await expect(
            page.getByRole("heading", {
                name: /Recompensas para tus Backers/i,
            })
        ).toBeVisible()
        await page.getByRole("button", { name: /Anterior/i }).click()

        // Should be on step 2
        await expect(
            page.getByRole("heading", { name: /^Historia$/i })
        ).toBeVisible()
        await expect(page.getByText("Paso 2 de 4")).toBeVisible()

        // Click Anterior again to go back to step 1
        await page.getByRole("button", { name: /Anterior/i }).click()

        await expect(
            page.getByRole("heading", { name: /Informacion Basica/i })
        ).toBeVisible()
        await expect(page.getByText("Paso 1 de 4")).toBeVisible()
    })

    // -----------------------------------------------------------------------
    // Stepper click navigation
    // -----------------------------------------------------------------------

    test("stepper allows clicking completed steps", async ({ page }) => {
        // Complete step 1
        await fillStep1AndProceed(page)

        // Now on step 2 -- click step 1 in the stepper (completed step shows checkmark)
        // The completed step 1 button should be clickable. It has a Check icon.
        // We target the stepper button that is not the current step (step 1 is completed).
        const stepperButtons = page.locator(
            '[role="progressbar"] button:not([disabled])'
        )
        // Step 1 should be the first clickable button in the stepper
        const step1StepperBtn = stepperButtons.first()
        await step1StepperBtn.click()

        // Should navigate back to step 1
        await expect(
            page.getByRole("heading", { name: /Informacion Basica/i })
        ).toBeVisible()

        // Previously entered data should still be there
        await expect(
            page.getByLabel(/Titulo de la campania/i)
        ).toHaveValue("Mi Album de Prueba")
    })

    // -----------------------------------------------------------------------
    // Validation: Step 1 required fields
    // -----------------------------------------------------------------------

    test("Step 1: shows validation errors for empty required fields", async ({
        page,
    }) => {
        // Try to submit step 1 without filling anything
        await page.getByRole("button", { name: /Siguiente/i }).click()

        // Should show validation error for titulo
        await expect(
            page.getByText(/El titulo es obligatorio/i)
        ).toBeVisible()

        // Should still be on step 1
        await expect(
            page.getByRole("heading", { name: /Informacion Basica/i })
        ).toBeVisible()
    })

    // -----------------------------------------------------------------------
    // Page heading for create mode
    // -----------------------------------------------------------------------

    test("shows 'Crear Nueva Campania' heading in create mode", async ({
        page,
    }) => {
        await expect(
            page.getByRole("heading", { name: /Crear Nueva Campania/i })
        ).toBeVisible()
        await expect(
            page.getByText(
                /Completa los pasos para publicar tu proyecto musical/i
            )
        ).toBeVisible()
    })

    // -----------------------------------------------------------------------
    // Full wizard flow (end to end through all steps)
    // -----------------------------------------------------------------------

    test("full wizard flow: step 1 through step 4", async ({ page }) => {
        // Step 1
        await expect(page.getByText("Paso 1 de 4")).toBeVisible()
        await fillStep1AndProceed(page)

        // Step 2
        await expect(page.getByText("Paso 2 de 4")).toBeVisible()
        await fillStep2AndProceed(page)

        // Step 3
        await expect(page.getByText("Paso 3 de 4")).toBeVisible()
        await skipStep3(page)

        // Step 4 -- all data visible
        await expect(page.getByText("Mi Album de Prueba")).toBeVisible()
        await expect(
            page.getByText(
                /Una descripcion corta para el proyecto musical de prueba/i
            )
        ).toBeVisible()
        await expect(
            page.getByRole("button", { name: /Crear campania/i })
        ).toBeVisible()
    })
})
