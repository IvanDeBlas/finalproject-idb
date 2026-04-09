import { render, screen } from "@/test-utils"
import { BackingStatsGrid } from "../BackingStatsGrid"
import type { CampaniaStats } from "@shared/types"

const mockStats: CampaniaStats = {
    campaniaId: "campania-1",
    totalBackers: 42,
    totalRecaudado: 5000,
    promedioAporte: 119,
    aporteMinimo: 10,
    aporteMaximo: 500,
    diasRestantes: 30,
}

const emptyStats: CampaniaStats = {
    campaniaId: "campania-1",
    totalBackers: 0,
    totalRecaudado: 0,
    promedioAporte: 0,
    aporteMinimo: 0,
    aporteMaximo: 0,
    diasRestantes: 30,
}

describe("BackingStatsGrid", () => {
    it("renders all three stat cards", () => {
        render(<BackingStatsGrid stats={mockStats} isLoading={false} />)

        expect(screen.getByText("Total Recaudado")).toBeInTheDocument()
        expect(screen.getByText("Total Apoyos")).toBeInTheDocument()
        expect(screen.getByText("Promedio por Aporte")).toBeInTheDocument()
    })

    it("displays stats values correctly", () => {
        render(<BackingStatsGrid stats={mockStats} isLoading={false} />)

        expect(screen.getByText("42")).toBeInTheDocument()
    })

    it("displays zero state when stats are empty", () => {
        render(<BackingStatsGrid stats={emptyStats} isLoading={false} />)

        expect(screen.getByText("0")).toBeInTheDocument()
    })

    it("renders loading skeletons when isLoading is true", () => {
        const { container } = render(
            <BackingStatsGrid stats={undefined} isLoading={true} />
        )

        const skeletons = container.querySelectorAll('[class*="animate-pulse"]')
        expect(skeletons.length).toBeGreaterThan(0)
    })

    it("renders stats without undefined when stats is undefined and not loading", () => {
        render(<BackingStatsGrid stats={undefined} isLoading={false} />)

        expect(screen.getByText("Total Recaudado")).toBeInTheDocument()
        expect(screen.getByText("0")).toBeInTheDocument()
    })
})
