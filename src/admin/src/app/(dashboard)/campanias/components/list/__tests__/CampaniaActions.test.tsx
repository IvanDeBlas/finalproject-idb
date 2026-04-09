import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { CampaniaActions } from "../CampaniaActions"

vi.mock("@shared/constants", () => ({
    CAMPANIA_ESTADOS: {
        BORRADOR: 1,
        PUBLICADA: 2,
        FINALIZADA: 3,
        CANCELADA: 4,
    },
}))

describe("CampaniaActions", () => {
    const defaultHandlers = {
        onEdit: vi.fn(),
        onView: vi.fn(),
        onPublish: vi.fn(),
        onDelete: vi.fn(),
    }

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("shows Editar and Publicar buttons for borrador (estado=1)", () => {
        render(
            <CampaniaActions
                campaniaId="camp-1"
                estado={1}
                {...defaultHandlers}
            />
        )

        expect(
            screen.getByRole("button", { name: /Editar/ })
        ).toBeInTheDocument()
        expect(
            screen.getByRole("button", { name: /Publicar/ })
        ).toBeInTheDocument()
    })

    it("hides Editar and Publicar for published (estado=2)", () => {
        render(
            <CampaniaActions
                campaniaId="camp-1"
                estado={2}
                {...defaultHandlers}
            />
        )

        // The inline Editar/Publicar buttons should not exist
        // Only Ver and the dropdown trigger should be present
        const buttons = screen.getAllByRole("button")
        const buttonTexts = buttons.map((btn) => btn.textContent)

        expect(
            buttonTexts.some((text) => text?.includes("Editar"))
        ).toBe(false)
        expect(
            buttonTexts.some((text) => text?.includes("Publicar"))
        ).toBe(false)
    })

    it("always shows Ver button", () => {
        // Borrador
        const { unmount } = render(
            <CampaniaActions
                campaniaId="camp-1"
                estado={1}
                {...defaultHandlers}
            />
        )
        expect(
            screen.getByRole("button", { name: /Ver/ })
        ).toBeInTheDocument()
        unmount()

        // Publicada
        render(
            <CampaniaActions
                campaniaId="camp-1"
                estado={2}
                {...defaultHandlers}
            />
        )
        expect(
            screen.getByRole("button", { name: /Ver/ })
        ).toBeInTheDocument()
    })

    it("calls onView with campaniaId when Ver clicked", async () => {
        const onView = vi.fn()
        const user = userEvent.setup()

        render(
            <CampaniaActions
                campaniaId="camp-42"
                estado={1}
                {...defaultHandlers}
                onView={onView}
            />
        )

        const verButton = screen.getByRole("button", { name: /Ver/ })
        await user.click(verButton)

        expect(onView).toHaveBeenCalledWith("camp-42")
    })
})
