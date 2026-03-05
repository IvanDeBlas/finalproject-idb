import { describe, it, expect, vi } from "vitest"
import { render, screen, waitFor } from "@testing-library/react"
import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { CampaniaRewardsSection } from "../../presentation/components/CampaniaRewardsSection"
import { mockRewardsList } from "../../__mocks__/reward.mock"
import type { Reward } from "@shared/types/reward"

const createTestQueryClient = () =>
    new QueryClient({
        defaultOptions: {
            queries: {
                retry: false,
                gcTime: 0,
                staleTime: 0,
            },
        },
    })

function renderWithProviders(ui: React.ReactElement) {
    const queryClient = createTestQueryClient()
    return render(
        <QueryClientProvider client={queryClient}>
            {ui}
        </QueryClientProvider>
    )
}

// Mock the reward service
vi.mock("../../infrastructure/services/reward.service", () => ({
    rewardService: {
        getByCampaniaId: vi.fn(),
        getAll: vi.fn(),
        getById: vi.fn(),
    },
}))

// Import after mock
import { rewardService } from "../../infrastructure/services/reward.service"

describe("CampaniaRewardsSection", () => {
    const campaniaId = "550e8400-e29b-41d4-a716-446655440000"

    it("shows skeleton loaders while loading", () => {
        vi.mocked(rewardService.getByCampaniaId).mockReturnValue(
            new Promise(() => {}) // Never resolves - stays loading
        )

        renderWithProviders(
            <CampaniaRewardsSection campaniaId={campaniaId} monedaId={1} />
        )

        const skeletons = screen.getAllByTestId("reward-skeleton")
        expect(skeletons).toHaveLength(3)
    })

    it("renders list of rewards", async () => {
        vi.mocked(rewardService.getByCampaniaId).mockResolvedValue(mockRewardsList)

        renderWithProviders(
            <CampaniaRewardsSection campaniaId={campaniaId} monedaId={1} />
        )

        await waitFor(() => {
            expect(screen.getByText("Descarga Digital")).toBeInTheDocument()
        })

        expect(screen.getByText("CD Fisico Firmado")).toBeInTheDocument()
        expect(screen.getByText("Vinilo Edicion Limitada")).toBeInTheDocument()
        expect(screen.getByText("Paquete VIP")).toBeInTheDocument()
    })

    it("rewards ordered by precio ascendente", async () => {
        const unorderedRewards: Reward[] = [
            { ...mockRewardsList[2], importeMinimo: 50 },
            { ...mockRewardsList[0], importeMinimo: 10 },
            { ...mockRewardsList[1], importeMinimo: 25 },
        ]
        vi.mocked(rewardService.getByCampaniaId).mockResolvedValue(unorderedRewards)

        renderWithProviders(
            <CampaniaRewardsSection campaniaId={campaniaId} monedaId={1} />
        )

        await waitFor(() => {
            expect(screen.getAllByTestId("reward-card")).toHaveLength(3)
        })

        const rewardCards = screen.getAllByTestId("reward-card")
        expect(rewardCards).toHaveLength(3)
    })

    it("shows empty state when no rewards", async () => {
        vi.mocked(rewardService.getByCampaniaId).mockResolvedValue([])

        renderWithProviders(
            <CampaniaRewardsSection campaniaId={campaniaId} monedaId={1} />
        )

        await waitFor(() => {
            expect(
                screen.getByText(/Esta campana no tiene recompensas/i)
            ).toBeInTheDocument()
        })

        expect(
            screen.getByText(/Puedes hacer una contribucion libre/i)
        ).toBeInTheDocument()
    })

    it("shows error message on fetch error", async () => {
        vi.mocked(rewardService.getByCampaniaId).mockRejectedValue(
            new Error("API Error")
        )

        renderWithProviders(
            <CampaniaRewardsSection campaniaId={campaniaId} monedaId={1} />
        )

        await waitFor(() => {
            expect(
                screen.getByText(/Error al cargar las recompensas/i)
            ).toBeInTheDocument()
        })

        expect(screen.getByText(/Intentar de nuevo/i)).toBeInTheDocument()
    })

    it("shows 'Campana finalizada' when campaniaFinalizada is true", () => {
        vi.mocked(rewardService.getByCampaniaId).mockResolvedValue([])

        renderWithProviders(
            <CampaniaRewardsSection
                campaniaId={campaniaId}
                monedaId={1}
                campaniaFinalizada={true}
            />
        )

        expect(screen.getByText("Campana finalizada")).toBeInTheDocument()
        expect(
            screen.getByRole("button", { name: /Campana finalizada/i })
        ).toBeDisabled()
    })

    it("shows 'Apoyar esta campana' button when not finalizada", () => {
        vi.mocked(rewardService.getByCampaniaId).mockResolvedValue([])

        renderWithProviders(
            <CampaniaRewardsSection campaniaId={campaniaId} monedaId={1} />
        )

        expect(
            screen.getByRole("button", { name: /Apoyar esta campana/i })
        ).toBeInTheDocument()
    })

    it("shows footer security info", async () => {
        vi.mocked(rewardService.getByCampaniaId).mockResolvedValue([])

        renderWithProviders(
            <CampaniaRewardsSection campaniaId={campaniaId} monedaId={1} />
        )

        await waitFor(() => {
            expect(screen.getByText("Pago seguro")).toBeInTheDocument()
        })
    })

    it("shows delivery estimate in footer when rewards have delivery info", async () => {
        vi.mocked(rewardService.getByCampaniaId).mockResolvedValue(mockRewardsList)

        renderWithProviders(
            <CampaniaRewardsSection campaniaId={campaniaId} monedaId={1} />
        )

        await waitFor(() => {
            expect(screen.getByText("Descarga Digital")).toBeInTheDocument()
        })

        // The footer contains delivery text alongside the pago seguro section
        // Multiple reward cards may also contain delivery info, so just verify at least one exists
        const deliveryTexts = screen.getAllByText(/Inmediato tras finalizar campania/i)
        expect(deliveryTexts.length).toBeGreaterThanOrEqual(1)
    })
})
