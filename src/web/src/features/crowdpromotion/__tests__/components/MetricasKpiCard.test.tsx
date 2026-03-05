import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { MousePointerClick } from "lucide-react"
import { MetricasKpiCard } from "../../metricas/presentation/components/MetricasKpiCard"

describe("MetricasKpiCard", () => {
    it("renders the label correctly", () => {
        render(
            <MetricasKpiCard
                label="Mis clicks"
                value={450}
                icon={MousePointerClick}
                iconColor="#3b82f6"
                iconBgClass="bg-blue-950/50"
            />
        )
        expect(screen.getByText("Mis clicks")).toBeInTheDocument()
    })

    it("renders a numeric value", () => {
        render(
            <MetricasKpiCard
                label="Mis clicks"
                value={450}
                icon={MousePointerClick}
                iconColor="#3b82f6"
                iconBgClass="bg-blue-950/50"
            />
        )
        expect(screen.getByText("450")).toBeInTheDocument()
    })

    it("renders monetary value with currency suffix", () => {
        render(
            <MetricasKpiCard
                label="Comision"
                value={50}
                icon={MousePointerClick}
                iconColor="#10b981"
                iconBgClass="bg-green-950/50"
                monetario
                monedaNombre="EUR"
            />
        )
        expect(screen.getByText("50")).toBeInTheDocument()
        expect(screen.getByText("EUR")).toBeInTheDocument()
    })

    it("applies the icon background class", () => {
        const { container } = render(
            <MetricasKpiCard
                label="Mis clicks"
                value={450}
                icon={MousePointerClick}
                iconColor="#3b82f6"
                iconBgClass="bg-blue-950/50"
            />
        )
        const iconContainer = container.querySelector("[class*='bg-blue-950']")
        expect(iconContainer).not.toBeNull()
    })

    it("defaults to EUR when monedaNombre is null for monetario", () => {
        render(
            <MetricasKpiCard
                label="Comision"
                value={50}
                icon={MousePointerClick}
                iconColor="#10b981"
                iconBgClass="bg-green-950/50"
                monetario
                monedaNombre={null}
            />
        )
        expect(screen.getByText("EUR")).toBeInTheDocument()
    })

    it("renders with value zero without errors", () => {
        render(
            <MetricasKpiCard
                label="Mis clicks"
                value={0}
                icon={MousePointerClick}
                iconColor="#3b82f6"
                iconBgClass="bg-blue-950/50"
            />
        )
        expect(screen.getByText("0")).toBeInTheDocument()
    })
})
