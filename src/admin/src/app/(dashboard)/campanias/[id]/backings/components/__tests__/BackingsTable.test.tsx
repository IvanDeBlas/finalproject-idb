import { render, screen, fireEvent } from "@/test-utils"
import { BackingsTable } from "../BackingsTable"
import type { BackingPublicDto } from "@shared/types"

const mockBackings: BackingPublicDto[] = [
    {
        id: "1",
        nombreBacker: "Maria Lopez",
        monto: 50,
        rewardNombre: "CD Fisico Firmado",
        mensaje: "Exito!",
        fechaCreacion: "2026-02-10T10:30:00Z",
    },
    {
        id: "2",
        nombreBacker: "Anonimo",
        monto: 25,
        rewardNombre: null,
        mensaje: null,
        fechaCreacion: "2026-02-09T09:15:00Z",
    },
    {
        id: "3",
        nombreBacker: "Carlos Sanchez",
        monto: 100,
        rewardNombre: "Vinilo Limitado",
        mensaje: null,
        fechaCreacion: "2026-02-08T08:45:00Z",
    },
]

const defaultProps = {
    backings: mockBackings,
    isLoading: false,
    searchTerm: "",
    onSearchChange: vi.fn(),
    selectedReward: null,
    onRewardFilterChange: vi.fn(),
}

describe("BackingsTable", () => {
    it("renders table headers", () => {
        render(<BackingsTable {...defaultProps} />)

        expect(screen.getByText("Backer")).toBeInTheDocument()
        expect(screen.getByText("Monto")).toBeInTheDocument()
        expect(screen.getByText("Recompensa")).toBeInTheDocument()
        expect(screen.getByText("Fecha")).toBeInTheDocument()
        expect(screen.getByText("Mensaje")).toBeInTheDocument()
    })

    it("renders backing rows", () => {
        render(<BackingsTable {...defaultProps} />)

        expect(screen.getByText("Maria Lopez")).toBeInTheDocument()
        expect(screen.getByText("Carlos Sanchez")).toBeInTheDocument()
    })

    it("displays anonymous backers with italic style", () => {
        render(<BackingsTable {...defaultProps} />)

        const anonimoElements = screen.getAllByText("Anonimo")
        const italicAnonimo = anonimoElements.find((el) =>
            el.classList.contains("italic")
        )
        expect(italicAnonimo).toBeDefined()
        expect(italicAnonimo).toHaveClass("italic", "text-muted-foreground")
    })

    it("shows 'Sin recompensa' when rewardNombre is null", () => {
        render(<BackingsTable {...defaultProps} />)

        expect(screen.getByText("Sin recompensa")).toBeInTheDocument()
    })

    it("renders Anonimo badge for anonymous backers", () => {
        render(<BackingsTable {...defaultProps} />)

        const anonimoElements = screen.getAllByText("Anonimo")
        // One italic name text + one badge
        expect(anonimoElements.length).toBeGreaterThanOrEqual(2)
    })

    it("renders empty state when no backings", () => {
        render(<BackingsTable {...defaultProps} backings={[]} />)

        expect(screen.getByText("No hay apoyos aun")).toBeInTheDocument()
    })

    it("filters backings by search term", () => {
        render(<BackingsTable {...defaultProps} searchTerm="maria" />)

        expect(screen.getByText("Maria Lopez")).toBeInTheDocument()
        expect(screen.queryByText("Carlos Sanchez")).not.toBeInTheDocument()
    })

    it("filters backings by reward", () => {
        render(
            <BackingsTable
                {...defaultProps}
                selectedReward="Vinilo Limitado"
            />
        )

        expect(screen.getByText("Carlos Sanchez")).toBeInTheDocument()
        expect(screen.queryByText("Maria Lopez")).not.toBeInTheDocument()
    })

    it("shows no results message when filters match nothing", () => {
        render(<BackingsTable {...defaultProps} searchTerm="xyz" />)

        expect(
            screen.getByText(
                "No se encontraron apoyos con los filtros aplicados"
            )
        ).toBeInTheDocument()
    })

    it("calls onSearchChange when typing in search input", () => {
        const onSearchChange = vi.fn()
        render(
            <BackingsTable
                {...defaultProps}
                onSearchChange={onSearchChange}
            />
        )

        const input = screen.getByPlaceholderText("Buscar por nombre...")
        fireEvent.change(input, { target: { value: "test" } })

        expect(onSearchChange).toHaveBeenCalledWith("test")
    })

    it("renders loading skeletons when isLoading is true", () => {
        const { container } = render(
            <BackingsTable {...defaultProps} isLoading={true} />
        )

        const skeletons = container.querySelectorAll('[class*="animate-pulse"]')
        expect(skeletons.length).toBeGreaterThan(0)
    })

    it("renders reward filter select when rewards exist", () => {
        render(<BackingsTable {...defaultProps} />)

        expect(
            screen.getByText("Todas las recompensas")
        ).toBeInTheDocument()
    })
})
