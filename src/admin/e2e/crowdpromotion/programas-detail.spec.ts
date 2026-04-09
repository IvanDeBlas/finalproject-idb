import { test, expect } from "@playwright/test"

const mockProgramaDetail = {
    id: "p1",
    titulo: "Programa Referidos Q1",
    descripcion: "Programa de referidos para Q1",
    tipoPromoId: 1,
    tipoPromoNombre: "Referral",
    campaniaCrowdfundingId: "c1",
    campaniaTitulo: "Mi Album Debut",
    proyectoArtisticoId: null,
    urlLanding: "https://example.com",
    codigoTrackingBase: "ref-q1",
    monedaId: 1,
    monedaNombre: "EUR",
    importeComisionPorcentaje: 10,
    importeComisionFija: null,
    esActivo: true,
    fechaInicio: "2026-01-01",
    fechaFin: "2026-06-30",
    fechaCreacion: "2025-12-20T10:00:00Z",
    fechaActualizacion: null,
    tareas: [
        {
            id: "t1",
            titulo: "Compartir en Instagram",
            descripcion: null,
            tipoEventoPromoId: 3,
            tipoEventoPromoNombre: "Share",
            tipoRewardId: 1,
            tipoRewardNombre: "Dinero",
            importeRecompensa: 10,
            monedaId: 1,
            monedaNombre: "EUR",
            puntosRecompensa: null,
            urlInstrucciones: null,
            esRepetible: false,
            maxRepeticiones: null,
            orden: 1,
            esActivo: true,
            fechaInicio: null,
            fechaFin: null,
            completadosPorPromotores: 3,
        },
    ],
    promotores: [
        {
            id: "pr1",
            promotorNombre: "DJ Promo",
            tipoPromotorNombre: "Influencer",
            esAprobado: true,
            esBloqueado: false,
            fechaAlta: "2026-02-10T10:00:00Z",
        },
    ],
    resumen: {
        totalPromotoresAprobados: 3,
        totalPromotoresPendientes: 1,
        totalEventos: 50,
        totalConversiones: 8,
        valorTotalGenerado: 1500,
    },
}

test.describe("Programas de Promocion - Detalle", () => {
    test.beforeEach(async ({ page }) => {
        await page.route("**/api/crowdpromotion/programas/p1", (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: mockProgramaDetail,
                    messages: [],
                }),
            })
        )
    })

    test("shows program title and badges", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await expect(
            page.getByText("Programa Referidos Q1")
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByText("Referral")).toBeVisible()
        await expect(page.getByText("ACTIVO")).toBeVisible()
    })

    test("shows KPI cards", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await expect(
            page.getByText("Promotores aprobados")
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByText("Promotores pendientes")).toBeVisible()
        await expect(page.getByText("Total eventos")).toBeVisible()
        await expect(page.getByText("Valor generado")).toBeVisible()
    })

    test("shows tabs", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await expect(
            page.getByRole("tab", { name: /Info general/i })
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByRole("tab", { name: /Tareas/i })).toBeVisible()
        await expect(page.getByRole("tab", { name: /Promotores/i })).toBeVisible()
        await expect(page.getByRole("tab", { name: /Resumen/i })).toBeVisible()
    })

    test("shows Desactivar button for active program", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await expect(
            page.getByText("Desactivar")
        ).toBeVisible({ timeout: 10000 })
    })

    test("opens desactivar dialog", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByText("Desactivar").click()

        await expect(
            page.getByText("Desactivar programa")
        ).toBeVisible({ timeout: 5000 })

        await expect(
            page.getByText(/Programa Referidos Q1/)
        ).toBeVisible()
    })
})
