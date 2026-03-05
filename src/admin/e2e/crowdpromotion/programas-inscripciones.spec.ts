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
    tareas: [],
    promotores: [],
    resumen: {
        totalPromotoresAprobados: 3,
        totalPromotoresPendientes: 2,
        totalPromotoresBloqueados: 1,
        totalEventos: 50,
        totalConversiones: 8,
        valorTotalGenerado: 1500,
    },
}

const mockInscripcionesPendientes = {
    data: {
        items: [
            {
                id: "insc-1",
                promotorId: "prom-1",
                promotorNombre: "DJ Marketing Pro",
                tipoPromotorNombre: "Influencer",
                promotorEmailContacto: "contacto@djmarketing.com",
                promotorUrlInstagram: "https://instagram.com/djmarketing",
                promotorUrlTikTok: null,
                promotorUrlSitioWeb: "https://djmarketing.com",
                esAprobado: false,
                esBloqueado: false,
                codigoReferido: null,
                fechaAlta: "2026-03-05T14:00:00Z",
                fechaBaja: null,
                estado: "Pendiente",
            },
            {
                id: "insc-2",
                promotorId: "prom-2",
                promotorNombre: "MusicFan2026",
                tipoPromotorNombre: "Fan Embajador",
                promotorEmailContacto: "fan@example.com",
                promotorUrlInstagram: null,
                promotorUrlTikTok: "https://tiktok.com/@musicfan",
                promotorUrlSitioWeb: null,
                esAprobado: false,
                esBloqueado: false,
                codigoReferido: null,
                fechaAlta: "2026-03-06T09:00:00Z",
                fechaBaja: null,
                estado: "Pendiente",
            },
        ],
        totalCount: 2,
        page: 1,
        pageSize: 20,
        totalPages: 1,
    },
    messages: [],
}

const mockInscripcionesAprobadas = {
    data: {
        items: [
            {
                id: "insc-3",
                promotorId: "prom-3",
                promotorNombre: "MusicBlog.es",
                tipoPromotorNombre: "Medio / Blog",
                promotorEmailContacto: "info@musicblog.es",
                promotorUrlInstagram: null,
                promotorUrlTikTok: null,
                promotorUrlSitioWeb: "https://musicblog.es",
                esAprobado: true,
                esBloqueado: false,
                codigoReferido: "album-2026-m3k2n",
                fechaAlta: "2026-03-06T09:00:00Z",
                fechaBaja: null,
                estado: "Aprobado",
            },
        ],
        totalCount: 1,
        page: 1,
        pageSize: 20,
        totalPages: 1,
    },
    messages: [],
}

const mockInscripcionesBloqueadas = {
    data: {
        items: [
            {
                id: "insc-4",
                promotorId: "prom-4",
                promotorNombre: "Spammer123",
                tipoPromotorNombre: "Fan Embajador",
                promotorEmailContacto: null,
                promotorUrlInstagram: null,
                promotorUrlTikTok: null,
                promotorUrlSitioWeb: null,
                esAprobado: false,
                esBloqueado: true,
                codigoReferido: null,
                fechaAlta: "2026-03-07T10:00:00Z",
                fechaBaja: null,
                estado: "Bloqueado",
            },
        ],
        totalCount: 1,
        page: 1,
        pageSize: 20,
        totalPages: 1,
    },
    messages: [],
}

const mockEmptyList = {
    data: {
        items: [],
        totalCount: 0,
        page: 1,
        pageSize: 20,
        totalPages: 0,
    },
    messages: [],
}

test.describe("Programas - Inscripciones Tabs", () => {
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

        await page.route(
            "**/api/crowdpromotion/programas/p1/inscripciones?estado=Pendiente*",
            (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify(mockInscripcionesPendientes),
                })
        )

        await page.route(
            "**/api/crowdpromotion/programas/p1/inscripciones?estado=Aprobado*",
            (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify(mockInscripcionesAprobadas),
                })
        )

        await page.route(
            "**/api/crowdpromotion/programas/p1/inscripciones?estado=Bloqueado*",
            (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify(mockInscripcionesBloqueadas),
                })
        )
    })

    test("shows Solicitudes tab with pending badge count", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        const solicitudesTab = page.getByRole("tab", { name: /Solicitudes/i })
        await expect(solicitudesTab).toBeVisible({ timeout: 10000 })

        await expect(page.getByLabel("2 solicitudes pendientes")).toBeVisible()
    })

    test("shows Aprobados and Bloqueados tabs", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await expect(
            page.getByRole("tab", { name: /Aprobados/i })
        ).toBeVisible({ timeout: 10000 })

        await expect(
            page.getByRole("tab", { name: /Bloqueados/i })
        ).toBeVisible()
    })

    test("Solicitudes tab shows pending promotor cards", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Solicitudes/i }).click()

        await expect(
            page.getByText("DJ Marketing Pro")
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByText("MusicFan2026")).toBeVisible()
    })

    test("Solicitudes tab shows action buttons", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Solicitudes/i }).click()

        await expect(
            page.getByText("DJ Marketing Pro")
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByText("Aprobar").first()).toBeVisible()
        await expect(page.getByText("Rechazar").first()).toBeVisible()
        await expect(page.getByText("Bloquear").first()).toBeVisible()
    })

    test("Solicitudes tab shows social links", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Solicitudes/i }).click()

        await expect(
            page.getByText("https://instagram.com/djmarketing")
        ).toBeVisible({ timeout: 10000 })
    })

    test("Aprobados tab shows approved promotor with codigo referido", async ({
        page,
    }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Aprobados/i }).click()

        await expect(
            page.getByText("MusicBlog.es")
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByText("album-2026-m3k2n")).toBeVisible()
    })

    test("Aprobados tab shows Dar de baja button", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Aprobados/i }).click()

        await expect(
            page.getByText("Dar de baja")
        ).toBeVisible({ timeout: 10000 })
    })

    test("Bloqueados tab shows blocked promotor with badge", async ({
        page,
    }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Bloqueados/i }).click()

        await expect(
            page.getByText("Spammer123")
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByText("BLOQUEADO")).toBeVisible()
    })

    test("Bloqueados tab shows informative blue banner", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Bloqueados/i }).click()

        await expect(
            page.getByText(/no pueden re-solicitar/i)
        ).toBeVisible({ timeout: 10000 })
    })

    test("Bloqueados tab has no action buttons", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Bloqueados/i }).click()

        await expect(
            page.getByText("Spammer123")
        ).toBeVisible({ timeout: 10000 })

        await expect(page.getByText("Dar de baja")).not.toBeVisible()
        await expect(page.getByText("Aprobar")).not.toBeVisible()
    })

    test("Rechazar button opens confirmation dialog", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Solicitudes/i }).click()

        await expect(
            page.getByText("DJ Marketing Pro")
        ).toBeVisible({ timeout: 10000 })

        await page.getByText("Rechazar").first().click()

        await expect(
            page.getByText("Rechazar solicitud")
        ).toBeVisible({ timeout: 5000 })

        await expect(page.getByText(/DJ Marketing Pro/)).toBeVisible()
    })

    test("Bloquear button opens confirmation dialog", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Solicitudes/i }).click()

        await expect(
            page.getByText("DJ Marketing Pro")
        ).toBeVisible({ timeout: 10000 })

        await page.getByText("Bloquear").first().click()

        await expect(
            page.getByText("Bloquear promotor")
        ).toBeVisible({ timeout: 5000 })

        await expect(
            page.getByText(/No podra volver a solicitar/)
        ).toBeVisible()
    })

    test("Dar de baja button opens confirmation dialog", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Aprobados/i }).click()

        await expect(
            page.getByText("MusicBlog.es")
        ).toBeVisible({ timeout: 10000 })

        await page.getByText("Dar de baja").click()

        await expect(
            page.getByText("Dar de baja al promotor")
        ).toBeVisible({ timeout: 5000 })

        await expect(
            page.getByText(/codigo referido quedara desactivado/)
        ).toBeVisible()
    })

    test("KPI cards show Promotores bloqueados", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await expect(
            page.getByText("Promotores bloqueados")
        ).toBeVisible({ timeout: 10000 })
    })
})

test.describe("Programas - Inscripciones Empty States", () => {
    test.beforeEach(async ({ page }) => {
        await page.route("**/api/crowdpromotion/programas/p1", (route) =>
            route.fulfill({
                status: 200,
                contentType: "application/json",
                body: JSON.stringify({
                    data: {
                        ...mockProgramaDetail,
                        resumen: {
                            ...mockProgramaDetail.resumen,
                            totalPromotoresPendientes: 0,
                        },
                    },
                    messages: [],
                }),
            })
        )

        await page.route(
            "**/api/crowdpromotion/programas/p1/inscripciones*",
            (route) =>
                route.fulfill({
                    status: 200,
                    contentType: "application/json",
                    body: JSON.stringify(mockEmptyList),
                })
        )
    })

    test("Solicitudes tab shows empty state", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Solicitudes/i }).click()

        await expect(
            page.getByText("No hay solicitudes pendientes")
        ).toBeVisible({ timeout: 10000 })
    })

    test("Aprobados tab shows empty state", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Aprobados/i }).click()

        await expect(
            page.getByText("No hay promotores aprobados en este programa")
        ).toBeVisible({ timeout: 10000 })
    })

    test("Bloqueados tab shows empty state", async ({ page }) => {
        await page.goto("/crowdpromotion/programas/p1")

        await page.getByRole("tab", { name: /Bloqueados/i }).click()

        await expect(
            page.getByText("No hay promotores bloqueados")
        ).toBeVisible({ timeout: 10000 })
    })
})
