import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import { MisCampaniasCard } from "../MisCampaniasCard"
import { mockMisCampanias } from "@/__mocks__/dashboard.mock"

describe("MisCampaniasCard", () => {
    it("renders card title", () => {
        render(<MisCampaniasCard campanias={mockMisCampanias} />)

        expect(screen.getByText("Mis Campanias")).toBeInTheDocument()
    })

    it("renders campania items", () => {
        render(<MisCampaniasCard campanias={mockMisCampanias} />)

        expect(screen.getByText("Mi Album Debut")).toBeInTheDocument()
        expect(screen.getByText("Gira Nacional 2026")).toBeInTheDocument()
        expect(screen.getByText("EP Acustico")).toBeInTheDocument()
    })

    it("renders 'Ver todas' link", () => {
        render(<MisCampaniasCard campanias={mockMisCampanias} />)

        const link = screen.getByRole("link", { name: "Ver todas" })
        expect(link).toBeInTheDocument()
        expect(link).toHaveAttribute("href", "/campanias")
    })

    it("shows loading skeletons when isLoading", () => {
        const { container } = render(<MisCampaniasCard isLoading={true} />)

        const skeletons = container.querySelectorAll('[class*="animate-pulse"]')
        expect(skeletons.length).toBeGreaterThan(0)
    })

    it("shows empty state when no campanias", () => {
        render(<MisCampaniasCard campanias={[]} />)

        expect(
            screen.getByText("No tienes campanias aun")
        ).toBeInTheDocument()
    })

    it("shows create campania link in empty state", () => {
        render(<MisCampaniasCard campanias={[]} />)

        const link = screen.getByRole("link", { name: "Crear campania" })
        expect(link).toBeInTheDocument()
        expect(link).toHaveAttribute("href", "/campanias/nueva")
    })

    it("limits displayed campanias to 5", () => {
        const manyCampanias = Array.from({ length: 8 }, (_, i) => ({
            ...mockMisCampanias[0],
            id: `campania-${i}`,
            titulo: `Campania ${i}`,
        }))

        render(<MisCampaniasCard campanias={manyCampanias} />)

        const items = screen.getAllByRole("link")
        // "Ver todas" link + 5 campania links
        const campaniaLinks = items.filter((link) =>
            link.getAttribute("href")?.startsWith("/campanias/campania-")
        )
        expect(campaniaLinks).toHaveLength(5)
    })

    it("renders without campanias prop", () => {
        render(<MisCampaniasCard />)

        expect(
            screen.getByText("No tienes campanias aun")
        ).toBeInTheDocument()
    })
})
