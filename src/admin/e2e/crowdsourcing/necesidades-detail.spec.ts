import { test, expect } from "@playwright/test"

/**
 * E2E tests for the Necesidad Detail page at /crowdsourcing/necesidades/[id].
 *
 * Tests verify detail structure, propuestas section, and action buttons.
 * Uses API route mocking for backend responses.
 */

const mockNecesidadDetail = {
    id: "nec-1",
    titulo: "Mezcla de pistas para EP",
    descripcion: "Buscamos un ingeniero de mezcla experimentado en indie rock.",
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
    numeroPropuestas: 2,
    fechaCreacion: "2026-02-16T10:00:00Z",
    fechaLimitePropuestas: "2026-03-15T00:00:00Z",
    fechaActualizacion: null,
    ubicacionCiudad: null,
    ubicacionPais: null,
    fechaInicioPrevista: "2026-04-01T00:00:00Z",
    proyectoArtisticoId: "proj-1",
    proyectoArtisticoNombre: "Mi Primer EP",
    propuestas: [
        {
            id: "prop-1",
            profesionalId: "prof-1",
            profesionalNombre: "Juan Perez",
            precioPropuesto: 600,
            monedaId: 1,
            tiempoEstimadoDias: 14,
            mensaje: "Tengo 8 anos de experiencia mezclando indie rock.",
            estadoPropuestaId: 1,
            estadoPropuestaNombre: "Pendiente",
            fechaCreacion: "2026-02-17T09:15:00Z",
        },
        {
            id: "prop-2",
            profesionalId: "prof-2",
            profesionalNombre: "Maria Garcia",
            precioPropuesto: 450,
            monedaId: 1,
            tiempoEstimadoDias: 10,
            mensaje: "Especializada en produccion indie.",
            estadoPropuestaId: 1,
            estadoPropuestaNombre: "Pendiente",
            fechaCreacion: "2026-02-18T11:30:00Z",
        },
    ],
}

const DETAIL_URL = "/crowdsourcing/necesidades/nec-1"

test.describe("Necesidad Detail Page", () => {
    test.beforeEach(async ({ page }) => {
        await page.route("**/api/crowdsourcing/necesidades/nec-1", (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: mockNecesidadDetail,
                    messages: [],
                }),
            })
        )
    })

    test.describe("Header", () => {
        test("renders necesidad title", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByRole("heading", { name: /Mezcla de pistas para EP/i })
            ).toBeVisible({ timeout: 10000 })
        })

        test("shows estado badge", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByText("Mezcla de pistas para EP")
            ).toBeVisible({ timeout: 10000 })

            await expect(page.getByText("Abierta")).toBeVisible()
        })

        test("shows Editar button for Abierta necesidad", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByText("Mezcla de pistas para EP")
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByRole("link", { name: /Editar/i })
            ).toBeVisible()
        })

        test("shows Cerrar button for Abierta necesidad", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByText("Mezcla de pistas para EP")
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByRole("button", { name: /Cerrar Necesidad/i })
            ).toBeVisible()
        })
    })

    test.describe("Details card", () => {
        test("shows descripcion", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByText(/Buscamos un ingeniero de mezcla/i)
            ).toBeVisible({ timeout: 10000 })
        })

        test("shows tipo necesidad", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByText("Mezcla de pistas para EP")
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByText("Ingenieria de Audio")
            ).toBeVisible()
        })
    })

    test.describe("Propuestas section", () => {
        test("shows propuestas count", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByText("Mezcla de pistas para EP")
            ).toBeVisible({ timeout: 10000 })

            await expect(
                page.getByText(/2 Propuestas/i)
            ).toBeVisible()
        })

        test("shows profesional names", async ({ page }) => {
            await page.goto(DETAIL_URL)

            await expect(
                page.getByText("Mezcla de pistas para EP")
            ).toBeVisible({ timeout: 10000 })

            await expect(page.getByText("Juan Perez")).toBeVisible()
            await expect(page.getByText("Maria Garcia")).toBeVisible()
        })
    })
})
