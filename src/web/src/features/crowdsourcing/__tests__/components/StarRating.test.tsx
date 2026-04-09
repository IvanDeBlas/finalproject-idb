import { render, screen, fireEvent } from "@testing-library/react"
import { describe, it, expect, vi } from "vitest"
import { StarRating } from "../../presentation/components/StarRating"

describe("StarRating", () => {
    it("renders 5 star buttons", () => {
        render(<StarRating value={0} onChange={vi.fn()} />)
        const buttons = screen.getAllByRole("radio")
        expect(buttons).toHaveLength(5)
    })

    it("renders all stars as empty when value is 0", () => {
        const { container } = render(
            <StarRating value={0} onChange={vi.fn()} />
        )
        const buttons = container.querySelectorAll("button")
        buttons.forEach((btn) => {
            expect(btn.classList.contains("text-[#334155]")).toBe(true)
        })
    })

    it("renders N filled stars when value is N", () => {
        const { container } = render(
            <StarRating value={3} onChange={vi.fn()} />
        )
        const buttons = Array.from(container.querySelectorAll("button"))
        const filled = buttons.filter((btn) =>
            btn.classList.contains("text-[#f59e0b]")
        )
        expect(filled).toHaveLength(3)
    })

    it("calls onChange with correct value when star is clicked", () => {
        const mockOnChange = vi.fn()
        render(<StarRating value={0} onChange={mockOnChange} />)
        fireEvent.click(screen.getByRole("radio", { name: /3 estrellas/i }))
        expect(mockOnChange).toHaveBeenCalledWith(3)
    })

    it("calls onChange with 1 when first star is clicked", () => {
        const mockOnChange = vi.fn()
        render(<StarRating value={0} onChange={mockOnChange} />)
        fireEvent.click(screen.getByRole("radio", { name: /1 estrella$/i }))
        expect(mockOnChange).toHaveBeenCalledWith(1)
    })

    it("calls onChange with 5 when last star is clicked", () => {
        const mockOnChange = vi.fn()
        render(<StarRating value={0} onChange={mockOnChange} />)
        fireEvent.click(screen.getByRole("radio", { name: /5 estrellas/i }))
        expect(mockOnChange).toHaveBeenCalledWith(5)
    })

    it("shows hover state on mouseenter", () => {
        const { container } = render(
            <StarRating value={0} onChange={vi.fn()} />
        )
        const buttons = Array.from(container.querySelectorAll("button"))
        fireEvent.mouseEnter(buttons[2]) // hover on star 3
        // Stars 1-3 should be highlighted
        expect(buttons[0].classList.contains("text-[#fbbf24]")).toBe(true)
        expect(buttons[1].classList.contains("text-[#fbbf24]")).toBe(true)
        expect(buttons[2].classList.contains("text-[#fbbf24]")).toBe(true)
        expect(buttons[3].classList.contains("text-[#334155]")).toBe(true)
    })

    it("clears hover state on mouseleave returning to selected value", () => {
        const { container } = render(
            <StarRating value={2} onChange={vi.fn()} />
        )
        const wrapper = container.querySelector("[role='radiogroup']")!
        const buttons = Array.from(container.querySelectorAll("button"))
        // Hover on star 4
        fireEvent.mouseEnter(buttons[3])
        expect(buttons[3].classList.contains("text-[#fbbf24]")).toBe(true)
        // Leave the radiogroup
        fireEvent.mouseLeave(wrapper)
        // Should revert to value=2
        expect(buttons[0].classList.contains("text-[#f59e0b]")).toBe(true)
        expect(buttons[1].classList.contains("text-[#f59e0b]")).toBe(true)
        expect(buttons[2].classList.contains("text-[#334155]")).toBe(true)
    })

    it("does not call onChange when disabled", () => {
        const mockOnChange = vi.fn()
        render(<StarRating value={0} onChange={mockOnChange} disabled />)
        fireEvent.click(screen.getByRole("radio", { name: /3 estrellas/i }))
        expect(mockOnChange).not.toHaveBeenCalled()
    })

    it("has role radiogroup with aria-label", () => {
        render(<StarRating value={0} onChange={vi.fn()} />)
        expect(
            screen.getByRole("radiogroup", {
                name: /puntuacion de 1 a 5 estrellas/i,
            })
        ).toBeInTheDocument()
    })

    it("each star button has descriptive aria-label", () => {
        render(<StarRating value={0} onChange={vi.fn()} />)
        expect(
            screen.getByRole("radio", { name: /1 estrella$/i })
        ).toBeInTheDocument()
        expect(
            screen.getByRole("radio", { name: /2 estrellas/i })
        ).toBeInTheDocument()
        expect(
            screen.getByRole("radio", { name: /5 estrellas/i })
        ).toBeInTheDocument()
    })

    it("selected star has aria-pressed true", () => {
        render(<StarRating value={3} onChange={vi.fn()} />)
        expect(
            screen.getByRole("radio", { name: /3 estrellas/i })
        ).toHaveAttribute("aria-pressed", "true")
        expect(
            screen.getByRole("radio", { name: /4 estrellas/i })
        ).toHaveAttribute("aria-pressed", "false")
    })

    it("unselected stars have aria-pressed false", () => {
        render(<StarRating value={2} onChange={vi.fn()} />)
        expect(
            screen.getByRole("radio", { name: /4 estrellas/i })
        ).toHaveAttribute("aria-pressed", "false")
        expect(
            screen.getByRole("radio", { name: /5 estrellas/i })
        ).toHaveAttribute("aria-pressed", "false")
    })

    it("star buttons are focusable (not disabled)", () => {
        render(<StarRating value={0} onChange={vi.fn()} />)
        const buttons = screen.getAllByRole("radio")
        buttons.forEach((btn) => {
            expect(btn).not.toBeDisabled()
        })
    })
})
