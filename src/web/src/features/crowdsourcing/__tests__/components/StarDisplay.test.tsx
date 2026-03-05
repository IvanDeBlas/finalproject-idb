import { render, screen } from "@testing-library/react"
import { describe, it, expect } from "vitest"
import { StarDisplay } from "../../presentation/components/StarDisplay"

describe("StarDisplay", () => {
    it("renders 5 star icons", () => {
        const { container } = render(<StarDisplay value={3} />)
        const svgs = container.querySelectorAll("svg")
        expect(svgs).toHaveLength(5)
    })

    it("renders filled stars according to value", () => {
        const { container } = render(<StarDisplay value={3} />)
        const svgs = container.querySelectorAll("svg")
        const filledStars = Array.from(svgs).filter((svg) =>
            svg.classList.contains("text-[#f59e0b]")
        )
        expect(filledStars).toHaveLength(3)
    })

    it("renders fractional star for decimal value (4.5)", () => {
        const { container } = render(<StarDisplay value={4.5} />)
        const svgs = container.querySelectorAll("svg")
        // Stars 1-4 should be full (text-[#f59e0b]), star 5 should be half (opacity-50)
        const halfStar = Array.from(svgs).filter(
            (svg) =>
                svg.classList.contains("text-[#f59e0b]") &&
                svg.classList.contains("opacity-50")
        )
        expect(halfStar).toHaveLength(1)
    })

    it("renders numeric value when showNumeric is true", () => {
        render(<StarDisplay value={4.5} showNumeric />)
        expect(screen.getByText("4.5 / 5")).toBeInTheDocument()
    })

    it("does not render numeric value when showNumeric is false", () => {
        render(<StarDisplay value={4.5} showNumeric={false} />)
        expect(screen.queryByText("4.5 / 5")).not.toBeInTheDocument()
    })

    it("applies sm size classes", () => {
        const { container } = render(<StarDisplay value={3} size="sm" />)
        const svg = container.querySelector("svg")
        expect(svg?.classList.contains("w-3.5")).toBe(true)
    })

    it("applies md size classes", () => {
        const { container } = render(<StarDisplay value={3} size="md" />)
        const svg = container.querySelector("svg")
        expect(svg?.classList.contains("w-4")).toBe(true)
    })

    it("applies lg size classes", () => {
        const { container } = render(<StarDisplay value={3} size="lg" />)
        const svg = container.querySelector("svg")
        expect(svg?.classList.contains("w-5")).toBe(true)
    })

    it("has role img with descriptive aria-label", () => {
        render(<StarDisplay value={4.5} />)
        expect(
            screen.getByRole("img", {
                name: "Puntuacion: 4.5 de 5 estrellas",
            })
        ).toBeInTheDocument()
    })

    it("star icons are aria-hidden", () => {
        const { container } = render(<StarDisplay value={3} />)
        const svgs = container.querySelectorAll("svg")
        svgs.forEach((svg) => {
            expect(svg).toHaveAttribute("aria-hidden", "true")
        })
    })

    it("aria-label includes the numeric value", () => {
        render(<StarDisplay value={3} />)
        const img = screen.getByRole("img")
        expect(img).toHaveAttribute(
            "aria-label",
            "Puntuacion: 3 de 5 estrellas"
        )
    })
})
