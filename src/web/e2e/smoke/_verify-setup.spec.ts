import { test, expect } from "@playwright/test"
import {
    LANDING_BASE,
    ADMIN_BASE,
    API_BASE,
    loginViaApi,
    registerViaApi,
    loginToLandingUI,
    loginToAdminUI,
    registerViaLandingUI,
    createCampaignViaWizard,
    addRewardViaModal,
    publishCampaign,
    createBackingViaUI,
    createCampaignViaApi,
    addRewardViaApi,
    publishViaApi,
    createBackingViaApi,
    createArtistaViaApi,
    verifyCampaignInLanding,
    verifyDashboardStats,
    fillWizardStep1,
    fillWizardStep2,
    clickSiguiente,
} from "../helpers"

test.describe("Setup Verification", { tag: "@setup" }, () => {
    test("helpers export correctly", () => {
        expect(LANDING_BASE).toBe("http://localhost:3000")
        expect(ADMIN_BASE).toBe("http://localhost:3001")
        expect(API_BASE).toBe("http://localhost:5001")

        expect(typeof loginViaApi).toBe("function")
        expect(typeof registerViaApi).toBe("function")
        expect(typeof loginToLandingUI).toBe("function")
        expect(typeof loginToAdminUI).toBe("function")
        expect(typeof registerViaLandingUI).toBe("function")
        expect(typeof createCampaignViaWizard).toBe("function")
        expect(typeof addRewardViaModal).toBe("function")
        expect(typeof publishCampaign).toBe("function")
        expect(typeof createBackingViaUI).toBe("function")
        expect(typeof createCampaignViaApi).toBe("function")
        expect(typeof addRewardViaApi).toBe("function")
        expect(typeof publishViaApi).toBe("function")
        expect(typeof createBackingViaApi).toBe("function")
        expect(typeof createArtistaViaApi).toBe("function")
        expect(typeof verifyCampaignInLanding).toBe("function")
        expect(typeof verifyDashboardStats).toBe("function")
        expect(typeof fillWizardStep1).toBe("function")
        expect(typeof fillWizardStep2).toBe("function")
        expect(typeof clickSiguiente).toBe("function")
    })
})
