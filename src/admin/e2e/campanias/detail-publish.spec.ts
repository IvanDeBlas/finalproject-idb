import { test, expect, type Page } from "@playwright/test"

/**
 * E2E tests for the Campaign Detail/Preview page and Publish flow.
 *
 * Page: /campanias/[id]  (CampaniaPreviewPage)
 *
 * The detail page fetches a campaign by ID from the API and renders:
 *   - PreviewLayout: hero image, title, funding progress, tabs
 *   - DraftBanner: shown when campaign is in "borrador" state
 *   - Action buttons: Editar, Publicar Ahora, Eliminar (borrador only)
 *   - PublishConfirmModal: dialog to confirm publishing
 *
 * These tests mock the API to return a campaign in different states and verify
 * the UI renders correctly.
 */

const CAMPANIA_ID = "test-campania-id-123"
const DETAIL_URL = `/campanias/${CAMPANIA_ID}`

// ---------------------------------------------------------------------------
// Mock data
// ---------------------------------------------------------------------------

function createMockCampania(overrides: Record<string, unknown> = {}) {
    return {
        id: CAMPANIA_ID,
        titulo: "Ecos de Medianoche",
        subtitulo: "Un viaje musical sin precedentes",
        descripcionCorta:
            "Este album explora los sonidos de la noche, con influencias de jazz y electronica.",
        importeObjetivo: 10000,
        importePledgedActual: 2500,
        estadoCampaniaId: 1, // Borrador
        tipoFinanciacionId: 1,
        monedaId: 1,
        fechaCreacion: "2026-01-10T00:00:00Z",
        fechaFin: "2026-05-10T00:00:00Z",
        imagenPrincipalUrl: "",
        videoPrincipalUrl: "",
        permiteAportacionesAnonimas: false,
        permitePropinas: false,
        ...overrides,
    }
}

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

/** Set up API mock for a single campaign fetch. */
async function mockCampaniaAPI(
    page: Page,
    campania: ReturnType<typeof createMockCampania>
) {
    await page.route(`**/api/campanias/${CAMPANIA_ID}`, (route) =>
        route.fulfill({
            status: 200,
            contentType: "application/json",
            body: JSON.stringify({
                data: campania,
                messages: [],
            }),
        })
    )
}

// ---------------------------------------------------------------------------
// Tests
// ---------------------------------------------------------------------------

test.describe("Campaign Detail & Publish", () => {
    test.describe("Campaign preview content", () => {
        test("shows campaign title and subtitle", async ({ page }) => {
            const campania = createMockCampania()
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            await expect(
                page.getByRole("heading", {
                    name: /Ecos de Medianoche/i,
                })
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByText(/Un viaje musical sin precedentes/i)
            ).toBeVisible()
        })

        test("shows funding progress section", async ({ page }) => {
            const campania = createMockCampania()
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            // The progress bar is rendered inside a Card
            // Check for percentage text
            await expect(
                page.getByText(/financiado/i)
            ).toBeVisible({ timeout: 10000 })

            // Check for backers count
            await expect(page.getByText(/0 backers/i)).toBeVisible()
        })

        test("shows tabs: Historia, Actualizaciones, Comentarios", async ({
            page,
        }) => {
            const campania = createMockCampania()
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            await expect(
                page.getByRole("tab", { name: /Historia/i })
            ).toBeVisible({ timeout: 10000 })
            await expect(
                page.getByRole("tab", { name: /Actualizaciones/i })
            ).toBeVisible()
            await expect(
                page.getByRole("tab", { name: /Comentarios/i })
            ).toBeVisible()
        })

        test("Historia tab shows campaign description", async ({ page }) => {
            const campania = createMockCampania()
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            // Historia tab is default active
            await expect(
                page.getByText(
                    /Este album explora los sonidos de la noche/i
                )
            ).toBeVisible({ timeout: 10000 })
        })

        test("Actualizaciones tab shows empty message", async ({ page }) => {
            const campania = createMockCampania()
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            // Click the Actualizaciones tab
            await page
                .getByRole("tab", { name: /Actualizaciones/i })
                .click()

            await expect(
                page.getByText(
                    /No hay actualizaciones por el momento/i
                )
            ).toBeVisible()
        })

        test("Comentarios tab shows empty message", async ({ page }) => {
            const campania = createMockCampania()
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            await page
                .getByRole("tab", { name: /Comentarios/i })
                .click()

            await expect(
                page.getByText(
                    /No hay comentarios por el momento/i
                )
            ).toBeVisible()
        })

        test("shows sidebar with 'Apoyar esta campania' button", async ({
            page,
        }) => {
            const campania = createMockCampania()
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            await expect(
                page.getByRole("button", {
                    name: /Apoyar esta campania/i,
                })
            ).toBeVisible({ timeout: 10000 })
        })

        test("'Apoyar' button is disabled for borrador campaigns", async ({
            page,
        }) => {
            const campania = createMockCampania({ estadoCampaniaId: 1 })
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            const apoyarBtn = page.getByRole("button", {
                name: /Apoyar esta campania/i,
            })
            await expect(apoyarBtn).toBeVisible({ timeout: 10000 })
            await expect(apoyarBtn).toBeDisabled()
        })
    })

    test.describe("Draft banner", () => {
        test("shows VISTA PREVIA banner for borrador campaigns", async ({
            page,
        }) => {
            const campania = createMockCampania({ estadoCampaniaId: 1 })
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            await expect(
                page.getByText(/VISTA PREVIA/i)
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByText(/Campania en borrador/i)
            ).toBeVisible()
        })

        test("does not show VISTA PREVIA banner for published campaigns", async ({
            page,
        }) => {
            const campania = createMockCampania({ estadoCampaniaId: 2 }) // Publicada
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            // Wait for the page to load (title visible)
            await expect(
                page.getByRole("heading", {
                    name: /Ecos de Medianoche/i,
                })
            ).toBeVisible({ timeout: 10000 })

            // Banner should not be visible
            await expect(
                page.getByText(/VISTA PREVIA/i)
            ).not.toBeVisible()
        })
    })

    test.describe("Action buttons (borrador state)", () => {
        test("shows Editar, Publicar Ahora, and Eliminar buttons", async ({
            page,
        }) => {
            const campania = createMockCampania({ estadoCampaniaId: 1 })
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            await expect(
                page.getByRole("button", { name: /Editar/i })
            ).toBeVisible({ timeout: 10000 })
            await expect(
                page.getByRole("button", { name: /Publicar Ahora/i })
            ).toBeVisible()
            await expect(
                page.getByRole("button", { name: /Eliminar/i })
            ).toBeVisible()
        })

        test("does not show action buttons for published campaigns", async ({
            page,
        }) => {
            const campania = createMockCampania({ estadoCampaniaId: 2 })
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            await expect(
                page.getByRole("heading", {
                    name: /Ecos de Medianoche/i,
                })
            ).toBeVisible({ timeout: 10000 })

            // Action buttons should not be visible for published campaigns
            await expect(
                page.getByRole("button", { name: /Publicar Ahora/i })
            ).not.toBeVisible()
        })

        test("Editar button navigates to edit page", async ({ page }) => {
            const campania = createMockCampania({ estadoCampaniaId: 1 })
            await mockCampaniaAPI(page, campania)

            // Also mock the edit page's campaign fetch
            await page.route(`**/api/campanias/${CAMPANIA_ID}`, (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: campania,
                        messages: [],
                    }),
                })
            )

            await page.goto(DETAIL_URL)

            const editarBtn = page.getByRole("button", { name: /Editar/i })
            await expect(editarBtn).toBeVisible({ timeout: 10000 })
            await editarBtn.click()

            await expect(page).toHaveURL(
                new RegExp(`/campanias/${CAMPANIA_ID}/editar`)
            )
        })
    })

    test.describe("Publish confirmation modal", () => {
        test("opens publish modal when clicking 'Publicar Ahora'", async ({
            page,
        }) => {
            const campania = createMockCampania({ estadoCampaniaId: 1 })
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            const publishBtn = page.getByRole("button", {
                name: /Publicar Ahora/i,
            })
            await expect(publishBtn).toBeVisible({ timeout: 10000 })
            await publishBtn.click()

            // Dialog should appear
            await expect(
                page.getByRole("heading", {
                    name: /Publicar sin recompensas/i,
                })
            ).toBeVisible()

            // Should show the warning about no rewards
            await expect(
                page.getByText(
                    /Tu campania no tiene recompensas/i
                )
            ).toBeVisible()
        })

        test("modal has Cancelar and Publicar buttons", async ({ page }) => {
            const campania = createMockCampania({ estadoCampaniaId: 1 })
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            await page
                .getByRole("button", { name: /Publicar Ahora/i })
                .click()

            await expect(
                page.getByRole("button", { name: /Cancelar/i })
            ).toBeVisible()
            await expect(
                page.getByRole("button", {
                    name: /Publicar sin recompensas/i,
                })
            ).toBeVisible()
        })

        test("Cancelar closes the modal", async ({ page }) => {
            const campania = createMockCampania({ estadoCampaniaId: 1 })
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            await page
                .getByRole("button", { name: /Publicar Ahora/i })
                .click()

            // Modal is visible
            await expect(
                page.getByRole("heading", {
                    name: /Publicar sin recompensas/i,
                })
            ).toBeVisible()

            // Click cancel
            await page.getByRole("button", { name: /Cancelar/i }).click()

            // Modal should close
            await expect(
                page.getByRole("heading", {
                    name: /Publicar sin recompensas/i,
                })
            ).not.toBeVisible()
        })

        test("shows reward recommendation alert in modal", async ({
            page,
        }) => {
            const campania = createMockCampania({ estadoCampaniaId: 1 })
            await mockCampaniaAPI(page, campania)
            await page.goto(DETAIL_URL)

            await page
                .getByRole("button", { name: /Publicar Ahora/i })
                .click()

            // Warning about rewards
            await expect(
                page.getByText(
                    /Podras agregar recompensas despues/i
                )
            ).toBeVisible()
        })
    })

    test.describe("Not found state", () => {
        test("shows 'Campania no encontrada' when API returns null", async ({
            page,
        }) => {
            await page.route(`**/api/campanias/${CAMPANIA_ID}`, (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: null,
                        messages: [],
                    }),
                })
            )

            await page.goto(DETAIL_URL)

            await expect(
                page.getByText(/Campania no encontrada/i)
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByText(
                    /La campania que buscas no existe o fue eliminada/i
                )
            ).toBeVisible()
        })

        test("shows link to go back to 'mis campanias'", async ({
            page,
        }) => {
            await page.route(`**/api/campanias/${CAMPANIA_ID}`, (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify({
                        data: null,
                        messages: [],
                    }),
                })
            )

            await page.goto(DETAIL_URL)

            await expect(
                page.getByText(/Volver a mis campanias/i)
            ).toBeVisible({ timeout: 10000 })
        })
    })
})
