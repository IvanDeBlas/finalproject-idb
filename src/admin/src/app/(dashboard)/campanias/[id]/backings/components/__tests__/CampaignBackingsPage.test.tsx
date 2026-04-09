import { render, screen } from "@/test-utils"
import CampaignBackingsPage from "../../page"
import type { BackingPublicDto, CampaniaStats } from "@shared/types"

const mockPush = vi.fn()
vi.mock("next/navigation", () => ({
    useParams: () => ({ id: "campania-1" }),
    useRouter: () => ({ push: mockPush }),
}))

vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

const mockBackings: BackingPublicDto[] = [
    {
        id: "1",
        nombreBacker: "Maria Lopez",
        monto: 50,
        rewardNombre: "CD Fisico",
        mensaje: "Exito!",
        fechaCreacion: "2026-02-10T10:00:00Z",
    },
    {
        id: "2",
        nombreBacker: "Anonimo",
        monto: 25,
        rewardNombre: null,
        mensaje: null,
        fechaCreacion: "2026-02-09T09:00:00Z",
    },
]

const mockStats: CampaniaStats = {
    campaniaId: "campania-1",
    totalBackers: 42,
    totalRecaudado: 5000,
    promedioAporte: 119,
    aporteMinimo: 10,
    aporteMaximo: 500,
    diasRestantes: 30,
}

let mockCampania: { titulo: string } | null = { titulo: "Mi Campania Test" }
let mockCampaniaLoading = false
let mockBackingsData: BackingPublicDto[] = mockBackings
let mockBackingsLoading = false
let mockBackingsError: Error | null = null
let mockStatsData: CampaniaStats | undefined = mockStats
let mockStatsLoading = false

vi.mock("@/hooks/use-campanias", () => ({
    useCampania: () => ({
        data: mockCampania,
        isLoading: mockCampaniaLoading,
    }),
    usePublicarCampania: () => ({ mutateAsync: vi.fn() }),
    useDeleteCampania: () => ({ mutateAsync: vi.fn() }),
}))

vi.mock("@/hooks/use-backings", () => ({
    useCampaignBackings: () => ({
        data: mockBackingsData,
        isLoading: mockBackingsLoading,
        error: mockBackingsError,
    }),
    useCampaignStats: () => ({
        data: mockStatsData,
        isLoading: mockStatsLoading,
    }),
}))

describe("CampaignBackingsPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
        mockCampania = { titulo: "Mi Campania Test" }
        mockCampaniaLoading = false
        mockBackingsData = mockBackings
        mockBackingsLoading = false
        mockBackingsError = null
        mockStatsData = mockStats
        mockStatsLoading = false
    })

    it("renders page title", () => {
        render(<CampaignBackingsPage />)

        expect(screen.getByText("Apoyos Recibidos")).toBeInTheDocument()
    })

    it("displays campania subtitle", () => {
        render(<CampaignBackingsPage />)

        expect(screen.getByText("Mi Campania Test")).toBeInTheDocument()
    })

    it("renders stats grid with data", () => {
        render(<CampaignBackingsPage />)

        expect(screen.getByText("Total Recaudado")).toBeInTheDocument()
        expect(screen.getByText("Total Apoyos")).toBeInTheDocument()
        expect(screen.getByText("Promedio por Aporte")).toBeInTheDocument()
    })

    it("renders backings table with data", () => {
        render(<CampaignBackingsPage />)

        expect(screen.getByText("Maria Lopez")).toBeInTheDocument()
        const anonimoElements = screen.getAllByText("Anonimo")
        expect(anonimoElements.length).toBeGreaterThanOrEqual(1)
    })

    it("shows loading skeletons when data is loading", () => {
        mockCampaniaLoading = true
        mockBackingsLoading = true
        mockStatsLoading = true
        mockBackingsData = []

        const { container } = render(<CampaignBackingsPage />)

        const skeletons = container.querySelectorAll('[class*="animate-pulse"]')
        expect(skeletons.length).toBeGreaterThan(0)
    })

    it("shows error state when backings fail to load", () => {
        mockBackingsError = new Error("Network error")
        mockBackingsData = []

        render(<CampaignBackingsPage />)

        expect(
            screen.getByText("Error al cargar los apoyos. Intenta nuevamente.")
        ).toBeInTheDocument()
        expect(screen.getByText("Reintentar")).toBeInTheDocument()
    })

    it("shows empty state when no backings", () => {
        mockBackingsData = []

        render(<CampaignBackingsPage />)

        expect(screen.getByText("No hay apoyos aun")).toBeInTheDocument()
    })

    it("shows not found state when campania is null", () => {
        mockCampania = null

        render(<CampaignBackingsPage />)

        expect(screen.getByText("Campania no encontrada")).toBeInTheDocument()
    })

    it("shows export CSV button when backings exist", () => {
        render(<CampaignBackingsPage />)

        expect(screen.getByText("Exportar CSV")).toBeInTheDocument()
    })

    it("hides export CSV button when no backings", () => {
        mockBackingsData = []

        render(<CampaignBackingsPage />)

        expect(screen.queryByText("Exportar CSV")).not.toBeInTheDocument()
    })
})
