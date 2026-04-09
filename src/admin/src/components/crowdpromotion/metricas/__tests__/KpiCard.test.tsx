import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import { KpiCard } from "../KpiCard"
import { MousePointerClick } from "lucide-react"

const defaultProps = {
    label: "Total Clicks",
    value: 1250 as string | number,
    icon: MousePointerClick,
    iconBgClass: "bg-blue-950/50",
    iconColorClass: "text-[#3b82f6]",
}

describe("KpiCard", () => {
    it("renders label text", () => {
        render(<KpiCard {...defaultProps} />)
        expect(screen.getByText("Total Clicks")).toBeInTheDocument()
    })

    it("renders numeric value formatted with toLocaleString", () => {
        render(<KpiCard {...defaultProps} />)
        // toLocaleString may or may not add thousands separator depending on env
        expect(screen.getByText(/1[.,]?250/)).toBeInTheDocument()
    })

    it("renders string value as-is", () => {
        render(<KpiCard {...defaultProps} value="0.96%" />)
        expect(screen.getByText("0.96%")).toBeInTheDocument()
    })

    it("renders suffix when provided", () => {
        render(<KpiCard {...defaultProps} suffix="EUR" />)
        expect(screen.getByText("EUR")).toBeInTheDocument()
    })

    it("renders note when provided", () => {
        render(<KpiCard {...defaultProps} note="conversiones / clicks" />)
        expect(screen.getByText("conversiones / clicks")).toBeInTheDocument()
    })

    it("applies valueColorClass to value element", () => {
        render(<KpiCard {...defaultProps} valueColorClass="text-[#f59e0b]" />)
        const valueEl = screen.getByText(/1[.,]?250/)
        expect(valueEl.className).toContain("text-[#f59e0b]")
    })

    it("does not render suffix when not provided", () => {
        render(<KpiCard {...defaultProps} />)
        expect(screen.queryByText("EUR")).not.toBeInTheDocument()
    })

    it("does not render note when not provided", () => {
        render(<KpiCard {...defaultProps} />)
        expect(screen.queryByText("conversiones / clicks")).not.toBeInTheDocument()
    })
})
