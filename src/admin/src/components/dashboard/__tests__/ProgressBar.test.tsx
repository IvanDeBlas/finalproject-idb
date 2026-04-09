import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import { ProgressBar } from "../ProgressBar"

describe("ProgressBar", () => {
    it("renders with amount labels by default", () => {
        render(<ProgressBar current={2340} goal={5000} />)

        // formatCurrency uses es-ES locale; separator may vary by env
        expect(screen.getByText(/2.?340/)).toBeInTheDocument()
        expect(screen.getByText(/5.?000/)).toBeInTheDocument()
    })

    it("renders progress bar element", () => {
        render(<ProgressBar current={2500} goal={5000} />)

        const progressBar = screen.getByRole("progressbar")
        expect(progressBar).toBeInTheDocument()
    })

    it("shows percentage when showPercentage is true", () => {
        render(
            <ProgressBar current={2340} goal={5000} showPercentage={true} />
        )

        expect(screen.getByText("46.80%")).toBeInTheDocument()
    })

    it("hides percentage when showPercentage is false", () => {
        render(
            <ProgressBar
                current={2340}
                goal={5000}
                showPercentage={false}
            />
        )

        expect(screen.queryByText(/46/)).not.toBeInTheDocument()
    })

    it("shows 'Meta alcanzada' badge when >= 100%", () => {
        render(
            <ProgressBar current={5500} goal={5000} showPercentage={true} />
        )

        expect(screen.getByText("Meta alcanzada")).toBeInTheDocument()
    })

    it("does not show meta alcanzada when < 100%", () => {
        render(<ProgressBar current={2340} goal={5000} />)

        expect(screen.queryByText("Meta alcanzada")).not.toBeInTheDocument()
    })

    it("handles 0% progress", () => {
        render(
            <ProgressBar current={0} goal={5000} showPercentage={true} />
        )

        expect(screen.getByText("0.00%")).toBeInTheDocument()
    })

    it("handles goal of 0 without crashing", () => {
        render(
            <ProgressBar current={0} goal={0} showPercentage={true} />
        )

        expect(screen.getByText("0.00%")).toBeInTheDocument()
    })

    it("hides amounts when showAmount is false", () => {
        render(
            <ProgressBar
                current={2340}
                goal={5000}
                showAmount={false}
                showPercentage={true}
            />
        )

        expect(screen.queryByText(/de/)).not.toBeInTheDocument()
    })
})
