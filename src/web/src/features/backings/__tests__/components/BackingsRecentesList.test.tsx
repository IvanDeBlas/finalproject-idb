import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { BackingsRecentesList } from "@/features/campanias/presentation/components/BackingsRecentesList"
import type { BackingPublicDto } from "@shared/types/backing"

const mockBackings: BackingPublicDto[] = [
    {
        id: "700e8400-e29b-41d4-a716-446655440001",
        nombreBacker: "Maria Lopez",
        monto: 25,
        rewardNombre: "CD Fisico Firmado",
        mensaje: "Mucha suerte con el proyecto!",
        fechaCreacion: new Date().toISOString(),
    },
    {
        id: "800e8400-e29b-41d4-a716-446655440002",
        nombreBacker: "Anonimo",
        monto: 50,
        rewardNombre: undefined,
        mensaje: undefined,
        fechaCreacion: new Date().toISOString(),
    },
]

describe("BackingsRecentesList", () => {
    it("renders list of backings", () => {
        render(<BackingsRecentesList backings={mockBackings} />)

        expect(screen.getByText("Maria Lopez")).toBeInTheDocument()
        expect(screen.getByText("Anonimo")).toBeInTheDocument()
    })

    it("shows 'Apoyos Recientes' title", () => {
        render(<BackingsRecentesList backings={mockBackings} />)

        expect(screen.getByText("Apoyos Recientes")).toBeInTheDocument()
    })

    it("shows amounts for each backing", () => {
        render(<BackingsRecentesList backings={mockBackings} />)

        expect(screen.getByText(/25/)).toBeInTheDocument()
        expect(screen.getByText(/50/)).toBeInTheDocument()
    })

    it("shows reward name when available", () => {
        render(<BackingsRecentesList backings={mockBackings} />)

        expect(screen.getByText(/CD Fisico Firmado/)).toBeInTheDocument()
    })

    it("shows message when available", () => {
        render(<BackingsRecentesList backings={mockBackings} />)

        expect(screen.getByText(/Mucha suerte con el proyecto/)).toBeInTheDocument()
    })

    it("shows empty state when no backings", () => {
        render(<BackingsRecentesList backings={[]} />)

        expect(screen.getByText(/Se el primero en apoyar/)).toBeInTheDocument()
    })

    it("shows custom empty message", () => {
        render(
            <BackingsRecentesList
                backings={[]}
                emptyMessage="No hay aportes aun"
            />
        )

        expect(screen.getByText("No hay aportes aun")).toBeInTheDocument()
    })

    it("shows loading skeletons when isLoading", () => {
        const { container } = render(
            <BackingsRecentesList backings={[]} isLoading={true} />
        )

        // Should show skeleton placeholders, not empty state
        expect(screen.queryByText(/Se el primero en apoyar/)).not.toBeInTheDocument()
        // Skeletons are rendered as divs with bg-[#1e293b]
        const skeletons = container.querySelectorAll("[class*='animate-pulse']")
        expect(skeletons.length).toBeGreaterThan(0)
    })
})
