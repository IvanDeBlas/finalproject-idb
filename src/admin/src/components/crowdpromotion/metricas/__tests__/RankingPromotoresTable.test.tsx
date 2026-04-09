import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import userEvent from "@testing-library/user-event"
import { RankingPromotoresTable } from "../RankingPromotoresTable"
import { mockRankingItems } from "@/__mocks__/cp-tracking-metricas.mock"

describe("RankingPromotoresTable", () => {
    it("renders promotor names", () => {
        render(<RankingPromotoresTable items={mockRankingItems} />)
        expect(screen.getByText("DJ Mark")).toBeInTheDocument()
        expect(screen.getByText("MusicBlog.es")).toBeInTheDocument()
        expect(screen.getByText("FanLuna")).toBeInTheDocument()
    })

    it("renders position numbers", () => {
        render(<RankingPromotoresTable items={mockRankingItems} />)
        expect(screen.getByText("#1")).toBeInTheDocument()
        expect(screen.getByText("#2")).toBeInTheDocument()
        expect(screen.getByText("#3")).toBeInTheDocument()
    })

    it("renders tipo promotor badge", () => {
        render(<RankingPromotoresTable items={mockRankingItems} />)
        expect(screen.getByText("Influencer")).toBeInTheDocument()
    })

    it("renders fallback for null tipoPromotorNombre", () => {
        render(<RankingPromotoresTable items={mockRankingItems} />)
        // FanLuna has null tipoPromotorNombre, so no badge for it
        const badges = screen.getAllByText(/Influencer|Medio \/ Blog/)
        expect(badges.length).toBe(2)
    })

    it("renders clicks value for each row", () => {
        render(<RankingPromotoresTable items={mockRankingItems} />)
        expect(screen.getByText("450")).toBeInTheDocument()
    })

    it("renders conversiones value", () => {
        render(<RankingPromotoresTable items={mockRankingItems} />)
        expect(screen.getByText("5")).toBeInTheDocument()
    })

    it("renders valorGenerado", () => {
        render(<RankingPromotoresTable items={mockRankingItems} />)
        expect(screen.getByText("500")).toBeInTheDocument()
    })

    it("renders comisionAcumulada", () => {
        render(<RankingPromotoresTable items={mockRankingItems} />)
        expect(screen.getByText("50")).toBeInTheDocument()
    })

    it("renders Avatar fallback initials", () => {
        render(<RankingPromotoresTable items={mockRankingItems} />)
        expect(screen.getByText("D")).toBeInTheDocument() // DJ Mark -> D
        expect(screen.getByText("M")).toBeInTheDocument() // MusicBlog.es -> M
        expect(screen.getByText("F")).toBeInTheDocument() // FanLuna -> F
    })

    it("renders sortable column headers", () => {
        render(<RankingPromotoresTable items={mockRankingItems} />)
        const clicksHeader = screen.getByText("Clicks").closest("th")
        expect(clicksHeader).toHaveAttribute("aria-sort")

        const convHeader = screen.getByText("Conv.").closest("th")
        expect(convHeader).toHaveAttribute("aria-sort")
    })

    it("sorts by clicks DESC on header click", async () => {
        render(<RankingPromotoresTable items={mockRankingItems} />)
        const clicksHeader = screen.getByText("Clicks").closest("th")!

        await userEvent.click(clicksHeader)

        const rows = screen.getAllByText(/#\d+/)
        // After clicking Clicks header, DJ Mark (450 clicks) should be first
        const firstRow = rows[0].closest("tr")
        expect(firstRow?.textContent).toContain("DJ Mark")
    })

    it("toggles sort direction on second click of same column", async () => {
        render(<RankingPromotoresTable items={mockRankingItems} />)
        const clicksHeader = screen.getByText("Clicks").closest("th")!

        // First click: DESC by clicks
        await userEvent.click(clicksHeader)
        // Second click: ASC by clicks
        await userEvent.click(clicksHeader)

        const rows = screen.getAllByText(/#\d+/)
        // After ASC sort, FanLuna (300 clicks) should be first
        const firstRow = rows[0].closest("tr")
        expect(firstRow?.textContent).toContain("FanLuna")
    })

    it("sorts by conversiones DESC by default", () => {
        render(<RankingPromotoresTable items={mockRankingItems} />)
        const rows = screen.getAllByText(/#\d+/)
        // Default: sorted by conversiones desc. DJ Mark (5) first
        const firstRow = rows[0].closest("tr")
        expect(firstRow?.textContent).toContain("DJ Mark")
    })

    it("renders empty state when items is empty", () => {
        render(<RankingPromotoresTable items={[]} />)
        expect(
            screen.getByText("Sin promotores con actividad en este periodo")
        ).toBeInTheDocument()
    })

    it("empty state occupies full row width", () => {
        render(<RankingPromotoresTable items={[]} />)
        const cell = screen
            .getByText("Sin promotores con actividad en este periodo")
            .closest("td")
        expect(cell).toHaveAttribute("colspan", "6")
    })
})
