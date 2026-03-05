import { describe, it, expect, vi } from "vitest"
import { render, screen } from "@testing-library/react"
import userEvent from "@testing-library/user-event"
import { AmountInput } from "../../presentation/components/AmountInput"

describe("AmountInput", () => {
    it("renders with initial value", () => {
        render(<AmountInput value={25} onChange={vi.fn()} />)

        const input = screen.getByRole("spinbutton")
        expect(input).toHaveValue(25)
    })

    it("shows currency symbol (€)", () => {
        render(<AmountInput value={10} onChange={vi.fn()} />)

        expect(screen.getByText("€")).toBeInTheDocument()
    })

    it("shows label 'Monto a Aportar'", () => {
        render(<AmountInput value={10} onChange={vi.fn()} />)

        expect(screen.getByLabelText("Monto a Aportar")).toBeInTheDocument()
    })

    it("calls onChange when value changes", async () => {
        const handleChange = vi.fn()
        const user = userEvent.setup()

        render(<AmountInput value={0} onChange={handleChange} />)

        const input = screen.getByRole("spinbutton")
        await user.clear(input)
        await user.type(input, "50")

        expect(handleChange).toHaveBeenCalled()
    })

    it("shows error message when error prop is provided", () => {
        render(
            <AmountInput
                value={0}
                onChange={vi.fn()}
                error="El monto es muy bajo"
            />
        )

        expect(screen.getByRole("alert")).toHaveTextContent("El monto es muy bajo")
    })

    it("shows hint when hint prop is provided and no error", () => {
        render(
            <AmountInput
                value={10}
                onChange={vi.fn()}
                hint="Minimo 10 EUR para esta recompensa"
            />
        )

        expect(screen.getByText("Minimo 10 EUR para esta recompensa")).toBeInTheDocument()
    })

    it("hides hint when error is present", () => {
        render(
            <AmountInput
                value={0}
                onChange={vi.fn()}
                error="Error"
                hint="This hint should not show"
            />
        )

        expect(screen.queryByText("This hint should not show")).not.toBeInTheDocument()
        expect(screen.getByRole("alert")).toHaveTextContent("Error")
    })

    it("sets aria-invalid when error is present", () => {
        render(
            <AmountInput
                value={0}
                onChange={vi.fn()}
                error="Invalid amount"
            />
        )

        const input = screen.getByRole("spinbutton")
        expect(input).toHaveAttribute("aria-invalid", "true")
    })

    it("has min and max attributes", () => {
        render(
            <AmountInput
                value={10}
                onChange={vi.fn()}
                minAmount={5}
                maxAmount={500}
            />
        )

        const input = screen.getByRole("spinbutton")
        expect(input).toHaveAttribute("min", "5")
        expect(input).toHaveAttribute("max", "500")
    })
})
