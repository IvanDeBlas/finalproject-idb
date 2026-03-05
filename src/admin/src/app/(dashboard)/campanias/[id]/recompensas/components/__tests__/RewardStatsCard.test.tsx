import { render, screen } from "@/test-utils"
import { RewardStatsCard } from "../RewardStatsCard"
import type { Reward } from "@shared/types"

const baseReward: Reward = {
    id: "reward-1",
    campaniaId: "campania-1",
    tipoRewardId: 1,
    nombre: "Descarga Digital",
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

describe("RewardStatsCard", () => {
    it("renders total count", () => {
        const rewards = [
            buildReward({ id: "1" }),
            buildReward({ id: "2" }),
            buildReward({ id: "3" }),
        ]

        render(<RewardStatsCard rewards={rewards} />)

        const totalSection = screen.getByText("Total:").closest("div")
        expect(totalSection).toHaveTextContent("3")
    })

    it("renders active count", () => {
        const rewards = [
            buildReward({ id: "1", esActivo: true }),
            buildReward({ id: "2", esActivo: false }),
            buildReward({ id: "3", esActivo: true }),
        ]

        render(<RewardStatsCard rewards={rewards} />)

        expect(screen.getByText("Activas:")).toBeInTheDocument()
        const activasSection = screen.getByText("Activas:").closest("div")
        expect(activasSection).toHaveTextContent("2")
    })

    it("shows Ilimitado when no limited stock rewards", () => {
        const rewards = [
            buildReward({ id: "1", cantidadMaxima: undefined }),
        ]

        render(<RewardStatsCard rewards={rewards} />)

        expect(screen.getByText("Ilimitado")).toBeInTheDocument()
    })

    it("renders empty state with zero rewards", () => {
        render(<RewardStatsCard rewards={[]} />)

        const totalSection = screen.getByText("Total:").closest("div")
        expect(totalSection).toHaveTextContent("0")
    })

    it("shows stock total for limited rewards", () => {
        const rewards = [
            buildReward({ id: "1", cantidadMaxima: 200 }),
            buildReward({ id: "2", cantidadMaxima: 100 }),
            buildReward({ id: "3", cantidadMaxima: undefined }),
        ]

        render(<RewardStatsCard rewards={rewards} />)

        expect(screen.getByText("Stock disponible:")).toBeInTheDocument()
        expect(screen.getByText(/300 unidades/)).toBeInTheDocument()
    })

    it("shows correct stats with mixed active/inactive rewards", () => {
        const rewards = [
            buildReward({ id: "1", esActivo: true, cantidadMaxima: 50 }),
            buildReward({ id: "2", esActivo: true, cantidadMaxima: undefined }),
            buildReward({ id: "3", esActivo: false, cantidadMaxima: 150 }),
        ]

        render(<RewardStatsCard rewards={rewards} />)

        const totalSection = screen.getByText("Total:").closest("div")
        expect(totalSection).toHaveTextContent("3")

        const activasSection = screen.getByText("Activas:").closest("div")
        expect(activasSection).toHaveTextContent("2")

        expect(screen.getByText(/200 unidades/)).toBeInTheDocument()
    })
})
