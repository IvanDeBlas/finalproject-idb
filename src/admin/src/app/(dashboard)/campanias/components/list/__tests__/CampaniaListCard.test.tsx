import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { CampaniaListCard } from "../CampaniaListCard"
import type { CampaniaListItem } from "@shared/types"

vi.mock("@shared/constants", () => ({
    CAMPANIA_ESTADOS: {
        BORRADOR: 1,
        PUBLICADA: 2,
        FINALIZADA: 3,
        CANCELADA: 4,
    },
    CAMPANIA_ESTADOS_LABELS: {
        1: "Borrador",
        2: "Publicada",
        3: "Finalizada",
        4: "Cancelada",
    },
    CAMPANIA_ESTADO_COLORS: {
        1: "yellow",
        2: "green",
        3: "blue",
        4: "red",
    },
}))

function buildCampania(
    overrides: Partial<CampaniaListItem> = {}
): CampaniaListItem {
    return {
        id: "camp-1",
        artistaId: "artist-1",
        titulo: "Mi Album Debut",
        descripcionCorta: "Un gran proyecto musical",
        importeObjetivo: 10000,
        importePledgedActual: 2500,
        estadoCampaniaId: 1,
        fechaCreacion: "2026-01-01T00:00:00Z",
        ...overrides,
    }
}

describe("CampaniaListCard", () => {
    const defaultHandlers = {
        onEdit: vi.fn(),
        onView: vi.fn(),
        onPublish: vi.fn(),
        onDelete: vi.fn(),
    }

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders campania title", () => {
        render(
            <CampaniaListCard
                campania={buildCampania()}
                {...defaultHandlers}
            />
        )

        expect(screen.getByText("Mi Album Debut")).toBeInTheDocument()
    })

    it("renders image when imagenPrincipalUrl provided", () => {
        render(
            <CampaniaListCard
                campania={buildCampania({
                    imagenPrincipalUrl: "https://example.com/image.jpg",
                })}
                {...defaultHandlers}
            />
        )

        const img = screen.getByAltText("Mi Album Debut")
        expect(img).toBeInTheDocument()
        expect(img).toHaveAttribute(
            "src",
            "https://example.com/image.jpg"
        )
    })

    it("renders fallback icon when no image", () => {
        render(
            <CampaniaListCard
                campania={buildCampania({ imagenPrincipalUrl: undefined })}
                {...defaultHandlers}
            />
        )

        // No img element should exist
        expect(
            screen.queryByAltText("Mi Album Debut")
        ).not.toBeInTheDocument()
    })

    it("shows correct percentage", () => {
        // 2500 / 10000 = 25%
        render(
            <CampaniaListCard
                campania={buildCampania({
                    importePledgedActual: 2500,
                    importeObjetivo: 10000,
                })}
                {...defaultHandlers}
            />
        )

        expect(screen.getByText("25% financiado")).toBeInTheDocument()

        const progressBar = screen.getByRole("progressbar")
        expect(progressBar).toHaveAttribute("aria-valuenow", "25")
    })

    it("calls onView when card clicked", async () => {
        const onView = vi.fn()
        const user = userEvent.setup()

        render(
            <CampaniaListCard
                campania={buildCampania()}
                {...defaultHandlers}
                onView={onView}
            />
        )

        // Click on the card title area (not on action buttons)
        await user.click(screen.getByText("Mi Album Debut"))

        expect(onView).toHaveBeenCalledWith("camp-1")
    })
})
