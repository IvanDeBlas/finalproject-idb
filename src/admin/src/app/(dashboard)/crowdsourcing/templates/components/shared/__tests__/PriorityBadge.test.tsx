import { describe, it, expect } from "vitest"
import { render, screen } from "@/test-utils"
import { PriorityBadge } from "../PriorityBadge"

describe("PriorityBadge", () => {
    it("renders Alta prioridad", () => {
        render(<PriorityBadge prioridad="Alta" />)

        expect(screen.getByText("Alta")).toBeInTheDocument()
    })

    it("renders Media prioridad", () => {
        render(<PriorityBadge prioridad="Media" />)

        expect(screen.getByText("Media")).toBeInTheDocument()
    })

    it("renders Baja prioridad", () => {
        render(<PriorityBadge prioridad="Baja" />)

        expect(screen.getByText("Baja")).toBeInTheDocument()
    })

    it("applies red color for Alta", () => {
        const { container } = render(<PriorityBadge prioridad="Alta" />)
        const badge = container.querySelector("[class*='red']")

        expect(badge).toBeInTheDocument()
    })

    it("applies yellow color for Media", () => {
        const { container } = render(<PriorityBadge prioridad="Media" />)
        const badge = container.querySelector("[class*='yellow']")

        expect(badge).toBeInTheDocument()
    })

    it("applies blue color for Baja", () => {
        const { container } = render(<PriorityBadge prioridad="Baja" />)
        const badge = container.querySelector("[class*='blue']")

        expect(badge).toBeInTheDocument()
    })
})
