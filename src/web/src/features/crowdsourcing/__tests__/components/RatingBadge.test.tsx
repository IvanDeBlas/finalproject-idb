import { render, screen } from "@testing-library/react"
import { describe, it, expect } from "vitest"
import { RatingBadge } from "../../presentation/components/RatingBadge"

describe("RatingBadge", () => {
    it("renders puntuacionMedia value", () => {
        render(<RatingBadge puntuacionMedia={4.5} totalValoraciones={12} />)
        expect(screen.getByText("4.5")).toBeInTheDocument()
    })

    it("renders totalValoraciones in parentheses", () => {
        render(<RatingBadge puntuacionMedia={4.5} totalValoraciones={12} />)
        expect(screen.getByText("(12)")).toBeInTheDocument()
    })

    it("renders puntuacion with one decimal (4.0 not 4)", () => {
        render(<RatingBadge puntuacionMedia={4.0} totalValoraciones={5} />)
        expect(screen.getByText("4.0")).toBeInTheDocument()
    })

    it("does not render when totalValoraciones is 0", () => {
        const { container } = render(
            <RatingBadge puntuacionMedia={null} totalValoraciones={0} />
        )
        expect(container.firstChild).toBeNull()
    })

    it("renders compact variant for cards", () => {
        const { container } = render(
            <RatingBadge
                puntuacionMedia={4.5}
                totalValoraciones={12}
                variant="compact"
            />
        )
        // Compact shows star icon + number + count in parentheses
        expect(screen.getByText("4.5")).toBeInTheDocument()
        expect(screen.getByText("(12)")).toBeInTheDocument()
        // Should NOT have StarDisplay (which renders role="img")
        expect(screen.queryByRole("img")).not.toBeInTheDocument()
        // Should have an SVG star icon
        expect(container.querySelector("svg")).toBeInTheDocument()
    })

    it("renders medium variant for profile header", () => {
        render(
            <RatingBadge
                puntuacionMedia={4.5}
                totalValoraciones={12}
                variant="medium"
            />
        )
        expect(screen.getByText("4.5")).toBeInTheDocument()
        expect(screen.getByText("(12 valoraciones)")).toBeInTheDocument()
    })

    it("compact variant renders single star icon", () => {
        const { container } = render(
            <RatingBadge
                puntuacionMedia={4.5}
                totalValoraciones={12}
                variant="compact"
            />
        )
        const svgs = container.querySelectorAll("svg")
        expect(svgs).toHaveLength(1)
    })

    it("medium variant renders StarDisplay", () => {
        render(
            <RatingBadge
                puntuacionMedia={4.5}
                totalValoraciones={12}
                variant="medium"
            />
        )
        // StarDisplay uses role="img"
        expect(screen.getByRole("img")).toBeInTheDocument()
    })
})
