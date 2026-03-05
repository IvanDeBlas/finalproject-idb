import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { RewardCard } from "../RewardCard"
import type { Reward } from "@shared/types"

// Mock @dnd-kit/sortable
vi.mock("@dnd-kit/sortable", () => ({
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
    descripcion: "Acceso anticipado al album completo en formato digital FLAC + MP3",
    importeMinimo: 10,
    monedaId: 1,
    esAddOn: false,
    incluyeEnvioFisico: false,
    orden: 1,
    esActivo: true,
    fechaCreacion: "2026-01-01T00:00:00Z",
}

function buildReward(overrides: Partial<Reward> = {}): Reward {
    return { ...baseReward, ...overrides }
}

describe("RewardCard", () => {
    const defaultHandlers = {
        onEdit: vi.fn(),
        onDelete: vi.fn(),
    }

    beforeEach(() => {
        vi.clearAllMocks()
    })

    it("renders precio and titulo", () => {
        render(<RewardCard reward={baseReward} {...defaultHandlers} />)

        expect(screen.getByText(/10\.00/)).toBeInTheDocument()
        expect(screen.getByText("Descarga Digital")).toBeInTheDocument()
    })

    it("renders descripcion", () => {
        render(<RewardCard reward={baseReward} {...defaultHandlers} />)

        expect(screen.getByText(/Acceso anticipado/)).toBeInTheDocument()
    })

    it("shows unlimited stock indicator when no cantidadMaxima", () => {
        const reward = buildReward({ cantidadMaxima: undefined })

        render(<RewardCard reward={reward} {...defaultHandlers} />)

        expect(screen.getByText("Ilimitadas disponibles")).toBeInTheDocument()
    })

    it("shows limited stock indicator when has cantidadMaxima", () => {
        const reward = buildReward({ cantidadMaxima: 200 })

        render(<RewardCard reward={reward} {...defaultHandlers} />)

        expect(screen.getByText(/200 disponibles/)).toBeInTheDocument()
    })

    it("renders Edit and Delete buttons", () => {
        render(<RewardCard reward={baseReward} {...defaultHandlers} />)

        expect(screen.getByText("Editar")).toBeInTheDocument()
    })

    it("calls onEdit when Edit clicked", async () => {
        const onEdit = vi.fn()
        const user = userEvent.setup()

        render(
            <RewardCard
                reward={baseReward}
                onEdit={onEdit}
                onDelete={vi.fn()}
            />
        )

        await user.click(screen.getByText("Editar"))

        expect(onEdit).toHaveBeenCalledTimes(1)
    })

    it("calls onDelete when Delete button clicked", async () => {
        const onDelete = vi.fn()
        const user = userEvent.setup()

        render(
            <RewardCard
                reward={baseReward}
                onEdit={vi.fn()}
                onDelete={onDelete}
            />
        )

        // The delete button is the one with red text class (text-red-400)
        const allButtons = screen.getAllByRole("button")
        const deleteBtn = allButtons.find((btn) =>
            btn.className.includes("text-red")
        )
        expect(deleteBtn).toBeDefined()
        await user.click(deleteBtn!)

        expect(onDelete).toHaveBeenCalledTimes(1)
    })

    it("shows backers count", () => {
        render(<RewardCard reward={baseReward} {...defaultHandlers} />)

        expect(screen.getByText(/0 backers/)).toBeInTheDocument()
    })

    it("hides descripcion when not provided", () => {
        const reward = buildReward({ descripcion: undefined })

        render(<RewardCard reward={reward} {...defaultHandlers} />)

        expect(
            screen.queryByText(/Acceso anticipado/)
        ).not.toBeInTheDocument()
    })
})
