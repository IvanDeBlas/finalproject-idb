import { test, expect } from "@playwright/test"

test.describe("Programas de Promocion - Listado", () => {
    test("shows empty state when no programs", async ({ page }) => {
        await page.route("**/api/crowdpromotion/programas/mis-programas*", (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: { items: [], totalCount: 0, page: 1, pageSize: 10 },
                    messages: [],
                }),
            })
        )

        await page.goto("/crowdpromotion/programas")

        await expect(
            page.getByText(/No tienes programas de promocion/i)
        ).toBeVisible({ timeout: 10000 })

        await expect(
            page.getByText(/Crear primer programa/i)
        ).toBeVisible()
    })

    test("shows program cards when data exists", async ({ page }) => {
        await page.route("**/api/crowdpromotion/programas/mis-programas*", (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: {
                        items: [
                            {
                                id: "p1",
                                titulo: "Programa Referidos",
                                tipoPromoId: 1,
                                tipoPromoNombre: "Referral",
                                campaniaTitulo: "Mi Album",
                                esActivo: true,
                                importeComisionPorcentaje: 10,
                                importeComisionFija: null,
                                monedaNombre: "EUR",
                                numeroPromotores: 5,
                                numeroTareas: 2,
                                fechaInicio: "2026-01-01",
                                fechaFin: "2026-06-30",
                                fechaCreacion: "2025-12-20T10:00:00Z",
                            },
                        ],
                        totalCount: 1,
                        page: 1,
                        pageSize: 10,
                    },
                    messages: [],
                }),
            })
        )

        await page.goto("/crowdpromotion/programas")

        await expect(
            page.getByText("Programa Referidos")
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByText("Referral")).toBeVisible()
        await expect(page.getByText("ACTIVO")).toBeVisible()
        await expect(page.getByText("5 promotores")).toBeVisible()
    })

    test("navigates to create page", async ({ page }) => {
        await page.route("**/api/crowdpromotion/programas/mis-programas*", (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: { items: [], totalCount: 0, page: 1, pageSize: 10 },
                    messages: [],
                }),
            })
        )

        await page.goto("/crowdpromotion/programas")

        await page.getByText(/Crear primer programa/i).click()

        await expect(page).toHaveURL(/\/crowdpromotion\/programas\/nuevo/)
    })
})
