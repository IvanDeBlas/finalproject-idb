import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import { StatsCard } from "../stats-card"
import { TrendingUp, Users } from "lucide-react"

describe("StatsCard", () => {
    it("renders title and value", () => {
        render(
            <StatsCard
                title="Total Recaudado"
                value="15.340 EUR"
                icon={TrendingUp}
            />
        )

        expect(screen.getByText("Total Recaudado")).toBeInTheDocument()
        expect(screen.getByText("15.340 EUR")).toBeInTheDocument()
    })

    it("displays icon", () => {
        const { container } = render(
            <StatsCard
                title="Total Recaudado"
                value="15.340 EUR"
                icon={TrendingUp}
            />
        )

        const icon = container.querySelector("svg")
        expect(icon).toBeInTheDocument()
    })

    it("shows positive trend", () => {
        render(
            <StatsCard
                title="Total Recaudado"
                value="15.340 EUR"
                icon={TrendingUp}
                trend={{ value: 12.5, isPositive: true }}
            />
        )

        expect(screen.getByText(/\+12.5%/)).toBeInTheDocument()
    })

    it("shows negative trend", () => {
        render(
            <StatsCard
                title="Backers"
                value="487"
                icon={Users}
                trend={{ value: 5.3, isPositive: false }}
            />
        )

        expect(screen.getByText(/-5.3%/)).toBeInTheDocument()
    })

    it("shows description when provided", () => {
        render(
            <StatsCard
                title="Campanias Activas"
                value="2"
                icon={TrendingUp}
                description="Campanias publicadas"
            />
        )

        expect(
            screen.getByText("Campanias publicadas")
        ).toBeInTheDocument()
    })

    it("renders without trend", () => {
        render(
            <StatsCard
                title="Total Recaudado"
                value="15.340 EUR"
                icon={TrendingUp}
            />
        )

        expect(screen.queryByText(/%/)).not.toBeInTheDocument()
    })

    it("applies custom className", () => {
        const { container } = render(
            <StatsCard
                title="Test"
                value="123"
                icon={TrendingUp}
                className="custom-class"
            />
        )

        expect(container.firstChild).toHaveClass("custom-class")
    })
})
