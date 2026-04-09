import { describe, it, expect } from "vitest"
import { render, screen } from "@testing-library/react"
import { WizardStepper } from "../../presentation/components/WizardStepper"

describe("WizardStepper", () => {
    it("renders 3 step labels", () => {
        render(<WizardStepper currentStep={1} />)

        expect(screen.getByText("Seleccionar")).toBeInTheDocument()
        expect(screen.getByText("Personalizar")).toBeInTheDocument()
        expect(screen.getByText("Confirmar")).toBeInTheDocument()
    })

    it("renders custom step labels", () => {
        render(
            <WizardStepper
                currentStep={1}
                stepLabels={["Uno", "Dos", "Tres"]}
            />
        )

        expect(screen.getByText("Uno")).toBeInTheDocument()
        expect(screen.getByText("Dos")).toBeInTheDocument()
        expect(screen.getByText("Tres")).toBeInTheDocument()
    })

    it("marks current step with aria-current", () => {
        render(<WizardStepper currentStep={2} />)

        const activeStep = screen.getByText("2").closest("div")
        expect(activeStep).toHaveAttribute("aria-current", "step")
    })

    it("shows step numbers for active and future steps", () => {
        render(<WizardStepper currentStep={1} />)

        expect(screen.getByText("1")).toBeInTheDocument()
        expect(screen.getByText("2")).toBeInTheDocument()
        expect(screen.getByText("3")).toBeInTheDocument()
    })

    it("shows check icon for completed steps", () => {
        render(<WizardStepper currentStep={3} />)

        // Steps 1 and 2 are completed - they should not show numbers
        expect(screen.queryByText("1")).not.toBeInTheDocument()
        expect(screen.queryByText("2")).not.toBeInTheDocument()
        // Step 3 is active - should show number
        expect(screen.getByText("3")).toBeInTheDocument()
    })

    it("has navigation landmark", () => {
        render(<WizardStepper currentStep={1} />)

        expect(
            screen.getByRole("navigation", { name: /progreso/i })
        ).toBeInTheDocument()
    })

    it("does not mark future steps as aria-current", () => {
        render(<WizardStepper currentStep={1} />)

        const step2 = screen.getByText("2").closest("div")
        expect(step2).not.toHaveAttribute("aria-current")

        const step3 = screen.getByText("3").closest("div")
        expect(step3).not.toHaveAttribute("aria-current")
    })
})
