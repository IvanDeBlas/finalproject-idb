import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import RewardsPage from "../../page"
import type { Reward } from "@shared/types"

// Mock Next.js navigation
const mockPush = vi.fn()
vi.mock("next/navigation", () => ({
    useParams: () => ({ id: "campania-1" }),
    useRouter: () => ({ push: mockPush }),
}))

// Mock sonner
vi.mock("sonner", () => ({
    toast: {
        success: vi.fn(),
        error: vi.fn(),
    },
}))

// Mock @dnd-kit
vi.mock("@dnd-kit/core", () => ({
    DndContext: ({ children }: { children: React.ReactNode }) => <>{children}</>,
    closestCenter: vi.fn(),
    KeyboardSensor: vi.fn(),
    PointerSensor: vi.fn(),
    useSensor: vi.fn(),
    useSensors: () => [],
}))

vi.mock("@dnd-kit/sortable", () => ({
    SortableContext: ({ children }: { children: React.ReactNode }) => <>{children}</>,
    verticalListSortingStrategy: {},
    arrayMove: vi.fn(),
    sortableKeyboardCoordinates: vi.fn(),
    useSortable: () => ({
        attributes: {},
        listeners: {},
        setNodeRef: vi.fn(),
        transform: null,
        transition: undefined,
        isDragging: false,
    }),
}))

vi.mock("@dnd-kit/utilities", () => ({
    CSS: {
        Transform: {
            toString: () => undefined,
        },
    },
}))

const baseReward: Reward = {
    id: "reward-1",
    campaniaId: "campania-1",
    tipoRewardId: 1,
    nombre: "Descarga Digital",
    descripcion: "Acceso anticipado al album",
    importeMinimo: 10,
    monedaId: 1,
    esAddOn: false,
    incluyeEnvioFisico: false,
    orden: 1,
    esActivo: true,
    fechaCreacion: "2026-01-01T00:00:00Z",
}

const baseReward2: Reward = {
    ...baseReward,
    id: "reward-2",
    nombre: "CD Fisico",
    importeMinimo: 25,
    orden: 2,
}

// Rewards hook mock state
let mockRewards: Reward[] = []
let mockIsLoading = false
const mockReorder = vi.fn()

vi.mock("@/hooks/use-rewards", () => ({
    useRewards: () => ({
        data: mockRewards,
        isLoading: mockIsLoading,
    }),
    useReorderRewards: () => ({
        mutate: mockReorder,
    }),
    useCreateReward: () => ({
        mutate: vi.fn(),
        isPending: false,
    }),
    useUpdateReward: () => ({
        mutate: vi.fn(),
        isPending: false,
    }),
    useDeleteReward: () => ({
        mutate: vi.fn(),
        isPending: false,
    }),
}))

describe("RewardsPage", () => {
    beforeEach(() => {
        vi.clearAllMocks()
        mockRewards = []
        mockIsLoading = false
    })

    it("shows loading skeletons when loading", () => {
        mockIsLoading = true

        render(<RewardsPage />)

        const skeletons = document.querySelectorAll(".animate-pulse")
        expect(skeletons.length).toBeGreaterThan(0)
    })

    it("shows empty state when no rewards", () => {
        mockRewards = []

        render(<RewardsPage />)

        expect(
            screen.getByText("No has creado recompensas aun")
        ).toBeInTheDocument()
    })

    it("renders rewards list when rewards exist", () => {
        mockRewards = [baseReward, baseReward2]

        render(<RewardsPage />)

        expect(screen.getByText("Descarga Digital")).toBeInTheDocument()
        expect(screen.getByText("CD Fisico")).toBeInTheDocument()
    })

    it("renders stats card when rewards exist", () => {
        mockRewards = [baseReward, baseReward2]

        render(<RewardsPage />)

        expect(screen.getByText("Total:")).toBeInTheDocument()
        expect(screen.getByText("Activas:")).toBeInTheDocument()
    })

    it("shows Agregar recompensa button when rewards exist", () => {
        mockRewards = [baseReward]

        render(<RewardsPage />)

        expect(
            screen.getByText("Agregar recompensa")
        ).toBeInTheDocument()
    })

    it("opens create modal when Agregar recompensa clicked", async () => {
        const user = userEvent.setup()
        mockRewards = [baseReward]

        render(<RewardsPage />)

        await user.click(screen.getByText("Agregar recompensa"))

        expect(screen.getByText("Nueva Recompensa")).toBeInTheDocument()
    })

    it("shows tip box when rewards exist", () => {
        mockRewards = [baseReward]

        render(<RewardsPage />)

        expect(
            screen.getByText(/Ordena tus recompensas/)
        ).toBeInTheDocument()
    })

    it("navigates back when Volver button clicked", async () => {
        const user = userEvent.setup()
        mockRewards = [baseReward]

        render(<RewardsPage />)

        await user.click(screen.getByText("Volver a Campania"))

        expect(mockPush).toHaveBeenCalledWith(
            "/dashboard/campanias/campania-1"
        )
    })

    it("renders page header", () => {
        render(<RewardsPage />)

        expect(screen.getByText("Recompensas")).toBeInTheDocument()
        expect(
            screen.getByText("Gestiona las recompensas de tu campania")
        ).toBeInTheDocument()
    })

    it("opens create modal from empty state action", async () => {
        const user = userEvent.setup()
        mockRewards = []

        render(<RewardsPage />)

        await user.click(screen.getByText("Crear primera recompensa"))

        expect(screen.getByText("Nueva Recompensa")).toBeInTheDocument()
    })
})
