import { test, expect } from "@playwright/test"
import {
    LANDING_BASE,
    API_BASE,
    loginViaApi,
    registerViaApi,
    loginToLandingViaForm,
    registerViaLandingUI,
} from "../helpers"

/**
 * Smoke tests for authentication flows.
 * Independent - creates its own test data, no seeding dependency.
 *
 * @smoke
 */

const TIMESTAMP = Date.now()
const SMOKE_USER = {
    name: `Smoke Auth ${TIMESTAMP}`,
    email: `smoke-auth-${TIMESTAMP}@weplay-test.com`,
    password: "SmokeTest123!",
}

test.describe("@smoke Auth", () => {
    test.describe.configure({ mode: "serial" })

    test("Register a new user via API and Landing UI login", async ({ page }) => {
        // Register via API first (reliable), then verify UI login works
        const { token } = await registerViaApi(
            SMOKE_USER.email,
            SMOKE_USER.password,
            SMOKE_USER.name
        )
        expect(token).toBeTruthy()

        // Verify Landing UI login form works with pressSequentially
        await loginToLandingViaForm(page, SMOKE_USER.email, SMOKE_USER.password)
        const currentUrl = page.url()
        expect(currentUrl).not.toContain("/auth/login")
    })

    test("Register via Landing UI form", async ({ page }) => {
        const uiUser = {
            name: `Smoke UI Reg ${TIMESTAMP}`,
            email: `smoke-auth-ui-${TIMESTAMP}@weplay-test.com`,
            password: "SmokeTest123!",
        }

        // Wait for API response after submit
        const responsePromise = page.waitForResponse(
            (res) => res.url().includes("/api/auth/register") && res.status() === 200,
            { timeout: 20000 }
        ).catch(() => null)

        await registerViaLandingUI(page, uiUser.name, uiUser.email, uiUser.password)
        const response = await responsePromise

        // Verify: either redirected away OR API returned success
        const currentUrl = page.url()
        const redirected = !currentUrl.includes("/auth/register")
        expect(redirected || response !== null).toBeTruthy()
    })

    test("Login fails with wrong password", async ({ page }) => {
        await page.goto(`${LANDING_BASE}/auth/login`)
        await page.waitForLoadState("networkidle")

        const submitBtn = page.getByRole("main").getByRole("button", { name: /Iniciar sesion/i })
        await submitBtn.waitFor({ state: "visible", timeout: 10000 })

        const emailInput = page.locator("#email")
        const passwordInput = page.locator("#password")

        await emailInput.click()
        await emailInput.clear()
        await emailInput.pressSequentially(SMOKE_USER.email, { delay: 10 })

        await passwordInput.click()
        await passwordInput.clear()
        await passwordInput.pressSequentially("WrongPassword999!", { delay: 10 })

        await page.waitForTimeout(100)
        await submitBtn.click()

        // Should stay on login page or show error
        await page.waitForTimeout(2000)
        const currentUrl = page.url()
        const hasError =
            currentUrl.includes("/auth/login") ||
            (await page.getByText(/error|incorrecta|invalido|failed/i).isVisible({ timeout: 3000 }).catch(() => false))

        expect(hasError).toBeTruthy()
    })

    test("Login via API returns valid token", async () => {
        const { token, userId } = await loginViaApi(SMOKE_USER.email, SMOKE_USER.password)

        expect(token).toBeTruthy()
        expect(typeof token).toBe("string")
        expect(token.length).toBeGreaterThan(10)
        expect(userId).toBeTruthy()
    })

    test("API /auth/me returns user info with valid token", async () => {
        const { token } = await loginViaApi(SMOKE_USER.email, SMOKE_USER.password)

        const res = await fetch(`${API_BASE}/api/auth/me`, {
            headers: { Authorization: `Bearer ${token}` },
        })

        expect(res.ok).toBeTruthy()

        const data = await res.json()
        const user = data.data ?? data
        expect(user.email).toBe(SMOKE_USER.email)
    })

    test("API /auth/me rejects invalid token", async () => {
        const res = await fetch(`${API_BASE}/api/auth/me`, {
            headers: { Authorization: "Bearer invalid-token-12345" },
        })

        expect(res.status).toBeGreaterThanOrEqual(401)
    })
})
